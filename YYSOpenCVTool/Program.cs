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
            //String templateFilePath = @"D:\Users\Administrator\Desktop\small.bmp";
            //String originalFilePath = @"D:\Users\Administrator\Desktop\big.bmp";
            ////读取图片文件
            //Mat templateImage = Cv2.ImRead(templateFilePath, OpenCvSharp.LoadMode.Color);
            //Mat originalImage = Cv2.ImRead(originalFilePath, OpenCvSharp.LoadMode.Color);

            //ImageRecognition imageRecognition = new ImageRecognition();
            //imageRecognition.matchImage(templateImage, originalImage);

            //Console.WriteLine("匹配的像素点总数：" + imageRecognition.getMatchesPointCount());

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
