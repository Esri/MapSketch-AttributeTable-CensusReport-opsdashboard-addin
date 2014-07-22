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

using ESRI.ArcGIS.OperationsDashboard;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Windows;
using System.Linq;
using client = ESRI.ArcGIS.Client;

namespace OperationsDashboardAddIns
{
  /// <summary>
  /// A FeatureAction is an extension to Operations Dashboard for ArcGIS which can be shown when a user right-clicks on
  /// a feature in a widget.
  /// 
  /// This feature action generate a census report based on a feature specified by the operation view user 
  /// and a study area radius configured by the author of the operation view
  /// 
  /// http://resources.arcgis.com/en/help/arcgis-rest-api/index.html#//02r30000022q000000
  /// </summary>
  [Export("ESRI.ArcGIS.OperationsDashboard.FeatureAction")]
  [ExportMetadata("DisplayName", "Generate Census Report")]
  [ExportMetadata("Description", "Generate a census report based on a given study area")]
  [ExportMetadata("ImagePath", "/OperationsDashboardAddIns;component/Images/ReportGeneration.png")]
  public class CensusReportFeatureAction : IFeatureAction
  {
    /// <summary>
    /// Serialize the radius of the study area so that it can be re-used when the view is reopened
    /// </summary>
    [DataMember]
    public int BufferRadius { get; set; }

    /// <summary>
    /// Serialize the id of the selected data source so that it can be re-used when the view is reopened
    /// </summary>
    [DataMember]
    public string DataSourceId { get; set; }

    public ESRI.ArcGIS.OperationsDashboard.DataSource SelectedDataSource { get; set; }

    public CensusReportFeatureAction()
    {
      if (OperationsDashboard.Instance == null)
        return;

      SelectedDataSource = OperationsDashboard.Instance.DataSources.FirstOrDefault(ds => ds.Id == DataSourceId);
    }

    #region IFeatureAction
    /// <summary>
    ///  Determines if a Configure button is shown for the feature action.
    /// </summary>
    public bool CanConfigure
    {
      // In this case user should be able to specify the radius of their study area
      //  Hence CanConfigure is true
      get { return true; }
    }

    /// <summary>
    ///  Provides functionality to the user to specify the radius of the study area.
    ///  Called when the user clicks the Configure button next to the feature action.
    /// </summary>
    public bool Configure(System.Windows.Window owner)
    {
      // Show the configuration dialog.
      CensusReportDialog dialog = new CensusReportDialog(SelectedDataSource, BufferRadius) { Owner = owner };
      if (dialog.ShowDialog() != true)
        return false;

      //Retrive the DataSource and radius of the study area from the dialog
      SelectedDataSource = dialog.SelectedDataSource;
      DataSourceId = SelectedDataSource.Id;
      BufferRadius = dialog.BufferRadius;
      return true;
    }

    /// <summary>
    /// Determines if the feature action can be executed based on: 
    /// whether the data source is the one specified in the config window. If not, check if the data source is the selection data source of the one specified;
    /// whether the data source is broken;
    /// whether the feature's geometry is not null; 
    /// whether the feature is a point feature
    /// </summary>
    public bool CanExecute(ESRI.ArcGIS.OperationsDashboard.DataSource dataSource, client.Graphic feature)
    {
      #region If there's a previously set DataSourceId, we try to use it
      if (!string.IsNullOrEmpty(DataSourceId))
        SelectedDataSource = OperationsDashboard.Instance.DataSources.FirstOrDefault(ds => ds.Id == DataSourceId);
      if (SelectedDataSource == null)
        return false;
      #endregion

      //Check if the data source picked by user is the one specified in the config window
      if (dataSource.Id != SelectedDataSource.Id)
      {
        //If not, check if the data source picked by user is the selection data source of the one specified in the config window
        if (SelectedDataSource.Name == string.Format("{0} Selection", dataSource.Name) && SelectedDataSource.IsSelectable)
          return true;
        else
          return false;
      }
      //Check if the data source is broken
      if (dataSource.IsBroken)
        return false;
      //Check if the feature's geometry is null
      if (feature == null || feature.Geometry == null || string.IsNullOrEmpty(feature.Geometry.ToJson()))
        return false;
      //Check if the feature is a point feature
      if (feature.Geometry.GetType() != typeof(client.Geometry.MapPoint))
        return false;

      return true;
    }

    /// <summary>
    /// Execute is called when the user chooses the feature action from the feature actions context menu. Only called if
    /// CanExecute returned true.
    /// </summary>
    public void Execute(ESRI.ArcGIS.OperationsDashboard.DataSource dataSource, client.Graphic feature)
    {
      int wkid = MapWidget.FindMapWidget(dataSource).Map.SpatialReference.WKID;
      GenerateReportAsync(feature, wkid);
    }
    #endregion

    /// <summary>
    /// Helper function which does the actual report generation work
    /// </summary>
    private async void GenerateReportAsync(client.Graphic feature, int wkid)
    {
      ReportGenerationService service = new ReportGenerationService();

      //Generate a census report and retrive the location of the report
      string reportLocation = await service.GetReportGenerationAsync(feature, BufferRadius, wkid);

      //Try to open the report in a pdf viewer
      if (string.IsNullOrEmpty(reportLocation))
        MessageBox.Show("Error generating report", "Report Generation", MessageBoxButton.OK, MessageBoxImage.None);
      else
        Process.Start(new ProcessStartInfo(reportLocation));
    }

  }
}
