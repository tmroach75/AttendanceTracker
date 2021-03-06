﻿using System;
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
using System.Xml;
using System.Windows.Markup;
using System.Media;

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

        /*
         * creates a new counter when user clicks on "Generate New Counter" button 
        */
        private void GenerateNewCounter_Click(object sender, RoutedEventArgs e)
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

        /*
         * deletes all records from the database
        */
        private void ClearDatabase_Click(object sender, RoutedEventArgs e)
        {
            string warningMessage = "WARNING: You are about to delete your database. All records will be lost, and " +
                "this action cannot be undone. Are you sure you want to delete your database?";
            if (MessageBox.Show(warningMessage, "Attendance Tracker", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
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
        }

        /*
         * exports attendance table from database to csv format
        */
        private void ExportData_Click(object sender, RoutedEventArgs e)
        {
            // needs to go in create directory method
            string path = Environment.CurrentDirectory + "\\Database_Exports";
            Directory.CreateDirectory(path);
            string fileName = "Database_" + DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString() + "_" + DateTime.Now.TimeOfDay.TotalSeconds.ToString() + ".csv";
            StreamWriter sw = new StreamWriter(System.IO.Path.Combine(path, fileName));

            // need two methods. One to deal with database query, one to deal with these loops
            Database databaseObject = new Database();
            string query = "SELECT * FROM attendance";
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            SQLiteDataReader reader = myCommand.ExecuteReader();
            
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
            SaveCounters();
        }

        /*
         * tallys attendance counts and exports a summary table of attendance per hour    
        */
        private void GetReportFormat_Click(object sender, RoutedEventArgs e)
        {
            // checks if user selected a date
            if (BeginningDate.SelectedDate.HasValue)
            {
                DateTime selectedBeginningDate = (DateTime)BeginningDate.SelectedDate;

                // gets the time for each time range
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

                // this could probably go in its own method -- create directory maybe?
                string path = Environment.CurrentDirectory + "\\Daily_Summary_Reports";
                Directory.CreateDirectory(path);

                string fileName = selectedBeginningDate.Year.ToString() + "-" + selectedBeginningDate.Month.ToString() + "-" + selectedBeginningDate.Day.ToString() + ".csv";
                StreamWriter sw = new StreamWriter(System.IO.Path.Combine(path, fileName));

                sw.WriteLine("8:30-10:00 AM, 10:00-11:00 AM, 11:00-12:00 PM, 12:00-1:00 PM, 1:00-2:00 PM, 2:00-3:00 PM, 3:00-4:00 PM, 4:00-5:00 PM, Maps, Gen Visitor, Pro Student, Faculty/Staff");
                sw.WriteLine(eightToTen + "," + tenToEleven + "," + elevenToTwelve + "," + twelveToOne + "," + oneToTwo + "," + twoToThree + "," + threeToFour + "," + fourToFive + "," + maps + "," + general + "," + prospective + "," + staff);
                sw.Close();
            } else
            {
                MessageBox.Show("No starting date was selected", "Attendance Tracker", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*
         * returns the amount of visitors for a specified time range
         * @ param year - the selected year
         * @ param month - the selected month
         * @ param beginningHour - the lower bound hour for the selected time period
         * @ param beginningMinutes - the number of minutes for the lower bound for the selected time (ex. 8:30)
         * @ param endingHour - the upper bound hour for the selected time period
         * @ param
        */
        private int GetTimeRangeSum(int year, int month, int day, int beginningHour, int beginningMinutes, int endingHour, string category)
        {
            DateTime lowerBoundTime = new DateTime(year, month, day, beginningHour, beginningMinutes, 0);
            DateTime upperBoundTime = new DateTime(year, month, day, endingHour, 0, 0);
            string query = "";
            if (category.Length <= 0)
            {
                query = "SELECT COALESCE(SUM(sum_type),0) FROM attendance WHERE category != 'Maps' AND timestamp >= '" + lowerBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND timestamp < '" + upperBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            } else
            {
                query = "SELECT COALESCE(SUM(sum_type),0) FROM attendance WHERE category = '" + category + "' AND timestamp >= '" + lowerBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "' AND timestamp < '" + upperBoundTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
            }

            return GetSumTypeSum(query);
        }

        /*
         * returns the sum of the sum_type column from the attendance table in the database
         * @param query - the SQL query to be executed
        */
        private int GetSumTypeSum(string query)
        {
            Database databaseObject = new Database();
            SQLiteCommand myCommand = new SQLiteCommand(query, databaseObject.myConnection);
            databaseObject.OpenConnection();
            SQLiteDataReader reader = myCommand.ExecuteReader();
            int sum = 0;
            if (reader.Read())
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

        // https://social.msdn.microsoft.com/Forums/vstudio/en-US/65a2064f-2d6a-4ecc-8076-60c72cb7070d/wpf-c-save-controls-created-at-runtime?forum=wpf
        // check the above link for example code of how to save user controls as XML serialization
        private void SaveCounters()
        {
            StringBuilder sb = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.OmitXmlDeclaration = true;
            settings.NewLineOnAttributes = true;

            XamlDesignerSerializationManager dsm = new XamlDesignerSerializationManager(XmlWriter.Create(sb, settings));
            dsm.XamlWriterMode = XamlWriterMode.Expression;

            XamlWriter.Save(CounterStackPanel, dsm);
            string savedControls = sb.ToString();

            File.WriteAllText(@"LastSession.xaml", savedControls);
        }

        private void ReloadCounters(object sender, RoutedEventArgs e)
        {
            StreamReader reader = new StreamReader(@"LastSession.xaml");
            string text = reader.ReadToEnd();
            reader.Close();

            StringReader strReader = new StringReader(text);
            XmlReader xmlReader = XmlReader.Create(strReader);

            StackPanel sp = (StackPanel)System.Windows.Markup.XamlReader.Load(xmlReader);

            foreach(FrameworkElement child in sp.Children)
            {
                CounterStackPanel.Children.Add(CloneFrameworkElement(child));
            }
        }

        FrameworkElement CloneFrameworkElement(FrameworkElement originalElement)
        {
            string elementString = XamlWriter.Save(originalElement);

            StringReader stringReader = new StringReader(elementString);
            XmlReader xmlReader = XmlReader.Create(stringReader);
            FrameworkElement clonedElement = (FrameworkElement)XamlReader.Load(xmlReader);

            return clonedElement;
        }

    }
}
