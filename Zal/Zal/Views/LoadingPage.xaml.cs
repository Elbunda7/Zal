using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Domain;
using Zal.Domain.Tools;

namespace Zal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadingPage : ContentPage
	{
        //SKCanvasView canvasView;
        float xRotationDegrees, yRotationDegrees, zRotationDegrees;
        string text = "stahu".ToMorse();
        SKPaint textPaint = new SKPaint
        {
            Typeface = GetTypeface("Zal/Resources/MorseSimple-normal.otf"),
            Color = SKColors.Black,
            TextSize = 100,
        };
        SKSize size;
        SKRect textBounds = new SKRect();
        //PointF[] points = new PointF[4];
        SKMatrix matrix;
        SKPoint centeredOrigin = new SKPoint();
        SKPoint Origin = new SKPoint();
        bool isReady = false;

        public LoadingPage()
        {
            InitializeComponent();
            Title = "Animated Rotation 3D";
            Lyt_absolute.SizeChanged += Lyt_absolute_SizeChanged;

            //canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            //canvasView.Background = Brush.White;
            //Content = canvasView;

            //var assembly = this.GetType().GetTypeInfo().Assembly;
            //var resources = assembly.GetManifestResourceNames();
            // Measure the text
                float a = textPaint.MeasureText(text, ref textBounds);
        }

        private void Lyt_absolute_SizeChanged(object sender, EventArgs e)
        {
            size = new SKSize((float)Lyt_absolute.Width, (float)Lyt_absolute.Height);
            var cloud = Img_cloud.Bounds;
            var device = Img_device.Bounds;

            new Animation(AnimationStep).Commit(this, "MorseFlow", length: 2000, repeat: () => true);



        }


        public static SKTypeface GetTypeface(string fullFontName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Zal.Resources.MorseSimple-normal.otf");
            if (stream == null) return null;
            return SKTypeface.FromStream(stream);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //new Timer(OnTimerElapsed, null, 200, 200);

            //new Animation((value) => yRotationDegrees = 360 * (float)value).
            //    Commit(this, "yRotationAnimation", length: 7000, repeat: () => true);

            //        new Animation((value) => xRotationDegrees = 360 * (float)value).
            //Commit(this, "xRotationAnimation", length: 25000, repeat: () => true);

            //new Animation((value) =>
            //{
            //    zRotationDegrees = 360 * (float)value;
            //    canvasView.InvalidateSurface();
            //}).Commit(this, "zRotationAnimation", length: 11000, repeat: () => true);
        }

        private Dictionary<char, int> charSize = new Dictionary<char, int>
        {
            {' ', 1},
            {'.', 11},
            {'-', 20},
            {'/', 20},
        };

        private char lastChar = ' ';
        private int spacesAtEnd = 0;
        private void AnimationStep(double value)
        {
            char firstChar = text[0];
            text = new string(' ', charSize[firstChar] - 1) + text.Substring(1);

            if (lastChar == ' ')
            {
                lastChar = firstChar;
                spacesAtEnd = 1;
            }
            else
            {
                spacesAtEnd++;
            }

            if (spacesAtEnd >= charSize[lastChar])
            {
                text = text.Substring(0, text.Length - (charSize[lastChar] - 1)) + lastChar;
                lastChar = ' ';
            }
            else text += ' ';

            canvasView.InvalidateSurface();
        }



        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.AbortAnimation("xRotationAnimation");
            this.AbortAnimation("yRotationAnimation");
            this.AbortAnimation("zRotationAnimation");
        }

        private void ComputeMatrix(SKPoint posFrom, SKPoint posTo, SKImageInfo info, float yAngleDeg)
        {
            SKPoint center = posFrom + posTo;
            center.X /= 2.0f;
            center.Y /= 2.0f;
            var dx = posTo.X - posFrom.X;
            var dy = posTo.Y - posFrom.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);

            float scale = (float)length / textBounds.Width;

            //float depth = info.Width * 0.7f;

            //yAngleDeg = 0;
            var zAngleRad = Math.Atan2(dy, dx);
            var zAngleDeg = (float)(zAngleRad * 180 / Math.PI);
            var yAngleRad = yAngleDeg * Math.PI / 180;
            var c = textBounds.Width * scale / 2.0;
            var a = Math.Sin(yAngleRad) * c;
            var b = Math.Sqrt(c * c - a * a);
            if (length <= 2 * b) length = 2 * b + 10;
            var depth = (float)(a * Math.Sqrt(length) / Math.Sqrt(length - 2 * b));
            if (depth == 0) depth = 1;
            var y1 = b * depth / (depth - a);
            var y2 = b * depth / (depth + a);
            var off = (y1 - y2) / 2.0;
            var length1 = y1 + y2;

            //var depth2 = a * Math.Sqrt(length1) / Math.Sqrt(length1 - 2 * b);


            //if (zAngleDeg < 0) zAngleDeg += 360;
            //if (zAngleDeg > 90 && zAngleDeg < 270) off *= -1;
            //off *= -1;

            float offX = (float)(off * Math.Cos(zAngleRad));
            float offY = (float)(off * Math.Sin(zAngleRad));

            //yAngleDeg = 315;

            SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, 0));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, yAngleDeg));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, zAngleDeg));

            SKMatrix44 perspectiveMatrix = SKMatrix44.CreateIdentity();
            perspectiveMatrix[3, 2] = -1 / depth;
            matrix44.PostConcat(perspectiveMatrix);            

            matrix = SKMatrix.MakeTranslation(-center.X, -center.Y);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeScale(scale, scale));
            SKMatrix.PostConcat(ref matrix, matrix44.Matrix);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeTranslation(center.X + offX, center.Y + offY));

            centeredOrigin.X = center.X - textBounds.MidX;
            centeredOrigin.Y = center.Y - textBounds.MidY;
        }

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKCanvas canvas = args.Surface.Canvas;

            if (!isReady)
            {
                float coef = info.Width / (float)Width;
                SKPoint posCloud = new SKPoint
                {
                    X = (float)(Img_cloud.Bounds.Left + 2 * Img_cloud.Bounds.Right) / 3f * coef,
                    Y = (float)(Img_cloud.Bounds.Top + 3 * Img_cloud.Bounds.Bottom) / 4f * coef
                };
                SKPoint posDevice = new SKPoint
                {
                    X = (float)(Img_device.Bounds.Left + Img_device.Bounds.Right) / 2f * coef,
                    Y = (float)(2 * Img_device.Bounds.Top + Img_device.Bounds.Bottom) / 3f * coef
                };

                var a = Img_device;
                var b = Img_cloud;
                Origin.X = info.Width / 2;
                Origin.Y = info.Height / 2;
                SKPoint posFrom = Origin;
                SKPoint posTo = Origin;
                //posFrom.X = 0;
                //posFrom.Y = 0;
                //posTo.X -= 200;
                posTo.Y += 200;

                ComputeMatrix(posDevice, posCloud, info, 290);
                isReady = true;
            }

            //var cloud = Img_cloud.Bounds;
            //var device = Img_device.Bounds;
            //float coef = (float)(info.Width / textBounds.Width);

            canvas.Clear();
            canvas.SetMatrix(matrix);
            canvas.DrawText(text, centeredOrigin, textPaint);            
        }
    }
}