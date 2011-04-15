using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InWFExample
{
    public partial class Authorize : Form
    {
        public delegate void AuthorizeCompletedDelegate(string oauth_token, string oauth_verifier);
        public event AuthorizeCompletedDelegate AuthorizeCompleted;
        private Timer tmrCloseCheck = new Timer();
        public Authorize()
        {
            InitializeComponent();
        }

        private void Verifier_Load(object sender, EventArgs e)
        {

        }

        private void ShowOfflienContent(string offlineUrl)
        {
            String sitePath = null;
            try
            {
                sitePath = Application.StartupPath + @"\OfflineDocs\" + offlineUrl;
                webBrowser1.Navigate(sitePath);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.ToString() + "\nSite Path: " + sitePath);
            }
        }

        public void Navigate(string url)
        {
            ShowOfflienContent("Navigating.htm");
            webBrowser1.Navigate(url);
        }

        private string oauth_token = null;
        private string oauth_verifier = null;

        private void webBrowser1_Navigated(object sender, WebBrowserNavigatedEventArgs e)
        {
            string responsed = e.Url.ToString();
            if (responsed.StartsWith("http://localhost/inverify"))
            {
                responsed = responsed.Replace("http://localhost/inverify?", "");
                string[] args = responsed.Split('&');
                foreach(string arg in args)
                {
                    string[] values  = arg.Split('=');
                    if(values[0] == "oauth_token")
                        oauth_token = values[1];

                    if(values[0] == "oauth_verifier")
                        oauth_verifier = values[1];
                }
            }

            if (oauth_token != null && oauth_verifier != null)
            {
                if (AuthorizeCompleted != null)
                {
                    AuthorizeCompleted(oauth_token, oauth_verifier);
                    oauth_token = null;
                    oauth_verifier = null;
                }
                tmrCloseCheck.Interval = 2000;
                tmrCloseCheck.Tick += new EventHandler(tmrCloseCheck_Tick);
                tmrCloseCheck.Start();
                ShowOfflienContent("Completed.htm");            
            }
        }

        void tmrCloseCheck_Tick(object sender, EventArgs e)
        {
            tmrCloseCheck.Stop();
            tmrCloseCheck.Dispose();
            this.Close();
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("webBrowser1_Navigating : " + e.Url);
        }

        private void webBrowser1_LocationChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("webBrowser1_LocationChanged");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("webBrowser1_DocumentCompleted" + e.Url);
        }
    }
}
