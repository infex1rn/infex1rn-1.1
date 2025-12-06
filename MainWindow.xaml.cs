using System.Windows;
using System.Linq;
using infex1rn.ViewModels;

namespace infex1rn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void IpswTreeView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                e.Effects = (files != null && files.Length > 0 && files[0].EndsWith(".ipsw", System.StringComparison.OrdinalIgnoreCase))
                    ? DragDropEffects.Copy
                    : DragDropEffects.None;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void IpswTreeView_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    var ipswFile = files.FirstOrDefault(f => f.EndsWith(".ipsw", System.StringComparison.OrdinalIgnoreCase));
                    if (!string.IsNullOrEmpty(ipswFile))
                    {
                        var viewModel = DataContext as MainViewModel;
                        if (viewModel != null)
                        {
                            viewModel.LoadIpswFile(ipswFile);
                        }
                    }
                }
            }
            e.Handled = true;
        }
    }
}
