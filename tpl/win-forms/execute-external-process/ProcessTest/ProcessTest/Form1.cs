using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProcessTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //TODO: configurable?
            const string ToolFileName = "c:\\temp\\Some.BAT";

            //from here: https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results
            //string output = RunExternalExe(ToolFileName);
            //MessageBox.Show(output);

            Task.Factory.StartNew(() =>
            {
                RunExternalProcess(ToolFileName);
            });
        }

        public void RunExternalProcess(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            //process.OutputDataReceived += (sender, args) 
            //    => textBox1.Invoke(new Action(() => textBox1.Text += args.Data + Environment.NewLine)); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.
            process.OutputDataReceived += (sender, args)
                => AppendText(args.Data + Environment.NewLine);

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                //textBox1.Invoke(new Action(() => textBox1.Text = stdOutput.ToString() + Environment.NewLine));
                AppendText(stdOutput.ToString() + Environment.NewLine);
            }
            else
            {

                if (!string.IsNullOrEmpty(stdError))
                {
                    //textBox1.Invoke(new Action(() => textBox1.Text += stdError + Environment.NewLine));
                    AppendText(stdError.ToString() + Environment.NewLine);
                }

                if (stdOutput.Length != 0)
                {
                    //textBox1.Invoke(new Action(() => textBox1.Text += stdOutput.ToString() + Environment.NewLine));
                    AppendText(stdOutput.ToString() + Environment.NewLine);
                }

                //throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        public string RunExternalExe(string filename, string arguments = null)
        {
            var process = new Process();

            process.StartInfo.FileName = filename;
            if (!string.IsNullOrEmpty(arguments))
            {
                process.StartInfo.Arguments = arguments;
            }

            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.UseShellExecute = false;

            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;
            var stdOutput = new StringBuilder();
            process.OutputDataReceived += (sender, args) => stdOutput.AppendLine(args.Data); // Use AppendLine rather than Append since args.Data is one line of output, not including the newline character.

            string stdError = null;
            try
            {
                process.Start();
                process.BeginOutputReadLine();
                stdError = process.StandardError.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception e)
            {
                throw new Exception("OS error while executing " + Format(filename, arguments) + ": " + e.Message, e);
            }

            if (process.ExitCode == 0)
            {
                return stdOutput.ToString();
            }
            else
            {
                var message = new StringBuilder();

                if (!string.IsNullOrEmpty(stdError))
                {
                    message.AppendLine(stdError);
                }

                if (stdOutput.Length != 0)
                {
                    message.AppendLine("Std output:");
                    message.AppendLine(stdOutput.ToString());
                }

                throw new Exception(Format(filename, arguments) + " finished with exit code = " + process.ExitCode + ": " + message);
            }
        }

        private string Format(string filename, string arguments)
        {
            return "'" + filename +
                   ((string.IsNullOrEmpty(arguments)) ? string.Empty : " " + arguments) +
                   "'";
        }

        void AppendText(string text)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((Action)delegate { this.AppendText(text); });
                return;
            }

            textBox1.AppendText(text);
        }
    }
}
