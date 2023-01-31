using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    internal static class CodonRateMatrices
    {
        static readonly double[][] codingL;
        static readonly double[][] codingR;
        static readonly double[] codingD;
        static readonly double[] codingDistribution;
        static readonly double[][] nonCodingL;
        static readonly double[][] nonCodingR;
        static readonly double[] nonCodingD;
        static readonly double[] nonCodingDistribution;
        static readonly double[][][] codingRate;
        static readonly double[][][] nonCodingRate;
        static readonly double[] T;

        

        static CodonRateMatrices()
        {
            try
            {
                //StreamReader streader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneFinder"+"."+"Data" +"." +"codingL.txt"));
                //codingL = ReadLines(() => Assembly.GetExecutingAssembly().GetManifestResourceStream("GeneFinder" + "." + "Data" + "." + "codingL.txt"))
                codingL = ReadLines("Data//newCodingL.txt")
                    .Select(l => l.Split(' ')
                        .Select(d => double.Parse(d, NumberStyles.Float)).ToArray())
                        .ToArray();
                //codingL = streader.
                //codingL = File.ReadAllLines("../../codingL.txt")
                //              .Select(l => l.Split(' ')
                //              .Select(d => double.Parse(d, NumberStyles.Float)).ToArray()).ToArray();
                codingD = ReadLines("Data//newCodingD.txt").First().Split(' ') //File.ReadLines(@"/Data/codingD.txt").First().Split(' ')
                              .Select(d => double.Parse(d, NumberStyles.Float)).ToArray();
                codingR = ReadLines("Data//newCodingR.txt") //File.ReadAllLines(@"/Data/codingR.txt")
                              .Select(l => l.Split(' ')
                              .Select(d => double.Parse(d, NumberStyles.Float)).ToArray()).ToArray();
                nonCodingL = ReadLines("Data//newNonCodingL.txt") //File.ReadAllLines(@"nonCodingL.txt")
                                 .Select(l => l.Split(' ')
                                 .Select(d => double.Parse(d, NumberStyles.Float)).ToArray()).ToArray();
                nonCodingD = ReadLines("Data//newNonCodingD.txt").First().Split(' ') //File.ReadLines("../../nonCodingD.txt").First().Split(' ')
                                 .Select(d => double.Parse(d, NumberStyles.Float)).ToArray();
                nonCodingR = ReadLines("Data//newNonCodingR.txt") //File.ReadAllLines("../../nonCodingR.txt")
                                 .Select(l => l.Split(' ')
                                 .Select(d => double.Parse(d, NumberStyles.Float)).ToArray()).ToArray();
                codingDistribution = ReadLines("Data//codingDistribution2.txt").First().Split(' ') //File.ReadLines("../../codingDistribution.txt").First().Split(' ')
                                         .Select(d => double.Parse(d, NumberStyles.Float)).ToArray();
                nonCodingDistribution = ReadLines("Data//nonCodingDistribution2.txt").First().Split(' ') //File.ReadLines("../../nonCodingDistribution.txt").First().Split(' ')
                                            .Select(d => double.Parse(d, NumberStyles.Float)).ToArray();
                T = ReadLines("Data//hcrPhyloT.txt").First().Split(' ') //File.ReadLines("../../hcrPhyloT.txt").Skip(1).First().Split(' ')
                        .Select(d => double.Parse(d, NumberStyles.Float)).ToArray();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }

            codingRate = new double[64][][];
            nonCodingRate = new double[64][][];

            for (int i = 0; i < 64; ++i)
            {
                codingRate[i] = new double[64][];
                nonCodingRate[i] = new double[64][];
                for (int j = 0; j < 64; ++j)
                {
                    codingRate[i][j] = new double[64];
                    nonCodingRate[i][j] = new double[64];
                    for (int k = 0; k < 64; ++k)
                    {
                        codingRate[i][j][k] = codingL[i][k] * codingR[k][j];
                        nonCodingRate[i][j][k] = nonCodingL[i][k] * nonCodingR[k][j];
                    }
                }
            }
        }

        public static double[] GetDistribution(bool coding)
        {
            return coding ? codingDistribution : nonCodingDistribution;
        }

        public static void CalculateSubstitutionMatrices(bool coding, double theta, double[][][] substitution)
        {
            double[][][] rate = coding ? codingRate : nonCodingRate;
            double[] diagonal = coding ? codingD : nonCodingD;
            //Console.WriteLine ("Theta " + theta); 
            for (int iSubject = 0; iSubject < 4; ++iSubject)
            {
                //	Console.WriteLine ("iSubject " + iSubject);
                for (int i = 0; i < 64; ++i)
                {
                    for (int j = 0; j < 64; ++j)
                    {
                        substitution[iSubject][i][j] = 0;
                        for (int k = 0; k < 64; ++k)
                            substitution[iSubject][i][j] += Math.Exp(theta * diagonal[k] * T[iSubject]) * rate[i][j][k];
                        //			Console.Write (String.Format("{0:0.0} ", substitution [iSubject] [i] [j]));
                    }
                    //		Console.WriteLine ();
                }
            }
        }

        public static IEnumerable<string> ReadLines(string streamProvider)
        {
            List<string> lines = new List<string>();
            //using (var stream = streamProvider())
            using (StreamReader reader = new StreamReader(streamProvider))
            {
                while (!reader.EndOfStream)
                {
                    lines.Add(reader.ReadLine());
                }
            }
            return lines;
        }
    }
}
