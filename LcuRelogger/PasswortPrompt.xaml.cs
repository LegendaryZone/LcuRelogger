using System.Collections.Generic;
using System.Windows;
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
                regionBox.SelectedValue = entry.Region;
                addBtn.Content = "Save";
            }

            regionBox.ItemsSource = getRegions();
            

        }

        private List<string> getRegions()
        {
            var list = new List<string>();
            list.Add("BR");
            list.Add("EUNE");
            list.Add("EUW");
            list.Add("JP");
            list.Add("KR");
            list.Add("LAN");
            list.Add("LAS");
            list.Add("NA");
            list.Add("OCE");
            list.Add("TR");
            list.Add("RU");

            return list;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (_editing)
            
               _manager.Entries.Remove(_entry);
            
            _manager.AddEntry(userNameField.Text, passwordField.Password, regionBox.Text);
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
