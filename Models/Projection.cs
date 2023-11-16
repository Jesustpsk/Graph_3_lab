using System;
using System.Windows.Controls;
using Graph_3_lab.Helpers;

namespace Graph_3_lab.Models;

public static class Projection
{
    public static double[,] UpdateProjectionMatrix(TextBox tbSx, TextBox tbSy, TextBox tbSz, TextBox tbD)
        {
            if (tbSx.Text.IsEmptyOrWhiteSpace() || tbSy.Text.IsEmptyOrWhiteSpace() || tbSz.Text.IsEmptyOrWhiteSpace() ||
                tbD.Text.IsEmptyOrWhiteSpace()) return new double[1,1]{{0}};
            double sx = Convert.ToDouble(tbSx.Text) /* значение sx из интерфейса */;
            double sy = Convert.ToDouble(tbSy.Text) /* значение sy из интерфейса */;
            double sz = Convert.ToDouble(tbSz.Text) /* значение sz из интерфейса */;
            double D = Convert.ToDouble(tbD.Text)   /* значение D из интерфейса */;

            // Рассчитываем углы ψx, ψy, ψz, λ
            CalculateAnglesFromVector(sx, sy, sz, out double psix, out double psiy, out double psiz, out double lam);
            // Создаем матрицу проецирования
            return CalculateProjectionMatrix(sx, sy, sz, lam, D, psix, psiy, psiz);
        }

    public static void DisplayProjectionMatrix(TextBox textBox, double[,] projectionMatrix)
        {
            // Отобразить матрицу в TextBox
            textBox.Text = "Projection Matrix:\n" +
                           $"[{RoundUp(Convert.ToDecimal(projectionMatrix[0,0]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[0,1]), 1)}, " +
                           $"{RoundUp(Convert.ToDecimal(projectionMatrix[0,2]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[0,3]), 1)}]\n" +
                           $"[{RoundUp(Convert.ToDecimal(projectionMatrix[1,0]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[1,1]), 1)}, " +
                           $"{RoundUp(Convert.ToDecimal(projectionMatrix[1,2]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[1,3]), 1)}]\n" +
                           $"[{RoundUp(Convert.ToDecimal(projectionMatrix[2,0]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[2,1]), 1)}, " +
                           $"{RoundUp(Convert.ToDecimal(projectionMatrix[2,2]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[2,3]), 1)}]\n" +
                           $"[{RoundUp(Convert.ToDecimal(projectionMatrix[3,0]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[3,1]), 1)}, " +
                           $"{RoundUp(Convert.ToDecimal(projectionMatrix[3,2]), 1)}, {RoundUp(Convert.ToDecimal(projectionMatrix[3,3]), 1)}]";
        }
        
        private static double[,] CalculateProjectionMatrix(double sx, double sy, double sz, double lam, double D, double psix, double psiy, double psiz)
        {
            var matrix = new double[4,4];

            // Рассчитываем элементы матрицы проецирования
            matrix[0,0] = sx * Math.Cos(lam);
            matrix[0,1] = -sy * Math.Sin(psix) * Math.Sin(lam);
            matrix[0,2] = -sz * Math.Cos(psiy) * Math.Sin(lam);
            matrix[0,3] = -D * Math.Sin(lam);

            matrix[1,0] = sy * Math.Sin(psix);
            matrix[1,1] = sx * Math.Cos(psix) * Math.Cos(lam);
            matrix[1,2] = -sz * Math.Sin(psiy) * Math.Cos(lam);
            matrix[1,3] = -D * Math.Cos(psix) * Math.Cos(lam);

            matrix[2,0] = sz * Math.Sin(psiy);
            matrix[2,1] = sz * Math.Cos(psiy);
            matrix[2,2] = -sz * Math.Cos(psiy) * Math.Sin(psiy);
            matrix[2,3] = -D * Math.Sin(psiy);

            matrix[3,0] = 0;
            matrix[3,1] = 0;
            matrix[3,2] = 0;
            matrix[3,3] = 1;

            return matrix;
        }
        
        private static void CalculateAnglesFromVector(double sx, double sy, double sz, out double psix, out double psiy, out double psiz, out double lam)
        {
            // Рассчитываем угол λ
            lam = Math.Atan(sz / Math.Sqrt(sx * sx + sy * sy));

            // Рассчитываем углы ψx, ψy, ψz
            psix = Math.Acos(sx / Math.Sqrt(sx * sx + sy * sy));
            psiy = Math.Acos(sy / Math.Sqrt(sx * sx + sy * sy));
            psiz = 0; 
        }
        private static decimal RoundUp(decimal number, int digits)
        {
            var factor = Convert.ToDecimal(Math.Pow(10, digits));
            return Math.Ceiling(number * factor) / factor;
        }
}