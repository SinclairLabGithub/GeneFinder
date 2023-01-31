using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    //We must search first in the small table before going to the biggest one
    //We must search first in the small table before going to the biggest one
    public class ShortTable
    {
        public IntervalTree tree { get; set; }

        public List<string> name2 { get; set; }//The identifier name2 int the original table
        public List<string> strand { get; set; }//the strand associated to the gene
        public List<int> pStart { get; set; }//the start position of gene "name2" in the original table
        public List<int> pEnd { get; set; }//the end position of gene "name2" in the original table
        public List<string> chromosome { get; set; }//the chomosome associated to gene

        public List<int> find(int a, int b)
        {
            return tree.findOverlap(a, b);
        }

        //Class constructor with the filename of the short table
        public ShortTable(string fileName)
        {
            tree = new IntervalTree();
            List<ItemRange> intervals = new List<ItemRange>();
            this.name2 = new List<string>();
            this.strand = new List<string>();
            this.pStart = new List<int>();
            this.pEnd = new List<int>();
            this.chromosome = new List<string>();

            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            line = file.ReadLine();
            int cont = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 15)
                {
                    string[] words = line.Split("\t,\t".ToCharArray(), StringSplitOptions.None);
                    this.name2.Add(words[3]);
                    this.pStart.Add(Int32.Parse(words[12]));
                    this.pEnd.Add(Int32.Parse(words[15]));
                    this.chromosome.Add(words[21]);
                    this.strand.Add(words[24]);
                    intervals.Add(new ItemRange(Int32.Parse(words[6]), Int32.Parse(words[9]), cont));
                    cont++;
                }
            }
            tree.make(intervals);
        }

        //find all the overlaps for the interval [a,b] in the small table

    }

    //Complete large table
    public class Table
    {
        public List<RowTable> rows { get; set; }
        public Table(string fileName)
        {
            string line;
            rows = new List<RowTable>();
            System.IO.StreamReader file = new System.IO.StreamReader(fileName);
            line = file.ReadLine();
            int cont = 0;
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 20)
                {
                    rows.Add(new RowTable(line));
                    cont++;
                }
            }
        }
    }

    //The complete table is made of rows
    public class RowTable
    {
        public string name { get; set; }
        public string strand { get; set; }
        public int exonCount { get; set; }
        public int intr5UTR { get; set; }
        public int exon5UTR { get; set; }
        public int intr3UTR { get; set; }
        public int exon3UTR { get; set; }
        public bool illDefined { get; set; }
        public List<int> UTR5 { get; set; }
        public List<int> UTR3 { get; set; }
        public List<int> e5UTR { get; set; }//exon overlapping 5UTR
        public List<int> i5UTR { get; set; }  //Introns overlapping 3UTR
        public List<int> e3UTR { get; set; } //Exons overlapping 3UTR
        public List<int> i3UTR { get; set; }  //Introns overlapping 3UTR
        public List<int> exonFrames { get; set; }//Store the exon frame of the transcripts
        public List<ItemRange> exonInt { get; set; }//the intervals of exons
        public List<ItemRange> intronInt { get; set; }//the intervals of introns

        public IntervalTree exons { get; set; }
        public IntervalTree introns { get; set; }

        //if there is at leat one item intersecting an UTR region, then fill it
        public List<int> reshapeIntersection(int count, string words)
        {
            List<int> Array = new List<int>();
            if (count > 0)
            {
                string[] token = words.Split(',');
                for (int i = 0; i < token.Length - 1; i++)
                {
                    Array.Add(Int32.Parse(token[i]));
                }
            }
            return Array;
        }

        public RowTable(string line)
        {
            string[] words = line.Split("\t".ToCharArray(), StringSplitOptions.None);

            name = words[1];
            strand = words[3];
            UTR5 = new List<int>();
            UTR3 = new List<int>();

            UTR5.Add(Int32.Parse(words[4]) + 1);
            UTR5.Add(Int32.Parse(words[6]));

            UTR3.Add(Int32.Parse(words[7]) + 1);
            UTR3.Add(Int32.Parse(words[5]));



            if (words[16].Equals("1"))
                illDefined = true;//we don't have to do anything else
            else
            {
                illDefined = false;//we must do stuff :(
                exonCount = Int32.Parse(words[8]);//How many exons are in the gene

                exon5UTR = Int32.Parse(words[17]);//How many exons overlap the 5UTR region
                intr5UTR = Int32.Parse(words[19]);//How many introns overlap the 5UTR region

                exon3UTR = Int32.Parse(words[21]);//How many exons are in the gene
                intr3UTR = Int32.Parse(words[23]);//How many exons are in the gene


                e5UTR = new List<int>();
                i5UTR = new List<int>();
                e5UTR = reshapeIntersection(exon5UTR, words[18]);
                i5UTR = reshapeIntersection(intr5UTR, words[20]);
                e3UTR = reshapeIntersection(exon3UTR, words[22]);
                i3UTR = reshapeIntersection(intr3UTR, words[24]);
                exonFrames = reshapeIntersection(exonCount, words[15]);

                exons = new IntervalTree();
                introns = new IntervalTree();

                List<ItemRange> exonIntervals = new List<ItemRange>();
                List<ItemRange> intronIntervals = new List<ItemRange>();



                string[] starts = words[9].Split(',');
                string[] ends = words[10].Split(',');

                for (int i = 0; i < exonCount; i++)
                {
                    exonIntervals.Add(new ItemRange(Int32.Parse(starts[i]) + 1, Int32.Parse(ends[i]), i));
                }
                for (int i = 0; i < exonCount - 1; i++)
                {
                    intronIntervals.Add(new ItemRange(Int32.Parse(ends[i]), Int32.Parse(starts[i + 1]), i));
                }
                exons.make(exonIntervals);
                introns.make(intronIntervals);
                exonInt = exonIntervals;
                intronInt = intronIntervals;
            }
        }
    }
}
