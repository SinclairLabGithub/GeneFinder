using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    [Serializable()]
    public class GroupReads : ISerializable
    {
        public List<AlignmentInstance> aligments { get; set; }
        public GroupReads()
        {
            aligments = new List<AlignmentInstance>();
        }

        internal static void CleanRepeats(List<GroupReads> paragraphsSelected)
        {
            for (int i = 0; i < paragraphsSelected.Count; i++)
            {
                for (int j = i + 1; j < paragraphsSelected.Count;)
                {
                    if (paragraphsSelected[i].Compare(paragraphsSelected[j]))
                    {
                        paragraphsSelected.RemoveAt(j);
                    }
                    else
                    {
                        j++;
                    }
                }
            }
        }

        private bool Compare(GroupReads groupReads)
        {
            foreach (var line in this.aligments)
            {
                var linesWithSameSpecie = groupReads.aligments.Where(q => q.specieName == line.specieName);
                foreach (var lineSimilar in linesWithSameSpecie)
                {
                    if (lineSimilar.possibleChromosome == line.possibleChromosome && lineSimilar.start == line.start)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("aligments", aligments);
        }

        public GroupReads(SerializationInfo info, StreamingContext context)
        {
            aligments = (List<AlignmentInstance>)info.GetValue("aligments", typeof(List<AlignmentInstance>));
        }
    }
}
