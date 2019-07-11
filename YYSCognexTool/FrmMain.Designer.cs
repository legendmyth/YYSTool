namespace YYSCognexTool
{
    partial class FrmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.spcMain = new System.Windows.Forms.SplitContainer();
            this.dgvMain = new System.Windows.Forms.DataGridView();
            this.CName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.CEnable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CImageSrc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C_DX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.C_DY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CDx = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CDy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.COpt = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).BeginInit();
            this.spcMain.Panel1.SuspendLayout();
            this.spcMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).BeginInit();
            this.SuspendLayout();
            // 
            // spcMain
            // 
            this.spcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.spcMain.Location = new System.Drawing.Point(0, 0);
            this.spcMain.Margin = new System.Windows.Forms.Padding(5);
            this.spcMain.Name = "spcMain";
            // 
            // spcMain.Panel1
            // 
            this.spcMain.Panel1.Controls.Add(this.dgvMain);
            this.spcMain.Size = new System.Drawing.Size(1098, 357);
            this.spcMain.SplitterDistance = 576;
            this.spcMain.SplitterWidth = 7;
            this.spcMain.TabIndex = 3;
            // 
            // dgvMain
            // 
            this.dgvMain.AllowUserToAddRows = false;
            this.dgvMain.AllowUserToDeleteRows = false;
            this.dgvMain.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMain.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dgvMain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CName,
            this.CText,
            this.CImage,
            this.CEnable,
            this.CImageSrc,
            this.C_DX,
            this.C_DY,
            this.CDx,
            this.CDy,
            this.COpt});
            this.dgvMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMain.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvMain.Location = new System.Drawing.Point(0, 0);
            this.dgvMain.Margin = new System.Windows.Forms.Padding(5);
            this.dgvMain.MultiSelect = false;
            this.dgvMain.Name = "dgvMain";
            this.dgvMain.RowTemplate.Height = 40;
            this.dgvMain.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvMain.Size = new System.Drawing.Size(576, 357);
            this.dgvMain.TabIndex = 0;
            this.dgvMain.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMain_CellContentClick);
            this.dgvMain.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMain_CellContentDoubleClick);
            // 
            // CName
            // 
            this.CName.DataPropertyName = "name";
            this.CName.HeaderText = "名称";
            this.CName.Name = "CName";
            this.CName.Visible = false;
            // 
            // CText
            // 
            this.CText.DataPropertyName = "text";
            this.CText.HeaderText = "名称";
            this.CText.Name = "CText";
            // 
            // CImage
            // 
            this.CImage.DataPropertyName = "image";
            this.CImage.HeaderText = "图像";
            this.CImage.Name = "CImage";
            this.CImage.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.CImage.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // CEnable
            // 
            this.CEnable.DataPropertyName = "enable";
            this.CEnable.HeaderText = "可用性";
            this.CEnable.Name = "CEnable";
            this.CEnable.Visible = false;
            // 
            // CImageSrc
            // 
            this.CImageSrc.DataPropertyName = "imageSrc";
            this.CImageSrc.HeaderText = "图片源";
            this.CImageSrc.Name = "CImageSrc";
            this.CImageSrc.Visible = false;
            // 
            // C_DX
            // 
            this.C_DX.DataPropertyName = "_dx";
            this.C_DX.HeaderText = "_dx";
            this.C_DX.Name = "C_DX";
            // 
            // C_DY
            // 
            this.C_DY.DataPropertyName = "_dy";
            this.C_DY.HeaderText = "_dy";
            this.C_DY.Name = "C_DY";
            // 
            // CDx
            // 
            this.CDx.DataPropertyName = "dx";
            this.CDx.HeaderText = "dx";
            this.CDx.Name = "CDx";
            // 
            // CDy
            // 
            this.CDy.DataPropertyName = "dy";
            this.CDy.HeaderText = "dy";
            this.CDy.Name = "CDy";
            // 
            // COpt
            // 
            this.COpt.DataPropertyName = "opt";
            this.COpt.HeaderText = "操作";
            this.COpt.Name = "COpt";
            this.COpt.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1098, 357);
            this.Controls.Add(this.spcMain);
            this.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "FrmMain";
            this.Text = "测试";
            this.Shown += new System.EventHandler(this.FrmMain_Shown);
            this.spcMain.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.spcMain)).EndInit();
            this.spcMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMain)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer spcMain;
        private System.Windows.Forms.DataGridView dgvMain;
        private System.Windows.Forms.DataGridViewTextBoxColumn CName;
        private System.Windows.Forms.DataGridViewTextBoxColumn CText;
        private System.Windows.Forms.DataGridViewImageColumn CImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn CEnable;
        private System.Windows.Forms.DataGridViewTextBoxColumn CImageSrc;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_DX;
        private System.Windows.Forms.DataGridViewTextBoxColumn C_DY;
        private System.Windows.Forms.DataGridViewTextBoxColumn CDx;
        private System.Windows.Forms.DataGridViewTextBoxColumn CDy;
        private System.Windows.Forms.DataGridViewImageColumn COpt;
    }
}

