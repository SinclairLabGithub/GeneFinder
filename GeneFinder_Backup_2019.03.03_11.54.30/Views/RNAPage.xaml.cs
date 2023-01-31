using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for RNAPage.xaml
    /// </summary>
    public partial class RNAPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        internal MainWindow ventanaPrincipal;

        public RNAPage()
        {
            InitializeComponent();
        }

        private void addConditionButton_Click(object sender, RoutedEventArgs e)
        {
            string conditionName = TextBoxConditionName.Text;
            if (conditionName.Length == 0)
            {
                MessageBox.Show("Must add a name for the condition.", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int mapQuality = (int)mapQualityNumUpDown.Value;
                modelo.ConfigSetup.conditionsName.Add(conditionName);
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.Filter = "sam file|*.sam";
                if (openDialog.ShowDialog() == true)
                {
                    busyIndicator.IsBusy = true;
                    var worker = new BackgroundWorker();

                    worker.DoWork += (s, ev) =>
                        {
                            RNA.MatchRNAtoSmorfs(openDialog.FileName, conditionName, mapQuality, modelo.smorfsList);
                        };

                    worker.RunWorkerCompleted += (s, ev) =>
                    {
                        busyIndicator.IsBusy = false;
                        UpdateGraph();
                    };

                    worker.RunWorkerAsync();
                }
            
            }
        }

        private void UpdateGraph()
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0 && modelo.ConfigSetup.conditionsName.Count > 0)
            {
                coverSeries.DataPoints.Clear();
                int universo = modelo.smorfsList.Count(p => p.Coord.Count > 0);
                int cuantosHits = modelo.smorfsList.Count(q => q.RnaHits != null);
                coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Covered: {0} - {1}%", cuantosHits, cuantosHits * 100 / universo), Value = cuantosHits });
                int demas = universo - cuantosHits;
                coverSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Not Covered: {0} - {1}%", demas, demas * 100 / universo), Value = demas });

                hitsSeries.DataPoints.Clear();
                int cuantosNames = modelo.smorfsList.Count(q => q.RnaConditionNames != null);
                hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Hit: {0} - {1}%", cuantosNames, cuantosNames * 100 / universo), Value = cuantosNames });
                demas = universo - cuantosNames;
                hitsSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("No Hit: {0} - {1}%", demas, demas * 100 / universo), Value = demas });

                graphGrid.Visibility = Visibility.Visible;
                ListBoxConditionsAdded.ItemsSource = modelo.ConfigSetup.conditionsName;
            }
        }
    }
}
