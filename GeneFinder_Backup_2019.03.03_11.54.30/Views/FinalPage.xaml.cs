﻿using GeneFinder.Models;
using GeneFinder.Viewmodels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Xml.Serialization;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.ChartView;

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for FinalPage.xaml
    /// </summary>
    public partial class FinalPage : Page
    {
        internal CompleteViewModel modelo = new CompleteViewModel();
        static object lockObj = new object();
        internal MainWindow ventanaPrincipal;
        internal List<SmorfsList> listaPrincipal;
        List<smorf> data;
        Annotate annotation;

        public FinalPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (modelo.smorfsList != null && modelo.smorfsList.Count > 0)
                {
                    listaPrincipal = new List<SmorfsList>();
                    foreach (var item in modelo.smorfsList)
                    {
                        //List<SmorfsList> agregados = SmorfsList.Generar(item);
                        listaPrincipal.Add(new SmorfsList(item));
                    }
                    GridSmorf.ItemsSource = listaPrincipal;
                    annotation = new Annotate(modelo.ConfigSetup.annotationSmallTableName, modelo.ConfigSetup.annotationBigTableName);
                    UpdatePlots();
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
                throw;
            }
            
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

                StackPanelGenes.Children.Clear();

                for (int i = 0; i < elemento.annotationResult.Count; i++)
                {
                    for (int j = 0; j < elemento.annotationResult[i].Count; j++)
                    {
                        if (j == 0)
                        {
                            System.Windows.Controls.Label newGene = new System.Windows.Controls.Label();
                            newGene.FontWeight = FontWeights.Bold;
                            newGene.FontSize = 12;
                            newGene.Height = 26;
                            newGene.Content = elemento.annotationResult[i][j][0];
                            StackPanelGenes.Children.Add(newGene);
                        }
                        System.Windows.Controls.Label newTranscript = new System.Windows.Controls.Label();
                        newTranscript.FontWeight = FontWeights.Light;
                        newTranscript.FontSize = 12;
                        newTranscript.Height = 50;
                        string transcriptName = elemento.annotationResult[i][j][1];
                        if (elemento.annotationResult[i][j][4] == "E")
                        {
                            transcriptName += string.Format(" ({0})", elemento.annotationResult[i][j][5]);
                        }
                        newTranscript.Content = transcriptName;
                        newTranscript.VerticalContentAlignment = VerticalAlignment.Center;
                        StackPanelGenes.Children.Add(newTranscript);
                    }
                }

                //foreach (var gene in elemento.rowRanges)
                //{
                //    int start = annotation.compressedTable.pStart[gene];
                //    int end = annotation.compressedTable.pEnd[gene];

                //    System.Windows.Controls.Label newGene = new System.Windows.Controls.Label();
                //    newGene.FontWeight = FontWeights.Bold;
                //    newGene.FontSize = 12;
                //    newGene.Height = 26;
                //    newGene.Content = annotation.table.rows[start].name; //aqui va name2;
                //    StackPanelGenes.Children.Add(newGene);

                //    for (int i = start; i <= end; i++)
                //    {
                //        System.Windows.Controls.Label newTranscript = new System.Windows.Controls.Label();
                //        newTranscript.FontWeight = FontWeights.Light;
                //        newTranscript.FontSize = 12;
                //        newTranscript.Height = 50;
                //        newTranscript.Content = annotation.table.rows[i].name;
                //        newTranscript.VerticalContentAlignment = VerticalAlignment.Center;
                //        StackPanelGenes.Children.Add(newTranscript);
                //    }
                //}

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


                try
                {
                    contenedorControles.Children.Clear();
                    if (elemento != null)
                    {
                        int upstream = 0; //(elemento.ExpandedAnotation.Length - elemento.Sequence.Length) / 2;
                        int llevo = 0;
                        int llevoAdentro = 0;
                        int largo = elemento.Length;//.ExpandedAnotation.Length;
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
                                        int inicioPedazo = elemento.Position + llevoAdentro * 3;
                                        foreach (var gene in elemento.rowRanges)
                                        {
                                            int start = annotation.compressedTable.pStart[gene];
                                            int end = annotation.compressedTable.pEnd[gene];


                                            for (int i = start; i <= end; i++)
                                            {
                                                PedazoAnotacionControl nuevoTranscrito = new PedazoAnotacionControl();
                                                nuevoTranscrito.Height = 50;
                                                if (i == start) nuevoTranscrito.Margin = new Thickness(0, 26, 0, 0);

                                                for (int interno = 0; interno < 3; interno++)
                                                {
                                                    int dentroPedazo = inicioPedazo + interno;

                                                    if (dentroPedazo == 46066910 || dentroPedazo == 46067091 || dentroPedazo == 46077514)
                                                    {
                                                        bool cambio = true;
                                                    }

                                                    int addEnd = 1;
                                                    int addStart = 0;

                                                    if (annotation.table.rows[i].strand == "-")
                                                    {
                                                        addEnd = 0;
                                                        addStart = 1;
                                                    }

                                                    bool puesto = false;

                                                    if (annotation.table.rows[i].UTR5[0] - addStart <= dentroPedazo)
                                                    {
                                                        if (dentroPedazo <= annotation.table.rows[i].UTR5[1] - addEnd)
                                                        {
                                                            if (elemento.Strand == "-" && annotation.table.rows[i].illDefined == false)
                                                            {
                                                                switch (interno)
                                                                {
                                                                    case 0:
                                                                        nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Yellow);
                                                                        break;
                                                                    case 1:
                                                                        nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Yellow);
                                                                        break;
                                                                    case 2:
                                                                        nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Yellow);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                switch (interno)
                                                                {
                                                                    case 0:
                                                                        nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Red);
                                                                        break;
                                                                    case 1:
                                                                        nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Red);
                                                                        break;
                                                                    case 2:
                                                                        nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Red);
                                                                        break;
                                                                    default:
                                                                        break;
                                                                }
                                                            }
                                                            
                                                            puesto = true;
                                                        }
                                                    }

                                                    if (!puesto)
                                                    {
                                                        if (annotation.table.rows[i].UTR3[0] - addStart < dentroPedazo)
                                                        {
                                                            if (dentroPedazo <= annotation.table.rows[i].UTR3[1] - addEnd)
                                                            {
                                                                if (elemento.Strand == "+")
                                                                {
                                                                    switch (interno)
                                                                    {
                                                                        case 0:
                                                                            nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Yellow);
                                                                            break;
                                                                        case 1:
                                                                            nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Yellow);
                                                                            break;
                                                                        case 2:
                                                                            nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Yellow);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    switch (interno)
                                                                    {
                                                                        case 0:
                                                                            nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Red);
                                                                            break;
                                                                        case 1:
                                                                            nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Red);
                                                                            break;
                                                                        case 2:
                                                                            nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Red);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                }
                                                                puesto = true;
                                                            }
                                                        }
                                                    }


                                                    if (!puesto)
                                                    {
                                                        int Pos = 0;
                                                        if (annotation.table.rows[i].intronInt != null)
                                                        {
                                                            while (!puesto && Pos < annotation.table.rows[i].intronInt.Count)
                                                            {
                                                                if (annotation.table.rows[i].intronInt[Pos].Range.From - addStart < dentroPedazo && dentroPedazo <= annotation.table.rows[i].intronInt[Pos].Range.To - addEnd)
                                                                {
                                                                    switch (interno)
                                                                    {
                                                                        case 0:
                                                                            nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Green);
                                                                            break;
                                                                        case 1:
                                                                            nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Green);
                                                                            break;
                                                                        case 2:
                                                                            nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Green);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                    puesto = true;
                                                                }
                                                                Pos++;
                                                            }
                                                        }

                                                        Pos = 0;

                                                        if (annotation.table.rows[i].exonInt != null)
                                                        {
                                                            while (!puesto && Pos < annotation.table.rows[i].exonInt.Count)
                                                            {
                                                                if (annotation.table.rows[i].exonInt[Pos].Range.From - addStart < dentroPedazo && dentroPedazo <= annotation.table.rows[i].exonInt[Pos].Range.To - addEnd)
                                                                {
                                                                    switch (interno)
                                                                    {
                                                                        case 0:
                                                                            nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Purple);
                                                                            break;
                                                                        case 1:
                                                                            nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Purple);
                                                                            break;
                                                                        case 2:
                                                                            nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Purple);
                                                                            break;
                                                                        default:
                                                                            break;
                                                                    }
                                                                    puesto = true;
                                                                }
                                                                Pos++;
                                                            }
                                                        }

                                                        

                                                        

                                                        if (!puesto)
                                                        {
                                                            switch (interno)
                                                            {
                                                                case 0:
                                                                    nuevoTranscrito.annottationOne.Fill = new SolidColorBrush(Colors.Gray);
                                                                    break;
                                                                case 1:
                                                                    nuevoTranscrito.annotationTwo.Fill = new SolidColorBrush(Colors.Gray);
                                                                    break;
                                                                case 2:
                                                                    nuevoTranscrito.annotationThree.Fill = new SolidColorBrush(Colors.Gray);
                                                                    break;
                                                                default:
                                                                    break;
                                                            }
                                                        }
                                                    }
                                                }

                                                nuevo.StackPanelTranscrips.Children.Add(nuevoTranscrito);
                                            }

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
                catch (Exception exp)
                {

                    throw exp;
                }

                
            }

        }

        private void ShowSequencesButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "GFN files|*.gfn2";
            dialog.Title = "Save GFN file";
            dialog.AddExtension = true;
            dialog.DefaultExt = "gfn2";

            if (dialog.ShowDialog() == true)
            {
                using (Stream fs = new FileStream(dialog.FileName, FileMode.Create, FileAccess.Write, FileShare.None))  
                {
                    //XmlSerializer xmlSerial = new XmlSerializer(typeof(CompleteViewModel));
                    //xmlSerial.Serialize(fs, modelo);
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, modelo);
                }
            }


            //List<SmorfsList> data = GridSmorf.Items.Cast<SmorfsList>().ToList();

            //SaveFileDialog dialog = new SaveFileDialog();
            //dialog.Filter = "Fasta files|*.fasta";
            //dialog.Title = "Save fasta file";
            //dialog.AddExtension = true;
            //dialog.DefaultExt = "fasta";

            //if (dialog.ShowDialog() == true)
            //{
            //    using (var wr = new StreamWriter(dialog.FileName))
            //    {
            //        foreach (var items in data)
            //        {
            //            var item = items.data;
            //            wr.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
            //                        item.MetaData,
            //                        item.Id,
            //                        modelo.ConfigSetup.specie1,
            //                        item.Length.ToString(),
            //                        item.Similarity.ToString(),
            //                        item.MafSource,
            //                        item.Chromosome,
            //                        item.Position.ToString(),
            //                        item.Strand,
            //                        item.NumberRepetition.ToString(),
            //                        item.TotalRepetitions.ToString(),
            //                        item.FlagUniqueSequence.ToString(),
            //                        item.CodingScore.ToString(),
            //                        item.NumCodons.ToString()
            //                                            );
            //            for (int i = 0; i < item.NumCodons; i++)
            //            {
            //                wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
            //            }
            //            wr.Write("{0}\t", item.Coord.Count);
            //            for (int i = 0; i < item.Coord.Count; i++)
            //            {
            //                wr.Write("{0}\t{1}\t{2}\t", item.Coord[i].Chromosome, item.Coord[i].Position, item.Coord[i].Strand);
            //            }
            //            wr.Write("{0}\t{1}\t{2}\t{3}\t",
            //                item.ConservationAverage,
            //                item.ConservationPrevious,
            //                item.ConservationPosterior,
            //                item.FlagConservation
            //                );
            //            if (item.ExpandedConservation != null)
            //            {
            //                wr.Write("{0}\t", item.ExpandedConservation.Length);
            //                for (int i = 0; i < item.ExpandedConservation.Length; i++)
            //                {
            //                    wr.Write("{0}\t", item.ExpandedConservation[i].ToString());
            //                }
            //            }
            //            else
            //            {
            //                wr.Write("0\t");
            //            }

            //            wr.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t",
            //                item.OverlapExon,
            //                item.OverlapIntron,
            //                item.OverlapIntergenic,
            //                item.OverlapUtr5,
            //                item.OverlapUtr3,
            //                item.OverlapExonReverse,
            //                item.Annotation,
            //                item.ExpandedAnotation,
            //                item.ExpandedAnotationReverse,
            //                item.FlagOverlapExonEitherStrand,
            //                item.KozakSequence,
            //                item.KozakScore,
            //                item.GCcontent1
            //                );

            //            wr.WriteLine();
            //            wr.WriteLine(item.Sequence);
            //            //if (item.Coord.Count > 0)
            //            //{
            //            //    if (item.Coord.Count > 0)
            //            //    {
            //            //        wr.Write(">{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
            //            //            item.MetaData,
            //            //            item.Id,
            //            //            modelo.ConfigSetup.specie1,
            //            //            item.Length.ToString(),
            //            //            item.Similarity.ToString(),
            //            //            item.MafSource,
            //            //            item.Chromosome,
            //            //            item.Position.ToString(),
            //            //            item.Strand,
            //            //            item.NumberRepetition.ToString(),
            //            //            item.TotalRepetitions.ToString(),
            //            //            item.FlagUniqueSequence.ToString(),
            //            //            item.CodingScore.ToString(),
            //            //            item.NumCodons.ToString()
            //            //                                );
            //            //        for (int i = 0; i < item.NumCodons; i++)
            //            //        {
            //            //            wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
            //            //        }
            //            //        wr.Write("{0}\t", item.Coord.Count);
            //            //        for (int i = 0; i < item.Coord.Count; i++)
            //            //        {
            //            //            wr.Write("{0}\t{1}\t{2}\t", item.Coord[i].Chromosome, item.Coord[i].Position, item.Coord[i].Strand);
            //            //        }
            //            //        wr.Write("{0}\t{1}\t{2}\t{3}\t",
            //            //            item.ConservationAverage,
            //            //            item.ConservationPrevious,
            //            //            item.ConservationPosterior,
            //            //            item.FlagConservation
            //            //            );
            //            //        if (item.ExpandedConservation != null)
            //            //        {
            //            //            wr.Write("{0}\t", item.ExpandedConservation.Length);
            //            //            for (int i = 0; i < item.ExpandedConservation.Length; i++)
            //            //            {
            //            //                wr.Write("{0}\t", item.ExpandedConservation[i].ToString());
            //            //            }
            //            //        }
            //            //        else
            //            //        {
            //            //            wr.Write("0\t");
            //            //        }

            //            //        wr.Write("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t",
            //            //            item.OverlapExon,
            //            //            item.OverlapIntron,
            //            //            item.OverlapIntergenic,
            //            //            item.OverlapUtr5,
            //            //            item.OverlapUtr3,
            //            //            item.OverlapExonReverse,
            //            //            item.Annotation,
            //            //            item.ExpandedAnotation,
            //            //            item.ExpandedAnotationReverse,
            //            //            item.FlagOverlapExonEitherStrand,
            //            //            item.KozakSequence,
            //            //            item.KozakScore,
            //            //            item.GCcontent1
            //            //            );

            //            //        wr.WriteLine();
            //            //        wr.WriteLine(item.Sequence);
            //            //        //wr.WriteLine(">{0}", modelo.ConfigSetup.specie2);
            //            //        ////wr.WriteLine(">Chimp");
            //            //        //wr.WriteLine(item.SequenceSecondSpecies);
            //            //        //wr.WriteLine(">{0}", modelo.ConfigSetup.specie3);
            //            //        ////wr.WriteLine(">Rhesus");
            //            //        //wr.WriteLine(item.SequenceThirdSpecies);
            //            //    }

            //            //    //wr.Write(">Human|{0}|{1}",//"{0}\t{1}\t{2}\t{3}\t",//{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t",
            //            //    //                        //item.MetaData,
            //            //    //                        //item.Id,
            //            //    //                        //modelo.ConfigSetup.specie1,
            //            //    //                        item.Annotation,
            //            //    //                        item.CodingScore
            //            //    //                        //item.NumCodons.ToString()
            //            //    //    //item.Similarity.ToString(),
            //            //    //    //item.MafSource,
            //            //    //    //item.Chromosome,
            //            //    //    //item.Position.ToString(),
            //            //    //    //item.Strand,
            //            //    //    //item.NumberRepetition.ToString(),
            //            //    //    //item.TotalRepetitions.ToString(),
            //            //    //    //item.FlagUniqueSequence.ToString(),
            //            //    //    //item.CodingScore.ToString(),
            //            //    //    //item.NumCodons.ToString()
            //            //    //                        );
            //            //    ////for (int i = 0; i < item.NumCodons; i++)
            //            //    ////{
            //            //    ////    wr.Write("{0}\t", item.ExpandedCodingScores[i].ToString());
            //            //    ////}
            //            //    //wr.WriteLine();
            //            //    //wr.WriteLine(item.Sequence);
            //            //    ////wr.WriteLine(">{0}", modelo.ConfigSetup.specie2);
            //            //    //wr.WriteLine(">Chimp");
            //            //    //wr.WriteLine(item.SequenceSecondSpecies);
            //            //    ////wr.WriteLine(">{0}", modelo.ConfigSetup.specie3);
            //            //    //wr.WriteLine(">Rhesus");
            //            //    //wr.WriteLine(item.SequenceThirdSpecies);
            //            //}
            //        }
            //    }
            //}
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
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3N: {0} - {1:0.00}%", conservedFirst, conservedFirst * 100 / (double)universo), Value = conservedFirst });
                    int conservedSecond = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3N: {0} - {1:0.00}%", conservedSecond, conservedSecond * 100 / (double)universo), Value = conservedSecond });
                    int conservedThird = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3N: {0} - {1:0.00}%", conservedThird, conservedThird * 100 / (double)universo), Value = conservedThird });
                    int conservedFourth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SN-3C: {0} - {1:0.00}%", conservedFourth, conservedFourth * 100 / (double)universo), Value = conservedFourth });
                    int conservedFifth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior < modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3N: {0} - {1:0.00}%", conservedFifth, conservedFifth * 100 / (double)universo), Value = conservedFifth });
                    int conservedSixth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage < modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SN-3C: {0} - {1:0.00}%", conservedSixth, conservedSixth * 100 / (double)universo), Value = conservedSixth });
                    int conservedSepth = data.Count(p => p.ConservationPrevious < modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5N-SC-3C: {0} - {1:0.00}%", conservedSepth, conservedSepth * 100 / (double)universo), Value = conservedSepth });
                    int conservedOcth = data.Count(p => p.ConservationPrevious >= modelo.ConfigSetup.conservationThreshold && p.ConservationAverage >= modelo.ConfigSetup.conservationThreshold && p.ConservationPosterior >= modelo.ConfigSetup.conservationThreshold);
                    conservationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5C-SC-3C: {0} - {1:0.00}%", conservedOcth, conservedOcth * 100 / (double)universo), Value = conservedOcth });


                    annotationSeries.DataPoints.Clear();
                    
                    double total = 0;
                    int exon = data.Sum(p => p.condensedResultTable[4]);
                    total += exon;
                    int intron = data.Sum(p => p.condensedResultTable[5]);
                    total += intron;
                    int cinco = data.Sum(p => p.condensedResultTable[2]);
                    total += cinco;
                    int tres = data.Sum(p => p.condensedResultTable[3]);
                    total += tres;
                    int ambiguous = data.Sum(p => p.condensedResultTable[7]);
                    total += ambiguous;

                    //int intergenic = data.Count(p => p.condensedResultTable[1] - p.condensedResultTable[6] == 0);

                    //int exon = data.Count(p => p.OverlapExon == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Exon: {0} - {1:0.00}%", exon, exon * 100 / total), Value = exon });

                    //int intron = data.Count(p => p.OverlapIntron == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intron: {0} - {1:0.00}%", intron, intron * 100 / total), Value = intron });

                    //int intergenic = data.Count(p => p.OverlapIntergenic == 1);
                    //annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Intergenic: {0} - {1}%", intergenic, intergenic * 100 / universo), Value = intergenic });

                    //int cinco = data.Count(p => p.OverlapUtr5 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("5' utr: {0} - {1:0.00}%", cinco, cinco * 100 / total), Value = cinco });

                    //int tres = data.Count(p => p.OverlapUtr3 == 1);
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("3' utr: {0} - {1:0.00}%", tres, tres * 100 / total), Value = tres });

                    //int ambiguous = universo - exon - intron - intergenic - cinco - tres;
                    annotationSeries.DataPoints.Add(new Telerik.Charting.PieDataPoint() { Label = string.Format("Ambiguous: {0} - {1:0.00}%", ambiguous, ambiguous * 100 / total), Value = ambiguous });


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

        private void GridSmorf_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            
        }

        private void CopiarScoreButton_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(CopiaScore);
        }

        public static void CopyUIElementToClipboard(FrameworkElement element)
        {
            double width = element.ActualWidth;
            double height = element.ActualHeight;
            RenderTargetBitmap bmpCopied = new RenderTargetBitmap((int)Math.Round(width), (int)Math.Round(height), 96, 96, PixelFormats.Default);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                VisualBrush vb = new VisualBrush(element);
                dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
            }
            bmpCopied.Render(dv);
            Clipboard.SetImage(bmpCopied);
        }

        private void startOverButton_Click(object sender, RoutedEventArgs e)
        {
            ventanaPrincipal.startOver();
        }

        private void CopiarLengthButtons_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(copiarLength);
        }

        private void CopiarConservationButtons_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(CopiaConservation);
        }

        private void CopiarAnnotationButton_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(CopiarAnnotation);
        }

        
        private void CopiarClassificationButton_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(CopiarClassification);
        }

        private void CopiarKozkButton_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(CopiarKozak);
        }

        private void CopiarDistributionButton_Click(object sender, RoutedEventArgs e)
        {
            CopyUIElementToClipboard(heatMapsPanel);
        }

        private void BackGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ventanaPrincipal.startOver();
        }

        private void addConditionButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openDialog = new OpenFileDialog();
            openDialog.Filter = "sam file|*.sam";
            openDialog.Multiselect = true;
            if (openDialog.ShowDialog() == true)
            {
                RNAConditionWindow ventanaRNA = new RNAConditionWindow();
                if (ventanaRNA.ShowDialog() == true)
                {
                    string conditionName = ventanaRNA.TextBoxConditionName.Text;
                    int mapQuality = (int)ventanaRNA.mapQualityNumUpDown.Value;
                    busyIndicator.IsBusy = true;
                    var worker = new BackgroundWorker();
                    worker.DoWork += (s, ev) =>
                    {
                        RNA.MatchRNAtoSmorfs(openDialog.FileNames, conditionName, mapQuality, modelo.smorfsList);
                    };

                    worker.RunWorkerCompleted += (s, ev) =>
                    {
                        busyIndicator.IsBusy = false;
                        UpdatePlots();
                    };

                    worker.RunWorkerAsync();
                }


                
            }
        }

        private void logoButton_Click(object sender, RoutedEventArgs e)
        {
            LogoWindow ventana = new LogoWindow();
            ventana.data = data;
            ventana.Show();
        }

        private void selectionScoreButton_Click(object sender, RoutedEventArgs e)
        {
            SelectScoreWindow ventana = new SelectScoreWindow();
            ventana.data = this.data;
            ventana.Show();
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
                    string newId = ventana.NewLabelTextBox.Text;
                    if ((bool)ventana.ChromosomeCheckBox.IsChecked) newId += "_" + s.Chromosome;
                    newId += "_" + num;
                    s.Id = newId;
                    num++;
                }
                GridSmorf.Rebind();
            }
        }
    }
}
