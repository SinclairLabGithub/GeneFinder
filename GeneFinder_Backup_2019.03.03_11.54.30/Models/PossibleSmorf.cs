using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    [Serializable()]
    public class PossibleSmorf
    {

        public PossibleSmorf()
        {

        }

        public PossibleSmorf(string readline)
        {
            string[] tokens = readline.Split('\t');
            id = int.Parse(tokens[0]);
            chromosome = tokens[8];
            startPosition = int.Parse(tokens[9]);
            internalIndex = 0;
            sourceFile = tokens[4];

        }
        public int id { get; set; }
        public string chromosome { get; set; }
        public int startPosition { get; set; }
        public int internalIndex { get; set; }
        public int endPosition { get; set; }
        public int sequenceLength { get; set; }
        public int similarity { get; set; }
        public string startCodon { get; set; }
        public string stopCodon { get; set; }
        public string sourceFile { get; set; }
        public string specie1Content { get; set; }
        public string specie2Content { get; set; }
        public string specie3Content { get; set; }
        public bool sameStartCodon { get; set; }
        public bool sameStopCodon { get; set; }
        public char strand { get; set; }
        public char originalStrand { get; set; }
    }
}
