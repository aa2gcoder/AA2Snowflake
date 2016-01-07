﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using SB3Utility;
using AA2Install;
using AA2Data;
using System.Drawing.Imaging;
using Paloma;

namespace AA2Snowflake
{
    public partial class formMain : Form
    {
#warning add a blank button for border editor
#warning use DevIL.NET instead of this shitty tga class
#warning add a toolbox form for editing character metadata
#warning add support for append + custom personalities
#warning add crash dialog and about dialog

        #region Form
        public formLoad load = new formLoad();
        public formMain()
        {
            InitializeComponent();
        }

        public void ShowLoadingForm()
        {
            load.Show(this);

            load.Location = new Point(this.Location.X + (this.Width / 2) - (load.Width / 2), this.Location.Y + (this.Height / 2) - (load.Height / 2));
            this.Enabled = false;
        }

        public void HideLoadingForm()
        {
            Tools.RefreshPPs();
            load.Hide();
            this.Activate();
            this.Enabled = true;
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            UpdateWindowState();
        }
        #endregion
        #region Menu Bar
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void snowflakeGuideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/aa2g/AA2Snowflake/wiki/Snowflake-Guide-v3");
        }
        #endregion

        #region Background
        string backgroundpath;

        private void cmbBackground_SelectedIndexChanged(object sender, EventArgs e)
        {
            backgroundpath = null;
            
            using (var mem = Tools.GetStreamFromSubfile(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_01_0" + cmbBackground.SelectedIndex.ToString() + ".bmp")))
            {
                if (imgBackground.Image != null)
                    imgBackground.Image.Dispose();

                imgBackground.Image = Image.FromStream(mem);
            }
        }

        private void btnLoadBG_Click(object sender, EventArgs e)
        {
            if (cmbBackground.SelectedIndex < 0)
                return;

            using (var file = new OpenFileDialog())
            {
                file.Filter = "Bitmap files (*.bmp)|*.bmp";
                file.Multiselect = false;
                
                if (file.ShowDialog() != DialogResult.Cancel)
                {
                    backgroundpath = file.FileName;
                    if (imgBackground.Image != null)
                        imgBackground.Image.Dispose();
                    
                    imgBackground.Image = Image.FromStream(Tools.GetStreamFromFile(file.FileName));
                }
            }
        }

        private void btnSaveBG_Click(object sender, EventArgs e)
        {
            if (!File.Exists(backgroundpath))
                return;
            
            var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_01_0" + cmbBackground.SelectedIndex.ToString() + ".bmp"));
            var sub = new Subfile(backgroundpath);
            sub.Name = "sp_04_01_0" + cmbBackground.SelectedIndex.ToString() + ".bmp";
            PP.jg2e06_00_00.Subfiles[index] = sub;
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreBG_Click(object sender, EventArgs e)
        {
            if (cmbBackground.SelectedIndex < 0)
                return;

            var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_01_0" + cmbBackground.SelectedIndex.ToString() + ".bmp"));
            var sub = new Subfile(Paths.BACKUP + @"\sp_04_01_0" + cmbBackground.SelectedIndex.ToString() + ".bmp");
            PP.jg2e06_00_00.Subfiles[index] = sub;
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreAllBG_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_01_0" + i.ToString() + ".bmp"));
                var sub = new Subfile(Paths.BACKUP + @"\sp_04_01_0" + i.ToString() + ".bmp");
                PP.jg2e06_00_00.Subfiles[index] = sub;
            }
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }
        #endregion
        #region Border
        string borderpath;

        private void cmbBorder_SelectedIndexChanged(object sender, EventArgs e)
        {
            borderpath = null;

            using (var mem = Tools.GetStreamFromSubfile(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_02_0" + cmbBorder.SelectedIndex.ToString() + ".tga")))
            {
                if (imgBorder.Image != null)
                    imgBorder.Image.Dispose();
                
                using (TargaImage t = new TargaImage(mem))
                    imgBorder.Image = new Bitmap(t.Image);
            }
        }

        private void btnLoadBorder_Click(object sender, EventArgs e)
        {
            if (cmbBorder.SelectedIndex < 0)
                return;

            using (var file = new OpenFileDialog())
            {
                file.Filter = "TGA image (*.tga)|*.tga";
                file.Multiselect = false;

                if (file.ShowDialog() != DialogResult.Cancel)
                {
                    borderpath = file.FileName;
                    if (imgBorder.Image != null)
                        imgBorder.Image.Dispose();

                    imgBorder.Image = new TargaImage(file.FileName).Image;
                }
            }
        }

        private void btnSaveBorder_Click(object sender, EventArgs e)
        {
            if (!File.Exists(borderpath))
                return;

            var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_02_0" + cmbBorder.SelectedIndex.ToString() + ".tga"));
            var sub = new Subfile(borderpath);
            sub.Name = "sp_04_02_0" + cmbBorder.SelectedIndex.ToString() + ".tga";
            PP.jg2e06_00_00.Subfiles[index] = sub;
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreBorder_Click(object sender, EventArgs e)
        {
            if (cmbBorder.SelectedIndex < 0)
                return;

            var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_02_0" + cmbBorder.SelectedIndex.ToString() + ".tga"));
            var sub = new Subfile(Paths.BACKUP + @"\sp_04_02_0" + cmbBorder.SelectedIndex.ToString() + ".tga");
            PP.jg2e06_00_00.Subfiles[index] = sub;
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreAllBorder_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 6; i++)
            {
                var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_02_0" + i.ToString() + ".tga"));
                var sub = new Subfile(Paths.BACKUP + @"\sp_04_02_0" + i.ToString() + ".tga");
                PP.jg2e06_00_00.Subfiles[index] = sub;
            }
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }
        #endregion
        #region Clothes
        string chrpath;

        private void cmbCharacter_SelectedIndexChanged(object sender, EventArgs e)
        {
            chrpath = null;

            using (var mem = Tools.GetStreamFromSubfile(PP.jg2e01_00_00.Subfiles.First(pp => pp.Name == "def0" + cmbCharacter.SelectedIndex.ToString() + ".png")))
            {
                if (imgCharacter.Image != null)
                    imgCharacter.Image.Dispose();

                imgCharacter.Image = Image.FromStream(mem);
            }
        }

        private void btnLoadCHR_Click(object sender, EventArgs e)
        {
            if (cmbCharacter.SelectedIndex < 0)
                return;

            using (var file = new OpenFileDialog())
            {
                file.Filter = "AA2 Card files (*.png)|*.png";
                file.Multiselect = false;

                if (file.ShowDialog() != DialogResult.Cancel)
                {
                    chrpath = file.FileName;
                    if (imgCharacter.Image != null)
                        imgCharacter.Image.Dispose();

                    imgCharacter.Image = Image.FromStream(Tools.GetStreamFromFile(file.FileName));
                }
            }
        }

        private void btnSaveCHR_Click(object sender, EventArgs e)
        {
            if (!File.Exists(chrpath))
                return;

            var index = PP.jg2e01_00_00.Subfiles.IndexOf(PP.jg2e01_00_00.Subfiles.First(pp => pp.Name == "def0" + cmbCharacter.SelectedIndex.ToString() + ".png"));
            var sub = new Subfile(chrpath);
            sub.Name = "def0" + cmbCharacter.SelectedIndex.ToString() + ".png";
            PP.jg2e01_00_00.Subfiles[index] = sub;
            var back = PP.jg2e01_00_00.WriteArchive(PP.jg2e01_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreCHR_Click(object sender, EventArgs e)
        {
            if (cmbCharacter.SelectedIndex < 0)
                return;

            var index = PP.jg2e01_00_00.Subfiles.IndexOf(PP.jg2e01_00_00.Subfiles.First(pp => pp.Name == "def0" + cmbCharacter.SelectedIndex.ToString() + ".png"));
            var sub = new Subfile(Paths.BACKUP + @"\def0" + cmbCharacter.SelectedIndex.ToString() + ".png");
            PP.jg2e01_00_00.Subfiles[index] = sub;
            var back = PP.jg2e01_00_00.WriteArchive(PP.jg2e01_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreAllCHR_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 2; i++)
            {
                var index = PP.jg2e01_00_00.Subfiles.IndexOf(PP.jg2e01_00_00.Subfiles.First(pp => pp.Name == "def0" + i.ToString() + ".png"));
                var sub = new Subfile(Paths.BACKUP + @"\def0" + i.ToString() + ".png");
                PP.jg2e01_00_00.Subfiles[index] = sub;
            }
            var back = PP.jg2e01_00_00.WriteArchive(PP.jg2e01_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }
        #endregion
        #region Poses
        #region 3.1
        private void btnBackup31_Click(object sender, EventArgs e)
        {
            Tools.BackupFile(Paths.AA2Edit + @"\jg2e01_00_00.pp");
        }

        private void btnRestore31_Click(object sender, EventArgs e)
        {
            Tools.RestoreFile(Paths.AA2Edit + @"\jg2e01_00_00.pp");
        }

        private void btnMove31_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you absolutely certain you want to move poses? Have you made a backup yet?", "Anti-fuckup dialog", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            int index;
            foreach (IWriteFile iw in PP.jg2p01_00_00.Subfiles)
            {
                if (iw.Name.StartsWith("HAK"))
                    iw.Name = "HAE" + iw.Name.Remove(0, 3);
                if (iw.Name.StartsWith("HSK"))
                    iw.Name = "HSE" + iw.Name.Remove(0, 3);

                index = PP.jg2e01_00_00.Subfiles.FindIndex(pp => pp.Name == iw.Name);
                if (index < 0)
                    PP.jg2e01_00_00.Subfiles.Add(iw);
                else
                    PP.jg2e01_00_00.Subfiles[index] = iw;
            }

            var back = PP.jg2e01_00_00.WriteArchive(PP.jg2e01_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            index = PP.jg2e00_00_00.Subfiles.IndexOf(PP.jg2e00_00_00.Subfiles.First(pp => pp.Name == "jg2e_00_01_00_00.lst"));
            var sub = Tools.ManipulateLst(PP.jg2e00_00_00.Subfiles[index], 4, "51");
            sub.Name = "jg2e_00_01_00_00.lst";
            PP.jg2e00_00_00.Subfiles[index] = sub;
            back = PP.jg2e00_00_00.WriteArchive(PP.jg2e00_00_00.FilePath, false, "bak", true);
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnDelete31_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete your backup?", "Anti-fuckup dialog", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            Tools.DeleteBackup(Paths.AA2Edit + @"\jg2e01_00_00.pp");
        }
        #endregion
        #region 3.2
        private void btnSet32_Click(object sender, EventArgs e)
        {
            var index = PP.jg2e00_00_00.Subfiles.IndexOf(PP.jg2e00_00_00.Subfiles.First(pp => pp.Name == "jg2e_00_01_00_00.lst"));
            var sub = PP.jg2e00_00_00.Subfiles[index];

            if (chkPose32.Checked)
                sub = Tools.ManipulateLst(sub, 6, numPose32.Value.ToString());
            if (chkEyebrow32.Checked)
                sub = Tools.ManipulateLst(sub, 7, numEyebrow32.Value.ToString());
            if (chkEye32.Checked)
                sub = Tools.ManipulateLst(sub, 8, numEye32.Value.ToString());

            sub = Tools.ManipulateLst(sub, 4, "51");
            sub.Name = "jg2e_00_01_00_00.lst";
            PP.jg2e00_00_00.Subfiles[index] = sub;
            var back = PP.jg2e00_00_00.WriteArchive(PP.jg2e00_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestore32_Click(object sender, EventArgs e)
        {
            var index = PP.jg2e00_00_00.Subfiles.IndexOf(PP.jg2e00_00_00.Subfiles.First(pp => pp.Name == "jg2e_00_01_00_00.lst"));
            var sub = new Subfile(Paths.BACKUP + @"\jg2e_00_01_00_00.lst");
            PP.jg2e00_00_00.Subfiles[index] = sub;
            var back = PP.jg2e00_00_00.WriteArchive(PP.jg2e00_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void chkPose32_CheckedChanged(object sender, EventArgs e)
        {
            numPose32.Enabled = chkPose32.Checked;
        }

        private void chkEyebrow32_CheckedChanged(object sender, EventArgs e)
        {
            numEyebrow32.Enabled = chkEyebrow32.Checked;
        }

        private void chkEye32_CheckedChanged(object sender, EventArgs e)
        {
            numEye32.Enabled = chkEye32.Checked;
        }
        #endregion
        #region 3.3
        private void btnSet33_Click(object sender, EventArgs e)
        {
            if (cmbFirst33.SelectedIndex < 0 ||
                cmbSecond33.SelectedIndex < 0)
                return;

            var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_00_" + cmbSecond33.SelectedIndex.ToString().PadLeft(2, '0') + ".tga"));
            var sub = new Subfile(Paths.Nature + @"\sp_04_00_" + cmbFirst33.SelectedIndex.ToString().PadLeft(2, '0') + ".tga");
            sub.Name = "sp_04_00_" + cmbSecond33.SelectedIndex.ToString().PadLeft(2, '0') + ".tga";
            PP.jg2e06_00_00.Subfiles[index] = sub;
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        private void btnRestoreAll33_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 25; i++)
            {
                var index = PP.jg2e06_00_00.Subfiles.IndexOf(PP.jg2e06_00_00.Subfiles.First(pp => pp.Name == "sp_04_00_" + i.ToString().PadLeft(2, '0') + ".tga"));
                var sub = new Subfile(Paths.Nature + @"\sp_04_00_" + i.ToString().PadLeft(2, '0') + ".tga");
                PP.jg2e06_00_00.Subfiles[index] = sub;
            }
            var back = PP.jg2e06_00_00.WriteArchive(PP.jg2e06_00_00.FilePath, false, "bak", true);
            ShowLoadingForm();
            back.RunWorkerAsync();
            while (back.IsBusy)
            {
                Application.DoEvents();
            }
            HideLoadingForm();
            MessageBox.Show("Finished!");
        }

        #endregion
        #endregion
        #region Card Face
        //I hope the AA2CardEditor anon doesn't mind that I borrowed his code
        private AA2Card card;
        private string cardpath;
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenCardFile();
            UpdateWindowState();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveCardFile();
            UpdateWindowState();
        }

        private void saveAsToolStripButton_Click(object sender, EventArgs e)
        {
            SaveAsCardFile();
            UpdateWindowState();
        }

        private void replaceCardFaceToolStripButton_Click(object sender, EventArgs e)
        {
            ReplaceCardFace();
            UpdateWindowState();
        }

        private void OpenCardFile()
        {
            using (var file = new OpenFileDialog())
            {
                file.Filter = "AA2 Card files (*.png)|*.png";
                file.Multiselect = false;
                if (file.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        card = new AA2Card(File.ReadAllBytes(file.FileName));
                        cardpath = file.FileName;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("The card file could not be opened.");
                    }
                }
            }
            UpdateWindowState();
        }

        private void SaveCardFile()
        {
            if (cardpath != null && card != null)
                File.WriteAllBytes(cardpath, card.raw);
            UpdateWindowState();
        }

        private void SaveAsCardFile()
        {
            if (cardpath != null && card != null)
                using (var file = new SaveFileDialog())
                {
                    file.Filter = "AA2 Card files (*.png)|*.png";
                    if (file.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(file.FileName, card.raw);
                        cardpath = file.FileName;
                    }
                }
            UpdateWindowState();
        }

        private void ReplaceCardFace()
        {
            if (cardpath != null && card != null)
                using (var file = new OpenFileDialog())
                {
                    file.Filter = "PNG (*.png)|*.png";
                    file.Multiselect = false;
                    if (file.ShowDialog() == DialogResult.OK)
                    {
                        var tempcard = new AA2Card(File.ReadAllBytes(file.FileName));
                        card.Image = tempcard.Image;
                    }
                }
            UpdateWindowState();
        }

        private void UpdateWindowState()
        {
            if (File.Exists(cardpath))
            {
                saveToolStripButton.Enabled = true;
                saveAsToolStripButton.Enabled = true;
                replaceCardFaceToolStripButton.Enabled = true;
                replaceCardRosterToolStripButton.Enabled = true;
                replaceCardRosterFromCardToolStripButton.Enabled = true;
            }
            else
            {
                saveToolStripButton.Enabled = false;
                saveAsToolStripButton.Enabled = false;
                replaceCardFaceToolStripButton.Enabled = false;
                replaceCardRosterToolStripButton.Enabled = false;
                replaceCardRosterFromCardToolStripButton.Enabled = false;
            }
            Size size;
            if (card != null)
            {
                imgRoster.Image = card.RosterImage;
                imgCard.Image = card.Image;
                size = card.Image.Size;
            }
            else
                size = new Size(0, 0);

            lblDimensions.Text = "[" + size.Width + ", " + size.Height + "]";
        }

        private void replaceCardRosterToolStripButton_Click(object sender, EventArgs e)
        {
            if (cardpath != null && card != null)
                using (var file = new OpenFileDialog())
                {
                    file.Filter = "PNG (*.png)|*.png";
                    file.Multiselect = false;
                    if (file.ShowDialog() == DialogResult.OK)
                    {
                        card.RosterImage = Image.FromFile(file.FileName);
                    }
                }
            UpdateWindowState();
        }

        private void replaceCardRosterFromCardToolStripButton_Click(object sender, EventArgs e)
        {
            if (cardpath != null && card != null)
                using (var file = new OpenFileDialog())
                {
                    file.Filter = "PNG (*.png)|*.png";
                    file.Multiselect = false;
                    if (file.ShowDialog() == DialogResult.OK)
                    {
                        var tempcard = new AA2Card(File.ReadAllBytes(file.FileName));
                        card.RosterImage = tempcard.RosterImage;
                    }
                }
            UpdateWindowState();
        }
        #endregion
    }
}