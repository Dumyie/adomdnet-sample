

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SourceGrid3;
using Microsoft.AnalysisServices.AdomdClient;

namespace ADOMD_Csharp_example
{
    /// <summary>
    /// This is an implementation of an Adomd Grid using the SourceGrid grid control.
    /// The SourceGrid control is found at http://www.devage.com/
    /// Adomd data is loaded into the control thru the LoadData method, which can take an
    /// Adomd.CellSet, a DataTable, or a AdomdDataReader.  The prefered way is to use an Adomd.CellSet
    /// </summary>
    public partial class AdomdGrid : SourceGrid3.Grid
    {
        private CellSet moSet = null;
        private DataTable moTable = null;

        public AdomdGrid()
        {
            InitializeComponent();

            
        }

        #region Helper methods
        /// <summary>
        /// Removes all data and cells from the grid.
        /// </summary>
        private void ClearGrid()
        {
            this.Rows.Clear();
            this.Columns.Clear();
            this.ColumnsCount = 0;
            this.RowsCount = 0;
        }

        /// <summary>
        /// Sets a header value on either the columns or rows.
        /// </summary>
        private void SetHeaderValue(int row, int col, string text)
        {
            this[row, col] = new SourceGrid3.Cells.Real.Header(text);
        }

        private SourceGrid3.Cells.Real.Cell SetCellValue(int row, int col, string value)
        {
            SourceGrid3.Cells.Real.Cell gridCell = new SourceGrid3.Cells.Real.Cell(value);
            this[row, col] = gridCell;
            return gridCell;
        }
        #endregion

        #region LoadData
        public void LoadData(DataTable dt)
        {
            moSet = null;
            moTable = dt;

            this.ClearGrid();

            this.ColumnsCount = dt.Columns.Count;
            this.RowsCount = 1;

            // set the column headers
            for (int i = 0; i<dt.Columns.Count; i++)
            {
                DataColumn oColumn = dt.Columns[i];
                SetHeaderValue(0, i, oColumn.Caption);
            }

            int iRow = 1;
            this.RowsCount = 1;
            foreach(DataRow oRow in dt.Rows)
            {
                this.RowsCount++;
                for (int iCol = 0; iCol < dt.Columns.Count; iCol++)
                {
                    string sValue = oRow[iCol].ToString();
                    SetCellValue(iRow, iCol, sValue);
                    //this[iRow, iCol] = new SourceGrid3.Cells.Real.Cell(sValue);
                }
                iRow++;
            }
        }

        /// <summary>
        /// Loads data into the grid using a Adomd.CellSet.
        /// </summary>
        /// <param name="set"></param>
        public void LoadData(CellSet set)
        {
            moTable = null;
            moSet = set;

            ClearGrid();

            // find the number of dimensions on the Rows and columns
            int iColDims = set.OlapInfo.AxesInfo.Axes[0].Hierarchies.Count;
            int iRowDims = 0;
            bool bHasRows = false;
            if (set.OlapInfo.AxesInfo.Axes.Count > 1)
            {
                iRowDims = set.OlapInfo.AxesInfo.Axes[1].Hierarchies.Count;
                bHasRows = true;
            }

            // find the number of data rows and columns needed
            int iNumCols = set.Axes[0].Set.Tuples.Count;

            int iNumRows = 1;
            if (bHasRows)
                iNumRows = set.Axes[1].Set.Tuples.Count;

            // figure out how many rows and columns we need
            // to display all the data and headers
            this.ColumnsCount = iRowDims + iNumCols;
            this.RowsCount = iColDims + iNumRows;

            // Set all of the column headers
            for (int iCol = 0; iCol < iNumCols; iCol++)
            {
                for (int iMember = 0; iMember < iColDims; iMember++)
                {
                    string sName = set.Axes[0].Positions[iCol].Members[iMember].Caption;
                    SetHeaderValue(iMember, iCol + iRowDims, sName);
                }
            }

            if (bHasRows)
            {
                // set all of the row headers
                for (int i = 0; i < iRowDims; i++)
                {
                    this.Rows[i].AutoSizeMode = SourceGrid3.AutoSizeMode.EnableAutoSize | SourceGrid3.AutoSizeMode.EnableStretch;
                }
                for (int iRow = 0; iRow < iNumRows; iRow++)
                {
                    for (int iMember = 0; iMember < iRowDims; iMember++)
                    {
                        string sName = set.Axes[1].Positions[iRow].Members[iMember].Caption;
                        SetHeaderValue(iRow + iColDims, iMember, sName);
                    }
                }
            }

            int iCurRow = iColDims;
            int iCurCol = iRowDims;
            int iDataCol = 0;

            // set the data in the grid1
            for(int iCell= 0; iCell<set.Cells.Count; iCell++)
            {
                Cell oCell = null ;
                string sValue = "";
                try
                {
                    oCell = set.Cells[iCell];
                    sValue = oCell.FormattedValue;
                }
                catch (Exception ex)
                {
                    sValue = ex.Message;
                }
                SourceGrid3.Cells.Real.Cell gridCell = SetCellValue(iCurRow, iCurCol, sValue);

                CheckCellProperties(oCell, gridCell); 

                iDataCol++;
                iCurCol++;

                if (iDataCol >= iNumCols)
                {
                    iDataCol = 0;
                    iCurCol = iRowDims;
                    iCurRow++;
                }
            }

            this.AutoStretchColumnsToFitWidth = true;
            this.AutoSize();
            this.Columns.StretchToFit();
        }

        //private void CheckCellProperties(Cell myCell, int row, int col)
        private void CheckCellProperties(Cell cubeCell, SourceGrid3.Cells.Real.Cell gridCell)
        {
            Color backColor = Color.White;
            Color foreColor = Color.Black;

            foreach (CellProperty prop in cubeCell.CellProperties)
            {
                if ((prop.Name == "BACK_COLOR") && (prop.Value != null))
                {
                    uint iColor = (uint)prop.Value;
                    int i = (int) iColor;
                    Color c = Color.FromArgb(  i);

                    backColor = Color.FromArgb(c.B, c.G, c.R); 
                }
                else if ((prop.Name == "FORE_COLOR") && (prop.Value != null))
                {
                    uint iColor = (uint)prop.Value;
                    int i = (int)iColor;
                    Color c = Color.FromArgb(i);

                    foreColor = Color.FromArgb(c.B, c.G, c.R);
                }

                SourceGrid3.Cells.Views.Cell view = new SourceGrid3.Cells.Views.Cell();
                view.BackColor = backColor;
                view.ForeColor = foreColor;

                gridCell.View = view;
            }
        }

        public void LoadData(AdomdDataReader reader)
        {
            ClearGrid();
            
            int iColCount = reader.FieldCount;
            this.ColumnsCount = iColCount;
            this.RowsCount = 1;

            // set the column headers
            for (int i = 0; i < iColCount; i++)
            {
                string sColumn = reader.GetName(i);

                SetHeaderValue(0, i, sColumn);
            }

            int iRow = 1;
            this.RowsCount = 1;
            while (reader.Read())
            {
                this.RowsCount++;
                for(int iCol=0; iCol < iColCount; iCol++)
                {
                    if (!reader.IsDBNull(iCol))
                    {
                        string sValue = reader.GetValue(iCol).ToString();
                        SetCellValue(iRow, iCol, sValue);
                    }
                    else
                    {
                        SetCellValue(iRow, iCol, "");
                    }
                }
                iRow++;
            }
        }

        #endregion

        public void CheckCellData(int row, int col)
        {
            if (moSet != null)
            {
                int iCellIndex = GridToCellSetIndex(row, col);
                if (iCellIndex > -1)
                {
                    Cell oCell = GetCell(iCellIndex);

                    

                    PropertiesDlg dlg = new PropertiesDlg(oCell);
                    dlg.Show();
                }
                else
                {
                    MessageBox.Show("Unable to find cell");
                }
            }
        }

        public int GridToCellSetIndex(int row, int col)
        {
            if (moSet != null)
            {
                // get some CellSet information
                CellSetInfo csInfo = GetCellSetInfo();
                int iCell = (row-1) * (csInfo.NumCols); 

                return iCell + (col-1);
            }

            return -1;
        }

        public Cell GetCell(int index)
        {
            return moSet[index];
        }

        private struct CellSetInfo
        {
            public int ColDims;
            public int RowDims;

            public bool HasRows;

            public int NumCols;
            public int NumRows;
        }

        private CellSetInfo GetCellSetInfo()
        {
            CellSetInfo csInfo = new CellSetInfo();

            // find the number of dimensions on the Rows and columns
            csInfo.ColDims = moSet.OlapInfo.AxesInfo.Axes[0].Hierarchies.Count;
            csInfo.RowDims = 0;
            csInfo.HasRows = false;
            if (moSet.OlapInfo.AxesInfo.Axes.Count > 1)
            {
                csInfo.RowDims = moSet.OlapInfo.AxesInfo.Axes[1].Hierarchies.Count;
                csInfo.HasRows = true;
            }

            // find the number of data rows and columns needed
            csInfo.NumCols = moSet.Axes[0].Set.Tuples.Count;

            csInfo.NumRows = 1;
            if (csInfo.HasRows)
                csInfo.NumRows = moSet.Axes[1].Set.Tuples.Count;

            return csInfo;
        }


        
    }
}
