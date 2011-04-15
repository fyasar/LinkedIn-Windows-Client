using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace InWFExample
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private oAuthLinkedIn _oauth = new oAuthLinkedIn();
        private string _authLink = null;

        private void btnCreateAuthorization_Click(object sender, EventArgs e)
        {
            _authLink = _oauth.AuthorizationLinkGet();
            txtRequestToken.Text = _oauth.Token;
            txtRequestTokenSecret.Text = _oauth.TokenSecret;
            
            btnAuthorize.Enabled = true;
        }


        private void btnAuthorize_Click(object sender, EventArgs e)
        {
            Authorize authorizeForm = new Authorize();
            authorizeForm.AuthorizeCompleted += new Authorize.AuthorizeCompletedDelegate(authorizeForm_AuthorizeCompleted);
            authorizeForm.Show();
            authorizeForm.Navigate(_authLink);
        }

        void authorizeForm_AuthorizeCompleted(string oauth_token, string oauth_verifier)
        {
            txtOauth_token.Text = oauth_token;
            txtOauth_verify.Text = oauth_verifier;
            btnGetAcToken.Enabled = true;
        }

        private void btnGetAcToken_Click(object sender, EventArgs e)
        {
            _oauth.Token = txtOauth_token.Text;
            _oauth.TokenSecret = txtRequestTokenSecret.Text;
            _oauth.Verifier = txtOauth_verify.Text;

            _oauth.AccessTokenGet(txtOauth_token.Text);

            txtOauth_Access_token.Text = _oauth.Token;
            txtOauth_Access_tokenSecret.Text = _oauth.TokenSecret;
            btnUpdateStatus.Enabled = true;
            btnGetProfileDetail.Enabled = true;
        }

        private void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            txtAPIResponse.Text = "";
            _oauth.Token = txtOauth_Access_token.Text;
            _oauth.TokenSecret = txtOauth_Access_tokenSecret.Text;
            _oauth.Verifier = txtOauth_verify.Text;

            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            xml += "<current-status>" + txtNewStatus.Text + "</current-status>";

            string response = _oauth.APIWebRequest("PUT", "http://api.linkedin.com/v1/people/~/current-status", xml);
            if (response == "")
                txtAPIResponse.Text = "Your new status updated";
        }

        private void btnGetProfileDetail_Click(object sender, EventArgs e)
        {
            txtAPIResponse.Text = "";
            _oauth.Token = txtOauth_Access_token.Text;
            _oauth.TokenSecret = txtOauth_Access_tokenSecret.Text;
            _oauth.Verifier = txtOauth_verify.Text;

            string response = _oauth.APIWebRequest("GET", "https://api.linkedin.com/v1/people/~", null);
            txtAPIResponse.Text = response;
        }

    }
}
