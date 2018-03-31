namespace WeatherService.Models.Widgets
{
    public class SingleStationValue<T> : ASingleStation
    {
        private object _value = null;

        // Generic types are not allowed to be nullable :(
        public bool IsNull
        {
            get
            {
                return _value == null;
            }
        }

        public T Value
        {
            set
            {
                _value = value;
            }

            get
            {
                return (T)_value;
            }
        }
    }
}