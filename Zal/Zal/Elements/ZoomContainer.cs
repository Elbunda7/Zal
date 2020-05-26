using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class ZoomContainer : ContentView
    {
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 3;
        private const double OVERSHOOT = 0.15;
        private double startScale = 1;
        private double StartX, StartY;
        bool panIsRunning = false;
        bool isActivatedFirstTime = true;
        Task<bool> Animation;
        double origHeight;
        double origWidth;

        double xOffset = 0;
        double yOffset = 0;
        private double startMidY;
        private double startMidX;

        public ZoomContainer()
        {
            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;
            Content.HorizontalOptions = LayoutOptions.Center;
            Content.VerticalOptions = LayoutOptions.Center;
            (Content as Image).Aspect = Aspect.AspectFit;
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            if (Animation != null && !Animation.IsCompleted) return;
            if (panIsRunning) CompletePan();

            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                startMidX = CurrentMiddleX();
                startMidY = CurrentMiddleY();
            }

            if (e.Status == GestureStatus.Running)
            {
                double currentScale = Content.Scale + (e.Scale - 1) * startScale;
                currentScale = Math.Max(1-OVERSHOOT, currentScale);

                Content.TranslationX = GetTranslationX(startMidX, currentScale);
                Content.TranslationY = GetTranslationY(startMidY, currentScale);

                if (currentScale > 1)
                {
                    Content.TranslationX = Math.Min(Content.Width / 2 * (currentScale - 1), Math.Max(Content.TranslationX, -Content.Width / 2 * (currentScale - 1)));
                    Content.TranslationY = Math.Min(Content.Height / 2 * (currentScale - 1), Math.Max(Content.TranslationY, -Content.Height / 2 * (currentScale - 1)));
                }
                else
                {
                    Content.TranslationX = 0;
                    Content.TranslationY = 0;
                }

                Content.Scale = currentScale;
            }

            if (e.Status == GestureStatus.Completed)
            {
                if (Content.Scale > MAX_SCALE)
                {
                    Content.TranslateTo(GetTranslationX(CurrentMiddleX(), MAX_SCALE), GetTranslationY(CurrentMiddleY(), MAX_SCALE), 250, Easing.SpringOut);
                    Animation = Content.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                }
                else if (Content.Scale < MIN_SCALE)
                {
                    Content.TranslateTo(0, 0, 250, Easing.SpringOut);
                    Animation = Content.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                }
            }
        }

        private double CurrentMiddleX()
        {
            double deltaX = xOffset / Width;
            double deltaWidth = Width / (Content.Width * startScale);
            double midX = -deltaX * deltaWidth;
            return midX;
        }

        private double CurrentMiddleY()
        {
            double deltaY = yOffset / Height;
            double deltaHeight = Content.Height / (Content.Height * startScale);
            double midY = -deltaY * deltaHeight;
            return midY;
        }

        private double GetTranslationX(double mid, double targetScale)
        {
            return -mid * targetScale * Width;
        }

        private double GetTranslationY(double mid, double targetScale)
        {
            return -mid * targetScale * Height;
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Console.WriteLine($"Pan");
            if (Animation != null && !Animation.IsCompleted) return;
            if (!panIsRunning)
            {
                StartX = e.TotalX;
                StartY = e.TotalY;
                panIsRunning = true;
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
            }

            switch (e.StatusType)
            {
                case GestureStatus.Running:
                    Translate(e.TotalX - StartX, e.TotalY - StartY);
                    break;
                case GestureStatus.Completed:
                    CompletePan();
                    break;
            }
        }

        private void Translate(double x, double y)
        {
            Console.WriteLine($"x:{x} y:{y}");
            double newX = (x * Scale) + xOffset;
            double newY = (y * Scale) + yOffset;

            double width = (Content.Width * Content.Scale);
            double height = (Content.Height * Content.Scale);

            double wBound = (width - Application.Current.MainPage.Width) / 2;
            double hBound = (height - Application.Current.MainPage.Height) / 2;

            if (newX < -wBound) newX = -wBound;
            if (newX > wBound) newX = wBound;
            if (width < Application.Current.MainPage.Width) newX = 0;

            if (newY < -hBound) newY = -hBound;
            if (newY > hBound) newY = hBound;
            if (height < Application.Current.MainPage.Height) newY = 0;

            Content.TranslationX = newX;
            Content.TranslationY = newY;
        }

        private void CompletePan()
        {
            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
            panIsRunning = false;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            double pageHeight = Application.Current.MainPage.Height;
            double pageWidth = Application.Current.MainPage.Width;

            if (Content.Width < 1 || Content.Height < 1) return;
            if (isActivatedFirstTime)
            {
                origHeight = Content.Height;
                origWidth = Content.Width;
                isActivatedFirstTime = false;
            }

            if (pageHeight / pageWidth < origHeight / origWidth)
            {
                FitContentByHeight();
            }
            else
            {
                FitContentByWidth();
            }
        }

        private void FitContentByHeight()
        {
            double coef = Application.Current.MainPage.Height / origHeight;
            Content.HeightRequest = Application.Current.MainPage.Height;
            Content.WidthRequest = origWidth * coef;
            VerticalOptions = LayoutOptions.Fill;
            Translate(0, 0);
        }

        private void FitContentByWidth()
        {
            double coef = Application.Current.MainPage.Width / origWidth;
            Content.HeightRequest = coef * origHeight;
            Content.WidthRequest = Application.Current.MainPage.Width;
            VerticalOptions = LayoutOptions.FillAndExpand;
            Translate(0, 0);
        }
    }

}
