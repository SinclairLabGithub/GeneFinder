using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class CodingScore
    {
        double globalScore;
        double[] codonScores;

        public static readonly double invalidScore = -1000000000;

        private CodingScore(CodingScore score)
        {
            this.globalScore = score.globalScore;
            this.codonScores = (double[])score.codonScores.Clone();
        }

        public CodingScore(int numCodons)
        {
            this.globalScore = 0;
            codonScores = new double[numCodons];
        }

        public double GlobalScore
        {
            get
            {
                return this.globalScore;
            }
            set
            {
                globalScore = value;
            }
        }

        public double this[int index]
        {
            get
            {
                return this.codonScores[index];
            }
            set
            {
                codonScores[index] = value;
            }
        }

        public int Length
        {
            get
            {
                return this.codonScores.Length;
            }
        }

        public override string ToString()
        {
            string str = "";
            str += string.Format("{0}\t{1}\t", globalScore, codonScores.Length);
            for (int iCodon = 0; iCodon < codonScores.Length; ++iCodon)
            {
                if (codonScores[iCodon] == invalidScore)
                {
                    str += "N";
                }
                else
                {
                    str += codonScores[iCodon].ToString();
                }
                if (iCodon < codonScores.Length)
                    str += ",";
            }
            return str;
        }

        public CodingScore Copy()
        {
            CodingScore newScore = new CodingScore(this);
            return newScore;
        }

        public static CodingScore operator -(CodingScore left, CodingScore right)
        {
            if (left.codonScores.Length != right.codonScores.Length)
                return null;
            CodingScore result = new CodingScore(left);
            result.globalScore -= right.globalScore;
            for (int i = 0; i < left.codonScores.Length; ++i)
                if (result[i] != invalidScore)
                    result[i] -= right[i];
            return result;
        }
    }
}
