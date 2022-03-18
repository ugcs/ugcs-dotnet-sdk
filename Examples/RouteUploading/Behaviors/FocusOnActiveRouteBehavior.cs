using MapControl;
using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UgCS.SDK.Examples.Common;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.UploadRouteFromSelectedWaypoint.Behaviors
{
    public sealed class FocusOnActiveRouteBehavior: Behavior<Map>, IDisposable
    {
        public void Dispose()
        {
            App.Current.ActiveRouteChanged -= app_ActiveRouteChanged;
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            App.Current.ActiveRouteChanged += app_ActiveRouteChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            App.Current.ActiveRouteChanged -= app_ActiveRouteChanged;
        }

        private void app_ActiveRouteChanged(object sender, EventArgs e)
        {
            ProcessedRoute activeRoute = App.Current.ActiveRoute;
            if (activeRoute == null)
                return;

            IList<Waypoint> waypoints = activeRoute.GetWaypoints();
            if (waypoints.Count == 0)
                return;

            AssociatedObject.ZoomToBounds(
                waypoints
                .Select(x => new Location(
                    x.Latitude.ToDegrees(), 
                    x.Longitude.ToDegrees()))
                .GetBoundingBox()
                .Extend(20));
        }
    }
}
