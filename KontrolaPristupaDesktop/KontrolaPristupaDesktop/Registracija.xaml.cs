using Accord.Video.FFMPEG;
using AForge.Video;
using AForge.Video.DirectShow;
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
    /// Interaction logic for Registracija.xaml
    /// </summary>
    public partial class Registracija : Window
    {
        DBConnect db;
        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("ee85c084b99f4ebc9c8c5c9101105f4d", "https://westeurope.api.cognitive.microsoft.com/face/v1.0");

        private FilterInfo _currentDevice;

        private BitmapImage _image;


        private IVideoSource _videoSource;
        private VideoFileWriter _writer;

        private string personGroupId = "students";

        string nazivSlike;


        public Registracija()
        {
            InitializeComponent();
            VideoDevices = new ObservableCollection<FilterInfo>();
            GetVideoDevices();

        }

        public ObservableCollection<FilterInfo> VideoDevices { get; set; }

        public BitmapImage Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public FilterInfo CurrentDevice
        {
            get { return _currentDevice; }
            set { _currentDevice = value; }
        }

        private async void BtnRegistrirajSe_Click(object sender, RoutedEventArgs e)
        {
            string rfid = txtRFID.Text.Replace("\r", string.Empty);
            string ime = txtIme.Text;
            string prezime = txtPrezime.Text;
            if (rfid != "" && ime != "" && prezime != "")
            {
                db = new DBConnect();
                string query = "INSERT INTO korisnik(rfid, ime, prezime) VALUES('" + rfid + "', '" + ime + "', '" + prezime + "')";
                if (db.Insert(query))
                {
                    string personGroupId = "students";
                    await faceServiceClient.UpdatePersonGroupAsync(personGroupId, "SIS students");

                    // Define Anna
                    CreatePersonResult friend1 = await faceServiceClient.CreatePersonAsync(
                        // Id of the PersonGroup that the person belonged to
                        personGroupId,
                        // Name of the person
                        rfid
                    );
                    string friend1ImageDir = AppDomain.CurrentDomain.BaseDirectory + @"\Slike\" + rfid + @"\";
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
                    string query1 = "INSERT INTO slika(id_slike, link, korisnik_rfid) VALUES(default, '" + nazivSlike + "', '" + rfid + "')";
                    db.Insert(query1);
                    Title = "Istrenirano";
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

        #region dohvacanje slike sa kamere

      
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
                MessageBox.Show("No webcam found");
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
            else
            {
                MessageBox.Show("Current device can't be null");
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
            string rfid = txtRFID.Text.Replace("\r", string.Empty);
            string nazivDirektorija = AppDomain.CurrentDomain.BaseDirectory + @"\Slike\" + rfid + @"\";
            System.IO.Directory.CreateDirectory(nazivDirektorija);
            nazivSlike = nazivDirektorija + myUniqueFileName;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Image));
            using (var filestream = new FileStream(nazivSlike, FileMode.Create))
            {
                encoder.Save(filestream);
            }
            StopCamera();
            DetektirajLice(nazivSlike);
        }

        public void Dispose()
        {
            if (_videoSource != null && _videoSource.IsRunning)
            {
                _videoSource.SignalToStop();
            }
            _writer?.Dispose();
        }

        #endregion

        private async void DetektirajLice(string filePath)
        {

            var fileUri = new Uri(filePath);
            var bitMapSource = new BitmapImage();
            bitMapSource = new BitmapImage();
            bitMapSource.BeginInit();
            bitMapSource.CacheOption = BitmapCacheOption.None;
            bitMapSource.UriSource = fileUri;
            bitMapSource.EndInit();
            slika.Source = bitMapSource;
            //Detect the faces
            Title = "Detecting....";
            FaceRectangle[] facesFound = await DetectTheFaces(filePath);
            Title = $"Found {facesFound.Length} faces";

            //Drawing rectangles
            if (facesFound.Length <= 0) return;
            var drwVisual = new DrawingVisual();
            var drwContex = drwVisual.RenderOpen();
            drwContex.DrawImage(bitMapSource, new Rect(0, 0, bitMapSource.Width, bitMapSource.Height));
            var dpi = bitMapSource.DpiX;
            var resizeFactor = 96 / dpi;
            foreach (var faceRect in facesFound)
            {
                drwContex.DrawRectangle(System.Windows.Media.Brushes.Transparent, new System.Windows.Media.Pen(System.Windows.Media.Brushes.Blue, 6),
                    new Rect(faceRect.Left * resizeFactor, faceRect.Top * resizeFactor, faceRect.Width * resizeFactor,
                    faceRect.Height * resizeFactor));
            }
            drwContex.Close();
            var renderToImageCtrl = new RenderTargetBitmap((int)(bitMapSource.PixelWidth * resizeFactor), (int)(bitMapSource.Height * resizeFactor), 96, 96, PixelFormats.Pbgra32);
            renderToImageCtrl.Render(drwVisual);
            slika.Source = renderToImageCtrl;
        }
        private async Task<FaceRectangle[]> DetectTheFaces(string filePath)
        {
            try
            {
                using (var imgStream = File.OpenRead(filePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imgStream);
                    var faceRectangles = faces.Select(face => face.FaceRectangle);
                    return faceRectangles.ToArray();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void BtnUkljuciKameru_Click(object sender, RoutedEventArgs e)
        {
            StartCamera();
        }

        private void BtnSnimiSliku_Click(object sender, RoutedEventArgs e)
        {
            SaveSnapshot();
        }
    }
}
