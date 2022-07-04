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
        private MainFormManager _MainFormManager;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Refresh()
        {
            _MainFormManager.Refresh();
        }

        private void SetState()
        {
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            _MainFormManager = new MainFormManager();
            Refresh();
        }

        private void pbOpen_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_MainFormManager.CurrentWorkFileName))
                return;

            System.Diagnostics.Process.Start(_MainFormManager.CurrentWorkFileName);
        }

        private void pbSelect_Click(object sender, EventArgs e)
        {
            string resultingFileName = "";
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = _MainFormManager.AppDirectory;
                openFileDialog.Filter = "Word files (*.doc)|*.doc";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                    resultingFileName = openFileDialog.FileName;
            }

            if (!string.IsNullOrEmpty(resultingFileName))
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _MainFormManager.FileSelected(resultingFileName);
                    tbDocFileName.Text = resultingFileName;
                    tbValidationErrors.Text = _MainFormManager.ValidationErrors;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }
    }
}
