using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    internal static class Parameters
    {
        static int localIndex;
        static int globalIndex;
        //static double[] localFactor;
        //static double[] globalFactor;
        static double[] localFactor;
        static double[] globalFactor;
        //static Random rand;

        static Parameters()
        {
            localIndex = 0;
            globalIndex = 0;
            //localFactor = new double[]{ 0.5, 0.75, 0.9 };
            //globalFactor = new double[]{ 0.5, 0.75, 0.9 };
            localFactor = new double[] { 0.7, 0.75, 0.8 };
            globalFactor = new double[] { 0.7, 0.75, 0.8 };
            // rand = new Random ();
        }

        public static double getLocalFactor()
        {
            //return 2 * rand.NextDouble ();
            //double[] localFactor = localFactor2;
            return localFactor[(localIndex++) % localFactor.Length];
        }

        public static double getGlobalFactor()
        {
            //			return 2 * rand.NextDouble ();
            //double[] globalFactor = globalFactor2;
            return globalFactor[((globalIndex++) / globalFactor.Length) % globalFactor.Length];
        }
    }
}
