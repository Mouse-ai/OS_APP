using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace FCFS_Lab.Converters;

public class MultiplyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int burst) return Math.Max(burst * 22, 45); // 22px за 1 ед. времени, минимум 45px
        return 45.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}