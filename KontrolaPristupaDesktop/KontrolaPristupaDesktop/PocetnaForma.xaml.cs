using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ee85c084b99f4ebc9c8c5c9101105f4d", "https://westeurope.api.cognitive.microsoft.com/face/v1.0");
        string ime;
        string prezime;
        string rfid;
        string guid;
        string nazivSlike;
        public PocetnaForma(string ime, string prezime, string rfid, string guid, string nazivSlike)
        {
            InitializeComponent();
            this.ime = ime;
            this.prezime = prezime;
            this.rfid = rfid;
            this.guid = guid;
            this.nazivSlike = nazivSlike;
            MessageBox.Show("Dobro dosli u sustav " + ime + " " + prezime);
            Train();

        }
        private async void Train()
        {
            String personGroupId = "students";
            await faceServiceClient.UpdatePersonGroupAsync(personGroupId, "SISstudenti");


            string friend1ImageDir = AppDomain.CurrentDomain.BaseDirectory + @"Slike\" + rfid + @"\";
            foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
            {
                using (Stream stream = File.OpenRead(imagePath))
                {
                    // Detect faces in the image and add to Anna
                    await faceServiceClient.AddPersonFaceAsync(
                        personGroupId, Guid.Parse(guid), stream);
                }
            }
            await faceServiceClient.TrainPersonGroupAsync(personGroupId);
            TrainingStatus trainingStatus = null;
            while (true)
            {
                trainingStatus = await faceServiceClient.GetPersonGroupTrainingStatusAsync(personGroupId);

                if (trainingStatus.Status != Status.Running)
                {
                    break;
                }

                await Task.Delay(1000);
            }
        }
    }
}
