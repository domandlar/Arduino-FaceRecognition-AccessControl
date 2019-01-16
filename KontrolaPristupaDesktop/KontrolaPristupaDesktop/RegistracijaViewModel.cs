using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KontrolaPristupaDesktop
{
    internal class RegistracijaViewModel : ObservableObject, IDisposable
    {
        #region Private fields
        DBConnect db;
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ee85c084b99f4ebc9c8c5c9101105f4d", "https://westeurope.api.cognitive.microsoft.com/face/v1.0");

        private FilterInfo _currentDevice;

        private BitmapImage _image;


        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private string _rfid;
        private string _ime;
        private string _prezime;

        private string personGroupId = "students";

        string nazivSlike;


        #endregion

        #region Constructor

        public RegistracijaViewModel()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            GetVideoDevices();
            StartSourceCommand = new RelayCommand(StartCamera);
            StopSourceCommand = new RelayCommand(StopCamera);
            SaveSnapshotCommand = new RelayCommand(SaveSnapshot);
            RegistrirajSeCommand = new RelayCommand(RegistrirajSe);
            DohvatiRFIDCommand = new RelayCommand(DohvatiRFID);
        }

        #endregion

        #region Properties

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { Set(ref _currentDevice, value); }
        }
        public string RFID
        {
            get { return _rfid; }
            set { Set(ref _rfid, value); }
        }
        public string Ime
        {
            get { return _ime; }
            set { Set(ref _ime, value); }
        }
        public string Prezime
        {
            get { return _prezime; }
            set { Set(ref _prezime, value); }
        }

        public ICommand StartSourceCommand { get; private set; }

        public ICommand StopSourceCommand { get; private set; }

        public ICommand SaveSnapshotCommand { get; private set; }

        public ICommand RegistrirajSeCommand { get; private set; }

        public ICommand DohvatiRFIDCommand { get; private set; }

        #endregion

        private async void RegistrirajSe()
        {
            string rfid = RFID.Replace("\r", string.Empty);
            string ime = Ime;
            string prezime = Prezime;
            if (rfid != "" && ime != "" && prezime != "")
            {
                db = new DBConnect();
                string query = "INSERT INTO korisnik(rfid, guid, ime, prezime) VALUES('" + rfid + "', 'null', '" + ime + "', '" + prezime + "')";
                if (db.Insert(query))
                {
                    string personGroupId = "students";
                    await faceServiceClient.UpdatePersonGroupAsync(personGroupId, "studenti");

                    // Define Anna
                    CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                        // Id of the PersonGroup that the person belonged to
                        personGroupId,
                        // Name of the person
                        rfid
                    );
                    string friend1ImageDir = AppDomain.CurrentDomain.BaseDirectory + @"Slike\" + rfid + @"\";
                    foreach (string imagePath in Directory.GetFiles(friend1ImageDir, "*.jpg"))
                    {
                        using (Stream s = File.OpenRead(imagePath))
                        {
                            // Detect faces in the image and add to Anna
                            await faceServiceClient.AddPersonFaceAsync(
                                personGroupId, friend1.PersonId, s);
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
                    string query2 = "UPDATE korisnik SET guid='" + friend1.PersonId + "' WHERE rfid='" + rfid + "'";
                    db.Update(query2);
                    nazivSlike = nazivSlike.Replace("\\", "\\\\");
                    string query1 = "INSERT INTO slika(id_slike, link, korisnik_rfid) VALUES(default, '" + nazivSlike + "', '" + rfid + "')";
                    db.Insert(query1);
                    MessageBox.Show("Korisnik " + ime + " " + prezime + " je uspjesno registriran!");
                }
                else
                {
                    MessageBox.Show("Korisnik s ovim RFID-om vec postoji!");
                }

                RFID = "";
                Ime = "";
                Prezime = "";
                Image = null;
            }
            else
            {
                MessageBox.Show("Niste unjeli sve podatke!");
            }
        }
        private void DohvatiRFID()
        {
            RFID = "";
            SerialPort myPort = new SerialPort();
            myPort.BaudRate = 9600;
            myPort.PortName = "COM3";
            myPort.Open();
            string rfid = myPort.ReadLine();
            RFID = rfid;
            myPort.Close();
        }
        
        private void GetVideoDevices()
        {
            var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo device in devices)
            {
                VideoDevices.Add(device);
            }
            if (VideoDevices.Any())
            {
                CurrentDevice = VideoDevices[0];
            }
            else
            {
                MessageBox.Show("Nije pronađena kamera");
            }
        }

        private void StartCamera()
        {
            if (CurrentDevice != null)
            {

                _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
                _videoSource.NewFrame += video_NewFrame;
                _videoSource.Start();

            }
        }

        private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
                {
                    var bi = bitmap.ToBitmapImage();
                    bi.Freeze();
                    Dispatcher.CurrentDispatcher.Invoke(() => Image = bi);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);
                StopCamera();
            }
        }

        private void StopCamera()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
                _videoSource.NewFrame -= video_NewFrame;
            }
            //Image = null;
        }
        private void SaveSnapshot()
        {
            var myUniqueFileName = $@"{DateTime.Now.Ticks}.jpg";
            string rfid = RFID.Replace("\r", string.Empty);
            string nazivDirektorija = AppDomain.CurrentDomain.BaseDirectory + @"Slike\" + rfid + @"\";
            System.IO.Directory.CreateDirectory(nazivDirektorija);
            nazivSlike = nazivDirektorija + myUniqueFileName;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
            using (var filestream = new FileStream(nazivSlike, FileMode.Create))
            {
                encoder.Save(filestream);
            }
            StopCamera();
        }

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
            _writer?.Dispose();
        }

    }
}
