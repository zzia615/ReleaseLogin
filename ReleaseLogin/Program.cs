using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace ReleaseLogin
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            ReleaseDll("ICSharpCode.SharpZipLib.dll");
           // ReleaseDll("Interop.IWshRuntimeLibrary.dll");



            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void ReleaseDll(string dll)
        {
            if (!File.Exists(dll))
            {
                var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ReleaseLogin."+ dll);
                StreamToFile(stream, dll);
            }
        }
        public static void StreamToFile(Stream stream, string fileName)

        {

            // 把 Stream 转换成 byte[]

            byte[] bytes = new byte[stream.Length];

            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始

            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件

            FileStream fs = new FileStream(fileName, FileMode.Create);

            BinaryWriter bw = new BinaryWriter(fs);

            bw.Write(bytes);

            bw.Close();

            fs.Close();

        }
    }
}
