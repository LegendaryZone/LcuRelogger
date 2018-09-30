using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using LcuRelogger.core;

namespace LcuRelogger
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Manager manager;


        public MainWindow()
        {
            InitializeComponent();
            manager = new Manager();
            listGrid.ItemsSource = manager.Entries;

            if (!manager.LolExists)
            {
                promptLol();
            }
            else
            {
                manager.LoadApi();
                Visibility = Visibility.Visible;
            }
        }


        private void promptLol()
        {
            var win = new LeagueFinder();
            win.Closed += (o, args) =>
            {
                manager.updateLeaguePath(win.value);
                if (manager.LolExists)
                {
                    manager.LoadApi();
                    Visibility = Visibility.Visible;
                }
                else
                {
                    promptLol();
                }
            };
            win.Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new PasswortPrompt(manager);
            win.Closed += (o, args) => { CollectionViewSource.GetDefaultView(listGrid.ItemsSource).Refresh(); };
            win.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var item = listGrid.SelectedItem;
            if (item == null) return;
            var win = new PasswortPrompt(manager, true, (Entry) item);
            win.Closed += (o, args) => { CollectionViewSource.GetDefaultView(listGrid.ItemsSource).Refresh(); };
            win.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var item = (Entry) listGrid.SelectedItem;
            if (item == null) return;


            if (!manager.Connected) manager.LoadApi();
            if (!manager.Connected) return;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                manager.Api.reloadApi();
                manager.Api.restartClient(() => { manager.Api.startSession(item); });
            }).Start();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            foreach (var listGridSelectedItem in listGrid.SelectedItems)
                manager.Entries.Remove((Entry) listGridSelectedItem);
            CollectionViewSource.GetDefaultView(listGrid.ItemsSource).Refresh();

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                manager.writeConfig();
            }).Start();
        }

        private void listGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = (Entry)listGrid.SelectedItem;
            if (item == null) return;

            if(!manager.Connected) manager.LoadApi();
            if (!manager.Connected) return;
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                manager.Api.reloadApi();
                manager.Api.restartClient(() => { manager.Api.startSession(item); });
            }).Start();
        }
    }
}