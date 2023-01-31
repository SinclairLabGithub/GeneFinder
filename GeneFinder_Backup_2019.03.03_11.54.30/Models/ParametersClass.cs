using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace GeneFinder.Models
{
    [Serializable()]
    public class ParametersClass : ISerializable
    {
        public List<string> startCodons { get; set; }
        public List<string> stopCodons { get; set; }
        public int minLenght { get; set; }
        public int maxLenght { get; set; }
        public int similarity { get; set; }
        public bool allowChange { get; set; }
        public string specie1 { get; set; }
        public string specie2 { get; set; }
        public string specie3 { get; set; }
        public string specie1Red { get; set; }
        public string specie2Red { get; set; }
        public string specie3Red { get; set; }
        public double ta { get; set; }
        public double tb { get; set; }
        public double tc { get; set; }
        public double td { get; set; }
        public double cutoff { get; set; }
        public string genomeFolder { get; set; }
        public string annotationBigTableName { get; set; }
        public string annotationSmallTableName { get; set; }
        public string conservationFolder { get; set; }
        public List<string> genomeFiles { get; set; }
        public string indexFile { get; set; }
        public int indexWindowSize { get; set; }
        public List<string> kozaks { get; set; }
        public int basesToCheck { get; set; }
        public float conservationThreshold { get; set; }
        public List<string> conditionsName { get; set; }
        public bool extracted { get; set; }
        public bool classified { get; set; }
        public bool realigned { get; set; }
        public bool annotated { get; set; }
        public bool conditions { get; set; }
        public bool temporalFile { get; set; }
        public string version { get; set; }
        public int alignedBy { get; set; }

        public ParametersClass()
        {
            using (StreamReader reader = new StreamReader("Data//DefaultSettings.txt"))
            {
                bool end = false;
                while (!end)
                {
                    string lector = reader.ReadLine();
                    switch (lector)
                    {
                        case "Version":
                            version = reader.ReadLine();
                            break;
                        case "Start Codons Number":
                            int numStartCodons = int.Parse(reader.ReadLine());
                            startCodons = new List<string>();
                            for (int i = 0; i < numStartCodons; i++)
                            {
                                startCodons.Add(reader.ReadLine());
                            }
                            break;
                        case "Stop Codons Number":
                            int numStopCodons = int.Parse(reader.ReadLine());
                            stopCodons = new List<string>();
                            for (int i = 0; i < numStopCodons; i++)
                            {
                                stopCodons.Add(reader.ReadLine());
                            }
                            break;
                        case "Min length":
                            minLenght = int.Parse(reader.ReadLine());
                            break;
                        case "Max length":
                            maxLenght = int.Parse(reader.ReadLine());
                            break;
                        case "Conservation Cutoff":
                            cutoff = double.Parse(reader.ReadLine());
                            break;
                        case "Genome Folder":
                            genomeFolder = reader.ReadLine();
                            break;
                        case "Annotation BigTableName":
                            annotationBigTableName = reader.ReadLine();
                            break;
                        case "Annotation SmallTableName":
                            annotationSmallTableName = reader.ReadLine();
                            break;
                        case "Conservation Folder":
                            conservationFolder = reader.ReadLine();
                            break;
                        case "Kozaks Number":
                            int numKozaks = int.Parse(reader.ReadLine());
                            kozaks = new List<string>();
                            for (int i = 0; i < numKozaks; i++)
                            {
                                kozaks.Add(reader.ReadLine());
                            }
                            break;
                        case "Bases to check":
                            basesToCheck = int.Parse(reader.ReadLine());
                            break;
                        case "Conservation Threshold":
                            conservationThreshold = float.Parse(reader.ReadLine());
                            break;
                        case "End Header":
                            end = true;
                            break;
                        default:

                            break;
                    }
                }
            }
            
            
            allowChange = true;
            ta = 0.039953;
            tb = 0.009168;
            tc = 0.007501;
            td = 0.026355;
            cutoff = 0.2;
            genomeFiles = new List<string>();
            indexFile = "";
            indexWindowSize = 23;
            conditionsName = new List<string>();
            extracted = false;
            classified = false;
            realigned = false;
            annotated = false;
            conditions = false;
            version = "1";
            alignedBy = 1;
        }

        internal void AddFileHeader(System.IO.StreamWriter outputStream)
        {
            outputStream.WriteLine("Version");
            outputStream.WriteLine(version);
            outputStream.WriteLine("Extracted");
            outputStream.WriteLine(extracted);
            outputStream.WriteLine("Classified");
            outputStream.WriteLine(classified);
            outputStream.WriteLine("Realigned");
            outputStream.WriteLine(realigned);
            outputStream.WriteLine("Annotated");
            outputStream.WriteLine(annotated);
            outputStream.WriteLine("Condition");
            outputStream.WriteLine(conditions);
            outputStream.WriteLine("Start Codons Number");
            outputStream.WriteLine(startCodons.Count);
            foreach(var salida in startCodons)
            {
                outputStream.WriteLine(salida);
            }
            outputStream.WriteLine("Stop Codons Number");
            outputStream.WriteLine(stopCodons.Count);
            foreach (var salida in stopCodons)
            {
                outputStream.WriteLine(salida);
            }
            outputStream.WriteLine("Min length");
            outputStream.WriteLine(minLenght);
            outputStream.WriteLine("Max length");
            outputStream.WriteLine(maxLenght);
            outputStream.WriteLine("TA");
            outputStream.WriteLine(ta);
            outputStream.WriteLine("TB");
            outputStream.WriteLine(tb);
            outputStream.WriteLine("TC");
            outputStream.WriteLine(tc);
            outputStream.WriteLine("TD");
            outputStream.WriteLine(td);
            outputStream.WriteLine("Conservation Cutoff");
            outputStream.WriteLine(cutoff);
            outputStream.WriteLine("Genomefiles Number");
            outputStream.WriteLine(genomeFiles.Count);
            foreach (var salida in genomeFiles)
            {
                outputStream.WriteLine(salida);
            }
            outputStream.WriteLine("Index File");
            outputStream.WriteLine(indexFile);
            outputStream.WriteLine("Genome Folder");
            outputStream.WriteLine(genomeFolder);
            outputStream.WriteLine("Annotation BigTableName");
            outputStream.WriteLine(annotationBigTableName);
            outputStream.WriteLine("Annotation SmallTableName");
            outputStream.WriteLine(annotationSmallTableName);
            outputStream.WriteLine("Conservation Folder");
            outputStream.WriteLine(conservationFolder);
            outputStream.WriteLine("Index Windows Size");
            outputStream.WriteLine(indexWindowSize);
            outputStream.WriteLine("Kozaks Number");
            outputStream.WriteLine(kozaks.Count());
            foreach (var salida in kozaks)
            {
                outputStream.WriteLine(salida);
            }
            outputStream.WriteLine("Bases to check");
            outputStream.WriteLine(basesToCheck);
            outputStream.WriteLine("Conservation Threshold");
            outputStream.WriteLine(conservationThreshold);
            outputStream.WriteLine("Conditions Number");
            outputStream.WriteLine(conditionsName.Count);
            foreach (var salida in conditionsName)
            {
                outputStream.WriteLine(salida);
            }
            outputStream.WriteLine("End Header");
        }

        internal void ReadFileHeader(System.IO.StreamReader inputStream)
        {
            bool end = false;
            while (!end)
            {
                string lector = inputStream.ReadLine();
                switch (lector)
                {
                    case "Version":
                        version = inputStream.ReadLine();
                        break;
                    case "Extracted":
                        extracted = bool.Parse(inputStream.ReadLine());
                        break;
                    case "Classified":
                        classified = bool.Parse(inputStream.ReadLine());
                        break;
                    case "Realigned":
                        realigned = bool.Parse(inputStream.ReadLine());
                        break;
                    case "Annotated":
                        annotated = bool.Parse(inputStream.ReadLine());
                        break;
                    case "Condition":
                        conditions = bool.Parse(inputStream.ReadLine());
                        break;
                    case "Start Codons Number":
                        int numStartCodons = int.Parse(inputStream.ReadLine());
                        startCodons = new List<string>();
                        for (int i = 0; i < numStartCodons; i++)
                        {
                            startCodons.Add(inputStream.ReadLine());
                        }
                        break;
                    case "Stop Codons Number":
                        int numStopCodons = int.Parse(inputStream.ReadLine());
                        stopCodons = new List<string>();
                        for (int i = 0; i < numStopCodons; i++)
                        {
                            stopCodons.Add(inputStream.ReadLine());
                        }
                        break;
                    case "Min length":
                        minLenght = int.Parse(inputStream.ReadLine());
                        break;
                    case "Max length":
                        maxLenght = int.Parse(inputStream.ReadLine());
                        break;
                    case "TA":
                        ta = double.Parse(inputStream.ReadLine());
                        break;
                    case "TB":
                        tb = double.Parse(inputStream.ReadLine());
                        break;
                    case "TC":
                        tc = double.Parse(inputStream.ReadLine());
                        break;
                    case "TD":
                        td = double.Parse(inputStream.ReadLine());
                        break;
                    case "Conservation Cutoff":
                        cutoff = double.Parse(inputStream.ReadLine());
                        break;
                    case "Genomefiles Number":
                        int genofilesCount = int.Parse(inputStream.ReadLine());
                        genomeFiles = new List<string>();
                        for (int i = 0; i < genofilesCount; i++)
                        {
                            genomeFiles.Add(inputStream.ReadLine());
                        }
                        break;
                    case "Index File":
                        indexFile = inputStream.ReadLine();
                        break;
                    case "Genome Folder":
                        genomeFolder = inputStream.ReadLine();
                        break;
                    case "Annotation BigTableName":
                        annotationBigTableName = inputStream.ReadLine();
                        break;
                    case "Annotation SmallTableName":
                        annotationSmallTableName = inputStream.ReadLine();
                        break;
                    case "Conservation Folder":
                        conservationFolder = inputStream.ReadLine();
                        break;
                    case "Index Windows Size":
                        indexWindowSize = int.Parse(inputStream.ReadLine());
                        break;
                    case "Kozaks Number":
                        int numKozaks = int.Parse(inputStream.ReadLine());
                        kozaks = new List<string>();
                        for (int i = 0; i < numKozaks; i++)
                        {
                            kozaks.Add(inputStream.ReadLine());
                        }
                        break;
                    case "Bases to check":
                        basesToCheck = int.Parse(inputStream.ReadLine());
                        break;
                    case "Conservation Threshold":
                        conservationThreshold = float.Parse(inputStream.ReadLine());
                        break;
                    case "Conditions Number":
                        int numConditiona = int.Parse(inputStream.ReadLine());
                        conditionsName = new List<string>();
                        for (int i = 0; i < numConditiona; i++)
                        {
                            conditionsName.Add(inputStream.ReadLine());
                        }
                        break;
                    case "End Header":
                        end = true;
                        break;
                    default:

                        break;
                }
            }
        }

        internal void loadFolders()
        {
            using (StreamReader reader = new StreamReader("Data//DefaultSettings.txt"))
            {
                bool end = false;
                while (!end)
                {
                    string lector = reader.ReadLine();
                    switch (lector)
                    {
                        case "Genome Folder":
                            genomeFolder = reader.ReadLine();
                            break;
                        case "Annotation BigTableName":
                            annotationBigTableName = reader.ReadLine();
                            break;
                        case "Annotation SmallTableName":
                            annotationSmallTableName = reader.ReadLine();
                            break;
                        case "Conservation Folder":
                            conservationFolder = reader.ReadLine();
                            break;
                        case "Kozaks Number":
                            if (kozaks.Count == 0)
                            {
                                int numKozaks = int.Parse(reader.ReadLine());
                                kozaks = new List<string>();
                                for (int i = 0; i < numKozaks; i++)
                                {
                                    kozaks.Add(reader.ReadLine());
                                }
                            }                            
                            break;
                        case "End Header":
                            end = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("startCodons", startCodons);
            info.AddValue("stopCodons", stopCodons);
            info.AddValue("minLenght", minLenght);
            info.AddValue("maxLenght", maxLenght);
            info.AddValue("similarity", similarity);
            info.AddValue("allowChange", allowChange);
            info.AddValue("specie1", specie1);
            info.AddValue("specie2", specie2);
            info.AddValue("specie3", specie3);
            info.AddValue("specie1Red", specie1Red);
            info.AddValue("specie2Red", specie2Red);
            info.AddValue("specie3Red", specie3Red);
            info.AddValue("ta", ta);
            info.AddValue("tb", tb);
            info.AddValue("tc", tc);
            info.AddValue("td", td);
            info.AddValue("cutoff", cutoff);
            info.AddValue("genomeFolder", genomeFolder);
            info.AddValue("annotationBigTableName", annotationBigTableName);
            info.AddValue("annotationSmallTableName", annotationSmallTableName);
            info.AddValue("conservationFolder", conservationFolder);
            info.AddValue("genomeFiles", genomeFiles);
            info.AddValue("indexFile", indexFile);
            info.AddValue("indexWindowSize", indexWindowSize);
            info.AddValue("kozaks", kozaks);
            info.AddValue("basesToCheck", basesToCheck);
            info.AddValue("conservationThreshold", conservationThreshold);
            info.AddValue("conditionsName", conditionsName);
            info.AddValue("extracted", extracted);
            info.AddValue("classified", classified);
            info.AddValue("realigned", realigned);
            info.AddValue("annotated", annotated);
            info.AddValue("conditions", conditions);
            info.AddValue("temporalFile", temporalFile);
            info.AddValue("version", version);
            info.AddValue("alignedBy", alignedBy);
        }

        public ParametersClass(SerializationInfo info, StreamingContext context)
        {
            startCodons = (List<string>)info.GetValue("startCodons", typeof(List<string>));
            stopCodons = (List<string>)info.GetValue("stopCodons", typeof(List<string>));
            minLenght = (int)info.GetValue("minLenght", typeof(int));
            maxLenght = (int)info.GetValue("maxLenght", typeof(int));
            similarity= (int)info.GetValue("similarity", typeof(int));
            allowChange = (bool)info.GetValue("allowChange", typeof(bool));
            specie1 = (string)info.GetValue("specie1", typeof(string));
            specie2 = (string)info.GetValue("specie2", typeof(string));
            specie3 = (string)info.GetValue("specie3", typeof(string));
            specie1Red = (string)info.GetValue("specie1Red", typeof(string));
            specie2Red = (string)info.GetValue("specie2Red", typeof(string));
            specie3Red = (string)info.GetValue("specie3Red", typeof(string));
            ta = (double)info.GetValue("ta", typeof(double));
            tb = (double)info.GetValue("tb", typeof(double));
            tc = (double)info.GetValue("tc", typeof(double));
            td = (double)info.GetValue("td", typeof(double));
            cutoff = (double)info.GetValue("cutoff", typeof(double));
            genomeFolder = (string)info.GetValue("genomeFolder", typeof(string));
            annotationBigTableName = (string)info.GetValue("annotationBigTableName", typeof(string));
            annotationSmallTableName = (string)info.GetValue("annotationSmallTableName", typeof(string));
            conservationFolder = (string)info.GetValue("conservationFolder", typeof(string));
            genomeFiles = (List<string>)info.GetValue("genomeFiles", typeof(List<string>));
            indexFile = (string)info.GetValue("indexFile", typeof(string));
            indexWindowSize = (int)info.GetValue("indexWindowSize", typeof(int));
            kozaks = (List<string>)info.GetValue("kozaks", typeof(List<string>));
            basesToCheck = (int)info.GetValue("basesToCheck", typeof(int));
            conservationThreshold = (float)info.GetValue("conservationThreshold", typeof(float));
            conditionsName = (List<string>)info.GetValue("conditionsName", typeof(List<string>));
            extracted = (bool)info.GetValue("extracted", typeof(bool));
            classified = (bool)info.GetValue("classified", typeof(bool));
            realigned = (bool)info.GetValue("realigned", typeof(bool));
            annotated = (bool)info.GetValue("annotated", typeof(bool));
            conditions = (bool)info.GetValue("conditions", typeof(bool));
            temporalFile = (bool)info.GetValue("temporalFile", typeof(bool));
            version = (string)info.GetValue("version", typeof(string));
        }
    }
}
