namespace ADOMD_Csharp_example
{
    partial class AdomdDimTree
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AdomdDimTree));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Cube");
            this.imageList1.Images.SetKeyName(1, "Dimension");
            this.imageList1.Images.SetKeyName(2, "Hier");
            this.imageList1.Images.SetKeyName(3, "HierAttr");
            this.imageList1.Images.SetKeyName(4, "Level1");
            this.imageList1.Images.SetKeyName(5, "Level2");
            this.imageList1.Images.SetKeyName(6, "Level3");
            this.imageList1.Images.SetKeyName(7, "Level4");
            this.imageList1.Images.SetKeyName(8, "Level5");
            this.imageList1.Images.SetKeyName(9, "Member");
            this.imageList1.Images.SetKeyName(10, "Measure");
            this.imageList1.Images.SetKeyName(11, "CalcMeasure");
            this.imageList1.Images.SetKeyName(12, "Folder");
            this.imageList1.Images.SetKeyName(13, "Level0");
            this.imageList1.Images.SetKeyName(14, "Kpi");
            this.imageList1.Images.SetKeyName(15, "set");
            this.imageList1.Images.SetKeyName(16, "OpenFolder");
            // 
            // AdomdDimTree
            // 
            this.ImageKey = "Member";
            this.ImageList = this.imageList1;
            this.LineColor = System.Drawing.Color.Black;
            this.SelectedImageIndex = 9;
            this.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.AdomdDimTree_BeforeExpand);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imageList1;
    }
}
