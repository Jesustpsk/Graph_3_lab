using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using Color = System.Drawing.Color;
namespace Graph_3_lab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string objectFilePath;
        private double[,] ProjMatrix;
        private static List<List<Point>> figure = new();
        public MainWindow()
        {
            ProjMatrix = new double[4, 4];
            InitializeComponent();
            PlotAbove.Plot.AddVerticalLine(x: 0, color: Color.Black, width: 1);
            PlotAbove.Plot.AddHorizontalLine(y: 0, color: Color.Black, width: 1);
            Plot3D.Plot.AddVerticalLine(x: 0, color: Color.Black, width: 1);
            Plot3D.Plot.AddHorizontalLine(y: 0, color: Color.Black, width: 1);
            PlotFront.Plot.AddVerticalLine(x: 0, color: Color.Black, width: 1);
            PlotFront.Plot.AddHorizontalLine(y: 0, color: Color.Black, width: 1);
            PlotRight.Plot.AddVerticalLine(x: 0, color: Color.Black, width: 1);
            PlotRight.Plot.AddHorizontalLine(y: 0, color: Color.Black, width: 1);
        }

        private void BtnUpload_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Object Files (*.obj)|*.obj|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != true) return;
            objectFilePath = openFileDialog.FileName;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadObjectFromFile(objectFilePath);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //UpdateProjectionMatrix();
            //DisplayProjectionMatrix();
            DisplayProjections();
        }
        private void BtnProj_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void LoadObjectFromFile(string filePath)
        {
            figure.Clear();
            using var reader = new StreamReader(filePath);
            var text = reader.ReadToEnd();
            if (text == "") return;
            
            var textArr = text.Split('\n');
            foreach (var t1 in textArr)
            {
                var temp = t1.Split(" ");
                var tempPoints = temp.Select(t => new Point(Convert.ToInt32(t.Split(';')[0]), Convert.ToInt32(t.Split(';')[1]))).ToList();
                figure.Add(tempPoints);
            }
        }

        private void UpdateProjectionMatrix()
        {
            double sx = Convert.ToDouble(tbSx.Text) /* значение sx из интерфейса */;
            double sy = Convert.ToDouble(tbSy.Text) /* значение sy из интерфейса */;
            double sz = Convert.ToDouble(tbSz.Text) /* значение sz из интерфейса */;
            double D = Convert.ToDouble(tbD.Text)   /* значение D из интерфейса */;

            // Рассчитываем углы ψx, ψy, ψz (для примера, можно использовать нули)
            double psix = 0;
            double psiy = 0;
            double psiz = 0;

            // Рассчитываем угол λ (для примера, можно использовать нуль)
            double lam = 0;

            // Создаем матрицу проецирования
            ProjMatrix = CalculateProjectionMatrix(sx, sy, sz, lam, D, psix, psiy, psiz);
        }

        private void DisplayProjectionMatrix()
        {
            // Отобразить матрицу в TextBox
            //MatrixTextBox.Text = "Projection Matrix:\n" + projectionMatrix.ToString();
        }

        private void DisplayProjections()
        {
            Plot3D.Plot.Clear();
            foreach (var t in figure)
            {
                for (var j = 1; j < t.Count; j++)
                {
                    Plot3D.Plot.AddLine(t[j - 1].X, t[j - 1].Y, t[j].X, t[j].Y,
                        Color.Red);
                }
            }
        }
        
        private double[,] CalculateProjectionMatrix(double sx, double sy, double sz, double lam, double D, double psix, double psiy, double psiz)
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

        
    }
}