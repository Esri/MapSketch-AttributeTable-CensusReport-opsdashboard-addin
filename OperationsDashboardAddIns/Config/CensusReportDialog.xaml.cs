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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ESRI.ArcGIS.OperationsDashboard;

namespace OperationsDashboardAddIns
{
  /// <summary>
  /// A dialog for operation view author to specify a radius of the study area
  /// </summary>
  public partial class CensusReportDialog : Window, INotifyPropertyChanged
  {
    private int bufferRadius;
    public int BufferRadius
    {
      get { return bufferRadius; }
      set
      {
        if (bufferRadius != value)
        {
          bufferRadius = value;
          OnPropertyChanged("BufferRadius");
        }
      }
    }

    //Determine if Ok button can be enabled
    private bool canOk;
    public bool CanOk
    {
      get { return canOk; }
      private set
      {
        if (canOk != value)
        {
          canOk = value;
          OnPropertyChanged("CanOk");
        }
      }
    }

    public DataSource SelectedDataSource { get; set; }

    public CensusReportDialog(DataSource InitialDataSource, int InitialRadius)
    {
      InitializeComponent();

      if (InitialDataSource != null) SelectedDataSource = InitialDataSource;
      DataSourceSelector.SelectedDataSource = SelectedDataSource;
      BufferRadius = InitialRadius <= 0 ? 3 : InitialRadius;

      DataContext = this;
    }

    private void RadiusChanged(object sender, TextChangedEventArgs e)
    {
      ValidateInput(sender, null);
    }

    //Validate user input whenever the text in the radius textbox is changed
    private void ValidateInput(object sender, TextChangedEventArgs e)
    {
      //The radius textbox only allows positive integer values
      string text = (sender as TextBox).Text;

      if (string.IsNullOrEmpty(text))
      {
        CanOk = false;
        return;
      }

      int bufferRadius;
      if (Int32.TryParse(text, out bufferRadius) == false || bufferRadius <= 0)
      {
        CanOk = false;
        return;
      }

      BufferRadius = bufferRadius;
      CanOk = true;
    }

    private void OKButton_Click(object sender, RoutedEventArgs e)
    {
      SelectedDataSource = DataSourceSelector.SelectedDataSource;

      //In order to generate a census report, we require that the data source must be a map-based data source, 
      //because the report generate services needs an input geometry, but for non map-based data sources (aka. external data source) 
      //they might not have a geometry
      MapWidget mw = OperationsDashboard.Instance.FindWidget(SelectedDataSource) as MapWidget;
      if (mw == null)
      {
        MessageBox.Show("External data source is not allowed", "Invalid data source", MessageBoxButton.OK, MessageBoxImage.None);
        return;
      }
      else
        DialogResult = true;
    }

    #region INotifyPropertyChanged
    void OnPropertyChanged(string prop)
    {
      if (PropertyChanged != null) { PropertyChanged(this, new PropertyChangedEventArgs(prop)); }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    #endregion
  }
}
