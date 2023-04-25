using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad_Farsi
{
    public partial class frmSearch : Form
    {
        readonly Form1 _form1;

        public frmSearch(Form1 form1)
        {
            InitializeComponent();
            _form1 = form1;
        }

        List<Search> listHistory = new List<Search>();
        private int IndexOfCurrentList = -1;

        private void btnSearch_Click(object sender, EventArgs e)
        {
            search();
        }

        public void RetrieveHistory(bool isDown)
        {
            try
            {
                if (isDown)
                {
                    IndexOfCurrentList++;
                }
                else
                {
                    IndexOfCurrentList--;
                }

                var showResult = listHistory[IndexOfCurrentList];
                _form1.document.SelectionStart = showResult.SelectionStart;
                _form1.document.SelectionLength = showResult.SelectionLength;
                _form1.document.SelectionBackColor = Color.Yellow;



                if (listHistory.Count <= IndexOfCurrentList)
                {
                    IndexOfCurrentList = 0;
                    listHistory = new List<Search>();
                }
            }
            catch (Exception e)
            {

                if (isDown)
                {
                    IndexOfCurrentList--;
                }
                else
                {
                    IndexOfCurrentList++;
                }


                var showResult = listHistory[IndexOfCurrentList];
                _form1.document.SelectionStart = showResult.SelectionStart;
                _form1.document.SelectionLength = showResult.SelectionLength;
                _form1.document.SelectionBackColor = Color.Yellow;
            }
        }

        private void frmSearch_FormClosing(object sender, FormClosingEventArgs e)
        {
            _form1.document.SelectionBackColor = _form1.document.BackColor;
        }

        private void replace_Click(object sender, EventArgs e)
        {
            search();
            if (!string.IsNullOrEmpty(_form1.document.SelectedText))
            {
                _form1.document.SelectionBackColor = _form1.document.BackColor;
                _form1.document.SelectedText =
                    _form1.document.SelectedText.Replace(_form1.document.SelectedText, txtReplace.Text);
            }
        }

        private void search()
        {
            _form1.document.SelectionBackColor = _form1.document.BackColor;
            if (listHistory.Count > 0)
            {
                RetrieveHistory(rdbDown.Checked);
                return;
            }
            else
            {
                listHistory = new List<Search>();
            }
            var searchOption = RichTextBoxFinds.None;
            if (rdbWholeWord.Checked)
            {
                searchOption = RichTextBoxFinds.WholeWord;
            }
            var searchKey = txtSearch.Text;
            var searchIndex = 0;
            while (searchIndex < _form1.document.TextLength)
            {
                var findIndex = _form1.document.Find(searchKey, searchIndex, searchOption);
                if (findIndex != -1)
                {
                    listHistory.Add(new Search()
                    {
                        SelectionStart = findIndex,
                        SelectionLength = searchKey.Length,
                        SearchKey = searchKey
                    }
                    );
                }
                else
                    break;
                searchIndex = findIndex + searchKey.Length;
            }

            if (!rdbDown.Checked)
            {
                IndexOfCurrentList = listHistory.Count;
            }
            RetrieveHistory(rdbDown.Checked);
        }

        private void replaceAll_Click(object sender, EventArgs e)
        {
            search();
            if (!string.IsNullOrEmpty(_form1.document.SelectedText))
            {
                _form1.document.SelectionBackColor = _form1.document.BackColor;
                _form1.document.Text =
                    _form1.document.Text.Replace(_form1.document.SelectedText, txtReplace.Text);
            }
        }
    }
}
public class Search
{
    public int SelectionStart { get; set; }
    public int SelectionLength { get; set; }
    public string SearchKey { get; set; }
}

