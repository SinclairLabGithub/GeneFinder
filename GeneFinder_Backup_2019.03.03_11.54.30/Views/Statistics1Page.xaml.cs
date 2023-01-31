using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
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
    /// Interaction logic for Statistics1Page.xaml
    /// </summary>
    public partial class Statistics1Page : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        internal MainWindow ventanaPrincipal;
        internal List<smorf> data;

        public Statistics1Page()
        {
            InitializeComponent();
        }

        private void filterSmorfButton_Click(object sender, RoutedEventArgs e)
        {
            SelectSmorfsWindow ventana = new SelectSmorfsWindow();
            ventana.GridSmorf.ItemsSource = modelo.smorfsList;
            //ventana.parameters = parameters;
            bool filtrado = (bool)ventana.ShowDialog();
            if (filtrado)
            {
                data = ventana.GridSmorf.Items.Cast<smorf>().ToList();
                UpdatePlots();
            }

        }

        private void UpdatePlots()
        {
            if (data == null)
            {
                data = new List<smorf>();
                data = modelo.smorfsList;
            }

            if (data.Count > 0)
            {

                numSelectedTextBox.Text = data.Count.ToString();
                numTotalTextBox.Text = modelo.smorfsList.Count.ToString();

                

                int universo = data.Count;

                //int conservedCheck = data.Count(p => p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved and check: {0} - {1}%", conservedCheck, conservedCheck * 100 / universo), Value = conservedCheck });
                //int conserved = data.Count(p => p.ConservationAverage > modelo.ConfigSetup.conservationThreshold && !p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved: {0} - {1}%", conserved, conserved * 100 / universo), Value = conserved });
                //int nonConserved = universo - conservedCheck - conserved;
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Non-conserved: {0} - {1}%", nonConserved, nonConserved * 100 / universo), Value = nonConserved });
                if (universo > 0)
                {
                    

                    foreach (var item in data)
                    {
                        if (float.IsNaN(item.CodingScore))
                        {
                            item.CodingScore = -50;
                        }
                    }

                    float minSimilarity = data.Min(q => q.CodingScore);
                    float maxSimilarity = data.Max(q => q.CodingScore);

                    horizontalAxisScore.Minimum = minSimilarity;
                    horizontalAxisScore.Maximum = maxSimilarity;

                    serieScore.DataPoints.Clear();

                    int numSteps = 200;
                    float step = (maxSimilarity - minSimilarity) / ((float)numSteps);
                    if (step < 1) step = 1;

                    for (float i = 100; i >= minSimilarity; i -= step)
                    {
                        serieScore.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.CodingScore <= i && q.CodingScore > i - step), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                    }

                    
                    classificationSeries.DataPoints.Clear();

                    int numPositive = data.Count(q => q.CodingScore >= modelo.ConfigSetup.cutoff);
                    classificationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Coding {0}%", numPositive * 100 / data.Count), Value = numPositive });
                    int numNegative = data.Count(q => q.CodingScore < modelo.ConfigSetup.cutoff);
                    classificationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("No Coding {0}%", numNegative * 100 / data.Count), Value = numNegative });


                    int minLength = data.Min(q => q.Length);
                    int maxLength = data.Max(q => q.Length);

                    horizontalAxisLength.Minimum = minLength;
                    horizontalAxisLength.Maximum = maxLength;

                    serieLength.DataPoints.Clear();

                    for (int i = minLength; i <= maxLength; i = i + 3)
                    {
                        serieLength.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.Length == i) });
                    }

                    var query = from smf in modelo.smorfsList
                                where smf.Id != null
                                group smf by smf.Chromosome
                                    into us
                                    select us.Key;

                    List<string> chromosomeList = query.ToList();
                    chromosomeList.Sort(new NaturalComparer());

                    heatMapsPanel.Children.Clear();

                    foreach (var item in chromosomeList)
                    {
                        HeatMapController mapaNuevo = new HeatMapController();
                        mapaNuevo.graphInstances = false;
                        var listaSeleccionado = data.Where(q => q.Chromosome == item).ToList();
                        mapaNuevo.titleLabel.Content = string.Format("{0} - {1}", item, listaSeleccionado.Count);
                        mapaNuevo.data = listaSeleccionado; //data.Where(q => q.Chromosome == item).ToList();
                        mapaNuevo.chr = item;
                        mapaNuevo.longitud = 0;
                        heatMapsPanel.Children.Add(mapaNuevo);
                    }
                }



            }


        }

        private void saveSelectionSmorfButton_Click(object sender, RoutedEventArgs e)
        {
            if (data.Count > 0)
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
                        foreach (var item in data)
                        {
                            item.WriteToFile(wr, modelo.ConfigSetup);
                        }
                    }
                }
            }

        }

        private void postProcessButton_Click(object sender, RoutedEventArgs e)
        {
            //modelo.ConfigSetup.conservationFolder = "F:\\Harvard\\Conservation";
            //modelo.ConfigSetup.annotationFolder = "F:\\Harvard\\Annotation";
            //modelo.ConfigSetup.genomeFolder = "F:\\Harvard\\Genome";

            modelo.ConfigSetup.loadFolders();

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
                //string annotationPFile = modelo.ConfigSetup.annotationFolder + "\\" + item + "+.txt";
                //string annotationNFile = modelo.ConfigSetup.annotationFolder + "\\" + item + "-.txt";
                string conservationFile = modelo.ConfigSetup.conservationFolder + "\\" + item + ".txt";
                if (File.Exists(conservationFile))//if (File.Exists(annotationNFile) && File.Exists(annotationPFile) && File.Exists(conservationFile))
                {
                    fileAllNames.Add(item);
                }
            }
            files = fileAllNames.ToArray();
            Array.Sort(files, new NaturalComparer());

            SelectChromosomes ventana = new SelectChromosomes();



            ventana.genomeFilesListBox.ItemsSource = files.ToList();
            ventana.genomeFilesListBox.SelectionHelper.AddToSelection(files.ToList());
            ventana.modelo = this.modelo;

            if ((bool)ventana.ShowDialog())
            {
                modelo.ConfigSetup.kozaks = ventana.listKozak.Items.Cast<string>().ToList();
                modelo.ConfigSetup.basesToCheck = (int)ventana.upStreamNumericUpDown.Value;
                var worker = new BackgroundWorker();
                busyIndicator.IsBusy = true;
                int conservationFlanqSize = modelo.ConfigSetup.basesToCheck;
                worker.DoWork += (sp, ev) =>
                {
                    //modelo.smorfsList.RemoveAll(q => q.Sequence.Contains('-') || q.SequenceSecondSpecies.Contains('-') || q.SequenceThirdSpecies.Contains('-'));
                    int cuantos = modelo.smorfsList.Count;
                    //for (int i = 0; i < cuantos; i++)
                    //{
                    //    modelo.smorfsList[i].NumberRepetition = 0;
                    //    if (modelo.smorfsList[i].Coord.Count > 0)
                    //    {
                    //        modelo.smorfsList[i].Chromosome = modelo.smorfsList[i].Coord[0].Chromosome;
                    //        modelo.smorfsList[i].Strand = modelo.smorfsList[i].Coord[0].Strand;
                    //        modelo.smorfsList[i].Position = modelo.smorfsList[i].Coord[0].Position;
                    //        for (int j = 1; j < modelo.smorfsList[i].Coord.Count; j++)
                    //        {
                    //            smorf nuevo = new smorf(modelo.smorfsList[i]);
                    //            nuevo.NumberRepetition = j;
                    //            nuevo.Chromosome = modelo.smorfsList[i].Coord[j].Chromosome;
                    //            nuevo.Strand = modelo.smorfsList[i].Coord[j].Strand;
                    //            nuevo.Position = modelo.smorfsList[i].Coord[j].Position;
                    //            nuevo.Coord[0] = modelo.smorfsList[i].Coord[j];
                    //            nuevo.Coord[j] = modelo.smorfsList[i].Coord[0];
                    //            modelo.smorfsList.Add(nuevo);
                    //        }
                    //    }

                    //}
                    float threshold = modelo.ConfigSetup.conservationThreshold;
                    functions.CalculateConservation(modelo.smorfsList, modelo.ConfigSetup.conservationFolder, conservationFlanqSize, modelo.ConfigSetup.genomeFiles);
                    functions.CalculateKozak(modelo.smorfsList, modelo.ConfigSetup.genomeFolder, modelo.ConfigSetup.genomeFiles, modelo.ConfigSetup.kozaks);

                    functions.CalculateAnnotation(modelo.smorfsList, modelo.ConfigSetup.annotationBigTableName, modelo.ConfigSetup.annotationSmallTableName);


                    //functions.CalculateAnnotation(modelo.smorfsList, modelo.ConfigSetup.annotationFolder);

                    //foreach (var item in modelo.ConfigSetup.genomeFiles)
                    //{
                    //    //this.Dispatcher.Invoke((Action)(() =>
                    //    //{
                    //    //    busyIndicator.BusyContent = string.Format("Reading {0}.", item);

                    //    //}));

                    //    //char[] conservation = ConservationAnnotationKozak.LoadConservationFile(modelo.ConfigSetup.conservationFolder + "\\" + item + ".txt");
                    //    char[] annotationPositive = ConservationAnnotationKozak.LoadAnnotationFile(modelo.ConfigSetup.annotationFolder + "\\" + item + "+.txt");
                    //    char[] annotationNegative = ConservationAnnotationKozak.LoadAnnotationFile(modelo.ConfigSetup.annotationFolder + "\\" + item + "-.txt");
                    //    //char[] chrSequence = functions.LoadFasta(modelo.ConfigSetup.genomeFolder + "\\" + item + ".fa");

                    //    //string chr = item;
                    //    //int llevo = 0;
                    //    //cuantos = modelo.smorfsList.Count;


                    //    //Parallel.For(0, cuantos,
                    //    //    iThread =>
                    //    //    {
                    //    //        smorf s = modelo.smorfsList[iThread];
                    //    //        //if (s.Coord.Count > 0 && s.Chromosome == chr)
                    //    //        if (s.Chromosome == chr)
                    //    //        {
                    //    //            ////s.Position = s.Coord[0].Position;
                    //    //            ////s.Strand = s.Coord[0].Strand;
                    //    //            ////conservation
                    //    //            //s.ExpandedConservation = new float[s.Length + 2 * conservationFlanqSize];

                    //    //            ////previous
                    //    //            //s.ConservationPrevious = 0;
                    //    //            //for (int i = 0; i < conservationFlanqSize; i++)
                    //    //            //{
                    //    //            //    s.ExpandedConservation[i] = conservation[s.Position - conservationFlanqSize + i] - '0';
                    //    //            //    s.ConservationPrevious += s.ExpandedConservation[i];
                    //    //            //}
                    //    //            //s.ConservationPrevious /= conservationFlanqSize;

                    //    //            ////sequence
                    //    //            //s.ConservationAverage = 0;
                    //    //            //for (int i = conservationFlanqSize; i < s.Length + conservationFlanqSize; i++)
                    //    //            //{
                    //    //            //    s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                    //    //            //    s.ConservationAverage += s.ExpandedConservation[i];
                    //    //            //}
                    //    //            //s.ConservationAverage /= s.Length;

                    //    //            ////posterior
                    //    //            //s.ConservationPosterior = 0;
                    //    //            //for (int i = s.Length + conservationFlanqSize; i < s.Length + 2 * conservationFlanqSize; i++)
                    //    //            //{
                    //    //            //    s.ExpandedConservation[i] = conservation[i + s.Position - conservationFlanqSize] - '0';
                    //    //            //    s.ConservationPosterior += s.ExpandedConservation[i];
                    //    //            //}
                    //    //            //s.ConservationPosterior /= conservationFlanqSize;

                    //    //            ////conservation flag
                    //    //            //s.FlagConservation = s.ConservationAverage > threshold && s.ConservationPrevious < s.ConservationAverage / 2 && s.ConservationPosterior < s.ConservationAverage / 2;

                    //    //            //annotation
                    //    //            s.ExpandedAnotation = "";
                    //    //            s.ExpandedAnotationReverse = "";

                    //    //            string tmpPreviousAnnotation = "";
                    //    //            string tmpPosteriorAnnotation = "";
                    //    //            string tmpPreviousAnnotationReverse = "";
                    //    //            string tmpPosteriorAnnotationReverse = "";

                    //    //            if (s.Strand == "+")
                    //    //            {
                    //    //                for (int i = 0; i < s.Length; i++)
                    //    //                {
                    //    //                    s.ExpandedAnotation += annotationPositive[i + s.Position];
                    //    //                    s.ExpandedAnotationReverse += annotationNegative[i + s.Position];
                    //    //                }
                    //    //                for (int i = -conservationFlanqSize; i < 0; i++)
                    //    //                {
                    //    //                    tmpPreviousAnnotation += annotationPositive[i + s.Position];
                    //    //                    tmpPreviousAnnotationReverse += annotationNegative[i + s.Position];
                    //    //                }
                    //    //                for (int i = 0; i < conservationFlanqSize; i++)
                    //    //                {
                    //    //                    tmpPosteriorAnnotation += annotationPositive[i + s.Position + s.Length];
                    //    //                    tmpPosteriorAnnotationReverse += annotationNegative[i + s.Position + s.Length];
                    //    //                }
                    //    //            }
                    //    //            if (s.Strand == "-")
                    //    //            {
                    //    //                for (int i = 0; i < s.Length; i++)
                    //    //                {
                    //    //                    s.ExpandedAnotation += annotationNegative[i + s.Position];
                    //    //                    s.ExpandedAnotationReverse += annotationPositive[i + s.Position];
                    //    //                }
                    //    //                for (int i = -conservationFlanqSize; i < 0; i++)
                    //    //                {
                    //    //                    tmpPreviousAnnotation += annotationNegative[i + s.Position];
                    //    //                    tmpPreviousAnnotationReverse += annotationPositive[i + s.Position];
                    //    //                }
                    //    //                for (int i = 0; i < conservationFlanqSize; i++)
                    //    //                {
                    //    //                    tmpPosteriorAnnotation += annotationNegative[i + s.Position + s.Length];
                    //    //                    tmpPosteriorAnnotationReverse += annotationPositive[i + s.Position + s.Length];
                    //    //                }
                    //    //            }

                    //    //            foreach (char c in s.ExpandedAnotation)
                    //    //            {
                    //    //                switch (c)
                    //    //                {
                    //    //                    default:
                    //    //                        break;
                    //    //                    case '0':
                    //    //                        s.OverlapIntergenic++;
                    //    //                        break;
                    //    //                    case 'e':
                    //    //                        s.OverlapExon++;
                    //    //                        break;
                    //    //                    case 'i':
                    //    //                        s.OverlapIntron++;
                    //    //                        break;
                    //    //                    case '3':
                    //    //                        s.OverlapUtr3++;
                    //    //                        break;
                    //    //                    case '5':
                    //    //                        s.OverlapUtr5++;
                    //    //                        break;
                    //    //                }
                    //    //            }

                    //    //            foreach (char c in s.ExpandedAnotationReverse)
                    //    //            {
                    //    //                if (c == 'e' || c == '3' || c == '5')
                    //    //                {
                    //    //                    s.OverlapExonReverse++;
                    //    //                }
                    //    //            }
                    //    //            s.OverlapIntergenic /= s.Length;
                    //    //            s.OverlapExon /= s.Length;
                    //    //            s.OverlapIntron /= s.Length;
                    //    //            s.OverlapUtr3 /= s.Length;
                    //    //            s.OverlapUtr5 /= s.Length;
                    //    //            s.OverlapExonReverse /= s.Length;

                    //    //            s.ExpandedAnotation = tmpPreviousAnnotation + s.ExpandedAnotation + tmpPosteriorAnnotation;
                    //    //            s.ExpandedAnotationReverse = tmpPreviousAnnotationReverse + s.ExpandedAnotationReverse + tmpPosteriorAnnotationReverse;

                    //    //            s.FlagOverlapExonEitherStrand = (s.OverlapExonReverse == 0) && (s.OverlapExon == 0);

                    //    //            //ambiguous annotation
                    //    //            string smorfAnnot = "A";
                    //    //            if (s.OverlapExon != 0)
                    //    //            {
                    //    //                smorfAnnot += "-E";
                    //    //            }
                    //    //            if (s.OverlapIntron != 0)
                    //    //            {
                    //    //                smorfAnnot += "-I";
                    //    //            }
                    //    //            if (s.OverlapIntergenic != 0)
                    //    //            {
                    //    //                smorfAnnot += "-X";
                    //    //            }
                    //    //            if (s.OverlapUtr3 != 0)
                    //    //            {
                    //    //                smorfAnnot += "-3U";
                    //    //            }
                    //    //            if (s.OverlapUtr5 != 0)
                    //    //            {
                    //    //                smorfAnnot += "-5U";
                    //    //            }

                    //    //            //non ambiguous annotation
                    //    //            if (s.OverlapExon == 1)
                    //    //            {
                    //    //                smorfAnnot = "E";
                    //    //            }
                    //    //            if (s.OverlapIntron == 1)
                    //    //            {
                    //    //                smorfAnnot = "I";
                    //    //            }
                    //    //            if (s.OverlapIntergenic == 1)
                    //    //            {
                    //    //                smorfAnnot = "X";
                    //    //            }
                    //    //            if (s.OverlapUtr3 == 1)
                    //    //            {
                    //    //                smorfAnnot = "3U";
                    //    //            }
                    //    //            if (s.OverlapUtr5 == 1)
                    //    //            {
                    //    //                smorfAnnot = "5U";
                    //    //            }
                    //    //            s.Annotation = smorfAnnot;

                    //    //            ////kozak
                    //    //            //string smorfKozak = "";
                    //    //            //if (s.Strand == "+")
                    //    //            //{
                    //    //            //    smorfKozak = new string(chrSequence, (s.Position - 9 - 1), (9 + 6));
                    //    //            //}
                    //    //            //if (s.Strand == "-")
                    //    //            //{
                    //    //            //    //smorfKozak = new string(fasta, (start - 9 - 1), (9 + 6));
                    //    //            //    smorfKozak = new string(chrSequence, (s.Position + s.Length - 6 - 1), (9 + 6));
                    //    //            //    smorfKozak = functions.ReverseComplement(smorfKozak);
                    //    //            //}
                    //    //            //s.KozakSequence = smorfKozak;
                    //    //            //s.KozakScore = ConservationAnnotationKozak.KozakScore(smorfKozak, modelo.ConfigSetup.kozaks) * (float)15;
                    //    //            //this.Dispatcher.Invoke((Action)(() =>
                    //    //            //{
                    //    //            //    llevo++;
                    //    //            //    busyIndicator.BusyContent = string.Format("Processing annotation {0} {1}.", item, ((int)(llevo * 100 / cuantos)).ToString());

                    //    //            //}));
                    //    //        }

                    //    //    }
                    //    //    );
                    //}
                };

                worker.RunWorkerCompleted += (sp, ev) =>
                {


                    if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
                    {
                        ventanaPrincipal.goToStatistics();
                        
                    }
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
            

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                UpdatePlots();
            }
        }

        private void BackGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ventanaPrincipal.startOver();
        }

        private void selectionScoreButton_Click(object sender, RoutedEventArgs e)
        {
            SelectScoreWindow ventana = new SelectScoreWindow();
            ventana.data = this.data;
            ventana.Show();
        }
    }
}
