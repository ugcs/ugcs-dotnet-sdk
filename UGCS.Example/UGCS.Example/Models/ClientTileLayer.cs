using MapControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using UGCS.Example.Properties;

namespace UGCS.Example.Models
{
    public class ClientTileLayer
    {
        public List<TileLayer> TileLayers { get; set; }

        public ClientTileLayer()
        {
            /*
             * 
             * <map:TileLayer x:Key="OpenStreetMap" SourceName="OpenStreetMap"
                       TileSource="http://{c}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                       MaxZoomLevel="19"/>
        <map:TileLayer x:Key="OpenCycleMap" SourceName="Thunderforest OpenCycleMap"
                       TileSource="http://{c}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png"/>
        <map:TileLayer x:Key="Landscape" SourceName="Thunderforest Landscape"
                       TileSource="http://{c}.tile.thunderforest.com/landscape/{z}/{x}/{y}.png"/>
        <map:TileLayer x:Key="Outdoors" SourceName="Thunderforest Outdoors"
                       TileSource="http://{c}.tile.thunderforest.com/outdoors/{z}/{x}/{y}.png"/>
        <map:TileLayer x:Key="Transport" SourceName="Thunderforest Transport"
                       TileSource="http://{c}.tile.thunderforest.com/transport/{z}/{x}/{y}.png"/>
        <map:TileLayer x:Key="TransportDark" SourceName="Thunderforest Transport Dark"
                       TileSource="http://{c}.tile.thunderforest.com/transport-dark/{z}/{x}/{y}.png"
                       Foreground="White" Background="Black"/>
        <map:TileLayer x:Key="MapQuest" SourceName="MapQuest OpenStreetMap"
                       TileSource="http://otile{n}.mqcdn.com/tiles/1.0.0/osm/{z}/{x}/{y}.jpg"
                       MaxZoomLevel="19"/>
        <map:TileLayer x:Key="Seamarks" SourceName="Seamarks"
                       TileSource="http://tiles.openseamap.org/seamark/{z}/{x}/{y}.png"
                       MinZoomLevel="9" MaxZoomLevel="18"/>

        <map:GoogleMapsTileLayer x:Key="GoogleSatellite" SourceName="GoogleSatellite" 
                       TileSource="https://mt1.google.com/vt/lyrs=s&amp;x={x}&amp;y={y}&amp;z={z}" />

        <map:GoogleMapsTileLayer x:Key="GoogleHybrid" SourceName="GoogleHybrid"                                 
                       TileSource="https://mt1.google.com/vt/lyrs=y&amp;x={x}&amp;y={y}&amp;z={z}" />

        <map:GoogleMapsTileLayer x:Key="GoogleMap" SourceName="GoogleMap"               
                       TileSource="https://mt1.google.com/vt/lyrs=m&amp;x={x}&amp;y={y}&amp;z={z}" />

        <map:UCSMapTilesLayer x:Key="UCSMap" SourceName="UCSMap"               
                       TileSource="{Binding UCSMapSource}" />

        <map:TileLayer x:Key="WorldOsm" SourceName="World OSM WMS"
                       Description="[World OSM WMS](http://www.osm-wms.de/) © [OpenStreetMap Contributors](http://www.openstreetmap.org/copyright)"
                       TileSource="http://129.206.228.72/cached/osm?SERVICE=WMS&amp;VERSION=1.1.1&amp;REQUEST=GetMap&amp;LAYERS=osm_auto:all&amp;STYLES=&amp;SRS=EPSG:900913&amp;BBOX={W},{S},{E},{N}&amp;WIDTH={X}&amp;HEIGHT={Y}&amp;FORMAT=image/png"/>

             */ 
            TileLayers = new List<TileLayer>()
            {
                new TileLayer() {
                    SourceName = "Open Street Map",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.openstreetmap.org/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Open Cycle Map",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.thunderforest.com/cycle/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Landscape",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.thunderforest.com/landscape/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Outdoors",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.thunderforest.com/outdoors/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Transport",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.thunderforest.com/transport/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Transport Dark",
                    TileSource = new TileSource { UriFormat = "http://{c}.tile.thunderforest.com/transport-dark/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.White)
                },
                new TileLayer() {
                    SourceName = "Map Quest",
                    TileSource = new TileSource { UriFormat = "http://otile{n}.mqcdn.com/tiles/1.0.0/osm/{z}/{x}/{y}.jpg" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new TileLayer() {
                    SourceName = "Seamarks",
                    TileSource = new TileSource { UriFormat = "http://tiles.openseamap.org/seamark/{z}/{x}/{y}.png" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                },
                new GoogleMapsTileLayer() {
                    SourceName = "Google (satellite)",
                    TileSource = new TileSource { UriFormat = "https://mt1.google.com/vt/lyrs=s&x={x}&y={y}&z={z}" },
                    DrawLinesColor = new SolidColorBrush(Colors.Lavender)
                },
                new GoogleMapsTileLayer() {
                    SourceName = "Google (hybrid)",
                    TileSource = new TileSource { UriFormat = "https://mt1.google.com/vt/lyrs=y&x={x}&y={y}&z={z}" },
                    DrawLinesColor = new SolidColorBrush(Colors.Lavender)
                },
                new GoogleMapsTileLayer() {
                    SourceName = "Google (map)",
                    TileSource = new TileSource { UriFormat = "https://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}" },
                    DrawLinesColor = new SolidColorBrush(Colors.Black)
                }
            };
        }
    }
}
