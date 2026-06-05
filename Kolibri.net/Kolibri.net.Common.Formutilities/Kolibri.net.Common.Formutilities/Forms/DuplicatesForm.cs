using Kolibri.net.Common.Utilities.Controller;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Kolibri.net.Common.Utilities;


namespace Kolibri.net.Common.Formutilities
{
    public partial class DuplicatesForm : Form
    {
        private ImageDuplicateFinder _scanner = new();
        private List<DuplicatePair> _foundDuplicates = new();
        private bool _ask = true;

        public DuplicatesForm(List<DuplicatePair> duplicatePair)
        {
            InitializeComponent();
            _foundDuplicates = duplicatePair;

            // Sørg for at bildene skalerer pent i boksene
            picImageA.SizeMode = PictureBoxSizeMode.Zoom;
            picImageB.SizeMode = PictureBoxSizeMode.Zoom;

            // Lytt til når brukeren endrer valg i listen
            lstDuplicates.SelectedIndexChanged += LstDuplicates_SelectedIndexChanged;

            Init(_foundDuplicates);

        }

        private void Init(List<DuplicatePair> foundDuplicates)
        {
            try
            {


                if (_foundDuplicates.Count == 0)
                {

                    return;
                }

                foreach (var pair in _foundDuplicates)
                {
                    lstDuplicates.Items.Add($"{pair.FileB.Name} -> Matcher: {pair.FileA.Name}");
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Skanning feilet: {ex.Message}", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {

            }
        }


        private void LstDuplicates_SelectedIndexChanged(object? sender, EventArgs e)
        {
            int selectedIndex = lstDuplicates.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < _foundDuplicates.Count)
            {
                var selectedPair = _foundDuplicates[selectedIndex];
                DisplayPreviews(selectedPair);
            }
            else
            {
                ClearPreviews();
            }
        }

        private void DisplayPreviews(DuplicatePair pair)
        {
            // Tøm gamle bilder fra minnet først så vi ikke låser filene
            ClearPreviews();

            try
            {
                // Bruk Image.FromStream i stedet for Image.FromFile. 
                // FromFile låser filen på disken, noe som hindrer sletting.
                if (File.Exists(pair.FileA.FullName))
                {

                    using var streamA = new FileStream(pair.FileA.FullName, FileMode.Open, FileAccess.Read);
                    picImageA.Image = Image.FromStream(streamA);
                    picImageA.Tag = pair.FileA.FullName;
                    toolTip1.SetToolTip(picImageA, picImageA.Tag.ToString());
                }

                if (File.Exists(pair.FileB.FullName))
                {
                    using var streamB = new FileStream(pair.FileB.FullName, FileMode.Open, FileAccess.Read);
                    picImageB.Image = Image.FromStream(streamB);
                    picImageB.Tag = pair.FileB.FullName;

                    toolTip1.SetToolTip(picImageB, picImageB.Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kunne ikke laste forhåndsvisning: {ex.Message}");
            }
        }

        private void ClearPreviews()
        {
            // Viktig i WinForms: Kast gamle Image-objekter for å frigjøre RAM og fil-låser
            if (picImageA.Image != null)
            {
                picImageA.Image.Dispose();
                picImageA.Image = null;
            }
            if (picImageB.Image != null)
            {
                picImageB.Image.Dispose();
                picImageB.Image = null;
            }
        }

        private void btnDeleteSelected_Click(object sender, EventArgs e)
        {
            try
            {
                int selectedIndex = lstDuplicates.SelectedIndex;
                if (selectedIndex >= 0 && selectedIndex < _foundDuplicates.Count)
                {
                    var selectedPair = _foundDuplicates[selectedIndex];

                    var filename = selectedPair.FileB.FullName;
                    if (sender.Equals(btnDeleteASelected))
                        filename = selectedPair.FileA.FullName;

                    if (!File.Exists(filename))
                    {

                        throw new FileNotFoundException(filename);
                    }
                    var confirmResult = DialogResult.Yes;
                    if (!selectedPair.SameDir)
                    {

                          confirmResult = MessageBox.Show(
                            $"Er du sikker på at du vil slette dette duplikatet permanent?\n\nVil slette: {filename}",
                            "Bekreft sletting",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);
                    }
                    
                    if (confirmResult == DialogResult.Yes)
                    {
                        // Tøm bildevisningen FØR sletting, ellers holder PictureBox filen låst
                        ClearPreviews();

                        var success = false;
                        if (sender.Equals(btnDeleteASelected))
                        { success = selectedPair.DeleteFileA(); }
                        else
                        {

                            success = selectedPair.DeleteFileB();
                        }

                        if (success)
                        {
                            //   MessageBox.Show("Filen ble slettet.", "Suksess", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            lstDuplicates.Items.RemoveAt(selectedIndex);
                            _foundDuplicates.RemoveAt(selectedIndex);
                        }
                        else
                        {
                            MessageBox.Show("Kunne ikke slette filen. Den kan være låst av et annet program.", "Feil", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            // Gjenopprett visningen hvis sletting feilet
                            LstDuplicates_SelectedIndexChanged(this, EventArgs.Empty);
                        }
                        FileUtilities.DeleteEmptyDirs(new FileInfo(filename).Directory);
                    }
                }
                else
                {
                    MessageBox.Show("Vennligst velg et duplikat fra listen først.", "Ingen valg gjort", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().Name);
            }
        }

        private void picImage_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender.Equals(picImageA) || sender.Equals(picImageB))
                {
                    FileInfo info = new FileInfo(((sender as PictureBox).Tag).ToString());
                    FileUtilities.OpenFolderHighlightFile(info);
                }
            }
        }
    }
}