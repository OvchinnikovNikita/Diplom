using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace Diplom;

using System.Collections.Generic;

public static class PolylineApproximation
{
    private static List<Edge> edge1 = new List<Edge>();
    private static List<Edge> edge2 = new List<Edge>();
    private static int polygonsCounter = -1;

    public static void Approximate(List<List<Point>> borders, out List<Point> appBordersVertices, out List<Edge> appBordersEdges)
    {
        appBordersEdges = new List<Edge>();
        appBordersVertices = new List<Point>();
        
        foreach (var border in borders)
        {
            var app = PolylineApproximation.Approximate(border, 1);
            appBordersVertices.AddRange(app);
                
            for (int i = 0; i < app.Count; i++)
            {
                var p1 = app[i];
                var p2 = app[(i + 1) % app.Count];
                appBordersEdges.Add(new Edge(p1, p2));
            }
        }
    }
    public static List<Point> Approximate(List<Point> points, double tolerance)
    {
        polygonsCounter++;
        edge1.Clear();
        edge2.Clear();

        var result = new List<Point> { points[0] };
        Approximate(points, result, 0, points.Count - 1, tolerance);
        result.Add(points[points.Count - 1]);
        
        var res = new HashSet<Point>();
        
        for (int i = 0; i < edge1.Count; i++)
        {
            res.Add(edge1[i].End);
        }
        for (int i = 0; i < edge2.Count; i++)
        {
            res.Add(edge1[i].End);
        }

        res.Add(points[points.Count - 1]);
        return res.ToList();
    }

    private static void Approximate(List<Point> points, List<Point> result, int startIndex, int endIndex, double tolerance)
    {
        double maxDistance = 0;
        int maxDistanceIndex = 0;

        for (int i = startIndex + 1; i < endIndex; i++)
        {
            double distance = DistanceToSegment(points[i], points[startIndex], points[endIndex]);

            if (distance > maxDistance)
            {
                maxDistance = distance;
                maxDistanceIndex = i;
            }
        }
        if (maxDistance >= tolerance)
        {
            result.Add(points[maxDistanceIndex]);
            Approximate(points, result, startIndex, maxDistanceIndex, tolerance);
            edge1.Add(new Edge(points[startIndex], points[maxDistanceIndex], polygonsCounter));
            Approximate(points, result, maxDistanceIndex, endIndex, tolerance);
            edge2.Add(new Edge(points[maxDistanceIndex], points[endIndex], polygonsCounter));
        }
    }

    private static double DistanceToSegment(Point myPoint, Point segmentStart, Point segmentEnd)
    {
        double segmentLength = Math.Sqrt(Math.Pow(segmentEnd.X - segmentStart.X, 2) + Math.Pow(segmentEnd.Y - segmentStart.Y, 2));

        if (segmentLength == 0)
        {
            return Math.Sqrt(Math.Pow(myPoint.X - segmentStart.X, 2) + Math.Pow(myPoint.Y - segmentStart.Y, 2));
        }

        double t = ((myPoint.X - segmentStart.X) * (segmentEnd.X - segmentStart.X) + (myPoint.Y - segmentStart.Y) * (segmentEnd.Y - segmentStart.Y)) / Math.Pow(segmentLength, 2);

        if (t < 0)
        {
            return Math.Sqrt(Math.Pow(myPoint.X - segmentStart.X, 2) + Math.Pow(myPoint.Y - segmentStart.Y, 2));
        }

        if (t > 1)
        {
            return Math.Sqrt(Math.Pow(myPoint.X - segmentEnd.X, 2) + Math.Pow(myPoint.Y - segmentEnd.Y, 2));
        }

        double projectionX = segmentStart.X + t * (segmentEnd.X - segmentStart.X);
        double projectionY = segmentStart.Y + t * (segmentEnd.Y - segmentStart.Y);

        return Math.Sqrt(Math.Pow(myPoint.X - projectionX, 2) + Math.Pow(myPoint.Y - projectionY, 2));
    }
}
