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
using Graph_3_lab.Helpers;
using Graph_3_lab.Models;
using HelixToolkit.Wpf;
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
        private double[,] ProjMatrixAbove;
        private double[,] ProjMatrixRight;
        private double[,] ProjMatrixFront;
        private static List<Point3D> vertices = new();
        public static List<Point3D> _figure = new();
        public MainWindow()
        {
            ProjMatrixAbove = new double[4, 4];
            ProjMatrixRight = new double[4, 4];
            ProjMatrixFront = new double[4, 4];
            InitializeComponent();
            Display.SetAxis(helixViewport, PlotAbove, PlotFront, PlotRight);
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
            
            vertices = Display.CreateFigure(_figure, meshVisual, helixViewport);
        }
        [Obsolete("Obsolete")]
        private void BtnProj_OnClick(object sender, RoutedEventArgs e)
        {
            PlotAbove.Plot.Clear();
            PlotRight.Plot.Clear();
            PlotFront.Plot.Clear();
            Display.SetPlotAxis(PlotAbove, PlotRight, PlotFront);
            ProjMatrixAbove = Projection.UpdateProjectionMatrix(tbSx, tbSy, tbSz, tbD)![0];
            ProjMatrixFront = Projection.UpdateProjectionMatrix(tbSx, tbSy, tbSz, tbD)![1];
            ProjMatrixRight = Projection.UpdateProjectionMatrix(tbSx, tbSy, tbSz, tbD)![2];
            Projection.DisplayProjectionMatrix(tbMatrix, ProjMatrixAbove);
            Projection.DisplayProjections(vertices, ProjMatrixAbove, ProjMatrixFront, ProjMatrixRight, PlotAbove, PlotFront, PlotRight);
            
            /*var projectionMatrix = Projection.UpdateProjectionMatrix(tbSx, tbSy, tbSz, tbD)![3];

            // Создание матрицы вида
            var viewMatrix = new Matrix3D(
                projectionMatrix[0, 0], projectionMatrix[0, 1], projectionMatrix[0, 2], projectionMatrix[0, 3],
                projectionMatrix[1, 0], projectionMatrix[1, 1], projectionMatrix[1, 2], projectionMatrix[1, 3],
                projectionMatrix[2, 0], projectionMatrix[2, 1], projectionMatrix[2, 2], projectionMatrix[2, 3],
                projectionMatrix[3, 0], projectionMatrix[3, 1], projectionMatrix[3, 2], projectionMatrix[3, 3]);

            // Создание камеры и применение матрицы вида
            var camera = new PerspectiveCamera
            {
                Transform = new MatrixTransform3D(viewMatrix)
            };
            // Установка камеры в HelixViewport3D
            helixViewport.Camera = camera;*/
        }
        
        private void LoadObjectFromFile(string filePath)
        {
            _figure.Clear();
            vertices.Clear();
            meshVisual.Children.Clear();
            helixViewport.Children.Clear();
            Display.SetAxis(helixViewport, PlotAbove, PlotFront, PlotRight);
            // Включение вращения мышью
            helixViewport.RotateGesture = new MouseGesture(MouseAction.LeftClick);
            // Установка камеры
            helixViewport.CameraController.ZoomExtents();
            using var reader = new StreamReader(filePath);
            var dots = reader.ReadToEnd();
            if (dots == "") return;
            _figure = Display.CreateList3D(dots);
        }

        
    }
}