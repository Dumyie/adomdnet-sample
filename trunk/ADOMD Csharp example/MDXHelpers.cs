using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AnalysisServices.AdomdClient;
using System.Text.RegularExpressions;

namespace ADOMD_Csharp_example
{
    class MDXHelpers
    {
        private static string mcsQuery = "SELECT \r\n\t{ {0} } ON COLUMNS,\r\n\t{ {1} } ON ROWS \r\nFROM {2} \r\nCELL PROPERTIES VALUE, BACK_COLOR, FORE_COLOR, FORMATTED_VALUE, FORMAT_STRING, FONT_NAME, FONT_SIZE, FONT_FLAGS";

        public static string CreateDefaultQuery(CubeDef cube)
        {
            // find the first time dimension
            Dimension oTimeDim = GetTimeDim(cube);
            Dimension oOtherDim = GetFirstNonTimeDim(cube);

            string oTimeMember = GetAllMember(oTimeDim);
            string oOtherMember = GetAllMember(oOtherDim);

            string sTime = oTimeMember + ".Children";
            string sOther = oOtherMember + ".Children";
            string sCube = cube.Name;
            if (sCube[0] != '[')
                sCube = "[" + sCube + "]";

            string sMDX = mcsQuery.Replace("{0}", sTime);
            sMDX = sMDX.Replace("{1}", sOther);
            sMDX = sMDX.Replace("{2}", sCube);
            //return string.Format(mcsQuery, sTime, sOther, cube.ToString());
            return sMDX;
        }


        private static string GetAllMember(Dimension dim)
        {
            if (dim.Hierarchies.Count > 0)
            {
                foreach (Hierarchy oHier in dim.Hierarchies)
                {
                    if (oHier.HierarchyOrigin != HierarchyOrigin.AttributeHierarchy)
                    {
                        return oHier.DefaultMember;
                    }
                }
                return dim.Hierarchies[0].DefaultMember;
            }
            else
            {
                return  dim.AttributeHierarchies[0].DefaultMember;
            }
        }

        private static Dimension GetTimeDim(CubeDef cube)
        {
            foreach (Dimension oDim in cube.Dimensions)
            {
                if (oDim.DimensionType == DimensionTypeEnum.Time)
                    return oDim;
            }

            return null;
        }

        private static Dimension GetFirstNonTimeDim(CubeDef cube)
        {
            foreach (Dimension oDim in cube.Dimensions)
            {
                if ((oDim.DimensionType != DimensionTypeEnum.Time) 
                    && (oDim.DimensionType != DimensionTypeEnum.Measure))
                    return oDim;
            }

            return null;
        }

        public static string FormatMDX(string mdx)
        {
            string sFormattedMdx = mdx;

            try
            {
                const string NewLine = "\r\n";
                const string Tab = "\t";

                // remove formatting
                sFormattedMdx = sFormattedMdx.Replace(NewLine, " ");
                sFormattedMdx = sFormattedMdx.Replace(Tab, " ");

                string sLowerMdx = mdx.ToLower();
                sLowerMdx = sLowerMdx.Replace(NewLine, " ");
                sLowerMdx = sLowerMdx.Replace(Tab, " ");
                
                // locate select
                int iSelectPos = sLowerMdx.IndexOf("select");

                if (iSelectPos > 0)
                {
                    int iFirstMemberPos = sFormattedMdx.IndexOf("member", 0, StringComparison.InvariantCulture);

                    if (iFirstMemberPos > -1)
                    {
                        int iMemberPos = sFormattedMdx.IndexOf("member", iFirstMemberPos, iSelectPos);
                        while (iMemberPos > -1)
                        {
                            sFormattedMdx = sFormattedMdx.Insert(iMemberPos, NewLine);
                            iSelectPos = sFormattedMdx.IndexOf("select");
                            iMemberPos = sFormattedMdx.IndexOf("member", iMemberPos + 4, iSelectPos);
                        }

                        sFormattedMdx = sFormattedMdx.Insert(iSelectPos, NewLine + NewLine);
                    }
                }

                sFormattedMdx = sFormattedMdx.Replace("select ", "select \n\r");
                sFormattedMdx = sFormattedMdx.Replace("select{", "select \n\r {");
            }
            catch (Exception ex)
            {
                string sMsg = "An error occured while formatting the mdx. \n\r" + ex.Message;

                System.Windows.Forms.MessageBox.Show(sMsg, "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);

                sFormattedMdx = mdx;                
            }

            return sFormattedMdx;
        }
    }

}
