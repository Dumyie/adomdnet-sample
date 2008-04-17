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
    public partial class AdomdFunctionsTree : TreeView 
    {
        private AdomdConnection moConn;
        
        public AdomdFunctionsTree()
        {
            InitializeComponent();
        }

        public AdomdConnection Connection
        {
            get { return moConn; }
            set
            {
                moConn = value;
                LoadData();
            }
        }

        public void LoadData()
        {
            this.Nodes.Clear();
            if (moConn == null) return;

            DataSet ds = GetFunctions();
            DataTable dt = ds.Tables[0];

            foreach (DataRow row in dt.Rows)
            {
                string sInterface = row["INTERFACE_NAME"].ToString();
                string sCaption = row["CAPTION"].ToString();
                string sFunction = row["FUNCTION_NAME"].ToString();
                string sObject = row["OBJECT"].ToString();
                string sDesc = row["DESCRIPTION"].ToString();
                string sParams = row["PARAMETER_LIST"].ToString();
                FunctionType eFtype = FunctionType.Function;
                
                string sDragValue = sFunction;
                if ((sParams != null) && (sParams.Length > 0))
                    sDragValue += "("+sParams+")";
                
                if ((sObject != null) && (sObject.Length > 0))
                    sDragValue = sObject + "." + sDragValue;
                else
                    eFtype = FunctionType.Property;

                LoadValue(eFtype, sInterface, sCaption, sDesc, sDragValue);
            }
        }



        private DataSet GetFunctions()
        {
            DataSet ds = moConn.GetSchemaDataSet(AdomdSchemaGuid.Functions, new object[0]);

            return ds;
        }

        private void LoadValue(FunctionType fType, string interfaceName, string caption, string description, string dragValue)
        {
            TreeNode oInterfaceNode = GetInterfaceNode(interfaceName);

            TreeNode oFunctionNode = new TreeNode(caption);
            FunctionData oData = new FunctionData(fType, interfaceName, caption, description, dragValue);
            oFunctionNode.Tag = oData;
            if (fType == FunctionType.Function)
            {
                oFunctionNode.ImageKey = "Function";
                oFunctionNode.SelectedImageKey = "Function";
            }
            else
            {
                oFunctionNode.ImageKey = "Property";
                oFunctionNode.SelectedImageKey = "Property";
            }

            oInterfaceNode.Nodes.Add(oFunctionNode);
        }

        private TreeNode GetInterfaceNode(string interfaceName)
        {
            TreeNode[] oNodes = this.Nodes.Find(interfaceName, false);
            if (oNodes.Length > 0)
            {
                return oNodes[0];
            }

            TreeNode oNode = this.Nodes.Add(interfaceName, interfaceName);
            
            oNode.ImageKey = "Group";
            oNode.SelectedImageKey = "Group";
            return oNode;
        }

    }

    public enum FunctionType
    {
        Property,
        Function
    }

    public class FunctionData
    {
        private string msInterfaceName;
        private string msCaption;
        private string msDescription;
        private string msDragValue;
        private FunctionType meFunctionType;

        public FunctionData(FunctionType fType, string interfaceName, string caption, string description, string dragValue)
        {
            meFunctionType = fType;    
            msInterfaceName = interfaceName;
            msCaption = caption;
            msDescription = description;
            msDragValue = dragValue;
        }

        public FunctionType FunctionType
        {
            get { return meFunctionType; }
        }

        public string InterfaceName
        {
            get { return msInterfaceName; }
        }

        public string Caption
        {
            get { return msCaption; }
        }

        public string Description
        {
            get { return msDescription; }
        }

        public string DragValue
        {
            get { return msDragValue; }
        }


    }
}
