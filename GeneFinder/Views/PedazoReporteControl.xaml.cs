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

namespace GeneFinder.Views
{
    /// <summary>
    /// Interaction logic for PedazoReporteControl.xaml
    /// </summary>
    public partial class PedazoReporteControl : UserControl
    {
        //public bool Inicio { get; set; }
        //public bool Final { get; set; }
        //public string Exon { get; set; }
        //public string Intron { get; set; }
        public bool InicioProteina { get; set; }
        public bool FinalProteina { get; set; }
        public string Proteina { get; set; }
        public string Cadena { get; set; }
        public string CadenaDos { get; set; }
        public string CadenaTres { get; set; }
        public double Coding { get; set; }
        public string Conservation { get; set; }

        public PedazoReporteControl()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {


            //if (Inicio)
            //{
            //    InicioExon.Visibility = Visibility.Visible;
            //    InicioIntron.Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    if (Final)
            //    {
            //        FinalExon.Visibility = Visibility.Visible;
            //        FinalIntron.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        ExonIntermedio.Visibility = Visibility.Visible;
            //        IntronIntermedio.Visibility = Visibility.Visible;
            //        if (Exon[0] == 'e')
            //        {
            //            exonObscuroUno.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[0] == 'i')
            //        {
            //            exonClaroUno.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[0] == '5')
            //        {
            //            exonObscuroUno.Visibility = Visibility.Visible;
            //            exonObscuroUno.Fill= new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Exon[0] == '3')
            //        {
            //            exonObscuroUno.Visibility = Visibility.Visible;
            //            exonObscuroUno.Fill = new SolidColorBrush(Colors.Gray);
            //        }
            //        if (Exon[1] == 'e')
            //        {
            //            exonObscuroDos.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[1] == 'i')
            //        {
            //            exonClaroDos.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[1] == '5')
            //        {
            //            exonObscuroDos.Visibility = Visibility.Visible;
            //            exonObscuroDos.Fill = new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Exon[1] == '3')
            //        {
            //            exonObscuroDos.Visibility = Visibility.Visible;
            //            exonObscuroDos.Fill = new SolidColorBrush(Colors.Gray);
            //        }
            //        if (Exon[2] == 'e')
            //        {
            //            exonObscuroTres.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[2] == 'i')
            //        {
            //            exonClaroTres.Visibility = Visibility.Visible;
            //        }
            //        else if (Exon[0] == '5')
            //        {
            //            exonObscuroTres.Visibility = Visibility.Visible;
            //            exonObscuroTres.Fill = new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Exon[0] == '3')
            //        {
            //            exonObscuroTres.Visibility = Visibility.Visible;
            //            exonObscuroTres.Fill = new SolidColorBrush(Colors.Gray);
            //        }

            //        if (Intron[0] == 'e')
            //        {
            //            intronObscuroUno.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[0] == 'i')
            //        {
            //            intronClaroUno.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[0] == '5')
            //        {
            //            intronObscuroUno.Visibility = Visibility.Visible;
            //            intronObscuroUno.Fill = new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Intron[0] == '3')
            //        {
            //            intronObscuroUno.Visibility = Visibility.Visible;
            //            intronObscuroUno.Fill = new SolidColorBrush(Colors.Gray);
            //        }

            //        if (Intron[1] == 'e')
            //        {
            //            intronObscuroDos.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[1] == 'i')
            //        {
            //            intronClaroDos.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[1] == '5')
            //        {
            //            intronObscuroDos.Visibility = Visibility.Visible;
            //            intronObscuroDos.Fill = new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Intron[1] == '3')
            //        {
            //            intronObscuroDos.Visibility = Visibility.Visible;
            //            intronObscuroDos.Fill = new SolidColorBrush(Colors.Gray);
            //        }

            //        if (Intron[2] == 'e')
            //        {
            //            intronObscuroTres.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[2] == 'i')
            //        {
            //            intronClaroTres.Visibility = Visibility.Visible;
            //        }
            //        else if (Intron[2] == '5')
            //        {
            //            intronObscuroTres.Visibility = Visibility.Visible;
            //            intronObscuroTres.Fill = new SolidColorBrush(Colors.Black);
            //        }
            //        else if (Intron[2] == '3')
            //        {
            //            intronObscuroTres.Visibility = Visibility.Visible;
            //            intronObscuroTres.Fill = new SolidColorBrush(Colors.Gray);
            //        }


            //    }
            //}
            labelProteina.Content = Proteina;

            if (InicioProteina) fondoProteina.Fill = new SolidColorBrush(Color.FromRgb(45, 169, 71));
            if (FinalProteina) fondoProteina.Fill = new SolidColorBrush(Color.FromRgb(237, 64, 30));
            if (Proteina == "") fondoProteina.Fill = null;
            labelCadena1.Content = Cadena[0];
            labelCadena2.Content = Cadena[1];
            labelCadena3.Content = Cadena[2];

            if (InicioProteina)
            {
                labelCadena1.Foreground = new SolidColorBrush(Color.FromRgb(45, 169, 71));
                labelCadena2.Foreground = new SolidColorBrush(Color.FromRgb(45, 169, 71));
                labelCadena3.Foreground = new SolidColorBrush(Color.FromRgb(45, 169, 71));
            }
            if (FinalProteina)
            {
                labelCadena1.Foreground = new SolidColorBrush(Color.FromRgb(237, 64, 30));
                labelCadena2.Foreground = new SolidColorBrush(Color.FromRgb(237, 64, 30));
                labelCadena3.Foreground = new SolidColorBrush(Color.FromRgb(237, 64, 30));
            }
            if (Coding > 0.05) Coding = .050;
            if (Coding < -0.05) Coding = -0.050;
            if (Coding >= 0)
            {
                lineaUnoArriba.Visibility = Visibility.Collapsed;
                double valor = Coding/0.05;
                GridConservation.RowDefinitions[0].Height = new GridLength(1 - valor, GridUnitType.Star);
                GridConservation.RowDefinitions[1].Height = new GridLength(valor, GridUnitType.Star);
                GridConservation.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                lineaUnoAbajo.Visibility = Visibility.Collapsed;
                double valor = -Coding/0.05;
                GridConservation.RowDefinitions[0].Height = new GridLength(1, GridUnitType.Star);
                GridConservation.RowDefinitions[1].Height = new GridLength(valor, GridUnitType.Star);
                GridConservation.RowDefinitions[2].Height = new GridLength(1 - valor, GridUnitType.Star);
            }
            if (Conservation[0] == '0')
            {
                conservationCuatro.Visibility = Visibility.Visible;
            }
            else if (Conservation[0] == '1')
            {
                conservationUno.Visibility = Visibility.Visible;
            }
            if (Conservation[1] == '0')
            {
                conservationCinco.Visibility = Visibility.Visible;
            }
            else if (Conservation[2] == '1')
            {
                conservationDos.Visibility = Visibility.Visible;
            }
            if (Conservation[2] == '0')
            {
                conservationSeis.Visibility = Visibility.Visible;
            }
            else if (Conservation[2] == '1')
            {
                conservationTres.Visibility = Visibility.Visible;
            }
            labelCadenaDos1.Content = CadenaDos[0];
            labelCadenaDos2.Content = CadenaDos[1];
            labelCadenaDos3.Content = CadenaDos[2];
            labelCadenaTres1.Content = CadenaTres[0];
            labelCadenaTres2.Content = CadenaTres[1];
            labelCadenaTres3.Content = CadenaTres[2];

        }


    }
}
