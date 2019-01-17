using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace KontrolaPristupaDesktop
{
    /// <summary>
    /// Interaction logic for FaceLogIn.xaml
    /// </summary>
    public partial class FaceLogIn : Window
    {
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ee85c084b99f4ebc9c8c5c9101105f4d", "https://westeurope.api.cognitive.microsoft.com/face/v1.0");

        private FilterInfo _currentDevice;

        private BitmapImage _image;


        private IVideoSource _videoSource;
        private VideoFileWriter _writer;
        private string rfid;

        string nazivSlike;

        DBConnect db;

        public FaceLogIn()
        {
            InitializeComponent();
            //VideoDevices = new ObservableCollection<FilterInfo>();
            //GetVideoDevices();
        }
        public FaceLogIn(string rfid)
        {
            InitializeComponent();
            //VideoDevices = new ObservableCollection<FilterInfo>();
            this.DataContext = new FaceLogInViewModel(rfid);
            //GetVideoDevices();
        }
        //public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        //public BitmapImage Image
        //{
        //    get { return _image; }
        //    set { _image = value; }
        //}

        //public FilterInfo CurrentDevice
        //{
        //    get { return _currentDevice; }
        //    set { _currentDevice = value; }
        //}
        //private void GetVideoDevices()
        //{
        //    var devices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        //    foreach (FilterInfo device in devices)
        //    {
        //        VideoDevices.Add(device);
        //    }
        //    if (VideoDevices.Any())
        //    {
        //        CurrentDevice = VideoDevices[0];
        //    }
        //    else
        //    {
        //        MessageBox.Show("No webcam found");
        //    }
        //}

        //private void StartCamera()
        //{
        //    if (CurrentDevice != null)
        //    {

        //        _videoSource = new VideoCaptureDevice(CurrentDevice.MonikerString);
        //        _videoSource.NewFrame += video_NewFrame;
        //        _videoSource.Start();

        //    }
        //    else
        //    {
        //        MessageBox.Show("Current device can't be null");
        //    }
        //}

        //private void video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        //{
        //    try
        //    {
        //        using (var bitmap = (Bitmap)eventArgs.Frame.Clone())
        //        {
        //            var bi = bitmap.ToBitmapImage();
        //            bi.Freeze();
        //            Dispatcher.CurrentDispatcher.Invoke(() => Image = bi);
        //        }
        //    }
        //    catch (Exception exc)
        //    {
        //        MessageBox.Show("Error on _videoSource_NewFrame:\n" + exc.Message, "Error", MessageBoxButton.OK,
        //            MessageBoxImage.Error);
        //        StopCamera();
        //    }
        //}

        //private void StopCamera()
        //{
        //    if (_videoSource != null && _videoSource.IsRunning)
        //    {
        //        _videoSource.SignalToStop();
        //        _videoSource.NewFrame -= video_NewFrame;
        //    }
        //    //Image = null;
        //}
        
        //private async void SaveSnapshot()
        //{
        //    var myUniqueFileName = $@"{DateTime.Now.Ticks}.jpg";
        //    string nazivDirektorija = AppDomain.CurrentDomain.BaseDirectory + @"\Slike\" + rfid + @"\";
        //    System.IO.Directory.CreateDirectory(nazivDirektorija);
        //    nazivSlike = nazivDirektorija + myUniqueFileName;
        //    var encoder = new PngBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create(Image));
        //    using (var filestream = new FileStream(nazivSlike, FileMode.Create))
        //    {
        //        encoder.Save(filestream);
        //    }
        //    StopCamera();
            
        //}
        
        //public void Dispose()
        //{
        //    if (_videoSource != null && _videoSource.IsRunning)
        //    {
        //        _videoSource.SignalToStop();
        //    }
        //    _writer?.Dispose();
        //}

        //private void BtnUkljuciKameru_Click(object sender, RoutedEventArgs e)
        //{
        //    StartCamera();
        //}

        //private void BtnSnimiSliku_Click(object sender, RoutedEventArgs e)
        //{
        //    SaveSnapshot();
        //}

        //private async void BtnPrijaviSe_Click(object sender, RoutedEventArgs e)
        //{
        //    string personGroupId = "students";
        //    string testImageFile = @"D:\Pictures\test_img1.jpg";

        //    using (Stream s = File.OpenRead(nazivSlike))
        //    {
        //        var faces = await faceServiceClient.DetectAsync(s);
        //        var faceIds = faces.Select(face => face.FaceId).ToArray();

        //        var results = await faceServiceClient.IdentifyAsync(personGroupId, faceIds);
        //        foreach (var identifyResult in results)
        //        {
        //            //MessageBox.Show(("Result of face: {0}" + identifyResult.FaceId));
        //            if (identifyResult.Candidates.Length == 0)
        //            {
        //                MessageBox.Show("No one identified");
        //            }
        //            else
        //            {
        //                // Get top 1 among all candidates returned
        //                var candidateId = identifyResult.Candidates[0].PersonId;
        //                var person = await faceServiceClient.GetPersonAsync(personGroupId, candidateId);
        //                //MessageBox.Show("GUID: " + person.Name);
        //                string rfidProba = person.Name.ToString();
        //                if (rfidProba == rfid)
        //                {
        //                    //MessageBox.Show("Uspjesna prijava " + person.Name);
        //                    db = new DBConnect();
        //                    string query = "SELECT * FROM korisnik WHERE rfid = '" + rfidProba + "'";
        //                    var listOfUsers = db.SelectKorisnik(query);
        //                    new PocetnaForma(listOfUsers[0].Ime, listOfUsers[0].Prezime).Show();
                           
        //                }
        //                else
        //                {
        //                    MessageBox.Show("Neuspjesna prijava " + person.Name);
        //                }
                        
        //            }
        //        }
        //    }
        //}
    }
}
