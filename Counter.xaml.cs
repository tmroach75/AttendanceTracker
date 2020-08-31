//using SQLite;
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
using System.Data.SQLite;
using System.Data;

namespace AttendanceTracker
{
    /// <summary>
    /// Interaction logic for Counter.xaml
    /// </summary>
    /// 

    // will need to add database logic in here rather than in MainWindow since we add
    // componenets at runtime
    public partial class Counter : UserControl
    {
        

        private int totalVisitors;
        Database databaseObject;
        public Counter()
        {
            DataContext = this;
            InitializeComponent();
            databaseObject = new Database();
        }

        public static DependencyProperty CounterTitleProperty = DependencyProperty.Register("CounterTitle", typeof(string), typeof(Counter));

        public string CounterTitle
        {
            get { return (string)GetValue(CounterTitleProperty); }
            set
            {
                SetValue(CounterTitleProperty, value);
                title.Text = value;
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

            // insert into database
            string query = "INSERT INTO attendance ('timestamp', 'category', 'sum_type') VALUES (@timestamp, @category, @sum_type)";
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            myCommand.Parameters.AddWithValue("@timestamp", DateTime.Now);
            myCommand.Parameters.AddWithValue("@category", title.Text);
            myCommand.Parameters.AddWithValue("@sum_type", 1);
            var result = myCommand.ExecuteNonQuery();
            databaseObject.CloseConnection();
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
