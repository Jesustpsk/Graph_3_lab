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
            
            Display.CreateFigure(_figure, meshVisual, helixViewport);
        }
        private void BtnProj_OnClick(object sender, RoutedEventArgs e)
        {
            ProjMatrix = Projection.UpdateProjectionMatrix(tbSx, tbSy, tbSz, tbD);
            Projection.DisplayProjectionMatrix(tbMatrix, ProjMatrix);
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

        
    }
}