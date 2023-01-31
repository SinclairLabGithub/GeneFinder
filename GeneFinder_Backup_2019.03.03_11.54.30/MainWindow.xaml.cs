using GeneFinder.Models;
using GeneFinder.Viewmodels;
using GeneFinder.Views;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
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
using Telerik.Windows.Documents.FormatProviders.Xaml;

namespace GeneFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<GroupReads> paragraphs = new List<GroupReads>();
        List<GroupReads> paragraphsSelected = new List<GroupReads>();
        List<string> species = new List<string>();
        string srcName = "";

        
        internal CompleteViewModel model = new CompleteViewModel();
        internal ExtractorPage2 Extractor;
        internal ClassifierPage Clasificador;
        internal RealignPage Realigner;
        internal AnnotationPage Annotator;
        internal ReportPage Report;
        internal RNAPage RNAData;
        internal StatisticsPage Statistics;
        internal WelcomePage Bienvenida;
        internal Statistics1Page EstadisticasPrimero;
        internal FinalPage final;
        internal DefaultSettingsPage Settings;


        public MainWindow()
        {
            InitializeComponent();
            Style = (Style)FindResource(typeof(Window));
            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Extractor = new ExtractorPage();

            //Extractor.paragraphs = model.Paragraphs;
            //Extractor.paragraphsSelected = model.ParagraphsSelected;
            //Extractor.species = model.Species;
            //Extractor.ventanaPrincipal = this;

            //ContentFrame.Navigate(Extractor);

            Bienvenida = new WelcomePage();
            Bienvenida.paragraphs = model.Paragraphs;
            Bienvenida.paragraphsSelected = model.ParagraphsSelected;
            Bienvenida.species = model.Species;
            Bienvenida.ventanaPrincipal = this;

            ContentFrame.Navigate(Bienvenida);

        }

        

        internal void goToClassifier()
        {
            if (Clasificador == null)
            {
                Clasificador = new ClassifierPage();
                Clasificador.modelo = this.model;
                Clasificador.ventanaPrincipal = this;
            }
            ContentFrame.Navigate(Clasificador);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = false;
        }

        internal void goToSettings()
        {
            Settings = new DefaultSettingsPage();
            Settings.modelo = this.model;
            Settings.ventanaPrincipal = this;
            ContentFrame.Navigate(Settings);
        }

        private void linkClassifier_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.goToClassifier();
        }

        private void linkRealign_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.goToRealigner();
        }

        internal void goToRealigner()
        {

            if (Realigner == null)
            {
                Realigner = new RealignPage();
                Realigner.modelo = this.model;
                Realigner.ventanaPrincipal = this;
            }
            ContentFrame.Navigate(Realigner);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = false;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = true;
        }

        private void linkExtractor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            goToExtractor();
        }

        internal void goToExtractor()
        {
            Extractor = new ExtractorPage2();
            Extractor.paragraphs = model.Paragraphs;
            Extractor.paragraphsSelected = model.ParagraphsSelected;
            Extractor.species = model.Species;
            Extractor.ventanaPrincipal = this;

            ContentFrame.Navigate(Extractor);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = false;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = true;
        }

        private void linkConservation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Annotator == null)
            {
                Annotator = new AnnotationPage();
                Annotator.modelo = this.model;
                Annotator.ventanaPrincipal = this;
            }
            ContentFrame.Navigate(Annotator);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = false;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = true;
        }

        private void linkReport_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Report == null)
            {
                Report = new ReportPage();
                Report.modelo = this.model;
                Report.ventanaPrincipal = this;
            }
            ContentFrame.Navigate(Report);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = false;
            this.linkClassifier.IsEnabled = true;
        }

        private void linkRNA_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RNAData == null)
            {
                RNAData = new RNAPage();
                RNAData.modelo = this.model;
                RNAData.ventanaPrincipal = this;
            }
            ContentFrame.Navigate(RNAData);
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = false;
            this.linkStatistics.IsEnabled = true;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = true;
        }

        private void linkStatistics_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            goToStatistics();
            this.linkExtractor.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRealign.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkConservation.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkRNA.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkStatistics.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            this.linkReport.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));
            this.linkClassifier.Foreground = new SolidColorBrush(Color.FromRgb(83, 118, 158));

            this.linkExtractor.IsEnabled = true;
            this.linkRealign.IsEnabled = true;
            this.linkConservation.IsEnabled = true;
            this.linkRNA.IsEnabled = true;
            this.linkStatistics.IsEnabled = false;
            this.linkReport.IsEnabled = true;
            this.linkClassifier.IsEnabled = true;
        }

        internal void goToStatistics()
        {
            final = new FinalPage();
            final.modelo = this.model;
            final.ventanaPrincipal = this;
            ContentFrame.Navigate(final);
        }

        internal void goToEstadisticasPrimera()
        {
            EstadisticasPrimero = new Statistics1Page();
            EstadisticasPrimero.modelo = this.model;
            EstadisticasPrimero.ventanaPrincipal = this;
            ContentFrame.Navigate(EstadisticasPrimero);
        }

        internal void startOver()
        {
            model = new CompleteViewModel();
            Extractor = new ExtractorPage2();
            Clasificador = new ClassifierPage();
            Realigner = new RealignPage();
            Annotator = new AnnotationPage();
            Report = new ReportPage();
            RNAData = new RNAPage();
            Statistics = new StatisticsPage();
            Bienvenida = new WelcomePage();
            EstadisticasPrimero = new Statistics1Page();
            final = new FinalPage();

            species = new List<string>();

            Bienvenida = new WelcomePage();
            Bienvenida.paragraphs = model.Paragraphs;
            Bienvenida.paragraphsSelected = model.ParagraphsSelected;
            Bienvenida.species = model.Species;
            Bienvenida.ventanaPrincipal = this;

            ContentFrame.Navigate(Bienvenida);

        }
    }
}
