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
using LcuRelogger.core;

namespace LcuRelogger
{
    /// <summary>
    /// Interaction logic for PasswortPrompt.xaml
    /// </summary>
    public partial class PasswortPrompt : Window
    {
        private bool _editing;
        private Entry _entry;
        private Manager _manager;
        public PasswortPrompt(Manager manager, bool editing = false, Entry entry = null)
        {
            _manager = manager;
            _editing = editing;
            _entry = entry;
            InitializeComponent();
            if (editing)
            {
                userNameField.Text = entry.Username;
                passwordField.Password = entry.Password;
                addBtn.Content = "Save";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_editing)
            
                _manager.Entries.Remove(_entry);
            
            _manager.AddEntry(userNameField.Text, passwordField.Password);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
