using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace GeneFinder.Models
{
    public class Reference : List<string>
    {
        public List<string> Names { get; set; }

        public Reference(string fastaFilename)
            : base()
        {
            Names = new List<string>();
            //using (var listStream = new StreamReader(fastaListFilename))
            //{
            //    string fastaFilename;
            //    while ((fastaFilename = listStream.ReadLine()) != null)
            //    {
                    
            //    }
            //}

            if (File.Exists(fastaFilename))
            {
                using (var stream = new StreamReader(fastaFilename))
                {
                    Names.Add(stream.ReadLine().TrimStart('>').Split()[0]);
                    this.Add(Regex.Replace(stream.ReadToEnd(), @"(\r|\r\n|\n)", String.Empty));
                }
            }
        }
    }
}
