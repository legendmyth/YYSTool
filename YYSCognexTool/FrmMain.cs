using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro;
using Cognex.VisionPro.PMAlign;
using System.Data;

namespace YYSCognexTool
{
    public partial class FrmMain : Form
    {
        List<Ptma> list = new List<Ptma>();
        List<CogPMAlignTool> toolList = new List<CogPMAlignTool>();
        CogToolDisplay cogToolDisplay = new CogToolDisplay();
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for(int i=0;i<toolList.Count;i++)
            {
                if (list[i].enable == 1)
                {
                    toolList[i].InputImage = new CogImage8Grey(new Bitmap(@"3.bmp"));
                    toolList[i].Run();
                }
            }
            
        }
        void cogPMAlignTool_Ran(object sender, EventArgs e)
        {
            cogToolDisplay.Tool = (sender as CogPMAlignTool);
        }

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            cogToolDisplay.Dock = DockStyle.Fill;
            this.spcMain.Panel2.Controls.Add(cogToolDisplay);
            this.list = Ptma.LoadFromPath("config.xml");
            for (int i = 0; i < list.Count; i++)
            {
                Cognex.VisionPro.PMAlign.CogPMAlignTool cogPMAlignTool = new Cognex.VisionPro.PMAlign.CogPMAlignTool();
                cogPMAlignTool.Pattern.TrainImage = new CogImage8Grey(new Bitmap(list[i].imageSrc));
                cogPMAlignTool.Pattern.TrainRegion = null;
                cogPMAlignTool.Pattern.Origin.TranslationX = cogPMAlignTool.Pattern.TrainImage.Width / 2;
                cogPMAlignTool.Pattern.Origin.TranslationY = cogPMAlignTool.Pattern.TrainImage.Height / 2;
                cogPMAlignTool.RunParams.AcceptThreshold = 0.8;
                //cogPMAlignTool.LastRunRecordDiagEnable = cogPMAlignTool.LastRunRecordDiagEnable&CogPMAlignLastRunRecordDiagConstants.ResultsMatchFeatures;
                cogPMAlignTool.LastRunRecordEnable = CogPMAlignLastRunRecordConstants.ResultsMatchShapeModels|CogPMAlignLastRunRecordConstants.ResultsCoordinateAxes | cogPMAlignTool.LastRunRecordEnable | CogPMAlignLastRunRecordConstants.ResultsMatchRegion;
                cogPMAlignTool.RunParams.RunAlgorithm = CogPMAlignRunAlgorithmConstants.BestTrained;
                cogPMAlignTool.RunParams.ZoneScale.Low = 0.8;// = new CogPMAlignZoneScale();
                cogPMAlignTool.RunParams.ZoneScale.High = 1.2;
                cogPMAlignTool.Pattern.Train();
                cogPMAlignTool.Ran += new EventHandler(cogPMAlignTool_Ran);
                this.toolList.Add(cogPMAlignTool);
            }
            this.dgvMain.DataSource = this.list;

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
                    ToolTip toolTip = new ToolTip();
                    toolTip.AutoPopDelay = 500;
                    toolTip.AutomaticDelay = 2000;
                    toolTip.UseAnimation = true;
                    toolTip.Show("训练图", this);
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
    }
}
