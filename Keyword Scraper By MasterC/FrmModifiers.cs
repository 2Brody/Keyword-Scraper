using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyword_Scraper_By_MasterC
{
    public partial class FrmModifiers : Form
    {
        List<string> prefixes = new List<string> { " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "y", "x", "y", "z", "how", "which", "why", "where", "who", "when", "are", "what" };
        List<string> suffixes = new List<string> { " ", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "y", "x", "y", "z", "like", "for", "without", "with", "versus", "vs", "to", "near", "except", "has" };
        private bool isPrefixMode;

        public FrmModifiers()
        {
            InitializeComponent();
        }

        private void FrmModifiers_Load(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void LoadPrefixes(List<string> prefixes)
        {
            this.prefixes = prefixes;
            isPrefixMode = true;
            customDataGridView1.Columns.Clear();
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Prefixes";
            column.Name = "Prefixes";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            customDataGridView1.Columns.Add(column);
            foreach (string prefix in prefixes)
            {
                customDataGridView1.Rows.Add(prefix);
            }
        }

        public void LoadPrefixesFromFile()
        {
            if (File.Exists("prefixes.txt"))
            {
                prefixes.Clear();
                isPrefixMode = true;
                customDataGridView1.Columns.Clear();
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = "Prefixes";
                column.Name = "Prefixes";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                customDataGridView1.Columns.Add(column);

                string[] lines = File.ReadAllLines("prefixes.txt");
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        prefixes.Add(line);
                        customDataGridView1.Rows.Add(line);
                    }
                }

            }
        }

        public void SavePrefixes()
        {
            if (File.Exists("prefixes.txt")) { File.WriteAllText("prefixes.txt", string.Empty); }
            prefixes.Clear();
            foreach (DataGridViewRow row in customDataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!(cell.Value.ToString() == ""))
                        {
                            File.AppendAllText("prefixes.txt", cell.Value?.ToString() + Environment.NewLine ?? "");
                            prefixes.Add(cell.Value.ToString() ?? "");
                        }
                    }
                }
            }
            this.Close();
        }

        public void LoadSuffixes(List<string> suffixes)
        {
            this.suffixes = suffixes;
            isPrefixMode = false;
            customDataGridView1.Columns.Clear();
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
            column.HeaderText = "Suffixes";
            column.Name = "Suffixes";
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            customDataGridView1.Columns.Add(column);
            foreach (string suffix in suffixes)
            {
                customDataGridView1.Rows.Add(suffix);
            }
        }

        public void LoadSuffixesFromFile()
        {
            if (File.Exists("suffixes.txt"))
            {
                suffixes.Clear();
                isPrefixMode = false;
                customDataGridView1.Columns.Clear();
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.HeaderText = "Suffixes";
                column.Name = "Suffixes";
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                customDataGridView1.Columns.Add(column);

                string[] lines = File.ReadAllLines("suffixes.txt");
                foreach (string line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        suffixes.Add(line);
                        customDataGridView1.Rows.Add(line);
                    }
                }
            }
        }

        public void SaveSuffixes()
        {
            if (File.Exists("suffixes.txt")) { File.WriteAllText("suffixes.txt", string.Empty); }
            suffixes.Clear();
            foreach (DataGridViewRow row in customDataGridView1.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (!(cell.Value.ToString() == ""))
                        {
                            File.AppendAllText("suffixes.txt", cell.Value?.ToString() + Environment.NewLine ?? "");
                            suffixes.Add(cell.Value.ToString() ?? "");
                        }
                    }
                }
            }
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (isPrefixMode)
            {
                SavePrefixes();
            }
            else
            {
                SaveSuffixes();
            }
        }
    }
}
