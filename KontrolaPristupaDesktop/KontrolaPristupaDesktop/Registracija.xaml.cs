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
    /// Interaction logic for Registracija.xaml
    /// </summary>
    public partial class Registracija : Window
    {
        DBConnect db;
        public Registracija()
        {
            InitializeComponent();
        }

        private void BtnRegistrirajSe_Click(object sender, RoutedEventArgs e)
        {
            string rfid = txtRFID.Text;
            string ime = txtIme.Text;
            string prezime = txtPrezime.Text;
            if(rfid != "" && ime != "" && prezime != "")
            {
                db = new DBConnect();
                string query = "INSERT INTO korisnik(rfid, ime, prezime) VALUES('" + rfid +"', '"+ime+"', '"+prezime+"')";
                if (db.Insert(query))
                {
                    MessageBox.Show("Korisnik " + ime + " " + prezime + " je uspjesno registriran!");
                }
                else
                {
                    MessageBox.Show("Korisnik s ovim RFID-om vec postoji!");
                }
                
                txtRFID.Text = "";
                txtIme.Text = "";
                txtPrezime.Text = "";

            }
            else
            {
                MessageBox.Show("Niste unjeli sve podatke!");
            }
        }

        private void BtnDohvatiRFID_Click(object sender, RoutedEventArgs e)
        {
            txtRFID.Text = "";
            SerialPort myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = "COM3";
            myPort.Open();
            string rfid = myPort.ReadLine();
            txtRFID.Text = rfid;
            myPort.Close();
            /*
            db = new DBConnect();
            var listOfRfid = db.SelectRfid("SELECT * FROM rfid ORDER BY id_rfid DESC LIMIT 1");
            foreach(Rfid rfid in listOfRfid)
            {
                txtRFID.Text = rfid.Value;
            }
            */
        }
    }
}
