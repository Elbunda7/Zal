using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
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
        public event Action Loaded;

        private int maxTextLength = 200;
        private float yAngleDeg = 295f;
        private string morseText = "stahuju".ToMorse();
        private string rawText;
        private string textToShow;
        private bool isReady = false;
        private const string DOT = "<<<<<<>>>>>";
        private const string DASH = "<<<<<<<<<<<<<<<<<<<>";
        private const string SPACE = "<>>>>>>>>>>>>>>>>>>>";
        private Dictionary<char, int> charSize = new Dictionary<char, int>
        {
            {' ', 1},
            {'.', 11},
            {'-', 20},
            {'/', 20},
        };
        private SKPaint textPaint = new SKPaint
        {
            Typeface = GetMorseTypeface(),
            Color = SKColors.Black,
            IsAntialias = true,
        };
        private SKRect textBounds = new SKRect();
        private SKMatrix matrix;
        private SKPoint centeredOrigin = new SKPoint();

        public LoadingPage()
        {
            InitializeComponent();
            InitText();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            textPaint.MeasureText(textToShow, ref textBounds);
        }

        public static SKTypeface GetMorseTypeface()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("Zal.Resources.MorseSimple-normal.otf");
            if (stream == null) return null;
            return SKTypeface.FromStream(stream);
        }

        private void InitText()
        {
            var sbText = new StringBuilder();
            foreach (char ch in morseText)
            {
                switch (ch)
                {
                    case '.': sbText.Append(DOT); break;
                    case '-': sbText.Append(DASH); break;
                    case '/': sbText.Append(SPACE); break;
                }
            }
            string emptySpace = new string(' ', maxTextLength);
            sbText.Insert(0, emptySpace).Append(emptySpace);
            textToShow = new string('-', maxTextLength / charSize['-']);
            rawText = sbText.ToString();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartDownloading();
        }

        private async void StartDownloading()
        {
            await Task.Delay(200);
            new Animation(AnimationStep).Commit(this, "MorseFlow", length: 3000, repeat: () => true, easing: EasingInOutAcos());
            //await DownloadVersion();
            //await LoginUser();
            //await DownloadData();
            await Task.Delay(3050);
            //this.AbortAnimation("MorseFlow");
            Loaded?.Invoke();
        }

        private Easing EasingInOutAcos() {
            return new Easing(x => (1 - Math.Acos(2 * x - 1) / Math.PI + x)/2.0);
        }

    private async Task DownloadVersion()
        {
            Version appVersion_device = new Version(VersionTracking.CurrentVersion);
            Version appVersion_last = await Zalesak.CurrentVersion();
            if (appVersion_device == appVersion_last)
            {
                //await DisplayAlert("Verze aplikace", $"aktuální {appVersion_device}", "OK");
            }
            else
            {
                await DisplayAlert("Verze aplikace", $"máte {appVersion_device} a poslední je {appVersion_last}", "Aktualizovat");
                //await Launcher.OpenAsync(new Uri("market://details?id=cz.seznam.mapy"));
            }
        }

        private async Task LoginUser()
        {
            await Zalesak.Session.TryLoginWithTokenAsync();
        }

        private async Task DownloadData()
        {
            await Zalesak.GraphGalleries.Synchronize();
        }

        private void AnimationStep(double value)
        {
            int index = (int)(value * (rawText.Length - maxTextLength));
            var sb = new StringBuilder(rawText.Substring(index, maxTextLength));
            sb = sb.Replace(DOT, ".").Replace(DASH, "-").Replace(SPACE, "/");
            sb = sb.Replace('<', ' ').Replace('>', ' ');
            textToShow = sb.ToString();
            canvasView.InvalidateSurface();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            this.AbortAnimation("MorseFlow");
        }

        private void ComputeMatrix(SKPoint posFrom, SKPoint posTo, SKImageInfo info)
        {
            SKPoint center = posFrom + posTo;
            center.X /= 2.0f;
            center.Y /= 2.0f;
            var dx = posTo.X - posFrom.X;
            var dy = posTo.Y - posFrom.Y;
            var length = Math.Sqrt(dx * dx + dy * dy);
            float scale = (float)length / textBounds.Width;
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
            float offX = (float)(off * Math.Cos(zAngleRad));
            float offY = (float)(off * Math.Sin(zAngleRad));

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
            if (!isReady)
            {
                SKImageInfo info = args.Info;
                float coef = info.Width / (float)Width;
                SKPoint posCloud = new SKPoint
                {
                    X = (float)(Img_cloud.Bounds.Left + 2 * Img_cloud.Bounds.Right) / 3f * coef,
                    Y = (float)(Img_cloud.Bounds.Top + 3 * Img_cloud.Bounds.Bottom) / 4f * coef
                };
                SKPoint posDevice = new SKPoint
                {
                    X = (float)(2 * Img_device.Bounds.Left + Img_device.Bounds.Right) / 3f * coef,
                    Y = (float)(2 * Img_device.Bounds.Top + Img_device.Bounds.Bottom) / 3f * coef
                };
                ComputeMatrix(posDevice, posCloud, info);
                isReady = true;
            }
            SKCanvas canvas = args.Surface.Canvas;
            canvas.Clear();
            canvas.SetMatrix(matrix);
            canvas.DrawText(textToShow, centeredOrigin, textPaint);            
        }
    }
}