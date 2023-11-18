using System;
using System.Collections.Generic;
using System.Numerics;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using Graph_3_lab.Helpers;
using ScottPlot;

namespace Graph_3_lab.Models;

public static class Projection
{
    public static List<double[,]>? UpdateProjectionMatrix(TextBox tbSx, TextBox tbSy, TextBox tbSz, TextBox tbD)
        {
            if (tbSx.Text.IsEmptyOrWhiteSpace() || tbSy.Text.IsEmptyOrWhiteSpace() || tbSz.Text.IsEmptyOrWhiteSpace() ||
                tbD.Text.IsEmptyOrWhiteSpace()) return null;
            double sx = Convert.ToDouble(tbSx.Text) /* значение sx из интерфейса */;
            double sy = Convert.ToDouble(tbSy.Text) /* значение sy из интерфейса */;
            double sz = Convert.ToDouble(tbSz.Text) /* значение sz из интерфейса */;
            double D = Convert.ToDouble(tbD.Text)   /* значение D из интерфейса */;

            // Рассчитываем углы ψx, ψy, ψz, λ
            CalculateAnglesFromVector(sx, sy, sz, out double psix, out double psiy, out double psiz, out double lam);
            // Создаем матрицу проецирования
            double[,] m1 = CalculateProjectionMatrixTop(sx, sy, sz, lam, D, psix, psiy, psiz);
            double[,] m2 = CalculateProjectionMatrixFront(sx, sy, sz, lam, D, psix, psiy, psiz);
            double[,] m3 = CalculateProjectionMatrixSide(sx, sy, sz, lam, D, psix, psiy, psiz);
            double[,] m4 = CalculateProjectionMatrix(sx, sy, sz, lam, D, psix, psiy, psiz);
            var list = new List<double[,]>
            {
                m1, m2, m3, m4
            };
            return list;
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

        public static double[,] CalculateProjectionMatrixTop(double sx, double sy, double sz, double lam, double D,
            double psix, double psiy, double psiz)
        {
            var matrix = new double[4, 4];

            // Рассчитываем элементы матрицы проецирования для вида сверху
            matrix[0, 0] = sx * Math.Cos(psix);
            matrix[0, 1] = sx * Math.Sin(psix) * Math.Sin(lam) - sy * Math.Cos(lam);
            matrix[0, 2] = sx * Math.Sin(psix) * Math.Cos(lam) + sy * Math.Sin(lam);
            matrix[0, 3] = -D * Math.Sin(psix);

            matrix[1, 0] = sy * Math.Cos(psix);
            matrix[1, 1] = sy * Math.Sin(psix) * Math.Sin(lam) + sx * Math.Cos(lam);
            matrix[1, 2] = sy * Math.Sin(psix) * Math.Cos(lam) - sx * Math.Sin(lam);
            matrix[1, 3] = -D * Math.Cos(psix);

            matrix[2, 0] = 0;
            matrix[2, 1] = -sz * Math.Cos(lam);
            matrix[2, 2] = -sz * Math.Sin(lam);
            matrix[2, 3] = 0;

            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;

            return matrix;
        }

        public static double[,] CalculateProjectionMatrixFront(double sx, double sy, double sz, double lam, double D, double psix, double psiy, double psiz)
        {
            var matrix = new double[4, 4];

            // Рассчитываем элементы матрицы проецирования для вида спереди
            matrix[0, 0] = sx * Math.Cos(psiy) * Math.Cos(lam) - sy * Math.Sin(lam);
            matrix[0, 1] = -sx * Math.Sin(psiy);
            matrix[0, 2] = sx * Math.Cos(psiy) * Math.Sin(lam) + sy * Math.Cos(lam);
            matrix[0, 3] = -D * Math.Cos(psiy);

            matrix[1, 0] = sy * Math.Cos(psiy) * Math.Cos(lam) + sx * Math.Sin(lam);
            matrix[1, 1] = -sy * Math.Sin(psiy);
            matrix[1, 2] = sy * Math.Cos(psiy) * Math.Sin(lam) - sx * Math.Cos(lam);
            matrix[1, 3] = -D * Math.Sin(psiy);

            matrix[2, 0] = 0;
            matrix[2, 1] = sz * Math.Cos(lam);
            matrix[2, 2] = -sz * Math.Sin(lam);
            matrix[2, 3] = 0;

            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;

            return matrix;
        }
        
        public static double[,] CalculateProjectionMatrixSide(double sx, double sy, double sz, double lam, double D, double psix, double psiy, double psiz)
        {
            var matrix = new double[4, 4];

            // Рассчитываем элементы матрицы проецирования для вида сбоку
            matrix[0, 0] = sx * Math.Cos(psiy);
            matrix[0, 1] = -sx * Math.Sin(psiy) * Math.Sin(lam) + sy * Math.Cos(lam);
            matrix[0, 2] = sx * Math.Sin(psiy) * Math.Cos(lam) + sy * Math.Sin(lam);
            matrix[0, 3] = -D * Math.Sin(psiy);

            matrix[1, 0] = sy * Math.Cos(psiy);
            matrix[1, 1] = -sy * Math.Sin(psiy) * Math.Sin(lam) - sx * Math.Cos(lam);
            matrix[1, 2] = sy * Math.Sin(psiy) * Math.Cos(lam) - sx * Math.Sin(lam);
            matrix[1, 3] = -D * Math.Cos(psiy);

            matrix[2, 0] = 0;
            matrix[2, 1] = sz * Math.Sin(lam);
            matrix[2, 2] = sz * Math.Cos(lam);
            matrix[2, 3] = 0;

            matrix[3, 0] = 0;
            matrix[3, 1] = 0;
            matrix[3, 2] = 0;
            matrix[3, 3] = 1;

            return matrix;
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
        
        [Obsolete("Obsolete")]
        public static void DisplayProjections(List<Point3D> vertices, double[,] projectionMatrixXY,double[,] projectionMatrixXZ,double[,] projectionMatrixYZ, WpfPlot plot1, WpfPlot plot2, WpfPlot plot3)
        {
            if (vertices.Count == 0)
                return;

            // Проецируем вершины на плоскости XZ, YZ, XY и показываем на графиках ScottPlot

            double[] xValuesXY = new double[vertices.Count];
            double[] yValuesXY = new double[vertices.Count];

            double[] xValuesXZ = new double[vertices.Count];
            double[] yValuesXZ = new double[vertices.Count];

            double[] xValuesYZ = new double[vertices.Count];
            double[] yValuesYZ = new double[vertices.Count];
            for (int i = 0; i < vertices.Count; i++)
            {
                // Применяем матрицу проецирования к вершине для плоскости XY
                Vector4 vertex = new Vector4((float)vertices[i].X, (float)vertices[i].Y, (float)vertices[i].Z, 1);
                Vector4 projectedVertexXY = Vector4.Transform(vertex, new Matrix4x4(
                    (float)projectionMatrixXY[0, 0], (float)projectionMatrixXY[0, 1], (float)projectionMatrixXY[0, 2],
                    (float)projectionMatrixXY[0, 3],
                    (float)projectionMatrixXY[1, 0], (float)projectionMatrixXY[1, 1], (float)projectionMatrixXY[1, 2],
                    (float)projectionMatrixXY[1, 3],
                    (float)projectionMatrixXY[2, 0], (float)projectionMatrixXY[2, 1], (float)projectionMatrixXY[2, 2],
                    (float)projectionMatrixXY[2, 3],
                    (float)projectionMatrixXY[3, 0], (float)projectionMatrixXY[3, 1], (float)projectionMatrixXY[3, 2],
                    (float)projectionMatrixXY[3, 3]
                ));

                // Применяем матрицу проецирования к вершине для плоскости XZ
                Vector4 projectedVertexXZ = Vector4.Transform(vertex, new Matrix4x4(
                    (float)projectionMatrixXZ[0, 0], (float)projectionMatrixXZ[0, 1], (float)projectionMatrixXZ[0, 2],
                    (float)projectionMatrixXZ[0, 3],
                    (float)projectionMatrixXZ[1, 0], (float)projectionMatrixXZ[1, 1], (float)projectionMatrixXZ[1, 2],
                    (float)projectionMatrixXZ[1, 3],
                    (float)projectionMatrixXZ[2, 0], (float)projectionMatrixXZ[2, 1], (float)projectionMatrixXZ[2, 2],
                    (float)projectionMatrixXZ[2, 3],
                    (float)projectionMatrixXZ[3, 0], (float)projectionMatrixXZ[3, 1], (float)projectionMatrixXZ[3, 2],
                    (float)projectionMatrixXZ[3, 3]
                ));

                // Применяем матрицу проецирования к вершине для плоскости YZ
                Vector4 projectedVertexYZ = Vector4.Transform(vertex, new Matrix4x4(
                    (float)projectionMatrixYZ[0, 0], (float)projectionMatrixYZ[0, 1], (float)projectionMatrixYZ[0, 2],
                    (float)projectionMatrixYZ[0, 3],
                    (float)projectionMatrixYZ[1, 0], (float)projectionMatrixYZ[1, 1], (float)projectionMatrixYZ[1, 2],
                    (float)projectionMatrixYZ[1, 3],
                    (float)projectionMatrixYZ[2, 0], (float)projectionMatrixYZ[2, 1], (float)projectionMatrixYZ[2, 2],
                    (float)projectionMatrixYZ[2, 3],
                    (float)projectionMatrixYZ[3, 0], (float)projectionMatrixYZ[3, 1], (float)projectionMatrixYZ[3, 2],
                    (float)projectionMatrixYZ[3, 3]
                ));
                
                xValuesXY[i] = projectedVertexXY.X;
                yValuesXY[i] = projectedVertexXY.Y;

                xValuesXZ[i] = projectedVertexXZ.X;
                yValuesXZ[i] = projectedVertexXZ.Z;

                xValuesYZ[i] = projectedVertexYZ.Y;
                yValuesYZ[i] = projectedVertexYZ.Z;
            }
            plot1.Plot.PlotScatter(xValuesXY, yValuesXY); //above
            plot2.Plot.PlotScatter(xValuesXZ, yValuesXZ); //front
            plot3.Plot.PlotScatter(xValuesYZ, yValuesYZ); //right

            plot1.Render();
            plot2.Render();
            plot3.Render();
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