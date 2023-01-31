using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    class HistogramValuePoint
    {
        public double xValue { get; set; }
        public double yValue { get; set; }
        public HistogramValuePoint(double x, double y)
        {
            xValue = x;
            yValue = y;
        }
    }
}
