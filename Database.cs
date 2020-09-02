using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Data.SqlClient;

namespace AttendanceTracker
{

    // much of the following code was taken from the OverSeas Media YouTube channel
    // link: https://www.youtube.com/watch?v=anTP-mgktiI
    class Database
    {
        public SQLiteConnection myConnection;

        public Database()
        {
            // this creates a new database, but it doesn't create any tables in
            // the database
            myConnection = new SQLiteConnection("Data Source=database.sqlite3");
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
            }

            // should probably initialize the database here
            string query = "CREATE TABLE IF NOT EXISTS attendance (id INTEGER PRIMARY KEY, timestamp DATETIME, category TEXT, log_notes TEXT, sum_type INT)";
            myConnection.Open();
            SQLiteCommand myCommand = new SQLiteCommand(query, myConnection);
            myCommand.ExecuteNonQuery();
            myConnection.Close();
            
        }

        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }
    }   
}
