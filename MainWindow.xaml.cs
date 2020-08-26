using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;



namespace AttendanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
    // The process of creating counters should probably be generalized so that they can be recreated 
    // easier, and so I don't have to rewrite so much code. This xaml file should deal with the main
    // interactions with the page, but it should not include the logic for all counters. This file is going
    // to get very long and messy real quick. Its possible that c# and WPF forms were not designed for reusable
    // componenets at all
    // Look into Control Templates
    public partial class MainWindow : Window
    {

        StackPanel currentPanel;
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            currentPanel = new StackPanel();
            currentPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            CounterStackPanel.Children.Add(currentPanel);
        }
        private void generateNewCounter_Click(object sender, RoutedEventArgs e)
        {
            Counter customCounter = new Counter();
            if (currentPanel.Children.Count == 2)
            {
                currentPanel = new StackPanel();
                CounterStackPanel.Children.Add(currentPanel);
                currentPanel.Orientation = System.Windows.Controls.Orientation.Horizontal;
            }
            currentPanel.Children.Add(customCounter);
        }

        
    }
}
