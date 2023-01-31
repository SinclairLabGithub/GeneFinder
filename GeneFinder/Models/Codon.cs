using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Codon
    {
        string chars;
        int number;

        public Codon(string str)
        {
            chars = str.Substring(0, 3);
            number = 0;
            int base4 = 16;
            for (int i = 0; i < 3; ++i)
            {
                switch (chars[i])
                {
                    case 'A':
                        break;
                    case 'C':
                        number += base4;
                        break;
                    case 'G':
                        number += 2 * base4;
                        break;
                    case 'T':
                        number += 3 * base4;
                        break;
                    default:
                        number = -1;
                        i = 3;
                        break;
                }
                base4 /= 4;
            }
        }

        public override string ToString()
        {
            return string.Format("[Codon: chars={0}]", chars);
        }

        public int Number
        {
            get
            {
                return this.number;
            }
        }
    }
}
