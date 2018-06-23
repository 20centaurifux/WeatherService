using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using WeatherService.Models;

namespace WeatherService.Data
{
    public class WidgetProvider
    {
        readonly string _filename;
        IEnumerable<Widget> _widgets;
        IEnumerable<WeatherStation> _stations;

        public WidgetProvider(string filename) => _filename = filename;

        public IEnumerable<Widget> LoadWidgets()
        {
            if (_widgets == null)
            {
                _widgets = JsonConvert.DeserializeObject<Widget[]>(File.ReadAllText(_filename));
            }

            return _widgets;
        }

        public Widget LoadWidget(Guid guid)
        {
            var widgets = LoadWidgets();

            return widgets.FirstOrDefault(w => w.Guid.Equals(guid));
        }

        public IEnumerable<WeatherStation> GetSupportedStations(Widget widget) => GetSupportedStations(widget, false);

        public IEnumerable<WeatherStation> GetSupportedPublicStations(Widget widget) => GetSupportedStations(widget, true);

        public static bool WidgetCompatibleToStation(Widget widget, WeatherStation station)
        {
            if (widget.RequiresTemperature && !station.HasTemperature)
            {
                return false;
            }

            if (widget.RequiresPressure && !station.HasPressure)
            {
                return false;
            }

            if (widget.RequiresHumidity && !station.HasHumidity)
            {
                return false;
            }

            if (widget.RequiresUV && !station.HasUV)
            {
                return false;
            }

            if (widget.RequiresWebcamUrl && string.IsNullOrEmpty(station.WebcamUrl))
            {
                return false;
            }

            return true;
        }

        public bool ValidateStationIds(Widget widget, IEnumerable<string> stationIds) => ValidateStationIds(widget, GetSupportedStations(widget), stationIds);

        public bool ValidatePublicStationIds(Widget widget, IEnumerable<string> stationIds) => ValidateStationIds(widget, GetSupportedPublicStations(widget), stationIds);

        public bool ValidateStationIds(Widget widget, IEnumerable<WeatherStation> supportedStations, IEnumerable<string> stationIds)
        {
            foreach(var idToCheck in stationIds)
            {
                var station = supportedStations.FirstOrDefault(s => s.Id.Equals(idToCheck));

                if(station == null)
                {
                    return false;
                }

                if(!WidgetCompatibleToStation(widget, station))
                {
                    return false;
                }
            }

            return true;
        }

        IEnumerable<WeatherStation> GetSupportedStations(Widget widget, bool publicOnly)
        {
            var stations = new List<WeatherStation>();

            foreach (var s in WeatherStations.Where(s => !publicOnly || s.IsPublic))
            {
                if (WidgetCompatibleToStation(widget, s))
                {
                    stations.Add(s);
                }
            }

            return stations;
        }

        IEnumerable<WeatherStation> WeatherStations
        {
            get
            {
                if(_stations == null)
                {
                    using (var db = new WeatherDb())
                    {
                        _stations = db.WeatherStation.Select(s => s).ToArray();
                    }
                }

                return _stations;
            }
        }
    }
}