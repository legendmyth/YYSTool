using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Threading;
using System.Runtime.InteropServices;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace YYSOpenCVTool
{
    public partial class FrmMain : Form
    {
        List<Ptma> list = new List<Ptma>();
        List<TrainedTemplate> toolList = new List<TrainedTemplate>();
        public FrmMain()
        {
            InitializeComponent();
        }
        
        void cogPMAlignTool_Ran(object sender, EventArgs e)
        {
            
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            this.list = Ptma.LoadFromPath("config.xml");
            for (int i = 0; i < list.Count; i++)
            {
                TrainedTemplate trainedTemplate = new TrainedTemplate();
                trainedTemplate.templateImage = Cv2.ImRead(list[i].imageSrc, OpenCvSharp.LoadMode.Color);

                SURF featureDetector = new SURF();
                //获取模板图的特征点
                KeyPoint[] templateKeyPoints = featureDetector.Detect(trainedTemplate.templateImage);
                //提取模板图的特征点
                Mat templateDescriptors = new Mat(trainedTemplate.templateImage.Rows, trainedTemplate.templateImage.Cols, trainedTemplate.templateImage.Type());
                SURF descriptorExtractor = new SURF();
                descriptorExtractor.Compute(trainedTemplate.templateImage, ref templateKeyPoints, templateDescriptors);
                trainedTemplate.templateDescriptors = templateDescriptors;
                trainedTemplate.templateKeyPoints = templateKeyPoints;
                this.toolList.Add(trainedTemplate);

            }
            this.dgvMain.DataSource = this.list;

            Thread bgThread = new Thread(CaptureAndAnalyse);
            bgThread.IsBackground = true;
            bgThread.Start();

        }

        private void dgvMain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (this.dgvMain.Columns[e.ColumnIndex] == this.dgvMain.Columns["COpt"])
                {
                    this.list[e.RowIndex].enable = this.list[e.RowIndex].enable == 1 ? 0 : 1;
                    this.list[e.RowIndex].opt = this.list[e.RowIndex].enable == 1 ? Properties.Resources.stop_40 : Properties.Resources.start_40;
                }
                if (this.dgvMain.Columns[e.ColumnIndex] == this.dgvMain.Columns["CImage"])
                {
                    //ToolTip toolTip = new ToolTip();
                    //toolTip.AutoPopDelay = 500;
                    //toolTip.AutomaticDelay = 2000;
                    //toolTip.UseAnimation = true;
                    //toolTip.Show("训练图", this);
                    //this.pbTrainImage.BackgroundImage = new Bitmap(this.dgvMain.Rows[e.RowIndex].Cells["CImageSrc"].Value.ToString());

                }
            }
            catch (Exception ex)
            {
            }
        }

        private void dgvMain_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private bool start = false;

        private void btnStart_Click(object sender, EventArgs e)
        {
            start = !start;
            this.btnStart.Text = !start ? "开始" : "结束";
        }

        private void Click(int x, int y, int _dx, int dx, int _dy, int dy)
        {
            System.Random random = new Random(DateTime.Now.Millisecond);
            x = x + random.Next(_dx, dx);
            y = y + random.Next(_dy, dy);
            SetCursorPos(x, y);

            Thread.Sleep(random.Next(1, 500));
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

            Thread.Sleep(random.Next(100, 200));
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

            int times = random.Next(0, 2);
            for (int i = 0; i < times; i++)
            {
                x = x + random.Next(_dx, dx);
                y = y + random.Next(_dy, dy);
                SetCursorPos(x, y);

                Thread.Sleep(random.Next(1, 300));
                mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
                ;
                Thread.Sleep(random.Next(100, 200));
                mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
        }

        private void CaptureAndAnalyse()
        {
            while (true)
            {
                if (start)
                {
                    IntPtr hadle = FindWindow(null, "阴阳师-网易游戏");
                    if(hadle==null)
                    {
                        break;
                    }
                    RECT rect = new RECT();
                    GetWindowRect(hadle, ref rect);
                    // 新建一个和屏幕大小相同的图片
                    Bitmap catchBmp = new Bitmap(rect.Right - rect.Left, rect.Bottom - rect.Top);
                    // 创建一个画板，让我们可以在画板上画图
                    // 这个画板也就是和屏幕大小一样大的图片
                    // 我们可以通过Graphics这个类在这个空白图片上画图
                    Graphics g = Graphics.FromImage(catchBmp);
                    // 把屏幕图片拷贝到我们创建的空白图片 CatchBmp中
                    //g.CopyFromScreen(rect.Left,rect.Top,rect.Right,rect.Bottom,)
                    g.CopyFromScreen(new System.Drawing.Point(rect.Left,rect.Top), new System.Drawing.Point(0,0), new System.Drawing.Size(rect.Right - rect.Left, rect.Bottom - rect.Top));
                    for (int i = 0; i < toolList.Count; i++)
                    {
                        if (list[i].enable == 1)
                        {
                            MatchResult result = MatchTool.Match(catchBmp, toolList[i]);
                            //this.pnlResult.Invoke(new MethodInvoker(delegate { this.pnlResult.CreateGraphics().DrawImage(catchBmp, 20, 20);   }));
                            if (result!=null)
                            {

                                //this.pnlResult.Invoke(new MethodInvoker(delegate { this.pnlResult.CreateGraphics().DrawRectangle(Pens.Green, result.left, result.top, result.right - result.left, result.bottom - result.top); }));
                                int x = result.left + toolList[i].templateImage.Width / 2 + rect.Left;
                                int y = result.top + toolList[i].templateImage.Height / 2 + rect.Top;

                                Click(x, y, (int)this.dgvMain.Rows[i].Cells["C_DX"].Value, (int)this.dgvMain.Rows[i].Cells["CDx"].Value, (int)this.dgvMain.Rows[i].Cells["C_DY"].Value, (int)this.dgvMain.Rows[i].Cells["CDy"].Value);
                                break;
                            }
                        }
                    }
                }
                Thread.Sleep(300);
            }


        }

        #region win32 api

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;                             //最左坐标
            public int Top;                             //最上坐标
            public int Right;                           //最右坐标
            public int Bottom;                        //最下坐标
        }

        [DllImport("User32.dll")]
        public extern static bool GetCursorPos(ref System.Drawing.Point pot);

        [DllImport("User32.dll")]
        public extern static void SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        static extern void mouse_event(int flags, int dx, int dy, uint data, int extraInfo);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, string lParam);

        [DllImport("User32.dll ")]
        public static extern IntPtr FindWindowEx(IntPtr parent, IntPtr childe, string strclass, string FrmText);
        //移动鼠标 
        const int MOUSEEVENTF_MOVE = 0x0001;
        //模拟鼠标左键按下 
        const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        //模拟鼠标左键抬起 
        const int MOUSEEVENTF_LEFTUP = 0x0004;
        //模拟鼠标右键按下 
        const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        //模拟鼠标右键抬起 
        const int MOUSEEVENTF_RIGHTUP = 0x0010;
        //模拟鼠标中键按下 
        const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        //模拟鼠标中键抬起 
        const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        //标示是否采用绝对坐标 
        const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        #endregion


    }
}
