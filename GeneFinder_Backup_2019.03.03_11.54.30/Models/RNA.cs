using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class RNA
    {
        public static void MatchRNAtoSmorfs(string rnaFileName, string rnaConditionName, int minMapQual, List<smorf> smorfs)
        {
            using (StreamReader rna = new StreamReader(rnaFileName))
            {
                List<string> chrList = new List<string>();
                List<long> chrLengths = new List<long>();
                int binLength = 600;

                //read headers
                string read = rna.ReadLine();
                while (read != null && read.Contains("@"))
                {
                    string[] fields = read.Split('\t');
                    //@SQ	SN:chr19	LN:59128983
                    if (fields[1].Contains("SN:"))
                    {
                        chrList.Add(fields[1].Substring("SN:".Length));
                        chrLengths.Add(long.Parse(fields[2].Replace("LN:", "")));
                    }
                    read = rna.ReadLine();
                }

                //create genome container, its entries are the smorf position in the gff list +1 (in order to leave 0 value for regions without smorfs)

                //chr  bin  smorfs
                List<List<List<int>>> hash = new List<List<List<int>>>();
                for (int currChr = 0; currChr < chrList.Count; currChr++)
                {
                    long numBins = (chrLengths[currChr] / binLength) + 1;
                    List<List<int>> chrHash = new List<List<int>>();
                    for (long bin = 0; bin < numBins; bin++)
                    {
                        List<int> smorfsOnBin = new List<int>();
                        chrHash.Add(smorfsOnBin);
                    }
                    hash.Add(chrHash);
                }

                //mark hash bins with the smorfs on it
                for (int i = 0; i < smorfs.Count; i++)
                {
                    int currChr = chrList.IndexOf(smorfs[i].Chromosome);
                    int binFirst = smorfs[i].Position / binLength;
                    int binLast = (smorfs[i].Position + smorfs[i].Length - 1) / binLength;

                    for (int bin = binFirst; bin <= binLast; bin++)
                    {
                        if (currChr >= 0 && hash.Count > currChr && hash[currChr].Count > bin)
                        {
                            hash.ElementAt(currChr).ElementAt(bin).Add(i);
                        }

                    }
                }

                do
                {//200321627 position
                    //333869 bin

                    //0     1       2       3           4       5       6       7       8       9           10
                    //id    flag    chr     position    mapQ    cigar   flag    flag    flag    seq         flag
                    //read1	16	    chr1	6649020	    255	    111M	*	    0   	0	    CTACAACCCGCGGCAGCGCAAGCTCCGCAACCTGATCATCGAGGACGAGAAGATGGTGGTGGTGGCGCTGCAGCCGCCTGCAGAGCTGGAGGTGGGCTCGGCGGAGGTCAT	HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH	AS:i:0	XN:i:0	XM:i:0	XO:i:0	XG:i:0	NM:i:0	MD:Z:111	YT:Z:UU
                    string[] readFields = read.Split('\t');

                    //chec map quality
                    if (int.Parse(readFields[4]) < minMapQual)
                    {
                        read = rna.ReadLine();
                        continue;
                    }

                    int currChr = chrList.IndexOf(readFields[2]);
                    int startRNA = int.Parse(readFields[3]);
                    int length = readFields[9].Length;
                    int endRNA = startRNA + length - 1;
                    int binFirst = startRNA / binLength;
                    int binLast = endRNA / binLength;

                    //get all potential smorf hits from the bins
                    HashSet<int> potentialSmorfHits = new HashSet<int>();
                    for (int bin = binFirst; bin <= binLast; bin++)
                    {
                        foreach (int smorfsInBin in hash.ElementAt(currChr).ElementAt(bin))
                        {
                            potentialSmorfHits.Add(smorfsInBin);
                        }
                    }

                    //compare the rna range to the smorf interval
                    foreach (int s in potentialSmorfHits)
                    {
                        int smorfEnd = smorfs[s].Position + smorfs[s].Length - 1;


                        //if rna overlaps smorf
                        if ((startRNA <= smorfs[s].Position && smorfs[s].Position <= endRNA) || (startRNA <= smorfEnd && smorfEnd <= endRNA))
                        {
                            if (smorfs[s].RnaConditionNames == null)
                            {
                                smorfs[s].RnaConditionNames = new List<string>();
                            }
                            if (smorfs[s].RnaCoverage == null)
                            {
                                smorfs[s].RnaCoverage = new List<int[]>();
                            }
                            if (smorfs[s].RnaHits == null)
                            {
                                smorfs[s].RnaHits = new List<int>();
                            }

                            int conditionIndex = smorfs[s].RnaConditionNames.IndexOf(rnaConditionName);
                            if (conditionIndex == -1)
                            {
                                smorfs[s].RnaConditionNames.Add(rnaConditionName);
                                int[] coverage = new int[smorfs[s].Length];
                                for (int i = 0; i < coverage.Length; i++)
                                {
                                    coverage[i] = 0;
                                }
                                smorfs[s].RnaCoverage.Add(coverage);
                                smorfs[s].RnaHits.Add(0);
                                conditionIndex = smorfs[s].RnaConditionNames.IndexOf(rnaConditionName);
                            }

                            smorfs[s].RnaHits[conditionIndex] = smorfs[s].RnaHits[conditionIndex] + 1;

                            for (int i = Math.Max(smorfs[s].Position, startRNA); i < Math.Min(smorfEnd, endRNA); i++)
                            {
                                smorfs[s].RnaCoverage[conditionIndex][i - smorfs[s].Position]++;
                            }
                        }
                    }

                    read = rna.ReadLine();
                } while (read != null);
            }
            
            


            

        }

        private static void MatchRNAtoGFF(string gffFile, string rnaFile, string chromosome, int mapQualTreshold, string outputFile)
        {
            DateTime timeStart = DateTime.Now;
            //load gff
            List<gff> smorfs = new List<gff>();

            StreamReader sr = new StreamReader(gffFile);
            string line = sr.ReadLine();
            do
            {
                string[] fields = line.Split('\t');
                gff smorf = new gff();
                //  0       1                       2   3           4           5   6   7   8  
                // chr2	E|Ribouix_Hsap|in-silico	CDS	152426633	152426797	.	-	-2 "E|Ribouix_Hsap|in-silico"; gene_id "E|HM_1021";
                // chr1	PCGF	CDS	234943365	234943607	.	+	0	gene_id "HE-NC-X|chr1.1|+|234943365|234943607|in-silico"; transcript_id "HE-NC-X|chr1.1|+|234943365|234943607|in-silico";
                smorf.chr = fields[0];
                smorf.start = long.Parse(fields[3]);
                smorf.end = long.Parse(fields[4]);
                smorf.strand = fields[6].ToCharArray()[0];
                smorf.gene_id = fields[8].Substring(fields[8].IndexOf("gene_id")).Split('"')[1];

                smorf.length = smorf.end - smorf.start + 1;
                smorf.coverage = new uint[smorf.length];

                smorfs.Add(smorf);
                line = sr.ReadLine();
            } while (line != null);

            //read rnafile
            StreamReader rna = new StreamReader(rnaFile);

            //skip headers
            string read = rna.ReadLine();
            long chrLength = 0;
            while (read != null && read.Contains("@"))
            {
                string[] fields = read.Split('\t');

                if (fields[1] == "SN:" + chromosome)
                {
                    chrLength = long.Parse(fields[2].Replace("LN:", ""));
                }
                read = rna.ReadLine();
            }


            //create genome container, its entries are the smorf position in the gff list +1 (in order to leave 0 value for regions without smorfs)
            int[] chr = new int[chrLength];
            for (int i = 0; i < smorfs.Count; i++)
            {
                for (long position = smorfs[i].start; position <= smorfs[i].end; position++)
                {
                    chr[position] = i + 1;
                }
            }



            //get read length
            string[] tmpFields = read.Split('\t');
            int readLength = tmpFields[9].Length;

            //0     1       2       3           4       5       6       7       8       9
            //id    flag    chr     position    flag    cigar   flag    flag    flag    seq
            //chr1_1.maf.gz|00-chr1_1-1-00000683-P.fasta	16	chr1	6649020	255	111M	*	0	0	CTACAACCCGCGGCAGCGCAAGCTCCGCAACCTGATCATCGAGGACGAGAAGATGGTGGTGGTGGCGCTGCAGCCGCCTGCAGAGCTGGAGGTGGGCTCGGCGGAGGTCAT	HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH	AS:i:0	XN:i:0	XM:i:0	XO:i:0	XG:i:0	NM:i:0	MD:Z:111	YT:Z:UU
            do
            {
                long position;
                int fieldStart = 0;
                int fieldEnd;
                char strand;
                int mapQual;

                //skip id field
                while (read[fieldStart] != '\t')
                {
                    fieldStart++;
                }
                fieldStart++;

                fieldEnd = fieldStart;
                //get strand flag
                while (read[fieldEnd] != '\t')
                {
                    fieldEnd++;
                }
                if (read.Substring(fieldStart, fieldEnd - fieldStart) == "16")
                {
                    strand = '-';
                }
                else
                {
                    strand = '+';
                }
                fieldEnd++;
                fieldStart = fieldEnd;
                //get chr
                while (read[fieldEnd] != '\t')
                {
                    fieldEnd++;
                }

                //skip unaligned reads
                if (read.Substring(fieldStart, fieldEnd - fieldStart) == "*")
                {
                    read = rna.ReadLine();
                    continue;
                }
                fieldEnd++;
                fieldStart = fieldEnd;
                //get position
                while (read[fieldEnd] != '\t')
                {
                    fieldEnd++;
                }
                position = long.Parse(read.Substring(fieldStart, fieldEnd - fieldStart));
                fieldEnd++;
                fieldStart = fieldEnd;
                //get mapQual
                while (read[fieldEnd] != '\t')
                {
                    fieldEnd++;
                }
                mapQual = int.Parse(read.Substring(fieldStart, fieldEnd - fieldStart));
                if (mapQual < mapQualTreshold)
                {
                    read = rna.ReadLine();
                    continue;
                }


                //Check if read is inside an smorf
                int smorfId = 0;
                for (long p = position; p <= position + readLength; p += 30)
                {
                    smorfId = chr[p];
                    if (smorfId != 0)
                    {
                        smorfId--;
                        if (strand == smorfs[smorfId].strand)
                        {
                            //mark smorf
                            for (long q = Math.Max(smorfs[smorfId].start, position); q <= Math.Min(smorfs[smorfId].end, position + readLength); q++)
                            {
                                smorfs[smorfId].coverage[q - smorfs[smorfId].start]++;
                            }
                            break;
                        }
                    }
                }

                //load read data into smorf
                read = rna.ReadLine();
            } while (read != null);

            //calculate minimum and maximum hits in smorf
            foreach (gff smorf in smorfs)
            {
                foreach (int hit in smorf.coverage)
                {
                    if (hit > smorf.maxHits)
                        smorf.maxHits = hit;
                    if (hit < smorf.minHits || smorf.minHits == 0)
                        smorf.minHits = hit;
                }
            }

            //print smorfs hits
            StreamWriter sw = new StreamWriter(outputFile);
            sw.WriteLine("smorfId,minHits,maxHits");
            foreach (gff smorf in smorfs)
            {
                sw.WriteLine(smorf.gene_id + "," + smorf.minHits + "," + smorf.maxHits);
            }
            sw.Close();
        }

        internal static void MatchRNAtoSmorfs(string[] p, string rnaConditionName, int minMapQual, List<smorf> smorfs)
        {
            List<string> chrList = new List<string>();
            List<long> chrLengths = new List<long>();
            int binLength = 600;
            foreach (string rnaFileName in p)
            {
                using (StreamReader rna = new StreamReader(rnaFileName))
                {
                    //read headers
                    string read = rna.ReadLine();
                    while (read != null && read.Contains("@"))
                    {
                        string[] fields = read.Split('\t');
                        //@SQ	SN:chr19	LN:59128983
                        if (fields[1].Contains("SN:"))
                        {
                            chrList.Add(fields[1].Substring("SN:".Length));
                            chrLengths.Add(long.Parse(fields[2].Replace("LN:", "")));
                        }
                        read = rna.ReadLine();
                    }

                    //create genome container, its entries are the smorf position in the gff list +1 (in order to leave 0 value for regions without smorfs)

                    //chr  bin  smorfs
                    List<List<List<int>>> hash = new List<List<List<int>>>();
                    for (int currChr = 0; currChr < chrList.Count; currChr++)
                    {
                        long numBins = (chrLengths[currChr] / binLength) + 1;
                        List<List<int>> chrHash = new List<List<int>>();
                        for (long bin = 0; bin < numBins; bin++)
                        {
                            List<int> smorfsOnBin = new List<int>();
                            chrHash.Add(smorfsOnBin);
                        }
                        hash.Add(chrHash);
                    }

                    //mark hash bins with the smorfs on it
                    for (int i = 0; i < smorfs.Count; i++)
                    {
                        int currChr = chrList.IndexOf(smorfs[i].Chromosome);
                        int binFirst = smorfs[i].Position / binLength;
                        int binLast = (smorfs[i].Position + smorfs[i].Length - 1) / binLength;

                        for (int bin = binFirst; bin <= binLast; bin++)
                        {
                            if (currChr >= 0 && hash.Count > currChr && hash[currChr].Count > bin)
                            {
                                hash.ElementAt(currChr).ElementAt(bin).Add(i);
                            }

                        }
                    }

                    do
                    {//200321627 position
                        //333869 bin

                        //0     1       2       3           4       5       6       7       8       9           10
                        //id    flag    chr     position    mapQ    cigar   flag    flag    flag    seq         flag
                        //read1	16	    chr1	6649020	    255	    111M	*	    0   	0	    CTACAACCCGCGGCAGCGCAAGCTCCGCAACCTGATCATCGAGGACGAGAAGATGGTGGTGGTGGCGCTGCAGCCGCCTGCAGAGCTGGAGGTGGGCTCGGCGGAGGTCAT	HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH	AS:i:0	XN:i:0	XM:i:0	XO:i:0	XG:i:0	NM:i:0	MD:Z:111	YT:Z:UU
                        string[] readFields = read.Split('\t');

                        //chec map quality
                        if (int.Parse(readFields[4]) < minMapQual)
                        {
                            read = rna.ReadLine();
                            continue;
                        }

                        int currChr = chrList.IndexOf(readFields[2]);
                        int startRNA = int.Parse(readFields[3]);
                        int length = readFields[9].Length;
                        int endRNA = startRNA + length - 1;
                        int binFirst = startRNA / binLength;
                        int binLast = endRNA / binLength;

                        //get all potential smorf hits from the bins
                        HashSet<int> potentialSmorfHits = new HashSet<int>();
                        for (int bin = binFirst; bin <= binLast; bin++)
                        {
                            foreach (int smorfsInBin in hash.ElementAt(currChr).ElementAt(bin))
                            {
                                potentialSmorfHits.Add(smorfsInBin);
                            }
                        }

                        //compare the rna range to the smorf interval
                        foreach (int s in potentialSmorfHits)
                        {
                            int smorfEnd = smorfs[s].Position + smorfs[s].Length - 1;


                            //if rna overlaps smorf
                            if ((startRNA <= smorfs[s].Position && smorfs[s].Position <= endRNA) || (startRNA <= smorfEnd && smorfEnd <= endRNA))
                            {
                                if (smorfs[s].RnaConditionNames == null)
                                {
                                    smorfs[s].RnaConditionNames = new List<string>();
                                }
                                if (smorfs[s].RnaCoverage == null)
                                {
                                    smorfs[s].RnaCoverage = new List<int[]>();
                                }
                                if (smorfs[s].RnaHits == null)
                                {
                                    smorfs[s].RnaHits = new List<int>();
                                }

                                int conditionIndex = smorfs[s].RnaConditionNames.IndexOf(rnaConditionName);
                                if (conditionIndex == -1)
                                {
                                    smorfs[s].RnaConditionNames.Add(rnaConditionName);
                                    int[] coverage = new int[smorfs[s].Length];
                                    for (int i = 0; i < coverage.Length; i++)
                                    {
                                        coverage[i] = 0;
                                    }
                                    smorfs[s].RnaCoverage.Add(coverage);
                                    smorfs[s].RnaHits.Add(0);
                                    conditionIndex = smorfs[s].RnaConditionNames.IndexOf(rnaConditionName);
                                }

                                smorfs[s].RnaHits[conditionIndex] = smorfs[s].RnaHits[conditionIndex] + 1;

                                for (int i = Math.Max(smorfs[s].Position, startRNA); i < Math.Min(smorfEnd, endRNA); i++)
                                {
                                    smorfs[s].RnaCoverage[conditionIndex][i - smorfs[s].Position]++;
                                }
                            }
                        }

                        read = rna.ReadLine();
                    } while (read != null);
                }

            }
        }
    }
}
