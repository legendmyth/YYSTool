using OpenCvSharp.CPlusPlus;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YYSOpenCVTool
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            System.Environment.SetEnvironmentVariable("path", System.Environment.GetEnvironmentVariable("path") + @";D:\opencv-2.4.10\opencv\build\x64\vc12\bin");

            String templateFilePath = @"C:\Users\Administrator\Desktop\template.bmp";
            String originalFilePath = @"C:\Users\Administrator\Desktop\original.bmp";

            ////读取图片文件
            Mat templateImage = Cv2.ImRead(templateFilePath, OpenCvSharp.LoadMode.Color);
            Mat originalImage = Cv2.ImRead(originalFilePath, OpenCvSharp.LoadMode.Color);
            //Cv2.CvtColor()
            //ImageRecognition imageRecognition = new ImageRecognition();
            ImageRecognition.matchImage(templateImage, originalImage, 0.6f);

            //Console.WriteLine("匹配的像素点总数：" + imageRecognition.getMatchesPointCount());

            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new FrmMain());
        }
    }
}
