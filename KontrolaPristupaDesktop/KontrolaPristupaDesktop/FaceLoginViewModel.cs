using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Microsoft.ProjectOxford.Face;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace KontrolaPristupaDesktop
{
    internal class FaceLogInViewModel : ObservableObject, IDisposable
    {

        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ee85c084b99f4ebc9c8c5c9101105f4d", "https://westeurope.api.cognitive.microsoft.com/face/v1.0");

        private FilterInfo _currentDevice;

        private BitmapImage _image;


        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private string rfid;

        string nazivSlike;

        DBConnect db;

        public FaceLogInViewModel()
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            GetVideoDevices();
            StartSourceCommand = new RelayCommand(StartCamera);
            SaveSnapshotCommand = new RelayCommand(SaveSnapshot);
            PrijaviSeCommand = new RelayCommand(PrijaviSe);
        }
        public FaceLogInViewModel(string rfid)
        {
            VideoDevices = new ObservableCollection<FilterInfo>();
            this.rfid = rfid;
            GetVideoDevices();
            StartSourceCommand = new RelayCommand(StartCamera);
            SaveSnapshotCommand = new RelayCommand(SaveSnapshot);
            PrijaviSeCommand = new RelayCommand(PrijaviSe);
        }

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
        public ICommand StartSourceCommand { get; private set; }

        public ICommand SaveSnapshotCommand { get; private set; }

        public ICommand PrijaviSeCommand { get; private set; }

        public ICommand DohvatiRFIDCommand { get; private set; }

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

        private async void SaveSnapshot()
        {
            string nazivDirektorija = AppDomain.CurrentDomain.BaseDirectory + @"Slike\";
            System.IO.Directory.CreateDirectory(nazivDirektorija);
            nazivSlike = nazivDirektorija + "Snapshot.jpg";
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


        private async void PrijaviSe()
        {
            string personGroupId = "students";

            using (Stream s = File.OpenRead(nazivSlike))
            {
                var faces = await faceServiceClient.DetectAsync(s);
                var faceIds = faces.Select(face => face.FaceId).ToArray();

                var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
                foreach (var identifyResult in results)
                {
                    //MessageBox.Show(("Result of face: {0}" + identifyResult.FaceId));
                    if (identifyResult.Candidates.Length == 0)
                    {
                        MessageBox.Show("No one identified");
                    }
                    else
                    {
                        // Get top 1 among all candidates returned
                        var candidateId = identifyResult.Candidates[0].PersonId;
                        var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
                        db = new DBConnect();
                        string query = "SELECT * FROM korisnik WHERE rfid = '" + rfid + "'";
                        var listOfUsers = db.SelectKorisnik(query);
                        //MessageBox.Show("GUID: " + person.Name);
                        //string rfidProba = person.Name.ToString();
                        if (person.PersonId.ToString() == listOfUsers[0].Guid)
                        {
                            //MessageBox.Show("Uspjesna prijava " + person.Name);
                           
                            new PocetnaForma(listOfUsers[0].Ime, listOfUsers[0].Prezime).Show();

                        }
                        else
                        {
                            MessageBox.Show("Neuspjesna prijava " + person.Name);
                        }

                    }
                }
            }
        }
    }
}
