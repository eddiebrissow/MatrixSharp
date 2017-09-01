using Gtk;
using System;
using Cairo;

public class WindowContext : Gtk.Window
{
    private int w;
    private int h;
    static DrawingArea area;
    Core core;
    Cairo.Context ctx;
    Button rotateRight;
    Button upScale;
    Button downScale;
    Button rotateLeft;

    public void addCore(Core core)
    {
        this.core = core;
    }


    void OnMousePress (object source, ButtonPressEventArgs args)
    {
            if(core != null)
                core.addPoint(args.Event.X, args.Event.Y);
            Draw();
    }

     void rotatePointsRight(object sender, EventArgs args)
    {
        core.rotatePoints(90);
        Draw();
    }

     void upScalePoints(object sender, EventArgs args)
    {
        core.scalePoints(1.05);
        Draw();
    }


     void downScalePoints(object sender, EventArgs args)
    {
        core.scalePoints(0.95);
        Draw();
    }

     void rotatePointsLeft(object sender, EventArgs args)
    {
        core.rotatePoints(-5);
        Draw();
    }
    [GLib.ConnectBefore]
     void OnKeyPress(object sender, KeyPressEventArgs args)
    {
        int moveFactor = 2;

        switch (args.Event.KeyValue)
        {
            case 65361:
                core.moveTo(new Point(moveFactor * -1, 0));
                break;
                
            case 65362:
                core.moveTo(new Point(0, moveFactor * -1));
                break;

            case 65363:
                core.moveTo(new Point(moveFactor, 0));
                break;

            case 65364:
                core.moveTo(new Point(0, moveFactor));
                break;

            
            default: break;
        }
        // Console.WriteLine("Key: " + args.Event.KeyValue);
        Draw();
    }


    void OnDrawingAreaExposed (object source, ExposeEventArgs args)
    {   
        DrawingArea area = (DrawingArea) source;
        ctx = Gdk.CairoHelper.Create (area.GdkWindow);

        ((IDisposable) ctx.Target).Dispose();
        ((IDisposable) ctx).Dispose ();
    }

    public WindowContext(Core core, string name, int w, int h): base(name)
    {
        this.core = core;
        Application.Init();
        area = new DrawingArea ();
        area.ExposeEvent += OnDrawingAreaExposed;

        area.AddEvents (
                (int)Gdk.EventMask.PointerMotionMask
                | (int)Gdk.EventMask.ButtonPressMask
                | (int) Gdk.EventMask.KeyPressMask
                | (int)Gdk.EventMask.ButtonReleaseMask);

        area.ButtonPressEvent += OnMousePress;
        KeyPressEvent += OnKeyPress;
        area.KeyPressEvent += OnKeyPress;

        DeleteEvent += delegate { Application.Quit(); };

        SetDefaultSize(w, h);
        SetPosition(WindowPosition.Center);
        VBox vbox = new VBox();
        vbox.Add(area);
        HBox hbox = new HBox();
        rotateRight =  new Button("Rotate Right");
        rotateLeft =  new Button("Rotate Left");
        upScale =  new Button("Up Scale");
        downScale =  new Button("Down Scale");
        hbox.Add(rotateRight);
        hbox.Add(rotateLeft);
        hbox.Add(upScale);
        hbox.Add(downScale);
        Alignment halign = new Alignment(1, 0, 0, 0);
        halign.Add(hbox);
        vbox.Add(hbox);
        vbox.PackStart(halign, false, false, 3);
        rotateRight.Clicked += rotatePointsRight;
        rotateLeft.Clicked += rotatePointsLeft;
        upScale.Clicked += upScalePoints;
        downScale.Clicked += downScalePoints;
        Add(vbox);
        ShowAll();
        Application.Run();
    }

    void Draw()
    {
        Clear();
        if(core.points.Count == 1)
            DrawPoint(core.points[0]);
        else
            for(int i=1; i < core.points.Count; i++ )
            {
                DrawLine(core.points[i-1], core.points[i]);
                DrawPointInfo(core.points[i-1]);

            }

        
    }


    void DrawPointInfo(Point point)
    {
        using (Cairo.Context ctx = Gdk.CairoHelper.Create (area.GdkWindow))
        {
                ctx.Color = new Cairo.Color (1, 0, 0);
                ctx.SelectFontFace("Georgia", FontSlant.Normal, FontWeight.Bold);
                ctx.SetFontSize(11);
                ctx.MoveTo(point.x, point.y);
                ctx.ShowText("(" + point.x.ToString("0.00") + ", " + point.y.ToString("0.00") + ")");
            
        }

    }


     void DrawPoint(Point point)
    {
          using (Cairo.Context ctx = Gdk.CairoHelper.Create (area.GdkWindow))
        {
                ctx.Color = new Cairo.Color (0, 0, 0);
                ctx.Rectangle(point.x, point.y, 1, 1);
                ctx.Stroke();
            
        }

    }


     void DrawLine(Point p1, Point p2)
    {
        double x1 = Math.Truncate(p2.x);
        double x0 = Math.Truncate(p1.x);
        double y0 = Math.Truncate(p1.y);
        double y1 = Math.Truncate(p2.y);

        double dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        double dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        double err = (dx > dy ? dx : -dy) / 2, e2;
        for(;;) {
            DrawPoint(new Point(x0,y0));
            if (x0 == x1 && y0 == y1) break;
            e2 = err;
            if (e2 > -dx) { err -= dy; x0 += sx; }
            if (e2 < dy) { err += dx; y0 += sy; }
        }
    }


     void Clear()
    {
        using (Cairo.Context ctx = Gdk.CairoHelper.Create (area.GdkWindow))
        {
                ctx.Color = new Cairo.Color (1, 1, 1);
                ctx.Rectangle(0, 0, 500, 500);
                ctx.FillPreserve();   
        }
    }



}