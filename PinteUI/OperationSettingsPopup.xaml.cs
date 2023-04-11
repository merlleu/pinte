using System;
using System.Collections.Generic;
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

namespace PinteUI
{
    /// <summary>
    /// Interaction logic for OperationSettingsPopup.xaml
    /// </summary>
    public partial class OperationSettingsPopup : Window
    {
        private UIElement[] values;
        private List<Field> fields;
        public bool Result { get; private set; }
        private DebugConsole debugConsole;

        #region Builder
        /// <summary>
        ///  Creates a Popup builder
        /// </summary>
        /// <param name="operationName">the name of the popup</param>
        public OperationSettingsPopup(string operationName, DebugConsole debugConsole)
        {
            InitializeComponent();
            this.Title = operationName + " - Pinte";
            this.fields = new List<Field>();
            this.values = new UIElement[0];
            this.Result = false;
            this.SubmitButton.Content = operationName;
            this.debugConsole = debugConsole;
        }


        /// <summary>
        /// Add a Double Entry to the popup builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="defaultValue"></param>
        /// <param name="exclude"></param>
        /// <returns>the builder</returns>
        public OperationSettingsPopup Double(string name, string desc, double min, double max, double defaultValue, bool exclude = false) {
            var settings = new string[3];
            settings[0] = min.ToString();
            settings[1] = max.ToString();
            settings[2] = exclude ? "exclude" : "include";
            var field = new Field(name, "double", defaultValue.ToString(), desc, settings);
            this.fields.Add(field);
            return this;
        }

        /// <summary>
        ///  Add a Int Entry to the popup builder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="defaultValue"></param>
        /// <param name="exclude"></param>
        /// <returns>the builder</returns>
        public OperationSettingsPopup Int(string name, string desc, int min, int max, int defaultValue, bool exclude = false) {
            var settings = new string[3];
            settings[0] = min.ToString();
            settings[1] = max.ToString();
            settings[2] = exclude ? "exclude" : "include";
            var field = new Field(name, "int", defaultValue.ToString(),desc, settings);
            this.fields.Add(field);
            return this;
        }

        public OperationSettingsPopup Color(string name, string desc, string defaultValue) {
            var field = new Field(name, "color", defaultValue, desc, new string[0]);
            this.fields.Add(field);
            return this;
        }

        public OperationSettingsPopup Bool(string name, string desc, bool defaultValue) {
            var field = new Field(name, "bool", defaultValue.ToString(), desc, new string[0]);
            this.fields.Add(field);
            return this;
        }

        public OperationSettingsPopup TextArea(string name, string desc, string defaultValue) {
            var field = new Field(name, "textarea", defaultValue, desc, new string[0]);
            this.fields.Add(field);
            return this;
        }

        public OperationSettingsPopup Text(string name, string desc, string defaultValue, int minLength=0, int maxLength = -1) {
            var field = new Field(name, "text", defaultValue, desc, new string[] { minLength.ToString(), maxLength.ToString() });
            this.fields.Add(field);
            return this;
        }

        /// <summary>
        /// Add fields to the popup
        /// </summary>
        public OperationSettingsPopup Finish() {
            string content = "[\n";
            foreach (var field in this.fields) {
                content += "\t"+field.ToString() + ",\n";
            }
            content = content.TrimEnd(',') + "]";
            this.debugConsole.WriteLine("Preparing popup \""+ this.Title +"\": " + content);
            this.values = new UIElement[this.fields.Count];
            for (int i = 0; i < this.fields.Count; i++) {
                var field = this.fields[i];

                // label
                var label = new Label();
                label.Content = field.name + " : ";
                
                UIElement input;
                switch (field.type) {
                    case "int":
                    case "double":
                        var textBox = new TextBox();
                        textBox.Text = field.defaultValue;
                        textBox.Height = 20;
                        input = textBox;
                        break;
                    case "color":
                        var colorPicker = new Xceed.Wpf.Toolkit.ColorPicker();
                        colorPicker.SelectedColor = (Color)ColorConverter.ConvertFromString(field.defaultValue);
                        colorPicker.Height = 25;
                        input = colorPicker;
                        break;
                    case "bool":
                        var checkBox = new CheckBox();
                        checkBox.IsChecked = bool.Parse(field.defaultValue);
                        checkBox.Height = 25;
                        input = checkBox;
                        break;
                    case "textarea":
                        var textArea = new TextBox();
                        textArea.Text = field.defaultValue;
                        textArea.Height = 100;
                        textArea.TextWrapping = TextWrapping.Wrap;
                        textArea.AcceptsReturn = true;
                        textArea.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
                        input = textArea;
                        break;
                    case "text":
                        var text = new TextBox();
                        text.Text = field.defaultValue;
                        text.Height = 20;
                        input = text;
                        break;
                    default:
                        throw new Exception("Invalid type");
                }
                

                // group both label and input on a single line.
                var stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                stackPanel.Margin = new Thickness(10, 0, 10, i == this.fields.Count-1 ? 15 : 0);
                stackPanel.Children.Add(label);
                stackPanel.Children.Add(input);
                stackPanel.Height = 30;

                this.FieldsPanel.Children.Add(stackPanel);
                this.values[i] = input;
            }
            // resize
            this.Height = 80 + this.fields.Count * 35;
            return this;
        }

        #endregion

        #region Validators
        /// <summary>
        /// Check wether any field is valid.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>"" if valid, error otherwise</returns>
        private string IsValid(int id) {
            var field = this.fields[id];
            
            switch (field.type) {
                case "int":
                    var txt = this.values[id] as TextBox;
                    if (txt == null) {
                        return "Invalid type";
                    }
                    return IsValid_Int(field, txt.Text);
                
                case "double":
                    var txt2 = this.values[id] as TextBox;
                    if (txt2 == null) {
                        return "Invalid type";
                    }
                    return IsValid_Double(field, txt2.Text);
                
                case "color":
                case "bool":
                    return "";
                
                case "text":
                    var txt3 = this.values[id] as TextBox;
                    if (txt3 == null) {
                        return "Invalid type";
                    }
                    return IsValid_Text(field, txt3.Text);
                
                default:
                    return "Invalid type";
            }
        }


        /// <summary>
        /// Check if a double is valid.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns>"" if valid, error otherwise</returns>
        private string IsValid_Double(Field field, string val) {
            double value;
            if (!double.TryParse(val, out value)) {
                return "Invalid value: must be X,XX";
            }
            if (field.settings[2] == "exclude") {
                if (value <= double.Parse(field.settings[0]) || value >= double.Parse(field.settings[1])) {
                    return "Value must be between " + field.settings[0] + " and " + field.settings[1];
                }
            } else {
                if (value < double.Parse(field.settings[0]) || value > double.Parse(field.settings[1])) {
                    return "Value must be between " + field.settings[0] + " and " + field.settings[1];
                }
            }
            return "";
        }

        /// <summary>
        /// Check if an int field is valid.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns>"" if valid, error otherwise</returns>
        private string IsValid_Int(Field field, string val) {
            int value;
            if (!int.TryParse(val, out value)) {
                return "Invalid value";
            }
            if (field.settings[2] == "exclude") {
                if (value <= int.Parse(field.settings[0]) || value >= int.Parse(field.settings[1])) {
                    return "Value must be between " + field.settings[0] + " and " + field.settings[1];
                }
            } else {
                if (value < int.Parse(field.settings[0]) || value > int.Parse(field.settings[1])) {
                    return "Value must be between " + field.settings[0] + " and " + field.settings[1];
                }
            }
            return "";
        }

        /// <summary>
        /// Check if a text field is valid.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns>"" if valid, error otherwise</returns>
        private string IsValid_Text(Field field, string val) {
            // field.settings[0] = min length
            // field.settings[1] = max length (-1 if no limit)

            if (val.Length < int.Parse(field.settings[0])) {
                return "Value must be at least " + field.settings[0] + " characters long";
            }
            if (field.settings[1] != "-1" && val.Length > int.Parse(field.settings[1])) {
                return "Value must be at most " + field.settings[1] + " characters long";
            }
            return "";
        }
        #endregion

        #region Getters
        /// <summary>
        /// Get value from a double field.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>the value</returns>
        public double GetDouble(int id) {
            var txt = this.values[id] as TextBox;
            if (txt == null) {
                throw new Exception("Invalid type");
            }
            return double.Parse(txt.Text);
        }

        /// <summary>
        /// Get value from an int field.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int GetInt(int id) {
            var txt = this.values[id] as TextBox;
            if (txt == null) {
                throw new Exception("Invalid type");
            }
            return int.Parse(txt.Text);
        }

        public Pixel GetColor(int id) {
            var colorPicker = this.values[id] as Xceed.Wpf.Toolkit.ColorPicker;
            if (colorPicker == null) {
                throw new Exception("Invalid type");
            }

            Color? selectedColor = colorPicker.SelectedColor;
            if (selectedColor == null)
            {
                throw new Exception("Invalid type");
            }
            var color = selectedColor.Value;
            return new Pixel(color.B, color.G, color.R);
        }

        public bool GetBool(int id) {
            var checkBox = this.values[id] as CheckBox;
            if (checkBox == null || checkBox.IsChecked == null) {
                throw new Exception("Invalid type");
            }
            return checkBox.IsChecked.Value;
        }

        public string GetText(int id) {
            var txt = this.values[id] as TextBox;
            if (txt == null) {
                throw new Exception("Invalid type");
            }
            return txt.Text;
        }

        #endregion

        #region Submit

        /// <summary>
        /// Executed when the button is pressed to send the response back to the opener.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSubmit(object sender, RoutedEventArgs e) {
            for (int i = 0; i < this.fields.Count; i++) {
                var error = IsValid(i);
                if (error != "") {
                    MessageBox.Show(error);
                    return;
                }
            }
            this.Result = true;
            this.Close();
        }

        #endregion
    }

    public class Field {
        public string name;
        public string type;
        public string defaultValue;
        public string desc;
        
        public string[] settings;

        public Field(string name, string type, string defaultValue, string desc, string[] settings) {
            this.name = name;
            this.type = type;
            this.defaultValue = defaultValue;
            this.settings = settings;
            this.desc = desc;
        }

        public override string ToString() {
            string sstr = "[";
            foreach (var s in settings) {
                sstr += "\"" + s + "\", ";
            }
            sstr += "]";
            return $"Field(name: \"{name}\", type: \"{type}\", defaultValue: \"{defaultValue}\", desc: \"{desc}\", settings: {sstr})";
        }
    }
}
