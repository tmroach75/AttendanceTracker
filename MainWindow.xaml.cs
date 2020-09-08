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
using System.Data.SQLite;
using System.IO;
using System.Data.Entity;
using System.Data.SqlClient;

namespace AttendanceTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
   
    
    public partial class MainWindow : System.Windows.Window
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

        // deletes all records from the database
        private void clearDatabase_Click(object sender, RoutedEventArgs e)
        {
            Database databaseObject = new Database();
            
            // this type of execution for sql commands can be done through a method
            // maybe just put the query as a string input
            string query = "DELETE FROM attendance";
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            myCommand.ExecuteNonQuery();
            databaseObject.CloseConnection();
        }

        // this could use some helper methods
        private void exportData_Click(object sender, RoutedEventArgs e)
        {
            Database databaseObject = new Database();
            
            string query = "SELECT * FROM attendance";
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();

            SQLiteDataReader reader = myCommand.ExecuteReader();
            string fileName = "test.csv";
            StreamWriter sw = new StreamWriter(fileName);
            object[] output = new object[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                output[i] = reader.GetName(i);
            }

            sw.WriteLine(string.Join(",", output));

            while (reader.Read())
            {
                reader.GetValues(output);
                sw.WriteLine(string.Join(",", output));
            }

            sw.Close();
            reader.Close();
            databaseObject.CloseConnection();   
        }

        // formats database data into the standard attendance reporting format used by the VIC
        private void getReportFormat_Click(object sender, RoutedEventArgs e)
        {
            int eightToTen = GetTimeRangeSum(2020, 9, 7, 8, 30, 10);
        }

        // need a date range, which can come from a calendar pop up
        // combine the date with the time to make a new DateTime
        private int GetTimeRangeSum(int year, int month, int day, int beginningHour, int beginningMinutes, int endingHour)
        {
            DateTime selectedBeginningDate = (DateTime)BeginningDate.SelectedDate;
            DateTime selectedEndingDate = (DateTime)EndingDate.SelectedDate;

            DateTime lowerBoundTime = new DateTime(year, month, day, beginningHour, beginningMinutes, 0);
            DateTime upperBoundTime = new DateTime(year, month, day, endingHour, 0, 0);
            Database databaseObject = new Database();
            string query = "SELECT SUM(sum_type) FROM attendance WHERE timestamp >= '" + lowerBoundTime + "' AND timestamp < '" + upperBoundTime + "'"; // this could possibly lead to a negative sum if only subtracted. So if negative, we want to set to 0
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            SQLiteDataReader reader = myCommand.ExecuteReader();
            int sum = 0;
            // this works, but I'm not sure if it does for the right reasons
            if (reader.Read().GetType().IsPrimitive)
            {
                if (reader.Read() && reader.GetInt32(0) >= 0)
                {
                    sum = reader.GetInt32(0);
                }
            }
            databaseObject.CloseConnection();
            return sum;
        }
    }
}
