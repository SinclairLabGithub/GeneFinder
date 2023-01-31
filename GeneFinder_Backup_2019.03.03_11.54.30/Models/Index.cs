using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Diagnostics;

namespace GeneFinder.Models
{
    public struct Alignment
    {
        public byte Chromosome { get; set; }
        public int Position { get; set; }

        public Alignment(byte chromosome, int position) : this ()
        {
            this.Chromosome = chromosome;
            this.Position = position;
        }
    }

    public class Index
    {
        private struct LookupEntry : IComparable
        {
            public Alignment Entry { get; set; }
            public string Key { get; set; }

            public LookupEntry(string key, byte chromosome, int position) : this ()
            {
                this.Entry = new Alignment(chromosome, position);
                this.Key = key;
            }

            public int CompareTo(object obj)
            {
                LookupEntry lookupEntry = (LookupEntry)obj;
                return string.Compare(this.Key, lookupEntry.Key, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        public int WindowSize { get; set; }
        public int WindowStep { get; set; }
        ILookup<string, Alignment> lookup;

        public Index(string fastaFilename, int windowSize)
        {
            LoadFromFasta(fastaFilename, windowSize);
        }   

        public Index(string indexFilename)
        {
            LoadFromBinary(indexFilename);
        }

        public Index(ParametersClass parametros)
        {
            LoadFromFasta(parametros);
        }

        private void LoadFromFasta(ParametersClass parametros)
        {
            // Load file and estimate number of dictionary entries
            int maxEntries;
            string completeChromosome;
            WindowSize = parametros.indexWindowSize;
            WindowStep = 31 - WindowSize;

            string fastaFilename;
            byte iChr = 0;
            var entries = new List<LookupEntry>();
            foreach (string name in parametros.genomeFiles)
            {
                fastaFilename = parametros.genomeFolder + "\\" + name + ".fa";
                if (File.Exists(fastaFilename))
                {
                    using (var stream = new StreamReader(fastaFilename))
                    {
                        stream.ReadLine();
                        completeChromosome = Regex.Replace(stream.ReadToEnd(), @"(\r|\r\n|\n)", String.Empty);
                        maxEntries = (completeChromosome.Length - WindowSize) / WindowStep;
                        //Console.WriteLine("{0}:{1}", completeChromosome.Length, maxEntries);
                    }

                    // Fill list of entries
                    string window;
                    int position = 0;
                    int keysIndex = 0;
                    for (uint iEntry = 0; iEntry < maxEntries; ++iEntry, position += WindowStep)
                    {
                        window = completeChromosome.Substring(position, WindowSize);
                        if (window.IndexOf("N", StringComparison.InvariantCultureIgnoreCase) == -1)
                        {
                            entries.Add(new LookupEntry(window, iChr, position));
                            keysIndex += WindowSize;
                        }
                    }
                    iChr++;
                }
                else
                {
                    //Console.WriteLine("File {0} not found", fastaFilename);
                }
            }
            entries.Sort();
            this.lookup = entries.ToLookup(keyedEntry => keyedEntry.Key, keyedEntry => keyedEntry.Entry, StringComparer.InvariantCultureIgnoreCase);
            
        }

        private void LoadFromFasta(string fastaListFilename, int windowSize)
        {
            // Load file and estimate number of dictionary entries
            int maxEntries;
            string completeChromosome;
            WindowSize = windowSize;
            WindowStep = 31 - WindowSize;
            if (File.Exists(fastaListFilename))
            {
                using (var listStream = new StreamReader(fastaListFilename))
                {
                    string fastaFilename;
                    byte iChr = 0;
                    var entries = new List<LookupEntry>();
                    while ((fastaFilename = listStream.ReadLine()) != null)
                    {
                        if (File.Exists(fastaFilename))
                        {
                            using (var stream = new StreamReader(fastaFilename))
                            {
                                stream.ReadLine();
                                completeChromosome = Regex.Replace(stream.ReadToEnd(), @"(\r|\r\n|\n)", String.Empty);
                                maxEntries = (completeChromosome.Length - WindowSize) / WindowStep;
                                Console.WriteLine("{0}:{1}", completeChromosome.Length, maxEntries);
                            }

                            // Fill list of entries
                            string window;
                            int position = 0;
                            int keysIndex = 0;
                            for (uint iEntry = 0; iEntry < maxEntries; ++iEntry, position += WindowStep)
                            {
                                window = completeChromosome.Substring(position, WindowSize);
                                if (window.IndexOf("N", StringComparison.InvariantCultureIgnoreCase) == -1)
                                {
                                    entries.Add(new LookupEntry(window, iChr, position));
                                    keysIndex += WindowSize;
                                }
                            }
                            iChr++;
                        }
                        else
                        {
                            Console.WriteLine("File {0} not found", fastaFilename);
                        }
                        entries.Sort();
                        this.lookup = entries.ToLookup(keyedEntry => keyedEntry.Key, keyedEntry => keyedEntry.Entry, StringComparer.InvariantCultureIgnoreCase);
                    }
                }
            }
        }

        private void LoadFromBinary(string filename)
        {
            if (File.Exists(filename))
            {
                using (var reader = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    WindowSize = reader.ReadInt32();
                    WindowStep = 31 - WindowSize;
                    int count = reader.ReadInt32();
                    var entries = new List<LookupEntry>(count);
                    string key;
                    int listCount;
                    byte chromosome;
                    int position;
                    for (int iEntry = 0; iEntry < count; ++iEntry)
                    {
                        key = reader.ReadString();
                        listCount = reader.ReadInt32();
                        for (int iList = 0; iList < listCount; ++iList)
                        {
                            chromosome = reader.ReadByte();
                            position = reader.ReadInt32();
                            entries.Add(new LookupEntry(key, chromosome, position));
                        }
                    }
                    this.lookup = entries.ToLookup(keyedEntry => keyedEntry.Key, keyedEntry => keyedEntry.Entry, StringComparer.InvariantCultureIgnoreCase);
                }
            }
        }

        public void Save(string filename)
        {
            using (var writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                writer.Write(WindowSize);
                writer.Write(lookup.Count);
                foreach (var entry in lookup)
                {
                    writer.Write(entry.Key);
                    writer.Write(entry.Count());
                    foreach (var listEntry in entry)
                    {
                        writer.Write(listEntry.Chromosome);
                        writer.Write(listEntry.Position);
                    }
                }
            }
        }

        public IEnumerable<Alignment> FindAll(string key)
        {
            return this.lookup[key];
        }
    }
}
