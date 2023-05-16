using System;
using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public static class DijkstrasAlgorithm
{
    public static List<int> GetShortestRoute(double[,] adjacencyMatrix, int s, int end)
    {
        // Инициализация
        var U = new HashSet<double>();
        var vertexCount = adjacencyMatrix.GetLength(0);
        var shortestWays = new List<double>(vertexCount);
        var ways = new int[vertexCount];

        ways[s] = -1;
        
        for (var i = 0; i < vertexCount; i++)
        {
            shortestWays.Add(double.PositiveInfinity);
        }

        shortestWays[s] = 0;
        
        // Шаг
        while (U.Count != vertexCount)
        {
            int u = -1;
            var min = double.MaxValue;
            
            for (int i = 0; i < shortestWays.Count; i++)
            {
                if (!U.Contains(i) && shortestWays[i] < min)
                {
                    min = shortestWays[i];
                    u = i;
                }
            }
            
            if (u == -1) throw new Exception("u = -1");
            U.Add(u);
            
            for (int i = 0; i < vertexCount; i++)
            {
                if (adjacencyMatrix[u, i] != 0)
                {
                    var tryValue = shortestWays[u] + adjacencyMatrix[u, i];
                    
                    if (shortestWays[i] > tryValue)
                    {
                        shortestWays[i] = tryValue;
                        ways[i] = u;
                    }
                }
            }

        }

        return RestorePath(ways, end);
    }
    
    public static List<Point> GetShortestRoute(List<Edge> edges, List<Point> vertices, Point start, Point end)
    {
        var result = new List<Point>();
        var adjacencyMatrix = GetAdjacencyMatrix(edges, vertices, start, end, out var startIndex, 
            out var endIndex, out var vertexNumbers);
        var way = GetShortestRoute(adjacencyMatrix, startIndex, endIndex);

        for (int i = 0; i < way.Count; i++)
        {
            foreach (var pair in vertexNumbers)
            {
                if (pair.Value == way[i]) result.Add(pair.Key);
            }
        }

        return result;
    }

    private static void TryToAddVertex(Point p, HashSet<Point> visitedVertices, Dictionary<Point, int> vertexNumbers, ref int c)
    {
        if (!visitedVertices.Contains(p))
        {
            visitedVertices.Add(p);
            vertexNumbers.Add(p, c);
            c++;
        }
    }
    
    private static double[,] GetAdjacencyMatrix(List<Edge> edges, List<Point> vertices, Point start, 
        Point end, out int startIndex, out int endIndex, out Dictionary<Point, int> vertexNumbers)
    {
        var edgesCount = edges.Count;
        var result = new double[vertices.Count, vertices.Count];
        var visitedVertices = new HashSet<Point>();
        var c = 0;
        vertexNumbers = new Dictionary<Point, int>();

        for (int i = 0; i < edgesCount; i++)
        {
            var edge = edges[i];
            TryToAddVertex(edge.Start, visitedVertices, vertexNumbers, ref c);
            TryToAddVertex(edge.End, visitedVertices, vertexNumbers, ref c);
        }

        for (int i = 0; i < edgesCount; i++)
        {
            var edge = edges[i];
            result[vertexNumbers[edge.Start], vertexNumbers[edge.End]] = edge.EdgeLength;
        }

        startIndex = vertexNumbers[start];
        endIndex = vertexNumbers[end];
        return result;
    }
    
    private static List<int> RestorePath(int[] ways, int p)
    {
        var current = ways[p];
        var res = new List<int>();
        res.Add(p);
        
        while (current != -1)
        {
            res.Add(current);
            current = ways[current];
        }

        res.Reverse();
        return res;
    }
}