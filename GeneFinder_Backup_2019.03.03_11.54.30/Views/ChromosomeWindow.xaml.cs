using GeneFinder.Models;
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
using System.Windows.Shapes;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for ChromosomeWindow.xaml
    /// </summary>
    public partial class ChromosomeWindow : Window
    {
        public List<smorf> data { get; set; }

        public int longitud = 0;

        public string chr;

        public bool graphInstances = true;

        public ChromosomeWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (data.Count > 0)
            {
                double minposition = 0;
                double maxposition = longitud;

                if (longitud == 0)
                {
                    minposition = data.Min(q => q.Position);
                    maxposition = data.Max(q => q.Position);

                    startPositionLabel.Content = minposition.ToString();
                    startPositionLabel.Visibility = Visibility.Visible;
                }
                endPositionLabel.Content = maxposition.ToString();
                endPositionLabel.Visibility = Visibility.Visible;

                ejeHorizontal.Minimum = minposition;
                ejeHorizontal.Maximum = maxposition;


                ejeVertical.Minimum = 0;

                int numSteps = 500;

                double stepSize = (maxposition - minposition) / (double)numSteps;

                ejeHorizontal.LabelInterval = 5;


                List<PointHeatMap> puntos = new List<PointHeatMap>();

                sequencesDistribution.DataPoints.Clear();

                int maxValue = 0;

                for (double i = minposition; i < maxposition; i += stepSize)
                {
                    if (graphInstances)
                    {
                        int value = data.Count(q => q.Coord.Any(t => t.Chromosome == chr && t.Position > i && t.Position <= i + stepSize));
                        sequencesDistribution.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() { XValue = (int)i, YValue = value });
                        puntos.Add(new PointHeatMap("", (int)i, value));
                        if (maxValue < value) maxValue = value;
                    }
                    else
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
                    maximumLabel.Content = maxValue;

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
    }
}
