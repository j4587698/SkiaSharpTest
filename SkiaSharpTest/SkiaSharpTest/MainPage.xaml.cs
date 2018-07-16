using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace SkiaSharpTest
{
	public partial class MainPage : ContentPage
	{

	    private BookPoint a, f, g, e, h, c, j, b, k, d, i;
	    private SKRegion _region;
	    private SKPath _pathA, _pathC;

        public MainPage()
		{
			InitializeComponent();
		    a = new BookPoint();
		    f = new BookPoint();
		    g = new BookPoint();
		    e = new BookPoint();
		    h = new BookPoint();
		    c = new BookPoint();
		    j = new BookPoint();
		    b = new BookPoint();
		    k = new BookPoint();
		    d = new BookPoint();
		    i = new BookPoint();
		    CanvasView.IgnorePixelScaling = true;
		    _region = new SKRegion();
		    _pathA = new SKPath();
		    _pathC = new SKPath();
		}

	    protected override void OnAppearing()
	    {
	        base.OnAppearing();
	        f.X = (float)Width;
	        f.Y = (float)Height;
	    }

	    private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs e)
	    {
	        if (a.X == 0 && a.Y == 0)
	        {
	            e.Surface.Canvas.DrawColor(SKColors.Green);
	        }
	        else
	        {
                e.Surface.Canvas.DrawColor(SKColors.Blue);
	            DrawPathCContent(e.Surface.Canvas);
	        }
	    }

	    private void OnCanvasViewPan(object sender, PanUpdatedEventArgs e)
	    {
	        a.X = (float) e.TotalX;
	        a.Y = (float) e.TotalY;
	        CalcPointsXY();
            CanvasView.InvalidateSurface();
	    }

	    private void CalcPointsXY()
	    {
	        g.X = (a.X + f.X) / 2;
	        g.Y = (a.Y + f.Y) / 2;

	        e.X = g.X - (f.Y - g.Y) * (f.Y - g.Y) / (f.X - g.X);
	        e.Y = f.Y;

	        h.X = f.X;
	        h.Y = g.Y - (f.X - g.X) * (f.X - g.X) / (f.Y - g.Y);

	        c.X = e.X - (f.X - e.X) / 2;
	        c.Y = f.Y;

	        j.X = f.X;
	        j.Y = h.Y - (f.Y - h.Y) / 2;

	        b = GetIntersectionPoint(a, e, c, j);
	        k = GetIntersectionPoint(a, h, c, j);

	        d.X = (c.X + 2 * e.X + b.X) / 4;
	        d.Y = (2 * e.Y + c.Y + b.Y) / 4;

	        i.X = (j.X + 2 * h.X + k.X) / 4;
	        i.Y = (2 * h.Y + j.Y + k.Y) / 4;
	    }

	    private BookPoint GetIntersectionPoint(BookPoint lineOneMyPointOne, BookPoint lineOneMyPointTwo, BookPoint lineTwoMyPointOne, BookPoint lineTwoMyPointTwo)
	    {
	        float x1 = lineOneMyPointOne.X;
	        float y1 = lineOneMyPointOne.Y;
	        float x2 = lineOneMyPointTwo.X;
	        float y2 = lineOneMyPointTwo.Y;
	        float x3 = lineTwoMyPointOne.X;
	        float y3 = lineTwoMyPointOne.Y;
	        float x4 = lineTwoMyPointTwo.X;
	        float y4 = lineTwoMyPointTwo.Y;

	        float pointX = ((x1 - x2) * (x3 * y4 - x4 * y3) - (x3 - x4) * (x1 * y2 - x2 * y1))
	                       / ((x3 - x4) * (y1 - y2) - (x1 - x2) * (y3 - y4));
	        float pointY = ((y1 - y2) * (x3 * y4 - x4 * y3) - (x1 * y2 - x2 * y1) * (y3 - y4))
	                       / ((y1 - y2) * (x3 - x4) - (x1 - x2) * (y3 - y4));

	        return new BookPoint(pointX, pointY);
	    }

	    public SKRectI ToSKRectI(SKRect rect)
	    {
	        return new SKRectI((int)rect.Left, (int)rect.Top, (int)rect.Right, (int)rect.Bottom);
	    }

	    public void ClipRegion(SKRegion region, SKPath path,
	        SKRegionOperation regionOperation = SKRegionOperation.Intersect)
	    {
	        using (SKRegion region1 = new SKRegion())
	        {
	            region1.SetPath(path);
	            region.Op(region1, regionOperation);
            }
	    }

	    private SKPath GetPathA(float width, float height)
	    {
	        _pathA.Reset();
	        _pathA.LineTo(0, height);//移动到左下角
	        _pathA.LineTo(c.X, c.Y);//移动到c点
	        _pathA.QuadTo(e.X, e.Y, b.X, b.Y);//从c到b画贝塞尔曲线，控制点为e
	        _pathA.LineTo(a.X, a.Y);//移动到a点
	        _pathA.LineTo(k.X, k.Y);//移动到k点
	        _pathA.QuadTo(h.X, h.Y, j.X, j.Y);//从k到j画贝塞尔曲线，控制点为h
	        _pathA.LineTo(width, 0);//移动到右上角
	        _pathA.Close();//闭合区域
	        return _pathA;
	    }

	    private SKPath GetPathC()
	    {
	        _pathC.Reset();
	        _pathC.MoveTo(i.X, i.Y);//移动到i点
	        _pathC.LineTo(d.X, d.Y);//移动到d点
	        _pathC.LineTo(b.X, b.Y);//移动到b点
	        _pathC.LineTo(a.X, a.Y);//移动到a点
	        _pathC.LineTo(k.X, k.Y);//移动到k点
	        _pathC.Close();//闭合区域

	        return _pathC;
	    }

        private void DrawPathCContent(SKCanvas canvas)
	    {
	        canvas.Save();
	        GetPathA((float)Width, (float)Height);
	        _region.SetRect(ToSKRectI(canvas.LocalClipBounds));
	        ClipRegion(_region, _pathA);
	        ClipRegion(_region, GetPathC(), SKRegionOperation.ReverseDifference);
	        canvas.ClipRegion(_region);
            canvas.DrawColor(SKColors.Green);
            canvas.Restore();
	    }

        private class BookPoint
	    {
	        public float X { get; set; }

	        public float Y { get; set; }

	        public BookPoint()
	        {
	        }

	        public BookPoint(float x, float y)
	        {
	            X = x;
	            Y = y;
	        }
	    }
    }
}
