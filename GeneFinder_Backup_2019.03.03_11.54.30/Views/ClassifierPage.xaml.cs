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
using Telerik.Windows.Controls.ChartView;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for ClassifierPage.xaml
    /// </summary>
    public partial class ClassifierPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        static object lockObj = new object();
        internal MainWindow ventanaPrincipal;

        public ClassifierPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (modelo.ConfigSetup != null)
            {
                textBoxSpecie1.Text = modelo.ConfigSetup.specie3;
                textBoxSpecie2.Text = modelo.ConfigSetup.specie2;
                textBoxSpecie3.Text = modelo.ConfigSetup.specie1;
                textBoxBranchC.Text = modelo.ConfigSetup.tc.ToString();
                textBoxBranchB.Text = modelo.ConfigSetup.tb.ToString();
                textBoxBranchA.Text = modelo.ConfigSetup.ta.ToString();
                textBoxBranchD.Text = modelo.ConfigSetup.td.ToString();
                textBoxCodingCutoff.Value = modelo.ConfigSetup.cutoff;
            }
            else
            {
                textBoxBranchC.Text = "0.007501";
                textBoxBranchB.Text = "0.009168";
                textBoxBranchA.Text = "0.039953";
                textBoxBranchD.Text = "0.026355";
                textBoxCodingCutoff.Value = 0.2;
            }




            if (modelo.pSmorfsList != null && modelo.pSmorfsList.Count > 0)
            {
                classifySequencesButton.IsEnabled = true;
                //sequencesCounttaxtblock.Text = modelo.pSmorfsList.Count.ToString();

                List<PossibleSmorf> data = modelo.pSmorfsList;

                if (data != null)
                {
                    int minSimilarity = data.Min(q => q.similarity);
                    int maxSimilarity = data.Max(q => q.similarity);

                    horizontalAxisSimilarity.Minimum = minSimilarity;
                    horizontalAxisSimilarity.Maximum = maxSimilarity;

                    serieSimilarity.DataPoints.Clear();

                    for (int i = maxSimilarity; i >= minSimilarity; i -= 3)
                    {
                        serieSimilarity.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.similarity >= i && q.similarity < i + 3), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
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

                    foreach (string start in ventanaPrincipal.model.ConfigSetup.startCodons)
                    {
                        int numStart = data.Count(q => q.startCodon == start);
                        startCodonSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("{0}: {1}", start, numStart), Value = numStart });
                    }


                    stopCodonSeries.DataPoints.Clear();

                    foreach (string stop in ventanaPrincipal.model.ConfigSetup.stopCodons)
                    {
                        int numStop = data.Count(q => q.stopCodon == stop);
                        stopCodonSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("{0}: {1}", stop, numStop), Value = numStop });
                    }

                    SequencesFoundNumber.Content = data.Count();
                }

                
                chartPossibleSmurfs.Visibility = Visibility.Visible;

            }

            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                CalculateCharts();
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
                            string lineaActual = "";
                            modelo.ConfigSetup.ReadFileHeader(sr);
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
                    if (modelo.ConfigSetup != null)
                    {
                        textBoxSpecie1.Text = modelo.ConfigSetup.specie3;
                        textBoxSpecie2.Text = modelo.ConfigSetup.specie2;
                        textBoxSpecie3.Text = modelo.ConfigSetup.specie1;
                    }

                    if (modelo.pSmorfsList != null && modelo.pSmorfsList.Count > 0)
                    {
                        classifySequencesButton.IsEnabled = true;
                        //sequencesCounttaxtblock.Text = modelo.pSmorfsList.Count.ToString();
                    }

                    if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
                    {
                        CalculateCharts();
                    }

                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
        }

        private void OpenFastaButton_ClickAnterior(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "fasta files|*.fasta";

            if (openDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                var worker = new BackgroundWorker();

                worker.DoWork += (s, ev) =>
                {
                    int line = 0;
                    FileInfo file = new FileInfo(openDialog.FileName);
                    if (modelo.ConfigSetup == null) modelo.ConfigSetup = new ParametersClass();
                    bool error = false;
                    using (FileStream originalFileStream = file.OpenRead())
                    {
                        using (var sr = new StreamReader(originalFileStream))
                        {
                            //string lineaActual = "";
                            //while (!sr.EndOfStream)
                            //{
                            //    lineaActual = sr.ReadLine();
                            //}

                            //PossibleSmorf nuevo = new PossibleSmorf(lineaActual);

                            //modelo.pSmorfsList.Add(nuevo);

                            //this.Dispatcher.Invoke((Action)(() =>
                            //{
                            //    sequencesCounttaxtblock.Text = modelo.pSmorfsList.Count().ToString();
                            //}));

                            char[] chars = { 'E', 'I', 'O', 'U', 'Q', 'W', 'R', 'Y', 'P', 'S', 'D', 'F', 'H', 'J', 'K', 'L', 'Z', 'X', 'V', 'B', 'M' };
                            string lineaActual = "";
                            while (!sr.EndOfStream)
                            {

                                if (line == 0 || !error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                }
                                if (line > 0 && error && lineaActual.StartsWith(">"))
                                {
                                    string lineaTemp = lineaActual.Substring(1);
                                    string[] dataSmorfs = lineaTemp.Split('|');
                                    if (modelo.ConfigSetup.specie1 != dataSmorfs[0])
                                    {
                                        lineaActual = sr.ReadLine();
                                        line++;
                                    }
                                }

                                PossibleSmorf nuevo = new PossibleSmorf();
                                if (lineaActual.StartsWith(">"))
                                {
                                    try
                                    {
                                        error = false;
                                        string lineaTemp = lineaActual.Substring(1);
                                        string[] dataSmorfs = lineaTemp.Split('|');
                                        if (line == 1)
                                        {
                                            modelo.ConfigSetup.specie1 = dataSmorfs[0];
                                        }
                                        else
                                        {
                                            if (modelo.ConfigSetup.specie1 != dataSmorfs[0])
                                                error = true;
                                        }
                                        nuevo.id = int.Parse(dataSmorfs[1]);
                                        nuevo.chromosome = dataSmorfs[2];
                                        nuevo.strand = dataSmorfs[3][0];
                                        nuevo.startPosition = int.Parse(dataSmorfs[4]);
                                        nuevo.endPosition = int.Parse(dataSmorfs[5]);
                                        nuevo.sequenceLength = int.Parse(dataSmorfs[6]);
                                        nuevo.similarity = int.Parse(dataSmorfs[7]);
                                        nuevo.startCodon = dataSmorfs[8];
                                        nuevo.stopCodon = dataSmorfs[9];
                                        nuevo.sourceFile = dataSmorfs[10];
                                    }
                                    catch (Exception exc)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        if (error)
                                        {
                                            lineaActual = sr.ReadLine();
                                            line++;
                                        }
                                        error = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                    if (error)
                                    {
                                        lineaActual = sr.ReadLine();
                                        line++;
                                    }
                                    error = true;
                                }

                                if (!error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                    if (lineaActual == null || lineaActual.StartsWith(">") || lineaActual.Length == 0)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        error = true;
                                        //break;
                                    }
                                    else
                                    {
                                        nuevo.specie1Content = lineaActual.ToUpperInvariant();
                                        if (nuevo.specie1Content.Length % 3 != 0)
                                            error = true;
                                        if (nuevo.specie1Content.IndexOfAny(chars) > -1)
                                            error = true;
                                    }

                                }

                                if (!error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                    if (lineaActual == null || !lineaActual.StartsWith(">") || lineaActual.Length == 0)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        error = true;
                                        //break;
                                    }
                                    else
                                    {
                                        string lineaTemp = lineaActual.Substring(1);
                                        string[] dataSmorfs = lineaTemp.Split('|');
                                        if (line == 3)
                                        {
                                            modelo.ConfigSetup.specie2 = dataSmorfs[0];
                                        }
                                        else
                                        {
                                            if (modelo.ConfigSetup.specie2 != dataSmorfs[0])
                                                error = true;
                                        }
                                    }
                                }

                                if (!error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                    if (lineaActual == null || lineaActual.StartsWith(">") || lineaActual.Length == 0)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        error = true;
                                        //break;
                                    }
                                    else
                                    {
                                        nuevo.specie2Content = lineaActual.ToUpperInvariant();
                                        if (nuevo.specie2Content.Length % 3 != 0)
                                            error = true;
                                        if (nuevo.specie2Content.IndexOfAny(chars) > -1)
                                            error = true;
                                    }

                                }

                                if (!error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                    if (lineaActual == null || !lineaActual.StartsWith(">") || lineaActual.Length == 0)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        error = true;
                                        //break;
                                    }
                                    else
                                    {
                                        string lineaTemp = lineaActual.Substring(1);
                                        string[] dataSmorfs = lineaTemp.Split('|');
                                        if (line == 5)
                                        {
                                            modelo.ConfigSetup.specie3 = dataSmorfs[0];
                                        }
                                        else
                                        {
                                            if (modelo.ConfigSetup.specie3 != dataSmorfs[0])
                                                error = true;
                                        }
                                    }
                                }

                                if (!error)
                                {
                                    lineaActual = sr.ReadLine();
                                    line++;
                                    if (lineaActual == null || lineaActual.StartsWith(">") || lineaActual.Length == 0)
                                    {
                                        MessageBox.Show(string.Format("Error in file. Line {0}", line), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                                        error = true;
                                        //break;
                                    }
                                    else
                                    {
                                        nuevo.specie3Content = lineaActual.ToUpperInvariant();
                                        if (nuevo.specie3Content.Length % 3 != 0)
                                            error = true;
                                        if (nuevo.specie3Content.IndexOfAny(chars) > -1)
                                            error = true;
                                    }

                                }

                                if (!error) modelo.pSmorfsList.Add(nuevo);

                                this.Dispatcher.Invoke((Action)(() =>
                                {
                                    //sequencesCounttaxtblock.Text = modelo.pSmorfsList.Count().ToString();
                                }));
                            }
                        }
                    }
                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    if (modelo.ConfigSetup != null)
                    {
                        textBoxSpecie1.Text = modelo.ConfigSetup.specie3;
                        textBoxSpecie2.Text = modelo.ConfigSetup.specie2;
                        textBoxSpecie3.Text = modelo.ConfigSetup.specie1;
                    }

                    if (modelo.pSmorfsList != null && modelo.pSmorfsList.Count > 0)
                    {
                        classifySequencesButton.IsEnabled = true;
                    }
                    busyIndicator.IsBusy = false;
                };

                worker.RunWorkerAsync();
            }
        }

        private void classifySequencesButton_Click(object sender, RoutedEventArgs e)
        {
            //using (StreamReader fastaStream = new StreamReader("/home/vero/MonoSolutions/C3/PhyloCSF/short.fasta"))

            try
            {
                modelo.ConfigSetup.tc = double.Parse(textBoxBranchC.Text);
                modelo.ConfigSetup.tb = double.Parse(textBoxBranchB.Text);
                modelo.ConfigSetup.ta = double.Parse(textBoxBranchA.Text);
                modelo.ConfigSetup.td = double.Parse(textBoxBranchD.Text);
                modelo.ConfigSetup.cutoff = (double)textBoxCodingCutoff.Value;

                using (StreamWriter writer = new StreamWriter("Data//hcrPhyloT.txt"))
                {
                    writer.WriteLine(string.Format("{0} {1} {2} {3}", modelo.ConfigSetup.tc, modelo.ConfigSetup.tb, modelo.ConfigSetup.ta, modelo.ConfigSetup.td));
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error in the parameters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.Title = "File Name for temporal output";
            saveDialog.Filter = "Genefinder Files|*.gfn";
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = ".gfn";




            if (saveDialog.ShowDialog() == true)
            {
                busyIndicator.IsBusy = true;
                StreamWriter outputStream = new StreamWriter(saveDialog.FileName);
                sequencesEvaluatedCountStack.Visibility = Visibility.Visible;

                var worker = new BackgroundWorker();
                DateTime inicio = DateTime.Now;

                worker.DoWork += (s, ev) =>
                {
                    //for (int i = 0; i < modelo.pSmorfsList.Count; i++)
                    //{
                    //    Problem problem;
                    //    problem = Problem.ReadProblem(modelo.pSmorfsList[i]);
                    //    if (problem != null)
                    //    {
                    //        ProblemSolver problemSolver = new ProblemSolver(problem);
                    //        CodingScore score = problemSolver.Solve();
                    //        lock (lockObj)
                    //        {
                    //            //Console.WriteLine("iThread = {0}\tName={1}\nFinal score={2}", 0, problem.Name, score);
                    //            outputStream.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t", problem.Name, problem.Human, problem.Chimp, problem.Rhesus, score);
                    //        }
                    //        //problem = Problem.ReadProblem(fastaStream);
                    //        this.Dispatcher.Invoke((Action)(() =>
                    //        {
                    //            sequencesCounttaxtblock.Text = (i+1).ToString();
                    //        }));
                    //    }
                    //}
                    int llevo = 0;
                    int total = modelo.pSmorfsList.Count;
                    modelo.ConfigSetup.AddFileHeader(outputStream);

                    Parallel.For(0, modelo.pSmorfsList.Count,
                        iThread =>
                        {
                            try
                            {
                                Problem problem;
                                problem = Problem.ReadProblem(modelo.pSmorfsList[iThread]);
                                if (problem != null)
                                {
                                    ProblemSolver problemSolver = new ProblemSolver(problem);
                                    CodingScore score = problemSolver.Solve();
                                    lock (lockObj)
                                    {
                                        //Console.WriteLine("iThread = {0}\tName={1}\nFinal score={2}", 0, problem.Name, score);
                                        llevo++;
                                        //outputStream.WriteLine("{0}\t{1}\t{2}\t{3}\t{4}\t", problem.Name, problem.Human, problem.Chimp, problem.Rhesus, score);
                                        smorf nuevo = new smorf(modelo.pSmorfsList[iThread], score);
                                        modelo.smorfsList.Add(nuevo);
                                        nuevo.WriteToFile(outputStream, modelo.ConfigSetup);
                                        TimeSpan going = DateTime.Now - inicio;
                                        int seconds = (int)going.TotalSeconds;
                                        seconds = (int)(seconds * (float)(total - llevo) / (float)llevo);
                                        this.Dispatcher.Invoke((Action)(() =>
                                        {
                                            busyIndicator.BusyContent = string.Format("Evaluation {0}%, Time left: {1} minutes, {2} seconds", llevo * 100 / total, seconds / 60, seconds % 60);
                                            sequencesEvaluatedCounttaxtblock.Text = llevo.ToString();
                                        }));
                                    }



                                    //problem = Problem.ReadProblem(fastaStream);
                                }
                            }
                            catch (Exception exc)
                            {
                                MessageBox.Show(string.Format("Error. {0} \nl {1}", exc.Message, exc.InnerException), "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        );
                };

                worker.RunWorkerCompleted += (s, ev) =>
                {
                    try
                    {
                        CalculateCharts();
                        outputStream.Close();
                        ShowSequencesButton.IsEnabled = true;
                        //ToRealignButton.IsEnabled = true;
                        busyIndicator.IsBusy = false;
                        ventanaPrincipal.goToEstadisticasPrimera();

                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                };

                worker.RunWorkerAsync();
            }

        }

        private void CalculateCharts()
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                foreach (var item in modelo.smorfsList)
                {
                    if (float.IsNaN(item.CodingScore))
                    {
                        item.CodingScore = -50;
                    }
                }
                float minSimilarity = modelo.smorfsList.Min(q => q.CodingScore);
                float maxSimilarity = modelo.smorfsList.Max(q => q.CodingScore);

                horizontalAxisScore.Minimum = minSimilarity;
                horizontalAxisScore.Maximum = maxSimilarity;

                serieScore.DataPoints.Clear();

                int numSteps = 200;
                float step = (maxSimilarity - minSimilarity) / ((float)numSteps);

                for (float i = minSimilarity; i <= maxSimilarity; i += step)
                {
                    try
                    {
                        serieScore.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = modelo.smorfsList.Count(q => q.CodingScore > i && q.CodingScore <= i + step), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message);
                    }

                }

                chartScore.Annotations.Clear();
                chartScore.Annotations.Add(new CartesianGridLineAnnotation { Axis = chartScore.HorizontalAxis, Value = modelo.ConfigSetup.cutoff, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 4 });


                classificationSeries.DataPoints.Clear();

                try
                {
                    int numPositive = modelo.smorfsList.Count(q => q.CodingScore >= modelo.ConfigSetup.cutoff);
                    classificationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Putative Coding {0}%", numPositive * 100 / modelo.smorfsList.Count), Value = numPositive });
                    int numNegative = modelo.smorfsList.Count(q => q.CodingScore < modelo.ConfigSetup.cutoff);
                    classificationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Putative Non-Coding {0}%", numNegative * 100 / modelo.smorfsList.Count), Value = numNegative });
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }



                graphsClassifiers.Visibility = Visibility.Visible;

            }
        }




        private void ToRealignButton_Click(object sender, RoutedEventArgs e)
        {
            ventanaPrincipal.goToRealigner();
        }

        private void ShowSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            SmorfListWindow ventana = new SmorfListWindow();
            ventana.GridSmorf.ItemsSource = modelo.pSmorfsList;
            ventana.parameters = modelo.ConfigSetup;
            ventana.ShowDialog();
        }

        private void textBoxCodingCutoff_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (modelo.ConfigSetup != null)
            {
                modelo.ConfigSetup.cutoff = (double)textBoxCodingCutoff.Value;
                CalculateCharts();
            }

        }

        private void exportToFastaButton_Click(object sender, RoutedEventArgs e)
        {
            if (modelo.pSmorfsList != null)
            {
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.Filter = "Fasta files|*.fasta";
                dialog.AddExtension = true;
                dialog.DefaultExt = "fasta";

                if (dialog.ShowDialog() == true)
                {

                    var csv = new StringBuilder();

                    using (StreamWriter outfile = new StreamWriter(dialog.FileName))
                    {
                        foreach (var item in modelo.pSmorfsList)
                        {
                            string header = string.Format(">{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", modelo.ConfigSetup.specie1, item.id, item.chromosome, item.strand, item.startPosition, item.endPosition, item.sequenceLength, item.similarity, item.startCodon, item.stopCodon, item.sourceFile);
                            outfile.WriteLine(header);
                            outfile.WriteLine(item.specie1Content);
                            if (!string.IsNullOrEmpty(item.specie2Content))
                            {
                                header = string.Format(">{0}|{1}", modelo.ConfigSetup.specie2, item.id);
                                outfile.WriteLine(header);
                                outfile.WriteLine(item.specie2Content);
                            }
                            if (!string.IsNullOrEmpty(item.specie3Content))
                            {
                                header = string.Format(">{0}|{1}", modelo.ConfigSetup.specie3, item.id);
                                outfile.WriteLine(header);
                                outfile.WriteLine(item.specie3Content);
                            }

                        }
                    }
                    

                }


            }
        }
    }
}
