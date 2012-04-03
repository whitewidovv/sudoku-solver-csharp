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
using System.Threading;

namespace SudokuSolver
{
    class SolverEngine
    {
        #region Global Variables
      
        private String[,] tableToSolve;
        private List<Tuple<int, int>> fixedPositions;

        private Stack<Tuple<Tuple<int, int>, List<string>>> stepsStack;

        private Solver.Status status;

        private int TABLEWIDTH;
        private int TABLEHEIGHT;
        private int MAXVALUE;

        private Delegate endCallback;

        #endregion


        #region Class Constructor

        public SolverEngine(int rank, String[,] _TableToSolve, List<Tuple<int, int>> _FixedPositions, Delegate _EndCallback)
        {
            tableToSolve = _TableToSolve;
            fixedPositions = _FixedPositions;
            endCallback = _EndCallback;

            TABLEWIDTH = rank;
            TABLEHEIGHT = rank;
            MAXVALUE = rank;
        }

        #endregion


        #region Public Methods

        public void StartSolveTask()
        {
            Thread trd = new Thread(Solve);
            trd.Priority = ThreadPriority.Highest;
            trd.Start();
        }

        #endregion 


        #region Private Methods

        private void Solve()
        {
            bool _continue = true;

            stepsStack = new Stack<Tuple<Tuple<int, int>, List<string>>>(0);

            if (!TableWorker.CheckForCloneNums(tableToSolve, MAXVALUE, TABLEWIDTH, TABLEHEIGHT, TableWorker.SearchDirection.Horizontal) && !TableWorker.CheckForCloneNums(tableToSolve, MAXVALUE, TABLEWIDTH, TABLEHEIGHT, TableWorker.SearchDirection.Vertical))
            {
                do
                {
                    if (stepsStack.Count == 0)
                    {
                       status = InsertNum(0, 0, tableToSolve, fixedPositions);
                    }
                    else
                    {
                        Tuple<Tuple<int, int>, List<string>> step = stepsStack.Peek();
                        tableToSolve.SetValue(String.Empty, step.Item1.Item1, step.Item1.Item2);

                        status = InsertNum(step.Item1.Item1, step.Item1.Item2, tableToSolve, fixedPositions);
                    }

                    switch (status)
                    {
                        case (Solver.Status.stackRollback):
                            _continue = true;
                            break;

                        case (Solver.Status.Solved):
                            _continue = false;
                            break;

                        case (Solver.Status.Error):
                            Result(false);
                            return;                       
                    }
                }
                while (_continue);

                if (TableWorker.CheckResult(tableToSolve, TABLEWIDTH, TABLEHEIGHT)) 
                    Result(true);
                else
                    Result(false);
            }
            else
                Result(false);
        }

        private Solver.Status InsertNum(int start_i, int start_j, String[,] tableToSolve, List<Tuple<int, int>> fixedPositions)
        {
            bool setStart_i = true;
            bool setStart_j = true;

            for (int i = 0; i < TABLEWIDTH; i++)
            {
                if (setStart_i)
                    if (i != start_i) { continue; }
                    else setStart_i = false;

                for (int j = 0; j < TABLEHEIGHT; j++)
                {
                    if (setStart_j)
                        if (j != start_j) { continue; ; }
                        else setStart_j = false;

                    if (!TableWorker.IsFixed(i, j, fixedPositions))
                        if ((string)tableToSolve.GetValue(i, j) == String.Empty)
                        {
                            Tuple<Solver.Status, string> chooseResult = ChooseNum(i, j, tableToSolve, fixedPositions);

                            switch (chooseResult.Item1)
                            {
                                case (Solver.Status.stackRollback):
                                    return Solver.Status.stackRollback;

                                case (Solver.Status.Error):
                                    return Solver.Status.Error;

                                case (Solver.Status.ValueChoosed):
                                    tableToSolve.SetValue(chooseResult.Item2, i, j);
                                    break;
                            }
                        }
                }
            }
            return Solver.Status.Solved;
        }

        private Tuple<Solver.Status, string> ChooseNum(int posToChoose_i, int posToChoose_j, String[,] tableToSolve, List<Tuple<int, int>> fixedPositions)
        {
            List<string> existNums = null;

            if (stepsStack.Count > 0)
            {
                Tuple<Tuple<int, int>, List<string>> lastStep = stepsStack.Peek();
                if ((lastStep.Item1.Item1 == posToChoose_i) && (lastStep.Item1.Item2 == posToChoose_j))
                {
                    existNums = lastStep.Item2;

                    stepsStack.Pop();
                }
                else
                    existNums = TableWorker.GetExistentNums(tableToSolve, TABLEWIDTH, TABLEHEIGHT, posToChoose_i, posToChoose_j);
            }
            else
                existNums = TableWorker.GetExistentNums(tableToSolve, TABLEWIDTH, TABLEHEIGHT, posToChoose_i, posToChoose_j);


            for (int k = 0; k <= MAXVALUE - 1; k++)
            {
                string value = TableWorker.CellsValue[k];

                if (!existNums.Contains(value))
                {
                    existNums.Add(value);
                    existNums.TrimExcess();

                    stepsStack.Push(new Tuple<Tuple<int, int>, List<string>>(
                        new Tuple<int, int>(posToChoose_i, posToChoose_j), existNums));

                    return new Tuple<Solver.Status ,string>(Solver.Status.ValueChoosed, value);
                }
            }

            if (stepsStack.Count > 0)
            {
                tableToSolve.SetValue(String.Empty, posToChoose_i, posToChoose_j);
                return new Tuple<Solver.Status ,string>(Solver.Status.stackRollback, String.Empty);
            }
            else
                return new Tuple<Solver.Status ,string>(Solver.Status.Error, String.Empty);
        }

        private void Result(bool solved)
        {
            if (solved)
            {
                endCallback.DynamicInvoke(tableToSolve, fixedPositions, Solver.Status.Solved);
            }
            else
            {
                endCallback.DynamicInvoke(tableToSolve, fixedPositions, Solver.Status.Error);
            }
        }

        #endregion

    }
}
