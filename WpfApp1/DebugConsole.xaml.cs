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

namespace WpfApp1;

public partial class DebugConsole : Window
{
    public bool Enabled { get; private set; }
    private int currentParent = 0;
    public DebugConsole(int parentId)
    {
        InitializeComponent();
        Enabled = true;
        if (Enabled) this.Show();
        currentParent = parentId;
    }
    
    public void WriteLine(string text)
    {
        if (Enabled)
            outputTextBox.AppendText(text + "\n");
    }

    public void OnParentClosed(int parentId)
    {
        WriteLine($"Parent Closed: {parentId}, currentOwner: {currentParent}");
        if (currentParent == parentId)
        {
            WriteLine("Closing Console...");
            this.Close();
        }
    }

    public void SetParent(int parentId)
    {
        WriteLine($"Setting Parent: {parentId}");
        currentParent = parentId;
    }
}
