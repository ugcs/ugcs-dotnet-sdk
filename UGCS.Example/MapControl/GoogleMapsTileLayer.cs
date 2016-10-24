// XAML Map Control - http://xamlmapcontrol.codeplex.com/
// © 2016 Clemens Fischer
// Licensed under the Microsoft Public License (Ms-PL)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Xml;
#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
#else
using System.Windows;
using System.Windows.Media.Imaging;
#endif

namespace MapControl
{
    /// <summary>
    /// Displays Bing Maps tiles. The static ApiKey property must be set to a Bing Maps API Key.
    /// </summary>
    public class GoogleMapsTileLayer : TileLayer
    {
        public enum MapMode
        {
            Road, Aerial, AerialWithLabels
        }

        public GoogleMapsTileLayer()
            : this(new TileImageLoader() { HttpUserAgent = "Mozilla/5.0" })
        {
        }

        public GoogleMapsTileLayer(TileImageLoader tileImageLoader)
            : base(tileImageLoader)
        {
            tileImageLoader.HttpUserAgent = "Mozilla/5.0";
        }
    }
}
