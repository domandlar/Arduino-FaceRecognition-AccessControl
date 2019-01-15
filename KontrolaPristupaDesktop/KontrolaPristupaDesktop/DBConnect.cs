using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Windows;

namespace KontrolaPristupaDesktop
{
    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnect()
        {
            Initialize();
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";
            database = "id7653434_sisanje";
            uid = "root";
            password = "";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        //open connection to database
        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0: 
                        break;

                    case 1045:  
                        break;
                }
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                return false;
            }
        }

        //Insert statement
        public bool Insert(string query)
        {
            //string query = "INSERT INTO rfid (id_rfid, value) VALUES(default, 'hahaha')";
            bool provjera = true;
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch(Exception e)
                {
                    return false;
                }
                //Execute command

                //close connection
                this.CloseConnection();
            }
            return provjera;
        }

        //Update statement
        public void Update(string query)
        {
            //string query = "UPDATE tableinfo SET name='Joe', age='22' WHERE name='John Smith'";

            //Open connection
            if (this.OpenConnection() == true)
            {
                //create mysql command
                MySqlCommand cmd = new MySqlCommand();
                //Assign the query using CommandText
                cmd.CommandText = query;
                //Assign the connection using Connection
                cmd.Connection = connection;

                //Execute query
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }

        //Delete statement
        public void Delete(string query)
        {
            //string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        //Select statement
        public List<Rfid> SelectRfid(string query)
        {
            //string query = "SELECT * FROM tableinfo";

            //Create a list to store the result
            var listOfRfid = new List<Rfid>();


            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var rfid = new Rfid();
                    rfid.Id = int.Parse(dr["id_rfid"].ToString());
                    rfid.Value = dr["value"].ToString();
                    
                    listOfRfid.Add(rfid);
                }

                //close Data Reader
                dr.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return listOfRfid;
            }
            else
            {
                return listOfRfid;
            }
        }
        public List<Korisnik> SelectKorisnik(string query)
        {
            //string query = "SELECT * FROM tableinfo";

            //Create a list to store the result
            var listOfUsers = new List<Korisnik>();


            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    var korisnik = new Korisnik();
                    korisnik.Rfid = dr["rfid"].ToString();
                    korisnik.Ime = dr["ime"].ToString();
                    korisnik.Prezime = dr["prezime"].ToString();

                    listOfUsers.Add(korisnik);
                }

                //close Data Reader
                dr.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return listOfUsers;
            }
            else
            {
                return listOfUsers;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }
    }
}