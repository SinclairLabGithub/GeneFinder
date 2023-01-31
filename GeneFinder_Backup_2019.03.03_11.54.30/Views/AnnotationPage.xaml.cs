using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for AnnotationPage.xaml
    /// </summary>
    public partial class AnnotationPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        static object lockObj = new object();
        internal MainWindow ventanaPrincipal;

        public AnnotationPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void deleteKozakButton_Click(object sender, RoutedEventArgs e)
        {
            if (listKozak.SelectedItem == null)
            {
                MessageBox.Show("Select an item to delete", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                listKozak.Items.Remove(listKozak.SelectedItem);
            }
        }

        private void addKozakButton_Click(object sender, RoutedEventArgs e)
        {
            string newKozak = kozakTextBox.Text;
            if (newKozak.Length != 15)
            {
                MessageBox.Show("Lenght must be 15 characters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string allowed = "ATGC";
                foreach (char c in newKozak)
                {
                    if (!allowed.Contains(c))
                    {
                        MessageBox.Show("Codon must only contains A, T, G or C", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                listKozak.Items.Add(newKozak);
            }
        }

        private void selectConservationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where conservation is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                modelo.ConfigSetup.conservationFolder = dialog.FileName;
                UpdateFolderPresentation();
                textBoxConservation.Text = dialog.FileName;
                
            }
            
        }

        private void UpdateFolderPresentation()
        {
            if (modelo.ConfigSetup.genomeFolder.Length > 0 && modelo.ConfigSetup.conservationFolder.Length > 0 && modelo.ConfigSetup.annotationBigTableName.Length > 0)
            {
                var files = Directory.GetFiles(modelo.ConfigSetup.genomeFolder, "*.fa");
                
                Array.Sort(files, new NaturalComparer());
                List<string> fileNames = new List<string>();
                foreach (var item in files)
                {
                    fileNames.Add(item.Substring(item.LastIndexOf('\\') + 1, item.LastIndexOf('.') - item.LastIndexOf('\\') - 1));
                }
                
                modelo.ConfigSetup.genomeFiles = fileNames;
                List<string> fileAllNames = new List<string>();
                foreach (var item in fileNames)
                {
                    string annotationPFile = modelo.ConfigSetup.annotationBigTableName + "\\" + item + "+.txt";
                    string annotationNFile = modelo.ConfigSetup.annotationBigTableName + "\\" + item + "-.txt";
                    string conservationFile = modelo.ConfigSetup.conservationFolder + "\\" + item + ".txt";
                    if (File.Exists(annotationNFile) && File.Exists(annotationPFile) && File.Exists(conservationFile))
                    {
                        fileAllNames.Add(item);
                    }
                }
                files = fileAllNames.ToArray();
                Array.Sort(files, new NaturalComparer());
                genomeFilesListBox.ItemsSource = files.ToList();
                genomeFilesListBox.SelectionHelper.AddToSelection(files.ToList());
                if (files.Length > 0 && modelo.smorfsList.Count > 0)
                {
                    annotateSequencesButton.IsEnabled = true;
                }

            }
        }

        private void selectAnnotationFolderButton_Click(object sender, RoutedEventArgs e)
        {
           CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where annotation is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                modelo.ConfigSetup.annotationBigTableName = dialog.FileName;
                textBoxAnnotation.Text = dialog.FileName;
                UpdateFolderPresentation();
            }
        }

        private void selectGenomeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where genome is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                modelo.ConfigSetup.genomeFolder = dialog.FileName;
                textBoxGenome.Text = dialog.FileName;
                UpdateFolderPresentation();
            }
        }

        private void genomeFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modelo.ConfigSetup == null)
            {
                modelo.ConfigSetup = new ParametersClass();
            }

            modelo.ConfigSetup.genomeFiles = genomeFilesListBox.SelectedItems.Cast<string>().ToList();
        }

        private void OpenFastaButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Genefinder files|*.gfn";
            openDialog.Multiselect = true;
            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                if (modelo.smorfsList == null) modelo.smorfsList = new List<smorf>();

                worker.DoWork += (s, ev) =>
                {
                    foreach (var archivo in openDialog.FileNames)
                    {
                        FileInfo file = new FileInfo(archivo);
                        if (modelo.ConfigSetup == null) modelo.ConfigSetup = new ParametersClass();
                        //bool error = false;
                        using (FileStream originalFileStream = file.OpenRead())
                        {
                            using (var sr = new StreamReader(originalFileStream))
                            {
                                modelo.ConfigSetup.ReadFileHeader(sr);
                                string lineaActual = "";
                                while (!sr.EndOfStream)
                                {
                                    lineaActual = sr.ReadLine();
                                    smorf nuevo = new smorf(lineaActual, modelo);
                                    lineaActual = sr.ReadLine();
                                    nuevo.Sequence = lineaActual;
                                    nuevo.SequenceAsProtein = functions.DNAToProtein(lineaActual);
                                    lineaActual = sr.ReadLine().TrimStart('>');
                                    if (modelo.smorfsList.Count == 0) modelo.ConfigSetup.specie2 = lineaActual;
                                    lineaActual = sr.ReadLine();
                                    nuevo.SequenceSecondSpecies = lineaActual;
                                    lineaActual = sr.ReadLine().TrimStart('>');
                                    if (modelo.smorfsList.Count == 0) modelo.ConfigSetup.specie3 = lineaActual;
                                    lineaActual = sr.ReadLine();
                                    nuevo.SequenceThirdSpecies = lineaActual;
                                    nuevo.GetSimilarities();
                                    modelo.smorfsList.Add(nuevo);
                                }





                                //PossibleSmorf nuevo = new PossibleSmorf(lineaActual);

                                //modelo.pSmorfsList.Add(nuevo);
                            }
                        }
                    }

                    
                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    UpdateCharts();
                    ShowSequencesButton.IsEnabled = true;
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
        }

        private void ShowSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Genefinder files|*.gfn";
            dialog.Title = "Save file";
            dialog.AddExtension = true;
            dialog.DefaultExt = "gfn";

            if (dialog.ShowDialog() == true)
            {
                using (var wr = new StreamWriter(dialog.FileName))
                {
                    modelo.ConfigSetup.AddFileHeader(wr);
                    foreach (var item in modelo.smorfsList)
                    {
                        item.WriteToFile(wr, modelo.ConfigSetup);
                    }
                }
            }

            
        }

        private void ToRNAButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void annotateSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            modelo.ConfigSetup.kozaks = listKozak.Items.Cast<string>().ToList();
            modelo.ConfigSetup.basesToCheck = (int)upStreamNumericUpDown.Value;
            var worker = new BackgroundWorker();
            busyIndicator.IsBusy = true;
            int conservationFlanqSize = modelo.ConfigSetup.basesToCheck;
            worker.DoWork += (sp, ev) =>
                {
                    modelo.smorfsList.RemoveAll(q => q.Sequence.Contains('-') || q.SequenceSecondSpecies.Contains('-') || q.SequenceThirdSpecies.Contains('-'));
                    int cuantos = modelo.smorfsList.Count;                    
                    for (int i = 0; i < cuantos; i++)
                    {
                        modelo.smorfsList[i].NumberRepetition = 0;
                        if (modelo.smorfsList[i].Coord.Count > 0)
                        {
                            modelo.smorfsList[i].Chromosome = modelo.smorfsList[i].Coord[0].Chromosome;
                            modelo.smorfsList[i].Strand = modelo.smorfsList[i].Coord[0].Strand;
                            modelo.smorfsList[i].Position = modelo.smorfsList[i].Coord[0].Position;
                            for (int j = 1; j < modelo.smorfsList[i].Coord.Count; j++)
                            {
                                smorf nuevo = new smorf(modelo.smorfsList[i]);
                                nuevo.NumberRepetition = j;
                                nuevo.Chromosome = modelo.smorfsList[i].Coord[j].Chromosome;
                                nuevo.Strand = modelo.smorfsList[i].Coord[j].Strand;
                                nuevo.Position = modelo.smorfsList[i].Coord[j].Position;
                                nuevo.Coord[0] = modelo.smorfsList[i].Coord[j];
                                nuevo.Coord[j] = modelo.smorfsList[i].Coord[0];
                                modelo.smorfsList.Add(nuevo);
                            }
                        }                        
                        
                    }
                    float threshold = modelo.ConfigSetup.conservationThreshold;
                    foreach (var item in modelo.ConfigSetup.genomeFiles)
                    {
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            busyIndicator.BusyContent = string.Format("Reading {0}.", item);

                        }));

                        char[] conservation = ConservationAnnotationKozak.LoadConservationFile(modelo.ConfigSetup.conservationFolder + "\\" + item + ".txt");
                        char[] annotationPositive = ConservationAnnotationKozak.LoadAnnotationFile(modelo.ConfigSetup.annotationBigTableName + "\\" + item + "+.txt");
                        char[] annotationNegative = ConservationAnnotationKozak.LoadAnnotationFile(modelo.ConfigSetup.annotationBigTableName + "\\" + item + "-.txt");
                        char[] chrSequence = functions.LoadFasta(modelo.ConfigSetup.genomeFolder + "\\" + item + ".fa");

                        string chr = item;
                        int llevo = 0;
                        cuantos = modelo.smorfsList.Count;
                        Parallel.For(0, cuantos,
                            iThread =>
                            {
                                smorf s = modelo.smorfsList[iThread];
                                //if (s.Coord.Count > 0 && s.Chromosome == chr)
                                if (s.Chromosome == chr)
                                {
                                    //s.Position = s.Coord[0].Position;
                                    //s.Strand = s.Coord[0].Strand;
                                    //conservation
                                    s.ExpandedConservation = new float[s.Length + 2 * conservationFlanqSize];

                                    //previous
                                    s.ConservationPrevious = 0;
                                    for (int i = 0; i < conservationFlanqSize; i++)
                                    {
                                        s.ExpandedConservation[i] = conservation[s.Position - conservationFlanqSize + i] - '0';
                                        s.ConservationPrevious += s.ExpandedConservation[i];
                                    }
                                    s.ConservationPrevious /= conservationFlanqSize;

                                    //sequence
                                    s.ConservationAverage = 0;
                                    for (int i = conservationFlanqSize; i < s.Length + conservationFlanqSize; i++)
                                    {
                                        s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                                        s.ConservationAverage += s.ExpandedConservation[i];
                                    }
                                    s.ConservationAverage /= s.Length;

                                    //posterior
                                    s.ConservationPosterior = 0;
                                    for (int i = s.Length + conservationFlanqSize; i < s.Length + 2 * conservationFlanqSize; i++)
                                    {
                                        s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                                        s.ConservationPosterior += s.ExpandedConservation[i];
                                    }
                                    s.ConservationPosterior /= conservationFlanqSize;

                                    //conservation flag
                                    s.FlagConservation = s.ConservationAverage > threshold && s.ConservationPrevious < s.ConservationAverage / 2 && s.ConservationPosterior < s.ConservationAverage / 2;

                                    //annotation
                                    s.ExpandedAnotation = "";
                                    s.ExpandedAnotationReverse = "";

                                    string tmpPreviousAnnotation = "";
                                    string tmpPosteriorAnnotation = "";
                                    string tmpPreviousAnnotationReverse = "";
                                    string tmpPosteriorAnnotationReverse = "";

                                    if (s.Strand == "+")
                                    {
                                        for (int i = 0; i < s.Length; i++)
                                        {
                                            s.ExpandedAnotation += annotationPositive[i + s.Position];
                                            s.ExpandedAnotationReverse += annotationNegative[i + s.Position];
                                        }
                                        for (int i = -conservationFlanqSize; i < 0; i++)
                                        {
                                            tmpPreviousAnnotation += annotationPositive[i + s.Position];
                                            tmpPreviousAnnotationReverse += annotationNegative[i + s.Position];
                                        }
                                        for (int i = 0; i < conservationFlanqSize; i++)
                                        {
                                            tmpPosteriorAnnotation += annotationPositive[i + s.Position + s.Length];
                                            tmpPosteriorAnnotationReverse += annotationNegative[i + s.Position + s.Length];
                                        }
                                    }
                                    if (s.Strand == "-")
                                    {
                                        for (int i = 0; i < s.Length; i++)
                                        {
                                            s.ExpandedAnotation += annotationNegative[i + s.Position];
                                            s.ExpandedAnotationReverse += annotationPositive[i + s.Position];
                                        }
                                        for (int i = -conservationFlanqSize; i < 0; i++)
                                        {
                                            tmpPreviousAnnotation += annotationNegative[i + s.Position];
                                            tmpPreviousAnnotationReverse += annotationPositive[i + s.Position];
                                        }
                                        for (int i = 0; i < conservationFlanqSize; i++)
                                        {
                                            tmpPosteriorAnnotation += annotationNegative[i + s.Position + s.Length];
                                            tmpPosteriorAnnotationReverse += annotationPositive[i + s.Position + s.Length];
                                        }
                                    }

                                    foreach (char c in s.ExpandedAnotation)
                                    {
                                        switch (c)
                                        {
                                            default:
                                                break;
                                            case '0':
                                                s.OverlapIntergenic++;
                                                break;
                                            case 'e':
                                                s.OverlapExon++;
                                                break;
                                            case 'i':
                                                s.OverlapIntron++;
                                                break;
                                            case '3':
                                                s.OverlapUtr3++;
                                                break;
                                            case '5':
                                                s.OverlapUtr5++;
                                                break;
                                        }
                                    }

                                    foreach (char c in s.ExpandedAnotationReverse)
                                    {
                                        if (c == 'e' || c == '3' || c == '5')
                                        {
                                            s.OverlapExonReverse++;
                                        }
                                    }
                                    s.OverlapIntergenic /= s.Length;
                                    s.OverlapExon /= s.Length;
                                    s.OverlapIntron /= s.Length;
                                    s.OverlapUtr3 /= s.Length;
                                    s.OverlapUtr5 /= s.Length;
                                    s.OverlapExonReverse /= s.Length;

                                    s.ExpandedAnotation = tmpPreviousAnnotation + s.ExpandedAnotation + tmpPosteriorAnnotation;
                                    s.ExpandedAnotationReverse = tmpPreviousAnnotationReverse + s.ExpandedAnotationReverse + tmpPosteriorAnnotationReverse;

                                    s.FlagOverlapExonEitherStrand = (s.OverlapExonReverse == 0) && (s.OverlapExon == 0);

                                    //ambiguous annotation
                                    string smorfAnnot = "A";
                                    if (s.OverlapExon != 0)
                                    {
                                        smorfAnnot += "-E";
                                    }
                                    if (s.OverlapIntron != 0)
                                    {
                                        smorfAnnot += "-I";
                                    }
                                    if (s.OverlapIntergenic != 0)
                                    {
                                        smorfAnnot += "-X";
                                    }
                                    if (s.OverlapUtr3 != 0)
                                    {
                                        smorfAnnot += "-3U";
                                    }
                                    if (s.OverlapUtr5 != 0)
                                    {
                                        smorfAnnot += "-5U";
                                    }

                                    //non ambiguous annotation
                                    if (s.OverlapExon == 1)
                                    {
                                        smorfAnnot = "E";
                                    }
                                    if (s.OverlapIntron == 1)
                                    {
                                        smorfAnnot = "I";
                                    }
                                    if (s.OverlapIntergenic == 1)
                                    {
                                        smorfAnnot = "X";
                                    }
                                    if (s.OverlapUtr3 == 1)
                                    {
                                        smorfAnnot = "3U";
                                    }
                                    if (s.OverlapUtr5 == 1)
                                    {
                                        smorfAnnot = "5U";
                                    }
                                    s.Annotation = smorfAnnot;

                                    //kozak
                                    string smorfKozak = "";
                                    if (s.Strand == "+")
                                    {
                                        smorfKozak = new string(chrSequence, (s.Position - 9 - 1), (9 + 6));
                                    }
                                    if (s.Strand == "-")
                                    {
                                        //smorfKozak = new string(fasta, (start - 9 - 1), (9 + 6));
                                        smorfKozak = new string(chrSequence, (s.Position + s.Length - 6 - 1), (9 + 6));
                                        smorfKozak = functions.ReverseComplement(smorfKozak);
                                    }
                                    s.KozakSequence = smorfKozak;
                                    s.KozakScore = ConservationAnnotationKozak.KozakScore(smorfKozak, modelo.ConfigSetup.kozaks) * (float)15;
                                    this.Dispatcher.Invoke((Action)(() =>
                                    {
                                        llevo++;
                                        busyIndicator.BusyContent = string.Format("Processing annotation {0} {1}.", item, ((int)(llevo * 100 / cuantos)).ToString());

                                    }));
                                }

                            }
                            );
                    }
                };


            worker.RunWorkerCompleted += (sp, ev) =>
            {


                if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
                {
                    UpdateCharts();
                    ShowSequencesButton.IsEnabled = true;
                    ToRNAButton.IsEnabled = true;
                }
                busyIndicator.IsBusy = false;
            };

            worker.RunWorkerAsync();

        }

        private void UpdateCharts()
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                conservationSeries.DataPoints.Clear();

                int universo = modelo.smorfsList.Count();

                //int conservedCheck = modelo.smorfsList.Count(p => p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved and check: {0} - {1}%", conservedCheck, conservedCheck * 100 / universo), Value = conservedCheck });
                //int conserved = modelo.smorfsList.Count(p => p.ConservationAverage > modelo.ConfigSetup.conservationThreshold && !p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved: {0} - {1}%", conserved, conserved * 100 / universo), Value = conserved });
                //int nonConserved = universo - conservedCheck - conserved;
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Non-conserved: {0} - {1}%", nonConserved, nonConserved * 100 / universo), Value = nonConserved });

                int conservedFirst = modelo.smorfsList.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3N: {0} - {1}%", conservedFirst, conservedFirst * 100 / universo), Value = conservedFirst });
                int conservedSecond = modelo.smorfsList.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3N: {0} - {1}%", conservedSecond, conservedSecond * 100 / universo), Value = conservedSecond });
                int conservedThird = modelo.smorfsList.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3N: {0} - {1}%", conservedThird, conservedThird * 100 / universo), Value = conservedThird });
                int conservedFourth = modelo.smorfsList.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3C: {0} - {1}%", conservedFourth, conservedFourth * 100 / universo), Value = conservedFourth });
                int conservedFifth = modelo.smorfsList.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3N: {0} - {1}%", conservedFifth, conservedFifth * 100 / universo), Value = conservedFifth });
                int conservedSixth = modelo.smorfsList.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3C: {0} - {1}%", conservedSixth, conservedSixth * 100 / universo), Value = conservedSixth });
                int conservedSepth = modelo.smorfsList.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3C: {0} - {1}%", conservedSepth, conservedSepth * 100 / universo), Value = conservedSepth });
                int conservedOcth = modelo.smorfsList.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3C: {0} - {1}%", conservedOcth, conservedOcth * 100 / universo), Value = conservedOcth });

                annotationSeries.DataPoints.Clear();
                int exon = modelo.smorfsList.Count(p => p.OverlapExon == 1);
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Exon: {0} - {1}%", exon, exon * 100 / universo), Value = exon });
                int intron = modelo.smorfsList.Count(p => p.OverlapIntron == 1);
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intron: {0} - {1}%", intron, intron * 100 / universo), Value = intron });
                int intergenic = modelo.smorfsList.Count(p => p.OverlapIntergenic == 1);
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intergenic: {0} - {1}%", intergenic, intergenic * 100 / universo), Value = intergenic });
                int cinco = modelo.smorfsList.Count(p => p.OverlapUtr5 == 1);
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5' utr: {0} - {1}%", cinco, cinco * 100 / universo), Value = cinco });
                int tres = modelo.smorfsList.Count(p => p.OverlapUtr3 == 1);
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("3' utr: {0} - {1}%", tres, tres * 100 / universo), Value = tres });
                int ambiguous = universo - exon - intron - intergenic - cinco - tres;
                annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Ambiguous: {0} - {1}%", ambiguous, ambiguous * 100 / universo), Value = ambiguous });


                seriesKozak.DataPoints.Clear();

                for (int i = 0; i <=15 ; i++)
                {
                    seriesKozak.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = i.ToString(), Value = modelo.smorfsList.Count(p => p.KozakScore == i) , Label = modelo.smorfsList.Count(p => p.KozakScore == i) });
                }

                graphGrid.Visibility = Visibility.Visible;
            }
        }

        private void textBoxThreshold_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (modelo.ConfigSetup != null && textBoxThreshold.Value != null)
            {
                modelo.ConfigSetup.conservationThreshold = (float)textBoxThreshold.Value;
            }
        }

        private void createConservationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select conservation File";
            dialog.Filter = "Wig files|*.wig";
            if (dialog.ShowDialog() == true)
            {
                ConservationAnnotationKozak.CreateConservationFile(dialog.FileName, dialog.FileName.Substring(0,dialog.FileName.LastIndexOf("//")));
            }
        }

        private void SelectAllFileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectAllExecuted(sender, e);
        }


        private void SelectAllExecuted(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < genomeFilesListBox.Items.Count; i++)
            {
                if (!genomeFilesListBox.SelectedItems.Contains(genomeFilesListBox.Items[i]))
                {
                    genomeFilesListBox.SelectedItems.Add(genomeFilesListBox.Items[i]);
                }
            }
        }

        private void SelectNoneExecuted(object sender, RoutedEventArgs e)
        {
            genomeFilesListBox.SelectedItems.Clear();
        }

        private void SelectNoneFileButton_Click(object sender, RoutedEventArgs e)
        {
            SelectNoneExecuted(sender, e);
        }

        private void createAnnotationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select genes file";
            dialog.Filter = "CSV files|*.csv";
            if (dialog.ShowDialog() == true)
            {
                string fileGenes = dialog.FileName;
                dialog.Title = "Select 3utr file";
                if (dialog.ShowDialog() == true)
                {
                    string tresUTRfile = dialog.FileName;
                    dialog.Title = "Select 5utr file";
                    if (dialog.ShowDialog() == true)
                    {
                        ConservationAnnotationKozak.CreateAnnotationFile("chr1","-",fileGenes,tresUTRfile,dialog.FileName,dialog.FileName.Substring(0,dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr2", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr3", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr4", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr5", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr6", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr7", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr8", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr9", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr10", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr11", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr12", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr13", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr14", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr15", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr16", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr17", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr18", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr19", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr20", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr21", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr22", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chrX", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chrY", "-", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr1", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr2", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr3", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr4", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr5", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr6", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr7", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr8", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr9", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr10", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr11", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr12", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr13", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr14", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr15", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr16", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr17", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr18", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr19", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr20", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr21", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chr22", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chrX", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                        ConservationAnnotationKozak.CreateAnnotationFile("chrY", "+", fileGenes, tresUTRfile, dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
                    }
                }
                
            }
        }
    }
}
