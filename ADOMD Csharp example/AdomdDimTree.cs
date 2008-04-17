using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Microsoft.AnalysisServices.AdomdClient;

namespace ADOMD_Csharp_example
{
    public partial class AdomdDimTree : TreeView 
    {
        private const string c_Loading = "Loading...";

        private CubeDef moCube;


        public AdomdDimTree()
        {
            InitializeComponent();

            
        }

        public CubeDef Cube
        {
            get { return moCube; }
            set { moCube = value; LoadData(); }
        }

        public void LoadData()
        {
            FillTree(moCube);
        }

        private void FillTree(CubeDef cube)
        {
            this.Nodes.Clear();

            try
            {
                if (cube != null)
                {
                    TreeNode oCubeNode = this.Nodes.Add(cube.Caption);
                    oCubeNode.Tag = cube;
                    oCubeNode.ImageKey = "Cube";
                    oCubeNode.SelectedImageKey  = "Cube";

                    if (cube.Dimensions == null) return;

                    AddMeasures(cube, oCubeNode);

                    foreach (Dimension oDim in cube.Dimensions)
                    {
                        TreeNode oDimNode = oCubeNode.Nodes.Add(oDim.Caption);
                        oDimNode.Tag = oDim;
                        {
                            oDimNode.ImageKey = "Dimension";
                            oDimNode.SelectedImageKey = "Dimension";

                            foreach (Hierarchy oHier in oDim.Hierarchies)
                            {
                                TreeNode oDisplayFolder = GetDisplayFolder(oDimNode, oHier.DisplayFolder);
                                if (oHier.HierarchyOrigin == HierarchyOrigin.AttributeHierarchy)
                                {
                                    TreeNode oAttrHierNode = oDisplayFolder.Nodes.Add(oHier.Caption);
                                    oAttrHierNode.Tag = oHier;
                                    oAttrHierNode.ImageKey = "HierAttr";
                                    oAttrHierNode.SelectedImageKey = "HierAttr";

                                    LoadLevels(oHier, oAttrHierNode);
                                }
                                else
                                {
                                    TreeNode oHierNode = oDisplayFolder.Nodes.Add(oHier.Caption);
                                    oHierNode.Tag = oHier;
                                    oHierNode.ImageKey = "Hier";
                                    oHierNode.SelectedImageKey = "Hier";

                                    LoadLevels(oHier, oHierNode);
                                }
                            }
                        }
                    }

                    TreeNode oSetsNode = oCubeNode.Nodes.Add("Named Sets");
                    oSetsNode.ImageKey = "Set";
                    oSetsNode.SelectedImageKey = "Set";
                    
                    foreach (NamedSet oSet in cube.NamedSets)
                    {
                        TreeNode oNode = oSetsNode.Nodes.Add(oSet.Name);
                        
                        oNode.Tag = oSet;
                        oNode.SelectedImageKey = "Set";
                        oNode.ImageKey = "Set";
                    }

                    TreeNode oKpiNode = oCubeNode.Nodes.Add("KPIs");
                    oKpiNode.ImageKey = "Kpi";
                    oKpiNode.SelectedImageKey = "Kpi";

                    foreach (Kpi okpi in cube.Kpis)
                    {
                        TreeNode oNode = oKpiNode.Nodes.Add(okpi.Caption);
                        oNode.Tag = okpi;
                        oNode.SelectedImageKey = "Kpi";
                        oNode.ImageKey = "Kpi";

                        TreeNode oKpiStatus = oNode.Nodes.Add("Status");
                        oKpiStatus.Tag = okpi;
                        oKpiStatus.SelectedImageKey = "Kpi";
                        oKpiStatus.ImageKey = "Kpi";

                        TreeNode oKpiValue = oNode.Nodes.Add("Value");
                        oKpiValue.Tag = okpi;
                        oKpiValue.SelectedImageKey = "Kpi";
                        oKpiValue.ImageKey = "Kpi";
                        
                        TreeNode oKpiGoal = oNode.Nodes.Add("Goal");
                        oKpiGoal.Tag = okpi;
                        oKpiGoal.SelectedImageKey = "Kpi";
                        oKpiGoal.ImageKey = "Kpi";
                        
                    }
                    oCubeNode.Expand();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddMeasures(CubeDef cube, TreeNode oCubeNode)
        {
            TreeNode oMeasuresNode = oCubeNode.Nodes.Add("Measure", "Measure", "Measure", "Measure");

            DataSet dsMeasures = moCube.ParentConnection.GetSchemaDataSet(AdomdSchemaGuid.Measures, new object[0]);

            foreach (Measure oMeasure in cube.Measures)
            {
                string sDisplayFolder = "";
                bool bIsCalc = false;

                GetMeasureInfo(oMeasure, dsMeasures, out sDisplayFolder, out bIsCalc);

                TreeNode oDisplayFolder = GetDisplayFolder(oMeasuresNode, sDisplayFolder);
                TreeNode oNode = oDisplayFolder.Nodes.Add(oMeasure.Caption);
                oNode.Tag = oMeasure;
                if (!bIsCalc)
                {
                    oNode.ImageKey = "Measure";
                    oNode.SelectedImageKey = "Measure";
                }
                else
                {
                    oNode.ImageKey = "CalcMeasure";
                    oNode.SelectedImageKey = "CalcMeasure";
                }
                
            }
        }

        private void GetMeasureInfo(Measure m, DataSet ds, out string measureGroup, out bool isCalcMeasure)
        {
            measureGroup = "";
            isCalcMeasure = false;

            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (row["MEASURE_UNIQUE_NAME"].ToString() == m.UniqueName)
                {
                    measureGroup = row["MEASUREGROUP_NAME"].ToString();
                    int iMeasureType = Convert.ToInt32( row["DATA_TYPE"].ToString());
                    isCalcMeasure = (iMeasureType == 12);
                    return;
                }
            }
  
        }

        private TreeNode GetDisplayFolder(TreeNode parentNode, string displayFolder)
        {
            if (displayFolder == "")
                return parentNode;

            TreeNode oFolderNode = parentNode.Nodes[displayFolder];

            if (oFolderNode == null)
            {
                oFolderNode = parentNode.Nodes.Add(displayFolder, displayFolder, "Folder", "OpenFolder");
            }

            return oFolderNode;
        }

        private void LoadLevels(Hierarchy oHier, TreeNode oAttrHierNode)
        {
            foreach (Level oLevel in oHier.Levels)
            {
                TreeNode oLevelNode = oAttrHierNode.Nodes.Add(oLevel.Caption);
                oLevelNode.Tag = oLevel;

                if (oLevel.LevelType == LevelTypeEnum.All)
                {
                    oLevelNode.ImageKey = "Level0";
                    oLevelNode.SelectedImageKey = "Level0";
                }
                else
                {
                    switch (oLevel.LevelNumber)
                    {
                        case 0:
                            oLevelNode.ImageKey = "Level0";
                            oLevelNode.SelectedImageKey = "Level0";
                            break;
                        case 1:
                            oLevelNode.ImageKey = "Level1";
                            oLevelNode.SelectedImageKey = "Level1";
                            break;
                        case 2:
                            oLevelNode.ImageKey = "Level2";
                            oLevelNode.SelectedImageKey = "Level2";
                            break;
                        case 3:
                            oLevelNode.ImageKey = "Level3";
                            oLevelNode.SelectedImageKey = "Level3";
                            break;
                        case 4:
                            oLevelNode.ImageKey = "Level4";
                            oLevelNode.SelectedImageKey = "Level4";
                            break;
                        default:
                            oLevelNode.ImageKey = "Level5";
                            oLevelNode.SelectedImageKey = "Level5";
                            break;
                    }
                }
                oLevelNode.Nodes.Add("Loading...");
            }
        }

        private void AdomdDimTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Tag is Level)
            {
                if ((e.Node.Nodes.Count == 1) && (e.Node.Nodes[0].Text == c_Loading))
                {
                    e.Node.Nodes.Clear();

                    Level oLevel = e.Node.Tag as Level;

                    MemberCollection oMembers = oLevel.GetMembers(0, 150);

                    foreach (Member oMember in oMembers)
                    {
                        TreeNode oMemberNode = e.Node.Nodes.Add(oMember.Caption);
                        oMemberNode.Tag = oMember;
                        if (oMember.Type == MemberTypeEnum.Measure)
                        {
                            oMemberNode.ImageKey = "Measure";
                            oMemberNode.SelectedImageKey = "Measure";
                        }
                        else if (oMember.Type == MemberTypeEnum.Formula)
                        {
                            oMemberNode.ImageKey = "CalcMeasure";
                            oMemberNode.SelectedImageKey = "CalcMeasure";
                        }
                        else
                        {
                            oMemberNode.ImageKey = "Member";
                            oMemberNode.SelectedImageKey = "Member";
                        }
                        if (oMember.ChildCount > 0)
                            oMemberNode.Nodes.Add(c_Loading);
                    }

                }
            }
            else if (e.Node.Tag is Member)
            {
                if ((e.Node.Nodes.Count == 1) && (e.Node.Nodes[0].Text == c_Loading))
                {
                    e.Node.Nodes.Clear();
                    Member oParentMember = e.Node.Tag as Member;

                    MemberCollection oMembers = oParentMember.GetChildren(0, 150);
                    foreach (Member oMember in oMembers)
                    {
                        TreeNode oMemberNode = e.Node.Nodes.Add(oMember.Caption);
                        oMemberNode.Tag = oMember;
                        oMemberNode.ImageKey = "Member";
                        oMemberNode.SelectedImageKey = "Member";

                        if (oMember.ChildCount > 0)
                            oMemberNode.Nodes.Add(c_Loading);
                    }
                }
            }
        }

    }
}
