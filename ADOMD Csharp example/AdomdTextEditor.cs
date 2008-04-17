using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Puzzle.Windows.Forms;

namespace ADOMD_Csharp_example
{
    public partial class AdomdTextEditor : System.Windows.Forms.UserControl
    {
        SyntaxBoxControl txtCtrl;
        private Puzzle.SourceCode.SyntaxDocument syntaxDocument1;

        public AdomdTextEditor()
        {

            InitializeComponent();
            this.AllowDrop = true;
            InitializeTextEditor(); 
        }

        public AdomdTextEditor(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

            InitializeTextEditor();
        }

        private void InitializeTextEditor()
        {
            txtCtrl = new SyntaxBoxControl();
            this.Controls.Add(txtCtrl);

            syntaxDocument1 = new Puzzle.SourceCode.SyntaxDocument();

            txtCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            txtCtrl.Document = syntaxDocument1;
            
            syntaxDocument1.Lines = new string[] {""};
            syntaxDocument1.MaxUndoBufferSize = 1000;
            syntaxDocument1.Modified = false;
            syntaxDocument1.UndoStep = 0;

            syntaxDocument1.SyntaxFile = "MSOLAP.syn";
            //syntaxDocument1.Text = "SELECT \n{  } on ROWS, \n{  } on COLUMNS \nFROM [Cube]";

            txtCtrl.ActiveView = Puzzle.Windows.Forms.ActiveView.BottomRight;
            txtCtrl.AllowBreakPoints = false;
            txtCtrl.AllowDrop = true;
            txtCtrl.AutoListPosition = null;
            txtCtrl.AutoListSelectedText = "a123";
            txtCtrl.AutoListVisible = false;
            txtCtrl.BackColor = System.Drawing.Color.White;
            txtCtrl.BorderStyle = Puzzle.Windows.Forms.BorderStyle.None;
            txtCtrl.CopyAsRTF = false;
            txtCtrl.Document = this.syntaxDocument1;
            txtCtrl.FontName = "Courier new";
            txtCtrl.GutterMarginWidth = 0;
            txtCtrl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            txtCtrl.InfoTipCount = 1;
            txtCtrl.InfoTipPosition = null;
            txtCtrl.InfoTipSelectedIndex = 1;
            txtCtrl.InfoTipVisible = false;
            txtCtrl.LineNumberBackColor = System.Drawing.SystemColors.GrayText;
            txtCtrl.LineNumberBorderColor = System.Drawing.SystemColors.ControlText;
            txtCtrl.LineNumberForeColor = System.Drawing.SystemColors.ControlText;
            txtCtrl.Location = new System.Drawing.Point(210, 12);
            txtCtrl.LockCursorUpdate = false;
            txtCtrl.Name = "syntaxBoxControl1";
            txtCtrl.ShowGutterMargin = false;
            txtCtrl.ShowScopeIndicator = false;
            txtCtrl.Size = new System.Drawing.Size(397, 373);
            txtCtrl.SmoothScroll = true;
            txtCtrl.SplitviewH = -4;
            txtCtrl.SplitviewV = -4;
            txtCtrl.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(243)))), ((int)(((byte)(234)))));
            txtCtrl.TabIndex = 0;
            txtCtrl.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            
            //txtCtrl.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(txtCtrl_QueryContinueDrag);
            //txtCtrl.DragEnter += new System.Windows.Forms.DragEventHandler(txtCtrl_DragEnter);
            //txtCtrl.DragDrop += new System.Windows.Forms.DragEventHandler(txtCtrl_DragDrop);
            //txtCtrl.DragOver += new System.Windows.Forms.DragEventHandler(txtCtrl_DragOver);

            
        }

        public override string Text
        {
            get { return syntaxDocument1.Text; }
            set { syntaxDocument1.Text = value; }
        }

        public override System.Windows.Forms.ContextMenu ContextMenu
        {
            get { return txtCtrl.ContextMenu; }
            set { txtCtrl.ContextMenu = value; }
        }

        public override System.Windows.Forms.ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return txtCtrl.ContextMenuStrip;
            }
            set
            {
                txtCtrl.ContextMenuStrip = value;
            }
        }

        public object BaseControl
        {
            get { return txtCtrl; }
        }

        public void Undo()
        {
            syntaxDocument1.Undo();
        }

        public void Redo()
        {
            syntaxDocument1.Redo();
        }

        public void Copy()
        {
            txtCtrl.Copy();
        }

        public void Cut()
        {
            txtCtrl.Cut();
        }

        public void Paste()
        {
            txtCtrl.Paste();
        }

        public void SelectAll()
        {
            txtCtrl.SelectAll();
        }

        
        public string SelectedText
        {
            get
            {
                return txtCtrl.Selection.Text;
            }
        }


    }
}
