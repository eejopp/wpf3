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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for History.xaml
    /// </summary>
    public partial class History : Window
    {
        public string Selected { get; private set; } = null;
        public History(List<string> list)
        {
            InitializeComponent();
            lbHistory.ItemsSource = list;
        }

        private void lbHistory_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (lbHistory.SelectedItem!=null)
            {
                Selected=(string)lbHistory.SelectedItem;
                DialogResult = true;
                Close();
            }
        }

        private void lbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbHistory.SelectedItem != null)
            {
                Selected = (string)lbHistory.SelectedItem;
                DialogResult = true;
                Close();
            }
        }
    }
}
