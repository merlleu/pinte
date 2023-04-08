using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Microsoft.Win32;
using PSILib;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private DebugConsole debugConsole;
        public static int windowID = 1;

        public Window1()
        {
            InitializeComponent();
            debugConsole = new DebugConsole(windowID);
        }

        /// <summary>
        /// When the object is destroyed, close the console
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            debugConsole.OnParentClosed(windowID);
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog with only .bmp files
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";

            // Display OpenFileDialog by calling ShowDialog method
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                if (filename != null)
                {
                    var image = new MyImage(filename);
                    var window = new MainWindow(image, filename, this.debugConsole);
                    window.Show();
                    this.Close();
                }
            }
        }

        private void NewCanvas_Click(object sender, RoutedEventArgs e)
        {
            // ask for width, height and background color
            var popup = new OperationSettingsPopup("New Canvas", this.debugConsole)
                .Int("Width", "Width of the image", 2, 10000, 512)
                .Int("Height", "Height of the image", 2, 10000, 512)
                .Color("Background", "Background color of the image", "#000000")
                .Finish();
            
            popup.ShowDialog();

            if (popup.Result)
            {
                var width = popup.GetInt(0);
                var height = popup.GetInt(1);
                var color = popup.GetColor(2);
                var image = new MyImage((uint)width, (uint)height, color);
                var window = new MainWindow(image, "New Image", this.debugConsole);
                window.Show();
                this.Close();
            }
            
        }

        private void NewFractale_Click(object sender, RoutedEventArgs e)
        {
            // ask for width, height
            var popup = new OperationSettingsPopup("New Fractale", this.debugConsole)
                .Int("Width", "Width of the image", 2, 10000, 512)
                .Int("Height", "Height of the image", 2, 10000, 512)
                .Finish();

            popup.ShowDialog();

            if (popup.Result)
            {
                var width = popup.GetInt(0);
                var height = popup.GetInt(1);
                var fractale = new FractaleBuilder((uint)width, (uint)height);
                var image = fractale.Basic();
                var window = new MainWindow(image, "New Fractale", this.debugConsole);
                window.Show();
                this.Close();
            }
        }
    }
}
