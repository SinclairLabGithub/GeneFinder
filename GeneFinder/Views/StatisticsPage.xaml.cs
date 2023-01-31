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
    /// Interaction logic for StatisticsPage.xaml
    /// </summary>
    public partial class StatisticsPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        internal MainWindow ventanaPrincipal;
        List<smorf> data;

        public StatisticsPage()
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
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

                conservationSeries.DataPoints.Clear();

                int universo = data.Count;

                //int conservedCheck = data.Count(p => p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved and check: {0} - {1}%", conservedCheck, conservedCheck * 100 / universo), Value = conservedCheck });
                //int conserved = data.Count(p => p.ConservationAverage > modelo.ConfigSetup.conservationThreshold && !p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved: {0} - {1}%", conserved, conserved * 100 / universo), Value = conserved });
                //int nonConserved = universo - conservedCheck - conserved;
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Non-conserved: {0} - {1}%", nonConserved, nonConserved * 100 / universo), Value = nonConserved });
                if (universo > 0)
                {
                    int conservedFirst = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3N: {0} - {1}%", conservedFirst, conservedFirst * 100 / universo), Value = conservedFirst });
                    int conservedSecond = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3N: {0} - {1}%", conservedSecond, conservedSecond * 100 / universo), Value = conservedSecond });
                    int conservedThird = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3N: {0} - {1}%", conservedThird, conservedThird * 100 / universo), Value = conservedThird });
                    int conservedFourth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3C: {0} - {1}%", conservedFourth, conservedFourth * 100 / universo), Value = conservedFourth });
                    int conservedFifth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3N: {0} - {1}%", conservedFifth, conservedFifth * 100 / universo), Value = conservedFifth });
                    int conservedSixth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3C: {0} - {1}%", conservedSixth, conservedSixth * 100 / universo), Value = conservedSixth });
                    int conservedSepth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3C: {0} - {1}%", conservedSepth, conservedSepth * 100 / universo), Value = conservedSepth });
                    int conservedOcth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3C: {0} - {1}%", conservedOcth, conservedOcth * 100 / universo), Value = conservedOcth });


                    annotationSeries.DataPoints.Clear();
                    int exon = data.Count(p => p.OverlapExon == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Exon: {0} - {1}%", exon, exon * 100 / universo), Value = exon });
                    int intron = data.Count(p => p.OverlapIntron == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intron: {0} - {1}%", intron, intron * 100 / universo), Value = intron });
                    int intergenic = data.Count(p => p.OverlapIntergenic == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intergenic: {0} - {1}%", intergenic, intergenic * 100 / universo), Value = intergenic });
                    int cinco = data.Count(p => p.OverlapUtr5 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5' utr: {0} - {1}%", cinco, cinco * 100 / universo), Value = cinco });
                    int tres = data.Count(p => p.OverlapUtr3 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("3' utr: {0} - {1}%", tres, tres * 100 / universo), Value = tres });
                    int ambiguous = universo - exon - intron - intergenic - cinco - tres;
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Ambiguous: {0} - {1}%", ambiguous, ambiguous * 100 / universo), Value = ambiguous });


                    seriesKozak.DataPoints.Clear();

                    for (int i = 0; i <= 15; i++)
                    {
                        seriesKozak.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = i.ToString(), Value = data.Count(p => p.KozakScore == i), Label = data.Count(p => p.KozakScore == (float)i / (float)15) });
                    }

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

                    for (float i = minSimilarity; i <= maxSimilarity; i += step)
                    {
                        serieScore.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.CodingScore > i && q.CodingScore <= i + step), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                    }

                    chartScore.Annotations.Clear();
                    chartScore.Annotations.Add(new CartesianGridLineAnnotation { Axis = chartScore.HorizontalAxis, Value = modelo.ConfigSetup.cutoff, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 4 });

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

                    coverSeries.DataPoints.Clear();

                    int cuantosHits = data.Count(q => q.RnaHits != null);
                    coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Covered: {0} - {1}%", cuantosHits, cuantosHits * 100 / universo), Value = cuantosHits });
                    int demas = universo - cuantosHits;
                    coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Not Covered: {0} - {1}%", demas, demas * 100 / universo), Value = demas });

                    hitsSeries.DataPoints.Clear();
                    int cuantosNames = data.Count(q => q.RnaConditionNames != null);
                    hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Hit: {0} - {1}%", cuantosNames, cuantosNames * 100 / universo), Value = cuantosNames });
                    demas = universo - cuantosNames;
                    hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("No Hit: {0} - {1}%", demas, demas * 100 / universo), Value = demas });


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

        private void UpdatePlotsAnterior()
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

                conservationSeries.DataPoints.Clear();

                int universo = data.Count(p => p.Coord.Count > 0);

                //int conservedCheck = data.Count(p => p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved and check: {0} - {1}%", conservedCheck, conservedCheck * 100 / universo), Value = conservedCheck });
                //int conserved = data.Count(p => p.ConservationAverage > modelo.ConfigSetup.conservationThreshold && !p.FlagConservation);
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Conserved: {0} - {1}%", conserved, conserved * 100 / universo), Value = conserved });
                //int nonConserved = universo - conservedCheck - conserved;
                //conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Non-conserved: {0} - {1}%", nonConserved, nonConserved * 100 / universo), Value = nonConserved });
                if (universo > 0)
                {
                    int conservedFirst = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3N: {0} - {1}%", conservedFirst, conservedFirst * 100 / universo), Value = conservedFirst });
                    int conservedSecond = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3N: {0} - {1}%", conservedSecond, conservedSecond * 100 / universo), Value = conservedSecond });
                    int conservedThird = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3N: {0} - {1}%", conservedThird, conservedThird * 100 / universo), Value = conservedThird });
                    int conservedFourth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3C: {0} - {1}%", conservedFourth, conservedFourth * 100 / universo), Value = conservedFourth });
                    int conservedFifth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3N: {0} - {1}%", conservedFifth, conservedFifth * 100 / universo), Value = conservedFifth });
                    int conservedSixth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3C: {0} - {1}%", conservedSixth, conservedSixth * 100 / universo), Value = conservedSixth });
                    int conservedSepth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3C: {0} - {1}%", conservedSepth, conservedSepth * 100 / universo), Value = conservedSepth });
                    int conservedOcth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3C: {0} - {1}%", conservedOcth, conservedOcth * 100 / universo), Value = conservedOcth });


                    annotationSeries.DataPoints.Clear();
                    int exon = data.Count(p => p.OverlapExon == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Exon: {0} - {1}%", exon, exon * 100 / universo), Value = exon });
                    int intron = data.Count(p => p.OverlapIntron == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intron: {0} - {1}%", intron, intron * 100 / universo), Value = intron });
                    int intergenic = data.Count(p => p.OverlapIntergenic == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intergenic: {0} - {1}%", intergenic, intergenic * 100 / universo), Value = intergenic });
                    int cinco = data.Count(p => p.OverlapUtr5 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5' utr: {0} - {1}%", cinco, cinco * 100 / universo), Value = cinco });
                    int tres = data.Count(p => p.OverlapUtr3 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("3' utr: {0} - {1}%", tres, tres * 100 / universo), Value = tres });
                    int ambiguous = universo - exon - intron - intergenic - cinco - tres;
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Ambiguous: {0} - {1}%", ambiguous, ambiguous * 100 / universo), Value = ambiguous });


                    seriesKozak.DataPoints.Clear();

                    for (int i = 0; i <= 15; i++)
                    {
                        seriesKozak.DataPoints.Add(new Telerik.Charting.CategoricalDataPoint() { Category = i.ToString(), Value = data.Count(p => p.KozakScore == (float)i / (float)15), Label = data.Count(p => p.KozakScore == (float)i / (float)15) });
                    }

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

                    for (float i = minSimilarity; i <= maxSimilarity; i += step)
                    {
                        serieScore.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.CodingScore > i && q.CodingScore <= i + step), }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                    }

                    chartScore.Annotations.Clear();
                    chartScore.Annotations.Add(new CartesianGridLineAnnotation { Axis = chartScore.HorizontalAxis, Value = modelo.ConfigSetup.cutoff, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 4 });

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

                    coverSeries.DataPoints.Clear();

                    int cuantosHits = data.Count(q => q.RnaHits != null);
                    coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Covered: {0} - {1}%", cuantosHits, cuantosHits * 100 / universo), Value = cuantosHits });
                    int demas = universo - cuantosHits;
                    coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Not Covered: {0} - {1}%", demas, demas * 100 / universo), Value = demas });

                    hitsSeries.DataPoints.Clear();
                    int cuantosNames = data.Count(q => q.RnaConditionNames != null);
                    hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Hit: {0} - {1}%", cuantosNames, cuantosNames * 100 / universo), Value = cuantosNames });
                    demas = universo - cuantosNames;
                    hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("No Hit: {0} - {1}%", demas, demas * 100 / universo), Value = demas });

                    heatMapsPanel.Children.Clear();

                    foreach (var item in modelo.ConfigSetup.genomeFiles)
                    {
                        HeatMapController mapaNuevo = new HeatMapController();
                        var listaSeleccionado = data.Where(q => q.Coord.Any(t => t.Chromosome == item)).ToList();
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
    }
}
