MapSketch-CensusReport-AttributeTable-opsdashboard-addin
=========================================================

An Operations Dashboard addin with a sketch map tool, a census report feature action, and an attribute table widget

![Using the Addin in Operations Dashboard](https://github.com/esri/MapSketch-AttributeTable-CensusReport-opsdashboard-addin/blob/master/ScreenShot.png)

## Features
 
* Sketch map tool
* Census report feature action
* Attribute table widget 

## Instructions

### Running this add-in
1. Ensure all requirements below are met.
2. Open and compile the addin project in Visual Studio 2012 or Visual Studio 2013.
3. Open the Token.cs file from the project. Replace the client_id and client_secret texts with yours.
4. Update Project Debug properties to correctly locate the add-in build path in /addinpath command line argument.
5. Run the project in Debug mode to start the Operations Dashboard application.
6. Open an operation view with at least one data source.
7. Add the sketch map tool. Pick a color and line width for the sketch line, then start drawing sketching on the map (touch device is not supported). When finish, save or e-mail the map image with the sketch, or discard the sketch.
8. To use the generate census report feature action, open the configure window of a widget (e.g. the map widget), locate the feature action and configure it.
9 . Pick a feature from the data source specified in the feature action's settings. Right click on the feature to execute the feature action. A census report will be generated based on the feature's location and the radius specified in the Configure window.
10. To use the attribute table widget, choose it from the Add Widget dialog box. Follow the on-screen instructions to configure the widget.
11. When the widget is added to the view, you can drag and drop the columns to reorder them. You can also double-click on a column header to order the records by that column, or you can right-click on a record to see the list of available feature actions.
12. To run the addin in Release environment, build the project and upload the .opdashboardaddin file from the output directory to ArcGIS Online or your Portal for ArcGIS, then add the addin to an operation view through the Manage Add-ins dialog box. Follow steps above starting from step 6 to step 10 to configure and use the addin.

### General Help
[New to Github? Get started here.](http://htmlpreview.github.com/?https://github.com/Esri/esri.github.com/blob/master/help/esri-getting-to-know-github.html)

## Requirements
* An ArcGIS organizational account
* An installation of the Esri ArcGIS Runtime SDK for WPF version 10.2.3 [Esri website](http://resources.arcgis.com/en/communities/runtime-wpf/)
* An installation of Microsoft Visual Studio 2012/2013 Pro or above


## Resources

Learn more about Esri's [Operations Dashboard for ArcGIS](http://www.esri.com/software/arcgis/arcgisonline/features/operations-dashboard).

## Issues

Find a bug or want to request a new feature?  Please let us know by submitting an issue.

## Contributing

Esri welcomes contributions from anyone and everyone. Please see our [guidelines for contributing](https://github.com/esri/contributing).

## Licensing

Copyright 2014 Esri

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0
         
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
                                 
A copy of the license is available in the repository's
[license.txt](https://github.com/esri/MapSketch-AttributeTable-CensusReport-opsdashboard-addin/blob/master/license.txt) file.
                                                                  
[](Esri Tags: User Conference)
[](Esri Tags: 2014)
[](Esri Language: C-Sharp)
                                                                                                               
                                                                                                                                                            
                                                                                                                                                            

