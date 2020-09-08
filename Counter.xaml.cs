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
using System.Text.RegularExpressions;

namespace AttendanceTracker
{
    /// <summary>
    /// Interaction logic for Counter.xaml
    /// </summary>
    /// 

    // will need to have a a parameter that checks what the sumValue is
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

        // need to fix naming here
        private void incrementButton_Click(object sender, RoutedEventArgs e)
        {
            int sumType = 1;
            if (customCount.Text.Length > 0)
            {
                int customCountAsInt = Int16.Parse(customCount.Text);
                sumType = customCountAsInt;
                totalVisitors = totalVisitors + customCountAsInt;
                customCount.Text = "";
            } else
            {
                totalVisitors++;
            }
            
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
            insertIntoDatabase(query, 1);
        }

        

        private void decrementButton_Click(object sender, RoutedEventArgs e)
        {
            int sumType = 0;
            if (customCount.Text.Length > 0)
            {
                int customCountAsInt = Int16.Parse(customCount.Text);
                if (totalVisitors - customCountAsInt < 0)
                {
                    customCountAsInt = totalVisitors;
                    totalVisitors = 0;
                } else
                {
                    totalVisitors = totalVisitors - customCountAsInt;
                }
                sumType = customCountAsInt * -1;
                customCount.Text = "";
            } else if (totalVisitors > 0)
            {
                sumType = -1;
                totalVisitors = totalVisitors + sumType;
            }
            
            counterLabel.Content = totalVisitors;
            generalTimestampLog.Text += totalVisitors + " - " + DateTime.Now + "\n";          
            
            string query = "INSERT INTO attendance ('timestamp', 'category', 'sum_type') VALUES (@timestamp, @category, @sum_type)";
            insertIntoDatabase(query, sumType);  
        }

        private void insertIntoDatabase(string query, int sumType)
        {
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            myCommand.Parameters.AddWithValue("@timestamp", DateTime.Now);
            myCommand.Parameters.AddWithValue("@category", title.Text);
            myCommand.Parameters.AddWithValue("@sum_type", sumType);
            var result = myCommand.ExecuteNonQuery();
            databaseObject.CloseConnection();
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

        private void CustomCountWatermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            customCountWatermarkedTxt.Visibility = System.Windows.Visibility.Collapsed;
            customCount.Visibility = System.Windows.Visibility.Visible;
            customCount.Focus();
        }

        private void CustomCount_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(logNotes.Text))
            {
                customCount.Visibility = System.Windows.Visibility.Collapsed;
                customCountWatermarkedTxt.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Reset_General(object sender, RoutedEventArgs e)
        {
            totalVisitors = 0;
            generalTimestampLog.Text = "";
            counterLabel.Content = totalVisitors;
        }

        // ensures that the user is only able to input numeric values into the customCount TextBox
        // TODO include stack overflow reference
        private void customCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
