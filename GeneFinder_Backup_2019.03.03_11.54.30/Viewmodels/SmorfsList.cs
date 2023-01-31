using GeneFinder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Viewmodels
{
    public class SmorfsList
    {
        public string Id { get; set; }
        public string Chromosome { get; set; }
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public int Length { get; set; }
        public int Similarity { get; set; }
        public string StartCodon { get; set; }
        public string StopCodon { get; set; }
        public string FileSource { get; set; }
        public string Specie1Content { get; set; }
        public string Specie2Content { get; set; }
        public string Specie3Content { get; set; }
        public string Strand { get; set; }
        public string Annotation { get; set; }
        public double Conservation { get; set; }
        public double Kozak { get; set; }
        public string KozakSequence { get; set; }
        public string Proteins { get; set; }
        public int AligmentNumber { get; set; }
        public int AligmentInstance { get; set; }
        public double CodingScore { get; set; }
        public smorf data { get; set; }
        public string Comment { get; set; }

        public string SimilarityStringSecond { get; set; }
        public string SimilarityStringThird { get; set; }
        public string GBCoordinates { get; set; }
        public int LengthAsProtein { get; set; }

        public bool IsIntergenic { get; set; }
        public int TranscriptNumber { get; set; }
        public int GenesNumber { get; set; }
        public int FiveUTRs { get; set; }
        public int ThreeUTRs { get; set; }
        public int Exons { get; set; }
        public int Introns { get; set; }
        public int Ambiguous { get; set; }
        public bool ExonInFrame { get; set; }



        public SmorfsList(smorf s)
        {
            data = s;
            Id = s.Id;
            if (s.Coord != null && s.Coord.Count > 0)
            {
                StartPosition = s.Coord[0].Position;
                Strand = s.Coord[0].Strand;
                Chromosome = s.Coord[0].Chromosome;
                AligmentNumber = s.Coord.Count;
                AligmentInstance = 1;
            }
            else
            {
                StartPosition = s.Position;
                Strand = s.Strand;
                Chromosome = s.Chromosome;
                AligmentNumber = 1;
                AligmentInstance = 1;
            }
            Specie1Content = s.Sequence;
            Specie2Content = s.SequenceSecondSpecies;
            Specie3Content = s.SequenceThirdSpecies;
            Length = Specie1Content.Length;
            EndPosition = StartPosition + Length;
            int sim = 0;
            for (int i = 0; i < Length; i++)
            {
                if (Specie1Content[i] == Specie2Content[i] && Specie2Content[i] == Specie3Content[i]) sim++;
            }
            Similarity = sim * 100 / Length;
            StartCodon = Specie1Content.Substring(0, 3);
            StopCodon = Specie1Content.Substring(Length - 3);
            FileSource = s.MafSource;
            Annotation = s.Annotation;
            Conservation = s.ConservationAverage;
            Kozak = s.KozakScore;
            KozakSequence = s.KozakSequence;
            Proteins = s.SequenceAsProtein;
            CodingScore = s.CodingScore;
            SimilarityStringSecond = s.SimilarityStringSecond;
            SimilarityStringThird = s.SimilarityStringThird;
            GBCoordinates = s.GBCoordinates;
            LengthAsProtein = s.LengthAsProtein;

            if (s.condensedResultTable == null)
            {
                GenesNumber = 0;
                TranscriptNumber = 0;
                FiveUTRs = 0;
                ThreeUTRs = 0;
                Exons = 0;
                Introns = 0;
                Ambiguous = 0;
                IsIntergenic = (TranscriptNumber == 0);
                ExonInFrame = false;
            }
            else
            {
                GenesNumber = s.condensedResultTable[0];
                TranscriptNumber = s.condensedResultTable[1] - s.condensedResultTable[6];
                FiveUTRs = s.condensedResultTable[2];
                ThreeUTRs = s.condensedResultTable[3];
                Exons = s.condensedResultTable[4];
                Introns = s.condensedResultTable[5];
                Ambiguous = s.condensedResultTable[7];
                IsIntergenic = (TranscriptNumber == 0);
                ExonInFrame = false;
                for (int i = 0; i < s.annotationResult.Count; i++)
                {
                    for (int j = 0; j < s.annotationResult[i].Count; j++)
                    {
                        if (s.annotationResult[i][j][5] == "0")
                        {
                            ExonInFrame = true;
                            break;
                        }
                    }
                    if (ExonInFrame) break;
                }
            }
            
        }

        internal static List<SmorfsList> Generar(smorf s)
        {
            List<SmorfsList> regresa = new List<SmorfsList>();
            if (s.Coord == null) s.Coord = new List<Coordinates>();
            for (int i = 0; i < s.Coord.Count; i++)
            {
                SmorfsList nuevo = new SmorfsList(s);
                if (i > 0)
                {
                    nuevo.StartPosition = s.Coord[i].Position;
                    nuevo.Strand = s.Coord[i].Strand;
                    nuevo.Chromosome = s.Coord[i].Chromosome;
                    nuevo.AligmentNumber = s.Coord.Count;
                    nuevo.AligmentInstance = 1 + i;
                }
            }
            return regresa;
        }
    }
}
