using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Point = System.Drawing.Point;

namespace SolarSystem
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private readonly List<Point> m_starCoordinates = new List<Point>();
        private bool m_animateBackground;
        private bool m_first = true;
        private float m_scale = 1;
        private SKCanvas m_surfaceCanvas;

        public MainPage()
        {
            InitializeComponent();
        }

        private void CreateStarPoints(int width, int height)
        {
            var xPos = new Random();
            var yPos = new Random();

            for (var i = 0; i < 30; i++) m_starCoordinates.Add(new Point(xPos.Next(0, width), yPos.Next(0, height)));
        }

        private async void BackgroundAnimation()
        {
            var d = .0;
            while (m_animateBackground)
            {
                m_scale = (float)(1 + .15 * Math.Sin(d));
                d += Math.PI / 360;
                canvasView.InvalidateSurface();
                await Task.Delay(TimeSpan.FromMilliseconds(25));
            }
        }

        private void DrawBackground(object sender, SKPaintSurfaceEventArgs e)
        {
            m_surfaceCanvas = e.Surface.Canvas;
            var info = e.Info;

            m_surfaceCanvas.Clear(Color.Black.ToSKColor());

            if (m_first) CreateStarPoints(info.Width, info.Height);
            m_first = false;

            foreach (var starCoordinate in m_starCoordinates) DrawStar(e, starCoordinate.X, starCoordinate.Y, Color.Goldenrod);
        }

        private void DrawStar(SKPaintSurfaceEventArgs args, float x, float y, Color color)
        {
            var canvas = args.Surface.Canvas;

            var starStroke = new SKPaint
            {
                Color = Color.DarkGoldenrod.ToSKColor(), Style = SKPaintStyle.Stroke, StrokeWidth = 2f, StrokeJoin = SKStrokeJoin.Miter
            };

            using (var paint = new SKPaint())
            {
                paint.Shader = SKShader.CreateRadialGradient(
                    new SKPoint(x, y + 5),
                    20,
                    new[] { SKColors.White, color.ToSKColor() },
                    null,
                    SKShaderTileMode.Clamp);
                var starPath = new SKPath();
                starPath.MoveTo(x, y);

                starPath.RLineTo(m_scale * 5, m_scale * 15);
                starPath.RLineTo(m_scale * 15, m_scale * 0);
                starPath.RLineTo(m_scale * -12, m_scale * 10);

                starPath.RLineTo(m_scale * 2, m_scale * 15);
                starPath.RLineTo(m_scale * -10, m_scale * -10);

                starPath.RLineTo(m_scale * -10, m_scale * 10);
                starPath.RLineTo(m_scale * 2, m_scale * -15);
                starPath.RLineTo(m_scale * -12, m_scale * -10);
                starPath.RLineTo(m_scale * 15, m_scale * 0);

                starPath.Close();
                canvas.DrawPath(starPath, starStroke);
                canvas.DrawPath(starPath, paint);
            }
        }

        private void Update(object sender, EventArgs e)
        {
            m_animateBackground = !m_animateBackground;
            if (m_animateBackground) BackgroundAnimation();

            SingleStar.TranslateTo(m_animateBackground ? 600 : 0, m_animateBackground ? -100 : 0, 400, Easing.CubicInOut);
        }

        private void SingleStar_OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var surfaceCanvas = e.Surface.Canvas;
            surfaceCanvas.Clear();
            DrawStar(e, 200, 900, Color.DarkRed);
        }
    }
}