using GeneFinder.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace GeneFinder
{
    /// <summary>
    /// Interaction logic for SmorfListWindow.xaml
    /// </summary>
    public partial class SmorfListWindow : Window
    {
        internal ParametersClass parameters;

        public SmorfListWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CalculateCharts();
        }

        private void CalculateCharts()
        {
            List<PossibleSmorf> data = GridSmorf.Items.Cast<PossibleSmorf>().ToList();

            if (data != null && data.Count > 0)
            {
                int minSimilarity = data.Min(q => q.similarity);
                int maxSimilarity = data.Max(q => q.similarity);

                horizontalAxisSimilarity.Minimum = minSimilarity;
                horizontalAxisSimilarity.Maximum = maxSimilarity;

                serieSimilarity.DataPoints.Clear();

                for (int i = minSimilarity; i <= maxSimilarity; i++)
                {
                    serieSimilarity.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.similarity == i),  }); //points.Add(new HistogramValuePoint(i, data.Count(q => q.similarity == i)));
                }


                int minLength = data.Min(q => q.sequenceLength);
                int maxLength = data.Max(q => q.sequenceLength);

                horizontalAxisLength.Minimum = minLength;
                horizontalAxisLength.Maximum = maxLength;

                serieLength.DataPoints.Clear();

                for (int i = minLength; i <= maxLength; i = i +3)
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


                strandSeries.DataPoints.Clear();

                int numPositive = data.Count(q => q.strand == '+');
                strandSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("+: {0}", numPositive), Value = numPositive });
                int numNegative = data.Count(q => q.strand == '-');
                strandSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("-: {0}", numNegative), Value = numNegative });


                int minPosition = data.Min(q => q.startPosition);
                int maxPosition = data.Max(q => q.startPosition);
                int stepPosition = (maxPosition - minPosition) / 500;

                horizontalAxisDistribution.Minimum = minPosition;
                horizontalAxisDistribution.Maximum = maxPosition + stepPosition;


                serieDistribution.DataPoints.Clear();

                for (int i = minPosition; i < maxPosition; i+= stepPosition)
                {
                    serieDistribution.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = i, YValue = data.Count(q => q.startPosition >= i && q.startPosition < i + stepPosition) });
                }
            }
            
            
        }

        private void GridSmorf_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            CalculateCharts();
        }

        private void exportFastaButton_Click(object sender, RoutedEventArgs e)
        {
            List<PossibleSmorf> data = GridSmorf.Items.Cast<PossibleSmorf>().ToList();

            if (data != null)
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
                        foreach (var item in data)
                        {
                            string header = string.Format(">{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", parameters.specie1, item.id, item.chromosome, item.strand, item.startPosition, item.endPosition, item.sequenceLength, item.similarity, item.startCodon, item.stopCodon, item.sourceFile);
                            outfile.WriteLine(header);
                            outfile.WriteLine(item.specie1Content);
                            if (!string.IsNullOrEmpty(item.specie2Content))
                            {
                                header = string.Format(">{0}|{1}", parameters.specie2, item.id);
                                outfile.WriteLine(header);
                                outfile.WriteLine(item.specie2Content);
                            }
                            if (!string.IsNullOrEmpty(item.specie3Content))
                            {
                                header = string.Format(">{0}|{1}", parameters.specie3, item.id);
                                outfile.WriteLine(header);
                                outfile.WriteLine(item.specie3Content);
                            }
                            
                        }
                    }



                        //foreach (var item in data)
                        //{
                        //    string header = string.Format(">{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}", parameters.specie1, item.id, item.chromosome, item.strand, item.startPosition, item.endPosition, item.sequenceLength, item.similarity, item.startCodon, item.stopCodon, item.sourceFile);
                        //    csv.AppendLine(header);
                        //    csv.AppendLine(item.specie1Content);
                        //    header = string.Format(">{0}|{1}", parameters.specie2, item.id);
                        //    csv.AppendLine(header);
                        //    csv.AppendLine(item.specie2Content);
                        //    header = string.Format(">{0}|{1}", parameters.specie3, item.id);
                        //    csv.AppendLine(header);
                        //    csv.AppendLine(item.specie3Content);
                        //}

                    //File.WriteAllText(dialog.FileName, csv.ToString());

                }

                
            }

        }

        private void ExportGridButton_Click(object sender, RoutedEventArgs e)
        {
            string extension = "csv";
            SaveFileDialog dialog = new SaveFileDialog()
            {
                DefaultExt = extension,
                Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "CSV file"),
                FilterIndex = 1
            };

            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    GridSmorf.Export(stream,
                     new GridViewExportOptions()
                     {
                         Format = ExportFormat.Csv,
                         ShowColumnHeaders = true,
                         ShowColumnFooters = true,
                         ShowGroupFooters = false,
                     });
                }
            }
        }
    }
}
