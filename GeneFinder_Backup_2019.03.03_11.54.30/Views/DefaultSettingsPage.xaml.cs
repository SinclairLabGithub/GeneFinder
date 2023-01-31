using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for DefaultSettings.xaml
    /// </summary>
    public partial class DefaultSettingsPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        internal ParametersClass parametros = new ParametersClass();
        internal MainWindow ventanaPrincipal;

        public DefaultSettingsPage()
        {
            InitializeComponent();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (StreamReader reader = new StreamReader("Data//DefaultSettings.txt"))
            {
                bool end = false;
                while (!end)
                {
                    string lector = reader.ReadLine();
                    switch (lector)
                    {
                        case "Version":
                            parametros.version = reader.ReadLine();
                            break;
                        case "Start Codons Number":
                            int numStartCodons = int.Parse(reader.ReadLine());
                            parametros.startCodons = new List<string>();
                            for (int i = 0; i < numStartCodons; i++)
                            {
                                parametros.startCodons.Add(reader.ReadLine());
                            }
                            break;
                        case "Stop Codons Number":
                            int numStopCodons = int.Parse(reader.ReadLine());
                            parametros.stopCodons = new List<string>();
                            for (int i = 0; i < numStopCodons; i++)
                            {
                                parametros.stopCodons.Add(reader.ReadLine());
                            }
                            break;
                        case "Min length":
                            parametros.minLenght = int.Parse(reader.ReadLine());
                            break;
                        case "Max length":
                            parametros.maxLenght = int.Parse(reader.ReadLine());
                            break;
                        case "Conservation Cutoff":
                            parametros.cutoff = double.Parse(reader.ReadLine());
                            break;
                        case "Genome Folder":
                            parametros.genomeFolder = reader.ReadLine();
                            break;
                        case "Annotation BigTableName":
                            parametros.annotationBigTableName = reader.ReadLine();
                            break;
                        case "Annotation SmallTableName":
                            parametros.annotationSmallTableName = reader.ReadLine();
                            break;
                        case "Conservation Folder":
                            parametros.conservationFolder = reader.ReadLine();
                            break;
                        case "Kozaks Number":
                            int numKozaks = int.Parse(reader.ReadLine());
                            parametros.kozaks = new List<string>();
                            for (int i = 0; i < numKozaks; i++)
                            {
                                parametros.kozaks.Add(reader.ReadLine());
                            }
                            break;
                        case "Bases to check":
                            parametros.basesToCheck = int.Parse(reader.ReadLine());
                            break;
                        case "Conservation Threshold":
                            parametros.conservationThreshold = float.Parse(reader.ReadLine());
                            break;
                        case "End Header":
                            end = true;
                            break;
                        default:

                            break;
                    }
                }
            }
            minLenght.Value = parametros.minLenght;
            maxLenght.Value = parametros.maxLenght;
            listStartCodons.Items.Clear();
            foreach (string startCodon in parametros.startCodons)
            {
                listStartCodons.Items.Add(startCodon);
            }
            listStopCodons.Items.Clear();
            foreach (string stopCodon in parametros.stopCodons)
            {
                listStopCodons.Items.Add(stopCodon);
            }
            textBoxConservation.Text = parametros.conservationFolder;
            textBoxGenome.Text = parametros.genomeFolder;
            //textBoxAnnotation.Text = parametros.annotationBigTableName.Substring(parametros.annotationBigTableName.LastIndexOf("\\")) + " , " + parametros.annotationSmallTableName.Substring(parametros.annotationSmallTableName.LastIndexOf("\\"));
            textBoxAnnotation.Text = parametros.annotationBigTableName + " , " + parametros.annotationSmallTableName;
            upStreamNumericUpDown.Value = parametros.basesToCheck;
            textBoxThreshold.Value = parametros.conservationThreshold;
            listKozak.Items.Clear();
            foreach (string kozak in parametros.kozaks)
            {
                listKozak.Items.Add(kozak);
            }
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            parametros.startCodons = new List<string>();
            foreach (string startCodon in listStartCodons.Items)
            {
                parametros.startCodons.Add(startCodon);
            }
            parametros.stopCodons = new List<string>();
            foreach (string stopCodon in listStopCodons.Items)
            {
                parametros.stopCodons.Add(stopCodon);
            }
            parametros.minLenght = (int)minLenght.Value;
            parametros.maxLenght = (int)maxLenght.Value;
            parametros.genomeFolder = textBoxGenome.Text;
            var annotationTables = textBoxAnnotation.Text.Split(',');
            parametros.annotationBigTableName = annotationTables[0].Trim();
            parametros.annotationSmallTableName = annotationTables[1].Trim();
            parametros.conservationFolder = textBoxConservation.Text;
            parametros.kozaks = new List<string>();
            foreach (string kozak in listKozak.Items)
            {
                parametros.kozaks.Add(kozak);
            }
            parametros.basesToCheck = (int)upStreamNumericUpDown.Value;
            parametros.conservationThreshold = (float)textBoxThreshold.Value;

            using (StreamWriter outputStream = new StreamWriter("Data//DefaultSettings.txt"))
            {
                outputStream.WriteLine("Version");
                outputStream.WriteLine(parametros.version);
                outputStream.WriteLine("Start Codons Number");
                outputStream.WriteLine(parametros.startCodons.Count);
                foreach (var salida in parametros.startCodons)
                {
                    outputStream.WriteLine(salida);
                }
                outputStream.WriteLine("Stop Codons Number");
                outputStream.WriteLine(parametros.stopCodons.Count);
                foreach (var salida in parametros.stopCodons)
                {
                    outputStream.WriteLine(salida);
                }
                outputStream.WriteLine("Min length");
                outputStream.WriteLine(parametros.minLenght);
                outputStream.WriteLine("Max length");
                outputStream.WriteLine(parametros.maxLenght);
                outputStream.WriteLine("Conservation Cutoff");
                outputStream.WriteLine(parametros.cutoff);
                outputStream.WriteLine("Genome Folder");
                outputStream.WriteLine(parametros.genomeFolder);
                outputStream.WriteLine("Annotation BigTableName");
                outputStream.WriteLine(parametros.annotationBigTableName);
                outputStream.WriteLine("Annotation SmallTableName");
                outputStream.WriteLine(parametros.annotationSmallTableName);
                outputStream.WriteLine("Conservation Folder");
                outputStream.WriteLine(parametros.conservationFolder);
                outputStream.WriteLine("Kozaks Number");
                outputStream.WriteLine(parametros.kozaks.Count());
                foreach (var salida in parametros.kozaks)
                {
                    outputStream.WriteLine(salida);
                }
                outputStream.WriteLine("Bases to check");
                outputStream.WriteLine(parametros.basesToCheck);
                outputStream.WriteLine("Conservation Threshold");
                outputStream.WriteLine(parametros.conservationThreshold);
                outputStream.WriteLine("End Header");
            }
            ventanaPrincipal.startOver();
        }

        private void BackGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ventanaPrincipal.startOver();
        }

        private void deleteKozakButton_Click(object sender, RoutedEventArgs e)
        {
            if (listKozak.SelectedItem == null)
            {
                MessageBox.Show("Select an item to delete", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                listKozak.Items.Remove(listKozak.SelectedItem);
            }
        }

        private void addKozakButton_Click(object sender, RoutedEventArgs e)
        {
            string newKozak = kozakTextBox.Text;
            if (newKozak.Length != 15)
            {
                MessageBox.Show("Lenght must be 15 characters", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string allowed = "ATGC";
                foreach (char c in newKozak)
                {
                    if (!allowed.Contains(c))
                    {
                        MessageBox.Show("Codon must only contains A, T, G or C", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                listKozak.Items.Add(newKozak);
            }
        }

        private void selectConservationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where conservation is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                
                textBoxConservation.Text = dialog.FileName;
            }

        }

        private void selectGenomeFolderButton_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            dialog.Title = "Select folder where genome is stored";
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                
                textBoxGenome.Text = dialog.FileName;
            }
        }

        private void selectAnnotationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select file for Big Table where annotation is stored";
            dialog.Filter = "TSV Files|*.tsv";
            bool result = (bool)dialog.ShowDialog();
            if (result)
            {
                string tempAnotation = dialog.FileName;

                dialog.Title = "Select file for Small Table where annotation is stored";
                dialog.Filter = "CSV File|*.csv";
                result = (bool)dialog.ShowDialog();
                if (result)
                {
                    tempAnotation = tempAnotation + " , " + dialog.FileName;
                    textBoxAnnotation.Text = tempAnotation;
                }
                
            }
        }

        private void createConservationFolderButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select conservation File";
            dialog.Filter = "BED files|*.bed";
            if (dialog.ShowDialog() == true)
            {
                functions.CreateConservationFile(dialog.FileName, parametros.conservationFolder);
                //ConservationAnnotationKozak.CreateConservationFile(dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            ventanaPrincipal.startOver();
        }

        private void createAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select Annotation TSV Table";
            dialog.Filter = "TSV files|*.tsv";
            if (dialog.ShowDialog() == true)
            {
                string tableName = dialog.FileName.Substring(dialog.FileName.LastIndexOf('\\') + 1);
                string rute = System.IO.Path.GetDirectoryName(dialog.FileName);
                string newLong = "NEW_" + tableName;//la misma tabla, pero con columnas extra
                string newShort = "Short_" + tableName + ".csv";//la tabla comprimida

                ReadTSV lector = new ReadTSV(rute, tableName, newLong, newShort);
                //functions.CreateConservationFile(dialog.FileName, parametros.conservationFolder);
                //ConservationAnnotationKozak.CreateConservationFile(dialog.FileName, dialog.FileName.Substring(0, dialog.FileName.LastIndexOf("//")));
            }
        }
    }
}
