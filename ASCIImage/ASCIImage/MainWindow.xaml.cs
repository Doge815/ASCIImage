using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.Win32;
using System.Windows.Threading;
using System.Drawing;

namespace ASCIImage
{
    public partial class MainWindow : Window
    {
        public BitmapImage Image { get; private set; }
        public string File { get; private set; } = "";
        public object Output { get; private set; }
        private bool ToText { get; set; } = true;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.CheckFileExists = true;
            openFileDialog.Filter = "Image Files(*.BMP; *.JPG; *.GIF)| *.BMP; *.JPG; *.GIF | All files(*.*) | *.*";
            if (openFileDialog.ShowDialog() == true)
            {
                File = openFileDialog.FileName;
                Image = new BitmapImage(new Uri(File));
                ImPrew.Source = Image; 
            }
        }

        private void BtsType_Click(object sender, RoutedEventArgs e)
        {
            ((Button)sender).Background = System.Windows.Media.Brushes.Green;
            (sender == BtImage ? BtText : BtImage).Background = System.Windows.Media.Brushes.Red;
            ToText = (sender == BtText);
        }

        private void BtExport_Click(object sender, RoutedEventArgs e)
        {
            if (Output == null) return;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName(File);
            if (Output is string) { saveFileDialog.Filter = "txt files|*.txt"; saveFileDialog.FileName = File + ".txt"; }
            else {saveFileDialog.Filter = "JPG files|*.jpg"; saveFileDialog.FileName = File + ".jpg"; }
            if(saveFileDialog.ShowDialog().Value)
            {
                if (Output is string)
                {
                    using (StreamWriter s = new StreamWriter(saveFileDialog.FileName, false))
                    {
                        s.Write(Output.ToString());
                        s.Close();
                    }
                }
                else
                {
                    ((Bitmap)Output).Save(saveFileDialog.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        private void BtRender_Click(object sender, RoutedEventArgs e)
        {
            if(File == "")
            {
                MessageBox.Show("No image given!", "failure", MessageBoxButton.OK);
                return;
            }
            MainGrid.IsEnabled = false;
            /*try
            {
                Output = ImageConverter.GetStringyImage(File, (int)(11 - SlReso.Value), x => { PbProgress.Value = x; PbProgress.Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render); });
                if(!ToText) Output = ImageConverter.GetImagyImage((string)Output, 12, x => { PbProgress.Value = x; PbProgress.Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render); });
                MessageBox.Show("Image rendered!", "Success", MessageBoxButton.OK);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.ToString());
                MessageBox.Show("Failure while rendering!", "failure", MessageBoxButton.OK);
            }*/
            Output = ImageConverter.GetStringyImage(File, (int)(11 - SlReso.Value), x => { PbProgress.Value = x; PbProgress.Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render); });
            if (!ToText) Output = ImageConverter.GetImagyImage((string)Output, 12, x => { PbProgress.Value = x; PbProgress.Dispatcher.Invoke(delegate () { }, DispatcherPriority.Render); });
            MessageBox.Show("Image rendered!", "Success", MessageBoxButton.OK);
            MainGrid.IsEnabled = true;
        }
    }
}
