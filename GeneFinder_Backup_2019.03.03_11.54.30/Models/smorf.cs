using GeneFinder.Viewmodels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


namespace GeneFinder.Models
{
    [Serializable()]
    public class smorf : ISerializable
    {

        //general info
        string id;
        string idComplete;
        string sequence;
        string sequenceAsProtein;
        int length;
        int numCodons;
        //multiple sequence repetition on genome
        Boolean flagUniqueSequence;
        int numberRepetition;
        int totalRepetitions;
        //coordinates
        string chromosome;
        string strand;
        int position;
        List<Coordinates> coord;

        //multi species alignment data
        string mafSource;
        string sequenceSecondSpecies;
        string sequenceThirdSpecies;
        float similarity;
        float similaritySecond;
        float similarityThird;
        string metaData;
        //coding
        float codingScore;
        float codingCodonsPercentage;
        float[] expandedCodingScores;
        //conservation
        float conservationAverage;
        float conservationPrevious;
        float conservationPosterior;
        Boolean flagConservation;
        float[] expandedConservation;
        //annotation
        float overlapExon;
        float overlapIntron;
        float overlapIntergenic;
        float overlapUtr5;
        float overlapUtr3;
        float overlapExonReverse;
        string annotation;
        string expandedAnotation;
        string expandedAnotationReverse;
        bool flagOverlapExonEitherStrand;

        List<transcrito> transcritos;

        //kozak
        string kozakSequence;
        float kozakScore;

        //other
        float GCcontent;

        //RNA
        List<string> rnaConditionNames;
        List<int[]> rnaCoverage;
        List<int> rnaHits;

        //New properties added Angel
        public List<List<List<string>>> annotationResult { get; set; }      //smorf annotation, tensor[ #ofRows, [#transcripsperRow, 8] ]
        public List<List<string>> expandedResultTable { get; set; }      //This will store the detailled table on slide 3, matrix of size [nTotalTrasncript,8]
        public List<int> condensedResultTable { get; set; }        //This wil store a row in the slide 1, always of size 8
        public bool isInTheTable;
        public string originalStrand { get; set; }
        //public int StartPosition { get; set; }
        public List<int> rowRanges { get; set; }
        public string EspecieName1 { get; set; }
        public string EspecieName2 { get; set; }
        public string EspecieName3 { get; set; }
        public string exonframe { get; set; }


        //getter and setters
        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public string IdComplete
        {
            get { return idComplete; }
            set { idComplete = value; }
        }
        public string Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }
        public string SequenceAsProtein
        {
            get { return sequenceAsProtein; }
            set { sequenceAsProtein = value; }
        }
        public int Length
        {
            get { return length; }
            set { length = value; }
        }
        public int LengthAsProtein
        {
            get {
                if (sequenceAsProtein == null)
                {
                    sequenceAsProtein = functions.DNAToProtein(this.sequence);
                }
                return sequenceAsProtein.Length - 1; }
        }
        public int NumCodons
        {
            get { return numCodons; }
            set { numCodons = value; }
        }
        public Boolean FlagUniqueSequence
        {
            get { return flagUniqueSequence; }
            set { flagUniqueSequence = value; }
        }
        public int NumberRepetition
        {
            get { return numberRepetition; }
            set { numberRepetition = value; }
        }
        public int TotalRepetitions
        {
            get { return totalRepetitions; }
            set { totalRepetitions = value; }
        }
        public string Chromosome
        {
            get { return chromosome; }
            set 
            {
                if (value.StartsWith("chr", StringComparison.InvariantCultureIgnoreCase))
                {
                    chromosome = value; 
                }
                else
                {
                    chromosome = "chr" + value;
                }
            }
        }
        public string Strand
        {
            get { return strand; }
            set { strand = value; }
        }
        public int Position
        {
            get { return position; }
            set { position = value; }
        }
        public int EndPosition
        {
            get { return position + length - 1; }
        }
        public string GBCoordinates
        {
            get { return String.Format("{0}:{1} - {2}", Chromosome, Position.ToString(), EndPosition.ToString()); }
        }
        //public int Similarity
        //{
        //    get
        //    {
        //        int sim = 0;
        //        for (int i = 0; i < Length; i++)
        //        {
        //            if (Sequence[i] == SequenceSecondSpecies[i] && SequenceSecondSpecies[i] == SequenceThirdSpecies[i]) sim++;
        //        }
        //        return sim * 100 / Length;
        //    }
        //}
        public string StarCodon
        {
            get { return sequence.Substring(0, 3); }
        }
        public string StopCodon
        {
            get { return sequence.Substring(Length - 3); }
        }
        public string MafSource
        {
            get { return mafSource; }
            set { mafSource = value; }
        }
        public string SequenceSecondSpecies
        {
            get { return sequenceSecondSpecies; }
            set { sequenceSecondSpecies = value; }
        }
        public string SequenceThirdSpecies
        {
            get { return sequenceThirdSpecies; }
            set { sequenceThirdSpecies = value; }
        }
        public float Similarity
        {
            get { return similarity; }
            set { similarity = value; }
        }
        public float SimilaritySecond
        {
            get { return similaritySecond; }
            set { similaritySecond = value; }
        }
        public float SimilarityThird
        {
            get { return similarityThird; }
            set { similarityThird = value; }
        }
        public string SimilarityStringSecond
        {
            get { return String.Format("{0:P2}", similaritySecond); }
        }
        public string SimilarityStringThird
        {
            get { return String.Format("{0:P2}", similarityThird); }
        }
        public string MetaData
        {
            get { return metaData; }
            set { metaData = value; }
        }
        public float CodingScore
        {
            get { return codingScore; }
            set { codingScore = value; }
        }
        public float CodingCodonsPercentage
        {
            get { return codingCodonsPercentage; }
            set { codingCodonsPercentage = value; }
        }
        public float[] ExpandedCodingScores
        {
            get { return expandedCodingScores; }
            set { expandedCodingScores = value; }
        }
        public float ConservationAverage
        {
            get { return conservationAverage; }
            set { conservationAverage = value; }
        }
        public float ConservationPrevious
        {
            get { return conservationPrevious; }
            set { conservationPrevious = value; }
        }
        public float ConservationPosterior
        {
            get { return conservationPosterior; }
            set { conservationPosterior = value; }
        }
        public Boolean FlagConservation
        {
            get { return flagConservation; }
            set { flagConservation = value; }
        }
        public float[] ExpandedConservation
        {
            get { return expandedConservation; }
            set { expandedConservation = value; }
        }
        public float OverlapExon
        {
            get { return overlapExon; }
            set { overlapExon = value; }
        }
        public float OverlapIntron
        {
            get { return overlapIntron; }
            set { overlapIntron = value; }
        }
        public float OverlapIntergenic
        {
            get { return overlapIntergenic; }
            set { overlapIntergenic = value; }
        }
        public float OverlapUtr5
        {
            get { return overlapUtr5; }
            set { overlapUtr5 = value; }
        }
        public float OverlapUtr3
        {
            get { return overlapUtr3; }
            set { overlapUtr3 = value; }
        }
        public float OverlapExonReverse
        {
            get { return overlapExonReverse; }
            set { overlapExonReverse = value; }
        }
        public string Annotation
        {
            get { return annotation; }
            set { annotation = value; }
        }
        public string ExpandedAnotation
        {
            get { return expandedAnotation; }
            set { expandedAnotation = value; }
        }
        public string ExpandedAnotationReverse
        {
            get { return expandedAnotationReverse; }
            set { expandedAnotationReverse = value; }
        }
        public bool FlagOverlapExonEitherStrand
        {
            get { return flagOverlapExonEitherStrand; }
            set { flagOverlapExonEitherStrand = value; }
        }
        public List<transcrito> Transcritos
        {
            get { return transcritos; }
            set { transcritos = value; }
        }
        public string KozakSequence
        {
            get { return kozakSequence; }
            set { kozakSequence = value; }
        }
        public float KozakScore
        {
            get { return kozakScore; }
            set { kozakScore = value; }
        }
        public float GCcontent1
        {
            get { return GCcontent; }
            set { GCcontent = value; }
        }
        public List<string> RnaConditionNames
        {
            get { return rnaConditionNames; }
            set { rnaConditionNames = value; }
        }
        public List<int[]> RnaCoverage
        {
            get { return rnaCoverage; }
            set { rnaCoverage = value; }
        }
        public List<int> RnaHits
        {
            get { return rnaHits; }
            set { rnaHits = value; }
        }
        public List<Coordinates> Coord
        {
            get { return coord; }
            set { coord = value; }
        }

        //functions
        public string getFastaProtein()
        {
            return ">" + id + "\n" + sequenceAsProtein;
        }

        public smorf()
        {
            coord = new List<Coordinates>();
            transcritos = new List<transcrito>();
            rnaConditionNames = new List<string>();
            rnaCoverage = new List<int[]>();
            rnaHits = new List<int>();
        }
        public smorf(smorf origen)
        {
            id = origen.id;
            idComplete = origen.idComplete;
            sequence = origen.sequence;
            sequenceAsProtein = origen.sequenceAsProtein;
            length = origen.length;
            numCodons = origen.numCodons;
            flagUniqueSequence = origen.flagUniqueSequence;
            totalRepetitions = origen.totalRepetitions;
            coord = new List<Coordinates>(origen.coord);
            mafSource = origen.mafSource;
            sequenceSecondSpecies = origen.sequenceSecondSpecies;
            sequenceThirdSpecies = origen.sequenceThirdSpecies;
            similarity = origen.similarity;
            GetSimilarities();
            metaData = origen.metaData;
            codingScore = origen.codingScore;
            codingCodonsPercentage = origen.codingCodonsPercentage;
            expandedCodingScores = origen.expandedCodingScores;
            conservationAverage = origen.conservationAverage;
            conservationPrevious = origen.conservationPrevious;
            conservationPosterior = origen.conservationPosterior;
            flagConservation = origen.flagConservation;
            expandedConservation = origen.expandedConservation;
            overlapExon = origen.overlapExon;
            overlapIntron = origen.overlapIntron;
            overlapIntergenic = origen.overlapIntergenic;
            overlapUtr5 = origen.overlapUtr5;
            overlapUtr3 = origen.overlapUtr3;
            overlapExonReverse = origen.overlapExonReverse;
            annotation = origen.annotation;
            expandedAnotation = origen.expandedAnotation;
            expandedAnotationReverse = origen.expandedAnotationReverse;
            flagOverlapExonEitherStrand = origen.flagOverlapExonEitherStrand;
            kozakSequence = origen.kozakSequence;
            kozakScore = origen.kozakScore;
            GCcontent = origen.GCcontent;
            Transcritos = new List<transcrito>();
            //rnaConditionNames = new List<string>(origen.rnaConditionNames);
            //rnaCoverage = new List<int[]>(rnaCoverage);
            //rnaHits = new List<int>(rnaHits);
        }

        public void GetSimilarities()
        {
            float igualesSecond = 0;
            float igualesThird = 0;
            for (int i = 0; i < this.sequence.Length; i++)
            {
                if (this.Sequence[i] == this.SequenceSecondSpecies[i])
                {
                    igualesSecond++;
                }
                if (this.Sequence[i] == this.sequenceThirdSpecies[i])
                {
                    igualesThird++;
                }
            }
            this.SimilaritySecond = igualesSecond / (float)this.sequence.Length;
            this.SimilarityThird = igualesThird / (float)this.sequence.Length;
            
        }

        public smorf(string codingClassifierLine)
        {
            // 0,      1    2      3    4    5      6       7               8               9               10      11    ...
            // header,seq1, seq2, seq3, chr, pos, strand, unique sequence, score global, number of codons, codon1, codon2, ...
            string[] fields = codingClassifierLine.Split(',');
            //0   1    2      3        4        5      6          7            8        9
            //id|chr|strand|start pos|end pos|length|similarity|start codon|end codon|source file
            string[] header = fields[0].Split('|');

            this.id = header[0];
            this.metaData = fields[0];
            this.length = int.Parse(header[5]);
            this.similarity = float.Parse(header[6]);
            this.mafSource = header[9];

            this.sequence = fields[1];
            this.sequenceSecondSpecies = fields[2];
            this.sequenceThirdSpecies = fields[3];

            this.Chromosome = fields[4];
            this.position = int.Parse(fields[5]);
            this.strand = fields[6];

            string[] tempUniqueSequence = fields[7].Split('|');
            this.numberRepetition = int.Parse(tempUniqueSequence[0]);
            this.totalRepetitions = int.Parse(tempUniqueSequence[1]);
            if (this.totalRepetitions != 1)
            {
                this.flagUniqueSequence = true;
            }
            else
            {
                this.flagUniqueSequence = false;
            }

            this.codingScore = float.Parse(fields[8]);
            this.numCodons = int.Parse(fields[9]);

            this.expandedCodingScores = new float[this.numCodons];
            for (int i = 0; i < this.numCodons; i++)
            {
                expandedCodingScores[i] = float.Parse(fields[10 + i]);
            }
            this.codingCodonsPercentage = expandedCodingScores.Average();

            //                  id       |      chr              |       strand      |      start pos      |        end pos                    |     length        |     similarity        |    start codon  |    end codon
            this.idComplete = this.id + "|" + this.chromosome + "|" + this.strand + "|" + this.position + "|" + (this.position + this.length) + "|" + this.length + "|" + this.similarity + "|" + header[7] + "|" + header[8];

            this.sequenceAsProtein = functions.DNAToProtein(this.sequence);

            this.GCcontent = 0;

            foreach (char c in this.sequence)
            {
                if (c == 'C' || c == 'G')
                {
                    this.GCcontent++;
                }
            }

            this.rnaConditionNames = new List<string>();
            this.rnaCoverage = new List<int[]>();
            this.rnaHits = new List<int>();
        }

        public smorf(PossibleSmorf possibleSmorf, CodingScore score)
        {
            this.id = possibleSmorf.id.ToString();
            this.metaData = string.Format(">{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}", possibleSmorf.id, possibleSmorf.chromosome, possibleSmorf.strand, possibleSmorf.startPosition, possibleSmorf.endPosition, possibleSmorf.sequenceLength, possibleSmorf.similarity, possibleSmorf.startCodon, possibleSmorf.stopCodon, possibleSmorf.sourceFile);
            this.length = possibleSmorf.sequenceLength;
            this.similarity = possibleSmorf.similarity;
            this.mafSource = possibleSmorf.sourceFile;

            this.sequence = possibleSmorf.specie1Content;
            this.sequenceSecondSpecies = possibleSmorf.specie2Content;
            this.sequenceThirdSpecies = possibleSmorf.specie3Content;
            this.GetSimilarities();

            this.Chromosome = possibleSmorf.chromosome;
            this.position = possibleSmorf.startPosition;
            this.strand = possibleSmorf.strand.ToString();

            this.numberRepetition = 0;
            this.totalRepetitions = 0;
            this.flagUniqueSequence = false;

            this.codingScore = (float)score.GlobalScore;
            this.numCodons = score.Length;

            this.expandedCodingScores = new float[this.numCodons];
            for (int i = 0; i < this.numCodons; i++)
            {
                expandedCodingScores[i] = (float)score[i];
            }
            this.codingCodonsPercentage = expandedCodingScores.Average();

            //                  id       |      chr              |       strand      |      start pos      |        end pos                    |     length        |     similarity          |           start codon          |          end codon
            this.idComplete = this.id + "|" + this.chromosome + "|" + this.strand + "|" + this.position + "|" + (this.position + this.length) + "|" + this.length + "|" + this.similarity + "|" + possibleSmorf.startCodon + "|" + possibleSmorf.stopCodon;

            this.sequenceAsProtein = functions.DNAToProtein(this.sequence);

            this.GCcontent = 0;

            foreach (char c in this.sequence)
            {
                if (c == 'C' || c == 'G')
                {
                    this.GCcontent++;
                }
            }

            conservationAverage = 0;
            conservationPrevious = 0;
            ConservationPosterior = 0;
            flagConservation = false;
            expandedConservation = new float[0];

            overlapExon = 0;
            overlapIntron = 0;
            overlapIntergenic = 0;
            overlapUtr5 = 0;
            overlapUtr3 = 0;
            overlapExonReverse = 0;

            annotation = "";
            expandedAnotation = "";
            expandedAnotationReverse = "";
            flagOverlapExonEitherStrand = false;

            kozakSequence = "";
            kozakScore = 0;

            this.rnaConditionNames = new List<string>();
            this.rnaCoverage = new List<int[]>();
            this.rnaHits = new List<int>();
        }

        public smorf(string readline, CompleteViewModel mod)
        {
            string[] tokens = readline.TrimStart('>').Split('\t');
            int numTokens = tokens.Length;
            metaData = tokens[0];
            id = tokens[1];
            mod.ConfigSetup.specie1 = tokens[2];
            length = int.Parse(tokens[3]);
            similarity = float.Parse(tokens[4]);
            mafSource = tokens[5];
            //sequence = tokens[6];
            Chromosome = tokens[6];
            if (Chromosome.StartsWith("chrchr")) Chromosome = Chromosome.Substring(3);
            position = int.Parse(tokens[7]);
            strand = tokens[8];
            numberRepetition = int.Parse(tokens[9]);
            totalRepetitions = int.Parse(tokens[10]);
            flagUniqueSequence = bool.Parse(tokens[11]);
            codingScore = float.Parse(tokens[12]);
            if (float.IsNaN(codingScore))
            {
                codingScore = -50;
            }
            numCodons = int.Parse(tokens[13]);
            expandedCodingScores = new float[numCodons];
            for (int i = 0; i < numCodons; i++)
            {
                expandedCodingScores[i] = float.Parse(tokens[14 + i]);
            }
            this.codingCodonsPercentage = expandedCodingScores.Average();
            if (numTokens == 15 + numCodons)
            {
                return;
            }
            int voy = 14 + numCodons;
            int numCord = int.Parse(tokens[voy++]);
            coord = new List<Coordinates>();
            for (int i = 0; i < numCord; i++)
            {
                string chr = tokens[voy++];
                int pos = int.Parse(tokens[voy++]);
                string str = tokens[voy++];
                if (!mod.ConfigSetup.genomeFiles.Contains(chr))
                {
                    mod.ConfigSetup.genomeFiles.Add(chr);
                    string[] ordenador = mod.ConfigSetup.genomeFiles.ToArray();
                    Array.Sort(ordenador, new NaturalComparer());
                    mod.ConfigSetup.genomeFiles.Clear();
                    mod.ConfigSetup.genomeFiles.AddRange(ordenador);
                }
                Coordinates nuevasCoordenadas = new Coordinates() { Chromosome = chr, Position = pos, Strand= str };
                coord.Add(nuevasCoordenadas);
            }
            conservationAverage = float.Parse(tokens[voy++]);
            conservationPrevious = float.Parse(tokens[voy++]);
            conservationPosterior = float.Parse(tokens[voy++]);
            flagConservation = bool.Parse(tokens[voy++]);
            int numExpandedConservation = int.Parse(tokens[voy++]);
            if (numExpandedConservation > 0) expandedConservation = new float[numExpandedConservation];
            for (int i = 0; i < numExpandedConservation; i++)
            {
                expandedConservation[i] = float.Parse(tokens[voy++]);
            }
            overlapExon = float.Parse(tokens[voy++]);
            overlapIntron = float.Parse(tokens[voy++]);
            overlapIntergenic = float.Parse(tokens[voy++]);
            overlapUtr5 = float.Parse(tokens[voy++]);
            overlapUtr3 = float.Parse(tokens[voy++]);
            overlapExonReverse = float.Parse(tokens[voy++]);
            annotation = tokens[voy++];
            expandedAnotation = tokens[voy++];
            expandedAnotationReverse = tokens[voy++];
            flagOverlapExonEitherStrand = bool.Parse(tokens[voy++]);
            kozakSequence = tokens[voy++];
            KozakScore = float.Parse(tokens[voy++]);
            GCcontent = float.Parse(tokens[voy++]);
            //SequenceAsProtein = functions.DNAToProtein(sequence);
        }

        public string getGFF()
        {
            string gffLine = "";
            //seqname
            gffLine += this.Chromosome + "\t";
            //source
            gffLine += "PCGF\t";
            //feature
            gffLine += "CDS\t";
            //start
            gffLine += this.Position + "\t";
            //end
            gffLine += (this.Position + this.Length - 1) + "\t";
            //score
            gffLine += ".\t";
            //strand
            gffLine += this.Strand + "\t";
            //frame
            gffLine += "0\t";
            //attributes
            gffLine += "gene_id \"" + this.Id + "\"; transcript_id \"" + this.Id + "\";";
            //comments
            return gffLine;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("metaData", metaData);
            info.AddValue("id", id);
            info.AddValue("idComplete", idComplete);
            info.AddValue("sequenceAsProtein", sequenceAsProtein);
            info.AddValue("length", length);
            info.AddValue("similarity", similarity);
            info.AddValue("mafSource", mafSource);
            info.AddValue("Chromosome", Chromosome);
            info.AddValue("position", position);
            info.AddValue("strand", strand);
            info.AddValue("numberRepetition", numberRepetition);
            info.AddValue("totalRepetitions", totalRepetitions);
            info.AddValue("flagUniqueSequence", flagUniqueSequence);
            info.AddValue("codingScore", codingScore);
            
            info.AddValue("numCodons", numCodons);
            info.AddValue("expandedCodingScores", expandedCodingScores);
            info.AddValue("expandedAnotation", expandedAnotation);
            info.AddValue("expandedConservation", expandedConservation);
            info.AddValue("kozakSequence", kozakSequence);
            info.AddValue("kozakScore", kozakScore);

            //New properties angel
            //info.AddValue("StartPosition", StartPosition);
            info.AddValue("annotationResult", annotationResult);
            info.AddValue("expandedResultTable", expandedResultTable);
            info.AddValue("condensedResultTable", condensedResultTable);
            info.AddValue("isInTheTable", isInTheTable);
            info.AddValue("rowRanges", rowRanges);
            info.AddValue("codingCodonsPercentage", codingCodonsPercentage);
            info.AddValue("sequence", sequence);
            info.AddValue("sequenceSecondSpecies", sequenceSecondSpecies);
            info.AddValue("sequenceThirdSpecies", sequenceThirdSpecies);
            info.AddValue("EspecieName1", EspecieName1);
            info.AddValue("EspecieName2", EspecieName2);
            info.AddValue("EspecieName3", EspecieName3);
            info.AddValue("transcritos", transcritos);
            

        }
        public smorf(SerializationInfo info, StreamingContext context)
        {
            metaData = (string)info.GetValue("metaData", typeof(string));
            id = (string)info.GetValue("id", typeof(string));
            idComplete = (string)info.GetValue("idComplete", typeof(string));
            length = (int)info.GetValue("length", typeof(int));
            similarity = (float)info.GetValue("similarity", typeof(float));
            mafSource = (string)info.GetValue("mafSource", typeof(string));
            Chromosome = (string)info.GetValue("Chromosome", typeof(string));
            position = (int)info.GetValue("position", typeof(int));
            strand = (string)info.GetValue("strand", typeof(string));
            numberRepetition = (int)info.GetValue("numberRepetition", typeof(int));
            totalRepetitions = (int)info.GetValue("totalRepetitions", typeof(int));
            flagUniqueSequence = (bool)info.GetValue("flagUniqueSequence", typeof(bool));
            codingScore = (float)info.GetValue("codingScore", typeof(float));
            codingCodonsPercentage = (float)info.GetValue("codingCodonsPercentage", typeof(float));
            numCodons = (int)info.GetValue("numCodons", typeof(int));
            expandedCodingScores = (float[])info.GetValue("expandedCodingScores", typeof(float[]));
            expandedAnotation = (string)info.GetValue("expandedAnotation", typeof(string));
            expandedConservation = (float[])info.GetValue("expandedConservation", typeof(float[]));
            kozakSequence = (string)info.GetValue("kozakSequence", typeof(string));
            kozakScore = (float)info.GetValue("kozakScore", typeof(float));

            //StartPosition = (int)info.GetValue("StartPosition", typeof(int));
            annotationResult = (List<List<List<string>>>)info.GetValue("annotationResult", typeof(List<List<List<string>>>));
            expandedResultTable = (List<List<string>>)info.GetValue("expandedResultTable", typeof(List<List<string>>));
            condensedResultTable = (List<int>)info.GetValue("condensedResultTable", typeof(List<int>));
            isInTheTable = (bool)info.GetValue("isInTheTable", typeof(bool));
            rowRanges = (List<int>)info.GetValue("rowRanges", typeof(List<int>));
            codingCodonsPercentage = (float)info.GetValue("codingCodonsPercentage", typeof(float));
            sequence = (string)info.GetValue("sequence", typeof(string));
            sequenceSecondSpecies = (string)info.GetValue("sequenceSecondSpecies", typeof(string));
            sequenceThirdSpecies = (string)info.GetValue("sequenceThirdSpecies", typeof(string));
            EspecieName1 = (string)info.GetValue("EspecieName1", typeof(string));
            EspecieName2 = (string)info.GetValue("EspecieName2", typeof(string));
            EspecieName3 = (string)info.GetValue("EspecieName3", typeof(string));
            ExpandedAnotation = (string)info.GetValue("expandedAnotation", typeof(string));
            ExpandedConservation = (float[])info.GetValue("expandedConservation", typeof(float[]));

            sequenceAsProtein = functions.DNAToProtein(sequence);
            GetSimilarities();
            /*  
             info.AddValue("codingCodonsPercentage", );
            info.AddValue("sequence", );
            info.AddValue("sequenceSecondSpecies", );
            info.AddValue("sequenceThirdSpecies", );*/
        }

        internal void WriteToFile(System.IO.StreamWriter outputStream, ParametersClass ConfigSetup)
        {
            outputStream.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
                                                MetaData,
                                                Id,
                                                ConfigSetup.specie1,
                                                Length.ToString(),
                                                Similarity.ToString(),
                                                MafSource,
                                                Chromosome,
                                                Position.ToString(),
                                                Strand,
                                                NumberRepetition.ToString(),
                                                TotalRepetitions.ToString(),
                                                FlagUniqueSequence.ToString(),
                                                CodingScore.ToString(),
                                                NumCodons.ToString()
                                                );
            for (int i = 0; i < NumCodons; i++)
            {
                outputStream.Write("{0}\t", ExpandedCodingScores[i].ToString());
            }
            if (coord == null) coord = new List<Coordinates>();
            outputStream.Write("{0}\t", coord.Count);
            for (int i = 0; i < coord.Count; i++)
            {
                outputStream.Write("{0}\t{1}\t{2}\t", coord[i].Chromosome, coord[i].Position, coord[i].Strand);
            }
            if (ExpandedConservation == null) ExpandedConservation = new float[0];
            outputStream.Write("{0}\t{1}\t{2}\t{3}\t{4}\t", ConservationAverage, ConservationPrevious, ConservationPosterior, flagConservation, ExpandedConservation.Count());
            for (int i = 0; i < ExpandedConservation.Count(); i++)
            {
                outputStream.Write("{0}\t", ExpandedConservation[i]);
            }
            outputStream.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t", OverlapExon, OverlapIntron, OverlapIntergenic, OverlapUtr5, OverlapUtr3, OverlapExonReverse, Annotation, ExpandedAnotation, ExpandedAnotationReverse, FlagOverlapExonEitherStrand, KozakSequence, KozakScore, GCcontent);
            outputStream.WriteLine();
            outputStream.WriteLine(Sequence);
            outputStream.WriteLine(">{0}", ConfigSetup.specie2);
            outputStream.WriteLine(SequenceSecondSpecies);
            outputStream.WriteLine(">{0}", ConfigSetup.specie3);
            outputStream.WriteLine(SequenceThirdSpecies);
        }

        //New class constructor Angel
        public smorf(string readline, int offset)
        {
            string[] tokens = readline.TrimStart('>').Split('\t');
            Position = int.Parse(tokens[0].Split('|')[3]);//angel
            EspecieName1 = tokens[2];//Angel
            int numTokens = tokens.Length;
            metaData = tokens[0];
            int val = int.Parse(tokens[1]) + offset;
            id = val.ToString();
            //mod.ConfigSetup.specie1 = tokens[2];
            length = int.Parse(tokens[3]);
            similarity = float.Parse(tokens[4]);
            mafSource = tokens[5];
            //sequence = tokens[6];
            Chromosome = tokens[6];
            position = int.Parse(tokens[7]);
            strand = tokens[8];
            numberRepetition = int.Parse(tokens[9]);
            totalRepetitions = int.Parse(tokens[10]);
            flagUniqueSequence = bool.Parse(tokens[11]);
            codingScore = float.Parse(tokens[12]);
            if (float.IsNaN(codingScore))
            {
                codingScore = -50;
            }
            numCodons = int.Parse(tokens[13]);
            expandedCodingScores = new float[numCodons];
            for (int i = 0; i < numCodons; i++)
            {
                expandedCodingScores[i] = float.Parse(tokens[14 + i]);
            }
            this.codingCodonsPercentage = expandedCodingScores.Average();
            if (numTokens == 15 + numCodons)
            {
                return;
            }
            int voy = 14 + numCodons;
            int numCord = int.Parse(tokens[voy++]);
            coord = new List<Coordinates>();
            for (int i = 0; i < numCord; i++)
            {
                string chr = tokens[voy++];
                int pos = int.Parse(tokens[voy++]);
                string str = tokens[voy++];
                /*
                if (!mod.ConfigSetup.genomeFiles.Contains(chr))
                {
                    mod.ConfigSetup.genomeFiles.Add(chr);
                    string[] ordenador = mod.ConfigSetup.genomeFiles.ToArray();
                    Array.Sort(ordenador, new NaturalComparer());
                    mod.ConfigSetup.genomeFiles.Clear();
                    mod.ConfigSetup.genomeFiles.AddRange(ordenador);
                }
                */
                Coordinates nuevasCoordenadas = new Coordinates() { Chromosome = chr, Position = pos, Strand = str };
                coord.Add(nuevasCoordenadas);
            }
            conservationAverage = float.Parse(tokens[voy++]);
            conservationPrevious = float.Parse(tokens[voy++]);
            conservationPosterior = float.Parse(tokens[voy++]);
            flagConservation = bool.Parse(tokens[voy++]);
            int numExpandedConservation = int.Parse(tokens[voy++]);
            if (numExpandedConservation > 0) expandedConservation = new float[numExpandedConservation];
            for (int i = 0; i < numExpandedConservation; i++)
            {
                expandedConservation[i] = float.Parse(tokens[voy++]);
            }
            overlapExon = float.Parse(tokens[voy++]);
            overlapIntron = float.Parse(tokens[voy++]);
            overlapIntergenic = float.Parse(tokens[voy++]);
            overlapUtr5 = float.Parse(tokens[voy++]);
            overlapUtr3 = float.Parse(tokens[voy++]);
            overlapExonReverse = float.Parse(tokens[voy++]);
            annotation = tokens[voy++];
            expandedAnotation = tokens[voy++];
            expandedAnotationReverse = tokens[voy++];
            flagOverlapExonEitherStrand = bool.Parse(tokens[voy++]);
            kozakSequence = tokens[voy++];
            KozakScore = float.Parse(tokens[voy++]);
            GCcontent = float.Parse(tokens[voy++]);
            GetSimilarities();
        }
    }
}
