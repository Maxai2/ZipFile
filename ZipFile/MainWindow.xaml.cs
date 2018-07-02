using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Text;
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

                            if (IsZip)
                            {
                                var zipResult = ZipStr(array);

                                using (FileStream fstream = new FileStream(FilePath, FileMode.OpenOrCreate))
                                {
                                    fstream.Write(zipResult, 0, zipResult.Length);
                                }


                            }
                            else
                            {
                                var UnZipResult = UnZipStr(FilePath, array);

                                using (FileStream fstream = new FileStream(FilePath, FileMode.OpenOrCreate))
                                {
                                    fstream.Write(UnZipResult, 0, UnZipResult.Length);
                                }
                            }

                            MessageBox.Show("Done");

                            return;

                            //var fileLength = fileText.Length;

                            //if (fileLength < 1000)
                            //{
                            //    FirstThreadVis = Visibility.Visible;

                            //    WinHeight = 180;
                            //}
                            //else
                            //if (1000 <= fileLength && fileLength < 2000)
                            //{
                            //    FirstThreadVis = Visibility.Visible;
                            //    SecondThreadVis = Visibility.Visible;

                            //    WinHeight = 210;
                            //}
                            //else
                            //if (2000 <= fileLength && fileLength < 4000)
                            //{
                            //    FirstThreadVis = Visibility.Visible;
                            //    SecondThreadVis = Visibility.Visible;
                            //    ThirdThreadVis = Visibility.Visible;

                            //    WinHeight = 240;
                            //}
                            //else
                            //if (4000 <= fileLength && fileLength < 8000)
                            //{
                            //    FirstThreadVis = Visibility.Visible;
                            //    SecondThreadVis = Visibility.Visible;
                            //    ThirdThreadVis = Visibility.Visible;
                            //    FourthThreadVis = Visibility.Visible;

                            //    WinHeight = 270;
                            //}
                            //else
                            //if (8000 <= fileLength && fileLength < 16000)
                            //{
                            //    FirstThreadVis = Visibility.Visible;
                            //    SecondThreadVis = Visibility.Visible;
                            //    ThirdThreadVis = Visibility.Visible;
                            //    FourthThreadVis = Visibility.Visible;
                            //    FifthThreadVis = Visibility.Visible;

                            //    WinHeight = 300;
                            //}
                            //else
                            //if (16000 <= fileLength)
                            //{
                            //    FirstThreadVis = Visibility.Visible;
                            //    SecondThreadVis = Visibility.Visible;
                            //    ThirdThreadVis = Visibility.Visible;
                            //    FourthThreadVis = Visibility.Visible;
                            //    FifthThreadVis = Visibility.Visible;
                            //    SixthThreadVis = Visibility.Visible;

                            //    WinHeight = 330;
                            //}

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

        public static byte[] ZipStr(byte[] str)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream deflateZip = new DeflateStream(output, CompressionMode.Compress))
                {
                    deflateZip.Write(str, 0, str.Length);
                }

                return output.ToArray();
            }
        }

        //--------------------------------------------------------------------

        public static byte[] UnZipStr(string file, byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            //using (FileStream inputStream = new FileStream(file, FileMode.OpenOrCreate))
            {
                using (DeflateStream deflateZip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    var bytes = new byte[10000000];
                    var len = 0;
                    //deflateZip.CopyTo(inputStream);
                    len = deflateZip.Read(bytes, 0, input.Length);
                }

                //return Encoding.ASCII.GetBytes(inputStream.ToString());
                return inputStream.ToArray();
            }
        }

        public static long Decompress(Stream inp, Stream outp)
        {
            byte[] buf = new byte[BUF_SIZE];
            long nBytes = 0;

            // Decompress the contents of the input file
            using (inp = new DeflateStream(inp, CompressionMode.Decompress))
            {
                int len;
                while ((len = inp.Read(buf, 0, buf.Length)) > 0)
                {
                    // Write the data block to the decompressed output stream
                    outp.Write(buf, 0, len);
                    nBytes += len;
                }
            }
            // Done
            return nBytes;
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
