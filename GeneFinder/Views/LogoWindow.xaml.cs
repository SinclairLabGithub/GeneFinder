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
    /// Interaction logic for LogoWindow.xaml
    /// </summary>
    public partial class LogoWindow : Window
    {
        internal List<smorf> data;

        public LogoWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            int a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[0] == 'A');
            int t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[0] == 'T');
            int g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[0] == 'G');
            int c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[0] == 'C');

            Columna00Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna00Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna00Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna00Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[1] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[1] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[1] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[1] == 'C');
            Columna01Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna01Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna01Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna01Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[2] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[2] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[2] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[2] == 'C');
            Columna02Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna02Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna02Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna02Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[3] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[3] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[3] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[3] == 'C');
            Columna03Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna03Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna03Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna03Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[4] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[4] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[4] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[4] == 'C');
            Columna04Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna04Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna04Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna04Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[5] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[5] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[5] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[5] == 'C');
            Columna05Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna05Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna05Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna05Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[6] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[6] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[6] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[6] == 'C');
            Columna06Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna06Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna06Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna06Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[7] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[7] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[7] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[7] == 'C');
            Columna07Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna07Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna07Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna07Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[8] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[8] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[8] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[8] == 'C');
            Columna08Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna08Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna08Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna08Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[9] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[9] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[9] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[9] == 'C');
            Columna09Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna09Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna09Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna09Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[10] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[10] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[10] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[10] == 'C');
            Columna10Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna10Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna10Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna10Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[11] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[11] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[11] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[11] == 'C');
            Columna11Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna11Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna11Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna11Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[12] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[12] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[12] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[12] == 'C');
            Columna12Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna12Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna12Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna12Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[13] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[13] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[13] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[13] == 'C');
            Columna13Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna13Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna13Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna13Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);

            a = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[14] == 'A');
            t = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[14] == 'T');
            g = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[14] == 'G');
            c = data.Count(q => !String.IsNullOrEmpty(q.KozakSequence) && q.KozakSequence[14] == 'C');
            Columna14Grid.RowDefinitions[0].Height = new GridLength((double)a, GridUnitType.Star);
            Columna14Grid.RowDefinitions[1].Height = new GridLength((double)t, GridUnitType.Star);
            Columna14Grid.RowDefinitions[2].Height = new GridLength((double)c, GridUnitType.Star);
            Columna14Grid.RowDefinitions[3].Height = new GridLength((double)g, GridUnitType.Star);


        }
    }
}
