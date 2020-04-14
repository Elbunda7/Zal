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
        Task<bool> Animation;

        double currentScale = 1;
        double startScale = 1;
        double xOffset = 0;
        double yOffset = 0, startTargetX, startTargetY;

        public PinchZoom()
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
                StartX = e.ScaleOrigin.X * Content.Width;
                StartY = e.ScaleOrigin.Y * Content.Height;

                double renderedX = Content.X + xOffset;
                double deltaX = renderedX / Width;
                double deltaWidth = Width / (Content.Width * startScale);
                double originX = (e.ScaleOrigin.X - Content.AnchorX - deltaX) * deltaWidth;

                double renderedY = Content.Y + yOffset;
                double deltaY = renderedY / Height;
                double deltaHeight = Height / (Content.Height * startScale);
                double originY = (e.ScaleOrigin.Y - Content.AnchorY - deltaY) * deltaHeight;

                startTargetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                startTargetY = yOffset - (originY * Content.Height) * (currentScale - startScale);
            }

            if (e.Status == GestureStatus.Running)
            {
                double currentScale = Content.Scale + (e.Scale - 1) * startScale;
                currentScale = Math.Max(1-OVERSHOOT, currentScale);

                //double renderedX = Content.X + xOffset;
                //double deltaX = renderedX / Width;
                //double deltaWidth = Width / (Content.Width * startScale);
                //double originX = (e.ScaleOrigin.X - Content.AnchorX - deltaX) * deltaWidth;

                //double renderedY = Content.Y + yOffset;
                //double deltaY = renderedY / Height;
                //double deltaHeight = Height / (Content.Height * startScale);
                //double originY = (e.ScaleOrigin.Y - Content.AnchorY - deltaY) * deltaHeight;

                //double targetX = xOffset - (originX * Content.Width) * (currentScale - startScale);
                //double targetY = yOffset - (originY * Content.Height) * (currentScale - startScale);

                double totalX = e.ScaleOrigin.X * Content.Width;
                double totalY = e.ScaleOrigin.Y * Content.Height;

                //Translate(totalX - StartX, totalY - StartY);

                if (currentScale > 1)
                {
                    Content.TranslationX = Math.Min(Content.Width / 2 * (currentScale - 1), Math.Max(startTargetX, -Content.Width / 2 * (currentScale - 1)));
                    Content.TranslationY = Math.Min(Content.Height / 2 * (currentScale - 1), Math.Max(startTargetY, -Content.Height / 2 * (currentScale - 1)));
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
                xOffset = Content.TranslationX;
                yOffset = Content.TranslationY;
                if (Content.Scale > MAX_SCALE)
                {
                    Content.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                    Animation = Content.TranslateTo(0, 0, 250, Easing.SpringOut);
                }
                else if (Content.Scale < MIN_SCALE)
                    Content.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
            }
        }

        public void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
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

            if (newY < -wBound) newY = -wBound;
            if (newY > hBound) newY = hBound;

            Content.TranslationX = newX;
            Content.TranslationY = newY;
        }

        private void CompletePan()
        {
            xOffset = Content.TranslationX;
            yOffset = Content.TranslationY;
            panIsRunning = false;
        }

    }

}
