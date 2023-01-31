using GeneFinder.Models;
using GeneFinder.Viewmodels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        static object lockObj = new object();
        internal MainWindow ventanaPrincipal;
        internal List<SmorfsList> listaPrincipal;

        public ReportPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
            {
                listaPrincipal = new List<SmorfsList>();
                foreach (var item in modelo.smorfsList)
                {
                    List<SmorfsList> agregados = SmorfsList.Generar(item);
                    listaPrincipal.Add(new SmorfsList(item));
                }
                GridSmorf.ItemsSource = listaPrincipal;
            }
        }

        private void GridSmorf_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {

        }

        private void GridSmorf_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangeEventArgs e)
        {
            AddCommentButton.Visibility = Visibility.Visible;
            CommentTextBox.Visibility = Visibility.Collapsed;
            SaveCommentButton.Visibility = Visibility.Collapsed;

            if (GridSmorf.SelectedItem != null)
            {
                smorf elemento = (GridSmorf.SelectedItem as SmorfsList).data;

                IdLabel.Text = elemento.Id;
                fileSourceLabel.Text = elemento.MafSource;
                chromosomeLabel.Text = elemento.Chromosome;
                strandLabel.Text = elemento.Strand;
                positionLabel.Text = elemento.Position.ToString();
                lengthLabel.Text = elemento.Length.ToString();
                conservationCheckLabel.Text = elemento.FlagConservation.ToString();
                exonOverlapCheckLabel.Text = elemento.FlagOverlapExonEitherStrand.ToString();
                kozakScoreCheckLabel.Text = elemento.KozakScore.ToString();
                gcContentCheckLabel.Text = elemento.GCcontent1.ToString();
                similarityLabel.Text = string.Format("{0}%", elemento.Similarity);
                codingCodonsLabel.Text = elemento.CodingCodonsPercentage.ToString();
                conservationSequencesLabel.Text = elemento.ConservationAverage.ToString();
                previosLabel.Text = elemento.ConservationPrevious.ToString();
                posteriorLabel.Text = elemento.ConservationPosterior.ToString();
                annotationLabel.Text = elemento.Annotation;

                SmorfsList elementoLista = (GridSmorf.SelectedItem as SmorfsList);

                if (elementoLista.Comment != null && elementoLista.Comment.Length > 0)
                {
                    AddCommentButton.Visibility = Visibility.Collapsed;
                    CommentTextBox.Visibility = Visibility.Visible;
                    SaveCommentButton.Visibility = Visibility.Visible;
                    CommentTextBox.Text = elementoLista.Comment;
                }
                else
                {
                    CommentTextBox.Text = "";
                }


                contenedorControles.Children.Clear();
                if (elemento != null)
                {
                    int upstream = (elemento.ExpandedAnotation.Length - elemento.Sequence.Length) / 2;
                    int llevo = 0;
                    int llevoAdentro = 0;
                    int largo = elemento.ExpandedAnotation.Length;
                    while (llevo * 3 < largo)
                    {
                        PedazoReporteControl nuevo = new PedazoReporteControl();
                        if (llevo * 3 < upstream - 9)
                        {
                            //if (llevo == 0)
                            //{
                            //    nuevo.Inicio = true;
                            //    nuevo.Final = false;
                            //}
                            //nuevo.Exon = elemento.ExpandedAnotation.Substring(llevo * 3, 3);
                            //nuevo.Intron = elemento.ExpandedAnotationReverse.Substring(llevo * 3, 3);
                            nuevo.InicioProteina = false;
                            nuevo.FinalProteina = false;
                            nuevo.Proteina = "";
                            nuevo.Cadena = "   ";
                            nuevo.CadenaDos = "   ";
                            nuevo.CadenaTres = "   ";
                            nuevo.Coding = 0;
                            nuevo.Conservation = string.Format("{0}{1}{2}", elemento.ExpandedConservation[llevo].ToString(), elemento.ExpandedConservation[llevo + 1].ToString(), elemento.ExpandedConservation[llevo + 2].ToString());
                        }
                        else
                        {
                            if (llevo * 3 < upstream)
                            {
                                //nuevo.Inicio = false;
                                //nuevo.Final = false;
                                //nuevo.Exon = elemento.ExpandedAnotation.Substring(llevo * 3, 3);
                                //nuevo.Intron = elemento.ExpandedAnotationReverse.Substring(llevo * 3, 3);
                                nuevo.InicioProteina = false;
                                nuevo.FinalProteina = false;
                                nuevo.Proteina = "";
                                nuevo.Cadena = elemento.KozakSequence.Substring(llevo * 3 - upstream + 9, 3);
                                nuevo.CadenaDos = "   ";
                                nuevo.CadenaTres = "   ";
                                nuevo.Coding = 0;
                                nuevo.Conservation = string.Format("{0}{1}{2}", elemento.ExpandedConservation[llevo].ToString(), elemento.ExpandedConservation[llevo + 1].ToString(), elemento.ExpandedConservation[llevo + 2].ToString());
                            }
                            else
                            {
                                if (llevoAdentro * 3 < elemento.Sequence.Length)
                                {
                                    //nuevo.Inicio = false;
                                    //nuevo.Final = false;
                                    //nuevo.Exon = elemento.ExpandedAnotation.Substring(llevo * 3, 3);
                                    //nuevo.Intron = elemento.ExpandedAnotationReverse.Substring(llevo * 3, 3);
                                    nuevo.InicioProteina = false;
                                    nuevo.FinalProteina = false;
                                    nuevo.Proteina = elemento.SequenceAsProtein[llevoAdentro].ToString();
                                    nuevo.Cadena = elemento.Sequence.Substring(llevoAdentro * 3, 3);
                                    nuevo.CadenaDos = elemento.SequenceSecondSpecies.Substring(llevoAdentro * 3, 3);
                                    nuevo.CadenaTres = elemento.SequenceThirdSpecies.Substring(llevoAdentro * 3, 3);
                                    nuevo.Coding = elemento.ExpandedCodingScores[llevoAdentro];
                                    nuevo.Conservation = string.Format("{0}{1}{2}", elemento.ExpandedConservation[llevo].ToString(), elemento.ExpandedConservation[llevo + 1].ToString(), elemento.ExpandedConservation[llevo + 2].ToString());
                                    if (llevoAdentro == 0)
                                    {
                                        nuevo.InicioProteina = true;
                                    }
                                    if (llevoAdentro * 3 == elemento.Sequence.Length - 3)
                                    {
                                        nuevo.FinalProteina = true;
                                    }
                                    llevoAdentro++;
                                }
                                else
                                {
                                    //nuevo.Inicio = false;
                                    //nuevo.Final = false;
                                    //if (llevo * 3 == largo - 3)
                                    //{
                                    //    nuevo.Final = true;
                                    //}
                                    //nuevo.Exon = elemento.ExpandedAnotation.Substring(llevo * 3, 3);
                                    //nuevo.Intron = elemento.ExpandedAnotationReverse.Substring(llevo * 3, 3);
                                    nuevo.InicioProteina = false;
                                    nuevo.FinalProteina = false;
                                    nuevo.Proteina = "";
                                    nuevo.Cadena = "   ";
                                    nuevo.CadenaDos = "   ";
                                    nuevo.CadenaTres = "   ";
                                    nuevo.Coding = 0;
                                    nuevo.Conservation = string.Format("{0}{1}{2}", elemento.ExpandedConservation[llevo].ToString(), elemento.ExpandedConservation[llevo + 1].ToString(), elemento.ExpandedConservation[llevo + 2].ToString());
                                }
                            }
                        }
                        contenedorControles.Children.Add(nuevo);
                        llevo++;
                    }
                    
                }
            }
            
        }

        private void ShowSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            List<SmorfsList> data = GridSmorf.Items.Cast<SmorfsList>().ToList();

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Fasta files|*.fasta";
            dialog.Title = "Save fasta file";
            dialog.AddExtension = true;
            dialog.DefaultExt = "fasta";

            if (dialog.ShowDialog() == true)
            {
                using (var wr = new StreamWriter(dialog.FileName))
                {
                    foreach (var items in data)
                    {
                        var item = items.data;
                        wr.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
                                    item.MetaData,
                                    item.Id,
                                    modelo.ConfigSetup.specie1,
                                    item.Length.ToString(),
                                    item.Similarity.ToString(),
                                    item.MafSource,
                                    item.Chromosome,
                                    item.Position.ToString(),
                                    item.Strand,
                                    item.NumberRepetition.ToString(),
                                    item.TotalRepetitions.ToString(),
                                    item.FlagUniqueSequence.ToString(),
                                    item.CodingScore.ToString(),
                                    item.NumCodons.ToString()
                                                        );
                        for (int i = 0; i < item.NumCodons; i++)
                        {
                            wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
                        }
                        wr.Write("{0}\t", item.Coord.Count);
                        for (int i = 0; i < item.Coord.Count; i++)
                        {
                            wr.Write("{0}\t{1}\t{2}\t", item.Coord[i].Chromosome, item.Coord[i].Position, item.Coord[i].Strand);
                        }
                        wr.Write("{0}\t{1}\t{2}\t{3}\t",
                            item.ConservationAverage,
                            item.ConservationPrevious,
                            item.ConservationPosterior,
                            item.FlagConservation
                            );
                        if (item.ExpandedConservation != null)
                        {
                            wr.Write("{0}\t", item.ExpandedConservation.Length);
                            for (int i = 0; i < item.ExpandedConservation.Length; i++)
                            {
                                wr.Write("{0}\t", item.ExpandedConservation[i].ToString());
                            }
                        }
                        else
                        {
                            wr.Write("0\t");
                        }

                        wr.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t",
                            item.OverlapExon,
                            item.OverlapIntron,
                            item.OverlapIntergenic,
                            item.OverlapUtr5,
                            item.OverlapUtr3,
                            item.OverlapExonReverse,
                            item.Annotation,
                            item.ExpandedAnotation,
                            item.ExpandedAnotationReverse,
                            item.FlagOverlapExonEitherStrand,
                            item.KozakSequence,
                            item.KozakScore,
                            item.GCcontent1
                            );

                        wr.WriteLine();
                        wr.WriteLine(item.Sequence);
                        //if (item.Coord.Count > 0)
                        //{
                        //    if (item.Coord.Count > 0)
                        //    {
                        //        wr.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
                        //            item.MetaData,
                        //            item.Id,
                        //            modelo.ConfigSetup.specie1,
                        //            item.Length.ToString(),
                        //            item.Similarity.ToString(),
                        //            item.MafSource,
                        //            item.Chromosome,
                        //            item.Position.ToString(),
                        //            item.Strand,
                        //            item.NumberRepetition.ToString(),
                        //            item.TotalRepetitions.ToString(),
                        //            item.FlagUniqueSequence.ToString(),
                        //            item.CodingScore.ToString(),
                        //            item.NumCodons.ToString()
                        //                                );
                        //        for (int i = 0; i < item.NumCodons; i++)
                        //        {
                        //            wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
                        //        }
                        //        wr.Write("{0}\t", item.Coord.Count);
                        //        for (int i = 0; i < item.Coord.Count; i++)
                        //        {
                        //            wr.Write("{0}\t{1}\t{2}\t", item.Coord[i].Chromosome, item.Coord[i].Position, item.Coord[i].Strand);
                        //        }
                        //        wr.Write("{0}\t{1}\t{2}\t{3}\t",
                        //            item.ConservationAverage,
                        //            item.ConservationPrevious,
                        //            item.ConservationPosterior,
                        //            item.FlagConservation
                        //            );
                        //        if (item.ExpandedConservation != null)
                        //        {
                        //            wr.Write("{0}\t", item.ExpandedConservation.Length);
                        //            for (int i = 0; i < item.ExpandedConservation.Length; i++)
                        //            {
                        //                wr.Write("{0}\t", item.ExpandedConservation[i].ToString());
                        //            }
                        //        }
                        //        else
                        //        {
                        //            wr.Write("0\t");
                        //        }

                        //        wr.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t",
                        //            item.OverlapExon,
                        //            item.OverlapIntron,
                        //            item.OverlapIntergenic,
                        //            item.OverlapUtr5,
                        //            item.OverlapUtr3,
                        //            item.OverlapExonReverse,
                        //            item.Annotation,
                        //            item.ExpandedAnotation,
                        //            item.ExpandedAnotationReverse,
                        //            item.FlagOverlapExonEitherStrand,
                        //            item.KozakSequence,
                        //            item.KozakScore,
                        //            item.GCcontent1
                        //            );

                        //        wr.WriteLine();
                        //        wr.WriteLine(item.Sequence);
                        //        //wr.WriteLine(">{0}", modelo.ConfigSetup.specie2);
                        //        ////wr.WriteLine(">Chimp");
                        //        //wr.WriteLine(item.SequenceSecondSpecies);
                        //        //wr.WriteLine(">{0}", modelo.ConfigSetup.specie3);
                        //        ////wr.WriteLine(">Rhesus");
                        //        //wr.WriteLine(item.SequenceThirdSpecies);
                        //    }

                        //    //wr.Write(">Human|{0}|{1}",//"{0}\t{1}\t{2}\t{3}\t",//{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
                        //    //                        //item.MetaData,
                        //    //                        //item.Id,
                        //    //                        //modelo.ConfigSetup.specie1,
                        //    //                        item.Annotation,
                        //    //                        item.CodingScore
                        //    //                        //item.NumCodons.ToString()
                        //    //    //item.Similarity.ToString(),
                        //    //    //item.MafSource,
                        //    //    //item.Chromosome,
                        //    //    //item.Position.ToString(),
                        //    //    //item.Strand,
                        //    //    //item.NumberRepetition.ToString(),
                        //    //    //item.TotalRepetitions.ToString(),
                        //    //    //item.FlagUniqueSequence.ToString(),
                        //    //    //item.CodingScore.ToString(),
                        //    //    //item.NumCodons.ToString()
                        //    //                        );
                        //    ////for (int i = 0; i < item.NumCodons; i++)
                        //    ////{
                        //    ////    wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
                        //    ////}
                        //    //wr.WriteLine();
                        //    //wr.WriteLine(item.Sequence);
                        //    ////wr.WriteLine(">{0}", modelo.ConfigSetup.specie2);
                        //    //wr.WriteLine(">Chimp");
                        //    //wr.WriteLine(item.SequenceSecondSpecies);
                        //    ////wr.WriteLine(">{0}", modelo.ConfigSetup.specie3);
                        //    //wr.WriteLine(">Rhesus");
                        //    //wr.WriteLine(item.SequenceThirdSpecies);
                        //}
                    }
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

        private void MakeTrackButton_Click(object sender, RoutedEventArgs e)
        {
            List<SmorfsList> data = GridSmorf.Items.Cast<SmorfsList>().ToList();

            List<string> chromosomes = data.Select(q => q.Chromosome).Distinct().ToList();

            double min = data.Min(q => q.CodingScore);
            double max = data.Max(q => q.CodingScore);

            ConfigureTrackWindow ventana = new ConfigureTrackWindow();
            ventana.valuesTextblock.Text = String.Format("Values from {0} to {1}.", min.ToString("F"), max.ToString("F"));
            bool aceptado = (bool)ventana.ShowDialog();

            if (aceptado)
            {
                string extension = "track";
                SaveFileDialog dialog = new SaveFileDialog()
                {
                    DefaultExt = extension,
                    Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, "Track file"),
                    FilterIndex = 1
                };

                if (dialog.ShowDialog() == true)
                {
                    foreach (var chromosome in chromosomes)
                    {
                        List<SmorfsList> dataByChromosome = data.FindAll(q => q.Chromosome == chromosome);

                        int startPoint = dataByChromosome.Min(q => q.StartPosition);
                        int endPoint = dataByChromosome.Max(q => q.EndPosition);

                        double multiplicador = 1000d / ((double)max - (double)min);

                        if (ventana.radioValue.IsChecked == true)
                        {
                            using (var wr = new StreamWriter(dialog.FileName + "_" + chromosome))
                            {
                                wr.WriteLine("browser position {0}:{1}-{2}", chromosome, startPoint, endPoint);
                                wr.WriteLine("track name=Smorfs description=\"Color by Value\" color=0,60,120 useScore=1");
                                
                                foreach (SmorfsList item in dataByChromosome)
                                {
                                    int valor = (int)((item.CodingScore - min) * multiplicador);
                                    wr.WriteLine("{0} {1} {2} smorf{3} {4} {5}", chromosome, item.StartPosition, item.EndPosition, item.Id, valor, item.Strand);
                                }
                            }
                        }
                        else
                        {
                            using (var wr = new StreamWriter(dialog.FileName + "_" + chromosome))
                            {
                                double threshold1 = (double)ventana.threshold1NumericUpDown.Value;
                                double threshold2 = (double)ventana.threshold1NumericUpDown.Value;
                                double threshold3 = (double)ventana.threshold1NumericUpDown.Value;
                                double threshold4 = (double)ventana.threshold1NumericUpDown.Value;

                                wr.WriteLine("browser position {0}:{1}-{2}", chromosome, startPoint, endPoint);
                                wr.WriteLine("track name=Smorfs description=\"Color by Thresholds\" itemRgb=\"On\"");
                                int voy = 1;
                                foreach (SmorfsList item in data)
                                {
                                    double valor = item.CodingScore;
                                    string color = "";
                                    if (valor < threshold1)
                                    {
                                        color = "51,153,255";
                                    }
                                    else
                                    {
                                        if (valor < threshold2)
                                        {
                                            color = "1,67,88";
                                        }
                                        else
                                        {
                                            if (valor < threshold3)
                                            {
                                                color = "31,138,113";
                                            }
                                            else
                                            {
                                                if (valor < threshold4)
                                                {
                                                    color = "189,214,59";
                                                }
                                                else
                                                {
                                                    color = "242,116,33";
                                                }
                                            }
                                        }
                                    }
                                    valor = (int)((item.CodingScore - min) * multiplicador);
                                    wr.WriteLine("{0} {1} {2} smorf{3} {4} {5} {1} {2} {6}", chromosome, item.StartPosition, item.EndPosition, item.Id, valor, item.Strand, color);
                                }
                            }
                        }
                    }

                }
            }
            
        }

        private void AddCommentButton_Click(object sender, RoutedEventArgs e)
        {
            AddCommentButton.Visibility = Visibility.Collapsed;
            CommentTextBox.Visibility = Visibility.Visible;
            SaveCommentButton.Visibility = Visibility.Visible;
        }

        private void SaveCommentButton_Click(object sender, RoutedEventArgs e)
        {
            if (GridSmorf.SelectedItem != null)
            {
                SmorfsList elementoLista = (GridSmorf.SelectedItem as SmorfsList);
                elementoLista.Comment = CommentTextBox.Text;
            }
            
        }

        private void ChangeIdButton_Click(object sender, RoutedEventArgs e)
        {
            NewIdDesigner ventana = new NewIdDesigner();
            if ((bool)ventana.ShowDialog())
            {
                int num = 1;
                foreach (var item in GridSmorf.Items)
                {
                    SmorfsList s = item as SmorfsList;
                    s.Id = ventana.NewLabelTextBox.Text + "_" + num;
                }
            }
        }
    }
}
