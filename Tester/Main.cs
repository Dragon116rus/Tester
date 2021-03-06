﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Telerik.WinControls;

namespace Tester {
    public partial class Main : Telerik.WinControls.UI.RadForm {
        internal string compiler = "g++";
        public Main() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            Program1 form1 = new Program1();
            form1.Show(this);
        }

        private void tests_Click(object sender, EventArgs e) {
            Tests form1 = new Tests();
            form1.Show(this);
        }

        private void testing_Click(object sender, EventArgs e) {
            Thread thread = new Thread(Testing);
            thread.Start();
            // Testing();
        }
        void Testing() {
            Process cmd = new Process();
            cmd.StartInfo = new ProcessStartInfo(@"cmd.exe");
            cmd.StartInfo.RedirectStandardInput = true;// перенаправить вход
            cmd.StartInfo.RedirectStandardOutput = true;//перенаправить выход
            cmd.StartInfo.UseShellExecute = false;//обязательный параметр, для работы предыдущих
            cmd.StartInfo.CreateNoWindow = true;
            cmd.Start();

            cmd.StandardInput.WriteLine(compiler + @" program.cpp");
            cmd.StandardInput.WriteLine("exit");
            cmd.Close();
            int i = 1;


            Process program = new Process();
            while ((File.Exists(string.Format("tests/test{0}", i))) && (File.Exists(string.Format("tests/test{0}a", i)))) {



                program.StartInfo = new ProcessStartInfo(@"a.exe");

                program.StartInfo.RedirectStandardInput = true;// перенаправить вход
                program.StartInfo.RedirectStandardOutput = true;//перенаправить выход
                program.StartInfo.UseShellExecute = false;//обязательный параметр, для работы предыдущих
                program.StartInfo.CreateNoWindow = true;
                program.Start();
                //  program.StandardInput.WriteLine(String.Format("a.exe < tests/test{0} > testAnswer", i));
                using (StreamReader srTest = new StreamReader(String.Format("tests/test{0}", i))) {
                    program.StandardInput.WriteLine(srTest.ReadToEnd());
                }

                StreamReader sr = program.StandardOutput;
                
                
               
                Invoke((MethodInvoker)delegate { testResultList.Items.Add(string.Format("{0}:{1}", i, Checker(string.Format("tests/test{0}a", i), sr))); });
                ;

                program.Close();
                i++;
            }
            cmd.Close();
        }
        string Checker(string path, StreamReader sr) {

            StreamReader srAnswer = new StreamReader(path);

            string[] answer = srAnswer.ReadToEnd().Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            string[] trueAnswer = sr.ReadToEnd().Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (trueAnswer.Length < answer.Length) {
                return "Error:a different number of characters";
            }
            for (int i = 0; i < trueAnswer.Length; i++) {
                if (trueAnswer[i] != answer[i])
                    return "Error:different value";
            }
            sr.Close();
            srAnswer.Close();
            return "true";
        }
 
    }
}
