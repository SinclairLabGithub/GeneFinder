using GeneFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Viewmodels
{
    public static class SmorfSearch
    {
        public static List<PossibleSmorf> Search(ParametersClass parameters, GroupReads reads)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();
            var specie1List = reads.aligments.Where(q => q.specieName == parameters.specie1);
            var specie2List = reads.aligments.Where(q => q.specieName == parameters.specie2);
            var specie3List = reads.aligments.Where(q => q.specieName == parameters.specie3);
            foreach (var specie1Instance in specie1List)
            {
                List<PossibleSmorf> sTemp = Search(parameters, specie1Instance);
                s.AddRange(sTemp);
            }

            //List<PossibleSmorf> checkedSmurfs = Confirm(parameters, s, reads.aligments.FirstOrDefault(q => q.specieName == parameters.specie2), reads.aligments.FirstOrDefault(q => q.specieName == parameters.specie3));
            List<PossibleSmorf> checkedSmurfs = Confirm(parameters, s, specie2List, specie3List);
            return checkedSmurfs;
        }

        private static List<PossibleSmorf> Confirm(ParametersClass parameters, List<PossibleSmorf> founded, IEnumerable<AlignmentInstance> specie2List, IEnumerable<AlignmentInstance> specie3List, bool inverse = false)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();
            foreach (PossibleSmorf smorf in founded)
            {
                bool firstTest = false;
                string secondReadFinal = "";
                string thirdReadFinal = "";
                string secondStartFinal = "";
                string secondStopFinal = "";
                string thirdStartFinal = "";
                string thirdStopFinal = "";
                AlignmentInstance readSecondsFinal = new AlignmentInstance();
                AlignmentInstance readThirdFinal = new AlignmentInstance();
                foreach (var readSeconds in specie2List)
                {
                    string secondRead = readSeconds.text.Substring(smorf.internalIndex, smorf.sequenceLength);
                    string secondStart = secondRead.Substring(0, 3);
                    string secondStop = secondRead.Substring(secondRead.Length - 3);
                    if (parameters.startCodons.Contains(secondStart) && parameters.stopCodons.Contains(secondStop) && CheckDashes(secondRead))
                    {
                        firstTest = true;
                        secondReadFinal = secondRead;
                        secondStartFinal = secondStart;
                        secondStopFinal = secondStop;
                        readSecondsFinal = readSeconds;
                        break;
                    }
                }

                
                if (firstTest)
                {
                    bool secondTest = false;
                    foreach (var readThird in specie3List)
                    {
                        string thirdRead = readThird.text.Substring(smorf.internalIndex, smorf.sequenceLength);
                        string thirdStart = thirdRead.Substring(0, 3);
                        string thirdStop = thirdRead.Substring(thirdRead.Length - 3);
                        if (parameters.startCodons.Contains(thirdStart) && parameters.stopCodons.Contains(thirdStop) && CheckDashes(thirdRead))
                        {
                            secondTest = true;
                            thirdReadFinal = thirdRead;
                            thirdStartFinal = thirdStart;
                            thirdStopFinal = thirdStop;
                            readThirdFinal = readThird;
                            break;
                        }
                    }
                    
                    if (secondTest)
                    {
                        smorf.specie2Content = secondReadFinal;
                        smorf.specie3Content = thirdReadFinal;
                        smorf.similarity = Similarity(smorf.specie1Content, smorf.specie2Content, smorf.specie3Content);
                        if (inverse)
                        {
                            smorf.sameStopCodon = (smorf.startCodon == secondStartFinal && smorf.startCodon == thirdStartFinal);
                            smorf.sameStartCodon = (smorf.stopCodon == secondStopFinal && smorf.stopCodon == thirdStopFinal);
                            smorf.specie1Content = Invert(smorf.specie1Content);
                            smorf.specie2Content = Invert(smorf.specie2Content);
                            smorf.specie3Content = Invert(smorf.specie3Content);
                            smorf.startCodon = smorf.specie1Content.Substring(0, 3);
                            smorf.stopCodon = smorf.specie1Content.Substring(smorf.specie1Content.Length - 3);
                        }
                        else
                        {
                            smorf.sameStartCodon = (smorf.startCodon == secondStartFinal && smorf.startCodon == thirdStartFinal);
                            smorf.sameStopCodon = (smorf.stopCodon == secondStopFinal && smorf.stopCodon == thirdStopFinal);
                        }
                        if (parameters.alignedBy == 2)
                        {
                            smorf.chromosome = readSecondsFinal.possibleChromosome;
                            smorf.originalStrand = readSecondsFinal.strand;

                            if (inverse)
                            {
                                if (smorf.originalStrand == '+')
                                {
                                    int reacomodo = 1;
                                    reacomodo -= readSecondsFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSecondsFinal.start + smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                    smorf.strand = '-';
                                }
                                else
                                {
                                    int reacomodo = 1 - smorf.specie2Content.Length;
                                    reacomodo += readSecondsFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSecondsFinal.sourceSize - readSecondsFinal.start - smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                    smorf.strand = '+';
                                }
                            }
                            else
                            {
                                if (smorf.originalStrand == '+')
                                {
                                    int reacomodo = 1;
                                    reacomodo -= readSecondsFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSecondsFinal.start + smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                    smorf.strand = '+';
                                }
                                else
                                {
                                    int reacomodo = 1 - smorf.specie2Content.Length;
                                    reacomodo += readSecondsFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSecondsFinal.sourceSize - readSecondsFinal.start - smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                    smorf.strand = '-';
                                }
                            }


                        }
                        else
                        {
                            if (parameters.alignedBy == 3)
                            {
                                smorf.chromosome = readThirdFinal.possibleChromosome;
                                smorf.originalStrand = readThirdFinal.strand;
                                if (inverse)
                                {
                                    if (smorf.originalStrand == '+')
                                    {
                                        int reacomodo = 1;
                                        reacomodo -= readThirdFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThirdFinal.start + smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                        smorf.strand = '-';
                                    }
                                    else
                                    {
                                        int reacomodo = 1 - smorf.specie3Content.Length;
                                        reacomodo += readThirdFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThirdFinal.sourceSize - readThirdFinal.start - smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                        smorf.strand = '+';
                                    }
                                }
                                else
                                {
                                    if (smorf.originalStrand == '+')
                                    {
                                        int reacomodo = 1;
                                        reacomodo -= readThirdFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThirdFinal.start + smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                        smorf.strand = '+';
                                    }
                                    else
                                    {
                                        int reacomodo = 1 - smorf.specie3Content.Length;
                                        reacomodo += readThirdFinal.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThirdFinal.sourceSize - readThirdFinal.start - smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                        smorf.strand = '-';
                                    }
                                }


                            }

                        }
                        s.Add(smorf);
                    }

                }
            }
            return s;
        }

        public static List<PossibleSmorf> SearchInverse(ParametersClass parameters, GroupReads reads)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();
            var specie1List = reads.aligments.Where(q => q.specieName == parameters.specie1);
            var specie2List = reads.aligments.Where(q => q.specieName == parameters.specie2);
            var specie3List = reads.aligments.Where(q => q.specieName == parameters.specie3);
            foreach (var specie1Instance in specie1List)
            {
                List<PossibleSmorf> sTemp = SearchInverse(parameters, specie1Instance);
                s.AddRange(sTemp);
            }
            //s = SearchInverse(parameters, reads.aligments.FirstOrDefault(q => q.specieName == parameters.specie1));
            //List<PossibleSmorf> checkedSmurfs = Confirm(parameters, s, reads.aligments.FirstOrDefault(q => q.specieName == parameters.specie2), reads.aligments.FirstOrDefault(q => q.specieName == parameters.specie3), true);
            List<PossibleSmorf> checkedSmurfs = Confirm(parameters, s, specie2List, specie3List, true);
            return checkedSmurfs;
        }

        public static string Invert(string p)
        {
            string r = "";
            for (int i = p.Length - 1; i >= 0; i--)
            {
                switch (p[i])
                {
                    case 'A':
                        r = r + 'T';
                        break;
                    case 'T':
                        r = r + 'A';
                        break;
                    case 'C':
                        r = r + 'G';
                        break;
                    case 'G':
                        r = r + 'C';
                        break;
                    default:
                        r = r + p[i];
                        break;
                }
            }
            return r;
        }

        private static List<PossibleSmorf> SearchInverse(ParametersClass parameters, AlignmentInstance read)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();

            string searching = read.text;

            int minMod3 = parameters.minLenght;
            while (minMod3 % 3 != 0)
            {
                minMod3++;
            }

            int indexOn = parameters.maxLenght;
            while (indexOn > 0)
            {
                int nuevo = -2;
                foreach (string sta in parameters.stopCodons)
                {
                    if (nuevo == -2)
                    {
                        nuevo = searching.LastIndexOf(sta);
                    }
                    else
                    {
                        int tempPosition = searching.LastIndexOf(sta);
                        if (nuevo < tempPosition)
                        {
                            nuevo = tempPosition;
                        }
                    }
                }

                if (nuevo >= minMod3)
                {
                    int origin = nuevo - parameters.maxLenght + 3;
                    string busquedaTemp = "";
                    if (origin < 0)
                    {
                        origin = 0;
                        busquedaTemp = searching.Substring(origin, nuevo + 3);
                    }
                    else
                    {
                        busquedaTemp = searching.Substring(origin, parameters.maxLenght);
                    }

                    indexOn = origin;
                    int menor = busquedaTemp.Length - 3;
                    //int menor = busquedaTemp.Length - minMod3;

                    for (; menor >= 0; menor = menor - 3)
                    {
                        string localCodon = busquedaTemp.Substring(menor, 3);
                        if (parameters.startCodons.Contains(localCodon))
                        {
                            break;
                        }
                    }

                    if (menor >= 0 && menor <= busquedaTemp.Length - minMod3)
                    {
                        indexOn += menor;
                        string possible = busquedaTemp.Substring(menor);
                        if (CheckDashes(possible))
                        {
                            PossibleSmorf psmorf = new PossibleSmorf()
                            {
                                chromosome = read.possibleChromosome,
                                internalIndex = indexOn,
                                sequenceLength = possible.Length,
                                startCodon = possible.Substring(0, 3),
                                stopCodon = possible.Substring(possible.Length - 3, 3),
                                sourceFile = read.fileName,
                                specie1Content = possible,
                                originalStrand = read.strand

                                //startPosition = read.start + indexOn,
                                //endPosition = read.start + indexOn + possible.Length,
                                //strand = '-'
                                
                            };

                            if (psmorf.originalStrand == '+')
                            {
                                int reacomodo = 1;
                                reacomodo -= read.text.Substring(0, indexOn).Count(q => q == '-');
                                psmorf.startPosition = read.start + indexOn + reacomodo;
                                psmorf.endPosition = psmorf.startPosition + possible.Length - 1;
                                psmorf.strand = '-';
                            }
                            else
                            {
                                int reacomodo = 1 - psmorf.specie1Content.Length;
                                reacomodo += read.text.Substring(0, indexOn).Count(q => q == '-');
                                psmorf.startPosition = read.sourceSize - read.start - indexOn + reacomodo;
                                psmorf.endPosition = psmorf.startPosition + possible.Length - 1;
                                psmorf.strand = '+';
                            }

                            s.Add(psmorf);
                        }
                        searching = searching.Substring(0, nuevo);
                    }
                    else
                    {
                        if (indexOn == 0)
                        {
                            indexOn += nuevo;
                        }
                        searching = searching.Substring(0, nuevo);
                    }



                }
                else
                {
                    break;
                }
            }
            return s;
        }

        private static List<PossibleSmorf> Confirm(ParametersClass parameters, List<PossibleSmorf> founded, AlignmentInstance readSeconds, AlignmentInstance readThird, bool inverse = false)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();
            foreach (PossibleSmorf smorf in founded)
            {
                string secondRead = readSeconds.text.Substring(smorf.internalIndex, smorf.sequenceLength);
                string secondStart = secondRead.Substring(0, 3);
                string secondStop = secondRead.Substring(secondRead.Length - 3);
                if (parameters.startCodons.Contains(secondStart) && parameters.stopCodons.Contains(secondStop) && CheckDashes(secondRead))
                {
                    string thirdRead = readThird.text.Substring(smorf.internalIndex, smorf.sequenceLength);
                    string thirdStart = thirdRead.Substring(0, 3);
                    string thirdStop = thirdRead.Substring(secondRead.Length - 3);
                    if (parameters.startCodons.Contains(thirdStart) && parameters.stopCodons.Contains(thirdStop) && CheckDashes(thirdRead))
                    {
                        smorf.specie2Content = secondRead;
                        smorf.specie3Content = thirdRead;
                        smorf.similarity = Similarity(smorf.specie1Content, smorf.specie2Content, smorf.specie3Content);
                        if (inverse)
                        {
                            smorf.sameStopCodon = (smorf.startCodon == secondStart && smorf.startCodon == thirdStart);
                            smorf.sameStartCodon = (smorf.stopCodon == secondStop && smorf.stopCodon == thirdStop);
                            smorf.specie1Content = Invert(smorf.specie1Content);
                            smorf.specie2Content = Invert(smorf.specie2Content);
                            smorf.specie3Content = Invert(smorf.specie3Content);
                            smorf.startCodon = smorf.specie1Content.Substring(0, 3);
                            smorf.stopCodon = smorf.specie1Content.Substring(smorf.specie1Content.Length - 3);
                        }
                        else
                        {
                            smorf.sameStartCodon = (smorf.startCodon == secondStart && smorf.startCodon == thirdStart);
                            smorf.sameStopCodon = (smorf.stopCodon == secondStop && smorf.stopCodon == thirdStop);
                        }
                        if (parameters.alignedBy == 2)
                        {
                            smorf.chromosome = readSeconds.possibleChromosome;
                            smorf.originalStrand = readSeconds.strand;

                            if (inverse)
                            {
                                if (smorf.originalStrand == '+')
                                {
                                    int reacomodo = 1;
                                    reacomodo -= readSeconds.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSeconds.start + smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                    smorf.strand = '-';
                                }
                                else
                                {
                                    int reacomodo = 1 - smorf.specie2Content.Length;
                                    reacomodo += readSeconds.text.Substring(0, smorf.internalIndex).Count(q => q == '-'); 
                                    smorf.startPosition = readSeconds.sourceSize - readSeconds.start - smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                    smorf.strand = '+';
                                }
                            }
                            else
                            {
                                if (smorf.originalStrand == '+')
                                {
                                    int reacomodo = 1;
                                    reacomodo -= readSeconds.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSeconds.start + smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                    smorf.strand = '+';
                                }
                                else
                                {
                                    int reacomodo = 1 - smorf.specie2Content.Length;
                                    reacomodo += readSeconds.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                    smorf.startPosition = readSeconds.sourceSize - readSeconds.start - smorf.internalIndex + reacomodo;
                                    smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                    smorf.strand = '-';
                                }
                            }

                            
                        }
                        else
                        {
                            if (parameters.alignedBy == 3)
                            {
                                smorf.chromosome = readThird.possibleChromosome;
                                smorf.originalStrand = readThird.strand;
                                if (inverse)
                                {
                                    if (smorf.originalStrand == '+')
                                    {
                                        int reacomodo = 1;
                                        reacomodo -= readThird.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThird.start + smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                        smorf.strand = '-';
                                    }
                                    else
                                    {
                                        int reacomodo = 1 - smorf.specie3Content.Length;
                                        reacomodo += readThird.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThird.sourceSize - readThird.start - smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength - 1;
                                        smorf.strand = '+';
                                    }
                                }
                                else
                                {
                                    if (smorf.originalStrand == '+')
                                    {
                                        int reacomodo = 1;
                                        reacomodo -= readThird.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThird.start + smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                        smorf.strand = '+';
                                    }
                                    else
                                    {
                                        int reacomodo = 1 - smorf.specie3Content.Length;
                                        reacomodo += readThird.text.Substring(0, smorf.internalIndex).Count(q => q == '-');
                                        smorf.startPosition = readThird.sourceSize - readThird.start - smorf.internalIndex + reacomodo;
                                        smorf.endPosition = smorf.startPosition + smorf.sequenceLength + 2;
                                        smorf.strand = '-';
                                    }
                                }
                                
                                
                            }

                        }
                        s.Add(smorf);
                    }
                    
                }
            }
            return s;
        }

        private static int Similarity(string p1, string p2, string p3)
        {
            int s = 0;
            for (int i = 0; i < p1.Length; i++)
            {
                if (p1[i]==p2[i] && p1[i] == p3[i])
                {
                    s++;
                }
            }
            return s * 100 / p1.Length;
        }

        

        static List<PossibleSmorf> Search(ParametersClass parameters, AlignmentInstance read)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();

            string searching = read.text;

            int minMod3 = parameters.minLenght;
            while (minMod3 % 3 != 0)
            {
                minMod3++;
            }

            int indexOn = 0;
            while (indexOn < read.text.Length)
            {
                int nuevo = -2;
                foreach (string sta in parameters.startCodons)
                {
                    if (nuevo == -2)
                    {
                        nuevo = searching.IndexOf(sta);
                    }
                    else
                    {
                        int tempPosition = searching.IndexOf(sta);
                        if (nuevo > tempPosition)
                        { 
                            nuevo = tempPosition;
                        }
                    }
                }

                if (nuevo >= 0)
                {
                    int maxLenght = Math.Min(parameters.maxLenght, searching.Length - nuevo);
                    string busquedaTemp = searching.Substring(nuevo, maxLenght);
                    indexOn += nuevo;
                    int menor = 3;
                    //int menor = minMod3;

                    for (; menor < maxLenght - 3 ; menor = menor +3)
                    {
                        string localCodon = busquedaTemp.Substring(menor, 3);
                        if (parameters.stopCodons.Contains(localCodon))
                        {
                            break;
                        }
                    }

                    if (menor < maxLenght - 3 && menor >= minMod3)
                    {
                        string possible = busquedaTemp.Substring(0, menor + 3);
                        if (CheckDashes(possible))
                        {
                            PossibleSmorf psmorf = new PossibleSmorf()
                            {
                                chromosome = read.possibleChromosome,
                                internalIndex = indexOn,
                                sequenceLength = menor + 3,
                                startCodon = busquedaTemp.Substring(0, 3),
                                stopCodon = busquedaTemp.Substring(menor, 3),
                                sourceFile = read.fileName,
                                specie1Content = busquedaTemp.Substring(0, menor + 3),
                                originalStrand = read.strand

                                //startPosition = read.start + indexOn,                                
                                //endPosition = read.start + indexOn + menor + 3,
                                //strand = '+'                                
                            };
                            
                            if (psmorf.originalStrand == '+')
                            {
                                int reacomodo = 1;
                                reacomodo -= read.text.Substring(0, indexOn).Count(q => q == '-');
                                psmorf.startPosition = read.start + indexOn + reacomodo;
                                psmorf.endPosition = psmorf.startPosition + menor + 2;
                                psmorf.strand = '+';
                            }
                            else
                            {
                                int reacomodo = 1 - psmorf.specie1Content.Length;
                                reacomodo += read.text.Substring(0, indexOn).Count(q => q == '-');
                                psmorf.startPosition = read.sourceSize - read.start - indexOn + reacomodo;
                                psmorf.endPosition = psmorf.startPosition + menor + 2;
                                psmorf.strand = '-';
                            }

                            s.Add(psmorf);
                            
                        }
                        indexOn += 3;
                        searching = searching.Substring(nuevo + 3);                        
                    }
                    else
                    {
                        indexOn += 3;
                        searching = searching.Substring(nuevo + 3);
                    }

                    

                }
                else
                {
                    break;
                }
            }
            return s;
        }

        private static bool CheckDashes(string possible)
        {
            if (possible.Contains('-') || possible.Contains('N')) return false;
            return true;

            //bool comply = true;
            //string[] splited = possible.Split(new string[] { "---" }, StringSplitOptions.None);
            //foreach (string l in splited)
            //{
            //    if (l.Contains('-') || l.Length % 3 != 0)
            //    {
            //        comply = false;
            //        break;
            //    }
            //}
            //return comply;
        }

        internal static List<PossibleSmorf> Search(ParametersClass parameters, string data, string name, int inicio)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();

            string searching = data;

            int minMod3 = parameters.minLenght;
            while (minMod3 % 3 != 0)
            {
                minMod3++;
            }

            int indexOn = 0;
            while (indexOn < data.Length)
            {
                int nuevo = -2;
                foreach (string sta in parameters.startCodons)
                {
                    if (nuevo == -2)
                    {
                        nuevo = searching.IndexOf(sta);
                    }
                    else
                    {
                        int tempPosition = searching.IndexOf(sta);
                        if (nuevo > tempPosition)
                        {
                            nuevo = tempPosition;
                        }
                    }
                }

                if (nuevo >= 0)
                {
                    int maxLenght = Math.Min(parameters.maxLenght, searching.Length - nuevo);
                    string busquedaTemp = searching.Substring(nuevo, maxLenght);
                    indexOn += nuevo;
                    int menor = 3;
                    //int menor = minMod3;

                    for (; menor < maxLenght - 3; menor = menor + 3)
                    {
                        string localCodon = busquedaTemp.Substring(menor, 3);
                        if (parameters.stopCodons.Contains(localCodon))
                        {
                            break;
                        }
                    }

                    if (menor < maxLenght - 3 && menor >= minMod3)
                    {
                        string possible = busquedaTemp.Substring(0, menor + 3);
                        if (CheckDashes(possible))
                        {
                            PossibleSmorf psmorf = new PossibleSmorf()
                            {
                                chromosome = name,
                                internalIndex = indexOn,
                                sequenceLength = menor + 3,
                                startCodon = busquedaTemp.Substring(0, 3),
                                stopCodon = busquedaTemp.Substring(menor, 3),
                                sourceFile = name,
                                specie1Content = busquedaTemp.Substring(0, menor + 3),
                                originalStrand = '+',
                                strand = '+',
                                startPosition = indexOn + inicio + 1,
                                endPosition = indexOn + inicio + menor + 3

                                //startPosition = read.start + indexOn,                                
                                //endPosition = read.start + indexOn + menor + 3,
                                //strand = '+'                                
                            };

                            s.Add(psmorf);

                        }
                        indexOn += 3;
                        searching = searching.Substring(nuevo + 3);
                    }
                    else
                    {
                        indexOn += 3;
                        searching = searching.Substring(nuevo + 3);
                    }



                }
                else
                {
                    break;
                }
            }

            return s;
        }

        internal static List<PossibleSmorf> SearchInverse(ParametersClass parameters, string data, string name, int inicio)
        {
            List<PossibleSmorf> s = new List<PossibleSmorf>();

            string searching = data;

            int minMod3 = parameters.minLenght;
            while (minMod3 % 3 != 0)
            {
                minMod3++;
            }

            int indexOn = parameters.maxLenght;
            while (indexOn > 0)
            {
                int nuevo = -2;
                foreach (string sta in parameters.stopCodons)
                {
                    if (nuevo == -2)
                    {
                        nuevo = searching.LastIndexOf(sta);
                    }
                    else
                    {
                        int tempPosition = searching.LastIndexOf(sta);
                        if (nuevo < tempPosition)
                        {
                            nuevo = tempPosition;
                        }
                    }
                }

                if (nuevo >= minMod3)
                {
                    int origin = nuevo - parameters.maxLenght + 3;
                    string busquedaTemp = "";
                    if (origin < 0)
                    {
                        origin = 0;
                        busquedaTemp = searching.Substring(origin, nuevo + 3);
                    }
                    else
                    {
                        busquedaTemp = searching.Substring(origin, parameters.maxLenght);
                    }

                    indexOn = origin;
                    int menor = busquedaTemp.Length - 3;
                    //int menor = busquedaTemp.Length - minMod3;

                    for (; menor >= 0; menor = menor - 3)
                    {
                        string localCodon = busquedaTemp.Substring(menor, 3);
                        if (parameters.startCodons.Contains(localCodon))
                        {
                            break;
                        }
                    }

                    if (menor >= 0 && menor <= busquedaTemp.Length - minMod3)
                    {
                        indexOn += menor;
                        string possible = busquedaTemp.Substring(menor);
                        if (CheckDashes(possible))
                        {
                            PossibleSmorf psmorf = new PossibleSmorf()
                            {
                                chromosome = name,
                                internalIndex = indexOn,
                                sequenceLength = possible.Length,
                                startCodon = possible.Substring(0, 3),
                                stopCodon = possible.Substring(possible.Length - 3, 3),
                                sourceFile = name,
                                specie1Content = Invert(possible),
                                originalStrand = '-',
                                strand= '-',
                                startPosition = indexOn + inicio + 1,
                                endPosition = indexOn + inicio + possible.Length

                                //startPosition = read.start + indexOn,
                                //endPosition = read.start + indexOn + possible.Length,
                                //strand = '-'

                            };

                            s.Add(psmorf);
                        }
                        searching = searching.Substring(0, nuevo);
                    }
                    else
                    {
                        if (indexOn == 0)
                        {
                            indexOn += nuevo;
                        }
                        searching = searching.Substring(0, nuevo);
                    }



                }
                else
                {
                    break;
                }
            }
            return s;
        }

        
    }
}
