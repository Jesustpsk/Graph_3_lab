using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Graph_3_lab.Models;

public static class Visual3d {
    public static void DrawCustomShape(Viewport3D MyView, List<Point3D> customShapePoints)
        {
            // Создаем объект Model3DGroup для хранения 3D моделей
            var myModel3DGroup = new Model3DGroup();

            // Создаем материал для поверхности фигуры
            var myMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Green));

            // Создаем геометрию фигуры
            var customShapeGeometry = CreateCustomShapeGeometry(customShapePoints);

            // Создаем геометрический объект GeometryModel3D для отображения фигуры
            var customShapeModel = new GeometryModel3D(customShapeGeometry, myMaterial);

            // Добавляем объект GeometryModel3D к Model3DGroup
            myModel3DGroup.Children.Add(customShapeModel);

            // Создаем 3D вьюпорт
            var mainModelVisual3D = new ModelVisual3D
            {
                Content = myModel3DGroup
            };

            // Добавляем 3D вьюпорт к элементу Viewport3D на форме
            MyView.Children.Add(mainModelVisual3D);

            // Добавляем источник света
            var myDirLight = new DirectionalLight
            {
                Color = Colors.White,
                Direction = new Vector3D(-1, -1, -1)
            };

            myModel3DGroup.Children.Add(myDirLight);
        }

    private static MeshGeometry3D CreateCustomShapeGeometry(List<Point3D> points)
    {
        var mesh = new MeshGeometry3D();

        // Добавляем точки в геометрию
        foreach (var point in points)
        {
            mesh.Positions.Add(point);
        }

        // Добавляем индексы для формирования треугольников
        if (points.Count < 3) return mesh;
        for (var i = 1; i < points.Count - 1; i++)
        {
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(i);
            mesh.TriangleIndices.Add(i + 1);
        }

        return mesh;
    }
}