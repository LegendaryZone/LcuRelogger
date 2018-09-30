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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace LcuRelogger
{
    /// <summary>
    /// Interaction logic for LeagueFinder.xaml
    /// </summary>
    public partial class LeagueFinder : Window
    {
        public string value;
        public LeagueFinder()
        {
            value = "";
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog()
            {
                ShowNewFolderButton = false
            };
            dialog.Description = "Select League Folder";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                valueBox.Text = dialog.SelectedPath;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var val = valueBox.Text;
            if (Directory.Exists(val))
            {
                value = val;
                Close();
            }

        }
    }
}
