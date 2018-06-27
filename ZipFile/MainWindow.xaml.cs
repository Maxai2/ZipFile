using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZipFile
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

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
    }
}
