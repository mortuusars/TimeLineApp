using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TimeLine
{
    [ValueConversion(typeof(string), typeof(BitmapImage))]
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            switch (value) {
                case null:
                    return null;
                case string svalue:
                    return new BitmapImage(new Uri((string)value, UriKind.RelativeOrAbsolute));
            }

            Type type = value.GetType();
            throw new InvalidOperationException("Unsupported type [" + type.Name + "]");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return null;
        }
    }
}
