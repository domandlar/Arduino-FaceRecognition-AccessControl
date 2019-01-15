using System;
using System.Collections.Generic;
using System.IO.Ports;
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
using System.Windows.Shapes;

namespace KontrolaPristupaDesktop
{
    /// <summary>
    /// Interaction logic for Prijava.xaml
    /// </summary>
    public partial class Prijava : Window
    {
        DBConnect db;
        public Prijava()
        {
            InitializeComponent();
        }

        private void BtnDohvatiRfid_Click(object sender, RoutedEventArgs e)
        {
            txtRfid.Text = "";
            SerialPort myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = "COM3";
            myPort.Open();
            string rfid = myPort.ReadLine();
            txtRfid.Text = rfid;
            myPort.Close();
        }

        private void BtnPrijaviSe_Click(object sender, RoutedEventArgs e)
        {
            if(txtRfid.Text != "")
            {
                db = new DBConnect();
                string query = "SELECT * FROM korisnik WHERE rfid = '" + txtRfid.Text + "'";
                var listOfUsers = db.SelectKorisnik(query);
                if(listOfUsers.Count != 0)
                {
                    foreach (Korisnik kor in listOfUsers)
                    {
                        MessageBox.Show("Uspjesna prijava za " + kor.Ime + " " + kor.Prezime);
                    }
                }
                else
                {
                    MessageBox.Show("Korisnik ne postoji!");
                }
                
            }
        }
    }
}
