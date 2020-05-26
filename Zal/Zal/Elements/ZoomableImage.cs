using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class PinchZoom : ContentView
    {

        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 3;
        private const double OVERSHOOT = 0.15;
        private double StartScale;
        private double StartX, StartY;
        bool panIsRunning = false;
        bool isActivatedFirstTime = true;
        Task<bool> Animation;
        double origHeight;
        double origWidth;

        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        private double startMidY;
        private double originY;
        double yOffset = 0, startTargetX, startTargetY;
        private double startMidX;
        private double originX;

        public PinchZoom()
        {
            PinchGestureRecognizer pinchGesture = new PinchGestureRecognizer();
            pinchGesture.PinchUpdated += PinchUpdated;
            GestureRecognizers.Add(pinchGesture);

            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);

            var doublePan = new PanGestureRecognizer();
            doublePan.PanUpdated += DoublePan_PanUpdated;
            doublePan.TouchPoints = 2;
            GestureRecognizers.Add(doublePan);
        }

        private void DoublePan_PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            Console.Write("Double Pan");
        }

        protected override void OnChildAdded(Element child)
        {
            base.OnChildAdded(child);
            Content.AnchorX = 0.5;
            Content.AnchorY = 0.5;
        }

        private void PinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            Console.Write("Pinch");
            if (Animation != null && !Animation.IsCompleted) return;
            if (panIsRunning) CompletePan();

            if (e.Status == GestureStatus.Started)
            {
                startScale = Content.Scale;
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                StartX = e.ScaleOrigin.X * Content.Width;
                StartY = e.ScaleOrigin.Y * Content.Height;

                double renderedX = /*Content.X +*/ xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                startMidX = - deltaX * deltaWidth;
                originX = (e.ScaleOrigin.X - Content.AnchorX - deltaX) * deltaWidth;

                double renderedY = /*Content.Y +*/ yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Content.Height / (Content.Height * startScale);
                startMidY = - deltaY * deltaHeight;
                originY = (e.ScaleOrigin.Y - Content.AnchorY - deltaY) * deltaHeight;

                startTargetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                startTargetY = yOffset - (originY * Content.Height) * (currentScale - startScale);
            }

            if (e.Status == GestureStatus.Running)
            {
                double currentScale = Content.Scale + (e.Scale - 1) * startScale;
                currentScale = Math.Max(1-OVERSHOOT, currentScale);

                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                //double originX = (e.ScaleOrigin.X - Content.AnchorX - deltaX) * deltaWidth;

                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                //double originY = (e.ScaleOrigin.Y - Content.AnchorY - deltaY) * deltaHeight;

                double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                double totalX = e.ScaleOrigin.X * Content.Width;
                double totalY = e.ScaleOrigin.Y * Content.Height;

                //Console.WriteLine($"Pinch {totalX - StartX}, {totalY - StartY}");
                //Translate(originX * currentScale * Width, originY * currentScale * Height);

                double changeCoef = currentScale / startScale;
                //double changeCoef
                double dir = changeCoef > 0 ? 1 - changeCoef : -1 - changeCoef;
                Content.TranslationX = GetTranslationX(startMidX, currentScale);// -startMidX * currentScale * Width;
                Content.TranslationY = GetTranslationY(startMidY, currentScale); //-startMidY * currentScale * Height;

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
                //xOffset = Content.TranslationX;
                //yOffset = Content.TranslationY;
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
            double renderedX = xOffset;
            double deltaX = renderedX / Width;
            double deltaWidth = Width / (Content.Width * startScale);
            double midX = -deltaX * deltaWidth;
            return midX;
        }

        private double CurrentMiddleY()
        {
            double renderedY = yOffset;
            double deltaY = renderedY / Height;
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
                    //double newY = ((Y) * Scale) + yOffset;

                    //double width = (Content.Width * Content.Scale);
                    //double height = (Content.Height * Content.Scale);
                    
                    //double wBound = (width - Application.Current.MainPage.Width) / 2;
                    //double hBound = (height - Application.Current.MainPage.Height) / 2;

                    //if (newX < -wBound) newX = -wBound;
                    //if (newX > wBound) newX = wBound;
                    
                    //if (newY < -wBound) newY = -wBound;
                    //if (newY > hBound) newY = hBound;

                    //Content.TranslationX = newX;
                    //Content.TranslationY = newY;
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

            Content.HorizontalOptions = LayoutOptions.Center;
            Content.VerticalOptions = LayoutOptions.Center;
            (Content as Image).Aspect = Aspect.AspectFit;

            if (Content.Width < 1 || Content.Height < 1) return;
            if (isActivatedFirstTime)
            {
                origHeight = Content.Height;
                origWidth = Content.Width;
                isActivatedFirstTime = false;
            }

            //if (pageHeight < Content.Height)
            //{
            //    FitContentByHeight();
            //    VerticalOptions = LayoutOptions.Fill;
            //}
            //else
            //{
            //    VerticalOptions = LayoutOptions.FillAndExpand;
            //}
            //if (pageWidth < Content.Width)
            //{
            //    FitContentByWidth();
            //}

            //if ((Content.Width < pageWidth && Content.Height < pageHeight) || pageHeight < Content.Height || pageWidth < Content.Width)
            //{
                if (pageHeight / pageWidth < origHeight / origWidth)
                {
                    FitContentByHeight();
                }
                else
                {
                    FitContentByWidth();
                }
            //}
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
