using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZipFile
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<ProgressBarItem> progressBars;
        public ObservableCollection<ProgressBarItem> ProgressBars
        {
            get => progressBars;
            set
            {
                progressBars = value;
                OnPropertyChanged();
            }
        }
        
        private bool isZip = true;
        public bool IsZip
        {
            get => isZip;
            set
            {
                isZip = value;
                OnPropertyChanged();
            }
        }

        private string filePath = "";
        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged();
            }
        }

        private double winHeight = 160;
        public double WinHeight
        {
            get => winHeight;
            set
            {
                winHeight = value;
                OnPropertyChanged();
            }
        }

        private Visibility firstThreadVis = Visibility.Collapsed;
        public Visibility FirstThreadVis
        {
            get => firstThreadVis;
            set
            {
                firstThreadVis = value;
                OnPropertyChanged();
            }
        }

        private Visibility secondThreadVis = Visibility.Collapsed;
        public Visibility SecondThreadVis
        {
            get => secondThreadVis;
            set
            {
                secondThreadVis = value;
                OnPropertyChanged();
            }
        }

        private Visibility thirdThreadVis = Visibility.Collapsed;
        public Visibility ThirdThreadVis
        {
            get => thirdThreadVis;
            set
            {
                thirdThreadVis = value;
                OnPropertyChanged();
            }
        }

        private Visibility fourthThreadVis = Visibility.Collapsed;
        public Visibility FourthThreadVis
        {
            get => fourthThreadVis;
            set
            {
                fourthThreadVis = value;
                OnPropertyChanged();
            }
        }

        private Visibility fifthThreadVis = Visibility.Collapsed;
        public Visibility FifthThreadVis
        {
            get => fifthThreadVis;
            set
            {
                fifthThreadVis = value;
                OnPropertyChanged();
            }
        }

        private double firstThreadProg;
        public double FirstThreadProg
        {
            get { return firstThreadProg; }
            set { firstThreadProg = value; OnPropertyChanged(); }
        }

        private double secondThreadProg;
        public double SecondThreadProg
        {
            get { return secondThreadProg; }
            set { secondThreadProg = value; OnPropertyChanged(); }
        }

        private double thirdThreadProg;
        public double ThirdThreadProg
        {
            get { return thirdThreadProg; }
            set { thirdThreadProg = value; OnPropertyChanged(); }
        }

        private double fourthThreadProg;
        public double FourthThreadProg
        {
            get { return fourthThreadProg; }
            set { fourthThreadProg = value; OnPropertyChanged(); }
        }

        private double fifthThreadProg;
        public double FifthThreadProg
        {
            get { return fifthThreadProg; }
            set { fifthThreadProg = value; OnPropertyChanged(); }
        }

        private double firstThreadMaxProg;
        public double FirstThreadMaxProg
        {
            get { return firstThreadMaxProg; }
            set { firstThreadMaxProg = value; OnPropertyChanged(); }
        }

        private double secondThreadMaxProg;
        public double SecondThreadMaxProg
        {
            get { return secondThreadMaxProg; }
            set { secondThreadMaxProg = value; OnPropertyChanged(); }
        }

        private double thirdThreadMaxProg;
        public double ThirdThreadMaxProg
        {
            get { return thirdThreadMaxProg; }
            set { thirdThreadMaxProg = value; OnPropertyChanged(); }
        }

        private double fourthThreadMaxProg;
        public double FourthThreadMaxProg
        {
            get { return fourthThreadMaxProg; }
            set { fourthThreadMaxProg = value; OnPropertyChanged(); }
        }

        private double fifthThreadMaxProg;
        public double FifthThreadMaxProg
        {
            get { return fifthThreadMaxProg; }
            set { fifthThreadMaxProg = value; OnPropertyChanged(); }
        }

        public double chunkSize { get; set; }

        //--------------------------------------------------------------------

        OpenFileDialog OpenFile;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            ProgressBars = new ObservableCollection<ProgressBarItem>();

            OpenFile = new OpenFileDialog();
            OpenFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            OpenFile.RestoreDirectory = true;

            chunkSize = (Environment.ProcessorCount * 2000) + 256;

            parts = Environment.ProcessorCount;
        }

        //--------------------------------------------------------------------

        private ICommand fileSelect;
        public ICommand FileSelect
        {
            get
            {
                if (fileSelect is null)
                {
                    fileSelect = new RelayCommand(
                        (param) =>
                        {
                            if (OpenFile.ShowDialog() == true)
                            {
                                FilePath = OpenFile.FileName;
                            }
                        });
                }

                return fileSelect;
            }
        }

        //--------------------------------------------------------------------

        List<byte[]> chunksList = new List<byte[]>();
        int parts;

        int tempLengthFs = 0;

        private ICommand startCom;
        public ICommand StartCom
        {
            get
            {
                if (startCom is null)
                {
                    startCom = new RelayCommand(
                        (param) =>
                        {
                            tempLengthFs = 0;

                            using (var fs = new FileStream(FilePath, FileMode.Open))
                            {
                                var chunkSize = (int)Math.Ceiling(fs.Length * 1.0 / parts);
                                var toRead = (int)Math.Min(fs.Length - fs.Position, chunkSize);

                                while (toRead > 0)
                                {
                                    var chunk = new byte[toRead];
                                    fs.Read(chunk, 0, toRead);
                                    tempLengthFs += toRead;
                                    chunksList.Add(chunk);
                                    toRead = (int)Math.Min(fs.Length - fs.Position, toRead);
                                }
                            }

                            if (IsZip)
                                Zip();
                            else
                                UnZip();
                            //Process.Start("explorer", FilePath.Substring(0, FilePath.LastIndexOf('\\')));
                        },
                        (param) =>
                        {
                            if (FilePath == "")
                                return false;
                            else
                                return true;
                        });
                }

                return startCom;
            }
        }

        //--------------------------------------------------------------------

        bool change = true;

        private ICommand cancelCom;
        public ICommand CancelCom
        {
            get
            {
                if (cancelCom is null)
                {
                    cancelCom = new RelayCommand(
                        (param) =>
                        {
                            if (change)
                            {
                                FirstThreadVis = Visibility.Visible;
                                SecondThreadVis = Visibility.Visible;
                                ThirdThreadVis = Visibility.Visible;
                                FourthThreadVis = Visibility.Visible;
                                FifthThreadVis = Visibility.Visible;

                                WinHeight = 330;
                            }
                            else
                            {
                                FirstThreadVis = Visibility.Collapsed;
                                SecondThreadVis = Visibility.Collapsed;
                                ThirdThreadVis = Visibility.Collapsed;
                                FourthThreadVis = Visibility.Collapsed;
                                FifthThreadVis = Visibility.Collapsed;

                                WinHeight = 160;
                            }

                            change = !change;
                        });
                }

                return cancelCom;
            }
        }

        //--------------------------------------------------------------------

        private object sync = new object();

        public void Zip()
        {
            var parts = chunksList.Count;

            for (int i = 0; i < parts; i++)
            {
                ProgressBarItem item = new ProgressBarItem
                {
                    BarValue = 20,
                    BarMaxValue = 100
                };

                ProgressBars.Add(item);
            }

            //FirstThreadVis = Visibility.Visible;
            //SecondThreadVis = Visibility.Visible;
            //ThirdThreadVis = Visibility.Visible;
            //FourthThreadVis = Visibility.Visible;
            //FifthThreadVis = Visibility.Visible;

            WinHeight = 300;

            //FirstThreadMaxProg = tempLengthFs;

            //Dispatcher.Invoke(() => FirstThreadProg = 0);
            //Dispatcher.Invoke(() => SecondThreadProg = 0);
            //Dispatcher.Invoke(() => ThirdThreadProg = 0);
            //Dispatcher.Invoke(() => FourthThreadProg = 0);
            //Dispatcher.Invoke(() => FifthThreadProg = 0);

            Task.Run(() =>
            {
                Parallel.For(0, parts, new ParallelOptions
                {
                    MaxDegreeOfParallelism = 5
                }, (i) =>
                {
                    using (var wms = new MemoryStream())
                    {
                        using (var ds = new DeflateStream(wms, CompressionMode.Compress))
                        {
                            int count = 0;
                            var length = chunksList[i].Length;

                            //switch (i)
                            //{
                            //    case 0:
                            //        FirstThreadMaxProg = chunksList[i].Length;
                            //        break;
                            //    case 1:
                            //        SecondThreadMaxProg = chunksList[i].Length;
                            //        break;
                            //    case 2:
                            //        ThirdThreadMaxProg = chunksList[i].Length;
                            //        break;
                            //    case 3:
                            //        FourthThreadMaxProg = chunksList[i].Length;
                            //        break;
                            //    case 4:
                            //        FifthThreadMaxProg = chunksList[i].Length;
                            //        break;
                            //}

                            while (count < length)
                            {
                                ds.WriteByte(chunksList[i][count]);
                                count++;

                                if (count % 100 == 0)
                                {
                                    lock (sync)
                                    {
                                        Thread.Sleep(10);

                                        //switch (i)
                                        //{
                                        //    case 0:
                                        //        Dispatcher.Invoke(() => FirstThreadProg += 100);
                                        //        break;
                                        //    case 1:
                                        //        Dispatcher.Invoke(() => SecondThreadProg += 100);
                                        //        break;
                                        //    case 2:
                                        //        Dispatcher.Invoke(() => ThirdThreadProg += 100);
                                        //        break;
                                        //    case 3:
                                        //        Dispatcher.Invoke(() => FourthThreadProg += 100);
                                        //        break;
                                        //    case 4:
                                        //        Dispatcher.Invoke(() => FifthThreadProg += 100);
                                        //        break;
                                        //}
                                    }
                                }
                            }
                        }

                        chunksList[i] = wms.ToArray();
                    }
                });
            }).ContinueWith((param) => MessageBox.Show("Done!"));
        }

        //--------------------------------------------------------------------

        public void UnZip()
        {
            //using (var fs = new FileStream(FilePath + ".zip", FileMode.Open))
            //{
            //    using (var ds = new DeflateStream(fs, CompressionMode.Decompress))
            //    {
            //        using (var br = new BinaryReader(ds))
            //        {
            //            var count = br.ReadInt32();
            //            for (var i = 0; i < count; ++i)
            //            {
            //                var len = br.ReadInt32();
            //                var chunk = br.ReadBytes(len);
            //                chunksQueue.Enqueue(chunk);
            //            }
            //        }
            //    }
            //}
        }

        //--------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //--------------------------------------------------------------------
    }
}

/*            var filename = "img.bmp";
            var chunksList = new List<byte[]>();
            var parts = 5;

            using (var fs = new FileStream(filename, FileMode.Open)) {
                var chunkSize = (int)Math.Ceiling(fs.Length * 1.0 / parts);
                var toRead = (int)Math.Min(fs.Length - fs.Position, chunkSize);
                while (toRead > 0) {
                    Console.WriteLine(fs.Length - fs.Position + " " + toRead);
                    var chunk = new byte[toRead];
                    fs.Read(chunk, 0, toRead);
                    chunksList.Add(chunk);
                    toRead = (int)Math.Min(fs.Length - fs.Position, toRead);
                }
            }

            Parallel.For(0, chunksList.Count, new ParallelOptions {
                MaxDegreeOfParallelism = parts
            }, (i) => {
                var wms = new MemoryStream();
                using (var ds = new DeflateStream(wms, CompressionMode.Compress)) {
                    ds.Write(chunksList[i], 0, chunksList[i].Length);
                }
                chunksList[i] = wms.ToArray();
            });

            using (var fs = new FileStream(filename + ".zippo", FileMode.Create)) {
                using (var bw = new BinaryWriter(fs)) {
                    bw.Write(chunksList.Count);
                    foreach (var item in chunksList) {
                        bw.Write(item.Length);
                        bw.Write(item);
                    }
                }
            }*/

/*var filename = "img.bmp";
        var chunkSize = 8192;
        var chunksQueue = new Queue<byte[]>();
        using (var fs = new FileStream(filename, FileMode.Open)) {
            var parts = (int)Math.Ceiling(fs.Length * 1.0 / chunkSize);
            var toRead = (int)Math.Min(fs.Length - fs.Position, chunkSize);
            while (toRead > 0) {
                var chunk = new byte[toRead];
                fs.Read(chunk, 0, toRead);
                chunksQueue.Enqueue(chunk);
                toRead = (int)Math.Min(fs.Length - fs.Position, toRead);
            }
        }
        using (var fs = new FileStream(filename + ".zip", FileMode.Create)) {
            using (var ds = new DeflateStream(fs, CompressionLevel.Optimal)) {
                using (var bw = new BinaryWriter(ds)) {
                    bw.Write(chunksQueue.Count);
                    while (chunksQueue.Count > 0) {
                        var chunk = chunksQueue.Dequeue();
                        bw.Write(chunk.Length);
                        bw.Write(chunk);
                    }
                }
            }
        }
        using (var fs = new FileStream(filename + ".zip", FileMode.Open)) {
            using (var ds = new DeflateStream(fs, CompressionMode.Decompress)) {
                using (var br = new BinaryReader(ds)) {
                    var count = br.ReadInt32();
                    for (var i = 0; i < count; ++i) {
                        var len = br.ReadInt32();
                        var chunk = br.ReadBytes(len);
                        chunksQueue.Enqueue(chunk);
                    }
                }
            }
        }
        using (var fs = new FileStream("unzipped_" + filename, FileMode.Create)) {                
            while (chunksQueue.Count > 0) {
                var chunk = chunksQueue.Dequeue();
                fs.Write(chunk, 0, chunk.Length);
            }
        }*/


/*            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("10.2.14.3"), 7534);

        socket.Bind(ep);
        socket.Listen(10);

        Console.WriteLine("Listening in " + ep.Address + ":" + ep.Port);

        var bytes = new byte[8192];

        var client = socket.Accept();

        while (true)
        {
            var length = client.Receive(bytes);

            var msg = Encoding.Default.GetString(bytes, 0, length);
            Console.WriteLine($"{client.RemoteEndPoint}: {msg}");
            client.Send(Encoding.Default.GetByte("asdf"));
        }*/


/*            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        var ep = new IPEndPoint(IPAddress.Parse("10.2.14.3"), 7534);

        socket.Connect(ep);
        var answer = new byte[8192];

        while (true)
        {
            var msg = Console.ReadLine(); ;
            var data = Encoding.Default.GetBytes(msg);
            socket.Send(data);
            var length = socket.Receive(answer);
            cw("server: " + Encoding.Default.Getstring(answer, 0, length))
        }*/



/* var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    var ep = new IPEndPoint(IPAddress.Parse("10.2.14.3"), 7534);

    var answer = new byte[8192];

    while (true)
    {
        var msg = Console.ReadLine(); ;
        var data = Encoding.Default.GetBytes(msg);
        socket.SendTo(data, ep);
        }
        */


/*
 var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 65Kb
    var ep = new IPEndPoint(IPAddress.Parse("10.2.14.3"), 7534);

    socket.Bind(ep);

    var bytes = new byte[socket.ReceiveBufferSize];
    EndPoint client = new IPEndPoint(IPAddress.Any, 0);

    while (true)
    {
        var length = socket.receiveFrom(bytes, ref client);

        var msg = Encoding.Default.GetString(bytes, 0, length);
        Console.WriteLine($"{client.RemoteEndPoint}: {msg}");

 */
