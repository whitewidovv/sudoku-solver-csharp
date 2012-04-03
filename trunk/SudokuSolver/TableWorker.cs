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

namespace SudokuSolver
{
    class TableWorker
    {
        public static bool CheckResult(String[,] tableToSolve, int tableWidth, int tableHeight)
        {
            for (int i = 0; i < tableWidth; i++)
            {
                for (int j = 0; j < tableHeight; j++)
                {
                    if ((string)tableToSolve.GetValue(i, j) == String.Empty)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static bool IsFixed(int pos_i, int pos_j, List<Tuple<int, int>> fixedPositions)
        {
            foreach (Tuple<int, int> fixedPos in fixedPositions)
            {
                if ((fixedPos.Item1 == pos_i) && (fixedPos.Item2 == pos_j)) return true;
            }
            return false;
        }

        public static List<string> GetExistentNums(String[,] tableToSolve, int tableWidth, int tableHeight, int pos_i, int pos_j)
        {
            List<string> existNums = new List<string>(9);

            for (int i = 0; i < tableWidth; i++)
            {
                if ((string)tableToSolve.GetValue(i, pos_j) != String.Empty)
                    if (!existNums.Contains((string)tableToSolve.GetValue(i, pos_j)))
                        existNums.Add((string)tableToSolve.GetValue(i, pos_j));
            }

            for (int j = 0; j < tableHeight; j++)
            {
                if ((string)tableToSolve.GetValue(pos_i, j) != String.Empty)
                    if (!existNums.Contains((string)tableToSolve.GetValue(pos_i, j)))
                        existNums.Add((string)tableToSolve.GetValue(pos_i, j));
            }

            return existNums;
        }

        public static bool CheckForCloneNums(String[,] tableToSolve, int rank, int tableWidth, int tableHeight, SearchDirection direction)
        {
            int count;

            for (int i = 0; i < tableWidth; i++)
            {
                for (int k = 1; k <= rank; k++)
                {
                    count = 0;

                    for (int j = 0; j < tableHeight; j++)
                    {
                        string value;

                        if (direction == SearchDirection.Vertical)
                            value = (string)tableToSolve.GetValue(i, j);
                        else
                            value = (string)tableToSolve.GetValue(j, i);

                        if (value == k.ToString()) count++;

                        if (count > 1) return true;
                    }
                }
            }

            return false;
        }

        public enum SearchDirection
        {
            Vertical = 0,
            Horizontal = 1,
        }

        public static List<string> CellsValue = new List<string> 
        { 
            "1", "2", "3", "4", "5", "6", "7", "8", "9"   
        };
    }
}
