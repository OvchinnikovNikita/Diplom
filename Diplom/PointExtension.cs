using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public static class PointExtension
{
    public static Dictionary<Point, int> Marks { get; set; } = new Dictionary<Point, int>();
    public static int GetMark(this Point p) => Marks[p];

    public static void SetMark(this Point p, int value)
    {
        if (!Marks.ContainsKey(p))
        {
            Marks.Add(p, value);
        }

        Marks[p] = value;
    }

    public static int CompareTo(this Point p1, Point p2)
    {
        // Сравниваем координаты X текущей точки с координатами X другой точки
        int result = p1.X.CompareTo(p2.X);
        if (result == 0)
        {
            // Если координаты X совпадают, сравниваем координаты Y
            result = p1.Y.CompareTo(p2.Y);
        }

        return result;
    }
}