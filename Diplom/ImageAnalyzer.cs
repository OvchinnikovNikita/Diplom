using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Diplom
{
    public class ImageAnalyzer
    {
        public Image Image { get; set; }
        public Bitmap Bitmap { get; set; }
        public List<List<int>> PixelsColors { get; set; }
        public bool[][] VisitedPixels { get; set; }
        public List<List<Point>> Borders { get; set; }
        public const double eps = 3;

        public ImageAnalyzer(Image image)
        {
            Image = image;
            Bitmap = new Bitmap(Image);
            PixelsColors = GetPixelsColors();
            VisitedPixels = new bool[Image.Height][];
            Borders = new List<List<Point>>();

            for (int i = 0; i < Image.Height; i++)
            {
                VisitedPixels[i] = new bool[Image.Width];
            }
        }

        private List<List<int>> GetPixelsColors()
        {
            var res = new List<List<int>>(Image.Height);
            for (int y = 0; y < Image.Height; y++)
            {
                var colors = new List<int>(Image.Width);
                for (int x = 0; x < Image.Width; x++)
                {
                    colors.Add(GetColor(x, y));
                }

                res.Add(colors);
            }

            return res;
        }

        private int GetColor(int x, int y)
        {
            var pixel = Bitmap.GetPixel(x, y);

            if (pixel.R > 230 && pixel.G > 230 && pixel.B > 230)
            {
                return 1;
            }

            if (pixel.R < 20 && pixel.G < 20 && pixel.B < 20)
            {
                return 0;
            }

            throw new ArgumentException("Изображение не ЧБ!!!!");
        }

        public List<List<Point>> GetBorders()
        {
            int i = -1; // 
            for (var y = 0; y < Image.Height; y++)
            {
                for (var x = 0; x < Image.Width; x++)
                {
                    if (VisitedPixels[y][x]) continue;

                    var color = GetColor(x, y);

                    if (color == 0)
                    {
                        var point = new Point(x, y);
                        // Вот тут сомнительно, но попробуем обработать случай, когда полигональный объект - черный пиксель
                        if (DotIsolated(point))
                        {
                            Borders.Add(new List<Point>() { point });
                            VisitedPixels[point.Y][point.X] = true;
                            continue;
                        }

                        var pair = FindPair(x, y);

                        var border = FindBorder(point, pair);
                        Borders.Add(border);
                    }

                    VisitedPixels[y][x] = true;
                }
            }

            return Borders;
        }

        private bool DotIsolated(Point p)
        {
            return PixelsColors[p.Y][p.X + 1] == 1 &&
                   PixelsColors[p.Y][p.X - 1] == 1 &&
                   PixelsColors[p.Y + 1][p.X] == 1 &&
                   PixelsColors[p.Y - 1][p.X] == 1;
        }

        public List<List<int>> GetColors()
        {
            return PixelsColors;
        }

        private double GetDistanceBetweenLineAndPoint(Point p0, Point p1, Point p2)
        {
            var numerator = Math.Abs((p2.Y - p1.Y) * p0.X - (p2.X - p1.X) * p0.Y + p2.X * p1.Y - p2.Y * p1.X);
            var denomenator = Math.Sqrt((p2.Y - p1.Y) * (p2.Y - p1.Y) + (p2.X - p1.X) * (p2.X - p1.X));

            return numerator / denomenator;
        }

        private KeyValuePair<Point, double> GetFurthestPoint(Point p1, Point p2, IEnumerable<Point> border)
        {
            var maxDistance = -1.0;
            var maxDistancePoint = new Point(-1, -1);

            foreach (var point in border)
            {
                var distance = GetDistanceBetweenLineAndPoint(point, p1, p2);

                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    maxDistancePoint = point;
                }
            }

            return new KeyValuePair<Point, double>(maxDistancePoint, maxDistance);
        }

        private Point GetIntersectionWithTheAbscissa(Point p1, Point p2)
        {
            if (p1.Y - p2.Y == 0)
                return new Point(int.MaxValue, 0);

            return new Point(-(p1.X * p2.Y - p1.Y * p2.X) / (p1.Y - p2.Y), 0);
        }

        private Point GetIntersectionWithTheOrdinate(Point p1, Point p2)
        {
            if (p1.X - p2.X == 0)
                return new Point(0, int.MaxValue);

            return new Point(0, (p1.X * p2.Y - p1.Y * p2.X) / (p1.X - p2.X));
        }

        private List<List<Point>> DividePointsWithALine(Point p1, Point p2, List<Point> border)
        {
            var leftPoints = new List<Point>();
            var rightPoints = new List<Point>();

            var ksiX = GetIntersectionWithTheAbscissa(p1, p2).X;
            var ksiY = GetIntersectionWithTheOrdinate(p1, p2).Y;

            if (ksiY < 0)
                ksiY = int.MaxValue;

            if (ksiX < 0)
                ksiX = int.MaxValue;

            foreach (var point in border)
            {
                if (point.X < ksiX && point.Y < ksiY)
                {
                    leftPoints.Add(point);
                }
                else
                {
                    rightPoints.Add(point);
                }
            }

            return new List<List<Point>>() { leftPoints, rightPoints };
        }


        private Point FindPair(int x, int y)
        {
            if (y - 1 >= 0)
                return new Point(x, y - 1);

            if (x == 0)
            {
                // Возоможно, нигде внутренние if не нужны в этом методе.
                if (y == 0)
                    return new Point(0, 1);

                return new Point(0, y - 1);
            }

            if (x == Image.Width - 1)
            {
                if (y == 0)
                    return new Point(x - 1, y);

                return new Point(x, y - 1);
            }

            return new Point(x - 1, y);
        }

        private List<Point> FindBorder(Point left, Point right)
        {
            var res = new List<Point>();
            res.Add(left);
            var trialPoint = GetNewTrialPoint(left, right);

            var c = 0;
            while ((res[0].X != trialPoint.X || res[0].Y != trialPoint.Y) && c < Image.Height * Image.Width)
            {
                c++;
                var color = GetColor(trialPoint.X, trialPoint.Y);

                if (color == 0)
                {
                    left.X = trialPoint.X;
                    left.Y = trialPoint.Y;

                    VisitedPixels[left.Y][left.X] = true;
                    // Console.WriteLine($"{left.X} {left.Y}");
                    res.Add(left);
                }
                else
                {
                    right.X = trialPoint.X;
                    right.Y = trialPoint.Y;
                }

                trialPoint = GetNewTrialPoint(left, right);
            }

            if (c == Image.Height * Image.Width)
            {
                throw new System.Exception("Метод оббежал все пиксели. Что-то не так");
            }

            // Пробуем сделать так, чтобы по внутренней части полигона алогритм не ходил
            var minY = res.Min(p => p.Y);
            var maxY = res.Max(p => p.Y);

            for (int y = minY; y <= maxY; y++)
            {
                var pointsWithCurrentYCoordinates = res.Where(p => p.Y == y).OrderBy(p => p.X).ToList();
                for (int i = 0; i < pointsWithCurrentYCoordinates.Count - 1; i++)
                {
                    var pointFrom = pointsWithCurrentYCoordinates[i];
                    var pointTo = pointsWithCurrentYCoordinates[i + 1];
                    var x = pointFrom.X;

                    if (PixelsColors[y][x + 1] == 0)
                    {
                        while (x <= pointTo.X)
                        {
                            x++;
                            VisitedPixels[y][x] = true;
                        }
                    }
                }
            }

            return res;
        }

        private Point GetNewTrialPoint(Point left, Point right)
        {
            int newX;
            int newY;

            if (left.X == right.X || right.Y == left.Y)
            {
                newX = right.X - (right.Y - left.Y);
                newY = right.Y + (right.X - left.X);
            }
            else
            {
                newX = (left.X + right.X - right.Y + left.Y) / 2;
                newY = (left.Y + right.Y + right.X - left.X) / 2;
            }

            return new Point(newX, newY);
        }
    }
}