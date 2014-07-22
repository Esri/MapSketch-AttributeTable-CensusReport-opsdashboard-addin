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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OperationsDashboardAddIns.Controls
{
  internal class ColorPicker : StackPanel
  {
    public static readonly DependencyProperty ColorProperty =
        DependencyProperty.Register("Color", typeof(Color), typeof(ColorPicker), new UIPropertyMetadata());
    public Color Color 
    { 
      get { return (Color)GetValue(ColorProperty); }
      set { SetValue(ColorProperty, value); }
    }

    public ColorPicker()
    {
      Orientation = 0;  //0 = horizontal
      AddColorsToSwatch();
      Color = (Color)ColorConverter.ConvertFromString("#000000");
    }

    private void AddColorsToSwatch()
    {
      ColorButton[] colorButtons = new ColorButton[]
      { 
        new ColorButton((Color)ColorConverter.ConvertFromString("#000000")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#FFFFFF")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#8064A2")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#4BACC6")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#F79646")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#FF0000")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#FFFF00")),
        new ColorButton((Color)ColorConverter.ConvertFromString("#92D050"))
      };

      foreach (ColorButton btn in colorButtons)
      {
        btn.Click += (s, e) => { Color = (btn.Background as SolidColorBrush).Color; };
        this.Children.Add(btn);
      }
    }

    class ColorButton : Button
    {
      public ColorButton(Color color)
      {
        Background = new SolidColorBrush(color);
        Width = 25;
        Height = Width;
        Margin = new Thickness(3, 3, 3, 3);
        VerticalAlignment = System.Windows.VerticalAlignment.Center;
        HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      }
    }

  }
}
