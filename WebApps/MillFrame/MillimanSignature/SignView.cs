using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MillimanSignature
{
    public partial class SignView : Form
    {
        private MillimanCommon.XMLFileSignature Signature = null;

        public SignView()
        {
            InitializeComponent();
        }

        private void Open_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            // Set filter options and filter index.
            openFileDialog1.Filter = "QVW Files (.qvw)|*.qvw|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            // Call the ShowDialog method to show the dialog box.
            DialogResult userClickedOK = openFileDialog1.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == DialogResult.OK)
            {
                if (LoadGrid(openFileDialog1.FileName) == false)
                {
                    MessageBox.Show(Signature.ErrorMessage); 
                }

                SignatureGrid.Enabled = true;
                Update.Enabled = true;
            }
        }

        private bool LoadGrid(string QualifiedFilename)
        {
            try
            {
                SignatureGrid.Rows.Clear();
                this.Text = "File:" + QualifiedFilename;
                Signature = new MillimanCommon.XMLFileSignature(QualifiedFilename);
                foreach (KeyValuePair<string, string> KVP in Signature.SignatureDictionary)
                {
                    SignatureGrid.Rows.Add(new string[] { KVP.Key, KVP.Value });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return string.IsNullOrEmpty(Signature.ErrorMessage); //no error,  we are good
        }

        private void Update_Click(object sender, EventArgs e)
        {
            //make sure there are not duplicate keys
            List<string> KeyList = new List<string>();
            foreach (DataGridViewRow Row in SignatureGrid.Rows)
            {
                if (Row.IsNewRow == false)
                {
                    string Key = Row.Cells["Key"].Value.ToString();
                    if (KeyList.Contains(Key) == true)
                    {
                        MessageBox.Show("Key '" + Key + "' is already present, duplicate keys are not allowed - signature not updated");
                        return;
                    }
                    else if (string.IsNullOrEmpty(Key))
                    {
                        MessageBox.Show("Empty keys are not allowed - signature not updated");
                        return;
                    }
                }
            }
            if (MessageBox.Show("Are you sure you want to update the signature?", "Update", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Signature.SignatureDictionary.Clear();
                foreach (DataGridViewRow DGVR in SignatureGrid.Rows)
                {
                    if (DGVR.IsNewRow == false)
                    {
                        string Key = DGVR.Cells["Key"].Value.ToString();
                        string Value = DGVR.Cells["Value"].Value.ToString();
                        if (string.IsNullOrEmpty(Key) == false)
                        {
                            Signature.SignatureDictionary.Add(Key, Value);
                        
                        }
                    }
                }

                if (Signature.SaveChanges())
                    MessageBox.Show("Signature updated.");
                else
                    MessageBox.Show("Could not update signature.");
            }
        }
    }
}
