using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    class gff
    {
        public string chr;
        public long start;
        public long end;
        public char strand;
        public string gene_id;
        public string transcript_id;
        public uint[] coverage;
        public long length;
        public int minHits;
        public int maxHits;
    }
}
