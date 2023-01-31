using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Annotate
    {
        public ShortTable compressedTable { get; set; }

        public Table table { get; set; }

        public Annotate(string shortTableName, string tableName)
        {
            compressedTable = new ShortTable(shortTableName);//Load small table
            table = new Table(tableName);//load complete table
        }



        public void AnnotateSmorf(smorf smorfToBeAnnotated)
        {
            //find the smorf in the compressed table
            List<int> selectGenes = compressedTable.find(smorfToBeAnnotated.Position, smorfToBeAnnotated.EndPosition);

            // if the number of selectGenes is 0, then the smorf is not founded in the table, 
            // and the smorf is not in the table
            if (selectGenes.Count == 0)
                smorfToBeAnnotated.isInTheTable = false;
            else
            {
                // The smorf was found in the table
                smorfToBeAnnotated.isInTheTable = true;
                smorfToBeAnnotated.expandedResultTable = new List<List<string>>();
                smorfToBeAnnotated.annotationResult = new List<List<List<string>>>();
                smorfToBeAnnotated.rowRanges = new List<int>();
                int i = 0, tableIdx = 0;
                string sameStrand, name2;
                foreach (int id in selectGenes)
                {
                    string temporal = compressedTable.chromosome[id];
                    if (compressedTable.chromosome[id].CompareTo(smorfToBeAnnotated.Chromosome) == 0)
                    {
                        smorfToBeAnnotated.annotationResult.Add(new List<List<string>>());
                        smorfToBeAnnotated.rowRanges.Add(id);
                        //the generic name (name 2 in the tables)
                        name2 = compressedTable.name2[id];

                        //check if the have the same strand
                        if (compressedTable.strand[id].CompareTo(smorfToBeAnnotated.Strand) == 0)
                            sameStrand = "S";
                        else
                            sameStrand = "D";

                        int K = 0;
                        for (int k = compressedTable.pStart[id]; k <= compressedTable.pEnd[id]; k++)
                        {

                            smorfToBeAnnotated.annotationResult[i].Add(
                                new List<string>()
                                {
                                    "N/A",//name2
                                    "N/A",//name
                                    "N/A",
                                    "N/A",
                                    "N/A",
                                    "N/A",
                                    "N/A"
                                }
                            );

                            smorfToBeAnnotated.expandedResultTable.Add(
                                new List<string>()
                                {
                                    "Id",//smorf ID
                                    "An",//AnnotationResult
                                    "0%",//5UTR Percent
                                    "0%",//3UTR Percent
                                    "0%",//Exon Percent
                                    "0%",//Intron Percent
                                    "0%",//Intergenic Percent
                                    name2//name
                                }
                                );

                            smorfToBeAnnotated.annotationResult[i][K][0] = name2;
                            smorfToBeAnnotated.annotationResult[i][K][1] = table.rows[k].name;
                            smorfToBeAnnotated.annotationResult[i][K][2] = sameStrand;

                            smorfToBeAnnotated.expandedResultTable[tableIdx][0] = table.rows[k].name;

                            AnnotateRow(smorfToBeAnnotated, table.rows[k], i, K, tableIdx);
                            K++;
                            tableIdx++;
                        }
                        i++;

                    }


                }
            }
            if (smorfToBeAnnotated.annotationResult == null)
            {
                smorfToBeAnnotated.isInTheTable = false;
                smorfToBeAnnotated.annotationResult = new List<List<List<string>>>();
            }
            if (smorfToBeAnnotated.annotationResult.Count == 0)
                smorfToBeAnnotated.isInTheTable = false;



            makeCondensedTable(smorfToBeAnnotated);
        }

        private void makeCondensedTable(smorf smorfToBeAnnotated)
        {
            smorfToBeAnnotated.condensedResultTable = new List<int>()
            {
                0,//#genes         0
                0,//transcripts    1
                0,//5              2
                0,//3              3
                0,//E              4
                0,//I              5
                0,//X              6
                0//Ambiguous       7
            };
            //List<List<List<string>>> annotationResult { get; set; }//this will be the result of the annotation
            if (smorfToBeAnnotated.isInTheTable)
            {

                for (int i = 0; i < smorfToBeAnnotated.annotationResult.Count; i++)
                {
                    smorfToBeAnnotated.condensedResultTable[0]++;
                    for (int k = 0; k < smorfToBeAnnotated.annotationResult[i].Count; k++)
                    {
                        smorfToBeAnnotated.condensedResultTable[1]++;

                        string temp = smorfToBeAnnotated.annotationResult[i][k][4];

                        if (temp.CompareTo("5") == 0)
                            smorfToBeAnnotated.condensedResultTable[2]++;
                        else if (temp.CompareTo("3") == 0)
                            smorfToBeAnnotated.condensedResultTable[3]++;
                        else if (temp.CompareTo("E") == 0)
                            smorfToBeAnnotated.condensedResultTable[4]++;
                        else if (temp.CompareTo("I") == 0)
                            smorfToBeAnnotated.condensedResultTable[5]++;
                        else if (temp.CompareTo("X") == 0)
                            smorfToBeAnnotated.condensedResultTable[6]++;
                        else
                            smorfToBeAnnotated.condensedResultTable[7]++;

                    }
                }
            }
        }

        private string computeExonFrame(int a, int b, int pos, bool strandPlus, RowTable row)
        {
            if (row.exonFrames[pos] != -1)
            {
                int x;
                if (strandPlus)
                    x = (a - row.exonInt[pos].Range.From + row.exonFrames[pos]) % 3;
                else
                    x = (row.exonInt[pos].Range.To - b + row.exonFrames[pos]) % 3;

                return x.ToString();
            }
            return "N/A";
        }

        private string relativePosition(int a, int b, int pos, bool strandPlus, RowTable row)
        {
            int x;
            if (strandPlus)
                x = a - row.exonInt[pos].Range.From;
            else
                x = row.exonInt[pos].Range.To - b;
            return x.ToString();
        }

        private void AnnotateRow(smorf smorfToBeAnnotated, RowTable row, int geneId, int internalId, int tableId)
        {
            bool strandPos = true;
            int a = smorfToBeAnnotated.Position,
                b = smorfToBeAnnotated.EndPosition;
            if (row.strand.CompareTo("-") == 0)
                strandPos = false;

            //check if the smorf is contained or partially contained 
            if (a >= row.UTR5[0] && b <= row.UTR3[1])
                smorfToBeAnnotated.annotationResult[geneId][internalId][3] = "C";
            else
                smorfToBeAnnotated.annotationResult[geneId][internalId][3] = "PC";

            if (a > row.UTR3[1] || b < row.UTR5[0])
            {
                smorfToBeAnnotated.annotationResult[geneId][internalId][3] = "NC";//not contained
                smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "X"; //The smorf is intergenic

                smorfToBeAnnotated.expandedResultTable[tableId][6] = "100%";
                smorfToBeAnnotated.expandedResultTable[tableId][1] = "X";
            }
            else
            {
                if (row.illDefined == true)
                {
                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "5";

                    smorfToBeAnnotated.expandedResultTable[tableId][2] = "100%";
                }
                else
                {   //check if the smorf lies in an intron region
                    List<int> contains = row.introns.findContained(a, b);

                    if (contains.Count == 1)// the smorf lies in an intron region
                    {
                        if (row.i5UTR.Count > 0 && row.i5UTR.Contains(contains[0]) == true)
                        {
                            if (strandPos)
                            {
                                if (a >= row.UTR5[1])
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "I";
                                    smorfToBeAnnotated.expandedResultTable[tableId][5] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "I";
                                }
                                else
                                {
                                    int porcentaje = 100 * (b - row.UTR5[1]) / (b - a);//intron
                                    if (porcentaje > 0 && porcentaje < 100)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-I-5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][2] = (100 - porcentaje).ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][5] = porcentaje.ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-I-5";
                                    }
                                    else
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][2] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "5";
                                    }
                                }
                            }
                            else
                            {
                                if (a >= row.UTR5[1])
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "I";
                                    smorfToBeAnnotated.expandedResultTable[tableId][5] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "I";
                                }
                                else
                                {
                                    int porcentaje = 100 * (b - row.UTR5[1]) / (b - a);//intron
                                    if (porcentaje > 0 && porcentaje < 100)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-I-3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][3] = (100 - porcentaje).ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][5] = porcentaje.ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-I-3";
                                    }
                                    else
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][3] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "3";
                                    }
                                }
                            }

                        }

                        else if (row.i3UTR.Count > 0 && row.i3UTR.Contains(contains[0]) == true)
                        {
                            if (strandPos)
                            {
                                if (b <= row.UTR3[0])
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "I";
                                    smorfToBeAnnotated.expandedResultTable[tableId][5] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "I";
                                }
                                else
                                {
                                    int porcentaje = 100 * (row.UTR3[0] - a) / (b - a);//intron
                                    if (porcentaje > 0 && porcentaje < 100)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-I-3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-I-3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][3] = (100 - porcentaje).ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][5] = porcentaje.ToString() + "%";
                                    }
                                    else
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "3";
                                        smorfToBeAnnotated.expandedResultTable[tableId][3] = "100%";
                                    }
                                }

                            }
                            else
                            {
                                if (b <= row.UTR3[0])
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "I";
                                    smorfToBeAnnotated.expandedResultTable[tableId][5] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "I";
                                }
                                else
                                {
                                    int porcentaje = 100 * (row.UTR3[0] - a) / (b - a);//intron
                                    if (porcentaje > 0 && porcentaje < 100)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-I-5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-I-5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][2] = (100 - porcentaje).ToString() + "%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][5] = porcentaje.ToString() + "%";
                                    }
                                    else
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "5";
                                        smorfToBeAnnotated.expandedResultTable[tableId][2] = "100%";
                                    }
                                }
                            }

                        }
                        else //the intron is "pure", i.e. doesn't overlap an UTR region
                        {
                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "I";
                            smorfToBeAnnotated.expandedResultTable[tableId][5] = "100%";
                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "I";
                        }
                        //smorf.annotationResult[geneId][internalId][7] = exonOrIntron(a, b, contains[0], false, strandPos, row);
                    }
                    else
                    {
                        //check if the propossed smorf lies in an exon region
                        contains = row.exons.findContained(a, b);
                        if (contains.Count == 1)// the smorf is an exon
                        {
                            bool exonflag = false;
                            //check if it lies into an any UTR region
                            if (row.e5UTR.Count > 0 && row.e5UTR.Contains(contains[0]) == true)
                            {

                                if (strandPos)
                                {
                                    if (row.UTR5[1] < a)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                        smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";
                                        exonflag = true;
                                    }
                                    else
                                    {
                                        int porcentaje = (-row.UTR5[1] + b) / (b - a) * 100;//exon
                                        if (porcentaje < 100 && porcentaje > 0)
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-E-5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-E-5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][2] = (100 - porcentaje).ToString() + "%";//5UTR
                                            smorfToBeAnnotated.expandedResultTable[tableId][4] = porcentaje.ToString() + "%";//exon 
                                        }
                                        else//All it is 5UTR
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][2] = "100%";
                                        }
                                    }
                                }
                                else
                                {
                                    if (row.UTR5[1] < a)
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                        smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";
                                        exonflag = true;
                                    }
                                    else
                                    {
                                        int porcentaje = (-row.UTR5[1] + b) / (b - a) * 100;
                                        if (porcentaje < 100 && porcentaje > 0)
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-E-3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-E-3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][3] = (100 - porcentaje).ToString() + "%";//3UTR
                                            smorfToBeAnnotated.expandedResultTable[tableId][4] = porcentaje.ToString() + "%";//exon 
                                        }
                                        else//All it is 3UTR
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][3] = "100%";
                                        }
                                    }
                                }

                            }
                            else if (row.e3UTR.Count > 0 && row.e3UTR.Contains(contains[0]) == true)
                            {
                                if (strandPos)
                                {
                                    if (b <= row.UTR3[0])
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                        smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";
                                        exonflag = true;
                                    }
                                    else
                                    {
                                        //exon percentage
                                        int porcentaje = (row.UTR3[0] - a) / (b - a) * 100;

                                        if (porcentaje > 0 && porcentaje < 100)
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-E-3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-E-3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][3] = (100 - porcentaje).ToString() + "%";//3UTR
                                            smorfToBeAnnotated.expandedResultTable[tableId][4] = porcentaje.ToString() + "%";//exon
                                        }
                                        else
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "3";
                                            smorfToBeAnnotated.expandedResultTable[tableId][3] = "100%";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "3";
                                        }
                                    }
                                }
                                else
                                {
                                    if (b <= row.UTR3[0])
                                    {
                                        smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                        smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                        smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";
                                        exonflag = true;
                                    }
                                    else
                                    {
                                        int porcentaje = (row.UTR3[0] - a) / (b - a) * 100;
                                        if (porcentaje > 0 && porcentaje < 100)
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A-E-5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "A-E-5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][2] = (100 - porcentaje).ToString() + "%";//3UTR
                                            smorfToBeAnnotated.expandedResultTable[tableId][4] = porcentaje.ToString() + "%";//exon
                                        }
                                        else
                                        {
                                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "5";
                                            smorfToBeAnnotated.expandedResultTable[tableId][2] = "100%";
                                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "5";
                                        }
                                    }

                                }
                            }
                            else
                                exonflag = true;
                            if (exonflag)
                            {
                                if (smorfToBeAnnotated.annotationResult[geneId][internalId][2].CompareTo("S") == 0)
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][5] = computeExonFrame(a, b, contains[0], strandPos, row);
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][6] = relativePosition(a, b, contains[0], strandPos, row);

                                    smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";

                                    smorfToBeAnnotated.exonframe = row.exonFrames[contains[0]].ToString();
                                }
                                else
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "E";
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][5] = "N/A";
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][6] = "N/A";

                                    smorfToBeAnnotated.expandedResultTable[tableId][4] = "100%";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] = "E";
                                }
                            }




                            //smorf.annotationResult[geneId][internalId][7] =  exonOrIntron(a, b, contains[0], true, strandPos, row);
                        }

                        else//the smorf is not an intron nor exon
                        {

                            contains = row.exons.findOverlap(a, b);

                            smorfToBeAnnotated.annotationResult[geneId][internalId][4] = "A";
                            smorfToBeAnnotated.expandedResultTable[tableId][1] = "A";

                            int total = 100;

                            if (smorfToBeAnnotated.annotationResult[geneId][internalId][3].CompareTo("PC") == 0)
                            {
                                smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " X";
                                smorfToBeAnnotated.expandedResultTable[tableId][1] += "X";
                                if (smorfToBeAnnotated.EndPosition > row.exonInt[row.exonInt.Count - 1].Range.To)
                                {
                                    int percent = Math.Abs(100 * (smorfToBeAnnotated.EndPosition - row.exonInt[row.exonInt.Count - 1].Range.To) / (smorfToBeAnnotated.EndPosition - smorfToBeAnnotated.Position));
                                    total -= percent;
                                    smorfToBeAnnotated.expandedResultTable[tableId][6] = percent.ToString() + "%";
                                }
                                else
                                {
                                    int percent = Math.Abs(100 * (row.exonInt[0].Range.From - smorfToBeAnnotated.Position) / (smorfToBeAnnotated.EndPosition - smorfToBeAnnotated.Position));
                                    total -= percent;
                                    smorfToBeAnnotated.expandedResultTable[tableId][6] = percent.ToString() + "%";
                                }
                            }

                            List<int> temp = row.introns.findOverlap(a, b);


                            if (row.e5UTR.Count > 0 && row.e5UTR.Contains(contains[0]) == true)
                            {
                                int percent = Math.Abs(100 * (smorfToBeAnnotated.EndPosition - row.exonInt[contains[0]].Range.From) / (smorfToBeAnnotated.EndPosition - smorfToBeAnnotated.Position));
                                total -= percent;
                                if (strandPos)
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " 5";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] += "5";
                                    smorfToBeAnnotated.expandedResultTable[tableId][2] = percent.ToString() + "%";
                                }
                                else
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " 3";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] += "3";
                                    smorfToBeAnnotated.expandedResultTable[tableId][3] = percent.ToString() + "%";
                                }
                            }
                            else if (row.e3UTR.Count > 0 && row.e3UTR.Contains(contains[contains.Count - 1]) == true)
                            {
                                int val = contains[contains.Count - 1];
                                int percent = Math.Abs(100 * (smorfToBeAnnotated.EndPosition - row.exonInt[val].Range.To) / (smorfToBeAnnotated.EndPosition - smorfToBeAnnotated.Position));

                                total -= percent;
                                if (strandPos)
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " 3";

                                    smorfToBeAnnotated.expandedResultTable[tableId][1] += " 3";
                                    smorfToBeAnnotated.expandedResultTable[tableId][3] = percent.ToString() + "%";
                                }
                                else
                                {
                                    smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " 5";
                                    smorfToBeAnnotated.expandedResultTable[tableId][1] += " 5";
                                    smorfToBeAnnotated.expandedResultTable[tableId][2] = percent.ToString() + "%";
                                }
                            }
                            else
                            {
                                int percent = Math.Abs(100 * (smorfToBeAnnotated.EndPosition - row.exonInt[contains[0]].Range.From) / (smorfToBeAnnotated.EndPosition - smorfToBeAnnotated.Position));
                                total -= percent;
                                smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " E";

                                smorfToBeAnnotated.expandedResultTable[tableId][1] += " E";
                                smorfToBeAnnotated.expandedResultTable[tableId][4] = percent.ToString() + "%";
                            }
                            if (total > 0)
                            {
                                smorfToBeAnnotated.annotationResult[geneId][internalId][4] += " I";

                                smorfToBeAnnotated.expandedResultTable[tableId][1] += " I";
                                smorfToBeAnnotated.expandedResultTable[tableId][4] = total.ToString() + "%";
                            }


                        }

                    }


                }

            }

        }


    }
}
