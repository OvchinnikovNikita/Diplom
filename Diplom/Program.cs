using System;
using System.IO;
using Point = System.Drawing.Point;

namespace Diplom
{
    internal class Program
    {
        static void Main()
        {
            var image = ImageLoader.Load($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..\\Images\\TestCases\\Test1.png");
            // var image = ImageLoader.Load($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..\\Images\\TestCases\\Test2.png");
            // var image = ImageLoader.Load($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..\\Images\\TestCases\\Test3.jpg");

            // Установить начальную и конечную точки. Для каждого примера они, разумеется, свои.
            // Примеры для первого (большого) тестового изображения
            // var pStart = new Point(188, 493);
            // var pEnd = new Point(1310, 266);
            
            // var pStart = new Point(177, 587);
            // var pEnd = new Point(229, 590);

            // var pStart = new Point(177, 587);
            // var pEnd = new Point(1686, 179);
            
            var pStart = new Point(589, 781);
            var pEnd = new Point(1809, 53);

            Console.WriteLine("Я начал считать");
            
            var analyzer = new ImageAnalyzer(image);
            var borders = analyzer.GetBorders();
            var colors = analyzer.GetColors();
            
            PolylineApproximation.Approximate(borders, out var appBordersVertices, out var appBordersEdges);
            
            appBordersVertices.Add(pStart);
            appBordersVertices.Add(pEnd);
            
            var graph = VisibilityGraphBuilder.BuildVisibilityGraph(appBordersVertices, appBordersEdges, colors);
            var way = DijkstrasAlgorithm.GetShortestRoute(graph, appBordersVertices, pStart, pEnd);
            var painter = new ImagePainter(image, borders, graph, appBordersVertices, way, pStart, pEnd);
            
            Console.WriteLine("Я закончил считать");
            
            painter.PaintImage();
            
            Console.WriteLine("Я сохранил изображение");
        }
    }
}