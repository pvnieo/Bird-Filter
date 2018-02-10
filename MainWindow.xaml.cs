using System.Windows;
using Twitter_Filter.View;

namespace Twitter_Filter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TF_View view = new TF_View(this);
        }

        public void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
