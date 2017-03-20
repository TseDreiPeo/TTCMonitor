using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PegasusGsMonitor {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private UriVm UriVm = null;
        private ScienceDataVm ScienceVm = null;

        public MainWindow() {
            InitializeComponent();
            UriVm = new UriVm();
            this.DataContext = UriVm;

            ScienceVm = new ScienceDataVm();
            this.ScienceDataTab.DataContext = ScienceVm;

        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.wb.CanGoBack)
            {
                this.wb.GoBack();
            }
        }

        private void Forward_Click(object sender, RoutedEventArgs e)
        {
            if (this.wb.CanGoForward)
            {
                this.wb.GoForward();
            }
        }

        private void Nav_Click(object sender, RoutedEventArgs e)
        {
            this.wb.Navigate(UriVm.Uri);
        }

        private void ScienceData_Click(object sender, RoutedEventArgs e)
        {
            ScienceVm.ParseForScienceData(UriVm.Uri);
            
        }

        private void ClrData_Click(object sender, RoutedEventArgs e)
        {
            ScienceVm.ClearResults();
        }
    }
}
