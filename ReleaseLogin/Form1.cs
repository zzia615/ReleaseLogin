using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReleaseLogin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dr = folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
                textBox1.Text = folderBrowserDialog1.SelectedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("请选择安装目录", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string path = Path.Combine(textBox1.Text, "热血江湖资料片之玄冰江迷宫");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Form form = new Form2(path);
            this.Visible = false;
            form.FormClosed += Form_FormClosed;
            form.ShowDialog();
        }

        private void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
        }

        void CheckGameDir(string newDir)
        {
            string checkedFileName = "help_V1.1.chm";
            //盘符
            string[] logDirs = Environment.GetLogicalDrives();
            string sysDir = Environment.GetFolderPath(Environment.SpecialFolder.System);
            ArrayList al = new ArrayList();
            foreach(string dir in logDirs)
            {
                try
                {
                    string[] tmp_dirs = Directory.GetDirectories(dir, checkedFileName, SearchOption.AllDirectories);
                    if (tmp_dirs != null && tmp_dirs.Length > 0)
                        al.AddRange(tmp_dirs);
                }
                catch (Exception)
                {
                    
                }
            }
            if (al.Count <= 0) return;

            string[] dirs = new string[al.Count];
            for(int i = 0; i < al.Count; i++)
            {
                string dir = al[i].ToString();
                dirs[i] = dir;
                if (dir == newDir)
                {
                    return; 
                }
            }

            string data = string.Join("\r\n", dirs);

            var dr = MessageBox.Show("当前选择的游戏目录不正确，系统已检测到您的游戏目录为\r\n" + al[0].ToString() + "\r\n是否安装到游戏目录？", "提示信息", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if(dr== DialogResult.Yes)
            {
                textBox1.Text = al[0].ToString();
            }
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        
    }
}
