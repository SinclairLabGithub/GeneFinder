using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for WelcomePage.xaml
    /// </summary>
    public partial class WelcomePage : Page
    {
        internal List<GroupReads> paragraphs = new List<GroupReads>();
        internal List<GroupReads> paragraphsSelected = new List<GroupReads>();
        internal List<string> species = new List<string>();
        internal string srcName = "";
        internal ParametersClass parameters;
        internal List<PossibleSmorf> pSmorfsList;
        internal MainWindow ventanaPrincipal;

        public WelcomePage()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ventanaPrincipal.goToExtractor();

        }

        private void Grid_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {

            //OpenTestSmorf();
            //ventanaPrincipal.model.ConfigSetup = new ParametersClass();
            //ventanaPrincipal.goToEstadisticasPrimera();
            //return;

            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Genefinder files|*.gfn|Binary files|*.bin|XML files|*.xml|Genefinder 2 files|*.gfn2";
            openDialog.Multiselect = true;
            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                if (ventanaPrincipal.model.smorfsList == null) ventanaPrincipal.model.smorfsList = new List<smorf>();

                if (openDialog.FileNames[0].EndsWith(".xml"))
                {
                    worker.DoWork += (s, ev) =>
                    {
                        ventanaPrincipal.model = new CompleteViewModel();
                        foreach (var archivoEnLista in openDialog.FileNames)
                        {
                            var stream = File.Open(archivoEnLista, FileMode.Open);
                            try
                            {
                                var bf = new XmlSerializer(typeof(List<smorf>));
                                var reader = XmlReader.Create(stream);
                                List<smorf> tempSmorfs = bf.Deserialize(reader) as List<smorf>;
                                ventanaPrincipal.model.smorfsList.AddRange(tempSmorfs);
                                //while (true)
                                //{
                                    
                                //    //smorf tempSmorf = bf.Deserialize(reader) as smorf;
                                //    //ventanaPrincipal.model.smorfsList.Add(tempSmorf);
                                //    if (stream.Length == stream.Position)
                                //        break;
                                //}
                            }
                            catch (Exception exc)
                            {

                                throw exc;
                            }
                            
                            //bf.Binder = new typeconvertor();
                            
                            stream.Close();
                        }

                        
                    };

                    worker.RunWorkerCompleted += (s, ev) =>
                    {
                        ventanaPrincipal.model.ConfigSetup = new ParametersClass();
                        ventanaPrincipal.goToEstadisticasPrimera();
                    };

                    worker.RunWorkerAsync();
                    return;
                }
                
                if (openDialog.FileNames[0].EndsWith(".gfn2"))
                {
                    worker.DoWork += (s, ev) =>
                    {
                        ventanaPrincipal.model = new CompleteViewModel();
                        foreach (var archivoEnLista in openDialog.FileNames)
                        {
                            var stream = File.Open(archivoEnLista, FileMode.Open, FileAccess.Read, FileShare.Read);
                            try
                            {
                                //var bf = new XmlSerializer(typeof(CompleteViewModel));
                                //var reader = XmlReader.Create(stream);
                                

                                IFormatter formatter = new BinaryFormatter();

                                //CompleteViewModel tempModel = formatter.Deserialize(stream) as CompleteViewModel;
                                //ventanaPrincipal.model= tempModel;

                                ParametersClass tempConfig = formatter.Deserialize(stream) as ParametersClass;
                                ventanaPrincipal.model.ConfigSetup = tempConfig;
                                int tempNumSmorf = (int)formatter.Deserialize(stream);

                                for (int i = 0; i < tempNumSmorf; i++)
                                {
                                    smorf tempSmorf = formatter.Deserialize(stream) as smorf;
                                    ventanaPrincipal.model.smorfsList.Add(tempSmorf);
                                }

                                //while (true)
                                //{

                                //    //smorf tempSmorf = bf.Deserialize(reader) as smorf;
                                //    //ventanaPrincipal.model.smorfsList.Add(tempSmorf);
                                //    if (stream.Length == stream.Position)
                                //        break;
                                //}
                            }
                            catch (Exception exc)
                            {

                                throw exc;
                            }

                            //bf.Binder = new typeconvertor();

                            stream.Close();
                        }


                    };

                    worker.RunWorkerCompleted += (s, ev) =>
                    {
                        ventanaPrincipal.model.ConfigSetup = new ParametersClass();
                        ventanaPrincipal.goToStatistics();
                    };

                    worker.RunWorkerAsync();
                    return;
                }

                worker.DoWork += (s, ev) =>
                {
                    foreach (var archivo in openDialog.FileNames)
                    {
                        FileInfo file = new FileInfo(archivo);
                        if (ventanaPrincipal.model.ConfigSetup == null) ventanaPrincipal.model.ConfigSetup = new ParametersClass();
                        //bool error = false;
                        using (FileStream originalFileStream = file.OpenRead())
                        {
                            using (var sr = new StreamReader(originalFileStream))
                            {
                                ventanaPrincipal.model.ConfigSetup.ReadFileHeader(sr);
                                string lineaActual = "";
                                while (!sr.EndOfStream)
                                {
                                    lineaActual = sr.ReadLine();
                                    smorf nuevo = new smorf(lineaActual, ventanaPrincipal.model);
                                    lineaActual = sr.ReadLine();
                                    nuevo.Sequence = lineaActual;
                                    nuevo.SequenceAsProtein = functions.DNAToProtein(lineaActual);
                                    nuevo.GCcontent1 = functions.GetGCcontent(lineaActual);
                                    lineaActual = sr.ReadLine().TrimStart('>');
                                    if (ventanaPrincipal.model.smorfsList.Count == 0) ventanaPrincipal.model.ConfigSetup.specie2 = lineaActual;
                                    lineaActual = sr.ReadLine();
                                    nuevo.SequenceSecondSpecies = lineaActual;
                                    lineaActual = sr.ReadLine().TrimStart('>');
                                    if (ventanaPrincipal.model.smorfsList.Count == 0) ventanaPrincipal.model.ConfigSetup.specie3 = lineaActual;
                                    lineaActual = sr.ReadLine();
                                    nuevo.SequenceThirdSpecies = lineaActual;
                                    nuevo.GetSimilarities();
                                    ventanaPrincipal.model.smorfsList.Add(nuevo);
                                }





                                //PossibleSmorf nuevo = new PossibleSmorf(lineaActual);

                                //modelo.pSmorfsList.Add(nuevo);
                            }
                        }
                    }


                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    smorf prueba = ventanaPrincipal.model.smorfsList[0];
                    busyIndicator.IsBusy = false;
                    if (prueba.ExpandedAnotation.Length == 0)
                    {
                        ventanaPrincipal.goToEstadisticasPrimera();
                    }
                    else
                    {
                        ventanaPrincipal.goToStatistics();
                    }
                };

                worker.RunWorkerAsync();
            }
        }

        private void OpenTestSmorf()
        {
            List<smorf> tempSmorfs = new List<smorf>();
            string tempSequence = "";
            int tempNumCodons = 0;
            float[] tempCoding;
            string tempStrand;
            int tempPosition = 0;

            tempSequence = "CCCG" + GeneraSequences(13) + "CGTCACA";
            tempStrand = "-";
            tempPosition = 46176472;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++){ tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "2", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GCGTT" + GeneraSequences(88) + "AGGAGG";
            tempStrand = "-";
            tempPosition = 46077515;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "3", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            //tempSequence = "GCGTT" + GeneraSequences(88) + "AGGAG";
            //tempStrand = "-";
            //tempPosition = 46077515;
            //tempNumCodons = tempSequence.Length / 3 + 1;
            //tempCoding = new float[tempNumCodons];
            //for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            //tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "4", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            //tempSequence = "GCGTT" + GeneraSequences(89) + "AGGA";
            //tempStrand = "-";
            //tempPosition = 46077515;
            //tempNumCodons = tempSequence.Length / 3;
            //tempCoding = new float[tempNumCodons];
            //for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            //tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "5", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            //tempSequence = "GAAC" + GeneraSequences(101) + "TCTC";
            //tempStrand = "-";
            //tempPosition = 46080642;
            //tempNumCodons = tempSequence.Length / 3;
            //tempCoding = new float[tempNumCodons];
            //for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            //tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "6", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GAAC" + GeneraSequences(100) + "TTCT";
            tempStrand = "-";
            tempPosition = 46080642;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "7", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GACC" + GeneraSequences(118) + "CTAG";
            tempStrand = "-";
            tempPosition = 46066054;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "9", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GACA" + GeneraSequences(10415) + "AAT";
            tempStrand = "-";
            tempPosition = 46067092;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "12", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "AATG" + GeneraSequences(3525) + "GA";
            tempStrand = "-";
            tempPosition = 46040140;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "13", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "TAAT" + GeneraSequences(3526) + "GAGAAATA" + GeneraSequences(191) + "CCG";
            tempStrand = "-";
            tempPosition = 46040139;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "14", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GGAA" + GeneraSequences(237) + "GGTACCG" + GeneraSequences(16) + "CAC";
            tempStrand = "-";
            tempPosition = 46176227;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "15", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GGACA" + GeneraSequences(173) + "GAAGGACA" + GeneraSequences(10415) + "AATGGCGT" + GeneraSequences(91) + "GAGG";
            tempStrand = "-";
            tempPosition = 46066910;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "16", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "TAAT" + GeneraSequences(3526) + "GAGAAATA" + GeneraSequences(191) + "CCGGGACG" + GeneraSequences(2036) + "GA";
            tempStrand = "-";
            tempPosition = 46040139;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "17", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "TAATG" + GeneraSequences(3525) + "GAGAAATA" + GeneraSequences(191) + "CCGGGACG" + GeneraSequences(2038) + "AATGTGGT" + GeneraSequences(163) + "CAA";
            tempStrand = "-";
            tempPosition = 46040139;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "19", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GAA" + GeneraSequences(178) + "CTAGAAACAG" + GeneraSequences(7330) + "TTAGATATAGAACCC";
            tempStrand = "+";
            tempPosition = 78620381;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "20", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "AAAGTTAGTG" + GeneraSequences(170) + "CTAGAAACA" + GeneraSequences(7330) + "TTAGATATAGACCC";
            tempStrand = "+";
            tempPosition = 78620382;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "21", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GAAAGT" + GeneraSequences(180) + "ACAGGTAA" + GeneraSequences(7326) + "TTAGATATAGAACAATGG" + GeneraSequences(470) + "GAAGGTTT" + GeneraSequences(549) + "ATA";
            tempStrand = "+";
            tempPosition = 78620381;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "43", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            tempSequence = "GGTCA" + GeneraSequences(167) + "ACTGGTAA" + GeneraSequences(157) + "TCAGGTGC" + GeneraSequences(27) + "TTGAGATAA" + GeneraSequences(4327) + "CCAAA";
            tempStrand = "+";
            tempPosition = 78641434;
            tempNumCodons = tempSequence.Length / 3;
            tempCoding = new float[tempNumCodons];
            for (int i = 0; i < tempNumCodons; i++) { tempCoding[i] = 0; }
            tempSmorfs.Add(new smorf() { Strand = tempStrand, Chromosome = "chr1", Id = "44", Position = tempPosition, Length = tempSequence.Length, Sequence = tempSequence, SequenceSecondSpecies = tempSequence, SequenceThirdSpecies = tempSequence, NumCodons = tempNumCodons, Similarity = 100, SimilaritySecond = 1, SimilarityThird = 1, ExpandedCodingScores = tempCoding });

            ventanaPrincipal.model.smorfsList.AddRange(tempSmorfs);
        }

        private string GeneraSequences(int Length)
        {
            string solution = new string('A', Length);
            return solution;
        }

        private void Grid_MouseLeftButtonDown_2(object sender, MouseButtonEventArgs e)
        {
            ventanaPrincipal.goToSettings();
        }


    }
}
