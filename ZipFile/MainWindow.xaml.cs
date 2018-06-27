using Microsoft.Win32;
using System;
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

        string fileText;

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
                            using (FileStream fstream = File.OpenRead(FilePath))
                            {
                                byte[] array = new byte[fstream.Length];

                                fstream.Read(array, 0, array.Length);

                                fileText = Encoding.Default.GetString(array);
                            }

                            var fileLength = fileText.Length;

                            if (fileLength < 1000)
                            {

                            }
                            else
                            if (1000 <= fileLength && fileLength < 2000)
                            {

                            }
                            else
                            if (2000 <= fileLength && fileLength < 4000)
                            {

                            }
                            else
                            if (4000 <= fileLength && fileLength < 8000)
                            {

                            }
                            else
                            if (8000 <= fileLength && fileLength < 16000)
                            {

                            }
                            else
                            if (16000 <= fileLength)
                            {

                            }

                        });
                }

                return startCom;
            }
        }

        //--------------------------------------------------------------------

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

                        });
                }

                return cancelCom;
            }
        }

        //--------------------------------------------------------------------

        public static byte[] ZipStr(String str)
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (DeflateStream gzip = new DeflateStream(output, CompressionMode.Compress))
                {
                    using (StreamWriter writer = new StreamWriter(gzip, System.Text.Encoding.UTF8))
                    {
                        writer.Write(str);
                    }
                }

                return output.ToArray();
            }
        }

        //public static void Compress(FileInfo fi)
        //{
        //    using (FileStream inFile = fi.OpenRead())
        //    {
        //        if ((File.GetAttributes(fi.FullName) & FileAttributes.Hidden) != FileAttributes.Hidden & fi.Extension != ".cmp")
        //        {
        //            using (FileStream outFile = File.Create(fi.FullName + ".cmp"))
        //            {
        //                using (DeflateStream Compress = new DeflateStream(outFile, CompressionMode.Compress))
        //                {
        //                    inFile.CopyTo(Compress);

        //                    //Console.WriteLine("Compressed {0} from {1} to {2} bytes.", fi.Name, fi.Length.ToString(), outFile.Length.ToString());
        //                }
        //            }
        //        }
        //    }
        //}

        //--------------------------------------------------------------------

        public static string UnZipStr(byte[] input)
        {
            using (MemoryStream inputStream = new MemoryStream(input))
            {
                using (DeflateStream gzip = new DeflateStream(inputStream, CompressionMode.Decompress))
                {
                    using (StreamReader reader = new StreamReader(gzip, System.Text.Encoding.UTF8))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        //public static void Decompress(FileInfo fi)
        //{
        //    using (FileStream inFile = fi.OpenRead())
        //    {
        //        string curFile = fi.FullName;
        //        string origName = curFile.Remove(curFile.Length - fi.Extension.Length);

        //        using (FileStream outFile = File.Create(origName))
        //        {
        //            using (DeflateStream Decompress = new DeflateStream(inFile, CompressionMode.Decompress))
        //            {
        //                Decompress.CopyTo(outFile);

        //                //Console.WriteLine("Decompressed: {0}", fi.Name);
        //            }
        //        }
        //    }
        //}

        //--------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //--------------------------------------------------------------------
    }
}
