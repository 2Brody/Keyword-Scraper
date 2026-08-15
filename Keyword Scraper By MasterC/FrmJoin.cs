using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyword_Scraper_By_MasterC
{
    public partial class FrmJoin : Form
    {
        private int countdown = 15;
        private Timer timer;
        private Timer colorTimer;
        private int colorIndex = 0;
        private Button currentButton = null;
        private Color[] colors = new Color[]
        {
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Green,
            Color.Blue,
            Color.Indigo,
            Color.Violet
        };

        public FrmJoin()
        {
            InitializeComponent();
            InitializeColorTimer();
            InitializeTimer();
        }

        private void InitializeColorTimer()
        {
            colorTimer = new Timer();
            colorTimer.Interval = 100;
            colorTimer.Tick += ColorTimer_Tick;
        }
        private void InitializeTimer()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += Timer_Tick;
            timer.Start();
            btnJoin.Text = "Join (15)";
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            countdown--;
            btnJoin.Text = $"Join ({countdown})";

            if (countdown <= 0)
            {
                timer.Stop();
                OpenTelegramAndMainForm();
            }
        }
        private void OpenTelegramAndMainForm()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://t.me/DarkChipers",
                UseShellExecute = true
            });

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmJoin_Load(object sender, EventArgs e)
        {
            lblTextShow.Text = "Join in Official Telegram channel\nAll updates and new Checkers will be there.";
        }

        private void btnNotNow_Click(object sender, EventArgs e)
        {
            timer.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            timer.Stop();
            OpenTelegramAndMainForm();
        }

        private void ColorTimer_Tick(object sender, EventArgs e)
        {
            if (currentButton != null)
            {
                currentButton.FlatAppearance.BorderColor = colors[colorIndex];
                colorIndex = (colorIndex + 1) % colors.Length;
            }
        }     

        private void btnNotNow_MouseEnter_1(object sender, EventArgs e)
        {
            currentButton = sender as Button;
            currentButton.FlatAppearance.BorderSize = 2;
            colorIndex = 0;
            colorTimer.Start();
        }

        private void btnJoin_MouseEnter(object sender, EventArgs e)
        {
            currentButton = sender as Button;
            currentButton.FlatAppearance.BorderSize = 2;
            colorIndex = 0;
            colorTimer.Start();
        }

        private void btnNotNow_MouseLeave(object sender, EventArgs e)
        {
            if (currentButton != null)
            {
                currentButton.FlatAppearance.BorderColor = Color.FromKnownColor(KnownColor.Control);
                currentButton.FlatAppearance.BorderSize = 1;
                currentButton = null;
            }
            colorTimer.Stop();
        }

        private void btnJoin_MouseLeave(object sender, EventArgs e)
        {
            if (currentButton != null)
            {
                currentButton.FlatAppearance.BorderColor = Color.FromKnownColor(KnownColor.Control);
                currentButton.FlatAppearance.BorderSize = 1;
                currentButton = null;
            }
            colorTimer.Stop();
        }
    }
}
