using System.Collections.Generic;
using System.Drawing;

namespace Diplom;

public class EdgeComparer : IComparer<Edge>
{
    private Point p;

    public EdgeComparer(Point p)
    {
        this.p = p;
    }

    public int Compare(Edge e1, Edge e2)
    {
        // Рассчитываем координаты точек пересечения ребер с лучом
        float x1, x2;
        if (e1.End.Y - e1.Start.Y != 0)
        {
            x1 = e1.Start.X + (p.Y - e1.Start.Y) * (e1.End.X - e1.Start.X) / (e1.End.Y - e1.Start.Y);
        }
        else
        {
            x1 = float.PositiveInfinity; 
        }

        if (e2.End.Y - e2.Start.Y != 0)
        {
            x2 = e2.Start.X + (p.Y - e2.Start.Y) * (e2.End.X - e2.Start.X) / (e2.End.Y - e2.Start.Y);
        }
        else
        {
            x2 = float.PositiveInfinity; 
        }

        // Сравниваем точки пересечения по координате x
        return x1.CompareTo(x2);
    }
}