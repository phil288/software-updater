using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Net;
using System.IO;

namespace SoftwareUpdater
{
    public partial class Form1 : Form
    {
        string targetDirectory = "";
        string sourceBaseUrl = "";
        public Form1()
        {
            InitializeComponent();
            string destinationDirectoryConfig = Path.Combine(Application.StartupPath, "destination directory.txt");
            if (!File.Exists(destinationDirectoryConfig))
            {
                MessageBox.Show("You cannot proceed, the destination directory configuration is missing");
                this.Close();
                return;
            }
            string remoteUrlConfig = Path.Combine(Application.StartupPath, "remote url.txt");
            if (!File.Exists(destinationDirectoryConfig))
            {
                MessageBox.Show("You cannot proceed, the remote url configuration is missing");
                this.Close();
                return;
            }
            this.targetDirectory = File.ReadAllText(destinationDirectoryConfig);
            if (!Directory.Exists(targetDirectory))
            {
                MessageBox.Show("The target directory does not exist, we cannot proceed");
                this.Close();
                return;
            }
            this.sourceBaseUrl = File.ReadAllText(remoteUrlConfig);
            if (this.sourceBaseUrl == "" || !this.sourceBaseUrl.StartsWith("http"))
            {
                MessageBox.Show("The remote URL is not valid, we cannot proceed");
                this.Close();
                return;
            }
            this.Log("To start the update click on the button 'Start Update'");
            //this.GetAllFiles();

        }
        private void StartDownload()
        {
            //get the data
            new Thread(() =>
            {
                string[] files = null;
                try
                {
                    this.Log("Getting all files from server");
                    files = this.GetAllFiles();
                    if (files == null)
                    {
                        MessageBox.Show("We could not find any files on the server, aborting");
                        return;
                    }
                    if (files.Length == 0)
                    {
                        MessageBox.Show("We could not find any files on the server, aborting");
                        return;
                    }
                    this.Log("Got " + files.Length + " files. We're going to download them. The files are: " + String.Join(", ", files));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred while trying to get the files that we should update. Please check your internet connection, if this does not work, contact you programmer with the following details:\r\n" + ex.Message, "Error");
                    this.Log("Please close the software and retry once the issue has been fixed");
                    return;
                }
                foreach (string f in files)
                {
                    try
                    {
                        this.Log("Downloading " + f);
                        string url = sourceBaseUrl + f;
                        string destinationFilename = this.targetDirectory + "\\" + f;
                        this.DownloadFile(url, destinationFilename);
                        this.Log("File " + f + " has been downloaded");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while trying to download " + f + ". Please check your internet connection, if this does not work, contact you programmer with the following details:\r\n" + ex.Message, "Error");
                    }
                }
                this.Log("Download completed, you can close the software");
            }).Start();
        }
        private string[] GetAllFiles()
        {
            CookieContainer CC = new CookieContainer();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(this.sourceBaseUrl + "/files.php");
            request.Headers.Add("Accept-Language: en-US,en;q=0.9");
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.85 Safari/537.36";
            request.ContentType = "text/plain;charset=\"utf-8\"";
            request.Accept = "text/plain";
            request.KeepAlive = false;
            request.Proxy = null;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //request.Connection = "keep-alive";
            request.ProtocolVersion = HttpVersion.Version10;
            request.ServicePoint.ConnectionLimit = 1;
            request.UseDefaultCredentials = true;
            request.CookieContainer = CC;
            request.Timeout = -1;
            request.PreAuthenticate = true;
            request.Credentials = CredentialCache.DefaultCredentials;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                readStream = new StreamReader(receiveStream);

                string data = readStream.ReadToEnd();

                receiveStream.Close();
                response.Close();
                readStream.Close();

                string[] tmp = data.Split(new string[] { "%123%" }, StringSplitOptions.None);
                return tmp;
            }
            return null;
        }
        private void DownloadFile(string remoteUri, string destinationFilename)
        {
            // Create a new WebClient instance.
            using (WebClient myWebClient = new WebClient())
            {
                // Download the Web resource and save it into the current filesystem folder.
                if (File.Exists(destinationFilename)) File.Delete(destinationFilename);
                myWebClient.DownloadFile(remoteUri, destinationFilename);
            }
        }
        private void Log(string text)
        {
            string separator = "\r\n";
            if (this.txtLog.Text == "")
            {
                separator = "";
            }
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    this.txtLog.Text += separator + text;
                }));
            }
            else
            {
                this.txtLog.Text += separator + text;
            }
        }

        private void btnStartUpdate_Click(object sender, EventArgs e)
        {
            this.StartDownload();
        }
    }
}