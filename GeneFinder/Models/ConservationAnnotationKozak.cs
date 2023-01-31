using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;

namespace GeneFinder.Models
{
    public class ConservationAnnotationKozak
    {
        const int maxChromosomeSize = 250000000;
        const double conservationThreshold = 0.5;
        /*                  
   List<string> kozak = {"GCCGCCACCATGGCG",
                           "GCCGCCGCCATGGCG",
                           "GCCGCCAACATGGCG",
                           "GCCGCCGACATGGCG",
                           };
         */
        //human         "GCCGCCACCATGGCG";
        // vertebrate GCCGCC(A/G)CCATGG
        public static float     KozakScore(string seq, List<string> kozak)
        {
            float[] scores = new float[kozak.Count];

            for (int i = 0; i < scores.Length; i++)
            {
                scores[i] = 0;
            }

            if (kozak[0].Length != seq.Length)
            {
                return -1;
            }

            for (int s = 0; s < seq.Length; s++)
            {
                for (int i = 0; i < kozak.Count; i++)
                {
                    if (kozak[i].ElementAt(s) == seq.ElementAt(s))
                    {
                        scores[i]++;
                    }
                }
            }

            return scores.Max() / seq.Length;
        }
        // intergenic - '0', exon - 'e', intron - 'i'
        internal static void CreateAnnotationFile(string chromosome, string strand, string exonFile, string utr3File, string utr5File, string outputPath)
        {
            CsvReader genesFile = new CsvReader(new StreamReader(exonFile));
            CsvReader utr3 = new CsvReader(new StreamReader(utr3File));
            CsvReader utr5 = new CsvReader(new StreamReader(utr5File));

            string[] headersExons;
            char[] annotation = new char[maxChromosomeSize];

            //initialize annotation
            for (int i = 0; i < maxChromosomeSize; i++)
            {
                annotation[i] = '0';
            }



            //read headers
            genesFile.Read();
            headersExons = genesFile.Context.HeaderRecord; //.FieldHeaders;
            do
            {
                //read gene
                if (genesFile.GetField("chrom") == "chr" + chromosome && genesFile.GetField("strand") == strand)
                {
                    int numExons = int.Parse(genesFile.GetField("exonCount"));
                    //annotate gene as intron, then overlap exons
                    int startGene = int.Parse(genesFile.GetField("txStart"));
                    int endGene = int.Parse(genesFile.GetField("txEnd"));

                    //skip if region is already marked as intron
                    //if region is marked as intergenic, mark it as intron
                    if (annotation[endGene] == '0')
                    {
                        for (int i = startGene; i <= endGene; i++)
                        {
                            if (i < maxChromosomeSize)
                            {
                                if (annotation[i] == '0')
                                {
                                    annotation[i] = 'i';
                                }
                            }
                            else
                            {
                                Console.Out.Write("Gene above maxChromosomeSize bound, end: " + genesFile.GetField("txEnd"));
                            }
                        }
                    }

                    //annotate exon
                    String[] startExons = genesFile.GetField("exonStarts").Split(',');
                    String[] endExons = genesFile.GetField("exonEnds").Split(',');
                    for (int i = 0; i < numExons; i++)
                    {
                        //skip if exon is already marked
                        int startExon = int.Parse(startExons[i]);
                        int endExon = int.Parse(endExons[i]);
                        if (annotation[startExon] == 'e' && annotation[endExon] == 'e' && annotation[(int)(startExon + endExon) / 2] == 'e')
                        {
                            continue;
                        }
                        for (int j = int.Parse(startExons[i]); j <= int.Parse(endExons[i]); j++)
                        {
                            if (j < maxChromosomeSize)
                            {
                                annotation[j] = 'e';
                            }
                            else
                            {
                                Console.Out.Write("Gene above maxChromosomeSize bound, end: " + endExons[i]);
                            }
                        }
                    }

                }
            } while (genesFile.Read());

            string[] headers;
            utr3.Read();
            headers = utr3.Context.HeaderRecord; //FieldHeaders;
            do
            {
                if (utr3.GetField("chr") == chromosome && utr3.GetField("strand") == strand)
                {
                    for (int i = int.Parse(utr3.GetField("start")); i <= int.Parse(utr3.GetField("end")); i++)
                    {
                        annotation[i] = '3';
                    }
                }
            } while (utr3.Read());

            utr5.Read();
            headers = utr5.Context.HeaderRecord; //FieldHeaders;
            {
                if (utr5.GetField("chr") == chromosome && utr5.GetField("strand") == strand)
                {
                    for (int i = int.Parse(utr5.GetField("start")); i <= int.Parse(utr5.GetField("end")); i++)
                    {
                        annotation[i] = '5';
                    }
                }
            } while (utr5.Read()) ;

            string sAnnotation = new string(annotation);
            File.WriteAllText(outputPath + chromosome + strand + ".txt", sAnnotation);
        }

        public static char[] LoadAnnotationFile(string file)
        {
            char[] buffer = new char[maxChromosomeSize];
            StreamReader sr = new StreamReader(file);
            sr.Read(buffer, 0, maxChromosomeSize);
            sr.Close();
            return buffer;
        }

        internal static void CreateConservationFile(string conservationFile, string conservationPath)
        {
            var conservationBed = new CsvReader(new StreamReader(conservationFile));
            char[] conservation = new char[maxChromosomeSize];//248956422];

            //initialize annotation
            for (int i = 0; i < maxChromosomeSize; i++)
            {
                conservation[i] = '0';
            }

            //read headers            
            int dummy = 0;
            string chr = "chrY";
            //            chr = "";
            while (conservationBed.Read())
            {
                dummy++;
                if (dummy % 10000000 == 0)
                {
                    Console.WriteLine(conservationBed.GetField(0) + " " + dummy);
                }
                string[] record = conservationBed.GetField(0).Split('\t');

                if (int.Parse(record[1]) % 10000000 == 0)
                {
                    Console.WriteLine(++dummy);
                }

                //flush data
                if (record[0] != chr)
                {
                    //                    continue;
                    GC.Collect();

                    StreamWriter tmpSW = new StreamWriter(conservationPath + chr + ".txt");
                    /*
                    for (int i = 0; i < maxChromosomeSize; i++)
                    {
                        tmpSW.Write( conservation[i] );
                    }
                     */
                    tmpSW.Write(conservation);
                    tmpSW.Close();

                    //string sConservation = new string(conservation);
                    //File.WriteAllText(conservationPath + chr + ".txt", sConservation);

                    chr = record[0];

                    for (int i = 0; i < maxChromosomeSize; i++)
                    {
                        conservation[i] = '0';
                    }
                    Console.WriteLine("chromosome " + chr);
                }
                if (float.Parse(record[3]) < conservationThreshold)
                {
                    conservation[int.Parse(record[2])] = '0';
                }
                else
                {
                    conservation[int.Parse(record[2])] = '1';
                }

            }
            string sConservationY = new string(conservation);
            File.WriteAllText("conservation/" + chr + ".txt", sConservationY);

        }

        public static char[] LoadConservationFile(string file)
        {
            char[] buffer = new char[maxChromosomeSize];
            StreamReader sr = new StreamReader(file);
            sr.Read(buffer, 0, maxChromosomeSize);
            sr.Close();
            return buffer;
        }

        // E - exon, I - intron, X - intergenic, 3U - utr3, 5U - utr5, A|<other>|<other2>|... - ambiguous        

        public static void GetConservationAnnotationKozak(List<smorf> smorfs, string pathConservation, int conservationFlanqSize, string pathAnnotation, string pathGenome, List<string> chromosomes, List<string> kozak)
        {
            HashSet<string> activeChromosomes = new HashSet<string>();
            foreach (smorf s in smorfs)
            {
                activeChromosomes.Add(s.Chromosome);
            }
            float threshold = (float)2 / 10;

            foreach (string chr in chromosomes)
            {
                if (!activeChromosomes.Contains(chr))
                {
                    continue;
                }
                GC.Collect();
                char[] conservation = LoadConservationFile(pathConservation + chr + ".txt");
                char[] annotationPositive = LoadAnnotationFile(pathAnnotation + chr + "+.txt");
                char[] annotationNegative = LoadAnnotationFile(pathAnnotation + chr + "-.txt");
                char[] chrSequence = functions.LoadFasta(pathGenome + chr + ".fa");

                foreach (smorf s in smorfs)
                {
                    if (s.Chromosome != chr)
                    {
                        continue;
                    }
                    //conservation
                    s.ExpandedConservation = new float[s.Length + 2 * conservationFlanqSize];

                    //previous
                    s.ConservationPrevious = 0;
                    for (int i = 0; i < conservationFlanqSize; i++)
                    {
                        s.ExpandedConservation[i] = conservation[s.Position - conservationFlanqSize + i] - '0';
                        s.ConservationPrevious += s.ExpandedConservation[i];
                    }
                    s.ConservationPrevious /= conservationFlanqSize;

                    //sequence
                    s.ConservationAverage = 0;
                    for (int i = conservationFlanqSize; i < s.Length + conservationFlanqSize; i++)
                    {
                        s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                        s.ConservationAverage += s.ExpandedConservation[i];
                    }
                    s.ConservationAverage /= s.Length;

                    //posterior
                    s.ConservationPosterior = 0;
                    for (int i = s.Length + conservationFlanqSize; i < s.Length + 2 * conservationFlanqSize; i++)
                    {
                        s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                        s.ConservationPosterior += s.ExpandedConservation[i];
                    }
                    s.ConservationPosterior /= conservationFlanqSize;

                    //conservation flag
                    s.FlagConservation = s.ConservationAverage > threshold && s.ConservationPrevious < s.ConservationAverage / 2 && s.ConservationPosterior < s.ConservationAverage / 2;

                    //annotation
                    s.ExpandedAnotation = "";
                    s.ExpandedAnotationReverse = "";

                    string tmpPreviousAnnotation = "";
                    string tmpPosteriorAnnotation = "";
                    string tmpPreviousAnnotationReverse = "";
                    string tmpPosteriorAnnotationReverse = "";

                    if (s.Strand == "+")
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            s.ExpandedAnotation += annotationPositive[i + s.Position];
                            s.ExpandedAnotationReverse += annotationNegative[i + s.Position];
                        }
                        for (int i = -conservationFlanqSize; i < 0; i++)
                        {
                            tmpPreviousAnnotation += annotationPositive[i + s.Position];
                            tmpPreviousAnnotationReverse += annotationNegative[i + s.Position];
                        }
                        for (int i = 0; i < conservationFlanqSize; i++)
                        {
                            tmpPosteriorAnnotation += annotationPositive[i + s.Position + s.Length];
                            tmpPosteriorAnnotationReverse += annotationNegative[i + s.Position + s.Length];
                        }
                    }
                    if (s.Strand == "-")
                    {
                        for (int i = 0; i < s.Length; i++)
                        {
                            s.ExpandedAnotation += annotationNegative[i + s.Position];
                            s.ExpandedAnotationReverse += annotationPositive[i + s.Position];
                        }
                        for (int i = -conservationFlanqSize; i < 0; i++)
                        {
                            tmpPreviousAnnotation += annotationNegative[i + s.Position];
                            tmpPreviousAnnotationReverse += annotationPositive[i + s.Position];
                        }
                        for (int i = 0; i < conservationFlanqSize; i++)
                        {
                            tmpPosteriorAnnotation += annotationNegative[i + s.Position + s.Length];
                            tmpPosteriorAnnotationReverse += annotationPositive[i + s.Position + s.Length];
                        }
                    }

                    foreach (char c in s.ExpandedAnotation)
                    {
                        switch (c)
                        {
                            default:
                                break;
                            case '0':
                                s.OverlapIntergenic++;
                                break;
                            case 'e':
                                s.OverlapExon++;
                                break;
                            case 'i':
                                s.OverlapIntron++;
                                break;
                            case '3':
                                s.OverlapUtr3++;
                                break;
                            case '5':
                                s.OverlapUtr5++;
                                break;
                        }
                    }

                    foreach (char c in s.ExpandedAnotationReverse)
                    {
                        if (c == 'e' || c == '3' || c == '5')
                        {
                            s.OverlapExonReverse++;
                        }
                    }
                    s.OverlapIntergenic /= s.Length;
                    s.OverlapExon /= s.Length;
                    s.OverlapIntron /= s.Length;
                    s.OverlapUtr3 /= s.Length;
                    s.OverlapUtr5 /= s.Length;
                    s.OverlapExonReverse /= s.Length;

                    s.ExpandedAnotation = tmpPreviousAnnotation + s.ExpandedAnotation + tmpPosteriorAnnotation;
                    s.ExpandedAnotationReverse = tmpPreviousAnnotationReverse + s.ExpandedAnotationReverse + tmpPosteriorAnnotationReverse;

                    s.FlagOverlapExonEitherStrand = (s.OverlapExonReverse == 0) && (s.OverlapExon == 0);

                    //ambiguous annotation
                    string smorfAnnot = "A";
                    if (s.OverlapExon != 0)
                    {
                        smorfAnnot += "-E";
                    }
                    if (s.OverlapIntron != 0)
                    {
                        smorfAnnot += "-I";
                    }
                    if (s.OverlapIntergenic != 0)
                    {
                        smorfAnnot += "-X";
                    }
                    if (s.OverlapUtr3 != 0)
                    {
                        smorfAnnot += "-3U";
                    }
                    if (s.OverlapUtr5 != 0)
                    {
                        smorfAnnot += "-5U";
                    }

                    //non ambiguous annotation
                    if (s.OverlapExon == 1)
                    {
                        smorfAnnot = "E";
                    }
                    if (s.OverlapIntron == 1)
                    {
                        smorfAnnot = "I";
                    }
                    if (s.OverlapIntergenic == 1)
                    {
                        smorfAnnot = "X";
                    }
                    if (s.OverlapUtr3 == 1)
                    {
                        smorfAnnot = "3U";
                    }
                    if (s.OverlapUtr5 == 1)
                    {
                        smorfAnnot = "5U";
                    }
                    s.Annotation = smorfAnnot;

                    //kozak
                    string smorfKozak = "";
                    if (s.Strand == "+")
                    {
                        smorfKozak = new string(chrSequence, (s.Position - 9 - 1), (9 + 6));
                    }
                    if (s.Strand == "-")
                    {
                        //smorfKozak = new string(fasta, (start - 9 - 1), (9 + 6));
                        smorfKozak = new string(chrSequence, (s.Position + s.Length - 6 - 1), (9 + 6));
                        smorfKozak = functions.ReverseComplement(smorfKozak);
                    }
                    s.KozakSequence = smorfKozak;
                    s.KozakScore = KozakScore(smorfKozak, kozak);

                }
            }

            /*
            AddConservationScore(smorfPath + phyloFile, smorfPath + consFile, conservationPath, conservationFlanqSize, treshold);
            GC.Collect();
            AddAnnotation(smorfPath + consFile, smorfPath + annotFile, annotationBinaryPath, genomePath);
            GC.Collect();
            AddProtein(smorfPath + annotFile, smorfPath + protFile);
            SmorfsToGFF(smorfPath + protFile, smorfPath + "gff/");
/**/

        }

    }
}
