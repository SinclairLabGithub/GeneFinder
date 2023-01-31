using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Sequence
    {
        Codon[] codons;
        string seq;

        public Codon this[int index]
        {
            get
            {
                return this.codons[index];
            }
        }

        public int Length
        {
            get
            {
                return codons.Length;
            }
        }

        public override string ToString()
        {
            return seq;
        }

        public Sequence(string str)
        {
            this.seq = str;
            int numCodons = str.Length / 3;
            codons = new Codon[numCodons];
            for (int i = 0; i < numCodons; ++i)
            {
                codons[i] = new Codon(str.Substring(3 * i, 3));
            }
        }
    }
}
