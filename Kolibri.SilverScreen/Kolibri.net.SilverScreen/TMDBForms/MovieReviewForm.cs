using Kolibri.net.Common.Utilities.Controller;
using Kolibri.net.SilverScreen.Controls;
using Plex.ServerApi.PlexModels.Account.Resources;
using System;
using System;
using System.Collections.Generic;
using System.Net.ServerSentEvents;
using System.Text;
using System.Windows.Forms;
using TMDbLib.Objects.Reviews;
using TMDbLib.Objects.Reviews;
namespace Kolibri.net.SilverScreen.TMDBForms
{ 

    public partial class MovieReviewForm : Form
    {
     
    
        public MovieReviewForm(ReviewBase review )  
            
        {
            InitializeComponent();
            // Sett dette vinduet til å være et underprosess-vindu (child) av hovedvinduet

            var mdiParent = ResourceController.GetMdiParent();
            if (mdiParent != null) this.MdiParent = mdiParent;

            DisplayReview(review);
        }
        private async void DisplayReview(ReviewBase review)
        {
            if (review == null) return;

            // Forfatter og tekst
            txtAuthor.Text = review.Author ?? "Ukjent";
            txtContent.Text = review.Content ?? string.Empty;
            lnkUrl.Text = review.Url ?? string.Empty;
            // WebView2-håndtering
            if (!string.IsNullOrEmpty(review.Url))
            {
                try
                {
                    await webView21.EnsureCoreWebView2Async(null);

                    // NYTT: Koble til funksjonen som fjerner cookies når siden er lastet ferdig
                    webView21.NavigationCompleted += WebView21_NavigationCompleted;

                    webView21.CoreWebView2.Navigate(review.Url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kunne ikke laste nettsiden: {ex.Message}");
                }
            }

            // Opprettelsesdato (Formateres til lokalt visningsformat)

            txtDate.Text = review.CreatedAt.ToLocalTime().ToString("g");

            // Henter ut rating fra AuthorDetails hvis det eksisterer
            if (review.AuthorDetails?.Rating != null)
            {
                txtRating.Text = $"{review.AuthorDetails.Rating} / 10";
            }
            else
            {
                txtRating.Text = "Ikke gitt";
            }
        }

        private void LnkUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!string.IsNullOrEmpty(lnkUrl.Text))
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = lnkUrl.Text,
                    UseShellExecute = true
                });
            }
        }


        private async void WebView21_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Sjekk at navigasjonen var vellykket
            if (!e.IsSuccess) return;

            // JavaScript som leter etter vanlige avvis-knapper og klikker på dem
            string jsCode = @"
        (function() {
            // Liste over vanlige ord som brukes på avvis-knapper
            const declineWords = ['reject', 'decline', 'avvis', 'nei', 'disagree', 'only essential', 'kun nødvendige'];
            
            // Finn alle knapper på websiden
            const buttons = document.querySelectorAll('button, a, div[role=""button""]');
            
            for (let btn of buttons) {
                const text = btn.innerText.toLowerCase().trim();
                
                // Hvis knappen inneholder et av ordene fra listen, klikk på den
                if (declineWords.some(word => text.includes(word))) {
                    btn.click();
                    console.log('Klikket automatisk på cookie-avvisning: ' + text);
                    break; 
                }
            }
        })();
    ";

            try
            {
                // Kjør skriptet i nettleseren
                await webView21.ExecuteScriptAsync(jsCode);
            }
            catch (Exception)
            {
                // Ignorer feil hvis skriptet feiler på enkelte sider
            }
        }

    }
}
