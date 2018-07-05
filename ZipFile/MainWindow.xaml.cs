﻿using Microsoft.Win32;
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
using System.Windows.Input;

namespace ZipFile
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
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

        private Visibility sixthThreadVis = Visibility.Collapsed;
        public Visibility SixthThreadVis
        {
            get => sixthThreadVis;
            set
            {
                sixthThreadVis = value;
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

        private double sixthThreadProg;
        public double SixthThreadProg
        {
            get { return sixthThreadProg; }
            set { sixthThreadProg = value; OnPropertyChanged(); }
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

        private double sixthThreadMaxProg;
        public double SixthThreadMaxProg
        {
            get { return sixthThreadMaxProg; }
            set { sixthThreadMaxProg = value; OnPropertyChanged(); }
        }

        public double chunkSize { get; set; }

        //--------------------------------------------------------------------

        OpenFileDialog OpenFile;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            OpenFile = new OpenFileDialog();
            OpenFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            OpenFile.RestoreDirectory = true;

            chunkSize = (Environment.ProcessorCount * 2000) + 256;
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

        //string fileText;

        List<byte[]> chunksList = new List<byte[]>();
        int parts = 6;

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
                                    Console.WriteLine(fs.Length - fs.Position + " " + toRead);
                                    var chunk = new byte[toRead];
                                    fs.Read(chunk, 0, toRead);
                                    chunksList.Add(chunk);
                                    toRead = (int)Math.Min(fs.Length - fs.Position, toRead);
                                }
                            }

                            Zip();

                            Process.Start("explorer", FilePath.Substring(0, FilePath.LastIndexOf('\\')));
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
                                SixthThreadVis = Visibility.Visible;

                                WinHeight = 330;
                            }
                            else
                            {
                                FirstThreadVis = Visibility.Collapsed;
                                SecondThreadVis = Visibility.Collapsed;
                                ThirdThreadVis = Visibility.Collapsed;
                                FourthThreadVis = Visibility.Collapsed;
                                FifthThreadVis = Visibility.Collapsed;
                                SixthThreadVis = Visibility.Collapsed;

                                WinHeight = 160;
                            }

                            change = !change;
                        });
                }

                return cancelCom;
            }
        }

        //--------------------------------------------------------------------

        int count = 0;

        public void Zip()
        {
            Parallel.For(0, chunksList.Count, new ParallelOptions
            {
                MaxDegreeOfParallelism = parts
            }, (i) =>
            {
                FirstThreadVis = Visibility.Visible;
                WinHeight = 180;

                using (var wms = new MemoryStream())
                {
                    Task.Run(() =>
                    {
                        FirstThreadMaxProg = chunksList[i].Length;

                        using (var ds = new DeflateStream(wms, CompressionMode.Compress))
                        {
                            while (count <= chunksList[i].Length)
                            {
                                ds.Write(chunksList[i], 0, count);
                                count += 10;

                                Dispatcher.Invoke(() =>
                                {
                                    FirstThreadProg++;
                                });
                            }
                        }

                        chunksList[i] = wms.ToArray();
                    });
                }
            });
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

        void ThreeThreadProgress()
        {
            FirstThreadVis = Visibility.Visible;
            SecondThreadVis = Visibility.Visible;
            ThirdThreadVis = Visibility.Visible;

            WinHeight = 240;
        }

        //--------------------------------------------------------------------

        void FourThreadProgress()
        {
            FirstThreadVis = Visibility.Visible;
            SecondThreadVis = Visibility.Visible;
            ThirdThreadVis = Visibility.Visible;
            FourthThreadVis = Visibility.Visible;

            WinHeight = 270;
        }

        //--------------------------------------------------------------------

        void FiveThreadProgress()
        {
            FirstThreadVis = Visibility.Visible;
            SecondThreadVis = Visibility.Visible;
            ThirdThreadVis = Visibility.Visible;
            FourthThreadVis = Visibility.Visible;
            FifthThreadVis = Visibility.Visible;

            WinHeight = 300;
        }

        //--------------------------------------------------------------------

        void SixThreadProgress()
        {
            FirstThreadVis = Visibility.Visible;
            SecondThreadVis = Visibility.Visible;
            ThirdThreadVis = Visibility.Visible;
            FourthThreadVis = Visibility.Visible;
            FifthThreadVis = Visibility.Visible;
            SixthThreadVis = Visibility.Visible;

            WinHeight = 330;
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
