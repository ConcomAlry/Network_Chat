using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace My_Client
{
    public partial class Form1 : Form
    {
        private IPAddress ip = null;
        private int port = 0;
        private Thread th;
        private NetworkStream stream;

        public Form1()
        {
            InitializeComponent();

            richTextBox1.Enabled = false;
            richTextBox2.Enabled = false;
            button2.Enabled = false;

            try
            {
                var sr = new StreamReader(@"Client_info/data_info.txt");
                string buffer = sr.ReadToEnd();
                sr.Close();
                string[] connect_info = buffer.Split(':');
                ip = IPAddress.Parse(connect_info[0]);
                string ips = ip.ToString();
                port = int.Parse(connect_info[1]);
                TcpClient client = new TcpClient(ips, port);
                stream = client.GetStream();

                label4.ForeColor = Color.Green;
                label4.Text = "Настройки: \nIP сервера: " + connect_info[0] + "\nПорт сервера: " + connect_info[1];
            }

            catch (Exception)
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Настройки не найдены!";
                Form2 form = new Form2();
                form.Show();
            }
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }

        void SendMessage(string message)
        {
            if (message != " " && message != "")
            {
                _ = new byte[1024];
                byte[] buffer = Encoding.Unicode.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
            }
        }
        
        void RecvMessage()
        {
            while(true)
            {
                byte[] readingData = new byte[256];
                StringBuilder completeMessage = new StringBuilder();
                do
                {
                    int numberOfBytesRead = stream.Read(readingData, 0, readingData.Length);
                    completeMessage.AppendFormat("{0}", Encoding.Unicode.GetString(readingData, 0, numberOfBytesRead));
                }
                while (stream.DataAvailable);
                string responseData = completeMessage.ToString();
                this.Invoke((MethodInvoker)delegate ()
                {
                    richTextBox1.AppendText(responseData);
                });
            }            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != " " && textBox1.Text != "")
            {
                button2.Enabled = true;
                richTextBox2.Enabled = true;
                richTextBox1.Enabled = true;
                richTextBox1.ReadOnly = true;
                if (ip != null)
                {
                    th = new Thread(delegate () { RecvMessage(); });
                    th.Start();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendMessage(textBox1.Text + ": " + richTextBox2.Text + "\n");
            richTextBox2.Clear();
        }

        private void авторToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Ты далбаёб? Нахуй тебе эта инфа?");
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (th != null) th.Abort();
            Application.Exit();
        }
    }
}
