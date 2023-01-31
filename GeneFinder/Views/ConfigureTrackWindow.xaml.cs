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
    /// Interaction logic for ConfigureTrackWindow.xaml
    /// </summary>
    public partial class ConfigureTrackWindow : Window
    {
        public ConfigureTrackWindow()
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


        private void radioValue_Checked(object sender, RoutedEventArgs e)
        {
            threshold1NumericUpDown.IsEnabled = false;
            threshold2NumericUpDown.IsEnabled = false;
            threshold3NumericUpDown.IsEnabled = false;
            threshold4NumericUpDown.IsEnabled = false;
        }

        private void radioValue_Unchecked(object sender, RoutedEventArgs e)
        {
            threshold1NumericUpDown.IsEnabled = true;
            threshold2NumericUpDown.IsEnabled = true;
            threshold3NumericUpDown.IsEnabled = true;
            threshold4NumericUpDown.IsEnabled = true;
        }
    }
}
