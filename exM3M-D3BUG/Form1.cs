﻿using System;
using System.Diagnostics;
using Binarysharp.MemoryManagement;
using System.Windows.Forms;
using System.Linq;

namespace ExternalTrainerDebugForm
{
    public partial class Form1 : Form
    {
        MemorySharp m = null;
        bool ProcessConnected = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void ProcessTimer_Tick(object sender, EventArgs e)
        {
            Process[] M3M_D3BUG = Process.GetProcessesByName(SelectProcess_textBox.Text);
            if (M3M_D3BUG.Length > 0)
            {
                ProcStatus_Label.Text = "PROCESS CONNECTED";
                ProcessConnected = true;
                m = new MemorySharp(Process.GetProcessesByName(SelectProcess_textBox.Text).First());
                return;
            }
            ProcStatus_Label.Text = "PROCESS NOT DETECTED";
            ProcessConnected = false;
        }

        private void ReadValue_Button_Click(object sender, EventArgs e)
        {
            if (!ProcessConnected)
            {
                return;
            }

            if (ProcAddress_textBox.Text != "")
            {
                string AddressLabel = ProcAddress_textBox.Text;
                IntPtr Address = new IntPtr(Convert.ToInt32(ProcAddress_textBox.Text, 16));
                var AddressValue = m.Read<int>(Address, false);
                int DecimalAddressValue = m.Read<int>(Address, false);
                OffsetAddress_Label.Text = AddressLabel.ToUpper() + " = ";

                //Print Value to label
                if (!OffsetValue2Hex_CheckBox.Checked)
                {
                    ProcAddressValu_Label.Text = DecimalAddressValue.ToString();
                }
                else
                {
                    ProcAddressValu_Label.Text = DecimalAddressValue.ToString("X");
                }

                //Offset Stuff
                if (AddrOffset_textBox.Text != "")
                {
                    //Turns Address Value into Its own Address
                    var PointerOffset = new IntPtr(AddressValue);

                    //Parses the Textbox input to Hex
                    var Offset = int.Parse(AddrOffset_textBox.Text, System.Globalization.NumberStyles.HexNumber);
                    
                    //Do math
                    var NewAddress = PointerOffset + Offset;

                    //Get Value of our new address
                    var OffsetAddressValue = m.Read<int>(NewAddress, false);

                    //Print Offset Address to label
                    OffsetAddress_Label.Text = NewAddress.ToString("X") + " = ";

                    //Print Value to label
                    if (!OffsetValue2Hex_CheckBox.Checked)
                    {
                        ProcAddressValu_Label.Text = OffsetAddressValue.ToString();
                    }
                    else
                    {
                        ProcAddressValu_Label.Text = OffsetAddressValue.ToString("X");
                    }
                }
            }
        }

        private void WriteValue_Button_Click(object sender, EventArgs e)
        {
            if (ProcAddrValue_textBox.Text != "")
            {
                if ((OffsetAddress_Label.Text != "") && (ChangeOffsetValue_checkBox.Checked))
                {
                    string OffsetLabel = OffsetAddress_Label.Text;
                    string OffsetAddressConversion = OffsetAddress_Label.Text.Replace(" = ", "").Replace(" ", "");
                    IntPtr OffsetAddress = new IntPtr(Convert.ToUInt32(OffsetAddressConversion, 16));
                    var OffsetValue = int.Parse(ProcAddrValue_textBox.Text);
                    m.Write<int>(OffsetAddress, OffsetValue, false);
                    ProcAddrNewValue_Label.Text = OffsetValue.ToString();
                    OffsetAddress_Label2.Text = OffsetLabel;
                }
                else
                {
                    IntPtr Address = new IntPtr(Convert.ToInt32(ProcAddress_textBox.Text, 16));
                    var Value = int.Parse(ProcAddrValue_textBox.Text);
                    m.Write<int>(Address, Value, false);
                    ProcAddrNewValue_Label.Text = Value.ToString();
                }
            }
        }
    }
}
