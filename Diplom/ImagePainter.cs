using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Diplom;

public class ImagePainter
{
    public Image Image { get; }
    public List<Edge> Graph { get; }
    public List<Point> Vertices { get; }
    public List<Point> Way { get; }
    public Point Start { get; }
    public Point End { get; }
    public List<List<Point>> Borders { get; }

    public ImagePainter(Image image, List<List<Point>> borders, List<Edge> graph, List<Point> vertices, List<Point> way, Point start, Point end)
    {
        Image = image;
        Graph = graph;
        Vertices = vertices;
        Way = way;
        Start = start;
        End = end;
        Borders = borders;
    }

    public void PaintImage()
    {
        using (var bitmap = new Bitmap(Image))
        {
            var gr = Graphics.FromImage(bitmap);

            // Рисуем граф
            foreach (var edge in Graph)
            {
                gr.DrawLine(Pens.Blue, edge.Start, edge.End);
            }
            
            // Рисуем границу
            foreach (var border in Borders)
            {
                foreach (var point in border)
                    bitmap.SetPixel(point.X, point.Y, Color.Blue);
            }
            
            //Отмечаем точки апроксимации
            foreach (var point in Vertices)
            {
                bitmap.SetPixel(point.X, point.Y, Color.Red);
            }

            // Рисуем кратчайший маршрут
            var pred = Way[0];
            for (int i = 1; i < Way.Count; i++)
            {
                var p = Way[i];
                gr.DrawLine(Pens.Red, pred, p);
                pred = p;
            }

            // Отмечаем стартовую и конечную точку
            bitmap.SetPixel(Start.X, Start.Y, Color.Green);
            bitmap.SetPixel(End.X, End.Y, Color.Green);
            
            // Сохраняем изображение 
            bitmap.Save($"{Directory.GetCurrentDirectory()}\\..\\..\\..\\..\\Images\\result.png");
        }
    }
}