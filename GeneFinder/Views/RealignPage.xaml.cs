using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
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
    /// Interaction logic for RealignPage.xaml
    /// </summary>
    public partial class RealignPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        static object lockObj = new object();
        internal MainWindow ventanaPrincipal;

        public RealignPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0 && modelo.ConfigSetup.genomeFolder.Length > 0 && modelo.ConfigSetup.genomeFiles.Count > 0)
            {
                realignSequencesButton.IsEnabled = true;
            }
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                UpdatePlots();
            }
        }

        private void OpenFastaButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "Genefinder files|*.gfn";

            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                if (modelo.smorfsList == null) modelo.smorfsList = new List<smorf>();

                worker.DoWork += (s, ev) =>
                {
                    FileInfo file = new FileInfo(openDialog.FileName);
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
                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    UpdatePlots();
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
        }

        private void realignSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            

            foreach (var item in modelo.ConfigSetup.genomeFiles)
            {
                string nameCheck = modelo.ConfigSetup.genomeFolder + "\\" + item + ".bin";
                if (!File.Exists(nameCheck))
                {
                    MessageBox.Show(string.Format("The Index file of {0} is not found.", item), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
            }
            
            var worker = new BackgroundWorker();
            busyIndicator.IsBusy = true;
            List<int> longitudes = new List<int>();
            worker.DoWork += (sp, ev) =>
                {
                    foreach (var item in modelo.ConfigSetup.genomeFiles)
                    {
                        string fastaFilename = modelo.ConfigSetup.genomeFolder + "\\" + item + ".fa";
                        string indexFilename = modelo.ConfigSetup.genomeFolder + "\\" + item + ".bin";
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            busyIndicator.BusyContent = string.Format("Reading {0}.", item);
                            
                        }));
                        var realigner = new Realigner(fastaFilename, indexFilename);
                        //realigner.Process(inputFilename, outputFilename);
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            busyIndicator.BusyContent = string.Format("Aligning {0}.", item);

                        }));
                        longitudes.Add(realigner.reference[0].Length);
                        realigner.Process(modelo.smorfsList);
                        
                    }
                };

            worker.RunWorkerCompleted += (sp, ev) =>
            {
                UpdatePlots(longitudes);

                if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
                {
                    ShowSequencesButton.IsEnabled = true;
                    ToAnnotationButton.IsEnabled = true;
                }
                busyIndicator.IsBusy = false;
            };

            worker.RunWorkerAsync();
        }

        private void UpdatePlots(List<int> longitudes)
        {
            int llevo = 0;
            serieChromosome.DataPoints.Clear();
            int max = 0;
            foreach (var item in modelo.ConfigSetup.genomeFiles)
            {
                HeatMapController mapaNuevo = new HeatMapController();
                var listaSeleccionado = modelo.smorfsList.Where(q => q.Coord.Any(t => t.Chromosome == item)).ToList();
                mapaNuevo.titleLabel.Content = string.Format("{0} - {1}", item, listaSeleccionado.Count);
                mapaNuevo.data = listaSeleccionado; //modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
                mapaNuevo.chr = item;
                if (longitudes.Count > llevo)
                {
                    mapaNuevo.longitud = longitudes[llevo++];
                }
                else
                {
                    mapaNuevo.longitud = 0;
                }
                heatMapsPanel.Children.Add(mapaNuevo);
                if (listaSeleccionado.Count > max) max = listaSeleccionado.Count;
                verticalAxisChromosomes.Maximum = max;
                verticalAxisChromosomes.MajorStep = max / 4;
                serieChromosome.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item, Value= listaSeleccionado.Count, Label = listaSeleccionado.Count.ToString() });
            }
            graphGrid.Visibility = Visibility.Visible;
        }

        private void UpdatePlots()
        {

            serieChromosome.DataPoints.Clear();
            int max = 0;
            heatMapsPanel.Children.Clear();

            var query = from smf in modelo.smorfsList
                        where smf.Id != null
                        group smf by smf.Chromosome
                            into us
                            select us.Key;

            List<string> chromosomeList = query.ToList();

            chromosomeList.Sort(new NaturalComparer());

            foreach (var item in chromosomeList)
            {
                HeatMapController mapaNuevo = new HeatMapController();
                mapaNuevo.graphInstances = false;
                var listaSeleccionado = modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
                int cuantosReales = listaSeleccionado.Count();
                //foreach (var instancia in listaSeleccionado)
                //{
                //    cuantosReales += instancia.Coord.Count(t => t.Chromosome == item);
                //}
                mapaNuevo.titleLabel.Content = string.Format("{0} - {1}", item, cuantosReales);
                mapaNuevo.data = listaSeleccionado; //modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
                mapaNuevo.chr = item;

                heatMapsPanel.Children.Add(mapaNuevo);
                if (listaSeleccionado.Count > max) max = listaSeleccionado.Count;
                verticalAxisChromosomes.Maximum = max;
                verticalAxisChromosomes.MajorStep = max / 4;
                serieChromosome.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item, Value = listaSeleccionado.Count, Label = listaSeleccionado.Count.ToString() });
            }
            graphGrid.Visibility = Visibility.Visible;

            //foreach (var item in modelo.ConfigSetup.genomeFiles)
            //{
            //    HeatMapController mapaNuevo = new HeatMapController();
            //    mapaNuevo.titleLabel.Content = item;
            //    mapaNuevo.data = modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
            //    heatMapsPanel.Children.Add(mapaNuevo);
            //}
        }

        private void UpdatePlotsAnterior()
        {
            
            serieChromosome.DataPoints.Clear();
            int max = 0;
            heatMapsPanel.Children.Clear();
            foreach (var item in modelo.ConfigSetup.genomeFiles)
            {
                HeatMapController mapaNuevo = new HeatMapController();
                var listaSeleccionado = modelo.smorfsList.Where(q => q.Coord.Any(t => t.Chromosome == item)).ToList();
                int cuantosReales = 0;
                foreach (var instancia in listaSeleccionado)
                {
                    cuantosReales += instancia.Coord.Count(t => t.Chromosome == item);
                }
                mapaNuevo.titleLabel.Content = string.Format("{0} - {1}", item, cuantosReales);
                mapaNuevo.data = listaSeleccionado; //modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
                mapaNuevo.chr = item;
                
                heatMapsPanel.Children.Add(mapaNuevo);
                if (listaSeleccionado.Count > max) max = listaSeleccionado.Count;
                verticalAxisChromosomes.Maximum = max;
                verticalAxisChromosomes.MajorStep = max / 4;
                serieChromosome.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item, Value = listaSeleccionado.Count, Label = listaSeleccionado.Count.ToString() });
            }
            graphGrid.Visibility = Visibility.Visible;

            //foreach (var item in modelo.ConfigSetup.genomeFiles)
            //{
            //    HeatMapController mapaNuevo = new HeatMapController();
            //    mapaNuevo.titleLabel.Content = item;
            //    mapaNuevo.data = modelo.smorfsList.Where(q => q.Chromosome == item).ToList();
            //    heatMapsPanel.Children.Add(mapaNuevo);
            //}
        }

        private void ShowSequencesButton_Click2(object sender, RoutedEventArgs e)
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

                    foreach (var item in modelo.smorfsList)
                    {
                        if (item.Coord.Count > 0)
                        {
                            wr.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
                                item.MetaData,
                                item.Id,
                                modelo.ConfigSetup.specie1,
                                item.Length.ToString(),
                                item.Similarity.ToString(), 
                                item.MafSource,
                                item.Chromosome,
                                item.Position.ToString(),
                                item.Strand,
                                item.NumberRepetition.ToString(),
                                item.TotalRepetitions.ToString(),
                                item.FlagUniqueSequence.ToString(),
                                item.CodingScore.ToString(),
                                item.NumCodons.ToString()
                                                    );
                            for (int i = 0; i < item.NumCodons; i++)
                            {
                                wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
                            }
                            wr.Write("{0}\t", item.Coord.Count);
                            for (int i = 0; i < item.Coord.Count; i++)
                            {
                                wr.Write("{0}\t{1}\t{2}\t", item.Coord[i].Chromosome, item.Coord[i].Position, item.Coord[i].Strand);
                            }
                            wr.Write("{0}\t{1}\t{2}\t{3}\t",
                                item.ConservationAverage,
                                item.ConservationPrevious,
                                item.ConservationPosterior,
                                item.FlagConservation
                                );
                            if (item.ExpandedConservation!=null)
                            {
                                wr.Write("{0}\t",item.ExpandedConservation.Length);
                                for (int i = 0; i < item.ExpandedConservation.Length; i++)
                                {
                                    wr.Write("{0}\t", item.ExpandedConservation[i].ToString());
                                }
                            }
                            else
                            {
                                wr.Write("0\t");
                            }
                            
                            wr.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t",
                                item.OverlapExon,
                                item.OverlapIntron,
                                item.OverlapIntergenic,
                                item.OverlapUtr5,
                                item.OverlapUtr3,
                                item.OverlapExonReverse,
                                item.Annotation,
                                item.ExpandedAnotation,
                                item.ExpandedAnotationReverse,
                                item.FlagOverlapExonEitherStrand,
                                item.KozakSequence,
                                item.KozakScore,
                                item.GCcontent1
                                );

                            wr.WriteLine();
                            wr.WriteLine(item.Sequence);
                            wr.WriteLine(">{0}", modelo.ConfigSetup.specie2);
                            //wr.WriteLine(">Chimp");
                            wr.WriteLine(item.SequenceSecondSpecies);
                            wr.WriteLine(">{0}", modelo.ConfigSetup.specie3);
                            //wr.WriteLine(">Rhesus");
                            wr.WriteLine(item.SequenceThirdSpecies);
                        }
                    }
                }
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

        private void selectGenomeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where genome is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok && dialog.FileName != null)
            {
                var files = Directory.GetFiles(dialog.FileName, "*.fa");
                textBoxGenome.Text = dialog.FileName;
                Array.Sort(files, new NaturalComparer());
                List<string> fileNames = new List<string>();
                foreach (var item in files)
                {
                    string temp = item.Substring(item.LastIndexOf('\\') + 1);
                    fileNames.Add(temp.Substring(0, temp.LastIndexOf('.')));
                }
                genomeFilesListBox.ItemsSource = fileNames;
                genomeFilesListBox.SelectionHelper.AddToSelection(fileNames);
                modelo.ConfigSetup.genomeFolder = dialog.FileName;
                modelo.ConfigSetup.genomeFiles = fileNames;
            }
            CheckNextStep();
        }

        private void CheckNextStep()
        {
            if (modelo.smorfsList.Count > 0 && modelo.ConfigSetup.genomeFiles.Count > 0)
            {
                realignSequencesButton.IsEnabled = true;
            }
            else
            {
                realignSequencesButton.IsEnabled = false;
            }

            if (modelo.ConfigSetup.genomeFiles.Count > 0)
            {
                CreateIndexButton.IsEnabled = true;
            }
            else
            {
                CreateIndexButton.IsEnabled = false;
            }
        }

        private void ToAnnotationButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void genomeFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modelo.ConfigSetup == null)
            {
                modelo.ConfigSetup = new ParametersClass();
            }

            modelo.ConfigSetup.genomeFiles = genomeFilesListBox.SelectedItems.Cast<string>().ToList();
            CheckNextStep();
        }

        private void selectIndexFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select index File";
            dialog.Filter = "Binary files|*.bin";
            if (dialog.ShowDialog() == true)
            {
                modelo.ConfigSetup.indexFile = dialog.FileName;
                textBoxIndex.Text = dialog.FileName;
                CheckNextStep();
            }
        }

        private void CreateIndexButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save index File";
            dialog.Filter = "Binary files|*.bin";
            dialog.AddExtension = true;
            dialog.DefaultExt = "bin";
            if (dialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                worker.DoWork += (s, ev) =>
                {
                    modelo.ConfigSetup.indexFile = dialog.FileName;
                    Index index = new Index(modelo.ConfigSetup);
                    index.Save(modelo.ConfigSetup.indexFile);
                };


                worker.RunWorkerCompleted += (s, ev) =>
                {
                    
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
        }

        private void RealignWithFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Multiselect = true;
            openDialog.Filter = "gz files|*.gz";
            List<GroupReads> paragraphs = new List<GroupReads>();
            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                var workerAligner = new BackgroundWorker();

                int NotFound = 0;
                int Found = 0;

                workerAligner.DoWork += (s, ev) =>
                {
                    Parallel.ForEach(modelo.smorfsList, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, item =>
                    {
                        bool founded = RealingWithFileData(item, paragraphs);

                        if (founded)
                            {
                                Found++;
                            }
                            else
                            {
                                NotFound++;
                            }

                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            busyIndicator.BusyContent = string.Format("Found {0}, Not Found {1}, Total {2}", Found.ToString(), NotFound.ToString(), modelo.smorfsList.Count().ToString());
                        }));

                    });
                };
                worker.DoWork += (s, ev) =>
                {
                    foreach (var selectedFile in openDialog.FileNames)
                    {
                        FileInfo file = new FileInfo(selectedFile);
                        using (FileStream originalFileStream = file.OpenRead())
                        {
                            string currentFileName = file.FullName;
                            string newFileName = currentFileName.Remove(currentFileName.Length - file.Extension.Length);

                            string srcName = file.Name.Remove(file.Name.Length - file.Extension.Length);

                            using (GZipStream decompressionStream = new GZipStream(originalFileStream, CompressionMode.Decompress))
                            {
                                using (var sr = new StreamReader(decompressionStream))
                                {

                                    while (!sr.EndOfStream)
                                    {
                                        string lineaActual = sr.ReadLine();
                                        while (!sr.EndOfStream && lineaActual.StartsWith("#"))
                                        {
                                            lineaActual = sr.ReadLine();
                                        }

                                        while (!sr.EndOfStream && !lineaActual.StartsWith("a"))
                                        {
                                            lineaActual = sr.ReadLine();
                                        }

                                        lineaActual = sr.ReadLine();

                                        GroupReads newParagraph = new GroupReads();
                                        bool spicies1 = false;
                                        bool spicies2 = false;
                                        bool spicies3 = false;
                                        while (lineaActual.StartsWith("s "))
                                        {
                                            AlignmentInstance lineTemp = new AlignmentInstance(lineaActual);
                                            if (!lineTemp.specieName.StartsWith("ancestral"))
                                            {
                                                lineTemp.fileName = srcName;
                                                if (lineTemp.specieName == modelo.ConfigSetup.specie1)
                                                {
                                                    newParagraph.aligments.Add(lineTemp);
                                                    spicies1 = true;
                                                }
                                                if (lineTemp.specieName == modelo.ConfigSetup.specie2)
                                                {
                                                    newParagraph.aligments.Add(lineTemp);
                                                    spicies2 = true;
                                                }
                                                if (lineTemp.specieName == modelo.ConfigSetup.specie3)
                                                {
                                                    newParagraph.aligments.Add(lineTemp);
                                                    spicies3 = true;
                                                }

                                            }
                                            if (!sr.EndOfStream)
                                            {
                                                lineaActual = sr.ReadLine();
                                            }
                                            else
                                            {
                                                lineaActual = "";
                                            }
                                        }

                                        if (spicies1 && spicies2 && spicies3)
                                        {
                                            paragraphs.Add(newParagraph);
                                        }
                                        
                                    }
                                }
                            }

                        }

                    }


                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    workerAligner.RunWorkerAsync();
                };

                workerAligner.RunWorkerCompleted += (s, ev) =>
                {
                    ShowSequencesButton.IsEnabled = true;
                    UpdatePlots();
                    busyIndicator.IsBusy = false;
                };


                worker.RunWorkerAsync();
            }
        }

        private bool RealingWithFileData(smorf item, List<GroupReads> paragraphs)
        {
            bool founded = false;
            foreach (GroupReads paragraph in paragraphs)
            {
                AlignmentInstance line = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie1);
                if (line != null)
                {
                    if (item.MafSource == line.fileName && item.Position >= line.start && item.Position <= line.size)
                    {
                        string search = line.text;
                        int point = search.IndexOf(item.Sequence);
                        while (point >= 0)
                        {
                            AlignmentInstance lineSeconS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie2);
                            string secondSpecies = lineSeconS.text.Substring(point, item.SequenceSecondSpecies.Length);
                            if (secondSpecies == item.SequenceSecondSpecies)
                            {
                                AlignmentInstance lineThirdS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie3);
                                string thirdSpecies = lineThirdS.text.Substring(point, item.SequenceThirdSpecies.Length);
                                if (thirdSpecies == item.SequenceThirdSpecies)
                                {
                                    if (line.strand == '+')
                                    {
                                        int reacomodo = 1;
                                        reacomodo -= line.text.Substring(0, point).Count(q => q == '-');
                                        item.Position = line.start + point + reacomodo;
                                        item.Strand = "+";
                                    }
                                    else
                                    {
                                        int reacomodo = 1 - item.Sequence.Length;
                                        reacomodo += line.text.Substring(0, point).Count(q => q == '-');
                                        item.Position = line.sourceSize - line.start - point + reacomodo;
                                        item.Strand = "-";
                                    }
                                    founded = true;
                                    break;
                                }
                            }
                            point = search.IndexOf(item.Sequence, point + 1);
                        }
                        if (!founded)
                        {
                            string inverseSmorf = SmorfSearch.Invert(item.Sequence);
                            point = search.IndexOf(inverseSmorf);
                            while (point >= 0)
                            {
                                AlignmentInstance lineSeconS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie2);
                                string secondSpecies = lineSeconS.text.Substring(point, item.SequenceSecondSpecies.Length);
                                if (secondSpecies == SmorfSearch.Invert(item.SequenceSecondSpecies))
                                {
                                    AlignmentInstance lineThirdS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie3);
                                    string thirdSpecies = lineThirdS.text.Substring(point, item.SequenceThirdSpecies.Length);
                                    if (thirdSpecies == SmorfSearch.Invert(item.SequenceThirdSpecies))
                                    {
                                        if (line.strand == '+')
                                        {
                                            int reacomodo = 1;
                                            reacomodo -= line.text.Substring(0, point).Count(q => q == '-');
                                            item.Position = line.start + point + reacomodo;
                                            item.Strand = "-";
                                        }
                                        else
                                        {
                                            int reacomodo = 1 - item.Sequence.Length;
                                            reacomodo += line.text.Substring(0, point).Count(q => q == '-');
                                            item.Position = line.sourceSize - line.start - point + reacomodo;
                                            item.Strand = "+";
                                        }
                                        founded = true;
                                        break;
                                    }
                                }
                                point = search.IndexOf(inverseSmorf, point + 1);
                            }
                        }
                    }
                }
            }
            return founded;
        }

        private void RealinWithFileData(List<GroupReads> paragraphs)
        {
            List<smorf> NotFound = modelo.smorfsList;
            List<smorf> Found = new List<smorf>();
            foreach (GroupReads paragraph in paragraphs)
            {
                AlignmentInstance line = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie1);
                if (line != null)
                {
                    
                    for (int i = 0; i < NotFound.Count(); )
                    {
                        bool smorfFounded = false;
                        if (NotFound[i].MafSource == line.fileName)
                        {
                            string search = line.text;
                            int point = search.IndexOf(NotFound[i].Sequence);
                            while (point >= 0)
                            {
                                AlignmentInstance lineSeconS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie2);
                                string secondSpecies = lineSeconS.text.Substring(point, NotFound[i].SequenceSecondSpecies.Length);
                                if (secondSpecies == NotFound[i].SequenceSecondSpecies)
                                {
                                    AlignmentInstance lineThirdS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie3);
                                    string thirdSpecies = lineThirdS.text.Substring(point, NotFound[i].SequenceThirdSpecies.Length);
                                    if (thirdSpecies == NotFound[i].SequenceThirdSpecies)
                                    {
                                        if (line.strand == '+')
                                        {
                                            int reacomodo = 1;
                                            reacomodo -= line.text.Substring(0, point).Count(q => q == '-');
                                            NotFound[i].Position = line.start + point + reacomodo;
                                            //NotFound[i].EndPosition = NotFound[i].Position + NotFound[i].Sequence.Length - 1;
                                            NotFound[i].Strand = "+";
                                        }
                                        else
                                        {
                                            int reacomodo = 1 - NotFound[i].Sequence.Length;
                                            reacomodo += line.text.Substring(0, point).Count(q => q == '-');
                                            NotFound[i].Position = line.sourceSize - line.start - point + reacomodo;
                                            //NotFound[i].endPosition = NotFound[i].startPosition + menor + 2;
                                            NotFound[i].Strand = "-";
                                        }
                                        smorfFounded = true;
                                        break;
                                    }
                                }
                                point = search.IndexOf(NotFound[i].Sequence, point + 1);
                            }
                            if (!smorfFounded)
                            {
                                string inverseSmorf = SmorfSearch.Invert(NotFound[i].Sequence);
                                point = search.IndexOf(inverseSmorf);
                                while (point >= 0)
                                {
                                    AlignmentInstance lineSeconS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie2);
                                    string secondSpecies = lineSeconS.text.Substring(point, NotFound[i].SequenceSecondSpecies.Length);
                                    if (secondSpecies == SmorfSearch.Invert(NotFound[i].SequenceSecondSpecies))
                                    {
                                        AlignmentInstance lineThirdS = paragraph.aligments.FirstOrDefault(q => q.specieName == modelo.ConfigSetup.specie3);
                                        string thirdSpecies = lineThirdS.text.Substring(point, NotFound[i].SequenceThirdSpecies.Length);
                                        if (thirdSpecies == SmorfSearch.Invert(NotFound[i].SequenceThirdSpecies))
                                        {
                                            if (line.strand == '+')
                                            {
                                                int reacomodo = 1;
                                                reacomodo -= line.text.Substring(0, point).Count(q => q == '-');
                                                NotFound[i].Position = line.start + point + reacomodo;
                                                //NotFound[i].EndPosition = NotFound[i].Position + NotFound[i].Sequence.Length - 1;
                                                NotFound[i].Strand = "-";
                                            }
                                            else
                                            {
                                                int reacomodo = 1 - NotFound[i].Sequence.Length;
                                                reacomodo += line.text.Substring(0, point).Count(q => q == '-');
                                                NotFound[i].Position = line.sourceSize - line.start - point + reacomodo;
                                                //NotFound[i].endPosition = NotFound[i].startPosition + menor + 2;
                                                NotFound[i].Strand = "+";
                                            }
                                            smorfFounded = true;
                                            break;
                                        }
                                    }
                                    point = search.IndexOf(inverseSmorf, point + 1);
                                }
                            }
                        }
                        if (smorfFounded)
                        {
                            NotFound[i].Chromosome = NotFound[i].Chromosome;
                            Found.Add(NotFound[i]);
                            NotFound.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }
                    }
                }
                
            }
            modelo.smorfsList = Found;
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
    }
}
