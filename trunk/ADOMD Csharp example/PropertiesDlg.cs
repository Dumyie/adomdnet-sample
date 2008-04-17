using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace ADOMD_Csharp_example
{
    public partial class PropertiesDlg : Form
    {
        public PropertiesDlg(object adomdObject)
        {
            InitializeComponent();

            propertyGrid1.SelectedObject = adomdObject;

            FillTree(adomdObject);

        }

        private void FillTree(object adomdObject)
        {
            foreach(PropertyInfo pInfo in adomdObject.GetType().GetProperties())
            {
                try
                {
                    string sName = pInfo.Name;
                    object  sValue = pInfo.GetValue(adomdObject, null);
                    TreeNode node = treeView1.Nodes.Add(sName + " = " + sValue.ToString());
                    if ((sName != "ParentCube"  ) && (!IsBaseType(sValue))) 
                        LoadProperties(sValue, node);
                      
                }
                catch
                { }
            }
            //adomdObject.GetType().GetProperty().GetValue(
            //System.Reflection.PropertyInfo 
        }

        private bool IsBaseType(object o)
        {
            string sTypeName = o.GetType().Name;

            if (sTypeName == "String")
                return true;
            else if (sTypeName == "Int16")
                return true;
            else if (sTypeName == "DateTime")
                return true;
            else if (sTypeName == "Int32")
                return true;
            else if (sTypeName == "Int64")
                return true;
            else if (sTypeName == "Double")
                return true;
            else if (sTypeName == "Boolean")
                return true;
            else if (sTypeName == "DataRowCollection")
                return true;

            return false;
        }

        private void LoadProperties(object adomdObject, TreeNode node)
        {
            PropertyInfo[] pCol = adomdObject.GetType().GetProperties();
            if ((pCol.Length == 2))
            {
                if (pCol[1].Name == "Length")
                    return;
            }
            foreach (PropertyInfo pInfo in adomdObject.GetType().GetProperties())
            {
                try
                {
                    string sName = pInfo.Name;
                    object sValue = pInfo.GetValue(adomdObject, null);

                    TreeNode aNode = node.Nodes.Add(sName + " = " + sValue.ToString());
                    if (!IsBaseType(sValue))
                        LoadProperties(sValue, aNode);
                }
                catch 
                {
                    
                }


            }
        }
    }
}