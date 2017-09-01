using System;
using System.Collections.Generic;

public class Core
{
    public List<Point> points = new List<Point>();
    Point center;

    public void addPoint(double x, double y)
    {
        points.Add(new  Point(x, y));
    }

     private void findCenter()
    {
        if(center != null ) return ;
        double maxy = 0;
        double maxx = 0;
        double minx = 1000;
        double miny = 1000;
        foreach (Point point in points)
        {
            if(point.x > maxx) maxx = point.x;
            if(point.y > maxy) maxy = point.y;
            if(point.x < minx) minx = point.x;
            if(point.y < miny) miny = point.y;
        }
        center  =  new Point((maxx - minx)/ 2 + minx , (maxy - miny) / 2 + miny);
    }


    public void scalePoints(double scale)
    {
        findCenter();
        foreach(Point point in points)
        {

            double dx = point.x - center.x;
            double dy = point.y - center.y;
            point.x = dx * scale + center.x;
            point.y = dy * scale + center.y;
        }
    }

    public void moveTo(Point mv_point)
    {
        foreach(Point point in points)
        {
            point.x = point.x + mv_point.x;
            point.y = point.y + mv_point.y;
        }
    }

    public void rotatePoints(int angle)
    {
        double angleR = angle * Math.PI / 180 ;
        double cosA = Math.Cos(angleR);
        double sinA = Math.Sin(angleR);
        findCenter();
        List<Point> pnew  =  new List<Point>();
        foreach (Point point in points)
        {
            double dx =  point.x - center.x;
            double dy =  point.y - center.y;
            point.x = cosA * dx - sinA * dy + center.x;
            point.y = sinA * dx + cosA * dy + center.y; 
        }
    }

}