using System.Collections.Generic;
using System.Linq;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using AdaptiveTileExtensions;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace NiceLookingTiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly List<Place> _places = new List<Place>
        {
            new Place("Poole", "Sunny")
            {
                Forecast = new List<WeatherItem>
                {
                    new WeatherItem("Sunny", 21, 11, "North", "Fri"),
                    new WeatherItem("CloudySunny", 18, 10, "South", "Sat"),
                    new WeatherItem("Sunny", 21, 14, "North", "Sun"),
                    new WeatherItem("Sunny", 18, 10, "South", "Mon"),
                    new WeatherItem("Sunny", 21, 14, "North", "Tue"),
                }
            },
            new Place("Seattle", "CloudySunny")
            {
                Forecast = new List<WeatherItem>
                {
                    new WeatherItem("CloudySunny", 21, 10, "North", "Fri"),
                    new WeatherItem("CloudySunny", 18, 10, "South", "Sat"),
                    new WeatherItem("CloudySunny", 21, 10, "North", "Sun"),
                    new WeatherItem("CloudySunny", 18, 10, "North", "Mon"),
                    new WeatherItem("CloudySunny", 21, 10, "North", "Tue"),
                }
            }
        };
            
        public MainPage()
        {
            this.InitializeComponent();
        }
        const string UriTemplate = "ms-appx:///Assets/{0}.png";
        const string WallpaperUriTemplate = "ms-appx:///Assets/{0}Wallpaper.png";
        private void WeatherTilesButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Clear();
            updater.EnableNotificationQueue(true);
            

            foreach (var place in _places)
            {
                var tile = AdaptiveTile.CreateTile();
                var wideTile = CreateTile(TemplateType.TileWide, place);
                var mediumTile = CreateTile(TemplateType.TileMedium, place, 2);

                tile.Tiles.Add(wideTile);
                tile.Tiles.Add(mediumTile);

                var notification = tile.GetNotification();
                
                updater.Update(notification);
            }
        }

        private static TileBinding CreateTile(TemplateType size, Place place, int limit = 10)
        {
            var tileBinding = TileBinding.Create(size);
            tileBinding.DisplayName = place.PlaceName;
            tileBinding.Branding = Branding.Name;
            tileBinding.OverlayOpacity = 20;

            tileBinding.Add(new TileImage(ImagePlacement.Background) {Source = string.Format(WallpaperUriTemplate, place.OverallForecast)});

            foreach (var forecast in place.Forecast.Take(limit))
            {
                var subGroup = new SubGroup {Width = 1};
                subGroup.AddText(new Text(forecast.Day) {Alignment = Alignment.Center});
                subGroup.AddImage(new TileImage(ImagePlacement.Inline) {RemoveMargin = true, Source = string.Format(UriTemplate, forecast.Forecast)});
                subGroup.AddText(new Text($"{forecast.TemperatureHigh}°") {Alignment = Alignment.Center});
                subGroup.AddText(new Text($"{forecast.TemperatureLow}°") {Alignment = Alignment.Center, Style = TextStyle.Caption, IsSubtleStyle = true});

                tileBinding.AddSubgroup(subGroup);
            }
            return tileBinding;
        }
    }

    public class Place
    {
        public Place(string placeName, string overallForecast)
        {
            PlaceName = placeName;
            OverallForecast = overallForecast;
            Forecast = new List<WeatherItem>();
        }

        public List<WeatherItem> Forecast { get; set; }
        public string PlaceName { get; set; }
        public string OverallForecast { get; set; }
    }

    public class WeatherItem
    {
        public WeatherItem(string forecast, int temperatureHigh, int temperatureLow, string windDirection, string day)
        {
            Forecast = forecast;
            TemperatureHigh = temperatureHigh;
            TemperatureLow = temperatureLow;
            WindDirection = windDirection;
            Day = day;
        }

        public string Forecast { get; set; }
        public int TemperatureHigh { get; set; }
        public int TemperatureLow { get; set; }
        public string WindDirection { get; set; }
        public string Day { get; set; }
    }
}
