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

        /*
         * Returns the total number of visitors displayed by the counter
        */
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

        /*
         * Increments the total number of visitors displayed by the counter 
        */
        private void IncrementButton_Click(object sender, RoutedEventArgs e)
        {
            int sumType = DetermineIncrementSumType();
            counterLabel.Content = totalVisitors;
            string query = "INSERT INTO attendance ('timestamp', 'category', 'sum_type', 'log_notes') VALUES (@timestamp, @category, @sum_type, @log_notes)";
            InsertIntoDatabase(query, sumType);
            UpdateTimestampLog();
        }

        /*
         * Updates the timestamp texbox log whenever an attendance record is made
        */
        private void UpdateTimestampLog()
        {
            String logNote = logNotes.Text;
            if (logNote.Length > 0)
            {
                logNote = "(" + logNote + ")";
            }
            logNotes.Text = null;
            generalTimestampLog.Text += totalVisitors + " - " + DateTime.Now + logNote + "\n";
        }

        /*
         * Returns the necessary sum type for an increment record
        */
        private int DetermineIncrementSumType()
        {
            int sumType = 1;
            if (customCount.Text.Length > 0)
            {
                int customCountAsInt = Int16.Parse(customCount.Text);
                sumType = customCountAsInt;
                totalVisitors = totalVisitors + customCountAsInt;
                customCount.Text = "";
            }
            else
            {
                totalVisitors++;
            }
            return sumType;
        }
        
        /*
         * Decreases the overall visitor count
        */ 
        private void DecrementButton_Click(object sender, RoutedEventArgs e)
        {
            int sumType = DetermineDecrementSumType();
            counterLabel.Content = totalVisitors;
            string query = "INSERT INTO attendance ('timestamp', 'category', 'sum_type', 'log_notes') VALUES (@timestamp, @category, @sum_type, @log_notes)";
            InsertIntoDatabase(query, sumType);
            UpdateTimestampLog();          
        }

        /*
         * Returns the necessary sum type for a decrement attendance record 
        */
        private int DetermineDecrementSumType()
        {
            int sumType = 0;
            if (customCount.Text.Length > 0)
            {
                int customCountAsInt = Int16.Parse(customCount.Text);
                if (totalVisitors - customCountAsInt < 0)
                {
                    customCountAsInt = totalVisitors;
                    totalVisitors = 0;
                }
                else
                {
                    totalVisitors = totalVisitors - customCountAsInt;
                }
                sumType = customCountAsInt * -1;
                customCount.Text = "";
            }
            else if (totalVisitors > 0)
            {
                sumType = -1;
                totalVisitors = totalVisitors + sumType;
            }
            return sumType;
        }

        /*
         * Inserts an attendance record into the database 
         * @param query - the SQL query to be executed
         * @param sumType - the number of visitors for this record
        */
        private void InsertIntoDatabase(string query, int sumType)
        {
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            myCommand.Parameters.AddWithValue("@timestamp", DateTime.Now);
            myCommand.Parameters.AddWithValue("@category", title.Text);
            myCommand.Parameters.AddWithValue("@sum_type", sumType);
            myCommand.Parameters.AddWithValue("@log_notes", logNotes.Text);
            var result = myCommand.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }

        
        // watermarked textboxes could maybe go in a new user control since
        // the following four methods are very redundant
        private void WatermarkedTxt_GotFocus(object sender, RoutedEventArgs e)
        {
            watermarkedTxt.Visibility = System.Windows.Visibility.Collapsed;
            logNotes.Visibility = System.Windows.Visibility.Visible;
            logNotes.Focus();
        }

        private void LogNotes_LostFocus(object sender, RoutedEventArgs e)
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

        /*
         * Resets the counter to zero, and eliminates previous attendance data for the counter
        */
        private void Reset_Counter(object sender, RoutedEventArgs e)
        {
            totalVisitors = 0;
            generalTimestampLog.Text = "";
            counterLabel.Content = totalVisitors;
            // delete previous database entries from this counter
        }

        /*
         * Ensures that the user is only able to input numeric values into the customCount TextBox
         * Citation:  https://stackoverflow.com/questions/1268552/how-do-i-get-a-textbox-to-only-accept-numeric-input-in-wpf/17687086
        */
        private void CustomCount_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
