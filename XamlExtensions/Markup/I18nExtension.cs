﻿using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using XamlExtensions.ExtensionMethods;

namespace XamlExtensions.Markup
{
    [MarkupExtensionReturnType(typeof(object))]
    public class I18nExtension : MarkupExtension
    {
        private static readonly I18nResourceConverter I18NResourceConverter = new I18nResourceConverter();

        [ConstructorArgument(nameof(Key))]
        public ComponentResourceKey Key { get; set; }

        public I18nExtension(ComponentResourceKey key) => Key = key;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Key == null)
                throw new NullReferenceException($"{nameof(Key)} cannot be null at the same time.");

            if (!(serviceProvider.GetService(typeof(IProvideValueTarget)) is IProvideValueTarget provideValueTarget))
                throw new ArgumentException(
                    $"The {nameof(serviceProvider)} must implement {nameof(IProvideValueTarget)} interface.");

            if (provideValueTarget.TargetObject?.GetType().FullName == "System.Windows.SharedDp") return this;

            return new Binding(nameof(I18nSource.Value))
            {
                Source = new I18nSource(Key, provideValueTarget.TargetObject),
                Mode = BindingMode.OneWay,
                Converter = I18NResourceConverter
            }.ProvideValue(serviceProvider);
        }

        private class I18nResourceConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                switch (value)
                {
                    case Bitmap bitmap:
                        return bitmap.ToBitmapSource();
                    case Icon icon:
                        return icon.ToImageSource();
                    default:
                        return value;
                }
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotSupportedException();
            }
        }
    }
}
