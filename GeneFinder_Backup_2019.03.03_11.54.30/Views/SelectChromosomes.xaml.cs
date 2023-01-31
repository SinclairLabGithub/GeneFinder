using GeneFinder.Models;
using GeneFinder.Viewmodels;
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
    /// Interaction logic for SelectChromosomes.xaml
    /// </summary>
    public partial class SelectChromosomes : Window
    {
        internal CompleteViewModel modelo = new CompleteViewModel();

        public SelectChromosomes()
        {
            InitializeComponent();
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
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

        private void genomeFilesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (modelo.ConfigSetup == null)
            {
                modelo.ConfigSetup = new ParametersClass();
            }

            modelo.ConfigSetup.genomeFiles = genomeFilesListBox.SelectedItems.Cast<string>().ToList();
        }

        private void textBoxThreshold_ValueChanged(object sender, Telerik.Windows.Controls.RadRangeBaseValueChangedEventArgs e)
        {
            if (modelo.ConfigSetup != null && textBoxThreshold.Value != null)
            {
                modelo.ConfigSetup.conservationThreshold = (float)textBoxThreshold.Value;
            }
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

        private void addKozakButton_Loaded(object sender, RoutedEventArgs e)
        {
            upStreamNumericUpDown.Value = modelo.ConfigSetup.basesToCheck;
            textBoxThreshold.Value = modelo.ConfigSetup.conservationThreshold;
            listKozak.Items.Clear();
            foreach (string kozak in modelo.ConfigSetup.kozaks)
            {
                listKozak.Items.Add(kozak);
            }
        }
    }
}
