﻿using System;
using System.Collections.Generic;

namespace WeatherService.Models.View
{
    [Serializable]
    public class DashboardItemUpdate
    {
        public string WidgetId { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public IEnumerable<string> Filter { get; set; }
    }
}