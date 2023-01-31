using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class ReadTSV
    {
        public List<TSVRow> rows { get; set; }
        public List<string> linesLongFile { get; set; }
        public List<string> linesshortFile { get; set; }
        public ReadTSV(string rute, string inputFileName, string newLongtableName, string newCompressedTableName)
        {
            string line, longHeader, shortHeader;
            rute += "\\";
            rows = new List<TSVRow>();
            linesLongFile = new List<string>();
            linesshortFile = new List<string>();

            System.IO.StreamReader file = new System.IO.StreamReader(rute + inputFileName);
            longHeader = file.ReadLine() + "\tillDefined\t#exon5UTR\texons\t#intron5UTR\tintrons\t#exon3UTR\texons\t#intron3UTR\tintron";
            shortHeader = "Id\t,\tname2\t,\tminPos\t,\tmaxPos\t,\tstartRow\t,\tendRow\t,\t#items\t,\tchromosome";
            linesLongFile.Add(longHeader);
            linesshortFile.Add(shortHeader);
            while ((line = file.ReadLine()) != null)
            {
                if (line.Length > 15)
                {
                    TSVRow rowtemp = new TSVRow();
                    linesLongFile.Add(rowtemp.expandLine(line));
                    rows.Add(rowtemp);
                }
            }

            string strand = rows[0].strand;
            string chromosome = rows[0].chromosome;
            line = rows[0].name2;
            int min = rows[0].minValue;
            int max = rows[0].maxValue;
            int tempStart = 0, val = 0, id = 0;
            int i;
            for (i = 0; i < rows.Count; i++)
            {
                if (line.Equals(rows[i].name2) == false)
                {
                    int aux = i - 1;
                    int aux2 = i - tempStart;
                    string temp = id.ToString() + "\t,\t" + line + "\t,\t" + min.ToString() + "\t,\t" + max.ToString() + "\t,\t" + tempStart.ToString() + "\t,\t" + aux.ToString() + "\t,\t" + aux2.ToString() + "\t,\t" + chromosome + "\t,\t" + strand;
                    linesshortFile.Add(temp);
                    tempStart = i;
                    line = rows[i].name2;
                    chromosome = rows[i].chromosome;
                    strand = rows[i].strand;
                    min = rows[i].minValue;
                    max = rows[i].maxValue;
                    id++;
                    val += i - tempStart;
                }

                if (rows[i].minValue < min)
                    min = rows[i].minValue;
                if (rows[i].maxValue > max)
                    max = rows[i].maxValue;
            }

            int auxiliar = i - 1;
            int auxiliar2 = i - tempStart;
            string temporal = id.ToString() + "\t,\t" + line + "\t,\t" + min.ToString() + "\t,\t" + max.ToString() + "\t,\t" + tempStart.ToString() + "\t,\t" + auxiliar.ToString() + "\t,\t" + auxiliar2.ToString() + "\t,\t" + chromosome + "\t,\t" + strand;
            linesshortFile.Add(temporal);

            string outputName = rute + newLongtableName;
            using (StreamWriter writetext = new StreamWriter(outputName))
            {
                foreach (string l in linesLongFile)
                    writetext.WriteLine(l);
            }

            outputName = rute + newCompressedTableName;
            using (StreamWriter writetext = new StreamWriter(outputName))
            {
                foreach (string l in linesshortFile)
                    writetext.WriteLine(l);
            }

        }
    }

    public class TSVRow
    {
        public string name { get; set; }
        public string name2 { get; set; }
        public int isIll { get; set; }
        public int txStart { get; set; }
        public int txEnd { get; set; }
        public int cdsStart { get; set; }
        public int cdsEnd { get; set; }
        public int exonCount { get; set; }
        public int minValue { get; set; }
        public int maxValue { get; set; }
        public string chromosome { get; set; }
        public string strand { get; set; }

        private string intervalCases(List<int> Positions)
        {
            string output = "";

            if (Positions.Count == 0)
                output += "-1\t,";
            else
            {
                output += Positions.Count.ToString() + "\t";
                foreach (int id in Positions)
                {
                    output += id.ToString() + ",";
                }
            }

            return output + "\t";
        }

        public string expandLine(string Input)
        {
            string output = Input;
            isIll = 0;
            string[] ret = Input.Split("\t".ToCharArray(), StringSplitOptions.None);

            name = ret[1];
            name2 = ret[12];
            chromosome = ret[2];
            strand = ret[3];
            txStart = Int32.Parse(ret[4]);
            txEnd = Int32.Parse(ret[5]);
            cdsStart = Int32.Parse(ret[6]);
            cdsEnd = Int32.Parse(ret[7]);
            exonCount = Int32.Parse(ret[8]);
            minValue = txStart;
            maxValue = txEnd;

            if (txEnd == cdsStart && cdsStart == cdsEnd)
                isIll = 1;
            output += "\t" + isIll.ToString() + "\t";
            if (isIll == 1)
                output += "-1\t,\t-1\t,\t-1\t,\t-1\t,";
            else
            {
                List<ItemRange> exon = new List<ItemRange>();
                List<ItemRange> intron = new List<ItemRange>();

                string[] starts = ret[9].Split(',');
                string[] ends = ret[10].Split(',');

                for (int id = 0; id < exonCount; id++)
                {
                    exon.Add(new ItemRange(Int32.Parse(starts[id]), Int32.Parse(ends[id]), id));
                }
                for (int id = 0; id < exonCount - 1; id++)
                {
                    intron.Add(new ItemRange(Int32.Parse(ends[id]), Int32.Parse(starts[id + 1]), id));
                }

                IntervalTree exonTree = new IntervalTree();
                IntervalTree intronTree = new IntervalTree();
                exonTree.make(exon);
                intronTree.make(intron);

                List<int> eUTR5 = exonTree.findOverlap(txStart, cdsStart);
                List<int> iUTR5 = intronTree.findOverlap(txStart, cdsStart);

                List<int> eUTR3 = exonTree.findOverlap(cdsEnd, txEnd);
                List<int> iUTR3 = intronTree.findOverlap(cdsEnd, txEnd);

                eUTR5.Sort();
                iUTR5.Sort();

                eUTR3.Sort();
                iUTR3.Sort();

                output += intervalCases(eUTR5);
                output += intervalCases(iUTR5);

                output += intervalCases(eUTR3);
                output += intervalCases(iUTR3);

                for (int k = 0; k < exonCount; k++)
                {
                    if (minValue > exon[k].Range.From)
                        minValue = exon[k].Range.From;
                    if (maxValue < exon[k].Range.To)
                        maxValue = exon[k].Range.To;
                }

            }

            return output;
        }
    }
}
