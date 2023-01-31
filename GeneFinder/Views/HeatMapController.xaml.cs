using System;
using System.Collections.Generic;
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
using GeneFinder.Models;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for HeatMapController.xaml
    /// </summary>
    public partial class HeatMapController : UserControl
    {
        public List<smorf> data { get; set; }

        public int longitud = 0;

        public string chr;

        public bool graphInstances = true;

        public HeatMapController()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (data.Count > 0)
            {
                double minposition = 0;
                double maxposition = longitud;

                if (longitud == 0)
                {
                    minposition = data.Min(q => q.Position);
                    maxposition = data.Max(q => q.Position);

                    startPositionLabel.Content = minposition.ToString("N0");
                    startPositionLabel.Visibility = Visibility.Visible;
                    
                }
                endPositionLabel.Content = maxposition.ToString("N0");
                endPositionLabel.Visibility = Visibility.Visible;

                ejeHorizontal.Minimum = minposition;
                ejeHorizontal.Maximum = maxposition;
                

                ejeVertical.Minimum = 0;

                int numSteps = 100;

                double stepSize = (maxposition - minposition) / (double)numSteps;

                ejeHorizontal.LabelInterval = numSteps - 1;
                

                List<PointHeatMap> puntos = new List<PointHeatMap>();

                sequencesDistribution.DataPoints.Clear();

                int maxValue = 0;
                if (graphInstances)
                {
                    for (double i = minposition; i < maxposition; i += stepSize)
                    {
                        int value = data.Count(q => q.Coord.Any(t => t.Chromosome == chr && t.Position > i && t.Position <= i + stepSize));
                        sequencesDistribution.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = (int)i, YValue = value });
                        puntos.Add(new PointHeatMap("", (int)i, value));
                        if (maxValue < value) maxValue = value;
                    }
                }
                else
                {
                    for (double i = minposition; i < maxposition; i += stepSize)
                    {
                        int value = data.Count(q => q.Position > i && q.Position <= i + stepSize);
                        sequencesDistribution.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = (int)i, YValue = value });
                        puntos.Add(new PointHeatMap("", (int)i, value));
                        if (maxValue < value) maxValue = value;
                    }
                }
                
                if (maxValue > 1)
                {
                    ejeVertical.Maximum = maxValue;
                    ejeVertical.MajorStep = maxValue;
                    maximumLabel.Content = maxValue.ToString("N0");

                }
                else
                {
                    ejeVertical.Maximum = 2;
                    ejeVertical.MajorStep = 2;
                    maximumLabel.Content = 2;
                }
                

                mapaData.ItemsSource = puntos;
            }
            
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ChromosomeWindow ventana = new ChromosomeWindow();
            ventana.graphInstances = graphInstances;
            ventana.Title = chr;
            ventana.titleLabel.Content = string.Format("{0} - {1}", chr, data.Count);
            ventana.chr = this.chr;
            ventana.data = this.data;
            ventana.longitud = this.longitud;
            ventana.Show();
        }
    }
}
