using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApp8
{
    public class Point
    {
        public double x, y;

        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        // Method used to display X and Y coordinates  
        // of a point  
        public static void displayPoint(Point p)
        {
            Console.WriteLine("(" + p.x + ", " + p.y + ")");
        }
    }

   

    class Program
    {
        public static double CalculateManhattanDistance(Point a, Point b)
        {
            return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
        }

        public static Point FindLineIntersection(Point A, Point B, Point C, Point D)
        {
            // Line AB represented as a1x + b1y = c1  
            double a1 = B.y - A.y;
            double b1 = A.x - B.x;
            double c1 = a1 * (A.x) + b1 * (A.y);

            // Line CD represented as a2x + b2y = c2  
            double a2 = D.y - C.y;
            double b2 = C.x - D.x;
            double c2 = a2 * (C.x) + b2 * (C.y);

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {               
                return new Point(double.MaxValue, double.MaxValue);
            }
            else
            {
                double x = (b2 * c1 - b1 * c2) / determinant;
                double y = (a1 * c2 - a2 * c1) / determinant;
                if (CheckIntersect(A,B,C,D)) 
                    return new Point(x, y);
                else
                    return new Point(double.MaxValue, double.MaxValue);
            }
        }

        static Boolean onSegment(Point p, Point q, Point r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r). 
        // The function returns following values 
        // 0 --> p, q and r are colinear 
        // 1 --> Clockwise 
        // 2 --> Counterclockwise 
        static double orientation(Point p, Point q, Point r)
        {           
            double val = (q.y - p.y) * (r.x - q.x) - (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0; // colinear 

            return (val > 0) ? 1 : 2; // clock or counterclock wise 
        }

        // The main function that returns true if line segment 'p1q1' 
        // and 'p2q2' intersect. 
        static Boolean CheckIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and 
            // special cases 
            double o1 = orientation(p1, q1, p2);
            double o2 = orientation(p1, q1, q2);
            double o3 = orientation(p2, q2, p1);
            double o4 = orientation(p2, q2, q1);

            // General case 
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases 
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1 
            if (o1 == 0 && onSegment(p1, p2, q1)) return true;

            // p1, q1 and q2 are colinear and q2 lies on segment p1q1 
            if (o2 == 0 && onSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2 
            if (o3 == 0 && onSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2 
            if (o4 == 0 && onSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases 
        }

        static List<Point> MapLine(Point center, string[] lines)
        {
            List<Point> wire = new List<Point>();
            string direction = string.Empty;
            int step = 0;
            Point current = center;
            wire.Add(center);
            
            foreach (var move in lines)
            {
                direction = move.Substring(0, 1);
                step = int.Parse(move.Substring(1));
                if (direction == "U")
                    wire.Add(new Point(current.x, current.y + step));
                if (direction == "D")
                    wire.Add(new Point(current.x, current.y - step));
                if (direction == "R")
                    wire.Add(new Point(current.x + step, current.y));
                if (direction == "L")
                    wire.Add(new Point(current.x - step, current.y));

                current = wire.Last();

            }
            return wire;
        }
        static void Main(string[] args)
        {
            Point Center = new Point(0, 0);
            string[] wires = File.ReadAllLines("Input.txt");
            List<Point> intersectionList = new List<Point>();
            List<Point> wire1 = MapLine(Center, wires[0].Split(','));
            List<Point> wire2 = MapLine(Center, wires[1].Split(','));

            for (int i = 0; i < wire1.Count() - 1; i++)
            {
                for (int j = 0; j < wire2.Count() - 1; j++)
                {
                    Point crossing = FindLineIntersection(wire1[i], wire1[i + 1], wire2[j], wire2[j + 1]);
                    if (crossing.x != double.MaxValue || crossing.y != double.MaxValue)
                    {
                        if (crossing.x != 0 && crossing.y != 0)
                        intersectionList.Add(crossing);
                    }
                }
            }

            Point nearestIntersection = intersectionList.OrderBy(p => CalculateManhattanDistance(Center, p)).ToList().First();

          
            double manhattanDistance = CalculateManhattanDistance(Center, nearestIntersection);
            Console.WriteLine(manhattanDistance);
        }
    }
}
