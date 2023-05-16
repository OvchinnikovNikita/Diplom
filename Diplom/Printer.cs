using System;
using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public static class Printer
{
    public static void Print(IEnumerable<IEnumerable<int>> enumerable)
    {
        foreach (var elements in enumerable)
        {
            foreach (var element in elements)
            {
                Console.Write(element + " ");
            }

            Console.WriteLine();
        }
    }

    public static void Print(IEnumerable<Edge> enumerable)
    {
        foreach (var element in enumerable)
        {
            Console.Write($"({element.Start.X},{element.Start.Y}) - ({element.End.X},{element.End.Y}); ");
        }

        Console.WriteLine();
    }

    public static void Print(IEnumerable<IEnumerable<Point>> enumerable)
    {
        foreach (var elements in enumerable)
        {
            foreach (var element in elements)
            {
                Console.Write($"({element.X}, {element.Y})" + " ");
            }

            Console.WriteLine();
        }
    }

    public static void Print(IEnumerable<Point> elements)
    {
        foreach (var element in elements)
        {
            Console.Write($"({element.X}, {element.Y})" + " ");
        }

        Console.WriteLine();
    }
}