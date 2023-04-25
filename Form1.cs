using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad_Farsi
{
    public partial class Form1 : Form
    {
        public bool TxtChanged = false;
        public string DocumentPath = "";
        List<string> strUndo = new List<string>();
        public Form1()
        {
            InitializeComponent();
            document.Font = fontDialog1.Font;
        }

        private void CreateDocument_Click(object sender, EventArgs e)
        {
            if (TxtChanged)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "تغییراتی در سند اعمال شده است .آیا میخواهید تغببرات را ذخیره کنید؟", "ذخیره سند",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    Save();
                    document.Text = "";
                    TxtChanged = false;
                }
                else if (dialogResult == DialogResult.No)
                {
                    document.Text = "";
                    TxtChanged = false;
                }
            }
            else
            {
                DocumentPath = "";
            }
        }

        private void document_TextChanged(object sender, EventArgs e)
        {
            TxtChanged = true;
            //strUndo.Add(document.Text);
        }

        private void openDocument_Click(object sender, EventArgs e)
        {
            var openFile = openFileDialog1.ShowDialog();
            if (openFile != DialogResult.OK) return;
            DocumentPath = openFileDialog1.FileName;
            using (StreamReader sr = new StreamReader(openFileDialog1.FileName))
            {

                document.Text = sr.ReadToEnd();
            }
        }

        private void openNewWindow_Click(object sender, EventArgs e)
        {
            string currentProcess = Process.GetCurrentProcess().ProcessName;
            Process.Start(currentProcess);
        }

        public void Save()
        {
            string location = "";
            if (string.IsNullOrEmpty(DocumentPath))
            {
                var saveResult = saveFileDialog1.ShowDialog();
                if (saveResult == DialogResult.OK)
                {
                    location = saveFileDialog1.FileName;
                }

            }
            else
            {
                location = DocumentPath;
            }

            using (StreamWriter sw = new StreamWriter(location))
            {
                sw.Write(document.Text);
            }
        }

        private void saveDocument_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(DocumentPath))
            {
                DocumentPath = saveFileDialog1.FileName;

            }
            Save();
        }

        private void saveAsDocument_Click(object sender, EventArgs e)
        {
            DocumentPath = "";
            Save();
        }

        private void chooseFont_Click(object sender, EventArgs e)
        {
            fontDialog1.ShowDialog();
            document.Font = fontDialog1.Font;
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (TxtChanged)
            {
                DialogResult dialogResult = MessageBox.Show(
                    "تغییراتی در سند اعمال شده است .آیا میخواهید تغببرات را ذخیره کنید؟", "ذخیره سند",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                if (dialogResult == DialogResult.Yes)
                {
                    Save();
                }
            }
        }

        private void statusbar_Click(object sender, EventArgs e)
        {
            statusStrip1.Visible = !statusStrip1.Visible;
            statusbar.Checked = !statusbar.Checked;
        }

        private void toolbox_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
            toolbox.Checked = !toolbox.Checked;
        }

        private void copy_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(!string.IsNullOrEmpty(document.SelectedText) ? document.SelectedText : document.Text);
        }

        private void Paste_Click(object sender, EventArgs e)
        {
            //document.Text += Clipboard.GetText(TextDataFormat.UnicodeText).ToString();

            var insertText = Clipboard.GetText(TextDataFormat.UnicodeText).ToString();
            var selectionIndex = document.SelectionStart;
            document.Text = document.Text.Insert(selectionIndex, insertText);
        }

        private void delete_Click(object sender, EventArgs e)
        {
            document.Text = document.Text.Replace(!string.IsNullOrEmpty(document.SelectedText) ? document.SelectedText : document.Text, "");
        }

        private void cut_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(document.SelectedText))
            {
                Clipboard.SetText(document.SelectedText);
                document.Text = document.Text.Replace(document.SelectedText, "");
            }
            else
            {
                Clipboard.SetText(document.Text);
                document.Text = document.Text.Replace(document.Text, "");
            }
        }

        private void selectAll_Click(object sender, EventArgs e)
        {
            document.SelectAll();
        }

        private void InsertDate_Click(object sender, EventArgs e)
        {
            int startIndex = document.SelectionStart;
            document.Text = document.Text.Insert(startIndex, $"{DateTime.Now.ToShortDateString()}-{DateTime.Now.ToShortTimeString()}");
        }

        private void undo_Click(object sender, EventArgs e)
        {
            //document.Text = strUndo.Last();
            //strUndo.RemoveAt(strUndo.Count-1);
        }

        private void print_Click(object sender, EventArgs e)
        {
            printDialog1.Document = printDocument1;
            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string str = document.Text;
            int chars;
            int lines;

            SolidBrush b = new SolidBrush(Color.Black);
            StringFormat strformat = new StringFormat();
            strformat.Trimming = StringTrimming.Word;
            RectangleF myrect = new RectangleF(e.MarginBounds.Left,
                e.MarginBounds.Top, e.MarginBounds.Width, e.MarginBounds.Height);
            SizeF sz = new SizeF(e.MarginBounds.Width, e.MarginBounds.Height - fontDialog1.Font.GetHeight(e.Graphics));
            e.Graphics.MeasureString(str, fontDialog1.Font, sz, strformat, out chars, out lines);
            string printstr = str.Substring(0, chars);
            e.Graphics.DrawString(printstr, fontDialog1.Font, b, myrect, strformat);
            if (str.Length > chars)
            {
                str = str.Substring(chars);
                e.HasMorePages = true;
            }
            else
                e.HasMorePages = false;
        }

        private void search_Click(object sender, EventArgs e)
        {
            frmSearch frmSearch = new frmSearch(this);
            frmSearch.ShowDialog();

        }

        private void replace_Click(object sender, EventArgs e)
        {
            frmSearch frmSearch = new frmSearch(this);
            frmSearch.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString();
        }
    }

}
