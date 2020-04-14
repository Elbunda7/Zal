using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Zal.Elements
{
    public class ZoomImage : Image
    {
        private const double MIN_SCALE = 1;
        private const double MAX_SCALE = 4;
        private const double OVERSHOOT = 0.15;
        private double StartScale, currentScale, LastScale;
        private double StartX, StartY, yOffset, xOffset;

        public ZoomImage()
        {
            var pinch = new PinchGestureRecognizer();
            pinch.PinchUpdated += OnPinchUpdated;
            GestureRecognizers.Add(pinch);

            var pan = new PanGestureRecognizer();
            pan.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(pan);

            var tap = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            tap.Tapped += OnTapped;
            GestureRecognizers.Add(tap);

            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            Scale = MIN_SCALE;
            TranslationX = TranslationY = 0;
            AnchorX = AnchorY = 0;
            return base.OnMeasure(widthConstraint, heightConstraint);
        }

        private void OnTapped(object sender, EventArgs e)
        {
            if (Scale > MIN_SCALE)
            {
                this.ScaleTo(MIN_SCALE, 250, Easing.CubicInOut);
                this.TranslateTo(0, 0, 250, Easing.CubicInOut);
            }
            else
            {
                AnchorX = AnchorY = 0.5; //TODO tapped position
                this.ScaleTo(MAX_SCALE, 250, Easing.CubicInOut);
            }
        }

        private void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            //switch (e.StatusType)
            //{
            //    case GestureStatus.Started:
            //        StartX = (1 - AnchorX) * Width;
            //        StartY = (1 - AnchorY) * Height;
            //        break;
            //    case GestureStatus.Running:
            //        AnchorX = Clamp(1 - (StartX + e.TotalX) / Width, 0, 1);
            //        AnchorY = Clamp(1 - (StartY + e.TotalY) / Height, 0, 1);
            //        break;
            //}
        }

        private void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        {
            switch (e.Status)
            {
                case GestureStatus.Started:
                    StartScale = Scale;
                    LastScale = Scale;
                    AnchorX = e.ScaleOrigin.X;
                    AnchorY = e.ScaleOrigin.Y;
                    break;
                case GestureStatus.Running:
                    double current = Scale + (e.Scale - 1) * StartScale;
                    Console.WriteLine(Scale + " * " + e.Scale);                    
                    Scale = Clamp(current, MIN_SCALE * (1 - OVERSHOOT), MAX_SCALE * (1 + OVERSHOOT));
                    break;
                case GestureStatus.Completed:
                    if (Scale > MAX_SCALE)
                        this.ScaleTo(MAX_SCALE, 250, Easing.SpringOut);
                    else if (Scale < MIN_SCALE)
                        this.ScaleTo(MIN_SCALE, 250, Easing.SpringOut);
                    break;
            }
        }

        //void OnPinchUpdated(object sender, PinchGestureUpdatedEventArgs e)
        //{
        //    if (e.Status == GestureStatus.Started)
        //    {
        //        // Store the current scale factor applied to the wrapped user interface element,
        //        // and zero the components for the center point of the translate transform.
        //        StartScale = Scale;
        //        AnchorX = 0;
        //        AnchorY = 0;
        //    }
        //    if (e.Status == GestureStatus.Running)
        //    {
        //        Console.WriteLine(e.Scale);
        //        // Calculate the scale factor to be applied.
        //        double currentScale = Scale + (e.Scale - 1) * StartScale;
        //        currentScale = Math.Max(1, currentScale);

        //        // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
        //        // so get the X pixel coordinate.
        //        double renderedX = X + xOffset;
        //        double deltaX = renderedX / Width;
        //        double deltaWidth = Width / (Width * StartScale);
        //        double originX = (e.ScaleOrigin.X - deltaX) * deltaWidth;

        //        // The ScaleOrigin is in relative coordinates to the wrapped user interface element,
        //        // so get the Y pixel coordinate.
        //        double renderedY = Y + yOffset;
        //        double deltaY = renderedY / Height;
        //        double deltaHeight = Height / (Height * StartScale);
        //        double originY = (e.ScaleOrigin.Y - deltaY) * deltaHeight;

        //        // Calculate the transformed element pixel coordinates.
        //        double targetX = xOffset - (originX * Width) * (currentScale - StartScale);
        //        double targetY = yOffset - (originY * Height) * (currentScale - StartScale);

        //        // Apply translation based on the change in origin.
        //        TranslationX = Clamp(targetX, -Width * (currentScale - 1), 0);
        //        TranslationY = Clamp(targetY, -Height * (currentScale - 1), 0);

        //        // Apply scale factor.
        //        Scale = currentScale;
        //    }
        //    if (e.Status == GestureStatus.Completed)
        //    {
        //        // Store the translation delta's of the wrapped user interface element.
        //        xOffset = TranslationX;
        //        yOffset = TranslationY;
        //    }
        //}

        private T Clamp<T>(T value, T minimum, T maximum) where T : IComparable
        {
            if (value.CompareTo(minimum) < 0)
                return minimum;
            else if (value.CompareTo(maximum) > 0)
                return maximum;
            else
                return value;
        }
    }
}