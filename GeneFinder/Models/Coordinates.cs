using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class Coordinates
    {
        public string Chromosome { get; set; }
        public int Position { get; set; }
        public string Strand { get; set; }
    }
}
