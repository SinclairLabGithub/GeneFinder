using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    class functions
    {
        const int maxChromosomeSize = 250000000;

        public static string ReverseComplement(string seq)
        {
            char[] seqInv = seq.ToArray<char>();
            Array.Reverse(seqInv);
            for (int i = 0; i < seqInv.Length; i++)
            {
                if (seqInv[i] == 'A')
                {
                    seqInv[i] = 'T';
                }
                else if (seqInv[i] == 'C')
                {
                    seqInv[i] = 'G';
                }
                else if (seqInv[i] == 'G')
                {
                    seqInv[i] = 'C';
                }
                else if (seqInv[i] == 'T')
                {
                    seqInv[i] = 'A';
                }
            }
            return new string(seqInv);
        }

        public static string CodonToAminoacid(string c)
        {
            switch (c)
            {
                case "TTT": return "F";
                case "TTC": return "F";
                case "TTA": return "L";
                case "TTG": return "L";
                case "CTT": return "L";
                case "CTC": return "L";
                case "CTA": return "L";
                case "CTG": return "L";
                case "ATT": return "I";
                case "ATC": return "I";
                case "ATA": return "I";
                case "ATG": return "M";
                case "GTT": return "V";
                case "GTC": return "V";
                case "GTA": return "V";
                case "GTG": return "V";
                case "TCT": return "S";
                case "TCC": return "S";
                case "TCA": return "S";
                case "TCG": return "S";
                case "CCT": return "P";
                case "CCC": return "P";
                case "CCA": return "P";
                case "CCG": return "P";
                case "ACT": return "T";
                case "ACC": return "T";
                case "ACA": return "T";
                case "ACG": return "T";
                case "GCT": return "A";
                case "GCC": return "A";
                case "GCA": return "A";
                case "GCG": return "A";
                case "TAT": return "Y";
                case "TAC": return "Y";
                case "TAA": return "*";//STOP
                case "TAG": return "*";//STOP
                case "CAT": return "H";
                case "CAC": return "H";
                case "CAA": return "Q";
                case "CAG": return "Q";
                case "AAT": return "N";
                case "AAC": return "N";
                case "AAA": return "K";
                case "AAG": return "K";
                case "GAT": return "D";
                case "GAC": return "D";
                case "GAA": return "E";
                case "GAG": return "E";
                case "TGT": return "C";
                case "TGC": return "C";
                case "TGA": return "*";//STOP
                case "TGG": return "W";
                case "CGT": return "R";
                case "CGC": return "R";
                case "CGA": return "R";
                case "CGG": return "R";
                case "AGT": return "S";
                case "AGC": return "S";
                case "AGA": return "R";
                case "AGG": return "R";
                case "GGT": return "G";
                case "GGC": return "G";
                case "GGA": return "G";
                case "GGG": return "G";

            }
            return "INCORRECT CODON " + c;
        }

        public static string DNAToProtein(string c)
        {
            string aminoacid = "";
            int length = (c.Length / 3);
            for (int i = 0; i < length; i++)
            {
                string codon = "" + c.ElementAt(i * 3 + 0) + c.ElementAt(i * 3 + 1) + c.ElementAt(i * 3 + 2);
                aminoacid += CodonToAminoacid(codon);
            }
            return aminoacid;
        }

        public static char[] LoadFasta(string file)
        {
            string annotation = File.ReadAllText(file);
            annotation = annotation.Substring(annotation.IndexOf("\n"));
            annotation = annotation.Replace("\n", "");
            annotation = annotation.ToUpper();
            return annotation.ToCharArray();
        }

        internal static void CreateConservationFile(string conservationFile, string conservationPath)
        {
            var conservationBed = new CsvReader(new StreamReader(conservationFile));
            int scaleMaxSize = 1;
            Byte[] conservationBuffer = new Byte[scaleMaxSize * maxChromosomeSize];
            //BinaryWriter writer;
            Byte[] tmpConservation;

            //initialize buffer
            for (int i = 0; i < scaleMaxSize * maxChromosomeSize; i++)
            {
                conservationBuffer[i] = 0;
            }

            int currChrLength = 0;
            string chr = "";

            while (conservationBed.Read())
            {
                string[] record = conservationBed.GetField(0).Split('\t');
                // record[0] ~ chromosome name
                // record[2] ~ position
                // record[3] ~ conservation score

                //flush data and create chromosome conservation file
                if (record[0] != chr && chr != "")
                {
                    GC.Collect();

                    //write conservation file
                    using (BinaryWriter writer = new BinaryWriter(File.Open(conservationPath +"\\" + chr + ".txt", FileMode.Create)))
                    {
                        tmpConservation = new Byte[currChrLength];
                        Array.Copy(conservationBuffer, tmpConservation, currChrLength);

                        writer.Write(tmpConservation);
                        writer.Close();
                    }
                    
                    

                    //update current chromosome
                    chr = record[0];

                    //reset conservation buffer
                    for (int i = 0; i < scaleMaxSize * maxChromosomeSize; i++)
                    {
                        conservationBuffer[i] = 0;
                    }
                    //Console.WriteLine("chromosome " + chr);
                    currChrLength = 0;
                }

                if (record.Length<4)
                {
                    continue;
                }
                int def = 0;
                if (record.Length > 1)
                {
                    int.TryParse(record[2], out def);
                }
                
                currChrLength = Math.Max(currChrLength, def);
                chr = record[0];

                //resize array if it excedes current size
                def = 0;
                if (record.Length > 1)
                {
                    int.TryParse(record[2], out def);
                }
                if (def > scaleMaxSize * maxChromosomeSize)
                {
                    scaleMaxSize++;

                    Byte[] tmp = new Byte[scaleMaxSize * maxChromosomeSize];
                    Array.Resize<Byte>(ref conservationBuffer, scaleMaxSize * maxChromosomeSize);
                }

                // project [0,1] float to [0, 255] integer
                float def2 = 1;
                if (record.Length>2)
                {
                    float.TryParse(record[3], out def2);
                }
                conservationBuffer[def] = (Byte)Math.Floor(def2 * 255);

            }

            BinaryWriter writer2 = new BinaryWriter(File.Open(conservationPath + "\\" + chr + ".txt", FileMode.Create));
            tmpConservation = new Byte[currChrLength];
            Array.Copy(conservationBuffer, tmpConservation, currChrLength);

            writer2.Write(tmpConservation);
            writer2.Close();
        }

        public static Byte[] LoadConservationFile(string file)
        {
            Byte[] buffer = new Byte[maxChromosomeSize];
            var F = File.Open(file, FileMode.Open);
            BinaryReader br = new BinaryReader(F);
            buffer = br.ReadBytes((int)F.Length);
            return buffer;
        }

        public static float KozakScore(string seq, List<string> kozak)
        {
            //human         "GCCGCCACCATGGCG";
            // vertebrate GCCGCC(A/G)CCATGG
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

            //return scores.Max() / seq.Length;
            return scores.Max();
        }

        public static void CalculateConservation(List<smorf> smorfs, string pathConservation, int conservationFlanqSize, List<string> chromosomes)
        {
            float threshold = (float)2 / 10;
            foreach (string chr in chromosomes)
            {
                //load conservation for current chr
                Byte[] conservation = LoadConservationFile(pathConservation + "\\" + chr + ".txt");
                foreach (smorf s in smorfs)
                {
                    //skip smorfs not in current chr
                    if (s.Chromosome != chr)
                    {
                        continue;
                    }

                    //if smorf is out of bounds of the chromosome
                    if (s.Position + s.Length > conservation.Length)
                    {
                        continue;
                    }

                    s.ExpandedConservation = new float[s.Length + 2 * conservationFlanqSize];

                    //previous
                    s.ConservationPrevious = 0;
                    for (int i = 0; i < conservationFlanqSize; i++)
                    {
                        s.ExpandedConservation[i] = (float)conservation[s.Position - conservationFlanqSize + i] / 255;// convert from [0,255] integer to [0,1] float
                        s.ConservationPrevious += s.ExpandedConservation[i];
                    }
                    s.ConservationPrevious /= conservationFlanqSize;

                    //sequence
                    s.ConservationAverage = 0;
                    for (int i = conservationFlanqSize; i < s.Length + conservationFlanqSize; i++)
                    {
                        float tmp = conservation[i + s.Position - conservationFlanqSize];
                        float tmp2 = tmp / 255;
                        s.ExpandedConservation[i] = (float)conservation[i + s.Position - conservationFlanqSize] / 255;

                        s.ConservationAverage += s.ExpandedConservation[i];
                    }
                    s.ConservationAverage /= s.Length;

                    //posterior
                    s.ConservationPosterior = 0;
                    for (int i = s.Length + conservationFlanqSize; i < s.Length + 2 * conservationFlanqSize; i++)
                    {
                        s.ExpandedConservation[i] = (float)conservation[i + s.Position - conservationFlanqSize] / 255;
                        s.ConservationPosterior += s.ExpandedConservation[i];
                    }
                    s.ConservationPosterior /= conservationFlanqSize;

                    //conservation flag
                    s.FlagConservation = s.ConservationAverage > threshold && s.ConservationPrevious < s.ConservationAverage / 2 && s.ConservationPosterior < s.ConservationAverage / 2;
                }
            }
        }

        public static void CalculateKozak(List<smorf> smorfs, string pathGenome, List<string> chromosomes, List<string> kozak)
        {
            foreach (string chr in chromosomes)
            {
                char[] chrSequence = functions.LoadFasta(pathGenome + "\\" + chr + ".fa");
                foreach (smorf s in smorfs)
                {
                    //skip smorfs not in current chr
                    if (s.Chromosome != chr)
                    {
                        continue;
                    }

                    //if smorf is out of bounds of the chromosome
                    if (s.Position + s.Length > chrSequence.Length)
                    {
                        continue;
                    }

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
        }

        public static void CalculateAnnotation(List<smorf> lista, string bigTablePath, string smallTablePath)
        {
            Annotate anotation = new Annotate(smallTablePath, bigTablePath);

            foreach (var s in lista)
            {
                anotation.AnnotateSmorf(s);
            }

            //Parallel.ForEach(lista, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, item =>
            //{
            //    anotation.AnnotateSmorf(item);

            //});
        }
    }
}
