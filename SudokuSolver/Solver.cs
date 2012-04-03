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
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace SudokuSolver
{
    static class Solver
    {
        #region Global Variables

        private static TextBox[,] cells;
        private static formMain frmMain;
        private static Delegate endCallBack;

        #endregion


        #region Public Method

        public static void SolverMain(formMain _frmMain, TextBox[,] _cells, Delegate _endCallback)
        {
            frmMain = _frmMain;
            cells = _cells;
            endCallBack = _endCallback;

            Tuple<String[,], List<Tuple<int, int>>> solveParam = PrepareToSolve();

            SolverEngine newSolver = new SolverEngine(Program.MAXVALUE, solveParam.Item1, solveParam.Item2, new SolveResultDlg(SolveResult));
            newSolver.StartSolveTask();
        }

        #endregion


        #region Results

        private delegate void SolveResultDlg(String[,] tableToSolve, List<Tuple<int, int>> fixedPositions, Status status);
        private static void SolveResult(String[,] tableToSolve, List<Tuple<int, int>> fixedPositions, Status status)
        {
            Task resultTask = new Task(ShowResult, new object[] { tableToSolve, fixedPositions, status });
            resultTask.Start();
        }

        private static void ShowResult(object state)
        {
            List<Tuple<TextBox, Tuple<int, int>>> cellsList = TextBoxArrayToList();

            Status status = (Solver.Status)(((object[])state)[2]);          
            String[,] tableToSolve = (String[,])(((object[])state)[0]);
            List<Tuple<int, int>> fixedPositions = (List<Tuple<int, int>>)(((object[])state)[1]);

            Random rnd = new Random();
            int index = cellsList.Count;

            do
            {
                int listIndc = rnd.Next(index);
                Tuple<TextBox, Tuple<int, int>> txtToShow = cellsList[listIndc];
                cellsList.RemoveAt(listIndc);

                if (status != Solver.Status.Error)
                {
                  IAsyncResult utbIAR = frmMain.BeginInvoke(new UpdateTextBoxDlg(UpdateTextBox), txtToShow, tableToSolve, fixedPositions);
                  frmMain.EndInvoke(utbIAR);
                }

                System.Threading.Thread.Sleep(10);

                index--;

            } while (index > 0);

            endCallBack.DynamicInvoke(status);
        }

        private delegate void UpdateTextBoxDlg(Tuple<TextBox, Tuple<int, int>> txtToShow, String[,] tableToSolve, List<Tuple<int, int>> fixedPositions);
        private static void UpdateTextBox(Tuple<TextBox, Tuple<int, int>> txtToShow, String[,] tableToSolve, List<Tuple<int, int>> fixedPositions)
        {
            if (TableWorker.IsFixed(txtToShow.Item2.Item1, txtToShow.Item2.Item2, fixedPositions))
            {
                using (Font cellsFixedFont = new Font(txtToShow.Item1.Font.Name, txtToShow.Item1.Font.Size, FontStyle.Bold))
                {
                    txtToShow.Item1.Font = cellsFixedFont;
                    txtToShow.Item1.BackColor = Color.LightGray;
                }
            }
            else
            {
                using (Font cellsFixedFont = new Font(txtToShow.Item1.Font.Name, txtToShow.Item1.Font.Size, FontStyle.Regular))
                {
                    txtToShow.Item1.Text = tableToSolve[txtToShow.Item2.Item1, txtToShow.Item2.Item2];
                }
            }

            txtToShow.Item1.Update();   
        }

        #endregion


        #region Support

        private static Tuple<String[,], List<Tuple<int, int>>> PrepareToSolve()
        {
            String[,] tableToSolve = new String[Program.TABLEWIDTH, Program.TABLEHEIGHT];
            List<Tuple<int, int>> fixedPositions = new List<Tuple<int, int>>(0);

            for (int i = 0; i < Program.TABLEWIDTH; i++)
                for (int j = 0; j < Program.TABLEHEIGHT; j++)
                {
                    if (cells[i, j].Text != String.Empty)
                    {
                        fixedPositions.Add(new Tuple<int, int>(i, j));
                    }

                    tableToSolve.SetValue(cells[i, j].Text, i, j);
                }

            fixedPositions.TrimExcess();

            return new Tuple<string[,], List<Tuple<int, int>>>(tableToSolve, fixedPositions);
        }

        private static List<Tuple<TextBox, Tuple<int, int>>> TextBoxArrayToList()
        {
            List<Tuple<TextBox, Tuple<int, int>>> cellsList = new List<Tuple<TextBox, Tuple<int, int>>>(81);

            for (int i = 0; i < Program.TABLEWIDTH; i++)
                for (int j = 0; j < Program.TABLEHEIGHT; j++)
                {
                    cellsList.Add(new Tuple<TextBox, Tuple<int, int>>(cells[i, j], new Tuple<int, int>(i, j)));
                }

            return cellsList;
        }

        #endregion


        public enum Status
        {
            Start = 1,
            Solved = 2,
            Error = 3,
            ValueChoosed = 4,
            stackRollback = 5,
        }
    }
}
