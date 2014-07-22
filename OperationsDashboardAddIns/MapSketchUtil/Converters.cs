//Copyright 2014 Esri
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.​

using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace OperationsDashboardAddIns
{
  public class LineWidthToBoolConverter: MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null || parameter == null)
        return null;

      LineWidth lineWidth = (LineWidth)value;
      if ((LineWidth)parameter == LineWidth.Thin && lineWidth == LineWidth.Thin)
        return true;
      if ((LineWidth)parameter == LineWidth.Medium && lineWidth == LineWidth.Medium)
        return true;
      if ((LineWidth)parameter == LineWidth.Thick && lineWidth == LineWidth.Thick)
        return true;

      return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if ((LineWidth)parameter == LineWidth.Thin)
        return LineWidth.Thin;
      if ((LineWidth)parameter == LineWidth.Medium)
        return LineWidth.Medium;
      if ((LineWidth)parameter == LineWidth.Thick)
        return LineWidth.Thick;

      return LineWidth.Thin;
    }
  }

  public class ColorToBrushConverter : MarkupExtension, IValueConverter
  {
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      return this;
    }

    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value == null)
        return null;

      if (value is Color)
        return new SolidColorBrush((Color)value);

      return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}
