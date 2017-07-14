using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchAccountCreator
{
    public partial class Form1 : Form
    {
        struct AccountInfo
        {
            public AccountInfo(string login, string pass, string mail)
            {
                this.Login = login;
                this.Password = pass;
                this.Email = mail;
            }

            public string Login;
            public string Password;
            public string Email;
        }

        public Form1()
        {
            InitializeComponent();
            this.waitHandle = new System.Threading.AutoResetEvent(false);
            this.Accounts = new List<AccountInfo>();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.button1.Text.Equals("Start") && backgroundWorker1.IsBusy)
            {
                MessageBox.Show("Bitte warten");
            }

            if (this.numericUpDown1.Value > 0 && this.button1.Text.Equals("Start") && !backgroundWorker1.IsBusy)
            {
                this.progressBar1.Value = 0;
                this.progressBar1.Maximum = (int)this.numericUpDown1.Value;
                this.label10.Text = "0/" + this.numericUpDown1.Value.ToString();
                this.button2.Enabled = false;
                this.numericUpDown1.Enabled = false;
                this.button1.Text = "Stop";
                backgroundWorker1.RunWorkerAsync(this.numericUpDown1.Value);
            }
            else if (this.button1.Text.Equals("Stop"))
            {
                this.waitHandle.Set();
                backgroundWorker1.CancelAsync();
            }
        }

        public void KeyPressed(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                this.waitHandle.Set();
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        public bool GetDataFromTwitch(ref string challenge, ref string token, ref Bitmap picture)
        {
            string SignUpPage = "";

            if (AccountCreation.GetSignupPage(ref SignUpPage))
            {
                if (AccountCreation.GetToken(SignUpPage, ref token))
                {
                    if (AccountCreation.GetRecaptchaChallenge(ref challenge))
                    {
                        if (AccountCreation.GetRecaptchaImage(challenge, ref picture))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.button2.Enabled = false;
            this.richTextBox1.Text += "Exporting Accounts... \n";
            WriteAccountDataToFile();
            this.Accounts.Clear();
            this.button2.Enabled = true;
        }

        private void WriteAccountDataToFile()
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("accounts.txt", true))
            {
                foreach (AccountInfo account in this.Accounts)
                {
                    file.WriteLine("Username: " + account.Login);
                    file.WriteLine("Password: " + account.Password);
                    //file.WriteLine("Email: " + account.Email);
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string username;
            string password;
            string email;
            string year;
            string day;
            string month;
            string challenge = "";
            string token = "";
            string response = "";
            Bitmap picture = new Bitmap(1,1);
            int iterations = (int)(decimal)e.Argument;

            for (int i = 0; i < iterations && !backgroundWorker1.CancellationPending; i++)
            {

                username = GenerateRandom.RandomString(12);
                password = GenerateRandom.RandomString(12);
                email = GenerateRandom.RandomStringOnlyChars(12) + "@aol.com";
                year = GenerateRandom.RandomNumber(1950, 1999);
                month = GenerateRandom.RandomNumber(1, 12);
                day = GenerateRandom.RandomNumber(1, 27);

                backgroundWorker1.ReportProgress(100, new string[] {username, password, email, year, month, day });

                if (GetDataFromTwitch(ref challenge, ref token, ref picture))
                {
                    backgroundWorker1.ReportProgress(100, picture);
                    this.waitHandle.WaitOne();

                    if (AccountCreation.SendFormData(this.textBox2.Text, this.textBox3.Text, this.textBox4.Text, this.textBox5.Text,
                                                this.textBox6.Text, this.textBox7.Text, token, challenge, this.textBox1.Text, ref response))
                    {
                        backgroundWorker1.ReportProgress(100, "Account No: " + i + " created \n");
                        backgroundWorker1.ReportProgress(100, "progressBar");
                        backgroundWorker1.ReportProgress(i, "label");
                        backgroundWorker1.ReportProgress(100, "captcha_sent");
                        this.Accounts.Add(new AccountInfo(this.textBox2.Text, this.textBox3.Text, this.textBox4.Text));
                    }
                    else
                    {
                        backgroundWorker1.ReportProgress(100, "Account No: " + i + " creation failed \n");
                        backgroundWorker1.ReportProgress(100, "captcha_sent");
                        i--;
                    }
                }
                else
                {
                    backgroundWorker1.ReportProgress(100, "Account No: " + i + " creation failed \n");
                    backgroundWorker1.ReportProgress(100, "captcha_sent");
                    i--;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState.GetType().ToString().Equals("System.Drawing.Bitmap"))
            {
                this.pictureBox1.Image = (Bitmap)e.UserState;
            }
            else if (e.UserState.GetType().ToString().Equals("System.String"))
            {
                if (e.UserState.Equals("progressBar"))
                {
                    this.progressBar1.Value++;
                }
                else if (e.UserState.Equals("label"))
                {
                    this.label10.Text = (e.ProgressPercentage + 1) + "/" + this.numericUpDown1.Value.ToString();
                }
                else if (e.UserState.Equals("captcha_sent"))
                {
                    this.textBox1.Text = "";
                }
                else
                {
                    this.richTextBox1.Text += e.UserState;
                }
            }
            else if (e.UserState.GetType().ToString().Equals("System.String[]"))
            {
                this.textBox2.Text = ((string[])e.UserState)[0];
                this.textBox3.Text = ((string[])e.UserState)[1];
                this.textBox4.Text = ((string[])e.UserState)[2];
                this.textBox5.Text = ((string[])e.UserState)[3];
                this.textBox6.Text = ((string[])e.UserState)[4];
                this.textBox7.Text = ((string[])e.UserState)[5];
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.button1.Text = "Start";

            
            if (this.Accounts.Count > 0)
            {
                this.button2.Enabled = true;
            }

            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox5.Text = "";
            this.textBox6.Text = "";
            this.textBox7.Text = "";
            this.pictureBox1.Image = null;

            this.numericUpDown1.Enabled = true;
        }
    }
}
