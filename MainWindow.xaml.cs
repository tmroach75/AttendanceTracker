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
using System.Data;

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
        // need to get report format for each day in the selected range. This means the CSV that
        // I export will have several rows
        // this will probably involve a loop of some kind
        // first, get summing to work for just one day
        // also need it to be generalized for which category I'm getting. This may need to be very specific for the terms of the visitor center format
        //
        // for now...might be smart to only have functionality for one day
        //      
        private void getReportFormat_Click(object sender, RoutedEventArgs e)
        {
            if (BeginningDate.SelectedDate.HasValue)
            {
                DateTime selectedBeginningDate = (DateTime)BeginningDate.SelectedDate;
                //DateTime selectedEndingDate = (DateTime)EndingDate.SelectedDate;

                // gets the time for each time range. Perhaps this can be done in a loop and added to a list of some kind to reduce redundancy, but that
                // might be a little over-engineering things
                
                int eightToTen = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 8, 30, 10, "");
                int tenToEleven = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 10, 0, 11, "");
                int elevenToTwelve = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 11, 0, 12, "");
                int twelveToOne = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 12, 0, 13, "");
                int oneToTwo = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 13, 0, 14, "");
                int twoToThree = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 14, 0, 15, "");
                int threeToFour = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 15, 0, 16, "");
                int fourToFive = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 16, 0, 17, "");
                int maps = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 8, 30, 17, "Maps");
                int general = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 8, 30, 17, "General");
                int prospective = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 8, 30, 17, "Prospective");
                int staff = GetTimeRangeSum(selectedBeginningDate.Year, selectedBeginningDate.Month, selectedBeginningDate.Day, 8, 30, 17, "Faculty/Staff");
                

                string fileName = selectedBeginningDate.Year.ToString() + "-" + selectedBeginningDate.Month.ToString() + "-" + selectedBeginningDate.Day.ToString() + ".csv";
                StreamWriter sw = new StreamWriter(fileName);

                
                sw.WriteLine("8:30-10:00 AM, 10:00-11:00 AM, 11:00-12:00 PM, 12:00-1:00 PM, 1:00-2:00 PM, 2:00-3:00 PM, 3:00-4:00 PM, 4:00-5:00 PM, Gen Visitor, Pro Student, Faculty/Staff");
                sw.WriteLine(eightToTen + "," + tenToEleven + "," + elevenToTwelve + "," + twelveToOne + "," + oneToTwo + "," + twoToThree + "," + threeToFour + "," + fourToFive + "," + maps + "," + general + "," + prospective + "," + staff);
                sw.Close();
            } else
            {
                MessageBox.Show("No starting date was selected", "Attendance Tracker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // returns the amount of visitors for a specified time range
        private int GetTimeRangeSum(int year, int month, int day, int beginningHour, int beginningMinutes, int endingHour, string category)
        {
            DateTime lowerBoundTime = new DateTime(year, month, day, beginningHour, beginningMinutes, 0);
            DateTime upperBoundTime = new DateTime(year, month, day, endingHour, 0, 0);

            Database databaseObject = new Database();

            string query = "";
            if (category.Length <= 0)
            {
                query = "SELECT COALESCE(SUM(sum_type),0) FROM attendance WHERE category != 'Maps' AND timestamp >= '" + lowerBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND timestamp < '" + upperBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            } else
            {
                query = "SELECT COALESCE(SUM(sum_type),0) FROM attendance WHERE category = '" + category + "' AND timestamp >= '" + lowerBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND timestamp < '" + upperBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }
                
            
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            SQLiteDataReader reader = myCommand.ExecuteReader();

            int sum = 0;
            
            if(reader.Read())
            {
                sum = reader.GetInt32(0);
            }

            
            databaseObject.CloseConnection();

            if (sum < 0)
            {
                sum = 0;
            }

            return sum;
        }
    }
}
