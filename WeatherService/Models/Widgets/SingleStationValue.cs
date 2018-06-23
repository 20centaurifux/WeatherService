namespace WeatherService.Models.Widgets
{
    public class SingleStationValue<T> : ASingleStation
    {
        T _value = default(T);

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