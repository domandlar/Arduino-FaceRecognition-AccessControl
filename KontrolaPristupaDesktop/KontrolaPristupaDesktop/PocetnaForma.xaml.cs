using System;
using System.Collections.Generic;
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
    /// Interaction logic for PocetnaForma.xaml
    /// </summary>
    public partial class PocetnaForma : Window
    {
        string ime;
        string prezime;
        public PocetnaForma(string ime, string prezime)
        {
            InitializeComponent();
            this.ime = ime;
            this.prezime = prezime;
            MessageBox.Show("Dobro dosli u sustav " + ime + " " + prezime);
        }
    }
}
