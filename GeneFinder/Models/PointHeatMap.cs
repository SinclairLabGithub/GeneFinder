using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    public class PointHeatMap
    {
        public string Chr { get; set; }
        public int Position { get; set; }
        public double Value { get; set; }
        

        public PointHeatMap()
        {
            Chr = "";
            Position = 0;
            Value = 0;
        }

        public PointHeatMap(string Chromosome, int pos, double val)
        {
            this.Chr = Chromosome;
            this.Position = pos;
            this.Value = val;
        }
    }
}
