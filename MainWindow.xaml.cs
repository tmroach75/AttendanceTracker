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
using Excel = Microsoft.Office.Interop.Excel;
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

        // unfortunately, this will not be like a simple browser download. May need to specify some
        // directory for them to go to. Perhaps they can be added to a file on the desktop
        // also need to add funtionality so that user can select which days they would like to export from
        // probably use date picker control and have a calendar be a dropdown. Then the SQL query can only 
        // include pieces of data WHERE timestamp is greater than selected start date and less than selected
        // end date
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
    }
}
