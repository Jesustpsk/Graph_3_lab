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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Graph_3_lab.Models;
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
        public static List<Point3D> _figure = new();
        public MainWindow()
        {
            ProjMatrix = new double[4, 4];
            InitializeComponent();
            Display.SetAxis(helixViewport, PlotAbove.Plot, PlotFront.Plot, PlotRight.Plot);
            // Включение вращения мышью
            helixViewport.RotateGesture = new MouseGesture(MouseAction.LeftClick);
        }

        private void BtnUpload_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|Object Files (*.obj)|*.obj|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() != true) return;
            objectFilePath = openFileDialog.FileName;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            LoadObjectFromFile(objectFilePath);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            //UpdateProjectionMatrix();
            //DisplayProjectionMatrix();
            Display.CreateFigure(_figure, meshVisual, helixViewport);
        }
        private void BtnProj_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
        
        private void LoadObjectFromFile(string filePath)
        {
            _figure.Clear();
            meshVisual.Children.Clear();
            helixViewport.Children.Clear();
            Display.SetAxis(helixViewport, PlotAbove.Plot, PlotFront.Plot, PlotRight.Plot);
            // Включение вращения мышью
            helixViewport.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            // Установка камеры
            helixViewport.CameraController.ZoomExtents();
            using var reader = new StreamReader(filePath);
            var dots = reader.ReadToEnd();
            if (dots == "") return;
            _figure = Display.CreateList3D(dots);
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