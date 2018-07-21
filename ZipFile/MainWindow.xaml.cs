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
        public ObservableCollection<ProgressBarItem> ProgressBars { get; set; }

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

        private bool filePathIsEnable = true;
        public bool FilePathIsEnable
        {
            get => filePathIsEnable;
            set
            {
                filePathIsEnable = value;
                OnPropertyChanged();
            }
        }

        private bool keyEncDecIsEnable = true;
        public bool KeyEncDecIsEnable
        {
            get => keyEncDecIsEnable;
            set
            {
                keyEncDecIsEnable = value;
                OnPropertyChanged();
            }
        }

        public double chunkSize { get; set; }

        OpenFileDialog OpenFile;
        ParallelOptions options;
        CancellationTokenSource cancelToken;

        List<byte[]> chunksList;
        int parts;
        bool StartState = false;

        //--------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            ProgressBars = new ObservableCollection<ProgressBarItem>();
            chunksList = new List<byte[]>();

            options = new ParallelOptions();
            cancelToken = new CancellationTokenSource();

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
                            using (var fs = new FileStream(FilePath, FileMode.Open))
                            {
                                var chunkSize = (int)Math.Ceiling(fs.Length * 1.0 / parts);
                                var toRead = (int)Math.Min(fs.Length - fs.Position, chunkSize);

                                while (toRead > 0)
                                {
                                    var chunk = new byte[toRead];
                                    fs.Read(chunk, 0, toRead);
                                    chunksList.Add(chunk);
                                    toRead = (int)Math.Min(fs.Length - fs.Position, toRead);
                                }
                            }

                            StartState = true;

                            FilePathIsEnable = false;
                            KeyEncDecIsEnable = false;

                            ZipUnZip(IsZip);

                            //Process.Start("explorer", FilePath.Substring(0, FilePath.LastIndexOf('\\')));
                        },
                        (param) =>
                        {
                            if (StartState)
                                return false;

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

        private object pause = new object();

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
                            //await Task.Yield();
                            lock (pause)
                            {
                                var result = MessageBox.Show("Do u want to cancel ecrypt?", "Alert", MessageBoxButton.YesNo);

                                if (result == MessageBoxResult.Yes)
                                {
                                    cancelToken.Cancel();
                                }
                            }
                        },
                        (param) =>
                        {
                            if (StartState)
                                return true;
                            else
                                return false;
                        });
                }

                return cancelCom;
            }
        }

        //--------------------------------------------------------------------

        private object sync = new object();

        public void ZipUnZip(bool mode)
        {
            var parts = chunksList.Count;

            ProgressBars.Clear();

            for (int i = 0; i < parts; i++)
            {
                ProgressBars.Add(new ProgressBarItem { BarValue = 0, BarMaxValue = 100 });

                WinHeight += 25;
            }

            options.CancellationToken = cancelToken.Token;
            options.MaxDegreeOfParallelism = parts;

            var DeflateMode = (mode == true) ? CompressionMode.Compress : CompressionMode.Decompress; // CompresionLevel.Optimal

            Task.Run(() =>
            {
                Parallel.For(0, parts, options, (i) =>
                {
                    using (var wms = new MemoryStream())
                    {
                        using (var ds = new DeflateStream(wms, DeflateMode))
                        {
                            int count = 0;
                            var length = chunksList[i].Length;

                            ProgressBars[i].BarMaxValue = length;

                            while (count < length)
                            {
                                options.CancellationToken.ThrowIfCancellationRequested();

                                ds.WriteByte(chunksList[i][++count]);

                                if (count % 100 == 0)
                                {
                                    lock (pause)
                                    {
                                        Dispatcher.Invoke(() => ProgressBars[i].BarValue += 100);
                                    }

                                    Thread.Sleep(7);
                                }
                            }
                        }

                        chunksList[i] = wms.ToArray();
                    }
                });
            }).ContinueWith(
                (param) =>
                {
                    if (!param.IsFaulted)
                    {
                        using (var ms = new FileStream(FilePath, FileMode.Open))
                        {
                            using (var bw = new BinaryWriter(ms))
                            {
                                bw.Write(chunksList.Count);
                                foreach (var item in chunksList)
                                {
                                    bw.Write(item.Length);
                                    bw.Write(item);
                                }
                            }
                        }

                        MessageBox.Show("Done!");
                    }
                    else
                        MessageBox.Show("Cancelled!");

                    FilePathIsEnable = true;
                    KeyEncDecIsEnable = true;

                    Dispatcher.Invoke(() => ProgressBars.Clear());

                    WinHeight = 160;

                    StartState = false;

                });
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
