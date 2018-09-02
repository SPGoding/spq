using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;

namespace spn
{
    public partial class Form1 : Form
    {
        string oldQues = "";
        string curQues = "";
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
            var cur = getWebCode("http://www.mcbbs.net/forum.php?mod=forumdisplay&fid=110&filter=typeid&typeid=2482", "UTF-8");
            var regex = new Regex(
                "class=\"s xst\">(.*?)</a>");
            var ans = regex.Match(cur).Value.Replace("class=\"s xst\">", "").Replace("</a>", "");
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
            for (int i = 0; i < 2; i++)
            {
                var speechSynthesizer = new SpeechSynthesizer();
                speechSynthesizer.Speak($"出现新问题：{curQues}。");
                Thread.Sleep(400);
            }
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
            speechSynthesizer.SpeakAsync($"当前最新问题：{oldQues}。");

            textBox1.Text = "";
            button1.Enabled = false;
            cnt = 1;
            timer1.Enabled = true;
        }
    }
}
