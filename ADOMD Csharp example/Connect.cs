using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;

namespace ADOMD_Csharp_example
{
    public partial class Connect : Form
    {
        private AdomdConnection moConn;
        private TreeNode moTreeClickedNode = null;
        private TreeNode moTreFunctionNode = null;
        private AdomdTextEditor adomdTextEditor1;

        public Connect()
        {
            InitializeComponent();
            InitializeTextEditor();

            moConn = new AdomdConnection();


        }

        private void InitializeTextEditor()
        {
            adomdTextEditor1 = new AdomdTextEditor();
            adomdTextEditor1.Dock = DockStyle.Fill;
            adomdTextEditor1.AllowDrop = true;
            splitContainer2.Panel1.Controls.Add(adomdTextEditor1);
            adomdTextEditor1.ContextMenuStrip = contextMenuStrip1;


        }

        void Default_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            ConnectToCube();
        }

        private void ConnectToCube()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string sServer = txtServer.Text;
                string sProvider = Settings1.Default.Provider;

                if (moConn.State == ConnectionState.Open)
                    moConn.Close();

                moConn.ConnectionString = "Provider=" + sProvider + ";data source=" + sServer + ";"; // Initial Catalog=" + sCatalog + ";";

                try
                {
                    moConn.Open();
                    txtServer.AutoCompleteCustomSource.Add(sServer);
                    LoadCatalogs();
                    LoadFunctions();
                }
                catch (AdomdConnectionException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            finally
            {
                this.Cursor = Cursors.Default;
                this.SelectNextControl(btnConnect, true, true, true, true);
            }
        }

        private void LoadCatalogs()
        {
            DataSet ds = moConn.GetSchemaDataSet(AdomdSchemaGuid.Catalogs, new object[0]);
            
            DataTable dt = ds.Tables[0];

            cboCatalogs.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                string sCatalogName = row[0].ToString();
                cboCatalogs.Items.Add(sCatalogName);
            }
        }

        private void LoadCubes(AdomdConnection conn)
        {
            cboCubes.Items.Clear();
            cboCubes.Text = "";
            try
            {
                if (conn.Cubes.Count > 0)
                {
                    foreach (CubeDef oCube in conn.Cubes)
                    {
                        if (oCube.Type == CubeType.Cube)
                        {
                            cboCubes.Items.Add(oCube);
                        }
                    }
                }
            }
            catch (AdomdException ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadFunctions()
        {
            treFunctions.Connection = moConn;
        }

        private void cboCubes_SelectedIndexChanged(object sender, EventArgs e)
        {
            CubeDef oCube = cboCubes.SelectedItem as CubeDef;
            adomdDimTree1.Cube = oCube;
            //txtMDX.Text = DefaultQueryBuilder.CreateDefaultQuery(oCube);
            adomdTextEditor1.Text = MDXHelpers.CreateDefaultQuery(oCube);
        }

        private void btnCommand_Click(object sender, EventArgs e)
        {
            Command dlgCommand = new Command();
            dlgCommand.Connection = moConn;
            dlgCommand.Cube = cboCubes.SelectedItem as CubeDef;

            dlgCommand.Show();
        }

        private void btnExecuteCellSet_Click(object sender, EventArgs e)
        {
            ExecuteCommandCellSet();
        }

        private string GetCommand()
        {
            return adomdTextEditor1.Text;
            //if (txtMDX.SelectedText.Length > 0)
            //    return txtMDX.SelectedText;
            //else
            //    return txtMDX.Text;

        }

        /// <summary>
        /// Execute an MDX command via the AdomdCommand.ExecuteCellSet method.
        /// </summary>
        private void ExecuteCommandCellSet()
        {
            string sMDX = GetCommand();

            try
            {
                AdomdCommand oCmd = new AdomdCommand(sMDX, moConn);

                CellSet set = oCmd.ExecuteCellSet();
                grdResults.LoadData(set);
                grdResults.AutoSize();

            }
            catch (AdomdException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (InvalidOperationException exo)
            {
                MessageBox.Show(exo.Message);
            }
        }

        /// <summary>
        /// Execute an MDX command via the AdomdCommand.ExecuteReader method.
        /// </summary>
        private void ExecuteCommandReader()
        {
            string sMDX = GetCommand();

            try
            {
                AdomdCommand oCmd = new AdomdCommand(sMDX, moConn);

                using (AdomdDataReader oReader = oCmd.ExecuteReader())
                {
                    grdResults.LoadData(oReader);
                    grdResults.AutoSize();
                }
            }
            catch (AdomdException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (InvalidOperationException exo)
            {
                MessageBox.Show(exo.Message);
            }
        }

        /// <summary>
        /// Execute an MDX command via a AdomdDataAdapter
        /// </summary>
        private void ExecuteCommandDataSet()
        {
            string sMDX = GetCommand();

            try
            {
                AdomdCommand oCmd = new AdomdCommand(sMDX, moConn);

                AdomdDataAdapter da = new AdomdDataAdapter(oCmd);

                DataTable dt = new DataTable();

                da.Fill(dt);

                grdResults.LoadData(dt);
                grdResults.AutoSize();
            }
            catch (AdomdException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (InvalidOperationException exo)
            {
                MessageBox.Show(exo.Message);
            }

        }


        private void btnExecuteReader_Click(object sender, EventArgs e)
        {
            ExecuteCommandReader();
        }

        private void btnExecuteDataSet_Click(object sender, EventArgs e)
        {
            ExecuteCommandDataSet();
        }


        private void adomdDimTree1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode oDragNode = moTreeClickedNode;

            if (oDragNode == null)
                oDragNode = adomdDimTree1.SelectedNode;

            string sData = GetNodeText(oDragNode);

            if (sData != string.Empty)
                DoDragDrop(sData, DragDropEffects.Move);

            //grdResults.Select();
            adomdTextEditor1.Select();

            /*
            if (oDragNode != null)
            {
                string sData = oDragNode.Text;
                object oTag = oDragNode.Tag;

                if (oTag != null)
                {
                    if (oTag is Level)
                    {
                        sData = (oTag as Level).UniqueName;
                    }
                    else if (oTag is Member)
                    {
                        sData = (oTag as Member).UniqueName;
                    }
                    else if (oTag is Dimension)
                    {
                        sData = (oTag as Dimension).UniqueName;
                    }
                    else if (oTag is Hierarchy)
                    {
                        sData = (oTag as Hierarchy).UniqueName;
                    }
                    else if (oTag is Measure)
                    {
                        sData = (oTag as Measure).UniqueName;
                    }
                    else if (oTag is CubeDef)
                    {
                        sData = "[" + (oTag as CubeDef).Name + "]";
                    }
                    else if (oTag is Kpi)
                    {
                        if (oDragNode.Nodes.Count > 0)
                            sData = (oTag as Kpi).Name;
                        else
                        {
                            oTag = oDragNode.Parent.Tag;
                            if (sData == "Status")
                            {
                                sData = "KpiStatus(\"" + (oTag as Kpi).Name + "\")";
                            }
                            else if (sData == "Value")
                            {
                                sData = "KpiValue(\"" + (oTag as Kpi).Name + "\")";
                            }
                            else if (sData == "Goal")
                            {
                                sData = "KpiGoal(\"" + (oTag as Kpi).Name + "\")";
                            }


                        }

                    }
                    else if (oTag is NamedSet)
                    {
                        sData = "["+(oTag as NamedSet).Name+"]";
                    }
                }
              

                DoDragDrop(sData, DragDropEffects.Move);
            }
             */
        }


        private string GetNodeText(TreeNode node)
        {
            string sData = string.Empty;
            if (node != null)
            {
                sData = node.Text;
                object oTag = node.Tag;

                if (oTag != null)
                {
                    if (oTag is Level)
                    {
                        sData = (oTag as Level).UniqueName;
                    }
                    else if (oTag is Member)
                    {
                        sData = (oTag as Member).UniqueName;
                    }
                    else if (oTag is Dimension)
                    {
                        sData = (oTag as Dimension).UniqueName;
                    }
                    else if (oTag is Hierarchy)
                    {
                        sData = (oTag as Hierarchy).UniqueName;
                    }
                    else if (oTag is Measure)
                    {
                        sData = (oTag as Measure).UniqueName;
                    }
                    else if (oTag is CubeDef)
                    {
                        sData = "[" + (oTag as CubeDef).Name + "]";
                    }
                    else if (oTag is Kpi)
                    {
                        if (node.Nodes.Count > 0)
                            sData = (oTag as Kpi).Name;
                        else
                        {
                            oTag = node.Parent.Tag;
                            if (sData == "Status")
                            {
                                sData = "KpiStatus(\"" + (oTag as Kpi).Name + "\")";
                            }
                            else if (sData == "Value")
                            {
                                sData = "KpiValue(\"" + (oTag as Kpi).Name + "\")";
                            }
                            else if (sData == "Goal")
                            {
                                sData = "KpiGoal(\"" + (oTag as Kpi).Name + "\")";
                            }


                        }

                    }
                    else if (oTag is NamedSet)
                    {
                        sData = "[" + (oTag as NamedSet).Name + "]";
                    }
                }

            }
            return sData;
        }
        //private void txtMDX_DragDrop(object sender, DragEventArgs e)
        //{
        //    Point pt = txtMDX.PointToClient(new Point(e.X, e.Y));

        //    string sData = e.Data.GetData(typeof(System.String)).ToString();
        //    txtMDX.Text += sData;
        //}

        //private void txtMDX_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effect = e.AllowedEffect;
        //}

        private void adomdDimTree1_MouseDown(object sender, MouseEventArgs e)
        {
            //moTreeClickedNode = adomdDimTree1.GetNodeAt(e.X, e.Y);
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmOptions dlg = new frmOptions();
            dlg.ShowDialog();

            this.Font = Settings1.Default.Font;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            dlg.ShowDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamReader oReader = new System.IO.StreamReader(openFileDialog1.FileName);
                string sText = "";
                string sLine = null;

                while ((sLine = oReader.ReadLine()) != null) 
                {
                    sText += sLine + "\n";    
                }

                //txtMDX.Text = sText;
                adomdTextEditor1.Text = sText;
                oReader.Close();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMDX();
        }

        private void SaveMDX()
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.IO.StreamWriter oWriter = new System.IO.StreamWriter(saveFileDialog1.FileName);
                //oWriter.Write(txtMDX.Text);
                oWriter.Write(adomdTextEditor1.Text);
                oWriter.Close();
            }
        }

        private void Connect_FormClosing(object sender, FormClosingEventArgs e)
        {
            Settings1.Default.StartingLoc = this.Location;
            Settings1.Default.StartingSize = this.Size;
            Settings1.Default.Servers = txtServer.AutoCompleteCustomSource;

            Settings1.Default.Save();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.Undo();
            adomdTextEditor1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.Redo();
            adomdTextEditor1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.Cut();
            adomdTextEditor1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.Copy();
            adomdTextEditor1.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.Paste();
            adomdTextEditor1.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //txtMDX.SelectAll();
            adomdTextEditor1.SelectAll();
        }


        private void cboCatalogs_SelectedIndexChanged(object sender, EventArgs e)
        {
            string sCatalogName = cboCatalogs.Text;

            moConn.ChangeDatabase(sCatalogName);

            LoadCubes(moConn);
        }

        private void schemaInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSchemaInfo oDlg = new frmSchemaInfo(moConn);
            oDlg.ShowDialog();
        }

        private void treFunctions_ItemDrag(object sender, ItemDragEventArgs e)
        {
            
            TreeNode oDragNode = moTreFunctionNode;

            if (e.Item is TreeNode)
                oDragNode = e.Item as TreeNode;

            if (oDragNode == null)
                oDragNode = treFunctions.SelectedNode;

            if (oDragNode.Parent == null) return;

            if (oDragNode != null)
            {
                string sData = oDragNode.Text;
                object oTag = oDragNode.Tag;

                if (oTag != null)
                {
                    if (oTag is FunctionData)
                    {
                        sData = (oTag as FunctionData).DragValue;
                        DoDragDrop(sData, DragDropEffects.Copy);
                    }
                }
            }
        }

        private void tabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            //moTreFunctionNode = treFunctions.GetNodeAt(e.X, e.Y);
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
        }

        private void treFunctions_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode oNode = treFunctions.GetNodeAt(e.X, e.Y);
            if (oNode != null)
            {
                object oTag = oNode.Tag;
                if ((oTag != null) && (oTag is FunctionData))
                {
                    FunctionData oData = oTag as FunctionData;
                    string sText = oData.DragValue;
                    if ((oData.Description != null) && (oData.Description.Length > 0))
                        sText += "\n" + oData.Description;

                    if (toolTip1.GetToolTip(treFunctions) != sText)
                        toolTip1.SetToolTip(treFunctions, sText);
                }
            }
            else
            {
                toolTip1.SetToolTip(treFunctions, "");
            }
        }

        private void adomdDimTree1_MouseMove(object sender, MouseEventArgs e)
        {
            TreeNode oNode = adomdDimTree1.GetNodeAt(e.X, e.Y);
            if (oNode != null)
            {
                object oTag = oNode.Tag;
                string sText;

                if (oTag is Member)
                {
                    Member oMember = oTag as Member;
                    sText = oMember.Caption + "\n" + oMember.UniqueName;
                    if (oMember.Description.Length > 0)
                        sText += "\n" + oMember.Description;
                    if (oMember.Properties.Count > 0)
                    {
                        string sProps = MemberPropertiesString(oMember);
                        sText += "\n" + sProps;
                    }
                }
                else if (oTag is Measure)
                {
                    Measure oMeasure = oTag as Measure;
                    sText = oMeasure.Caption + "\n" + oMeasure.UniqueName;
                    if (oMeasure.Description.Length > 0)
                        sText += "\n" + oMeasure.Description;
                    
                }
                else if (oTag is Dimension)
                {
                    Dimension oDim = oTag as Dimension;
                    sText = oDim.Caption + "\n" + oDim.UniqueName;
                }
                else if (oTag is Hierarchy)
                {
                    Hierarchy oHier = oTag as Hierarchy;
                    sText = oHier.Caption + "\n" + oHier.UniqueName;
                }
                else if (oTag is Level)
                {
                    Level oLevel = oTag as Level;
                    sText = oLevel.Caption + "\n" + oLevel.UniqueName;
                }
                else if (oTag is Kpi)
                {
                    Kpi oKpi = oTag as Kpi;
                    sText = oKpi.Caption + "\n" + oKpi.Description;
                }
                else
                {
                    toolTip1.SetToolTip(adomdDimTree1, "");
                    return;
                }

                if (toolTip1.GetToolTip(adomdDimTree1) != sText)
                    toolTip1.SetToolTip(adomdDimTree1, sText);

            }
            else
            {
                toolTip1.SetToolTip(adomdDimTree1, "");
            }
        }

        private string MemberPropertiesString(Member aMember)
        {
            string sText = "";

            foreach (Property oProp in aMember.Properties)
            {
                if (sText.Length > 0)
                    sText += "\n" +oProp.Name + "=" + oProp.Value;
                else
                    sText = oProp.Name + "=" + oProp.Value;

            }

            return sText;
        }

        private void menuDimTree_Opening(object sender, CancelEventArgs e)
        {
            copyCaptionToolStripMenuItem.Enabled = false;
            copyNameToolStripMenuItem.Enabled = false;
            if (moTreeClickedNode != null)
            {
                copyCaptionToolStripMenuItem.Enabled = true;
                
             //   e.Cancel = true;
            }
            propertiesToolStripMenuItem.Enabled = false;
            if ((adomdDimTree1.SelectedNode != null) && (adomdDimTree1.SelectedNode.Tag != null))
                propertiesToolStripMenuItem.Enabled = true;

        }

        private void txtServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void txtServer_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ConnectToCube();
        }

        private void windowSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this.Size.ToString());
        }

        private void Connect_Load(object sender, EventArgs e)
        {

        }

        private bool mbActivated = false;
        private void Connect_Activated(object sender, EventArgs e)
        {
            if (mbActivated) return;

            //if (Settings1.Default.StartingLoc != null)
            //    this.Location = (System.Drawing.Point)Settings1.Default.StartingLoc;
            //if (Settings1.Default.StartingSize != null)
            //    this.Size = (System.Drawing.Size)Settings1.Default.StartingSize;

            if (Settings1.Default.Servers != null)
                txtServer.AutoCompleteCustomSource = Settings1.Default.Servers;
            else
                txtServer.AutoCompleteCustomSource = new AutoCompleteStringCollection();

            this.Font = Settings1.Default.Font;
            Settings1.Default.PropertyChanged += new PropertyChangedEventHandler(Default_PropertyChanged);
            mbActivated = true;

            //txtMDX.AutoWordSelection = true;
            //txtMDX.AutoWordSelection = false;
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (adomdDimTree1.SelectedNode != null)
            {
                PropertiesDlg dlg = new PropertiesDlg(adomdDimTree1.SelectedNode.Tag);
                dlg.ShowDialog();
            }
        }

        private void copyCaptionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (adomdDimTree1.SelectedNode != null)
                Clipboard.SetText(adomdDimTree1.SelectedNode.Text);
        }

        private void copyNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (adomdDimTree1.SelectedNode != null)
                Clipboard.SetText(adomdDimTree1.SelectedNode.Text);

        }

        private void adomdDimTree1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void adomdDimTree1_MouseClick(object sender, MouseEventArgs e)
        {
            if ((e.Button == MouseButtons.Right) && (adomdDimTree1.SelectedNode != null))
                menuDimTree.Show();

        }

        private void connectionStringToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (moConn.State == ConnectionState.Open)
                MessageBox.Show(moConn.ConnectionString);
            else
                MessageBox.Show("You must be connected to the cube for this option to work.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void propertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PropertiesDlg dlg = new PropertiesDlg(adomdTextEditor1.BaseControl );
            dlg.ShowDialog();
        }

        private void btnReconnect_Click(object sender, EventArgs e)
        {
            if ((cboCubes.SelectedItem != null) && (cboCatalogs.SelectedItem != null))
            {
                if (adomdTextEditor1.Text == "")
                {
                    string sCatalogName = cboCatalogs.Text;
                    string sCubeName = cboCubes.Text;

                    moConn.ChangeDatabase(sCatalogName);
                    LoadCubes(moConn);

                    cboCubes.Text = sCubeName;

                    CubeDef oCube = cboCubes.SelectedItem as CubeDef;
                    adomdDimTree1.Cube = oCube;
                }
            }
        }

        private void formatMDXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            adomdTextEditor1.Text = MDXHelpers.FormatMDX(adomdTextEditor1.Text);
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            
        }

        private void menuGrid_Opening(object sender, CancelEventArgs e)
        {

        }

        private void cellPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int iRow = grdResults.Selection.ActivePosition.Row;
            int iCol = grdResults.Selection.ActivePosition.Column;

            grdResults.CheckCellData(iRow, iCol);
        }

        private void adomdDimTree1_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.Control) && (e.KeyCode == Keys.C))
            {
                string sData = GetNodeText(adomdDimTree1.SelectedNode);

                Clipboard.SetText(sData);
            }
        }

        private void adomdDimTree1_MouseDown_1(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TreeNode node = adomdDimTree1.GetNodeAt(new Point(e.X, e.Y));

                adomdDimTree1.SelectedNode = node;

            }

        }
    }
}