// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// © 2016 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Windows;
using System.Windows.Input;

namespace MapControl
{
    /// <summary>
    /// MapBase with default input event handling.
    /// </summary>
    public class Map : MapBase
    {
        public static readonly DependencyProperty ManipulationModeProperty = DependencyProperty.Register(
            "ManipulationMode", typeof(ManipulationModes), typeof(Map), new PropertyMetadata(ManipulationModes.All));

        public static readonly DependencyProperty MouseWheelZoomDeltaProperty = DependencyProperty.Register(
            "MouseWheelZoomDelta", typeof(double), typeof(Map), new PropertyMetadata(1d));

        private Point? mousePosition;

        static Map()
        {
            IsManipulationEnabledProperty.OverrideMetadata(typeof(Map), new FrameworkPropertyMetadata(true));
        }

        /// <summary>
        /// Gets or sets a value that specifies how the map control handles manipulations.
        /// </summary>
        public ManipulationModes ManipulationMode
        {
            get { return (ManipulationModes)GetValue(ManipulationModeProperty); }
            set { SetValue(ManipulationModeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the amount by which the ZoomLevel property changes during a MouseWheel event.
        /// </summary>
        public double MouseWheelZoomDelta
        {
            get { return (double)GetValue(MouseWheelZoomDeltaProperty); }
            set { SetValue(MouseWheelZoomDeltaProperty, value); }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!IsMouseZoomEnabled)
            {
                return;
            }
            base.OnMouseWheel(e);
            var zoomDelta = MouseWheelZoomDelta * (double)e.Delta / 2000d;
            if (ManipulationMode == ManipulationModes.Scale)
            {
                ZoomMap(e.GetPosition(this), TargetZoomLevel + zoomDelta, false);

            }
            else
            {
                ZoomMap(e.GetPosition(this), TargetZoomLevel + zoomDelta);
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (!IsMouseMoveEnabled)
            {
                return;
            }
            base.OnMouseLeftButtonDown(e);

            if (CaptureMouse())
            {
                mousePosition = e.GetPosition(this);
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!IsMouseMoveEnabled)
            {
                return;
            }
            base.OnMouseLeftButtonUp(e);

            if (mousePosition.HasValue)
            {
                mousePosition = null;
                ReleaseMouseCapture();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (!IsMouseMoveEnabled)
            {
                return;
            }
            base.OnMouseMove(e);

            if (mousePosition.HasValue)
            {
                var position = e.GetPosition(this);
                TranslateMap(position - mousePosition.Value);
                mousePosition = position;
            }
        }

        protected override void OnManipulationStarted(ManipulationStartedEventArgs e)
        {
            base.OnManipulationStarted(e);

            Manipulation.SetManipulationMode(this, ManipulationMode);
        }

        protected override void OnManipulationDelta(ManipulationDeltaEventArgs e)
        {
            if (!IsMouseZoomEnabled)
            {
                return;
            }
            base.OnManipulationDelta(e);
            if (ManipulationMode == ManipulationModes.Scale)
            {   
                TransformMapZoom((e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2d);
            }
            else
            {
                TransformMap(e.ManipulationOrigin,
                    e.DeltaManipulation.Translation, e.DeltaManipulation.Rotation,
                    (e.DeltaManipulation.Scale.X + e.DeltaManipulation.Scale.Y) / 2d);
            }
        }
    }
}
