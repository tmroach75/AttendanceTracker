using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AttendanceTracker
{
    /// <summary>
    /// Interaction logic for Counter.xaml
    /// </summary>
    public partial class Counter : UserControl
    {
        

        private int totalVisitors;
        public Counter()
        {
            DataContext = this;
            InitializeComponent();
        }

        public static DependencyProperty CounterTitleProperty = DependencyProperty.Register("CounterTitle", typeof(string), typeof(Counter));

        public string CounterTitle
        {
            get { return (string)GetValue(CounterTitleProperty); }
            set
            {
                SetValue(CounterTitleProperty, value);
                title.Content = value;
            }
        }


        public int TotalVisitors
        {
            get { return totalVisitors; }
            set
            {
                if (totalVisitors != value)
                {
                    totalVisitors = value;
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            totalVisitors++;
            counterLabel.Content = totalVisitors;
            String logNote = logNotes.Text;
            if (logNote.Length > 0)
            {
                logNote = "(" + logNote + ")";
            }
            logNotes.Text = null;
            // reset watermark after counter is pressed
            generalTimestampLog.Text += totalVisitors + " - " + DateTime.Now + logNote + "\n";
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (totalVisitors > 0)
            {
                totalVisitors--;
            }
            counterLabel.Content = totalVisitors;
            generalTimestampLog.Text += totalVisitors + " - " + DateTime.Now + "\n";
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void watermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkedTxt.Visibility = System.Windows.Visibility.Collapsed;
            logNotes.Visibility = System.Windows.Visibility.Visible;
            logNotes.Focus();
        }

        private void logNotes_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(logNotes.Text))
            {
                logNotes.Visibility = System.Windows.Visibility.Collapsed;
                watermarkedTxt.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Reset_General(object sender, RoutedEventArgs e)
        {
            totalVisitors = 0;
            generalTimestampLog.Text = "";
            counterLabel.Content = totalVisitors;
        }
    }
}
