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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PSILib;

namespace PinteUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MyImage image;
        private string uuid;
        private string filename;
        private DebugConsole debugConsole;
        public static int windowID = 2;

        public MainWindow(MyImage image, string filename, DebugConsole debugConsole)
        {
            InitializeComponent();
            this.uuid = Guid.NewGuid().ToString();
            this.image = image;
            this.filename = filename.Substring(filename.LastIndexOf("\\") + 1);
            this.Title = this.filename + " - Pinte - Comme MS Paint(tm) mais en moins bien. Rémi L - Elyess K";
            this.debugConsole = debugConsole;
            debugConsole.SetParent(windowID);
            this.Height = image.Height + 200;
            this.Width = image.Width + 100;
            RenderPreview();
        }

        /// <summary>
        /// When the object is destroyed, close the console
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            debugConsole.OnParentClosed(windowID);
        }

        private void RenderPreview() {
            // Create the temp folder if it doesn't exist
            if (!System.IO.Directory.Exists(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "pinte-cache"))) {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "pinte-cache"));
                this.debugConsole.WriteLine("[Main/RenderPreview] Created temp folder");
            }

            // save image to temp file in the appdata temp folder with a random name & load it in the preview
            this.uuid = Guid.NewGuid().ToString();

            var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "pinte-cache", this.uuid + ".bmp");
            this.image.Save(tempPath);

            this.debugConsole.WriteLine($"[Main/RenderPreview] Saved image to {tempPath}");

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(tempPath, UriKind.Absolute);
            bitmap.EndInit();
            this.ImagePreview.Source = bitmap;

            // delete old temp files
            foreach (string file in System.IO.Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "pinte-cache"))) {
                if (file != tempPath) {
                    try {
                        System.IO.File.Delete(file);
                        this.debugConsole.WriteLine("[Main/RenderPreview] Deleted " + file);
                    } catch {

                    }
                }
            }
        }

        public void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            // save image to file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
            bool? result = saveFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveFileDialog.FileName;
                if (filename != null)
                {
                    this.image.Save(filename);
                }
            }
        }

        public void NewFile_Click(object sender, RoutedEventArgs e)
        {
            // open a instance of the app
            System.Diagnostics.Process.Start(System.Reflection.Assembly.GetExecutingAssembly().Location);
        }

        private void CloseOpenedMenus()
        {
            // close all opened menus
            this.ImageMenu.Visibility = Visibility.Hidden;
            this.StegaMenu.Visibility = Visibility.Hidden;
            this.EffectsMenu.Visibility = Visibility.Hidden;
        }

        #region Image operations
        public void Resize_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for factor
            var PopupAsk = new OperationSettingsPopup("Resize", this.debugConsole)
                .Double("Factor", "The multiplier for the size.", 0.0, 10000.0, 0.8, exclude: true)
                .Finish();
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            double factor = PopupAsk.GetDouble(0);
            this.image.Resize(factor);
            RenderPreview();
        }

        public void Rotate_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for angle
            var PopupAsk = new OperationSettingsPopup("Rotate", this.debugConsole)
                .Int("Angle", "The angle of rotation.", -360, 360, 0)
                .Finish();
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            int angle = PopupAsk.GetInt(0);
            this.image.Rotate(angle);
            RenderPreview();
        }

        public void AddBorders_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for border size and color
            var PopupAsk = new OperationSettingsPopup("Add borders", this.debugConsole)
                .Int("Size", "The size of the border.", 0, (int)this.image.Height/2, 1)
                .Color("Color", "The color of the border.", "#000000")
                .Finish();

            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            uint size = (uint)PopupAsk.GetInt(0);
            Pixel color = PopupAsk.GetColor(1);

            this.image.AddBorder(size, color);
            RenderPreview();
        }

        public void RemoveBorders_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for border size
            var PopupAsk = new OperationSettingsPopup("Remove borders", this.debugConsole)
                .Int("Max Size", "The size of the border.", -1, (int)this.image.Height/2, -1)
                .Color("Color", "The color of the border.", "#000000")
                .Finish();

            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            int size = PopupAsk.GetInt(0);
            Pixel color = PopupAsk.GetColor(1);

            this.image.RemoveBorder(color, max_length: size);
            RenderPreview();
        }

        public void Crop_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for crop size
            var PopupAsk = new OperationSettingsPopup("Crop", this.debugConsole)
                .Int("Start X", "The X coordinate of the bottom left corner of the crop.", 0, (int)this.image.Width, 0)
                .Int("Start Y", "The Y coordinate of the bottom left corner of the crop.", 0, (int)this.image.Height, 0)
                .Int("Width", "The width of the crop.", 0, (int)this.image.Width, (int)this.image.Width)
                .Int("Height", "The height of the crop.", 0, (int)this.image.Height, (int)this.image.Height)
                .Finish();

            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            int startX = PopupAsk.GetInt(0);
            int startY = PopupAsk.GetInt(1);
            int width = PopupAsk.GetInt(2);
            int height = PopupAsk.GetInt(3);

            this.image.Crop((uint)startX, (uint)startY, (uint)width, (uint)height);
            RenderPreview();
        }

        public void Difference_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            // ask for image to compare with
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                string filename = openFileDialog.FileName;
                if (filename != null)
                {
                    try {
                        var otherImage = new MyImage(filename);
                        this.image.Diff(otherImage);
                        RenderPreview();
                    } catch (Exception ex) {
                        this.debugConsole.WriteLine(ex.Message);
                    }
                }
            }
        }

        #endregion

        #region Filters
        public void Grayscale_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            this.image.Grayscale();
            RenderPreview();
        }

        public void Negative_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            this.image.Invert();
            RenderPreview();
        }

        public void EdgeDetection_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            var PopupAsk = new OperationSettingsPopup("Edge detection", this.debugConsole)
                .Bool("Discard borders", "Discard the borders of the image.", true)
                .Bool("Grayscale", "Convert the image to grayscale before applying the filter.", true)
                .Finish();
            
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            bool discardBorders = PopupAsk.GetBool(0);
            bool grayscale = PopupAsk.GetBool(1);

            this.image.EdgeDetection(discardImageArea: discardBorders, gray: grayscale);
            RenderPreview();
        }

        public void Sharpen_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            this.image.Sharpen();
            RenderPreview();
        }

        public void GaussianBlur_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            var PopupAsk = new OperationSettingsPopup("Gaussian blur", this.debugConsole)
                .Bool("5x5 matrix", "Use a 5x5 matrix instead of a 3x3 matrix.", true)
                .Finish();
            
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            bool use5x5 = PopupAsk.GetBool(0);
            
            if (use5x5) this.image.GaussianBlur5();
            else this.image.GaussianBlur3();

            RenderPreview();
        }

        public void BoxBlur_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            this.image.BoxBlur();
            RenderPreview();
        }

        public void UnsharpMask_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            this.image.UnsharpMask5();
            RenderPreview();
        }

        public void CustomKernel_Click(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            var PopupAsk = new OperationSettingsPopup("Custom kernel", this.debugConsole)
                .Int("Size", "The size of the kernel.", 3, 5, 3, false)
                .Int("Divisor", "The divisor of the kernel.", 1, 100, 1, false)
                .Finish();
            
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            int size = PopupAsk.GetInt(0);
            int divisor = PopupAsk.GetInt(1);
            
            
            if (size != 3 && size != 5) {
                MessageBox.Show("The size of the kernel must be 3 or 5.");
                return;
            }

            // ask for kernel
            var PopupAsk2 = new OperationSettingsPopup("Custom kernel", this.debugConsole);
            for (int i = 1; i <= size; i++) {
                PopupAsk2.Text("Line " + i, "The line " + i + " of the kernel.", "0 0 0");
            }
            PopupAsk2.Finish();
            
            PopupAsk2.ShowDialog();
            if (!PopupAsk2.Result) return;

            int[,] kernel = new int[size, size];
            for (int i = 1; i <= size; i++) {
                string[] values = PopupAsk2.GetText(i-1).Split(' ');
                if (values.Length != size) {
                    MessageBox.Show("The line " + i + " of the kernel is not valid.");
                    return;
                }

                for (int j = 0; j < size; j++) {
                    if (!int.TryParse(values[j], out kernel[i-1, j])) {
                        MessageBox.Show("The line " + i + " of the kernel is not valid.");
                        return;
                    }
                }
            }
            try {
                this.image.Convolution(kernel, div: divisor);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            RenderPreview();
        }
        #endregion

        #region Spy tools

        /// <summary>
        /// Hide an image in the current image
        /// </summary>
        public void HideImage(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Bitmap Files (*.bmp)|*.bmp";
            bool? result = fileDialog.ShowDialog();
            if (result != true) return;

            var imageToHide = new MyImage(fileDialog.FileName);

            if (imageToHide.Width > this.image.Width || imageToHide.Height > this.image.Height) {
                MessageBox.Show("The image to hide is bigger than the image to hide it in.");
                return;
            }

            // ask for bits
            var PopupAsk = new OperationSettingsPopup("Hide image", this.debugConsole)
                .Int("Bits", "The number of bits to hide.", 1, 8, 3)
                .Int("X", "The X position of the image.", 0, (int)(this.image.Width - imageToHide.Width), 0)
                .Int("Y", "The Y position of the image.", 0, (int)(this.image.Height - imageToHide.Height), 0)
                .Finish();

            PopupAsk.ShowDialog();

            if (!PopupAsk.Result) return;

            int bits = PopupAsk.GetInt(0);
            uint x = (uint)PopupAsk.GetInt(1);
            uint y = (uint)PopupAsk.GetInt(2);

            this.debugConsole.WriteLine("Hiding image in " + x + ", " + y + " with " + bits + " bits");
            try {
                this.image.HideImage(imageToHide, x, y, bits);
            } catch (Exception ex) {
                MessageBox.Show(ex.Source + ": " + ex.Message);
            }

            RenderPreview();
        }

        /// <summary>
        /// Extract an image from the current image
        /// </summary>
        public void RetrieveHiddenImage(object sender, RoutedEventArgs e) {
            CloseOpenedMenus();
            var PopupAsk = new OperationSettingsPopup("Retrieve hidden image", this.debugConsole)
                .Int("Bits", "The number of bits used to hide.", 1, 8, 3)
                .Int("X", "The X position of the image.", 0, (int)(this.image.Width), 0)
                .Int("Y", "The Y position of the image.", 0, (int)(this.image.Height), 0)
                .Int("Width", "The width of the image.", 1, (int)(this.image.Width), (int)(this.image.Width))
                .Int("Height", "The height of the image.", 1, (int)(this.image.Height), (int)(this.image.Height))
                .Finish();
            
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            int bits = PopupAsk.GetInt(0);
            int x = PopupAsk.GetInt(1);
            int y = PopupAsk.GetInt(2);
            int width = PopupAsk.GetInt(3);
            int height = PopupAsk.GetInt(4);

            this.debugConsole.WriteLine("Extracting image from " + x + ", " + y + " with size " + width + "x" + height + " and " + bits + " bits.");
            try {
                this.image.ExtractImage((uint)x, (uint)y, (uint)width, (uint)height, bits);
            } catch (Exception ex) {
                MessageBox.Show(ex.Source + ": " + ex.Message);
            }

            RenderPreview();
        }

        /// <summary>
        /// Hide a text in the current image
        /// </summary>
        public void HideText(object sender, RoutedEventArgs e)
        {
            CloseOpenedMenus();
            var PopupAsk = new OperationSettingsPopup("Hide text", this.debugConsole)
                .Text("Text", "The text to hide.", "Pinte >>>> Paint !", 0, (int)(this.image.Height * this.image.Width/3))
                .Finish();
            
            PopupAsk.ShowDialog();
            if (!PopupAsk.Result) return;

            string text = PopupAsk.GetText(0);

            this.debugConsole.WriteLine("Hiding text: " + text);
            try {
                this.image.HideText(text);
            } catch (Exception ex) {
                MessageBox.Show(ex.Source + ": " + ex.Message);
            }

            RenderPreview();
        }

        /// <summary>
        /// Extract a text from the current image
        /// </summary>
        public void RetrieveHiddenText(object sender, RoutedEventArgs e) {
            CloseOpenedMenus();
            string text;
            try {
                text = this.image.ExtractText();
            } catch (Exception ex) {
                MessageBox.Show(ex.Source + ": " + ex.Message);
                return;
            }

            this.debugConsole.WriteLine("Extracted text: " + text);
            MessageBox.Show(text);

            RenderPreview();
        }
        #endregion
    }
}
