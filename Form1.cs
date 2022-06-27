using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace Dear_Diary
{
    public partial class Form1 : Form
    {
        static MySqlConnection conn1 = new MySqlConnection();
        MySqlDataAdapter adap = new MySqlDataAdapter("Select*from notes", conn1);
        DataSet ds = new DataSet();

        MySqlConnection conn =
            new MySqlConnection("datasource=localhost;port=3306;username=root;password='';database=sakila");

        DataTable dTable = new DataTable();
        static bool t;

        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("uxtheme.dll", EntryPoint = "#95")]
        public static extern uint GetImmersiveColorFromColorSetEx(uint dwImmersiveColorSet, uint dwImmersiveColorType,
            bool bIgnoreHighContrast, uint dwHighContrastCacheMode);

        [DllImport("uxtheme.dll", EntryPoint = "#96")]
        public static extern uint GetImmersiveColorTypeFromName(IntPtr pName);

        [DllImport("uxtheme.dll", EntryPoint = "#98")]
        public static extern int GetImmersiveUserColorSetPreference(bool bForceCheckRegistry, bool bSkipCheckOnFail);

        public Color GetThemeColor()
        {
            var colorSetEx = GetImmersiveColorFromColorSetEx(
                (uint) GetImmersiveUserColorSetPreference(false, false),
                GetImmersiveColorTypeFromName(Marshal.StringToHGlobalUni("ImmersiveStartSelectionBackground")),
                false, 0);

            var col = Color.FromArgb((byte) ((0xFF000000 & colorSetEx) >> 24), (byte) (0x000000FF & colorSetEx),
                (byte) ((0x0000FF00 & colorSetEx) >> 8), (byte) ((0x00FF0000 & colorSetEx) >> 16));

            return col;
        }

        private void Loader(object Sender, EventArgs e)
        {
            lbl1.Visible = false;
            btn1.Visible = false;
            btn2.Visible = false;
            datagv1.Enabled = false;
            ritb1.Visible = false;
            btn3.Visible = false;
            btn4.Visible = true;
            closeNoteToolStripMenuItem.Enabled = false;
            //ritb1.MaxLength = 1000;
            Text = "Dear Diary...";
            //MessageBox.Show(Convert.ToString(lbl1.Visible));
            btn4.BackColor = GetThemeColor();
        }


        private void semisonic(object sender, FormClosingEventArgs ev)
        {
            if (t)
            {
                DialogResult result = MessageBox.Show("You have unsaved work. Are you sure you want to exit?", Text,
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                ev.Cancel = (result == DialogResult.No);
            }
        }

        private static bool existence(string form)
        {
            foreach (Form loaded in Application.OpenForms)
            {
                if (loaded.Text.IndexOf(form) >= 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void myNotesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataTable dta = (DataTable) datagv1.DataSource;
            if (dta != null)
            {
                dta.Clear();
            }

            ritb1.Visible = false;
            lbl1.Visible = false;
            btn1.Visible = false;
            btn2.Visible = false;
            datagv1.Enabled = true;
            datagv1.Visible = true;
            btn3.Visible = true;
            btn4.Visible = false;
            myNotesToolStripMenuItem.Enabled = false;
            conn1.ConnectionString = "datasource=localhost;port=3306;username=root;password='';database=sakila";

            adap.Fill(ds, "notes");
            datagv1.DataSource = ds.Tables[0];
            datagv1.Columns[0].HeaderText = "Date Created";
            datagv1.Columns[1].HeaderText = "Note";
            //datagv1.Columns[0].ReadOnly = true;
            datagv1.Columns[2].Visible = false;

            for (int i = 0; i < datagv1.Columns.Count; i++)
            {
                datagv1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }

            datagv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            //datagv1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        private void newNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            newNoteToolStripMenuItem.BackColor = Color.DarkGray;

            ritb1.Visible = true;
            closeNoteToolStripMenuItem.Enabled = true;
            lbl1.Visible = true;
            btn1.Visible = true;
            btn2.Visible = true;
            btn3.Visible = false;
            btn4.Visible = true;
            datagv1.Visible = false;
            myNotesToolStripMenuItem.Enabled = true;
            //MessageBox.Show(Convert.ToString(ritb1.Visible));
            //MessageBox.Show(Convert.ToString(lbl1.Visible));
        }

        private void closeNoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ritb1.Visible = false;
            btn1.Visible = false;
            btn2.Visible = false;
            lbl1.Visible = false;
            closeNoteToolStripMenuItem.Enabled = false;
        }


        void loadnotes()
        {
            try
            {
                ritb1.Visible = false;
                lbl1.Visible = false;
                btn1.Visible = false;
                btn2.Visible = false;
                datagv1.Enabled = true;
                btn3.Visible = true;
                conn1.ConnectionString = "datasource=localhost;port=3306;username=root;password='';database=sakila";
                DataTable dta = (DataTable) datagv1.DataSource;
                if (dta != null)
                {
                    dta.Clear();
                }

                adap.Fill(ds, "notes");
                adap.TableMappings.Add("notes", "notes");
                datagv1.DataSource = ds.Tables[0];
                datagv1.Columns[0].HeaderText = "Date Created";
                datagv1.Columns[1].HeaderText = "Note";
                //datagv1.Columns[0].ReadOnly = true;
                datagv1.Columns[2].Visible = false;

                for (int i = 0; i < datagv1.Columns.Count; i++)
                {
                    datagv1.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }

                datagv1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                //datagv1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn1_Click(object sender, EventArgs e)
        {
            try
            {
                string query = "Insert into notes (datemod,note) values('" + DateTime.Now.ToString("yyyy-MM-dd") +
                               "','" + ritb1.Text + "');";
                conn.Open();
                MySqlCommand comm = new MySqlCommand(query, conn);
                if (comm.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("Saved");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btn3_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlCommandBuilder cmb = new MySqlCommandBuilder(adap);

                adap.Update(ds, "notes");
                MessageBox.Show("Updated");

                loadnotes();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void datagv1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow dg in datagv1.Rows)
            {
                if (Convert.ToString(dg.Cells[0].Value) == string.Empty)
                {
                    dg.Cells[0].Value = Convert.ToString(DateTime.Now.ToString("yyyy-MM-dd"));
                }
            }
        }

        private void btn4_Click(object sender, EventArgs e)
        {
            if (ofd1.ShowDialog() == DialogResult.OK)
            {
                ritb1.Text=File.ReadAllText(ofd1.FileName);
                //ritb1.LoadFile(ofd1.FileName);
                ritb1.Visible = true;
                btn2.Visible = true;
                btn1.Visible = true;
                lbl1.Visible = true;
            }
        }

        private void btn2_Click(object sender, EventArgs e)
        {
            if (sfd1.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd1.FileName, this.ritb1.Text);
                //ritb1.SaveFile(sfd1.FileName);
            }
        }

        private void viewToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            viewToolStripMenuItem.BackColor = Color.DarkGray;
            viewToolStripMenuItem.ForeColor = Color.Gray;
        }

        private void viewToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            viewToolStripMenuItem.BackColor = Color.FromArgb(64, 64, 64);
            viewToolStripMenuItem.ForeColor = Color.WhiteSmoke;
        }

        private void fileToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
            fileToolStripMenuItem.BackColor = Color.DarkGray;
            fileToolStripMenuItem.ForeColor = Color.Gray;
        }

        private void fileToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            fileToolStripMenuItem.BackColor = Color.FromArgb(64, 64, 64);
            fileToolStripMenuItem.ForeColor = Color.WhiteSmoke;
        }

        private void newNoteToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
            newNoteToolStripMenuItem.BackColor = Color.FromArgb(64, 64, 64);
            newNoteToolStripMenuItem.ForeColor = Color.White;
        }

        private void dgv1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds =
                new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.HighlightText, headerBounds, centerFormat);
        }
    }
}