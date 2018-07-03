using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
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


        //--------------------------------------------------------------------

        OpenFileDialog OpenFile;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            OpenFile = new OpenFileDialog();
            OpenFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;

            OpenFile.RestoreDirectory = true;
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
                            byte[] array = null;

                            using (FileStream fstream = File.OpenRead(FilePath))
                            {
                                array = new byte[fstream.Length];

                                fstream.Read(array, 0, array.Length);
                            }

                            var Arrlength = array.Length;

                            byte[] resArray = null;
                            resArray = new byte[Arrlength];

                            if (IsZip)
                            {
                                if (Arrlength <= 100000)
                                {
                                    FirstThreadVis = Visibility.Visible;
                                    FirstThreadMaxProg = Arrlength;

                                    WinHeight = 180;

                                    Task.Run(() => resArray = ZipStr(array, "FirstThreadProg"));
                                }
                                else
                                {
                                    FirstThreadVis = Visibility.Visible;
                                    SecondThreadVis = Visibility.Visible;

                                    Dispatcher.Invoke(() =>
                                    {
                                        SecondThreadMaxProg = FirstThreadMaxProg = Arrlength / 2;
                                    });

                                    WinHeight = 210;

                                    byte[] ArrF = new byte[Arrlength / 2];
                                    byte[] ArrS = new byte[Arrlength / 2];

                                    for (int i = 0; i < Arrlength / 2; i++)
                                    {
                                        ArrF[i] = array[i];
                                    }

                                    for (int i = Arrlength / 2, j = 0; i < Arrlength - 1; i++, j++)
                                    {
                                        ArrS[j] = array[i];
                                    }

                                    byte[] tempArr = new byte[Arrlength / 2];

                                    Task.Run(() => resArray = ZipStr(ArrF, "FirstThreadProg"));
                                    Task.Run(() => tempArr = ZipStr(ArrS, "SecondThreadProg"));

                                    for (int i = 0, j = tempArr.Length; i < tempArr.Length; i++, j++)
                                    {
                                        resArray[j] = tempArr[i];
                                    }
                                }
                            }
                            else
                                resArray = UnZipStr(array);

                            using (FileStream fstream = new FileStream(FilePath, FileMode.OpenOrCreate))
                            {
                                fstream.Write(resArray, 0, resArray.Length);
                            }

                            MessageBox.Show("Done");

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

        static object zip = new object();

        public byte[] ZipStr(byte[] str, string thread)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream deflateZip = new DeflateStream(output, CompressionMode.Compress))
                {
                    for (int i = 0; i < str.Length; i++)
                    {
                        lock (zip)
                        {
                            deflateZip.WriteByte(str[i]);

                            switch (thread)
                            {
                                case "FirstThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        FirstThreadProg += 1;
                                    });
                                    break;
                                case "SecondThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        SecondThreadProg += 1;
                                    });
                                    break;
                                case "ThirdThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        ThirdThreadProg += 1;
                                    });
                                    break;
                                case "FourthThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        FourthThreadProg += 1;
                                    });
                                    break;
                                case "FifthThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        FifthThreadProg += 1;
                                    });
                                    break;
                                case "SixthThreadProg":
                                    Dispatcher.Invoke(() =>
                                    {
                                        SixthThreadProg += 1;
                                    });
                                    break;
                            }
                        }

                        Thread.Sleep(10);
                    }
                }

                return output.ToArray();
            }
        }

        //--------------------------------------------------------------------

        public byte[] UnZipStr(byte[] input)
        {
            byte[] buf = new byte[256];
            long nBytes = 0;

            using (MemoryStream outp = new MemoryStream())
            {
                using (MemoryStream stream = new MemoryStream(input))
                {
                    using (var inp = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        int len;
                        while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                        {
                            outp.Write(buf, 0, len);
                            nBytes += len;
                        }
                    }
                }
            }

            return Encoding.ASCII.GetBytes(nBytes.ToString());
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
