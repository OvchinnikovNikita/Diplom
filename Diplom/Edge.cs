using System;
using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public class Edge : IEquatable<Edge>
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public int PolygonsMark { get; }
    public int Id { get; set; }
    public double EdgeLength { get; }

    public Edge(Point start, Point end, int mark = -1)
    {
        Start = start;
        End = end;
        PolygonsMark = mark;
        EdgeLength = Math.Sqrt((End.X - Start.X)*(End.X - Start.X) + (End.Y - Start.Y)*(End.Y - Start.Y));
    }

    static int Orientation(Point p, Point q, Point r)
    {
        double val = (q.Y - p.Y) * (r.X - q.X) - (q.X - p.X) * (r.Y - q.Y);
        if (val == 0) return 0;
        return (val > 0) ? 1 : 2;
    }

    static bool OnSegment(Point p, Point q, Point r)
    {
        if (q.X <= Math.Max(p.X, r.X) && q.X >= Math.Min(p.X, r.X) &&
            q.Y <= Math.Max(p.Y, r.Y) && q.Y >= Math.Min(p.Y, r.Y))
            return true;
        return false;
    }

    public static List<Edge> GetIncidentEdges(Point vertex, List<Edge> edges)
    {
        var result = new List<Edge>();

        foreach (var edge in edges)
        {
            if (vertex.Equals(edge.Start) || vertex.Equals(edge.End))
            {
                result.Add(edge);
            }
        }

        return result;
    }
    public static bool Intersect(Edge e1, Edge e2)
    {
        Point p1 = e1.Start, q1 = e1.End, p2 = e2.Start, q2 = e2.End;

        int o1 = Orientation(p1, q1, p2);
        int o2 = Orientation(p1, q1, q2);
        int o3 = Orientation(p2, q2, p1);
        int o4 = Orientation(p2, q2, q1);

        if (o1 != o2 && o3 != o4)
        {
            return true;
        }

        if (o1 == 0 && OnSegment(p1, p2, q1))
        {
            return true;
        }

        if (o2 == 0 && OnSegment(p1, q2, q1))
        {
            return true;
        }

        if (o3 == 0 && OnSegment(p2, p1, q2))
        {
            return true;
        }

        if (o4 == 0 && OnSegment(p2, q1, q2))
        {
            return true;
        }

        return false;
    }

    public static bool IntervalIntersects(Edge e1, Edge e2)
    {
        // Проверяем, что начальная точка текущего интервала находится между начальной и конечной точками другого интервала
        bool startInside = (e1.Start.CompareTo(e2.Start) >= 0 && e1.Start.CompareTo(e2.End) <= 0);
        // Проверяем, что конечная точка текущего интервала находится между начальной и конечной точками другого интервала
        bool endInside = (e1.End.CompareTo(e2.Start) >= 0 && e1.End.CompareTo(e2.End) <= 0);

        // Если хотя бы одна из точек текущего интервала находится внутри другого интервала, то они пересекаются
        return startInside || endInside;
    }

    public static (Edge ray1, Edge ray2) GetPerpendicularRays(Edge p)
    {
        // Вычисляем середину отрезка
        double centerX = (p.Start.X + p.End.X) / 2;
        double centerY = (p.Start.Y + p.End.Y) / 2;
        Point midPoint = new Point((int)centerX, (int)centerY);
        
        // Находим угол наклона отрезка
        double angle = Math.Atan2(p.End.Y - p.Start.Y, p.End.X - p.Start.X);

        // Строим первый перпендикулярный луч
        double angle1 = angle + Math.PI / 2;
        Point p1 = new Point((int)(midPoint.X + 10_000 * Math.Cos(angle1)),
            (int)(midPoint.Y + 10_000 * Math.Sin(angle1)));
        var ray1 = new Edge(midPoint, p1);

        // Строим второй перпендикулярный луч
        double angle2 = angle - Math.PI / 2;
        var p2 = new Point((int)(midPoint.X + 10_000 * Math.Cos(angle2)),
            (int)(midPoint.Y + 10_000 * Math.Sin(angle2)));
        var ray2 = new Edge(midPoint, p2);

        return (ray1, ray2);
    }

    public bool Equals(Edge? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return (Start.Equals(other.Start) || Start.Equals(other.End)) && (End.Equals(other.End) || End.Equals(other.Start));
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Edge)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Start, End, PolygonsMark);
    }

    public static List<Point> GetAdjacentVertices(List<Edge> edges, Point p)
    {
        var result = new List<Point>();
        
        foreach (var edge in edges)
        {
            if (edge.Start.Equals(p))
            {
                result.Add(edge.End);
            }
            else if (edge.End.Equals(p))
            {
                result.Add(edge.Start);
            }
        }

        return result;
    }
}