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
using System.Drawing;
using System.Windows.Forms;

namespace SudokuSolver
{
    static class Program
    {
        #region Global Variables

        public static int TABLEWIDTH = 9;     
        public static int TABLEHEIGHT = 9;
        public static int CELLRANKGROUP = 3;
        public static int MAXVALUE = 9;

        public static Font cellsFont = new Font("Arial", 11, FontStyle.Regular);

        #endregion

        /// <summary>
        /// Punto di ingresso principale dell'applicazione.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new formMain());
        }
    }
}
