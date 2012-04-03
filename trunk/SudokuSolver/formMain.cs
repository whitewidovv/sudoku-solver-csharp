/*

    Copyright (C) <2012>  <Fuoritempo>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using System;
using System.Windows.Forms;
using System.Drawing;

namespace SudokuSolver
{
    public partial class formMain : Form
    {
        #region Global Variables

        TextBox[,] cells;

        int cellsRank = 0;

        #endregion


        #region Class Constructor

        public formMain()
        {
            InitializeComponent();
            BuildSudokuTable();
        }

        #endregion


        #region Button Events

        private void buttonStart_Click(object sender, EventArgs e)
        {
            SetApplication(Solver.Status.Start);

            Solver.SolverMain(this, cells, new EndCallbackDlg(EndCallback));
        }

        #endregion


        #region TextBoxes Events

        private void cell_TextChanged(object sender, EventArgs e)
        {
            int result;
            if ((!Int32.TryParse(((TextBox)sender).Text, out result)) || (((TextBox)sender).Text == "0"))
                ((TextBox)sender).Text = String.Empty;
        }

        #endregion


        #region Menu Strip

        private void solveNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetCells();

            buttonStart.Enabled = true;

            toolStripStatusLabel1.Text = "Ready...";
        }

        private void aboutSudokuSolverToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formCredits frmCredits = new formCredits();
            frmCredits.ShowDialog();
        }

        #endregion


        #region Support

        private void BuildSudokuTable()
        {
            cells = new TextBox[Program.TABLEWIDTH, Program.TABLEHEIGHT];

            int yCellOffset = 1;
            int xCellOffset = 1;

            int hMargin = 2;
            int vMargin = 0;

            int hlineCellsCounter = 1;
            int vLineCellsCounter = 1;

            for (int i = 0; i < Program.TABLEWIDTH; i++)
            {
                for (int j = 0; j < Program.TABLEHEIGHT; j++)
                {
                    TextBox cell = new TextBox();

                    cell.Parent = panelSudokuTable;
                    cell.BorderStyle = BorderStyle.FixedSingle;
                    cell.Location = new Point(40 * (xCellOffset - 1) + hMargin - 1, 30 * (yCellOffset - 1) + vMargin + 1);
                    cell.Width = 40;
                    cell.Height = 0;
                    cell.TextAlign = HorizontalAlignment.Center;
                    cell.MaxLength = 1;
                    cell.Font = Program.cellsFont;
                    cell.TextChanged += new EventHandler(cell_TextChanged);
                    cell.Show();

                    cells[i, j] = cell;

                    if (hlineCellsCounter % Program.CELLRANKGROUP == 0)
                        hMargin = hMargin + 2;

                    hlineCellsCounter++;
                    xCellOffset++;
                }
                if (vLineCellsCounter % Program.CELLRANKGROUP == 0)
                    vMargin = vMargin + 1;

                vLineCellsCounter++;
                hlineCellsCounter = 1;
                xCellOffset = 1;
                yCellOffset++;
                hMargin = 2;
            }
            cellsRank = Program.MAXVALUE;

            panelSudokuTable.Width = cells[Program.TABLEWIDTH - 1, Program.TABLEWIDTH - 1].Right + 1;
            panelSudokuTable.Height = cells[Program.TABLEHEIGHT - 1, Program.TABLEHEIGHT - 1].Bottom + 1;

            this.Width = panelSudokuTable.Width + 62;
            this.Height = panelSudokuTable.Height + 188;

            panelSudokuTable.Location = new Point(((groupBox1.Width - panelSudokuTable.Width) / 2),
            ((groupBox1.Height - panelSudokuTable.Height) / 2) + 3 );
        }

        private void ResetCells()
        {
            if (cellsRank != 0)
            {
                for (int i = 0; i < cellsRank; i++)
                    for (int j = 0; j < cellsRank; j++)
                    {
                        using (Font cellsDefaultFont = new Font(cells[i, j].Font.Name, cells[i, j].Font.Size, FontStyle.Regular))
                        {
                            cells[i, j].Text = String.Empty;
                            cells[i, j].BackColor = Color.White;
                            cells[i, j].Font = cellsDefaultFont;
                        }
                    }
            }
        }

        private delegate void SetApplicationDlg(Solver.Status appStatus);
        private void SetApplication(Solver.Status appStatus)
        {
            switch (appStatus)
            {
                case (Solver.Status.Start):
                    toolStripStatusLabel1.Text = "Searching...";
                     panelSudokuTable.Enabled = false;
                     solveToolStripMenuItem.Enabled = false;
                     buttonStart.Enabled = false;
                    break;

                case(Solver.Status.Solved):
                    toolStripStatusLabel1.Text = "Solved!";
                    panelSudokuTable.Enabled = true;    
                    solveToolStripMenuItem.Enabled = true;
                    break;

                case(Solver.Status.Error):
                    toolStripStatusLabel1.Text = "Error!";
                    panelSudokuTable.Enabled = true;   
                    solveToolStripMenuItem.Enabled = true;
                    buttonStart.Enabled = true;
                    break;
            }
        }

        private delegate void EndCallbackDlg(Solver.Status appStatus);
        private void EndCallback(Solver.Status appStatus)
        {
            this.BeginInvoke(new SetApplicationDlg(SetApplication), appStatus);
        }

        #endregion
    }
}
