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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for SelectScoreWindows.xaml
    /// </summary>
    public partial class SelectScoreWindow : Window
    {
        internal List<smorf> data;
        internal double scoretreshold;
        internal int percentage;
        internal int smorfnumber;

        public SelectScoreWindow()
        {
            InitializeComponent();
            
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void UpdateData(double score, int p)
        {
            double min = data.Min(q => q.CodingScore);
            double max = data.Max(q => q.CodingScore);
            double numSteps = 500;
            double stepLength = (max - min) / numSteps;

            horizontalAxisScore.Minimum = min;
            horizontalAxisScore.Maximum = max;

            int newPercantage = 0;
            double newScore = min;

            serieScore.DataPoints.Clear();
            for (double i = min; i < max; i += stepLength)
            {
                int pTemp = (int)(100 * data.Count(q => q.CodingScore > i) / (double)data.Count);
                serieScore.DataPoints.Add(new Telerik.Charting.ScatterDataPoint() {XValue = i, YValue= pTemp });//i, pTemp));
                if (i < score) newPercantage = pTemp;
                if (pTemp > p) newScore = i;
            }

            chartScore.Annotations.Clear();
                    chartScore.Annotations.Add(new CartesianGridLineAnnotation { Axis = chartScore.HorizontalAxis, Value = score, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 4 });

            
            
            if (score != scoretreshold)
            {
                scoretreshold = score;
                smorfNumberLabel.Content = data.Count(q => q.CodingScore > scoretreshold);
                percentage = newPercantage;
                textBoxPercentageThreshold.Value = percentage;
                return;
            }
            if (p != percentage)
            {
                scoretreshold = newScore;
                percentage = p;
                smorfNumberLabel.Content = data.Count(q => q.CodingScore > scoretreshold);
                textBoxThreshold.Value = scoretreshold;
                return;
            }
        }

        private void textBoxThreshold_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (data!= null && textBoxPercentageThreshold != null && textBoxThreshold != null)
            {
                double score = (double)textBoxThreshold.Value;
                int p = (int)textBoxPercentageThreshold.Value;
                UpdateData(score, p);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double score = (double)textBoxThreshold.Value;
            int p = (int)textBoxPercentageThreshold.Value;
            UpdateData(score, p);
        }

        private void textBoxPercentageThreshold_ValueChanged(object sender, RadRangeBaseValueChangedEventArgs e)
        {
            if (data != null && textBoxPercentageThreshold != null && textBoxThreshold != null)
            {
                double score = (double)textBoxThreshold.Value;
                int p = (int)textBoxPercentageThreshold.Value;
                UpdateData(score, p);
            }
        }

        

        
    }
}
