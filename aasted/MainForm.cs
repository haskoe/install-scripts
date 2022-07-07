using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace aasted
{
    public partial class MainForm : Form
    {
        private enum eState
        {
            NoDocumentSelected = 2,
            SelectedDocumentInvalid = 3,
            SelectedDocumentValid = 4
        }

        private MainFormManager _MainFormManager;
        private eState _state;


        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Go();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
        }

        private void Go()
        {
            pbOverWrite.Enabled = false;
            pbClose.Enabled = false;
            _MainFormManager = new MainFormManager();
            try
            {
                SetDocumentText("Processing file. Please wait");
                Application.DoEvents();
                Cursor.Current = Cursors.WaitCursor;
                string resultingFile = _MainFormManager.TryProcessLatestFile();
                SetDocumentText(_MainFormManager.ValidationErrors);
                //Application.DoEvents();
                if (!string.IsNullOrEmpty(resultingFile))
                    System.Diagnostics.Process.Start(resultingFile);


            }
            catch (Exception ex)
            {
                SetDocumentText(ex.Message);
            }
            finally
            {
                pbOverWrite.Enabled = !_MainFormManager.ValidDocument;
                pbClose.Enabled = true;
                Cursor.Current = Cursors.Default;
            }
        }

        private void SetDocumentText(string txt)
        {
            webBrowser1.Navigate("about:blank");
            webBrowser1.Document.OpenNew(false);
            webBrowser1.Document.Write(txt);
            webBrowser1.Refresh();
        }

        private void pbOverWrite_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(_MainFormManager.Overwrite());

            }
            catch (Exception ex)
            {
                SetDocumentText(ex.Message);
            }
        }

        private void pbClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
