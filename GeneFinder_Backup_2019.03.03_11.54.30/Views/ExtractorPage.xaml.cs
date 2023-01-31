using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
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
    /// Interaction logic for ExtractorPage.xaml
    /// </summary>
    public partial class ExtractorPage : Page
    {
        internal List<GroupReads> paragraphs = new List<GroupReads>();
        internal List<GroupReads> paragraphsSelected = new List<GroupReads>();
        internal List<string> species = new List<string>();
        internal string srcName = "";
        internal ParametersClass parameters;
        internal List<PossibleSmorf> pSmorfsList;
        internal MainWindow ventanaPrincipal;

        public ExtractorPage()
        {
            InitializeComponent();
        }

        private void OpenGzFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();

            openDialog.Multiselect = true;
            openDialog.Filter = "gz files|*.gz";
            
            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                worker.DoWork += (s, ev) =>
                {
                    foreach (var selectedFile in openDialog.FileNames)
                    {
                        FileInfo file = new FileInfo(selectedFile);
                        using (FileStream originalFileStream = file.OpenRead())
                        {
                            string currentFileName = file.FullName;
                            string newFileName = currentFileName.Remove(currentFileName.Length - file.Extension.Length);

                            srcName = file.Name.Remove(file.Name.Length - file.Extension.Length);

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
                                        while (!sr.EndOfStream && lineaActual.StartsWith("s "))
                                        {
                                            AlignmentInstance lineTemp = new AlignmentInstance(lineaActual);
                                            if (!lineTemp.specieName.StartsWith("ancestral"))
                                            {
                                                lineTemp.fileName = srcName;
                                                newParagraph.aligments.Add(lineTemp);
                                                string tempSpecie = lineTemp.specieName;
                                                if (!species.Contains(tempSpecie)) species.Add(tempSpecie);
                                            }

                                            lineaActual = sr.ReadLine();
                                        }
                                        paragraphs.Add(newParagraph);
                                    }
                                }
                            }

                        }
                        
                    }

                    
                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    foreach (var item in species)
                    {
                        int count = paragraphs.Count(p => p.aligments.Any(a => a.specieName == item));
                        //foreach (var p in paragraphs)
                        //{
                        //    if (p.aligments.Any(a => a.specieName == item)) count++;
                        //}

                        serieSpecies.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = item, Value = count });
                    }

                    chartParagraphs.Visibility = Visibility.Visible;

                    selectionAuto.Visibility = Visibility.Visible;
                    selectionManual.Visibility = Visibility.Collapsed;

                    comboBoxSpecie1.ItemsSource = species;
                    comboBoxSpecie2.ItemsSource = species;
                    comboBoxSpecie3.ItemsSource = species;
                    extractSequencesButton.IsEnabled = true;
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }


        }

        //void worker_DoWork(object sender, DoWorkEventArgs e)
        //{
        //    throw new NotImplementedException();
        //}

        private void comboBoxSpecie1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelected();
        }

        private void UpdateSelected()
        {
            List<string> selectedSpecies = new List<string>();
            if (comboBoxSpecie1.SelectedItem != null) selectedSpecies.Add(comboBoxSpecie1.SelectedItem as string);
            if (comboBoxSpecie2.SelectedItem != null) selectedSpecies.Add(comboBoxSpecie2.SelectedItem as string);
            if (comboBoxSpecie3.SelectedItem != null) selectedSpecies.Add(comboBoxSpecie3.SelectedItem as string);
            paragraphsSelected = new List<GroupReads>();
            if (selectedSpecies.Count == 0)
            {
                selectionNumber.Visibility = Visibility.Collapsed;
            }
            else
            {
                selectionNumber.Visibility = Visibility.Visible;
                paragraphsSelected.AddRange(paragraphs);
                foreach (var item in selectedSpecies)
                {
                    paragraphsSelected = paragraphsSelected.FindAll(q => q.aligments.Any(a => a.specieName == item));
                }
                labelNumParagraph.Content = paragraphsSelected.Count;
            }

        }

        private void comboBoxSpecie2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelected();
        }

        private void comboBoxSpecie3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSelected();
        }

        private void deleteStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (listStartCodons.SelectedItem == null)
            {
                MessageBox.Show("Select an item to delete", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                listStartCodons.Items.Remove(listStartCodons.SelectedItem);
            }
        }

        private void addStartButton_Click(object sender, RoutedEventArgs e)
        {
            string newStartCodon = textBoxAddStart.Text;
            if (newStartCodon.Length != 3)
            {
                MessageBox.Show("Lenght must be 3 characters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string allowed = "ATGC";
                foreach (char c in newStartCodon)
                {
                    if (!allowed.Contains(c))
                    {
                        MessageBox.Show("Codon must only contains A, T, G or C", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                listStartCodons.Items.Add(newStartCodon);
            }
        }

        private void deleteStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (listStopCodons.SelectedItem == null)
            {
                MessageBox.Show("Select an item to delete", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                listStopCodons.Items.Remove(listStopCodons.SelectedItem);
            }
        }

        private void addStopButton_Click(object sender, RoutedEventArgs e)
        {
            string newStopCodon = textBoxAddStop.Text;
            if (newStopCodon.Length != 3)
            {
                MessageBox.Show("Lenght must be 3 characters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string allowed = "ATGC";
                foreach (char c in newStopCodon)
                {
                    if (!allowed.Contains(c))
                    {
                        MessageBox.Show("Codon must only contains A, T, G or C", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                listStopCodons.Items.Add(newStopCodon);
            }
        }

        private void extractSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            List<string> starts = new List<string>();
            foreach (var s in listStartCodons.Items)
            {
                starts.Add(s as string);
            }
            List<string> stops = new List<string>();
            foreach (var st in listStopCodons.Items)
            {
                stops.Add(st as string);
            }
            parameters = new ParametersClass()
            {
                maxLenght = (int)maxLenght.Value,
                minLenght = (int)minLenght.Value,
                startCodons = starts,
                stopCodons = stops,
                allowChange = false,
                similarity = 80,
                specie1 = comboBoxSpecie1.SelectedItem as string,
                specie2 = comboBoxSpecie2.SelectedItem as string,
                specie3 = comboBoxSpecie3.SelectedItem as string
            };

            if (RadioSpecies1.IsChecked == true)
            {
                parameters.alignedBy = 1;
            }
            else
            {
                if (RadioSpecies2.IsChecked == true)
                {
                    parameters.alignedBy = 2;
                }
                else
                {
                    parameters.alignedBy = 3;
                }
            }

            ConcurrentBag<PossibleSmorf> pSmorfs = new ConcurrentBag<PossibleSmorf>();
            pSmorfsList = new List<PossibleSmorf>();

            ParametersClass parametersInverses = new ParametersClass()
            {
                allowChange = parameters.allowChange,
                maxLenght = parameters.maxLenght,
                minLenght = parameters.minLenght,
                similarity = parameters.similarity,
                specie1 = parameters.specie1,
                specie2 = parameters.specie2,
                specie3 = parameters.specie3,
            };

            if (RadioSpecies1.IsChecked == true)
            {
                parametersInverses.alignedBy = 1;
            }
            else
            {
                if (RadioSpecies2.IsChecked == true)
                {
                    parametersInverses.alignedBy = 2;
                }
                else
                {
                    parametersInverses.alignedBy = 3;
                }
            }

            List<string> inversesStarts = new List<string>();
            foreach (string start in parameters.startCodons)
            {
                inversesStarts.Add(SmorfSearch.Invert(start));
            }
            List<string> inversesStops = new List<string>();
            foreach (string stop in parameters.stopCodons)
            {
                inversesStops.Add(SmorfSearch.Invert(stop));
            }

            parametersInverses.startCodons = inversesStops;
            parametersInverses.stopCodons = inversesStarts;



            busyIndicator.IsBusy = true;

            BackgroundWorker worker = new BackgroundWorker();
            int count = 0;
            Object thisLock = new Object();

            worker.DoWork += (s, ev) =>
            {
                Parallel.ForEach(paragraphsSelected, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, item =>
                {
                    List<PossibleSmorf> tempList = SmorfSearch.Search(parameters, item);
                    foreach (var smo in tempList) 
                    {
                        pSmorfs.Add(smo);
                    }
                    tempList = SmorfSearch.SearchInverse(parametersInverses, item);
                    foreach (var smo in tempList)
                    {
                        pSmorfs.Add(smo);
                    }

                    lock (thisLock)
                    {
                        updateIndicator(++count, paragraphsSelected.Count);
                    }


                    //pSmorfs.AddRange(SmorfSearch.Search(parameters, item));
                    //pSmorfs.AddRange(SmorfSearch.SearchInverse(parametersInverses, item));
                });
            };


            worker.RunWorkerCompleted += (s, ev) =>
            {
                pSmorfsList.AddRange(pSmorfs.ToArray());

                //foreach (GroupReads item in paragraphsSelected)
                //{
                //    pSmorfs.AddRange(SmorfSearch.Search(parameters, item));
                //    pSmorfs.AddRange(SmorfSearch.SearchInverse(parameters, item));
                //}
                for (int i = 0; i < pSmorfsList.Count; i++)
                {
                    pSmorfsList[i].id = i;
                }

                busyIndicator.IsBusy = false;

                ChangeCharts();
                ToClassifierButton.IsEnabled = true;
                ShowSequencesButton.IsEnabled = true;
                ventanaPrincipal.model.pSmorfsList = pSmorfsList;
                ventanaPrincipal.model.ConfigSetup = parameters;
            };

            worker.RunWorkerAsync();

        }

        private void ChangeCharts()
        {
            List<PossibleSmorf> data = pSmorfsList;

            if (data != null)
            {
                int minSimilarity = data.Min(q => q.similarity);
                int maxSimilarity = data.Max(q => q.similarity);

                horizontalAxisSimilarity.Minimum = minSimilarity;
                horizontalAxisSimilarity.Maximum = maxSimilarity;

                serieSimilarity.DataPoints.Clear();

                for (int i = minSimilarity; i <= maxSimilarity; i+=3)
                {
                    serieSimilarity.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.similarity >= i && q.similarity<i+3), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                }


                int minLength = data.Min(q => q.sequenceLength);
                int maxLength = data.Max(q => q.sequenceLength);

                horizontalAxisLength.Minimum = minLength;
                horizontalAxisLength.Maximum = maxLength;

                serieLength.DataPoints.Clear();

                for (int i = minLength; i <= maxLength; i = i + 3)
                {
                    serieLength.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.sequenceLength == i) }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                }


                startCodonSeries.DataPoints.Clear();

                foreach (string start in parameters.startCodons)
                {
                    int numStart = data.Count(q => q.startCodon == start);
                    startCodonSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("{0}: {1}", start, numStart), Value = numStart });
                }


                stopCodonSeries.DataPoints.Clear();

                foreach (string stop in parameters.stopCodons)
                {
                    int numStop = data.Count(q => q.stopCodon == stop);
                    stopCodonSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("{0}: {1}", stop, numStop), Value = numStop });
                }

                SequencesFoundNumber.Content = data.Count();
            }

            chartParagraphs.Visibility = Visibility.Collapsed;
            chartPossibleSmurfs.Visibility = Visibility.Visible;
        }

        private void updateIndicator(int p, int total)
        {
            int percentage = p * 100 / total;

            this.Dispatcher.Invoke((Action)(() =>
            {
                busyIndicator.BusyContent = string.Format("{0}%", percentage);
            }));


        }

        private void ToClassifierButton_Click(object sender, RoutedEventArgs e)
        {
            ventanaPrincipal.model.pSmorfsList = pSmorfsList;
            ventanaPrincipal.model.ConfigSetup = parameters;
            ventanaPrincipal.goToClassifier();
        }

        private void ShowSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            SmorfListWindow ventana = new SmorfListWindow();
            ventana.GridSmorf.ItemsSource = pSmorfsList;
            ventana.parameters = parameters;
            ventana.ShowDialog();
        }
    }
}
