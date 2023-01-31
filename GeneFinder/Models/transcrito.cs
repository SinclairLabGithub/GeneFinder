using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    [Serializable()]
    public class transcrito
    {
        public string GeneName { get; set; }
        public string TranscritName { get; set; }
        public string SameStrand { get; set; }
        public string Content { get; set; }
        public string Notation { get; set; }
        public int RelativeFrame { get; set; }
        public int ExonPosition { get; set; }
        public string ExpandedAnotation { get; set; }
    }
}
