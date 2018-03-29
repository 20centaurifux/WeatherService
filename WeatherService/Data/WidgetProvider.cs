using Microsoft.AspNetCore.Hosting;
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
        private readonly string _filename;
        private IEnumerable<Widget> _widgets;
        private IEnumerable<WeatherStation> _stations;

        public WidgetProvider(string filename)
        {
            _filename = filename;
        }

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

        public IEnumerable<WeatherStation> GetSupportedStations(Widget widget)
        {
            return GetSupportedStations(widget, false);
        }

        public IEnumerable<WeatherStation> GetSupportedPublicStations(Widget widget)
        {
            return GetSupportedStations(widget, true);
        }

        private IEnumerable<WeatherStation> GetSupportedStations(Widget widget, bool publicOnly)
        {
            var stations = new List<WeatherStation>();

            foreach (var s in WeatherStations.Where(s => !publicOnly || s.IsPublic))
            {
                if(widget.RequiresTemperature && !s.HasTemperature)
                {
                    continue;
                }

                if (widget.RequiresPressure && !s.HasPressure)
                {
                    continue;
                }

                if (widget.RequiresHumidity && !s.HasHumidity)
                {
                    continue;
                }

                if (widget.RequiresUV && !s.HasUV)
                {
                    continue;
                }

                stations.Add(s);
            }

            return stations;
        }

        private IEnumerable<WeatherStation> WeatherStations
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