using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Realigner
    {
        internal Index index;
        internal Reference reference;

        public List<Alignment> align(string sequence)
        {
            var list = new List<Alignment>();
            for (int offset = 0; offset < index.WindowStep; ++offset)
            {
                var hits = index.FindAll(sequence.Substring(offset, index.WindowSize));
                foreach (var hit in hits)
                {
                    var chromosome = reference[hit.Chromosome];
                    var before = string.Compare(sequence, 0,
                                      chromosome, hit.Position - offset,
                                      offset, StringComparison.InvariantCultureIgnoreCase);
                    var after = string.Compare(sequence, offset + index.WindowSize,
                                     chromosome, hit.Position + index.WindowSize,
                                     sequence.Length - offset - index.WindowSize, StringComparison.InvariantCultureIgnoreCase);
                    if (before == 0 && after == 0)
                        list.Add(new Alignment(hit.Chromosome, hit.Position - offset + 1));
                }
            }
            return list;
        }

        private static string ReverseComplement(string str)
        {
            var chars = str.ToCharArray();
            var strLen = chars.Length;
            var newChars = new char[strLen];
            for (int i = 0; i < strLen; ++i)
            {
                switch (chars[i])
                {
                    case 'a':
                        newChars[strLen - i - 1] = 't';
                        break;
                    case 'A':
                        newChars[strLen - i - 1] = 'T';
                        break;
                    case 'c':
                        newChars[strLen - i - 1] = 'g';
                        break;
                    case 'C':
                        newChars[strLen - i - 1] = 'G';
                        break;
                    case 'g':
                        newChars[strLen - i - 1] = 'c';
                        break;
                    case 'G':
                        newChars[strLen - i - 1] = 'C';
                        break;
                    case 't':
                        newChars[strLen - i - 1] = 'a';
                        break;
                    case 'T':
                        newChars[strLen - i - 1] = 'A';
                        break;
                    default:
                        newChars[strLen - i - 1] = chars[i];
                        break;
                }
            }
            return new string(newChars);
        }

        public void Process(string inputFilename, string outputFilename)
        {
            if (File.Exists(inputFilename))
            {
                using (StreamReader inputStream = new StreamReader(inputFilename))
                using (StreamWriter outputStream = new StreamWriter(outputFilename))
                {
                    string line;
                    while ((line = inputStream.ReadLine()) != null)
                    {
                        var fields = line.Split('\t');
                        var sequence = fields[1];
                        // Align positive strand
                        var posResults = align(sequence);
                        if (posResults.Count > 0)
                        {
                            // Output first (or unique) result
                            outputStream.Write("{0}\t{1}\t{2}\t+\t1|{3}", fields[0], reference.Names[posResults[0].Chromosome], posResults[0].Position, posResults.Count);
                            for (int iField = 1; iField < fields.Length; ++iField)
                                outputStream.Write("\t{0}", fields[iField]);
                            outputStream.WriteLine();
                            if (posResults.Count > 1)
                            {
                                for (int iResult = 1; iResult < posResults.Count; ++iResult)
                                    outputStream.WriteLine("{0}\t{1}\t{2}\t+\t{3}|{4}", fields[0], reference.Names[posResults[iResult].Chromosome], posResults[iResult].Position, iResult + 1, posResults.Count);
                            }
                        }
                        var negResults = align(ReverseComplement(sequence));
                        if (negResults.Count > 0)
                        {
                            // Output first (or unique) result
                            outputStream.Write("{0}\t{1}\t{2}\t-\t1|{3}", fields[0], reference.Names[negResults[0].Chromosome], negResults[0].Position, negResults.Count);
                            for (int iField = 1; iField < fields.Length; ++iField)
                                outputStream.Write("\t{0}", fields[iField]);
                            outputStream.WriteLine();
                            if (posResults.Count > 1)
                            {
                                for (int iResult = 1; iResult < negResults.Count; ++iResult)
                                    outputStream.WriteLine("{0}\t{1}\t{2}\t-\t{3}|{4}", fields[0], reference.Names[negResults[iResult].Chromosome], negResults[iResult].Position, iResult + 1, negResults.Count);
                            }
                        }
                    }
                }
            }
        }

        public Realigner(string fastaListFilename, string indexFilename)
        {
            if (File.Exists(fastaListFilename) && File.Exists(indexFilename))
            {
                index = new Index(indexFilename);
                reference = new Reference(fastaListFilename);
            }
            else
            {
                index = null;
                reference = null;
                Console.WriteLine("Realigner not initialized");
            }
        }

        internal void Process(List<smorf> smorfsList)
        {
            Parallel.For(0, smorfsList.Count,
                iThread =>
                {
                    var sequence = smorfsList[iThread].Sequence;
                    var posResults = align(sequence);
                    if (smorfsList[iThread].Coord == null) smorfsList[iThread].Coord = new List<Coordinates>();
                    foreach (var item in posResults)
                    {
                        smorfsList[iThread].Coord.Add(new Coordinates() { Chromosome = reference.Names[0], Position = item.Position, Strand = "+" });
                        //smorfsList[iThread].Chromosome = reference.Names[0];
                        //smorfsList[iThread].Position = item.Position;
                        //smorfsList[iThread].Strand = "+";
                    }
                    var negResults = align(ReverseComplement(sequence));
                    foreach (var item in negResults)
                    {
                        smorfsList[iThread].Coord.Add(new Coordinates() { Chromosome = reference.Names[0], Position = item.Position, Strand = "-" });
                        //smorfsList[iThread].Chromosome = reference.Names[0];
                        //smorfsList[iThread].Position = item.Position;
                        //smorfsList[iThread].Strand = "-";
                    }
                }
            );
        }
    }
}
