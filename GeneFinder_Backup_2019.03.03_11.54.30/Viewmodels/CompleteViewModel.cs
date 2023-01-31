using GeneFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GeneFinder.Viewmodels
{
    [Serializable()]
    public class CompleteViewModel : ISerializable
    {
        public List<GroupReads> Paragraphs { get; set; }
        public List<GroupReads> ParagraphsSelected { get; set; }
        public List<string> Species { get; set; }
        public ParametersClass ConfigSetup { get; set; }
        public List<PossibleSmorf> pSmorfsList { get; set; }
        public List<smorf> smorfsList { get; set; }

        public CompleteViewModel()
        {
            Paragraphs = new List<GroupReads>();
            ParagraphsSelected = new List<GroupReads>();
            Species = new List<string>();
            pSmorfsList = new List<PossibleSmorf>();
            smorfsList= new List<smorf>();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Paragraphs", Paragraphs);
            info.AddValue("ParagraphsSelected", ParagraphsSelected);
            info.AddValue("Species", Species);
            info.AddValue("ConfigSetup", ConfigSetup);
            info.AddValue("pSmorfsList", pSmorfsList);
            info.AddValue("smorfsList", smorfsList);
        }

        public CompleteViewModel(SerializationInfo info, StreamingContext context)
        {
            Paragraphs = (List<GroupReads>)info.GetValue("Paragraphs", typeof(List<GroupReads>));
            ParagraphsSelected = (List<GroupReads>)info.GetValue("ParagraphsSelected", typeof(List<GroupReads>));
            Species = (List<string>)info.GetValue("Species", typeof(List<string>));
            ConfigSetup = (ParametersClass)info.GetValue("ConfigSetup", typeof(ParametersClass));
            pSmorfsList = (List<PossibleSmorf>)info.GetValue("pSmorfsList", typeof(List<PossibleSmorf>));
            smorfsList = (List<smorf>)info.GetValue("smorfsList", typeof(List<smorf>));
        }
    }
}
