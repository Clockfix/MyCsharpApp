using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Specialized;

namespace MyNewApp
{
    public partial class FormMain : Form
    {
        SerialPort mySerialPort;
        //PerformanceCounter ramCounter;
        
        public FormMain()
        {
            
            InitializeComponent();

            
            this.FormClosing += new FormClosingEventHandler(myForm_FormClosing);

            var ports = SerialPort.GetPortNames();
            comboBoxPort.DataSource = ports;

            mySerialPort = new SerialPort();
            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(MySerialPort_DataReceived);
        }
        void myForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            mySerialPort.Close();
        }
        delegate void SetTextCallback(Form f, Control ctrl, string text);
        /// <summary>
        /// Set text property of various controls
        /// </summary>
        /// <param name="form">The calling form</param>
        /// <param name="ctrl"></param>
        /// <param name="text"></param>
        public static void SetText(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = text ;
            }
        }

        public static void AddTextIn(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddTextIn);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = ctrl.Text + "-->" + text + "\n" ;
            }
        }

        public static void AddTextOut(Form form, Control ctrl, string text)
        {
            // InvokeRequired required compares the thread ID of the 
            // calling thread to the thread ID of the creating thread. 
            // If these threads are different, it returns true. 
            if (ctrl.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(AddTextOut);
                form.Invoke(d, new object[] { form, ctrl, text });
            }
            else
            {
                ctrl.Text = ctrl.Text + "<--" + text + "\n";
            }
        }


        private void buttonConnect_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(comboBoxPort.Text))
            {
                SetText(this, TextBoxSettings, "Please select a COM port from drop-down menu! Press Refresh button to refresh the list of available COM ports.");
            }
            else
            {
                try
                {
                    if (mySerialPort.IsOpen)
                    {
                        mySerialPort.Close();
                    }
                    mySerialPort.PortName = comboBoxPort.Text;
                    mySerialPort.Open();
                    if (mySerialPort.IsOpen)
                    {
                        SetText(this, TextBoxSettings, "Connected to " + comboBoxPort.Text);
                    }
                }
                catch (IOException ex)
                {
                    SetText(this, TextBoxSettings, ex.Message);
                }
            }
        }
        private void buttonDisconnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (mySerialPort.IsOpen)
                {
                    mySerialPort.Close();
                    SetText(this, TextBoxSettings, "COM port disconnected!");
                }
            }
            catch (IOException ex)
            {
                SetText(this, TextBoxSettings, ex.Message);
            }
                       
        }
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            var ports = SerialPort.GetPortNames();
            comboBoxPort.DataSource = ports;
        }
        private void MySerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string rxString = mySerialPort.ReadExisting();
                byte[] txBuffer = new byte[64];
                AddTextIn(this, COMTextBox, rxString);
                switch (rxString[0])
                {
                    case 'b':
                        
                        //SetText(this, COMTextBoxIn,  rxString);
                        break;
                    case 'v':
                        
                        break;
                    case 'f':
                        
                        break;
                }
            }
            catch (IOException ex)
            {
                SetText(this, TextBoxSettings, ex.Message);
            }
        }
                
        // all Color buttons:
        private void PrintColor(Control ctrl)
        {
            // gets last 2 button name characters
            var str = ctrl.Name.ToString();
            // Get first three characters.
            // string sub = input.Substring(0, 3);
            str = str.Substring(str.Length - 2);
            str = "b" + str + ctrl.BackColor.R.ToString("D3") + ctrl.BackColor.G.ToString("D3") + ctrl.BackColor.B.ToString("D3");
            

            if (mySerialPort.IsOpen)
            {
                AddTextOut(this, COMTextBox, str);
                mySerialPort.Write( str + "\n");
                //mySerialPort.Close();                
            }
            else { 
                SetText(this, TextBoxSettings, "First connect to COM port!"); 
            }
            
        }              
        
        private void ColorSelect_Click(object sender, System.EventArgs e)
        {
            ColorDialog MyDialog = new ColorDialog();
            // Keeps the user from selecting a custom color.
            MyDialog.AllowFullOpen = true;
            // Allows the user to get help. (The default is false.)
            MyDialog.ShowHelp = true;
            // Sets the initial color select to the current text color.
            MyDialog.Color = ColorSelect.BackColor;

            // Update the text box color if the user clicks OK 
            if (MyDialog.ShowDialog() == DialogResult.OK)
            {
                ColorSelect.BackColor = MyDialog.Color;
                PrintColor(ColorSelect);
            }
        }
        private void FixedColor_Click(object sender, System.EventArgs e)
        {
            Button btnClick = (Button)sender;
            ColorSelect.BackColor = btnClick.BackColor;
            //PrintColor(ColorSelect);            
        }        
        private void Matrix_Click(object sender, System.EventArgs e)
        {
            Button btnClick = (Button)sender;
            var butname = sender as Button;
            btnClick.BackColor = ColorSelect.BackColor;
            PrintColor(btnClick);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {            
            if (mySerialPort.IsOpen)
            {
                mySerialPort.Write("c\n");
                SetText(this, COMTextBox, "");
                // clear buttons 
                button00.BackColor = Color.Black;
                button01.BackColor = Color.Black;
                button02.BackColor = Color.Black;
                button03.BackColor = Color.Black;
                button04.BackColor = Color.Black;
                button05.BackColor = Color.Black;
                button06.BackColor = Color.Black;
                button07.BackColor = Color.Black;
                button08.BackColor = Color.Black;
                button09.BackColor = Color.Black;
                button10.BackColor = Color.Black;
                button11.BackColor = Color.Black;
                button12.BackColor = Color.Black;
                button13.BackColor = Color.Black;
                button14.BackColor = Color.Black;
                button15.BackColor = Color.Black;
                butto224.BackColor = Color.Black;
                butto225.BackColor = Color.Black;
                butto226.BackColor = Color.Black;
                butto227.BackColor = Color.Black;
                butto228.BackColor = Color.Black;
                butto229.BackColor = Color.Black;
                butto330.BackColor = Color.Black;
                butto331.BackColor = Color.Black;
                butto223.BackColor = Color.Black;
                butto222.BackColor = Color.Black;
                butto221.BackColor = Color.Black;
                butto220.BackColor = Color.Black;
                butto119.BackColor = Color.Black;
                butto118.BackColor = Color.Black;
                butto117.BackColor = Color.Black;
                butto116.BackColor = Color.Black;
                butto556.BackColor = Color.Black;
                butto557.BackColor = Color.Black;
                butto558.BackColor = Color.Black;
                butto559.BackColor = Color.Black;
                butto660.BackColor = Color.Black;
                butto661.BackColor = Color.Black;
                butto662.BackColor = Color.Black;
                butto663.BackColor = Color.Black;
                butto555.BackColor = Color.Black;
                butto554.BackColor = Color.Black;
                butto553.BackColor = Color.Black;
                butto552.BackColor = Color.Black;
                butto551.BackColor = Color.Black;
                butto550.BackColor = Color.Black;
                butto449.BackColor = Color.Black;
                butto448.BackColor = Color.Black;
                button40.BackColor = Color.Black;
                button41.BackColor = Color.Black;
                button42.BackColor = Color.Black;
                button43.BackColor = Color.Black;
                button44.BackColor = Color.Black;
                button45.BackColor = Color.Black;
                button46.BackColor = Color.Black;
                button47.BackColor = Color.Black;
                butto339.BackColor = Color.Black;
                butto338.BackColor = Color.Black;
                butto337.BackColor = Color.Black;
                butto336.BackColor = Color.Black;
                butto335.BackColor = Color.Black;
                butto334.BackColor = Color.Black;
                butto333.BackColor = Color.Black;
                butto332.BackColor = Color.Black;
            }
            else
            {
                SetText(this, TextBoxSettings, "First connect to COM port!");
            }
        }
    }
}
