using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace DehexifyStrings
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void ButtonFromFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = openFileDialog1.FileName;

                using (StreamReader Reader = new StreamReader(FileName))
                {
                    string Line;
                    while (Reader.Peek() > 0)
                    {
                        Line = Reader.ReadLine();
                        TextBoxResult.Text += HexToString(Line) + "\r\n";
                    }
                }
            }
        }

        private string HexToString(string Input)
        {
            if (!Regex.IsMatch(Input, @"^[a-fA-F0-9]+$"))
            {
                return null;
            }

            string ReturnVal = string.Empty;

            while (Input.Length > 1)
            {
                string NextCharHex = Input.Substring(0, 2);
                Input = Input.Substring(2);
                ReturnVal += System.Convert.ToChar(Convert.ToUInt16(NextCharHex, 16));
            }

            return ReturnVal;
        }
    }
}
