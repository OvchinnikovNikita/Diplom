using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public class VisibilityGraphBuilder
{
    private static List<List<int>> Colors = new();
    public static List<Edge> BuildVisibilityGraph(List<Point> vertices, List<Edge> edges, List<List<int>> colors)
    {
        Colors = colors;
        var result = new List<Edge>();
        
        foreach (var vertex in vertices)
        {
            var visibleVertices = GetVisibleVertices(vertex, vertices, edges);

            foreach (var visibleVertex in visibleVertices)
            {
                result.Add(new Edge(vertex, visibleVertex));
            }
        }
        
        Colors.Clear();
        return result;
    }

    private static List<Point> GetVisibleVertices(Point vertex, List<Point> vertices, List<Edge> edges)
    {
        var result = new List<Point>();

        for (int i = 0; i < vertices.Count; i++)
        {
            var w = vertices[i];

            if (IsVisible(vertex, w, edges))
                result.Add(w);
        }

        return result;
    }

    private static bool IsVisible(Point p, Point w, List<Edge> edges)
    {
        var pw = new Edge(p, w);

        // Проверим, смежные ли вершины pw. Если да - вернём true
        foreach (var edge in edges)
        {
            if (edge.Start.Equals(pw.Start) && edge.End.Equals(pw.End) ||
                edge.Start.Equals(pw.End) && edge.End.Equals(pw.Start))
                return true;
        }

        // Исключим те, которые соединяют полигон изнутри
        if (IsCrossesInteriorOfPolygon(pw, edges))
            return false;

        // Проверим пересечения с любымы ребрами
        foreach (var edge in edges)
        {
            if (edge != null && Edge.Intersect(pw, edge))
            {
                if (!pw.Start.Equals(edge.End) && !pw.Start.Equals(edge.Start) && !pw.End.Equals(edge.Start) &&
                    !pw.End.Equals(edge.End))
                    return false;
            }
        }

        return true;
    }

    private static bool IsCrossesInteriorOfPolygon(Edge edge, List<Edge> edges)
    {
        // Вычисляем середину отрезка
        double centerX = (edge.Start.X + edge.End.X) / 2;
        double centerY = (edge.Start.Y + edge.End.Y) / 2;
        Point midPoint = new Point((int)centerX, (int)centerY);
        
        if (Colors[midPoint.Y][midPoint.X] == 0) 
            return true;
        
        return false;
    }
}