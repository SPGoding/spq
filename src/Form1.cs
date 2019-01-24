using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace spn
{
    public partial class Form1 : Form
    {
        string oldQues = "";
        string curQues = "";
        string tid = "";
        int cnt = 1;

        public Form1()
        {
            InitializeComponent();
        }

        private void main()
        {
            curQues = getQuestion();

            textBox1.Text += $"第 {cnt++.ToString().PadLeft(4, '0')} 次：{curQues}\r\n";
            textBox1.SelectionStart = textBox1.TextLength;
            textBox1.ScrollToCaret();
            if (curQues != oldQues)
            {
                changed();
            }
        }

        private string getQuestion()
        {
            var cur = getWebCode("http://www.mcbbs.net/forum-qanda-1.html", "UTF-8");
            var tidRegex = new Regex("<tbody id=\"normalthread_\\d+\">");
            var tid = tidRegex.Match(cur).Value.Replace("<tbody id=\"normalthread_", "").Replace("\">", "");
            this.tid = tid;
            var regex = new Regex("class=\"s xst\">(.*?)</a>");
            var ans = regex.Match(cur.Substring(cur.IndexOf(tid))).Value.Replace("class=\"s xst\">", "").Replace("</a>", "");
            var speechSynthesizer = new SpeechSynthesizer();
            if (ans == "")
            {
                speechSynthesizer.Speak("MCBBS 崩溃了。");
                return curQues;
            }
            else
            {
                return ans;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            main();
        }

        private void changed()
        {
            var speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.Speak($"出现新问题：{curQues}。");
            timer1.Enabled = false;
            button1.Enabled = true;
        }

        private void error()
        {
            for (int i = 0; i < 1; i++)
            {
                Console.Beep(3000, 150);
                Thread.Sleep(850);
            }
        }

        private string getWebCode(string url, string encoder)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                var response = (HttpWebResponse)request.GetResponse();
                var receiveStream = response.GetResponseStream();
                var readStream = new StreamReader(receiveStream, Encoding.GetEncoding(encoder));
                string SourceCode = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                return SourceCode;
            }
            catch
            {
                error();
                return "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oldQues = getQuestion();
            var speechSynthesizer = new SpeechSynthesizer();
            speechSynthesizer.Speak($"当前最新问题：{oldQues}。");

            textBox1.Text = "";
            button1.Enabled = false;
            cnt = 1;
            timer1.Enabled = true;
        }

        private void textBox1_DoubleClick(object sender, EventArgs e)
        {
            var url = $"http://www.mcbbs.net/thread-{tid}-1-1.html";
            Process.Start(url);
        }
    }
}
