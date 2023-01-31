using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    [Serializable()]
    public class AlignmentInstance : ISerializable
    {
        
        public AlignmentInstance(string source)
        {
            string[] fastCheck = System.Text.RegularExpressions.Regex.Split(source, @"\s+");
            int beforeDot = fastCheck[1].IndexOf('.');
            specieName = fastCheck[1].Substring(0, beforeDot);
            if (specieName.StartsWith("ancestral")) return;
            this.possibleChromosome = fastCheck[1].Substring(beforeDot + 1);
            
            start = int.Parse(fastCheck[2]);
            //size = int.Parse(fastCheck[3]);
            strand = fastCheck[4][0];
            sourceSize = int.Parse(fastCheck[5]);
            text = fastCheck[6].ToUpperInvariant();
            size = start + text.Length;
        }

        public AlignmentInstance()
        {
            fileName = "";
            specieName = "";
            possibleChromosome = "";
            start = size = sourceSize = 0;
            strand = ' ';
            text = "";
        }

        public string fileName { get; set; }
        public string specieName { get; set; }
        public string possibleChromosome { get; set; }
        public int start { get; set; }
        public int size { get; set; }
        public char strand { get; set; }
        public int sourceSize { get; set; }
        public string text { get; set; }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("fileName", fileName);
            info.AddValue("specieName", specieName);
            info.AddValue("possibleChromosome", possibleChromosome);
            info.AddValue("start", start);
            info.AddValue("size", size);
            info.AddValue("strand", strand);
            info.AddValue("sourceSize", sourceSize);
            info.AddValue("text", text);
        }
    }
}
