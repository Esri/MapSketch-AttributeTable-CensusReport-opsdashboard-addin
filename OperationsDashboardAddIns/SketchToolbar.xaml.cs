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

using ESRI.ArcGIS.Client.Symbols;
using ESRI.ArcGIS.OperationsDashboard;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using client = ESRI.ArcGIS.Client;

namespace OperationsDashboardAddIns
{
  /// <summary>
  /// A MapToolbar is a customization for Operations Dashboard for ArcGIS which can be set to replace the configured toolbar 
  /// of a map widget. 
  /// 
  /// This map tool provides users with the tools to sketch on the map. The map with the sketch can then be saved as a map image, 
  /// or can be emailed
  /// </summary>
  public partial class SketchToolbar : UserControl, IMapToolbar, INotifyPropertyChanged
  {
    //The map widget that the toolbar has been installed to.
    private MapWidget _mapWidget = null;

    //Graphic layer that stores the map sketch
    private client.GraphicsLayer sketchLayer;

    //The sketch graphic
    private client.Graphic sketch;

    //Width of the sketch line (in pixel)
    private int selectedWidth = 2;

    //Width of the sketch line (represented by the LineWidth enum)
    private LineWidth _selectedLineWidth;
    public LineWidth SelectedLineWidth
    {
      get { return _selectedLineWidth; }
      set
      {
        SetField(ref _selectedLineWidth, value, () => SelectedLineWidth);

        switch (SelectedLineWidth)
        {
          case (LineWidth.Thin):
            selectedWidth = 2; break;
          case (LineWidth.Medium):
            selectedWidth = 4; break;
          case (LineWidth.Thick):
            selectedWidth = 6; break;
          default:
            selectedWidth = 2; break;
        }
      }
    }

    #region Properties for updating the UI elements
    private bool chooseColorIsChecked;
    public bool ChooseColorIsChecked
    {
      get { return chooseColorIsChecked; }
      set { SetField(ref chooseColorIsChecked, value, () => ChooseColorIsChecked); }
    }

    private Color _selectedColor;
    public Color SelectedColor
    {
      get { return _selectedColor; }
      set
      {
        SetField(ref _selectedColor, value, () => SelectedColor);

        ChooseColorIsChecked = false;
      }
    }
    #endregion

    public SketchToolbar(MapWidget mapWidget)
    {
      InitializeComponent();

      // Store a reference to the MapWidget that the toolbar has been installed to.
      _mapWidget = mapWidget;

      DataContext = this;
    }

    /// <summary>
    /// OnActivated is called when the toolbar is installed into the map widget.
    /// </summary>
    public void OnActivated()
    {
      if (_mapWidget == null)
        return;

      //Register left button down i.e. sketch begins
      //Register mouse move i.e. sketch continues
      //Register left button up i.e. sketch finishes
      _mapWidget.Map.MouseLeftButtonDown += Map_MouseLeftButtonDown;
      _mapWidget.Map.MouseMove += Map_MouseMove;
      _mapWidget.Map.MouseLeftButtonUp += Map_MouseLeftButtonUp;

      //Crate the sketch layer and add it to the map
      sketchLayer = new client.GraphicsLayer();
      _mapWidget.Map.Layers.Add(sketchLayer);

      //Set the default color and width of the sketch
      SelectedColor = (Color)ColorConverter.ConvertFromString("#000000");
      SelectedLineWidth = LineWidth.Thin;
    }

    /// <summary>
    ///  OnDeactivated is called before the toolbar is uninstalled from the map widget. 
    /// </summary>
    public void OnDeactivated()
    {
      if (_mapWidget == null)
        return;

      //Unregister the events
      _mapWidget.Map.MouseLeftButtonDown -= Map_MouseLeftButtonDown;
      _mapWidget.Map.MouseMove -= Map_MouseMove;
      _mapWidget.Map.MouseLeftButtonUp -= Map_MouseLeftButtonUp;

      //Discard the sketch layer
      _mapWidget.Map.Layers.Remove(sketchLayer);
      sketchLayer = null;
    }

    #region Create map sketch based on user's interaction
    //Start a new sketch
    void Map_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (sketch != null)
        return;

      //Mark the event arg as handled so the map won't treat the action as a pan action
      e.Handled = true;

      //Get the map point (first point of the sketch)
      client.Map map = sender as client.Map;
      client.Geometry.MapPoint mapPoint = map.ScreenToMap(e.GetPosition(map));

      //Create the geometry (polyline) of the sketch and add the first point to it
      client.Geometry.Polyline pl = new client.Geometry.Polyline();
      pl.SpatialReference = mapPoint.SpatialReference;
      pl.Paths.Add(new client.Geometry.PointCollection());
      pl.Paths[0].Add(mapPoint);

      //Create the sketch graphic with the geometry and the symbol
      sketch = new client.Graphic();
      sketch.Symbol = new SimpleLineSymbol()
      {
        Color = new SolidColorBrush(SelectedColor),
        Width = selectedWidth
      };
      sketch.Geometry = pl;
      sketch.Geometry.SpatialReference = _mapWidget.Map.SpatialReference;

      //Add the sketch graphic to the layer
      sketchLayer.Graphics.Add(sketch);
    }

    //For each move, get the map point and add it as a vertex of the polyline
    void Map_MouseMove(object sender, MouseEventArgs e)
    {
      if (sketch == null)
        return;

      //Get the map point 
      client.Map map = sender as client.Map;
      client.Geometry.MapPoint mapPoint = map.ScreenToMap(e.GetPosition(map));

      //Add the map point to the sketch
      client.Geometry.Polyline polyline = sketch.Geometry as client.Geometry.Polyline;
      polyline.Paths[0].Add(mapPoint);
    }

    //Finish the sketch when left mouse button is up
    void Map_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      sketch = null;
    }
    #endregion

    #region Save, email, or discard the sketch
    //Clear all graphics on the sketch layer
    private void DiscardSketch_Click(object sender, RoutedEventArgs e)
    {
      sketchLayer.Graphics.Clear();
    }

    // When the user is finished with the toolbar, revert to the default toolbar.
    private void DoneButton_Click(object sender, RoutedEventArgs e)
    {
      if (_mapWidget != null)
      {
        _mapWidget.SetToolbar(null);
      }
    }

    //Capture the map image then prompt user to save
    private void SaveSketch_Click(object sender, RoutedEventArgs e)
    {
      PromptToSaveMapImage();
    }

    //Capture the map image, then pass some default settings as well as the image attachment to Outlook.exe 
    private void MailSketch_Click(object sender, RoutedEventArgs e)
    {
      BitmapEncoder bitmapEncoder = CaptureMapImage();
      if (bitmapEncoder == null)
        return;

      string mapCaption = _mapWidget.Caption;

      //replace any invalid char and space in the map title by an underscore
      char[] invalidChars = Path.GetInvalidFileNameChars();
      foreach (char c in invalidChars)
        mapCaption = mapCaption.Replace(c.ToString(), "_");
      mapCaption = mapCaption.Replace(' ', '_');
      string time = DateTime.Now.ToString("yyyy_MM_dd_hh_ss_tt");
      string fileName = string.Format("{0}_at_{1}", mapCaption, time);
      string tempFolder = Path.GetTempPath();
      string filePath = string.Format("{0}{1}.jpg", tempFolder, fileName);

      bool saveMapToTemp = SaveMapImage(bitmapEncoder, filePath);
      if (saveMapToTemp == false)
        MessageBox.Show("Error sending map image");

      string subject = "Emailing%20operational%20area%20" + mapCaption;
      string body = "Open%20the%20attachment%20to%20view%20the%20operational%20area%20" + mapCaption;
      string mailto = string.Format("&Subject={0}&Body={1}", subject, body);

      try
      {
        ProcessStartInfo info = new ProcessStartInfo();
        //Change the following line to use other mail clients if Outlook is not available 
        info.FileName = @"C:\Program Files (x86)\Microsoft Office\Office15\OUTLOOK.EXE";
        info.Arguments = string.Format(@"/a {0}  /c ipm.note /m {1}", filePath, mailto);
        System.Diagnostics.Process.Start(info);
      }
      catch
      {
        MessageBox.Show("Error sending map image");
      }
    }

    //Capture the map image then create a bitmap encorder for the image
    private BitmapEncoder CaptureMapImage()
    {
      //Get the width and height of the map and set it as the dimensions of the imaeg to create
      int imgWidth = Convert.ToInt16(Math.Round(_mapWidget.Map.ActualWidth));
      int imgHeight = Convert.ToInt16(Math.Round(_mapWidget.Map.ActualHeight));

      try
      {
        //Encode the captured image
        RenderTargetBitmap bmp = new RenderTargetBitmap(imgWidth, imgHeight, 96, 96, PixelFormats.Default);
        DrawingVisual dv = new DrawingVisual();
        using (DrawingContext dc = dv.RenderOpen())
        {
          VisualBrush vb = new VisualBrush(_mapWidget.Map);
          dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(imgWidth, imgHeight)));
        }
        bmp.Render(dv);
        BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
        bitmapEncoder.Frames.Add(BitmapFrame.Create(bmp));
        return bitmapEncoder;
      }
      catch
      {
        MessageBox.Show("Error cpturing map image");
        return null;
      }
    }

    //Capture the map image, then prompt user to save it as an image file
    private void PromptToSaveMapImage()
    {
      BitmapEncoder bitmapEncoder = CaptureMapImage();
      if (bitmapEncoder == null)
        return;

      SaveFileDialog saveDialog = new SaveFileDialog()
      {
        FileName = "",
        DefaultExt = ".jpg",
        Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png"
      };

      Nullable<bool> result = saveDialog.ShowDialog();
      if (result == true)
        SaveMapImage(bitmapEncoder, saveDialog.FileName);
    }

    private bool SaveMapImage(BitmapEncoder bitmapEncoder, string filename)
    {
      try
      {
        using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
          bitmapEncoder.Save(fs);
        };
      }
      catch
      {
        MessageBox.Show("Error saving map image");
        return false;
      }
      return true;
    }


    #endregion

    #region implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged<T>(Expression<Func<T>> expression)
    {
      if (expression == null) return;
      MemberExpression body = expression.Body as MemberExpression;
      if (body == null) return;
      OnPropertyChanged(body.Member.Name);
    }

    private void OnPropertyChanged(string propertyName)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, Expression<Func<T>> expression)
    {
      if (EqualityComparer<T>.Default.Equals(field, value)) return false;
      field = value;
      OnPropertyChanged(expression);
      return true;
    }
    #endregion
  }

  public enum LineWidth
  {
    Thin,
    Medium,
    Thick
  }
}
