using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace ReleaseLogin
{
    public partial class Form2 : Form
    {
        public string TargetFolder { get; }
        public Form2(string targetFolder)
        {
            InitializeComponent();
            TargetFolder = targetFolder;
        }
        public void SetWindowRegion()
        {

            System.Drawing.Drawing2D.GraphicsPath FormPath;

            FormPath = new System.Drawing.Drawing2D.GraphicsPath();

            Rectangle rect = new Rectangle(0, 22, this.Width, this.Height - 22);//this.Left-10,this.Top-10,this.Width-10,this.Height-10);

            FormPath = GetRoundedRectPath(rect, 10);

            this.Region = new Region(FormPath);

        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {

            int diameter = radius;

            Rectangle arcRect = new Rectangle(rect.Location, new Size(diameter, diameter));

            GraphicsPath path = new GraphicsPath();

            // 左上角

            path.AddArc(arcRect, 180, 90);

            // 右上角

            arcRect.X = rect.Right - diameter;

            path.AddArc(arcRect, 270, 90);

            // 右下角

            arcRect.Y = rect.Bottom - diameter;

            path.AddArc(arcRect, 0, 90);

            // 左下角

            arcRect.X = rect.Left;

            path.AddArc(arcRect, 90, 90);

            path.CloseFigure();

            return path;

        }

        protected override void OnResize(System.EventArgs e)
        {

            this.Region = null;

            SetWindowRegion();

        }

        private void Form2_Load(object sender, EventArgs e)
        {

            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += Bg_DoWork;
            bg.RunWorkerCompleted += Bg_RunWorkerCompleted;
            bg.RunWorkerAsync();
        }

        private void Bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Result is Exception)
            {
                var ex = e.Result as Exception;
                MessageBox.Show("解压出错，错误信息：" + ex.Message, "安装异常", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.Close();
        }

        private void Bg_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                InstallGame();
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void InstallGame()
        {
            //解压缩
            Decompress();
            //创建桌面快捷方式
            string desk_dir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ShortcutCreator.CreateShortcut(desk_dir, "热血江湖资料片之玄冰迷宫.link", Path.Combine(TargetFolder, "Launcher.exe"));
            //打开网页
            Process.Start("http://baidu.com");
            
        }
        
        void SetProgress(int position,int count,string sm)
        {
            this.Invoke(new Action(() =>
            {
                progressBar1.Maximum = count;
                progressBar1.Value = position;
                label2.Text = ((position * 100) / count).ToString() + "%";
            }));
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="sourceFile">stream</param>
        public bool Decompress()
        {
            System.IO.Stream tmp1 = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReleaseLogin.Game.zip");
            System.IO.Stream tmp2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReleaseLogin.Game.zip");

            int count = 0;
            using (ZipInputStream s = new ZipInputStream(tmp1))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                    count++;
            }
            int progress = 0;
            using (ZipInputStream s = new ZipInputStream(tmp2))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    progress++;
                    SetProgress(progress, count, "");
                    theEntry.CompressedSize = 9;
                    string directorName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // 创建目录
                    if (directorName.Length > 0)
                    {
                        directorName = Path.Combine(TargetFolder, directorName);
                        if (!Directory.Exists(directorName))
                            Directory.CreateDirectory(directorName);
                    }


                    if (fileName != string.Empty)
                    {
                        fileName = Path.Combine(directorName, fileName);
                        using (FileStream streamWriter = File.Create(fileName))
                        {
                            int size = 4096;
                            byte[] data = new byte[4 * 1024];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else break;
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
            return true;
        }


        private bool confirmOverride(string fileName)
        {
            return true;
        }
    }
}
