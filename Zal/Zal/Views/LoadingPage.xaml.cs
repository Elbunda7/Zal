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
        string text = "stah".ToMorse();
        SKPaint textPaint = new SKPaint
        {
            Typeface = GetTypeface("Zal/Resources/MorseSimple-normal.otf"),
            Color = SKColors.Black,
        };
        SKRect textBounds = new SKRect();


        public LoadingPage()
        {
            InitializeComponent();
            Title = "Animated Rotation 3D";

            //canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;
            //Content = canvasView;

            //var assembly = this.GetType().GetTypeInfo().Assembly;
            //var resources = assembly.GetManifestResourceNames();
            // Measure the text
                float a = textPaint.MeasureText(text, ref textBounds);
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

            new Animation(AnimationStep).Commit(this, "MorseFlow",length:2000, repeat: () => true);

            new Animation((value) => xRotationDegrees = 360 * (float)value).
                Commit(this, "xRotationAnimation", length: 5000, repeat: () => true);

            //new Animation((value) => yRotationDegrees = 360 * (float)value).
            //    Commit(this, "yRotationAnimation", length: 7000, repeat: () => true);

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

        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            // Find center of canvas
            float xCenter = info.Width / 2;
            float yCenter = info.Height / 2;

            // Translate center to origin
            SKMatrix matrix = SKMatrix.MakeTranslation(-xCenter, -yCenter);

            // Scale so text fits
            float scale = Math.Min(info.Width / textBounds.Width,
                                   info.Height / textBounds.Height);
            SKMatrix.PostConcat(ref matrix, SKMatrix.MakeScale(scale, scale));

            // Calculate composite 3D transforms
            float depth = 0.75f * scale * textBounds.Width;

            SKMatrix44 matrix44 = SKMatrix44.CreateIdentity();
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(1, 0, 0, 0));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 1, 0, 50));
            matrix44.PostConcat(SKMatrix44.CreateRotationDegrees(0, 0, 1, 315));

            SKMatrix44 perspectiveMatrix = SKMatrix44.CreateIdentity();
            perspectiveMatrix[3, 2] = -1 / depth;
            matrix44.PostConcat(perspectiveMatrix);

            // Concatenate with 2D matrix
            SKMatrix.PostConcat(ref matrix, matrix44.Matrix);

            // Translate back to center
            SKMatrix.PostConcat(ref matrix,
                SKMatrix.MakeTranslation(xCenter, yCenter));
            // Set the matrix and display the text
            canvas.SetMatrix(matrix);
            float xText = xCenter - textBounds.MidX;
            float yText = yCenter - textBounds.MidY;
            canvas.DrawText(text, xText, yText, textPaint);
        }
    }
}