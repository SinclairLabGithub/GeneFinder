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

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for SelectSmorfsWindow.xaml
    /// </summary>
    public partial class SelectSmorfsWindow : Window
    {
        public string specieName= "";

        public SelectSmorfsWindow()
        {
            InitializeComponent();
        }



        private void acceptButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void ExportGridButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "CSV file|*.csv|Fasta files|*.fa";
            dialog.Title = "Save file";
            dialog.AddExtension = true;
            dialog.DefaultExt = "csv";

            //string extension = "csv";
            //SaveFileDialog dialog = new SaveFileDialog()
            //{
            //    DefaultExt = extension,
            //    Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "CSV file"),
            //    FilterIndex = 1
            //};

            if (dialog.ShowDialog() == true)
            {
                if (dialog.FileName.EndsWith(".csv"))
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

                if (dialog.FileName.EndsWith(".fa"))
                {
                    using (var wr = new StreamWriter(dialog.FileName))
                    {
                        foreach (var item in GridSmorf.Items.Cast<smorf>().ToList())
                        {
                            wr.WriteLine(">{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}",
                                        item.Id,
                                        specieName,
                                        item.Length.ToString(),
                                        item.MafSource,
                                        item.Chromosome,
                                        item.Position.ToString(),
                                        item.Strand,
                                        item.CodingScore.ToString()
                                        );
                            wr.WriteLine(item.Sequence);
                        }
                    }
                }

            }
        }
    }
}
