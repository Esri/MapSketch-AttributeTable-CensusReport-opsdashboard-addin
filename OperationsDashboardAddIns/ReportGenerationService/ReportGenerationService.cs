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

using ESRI.ArcGIS.Client.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OperationsDashboardAddIns
{
  public class ReportGenerationService
  {
    public Token Token { get; private set; }
    public Token TokenCtor{ get; set; }
    
    /// <summary>
    /// Create a .pdf document based on location and buffer radius
    /// </summary>
    public async Task<string> GetReportGenerationAsync(ESRI.ArcGIS.Client.Graphic feature, int bufferRadius, int wkid)
    {
      //Create a token so we can gain access to the geoenrich service
      TokenService tokenService = new TokenService();
      Token = await tokenService.GenerateTokenAsync();

      string pdfLocation = Assembly.GetExecutingAssembly().Location;
      pdfLocation = pdfLocation.Replace(".dll", ".pdf");
      if (File.Exists(pdfLocation))
        File.Delete(pdfLocation);

      if (bufferRadius <= 0) bufferRadius = 3;

      MapPoint currentLocation = feature.Geometry as MapPoint;

      string token = string.Format("&token={0}", Token.AccessToken);
      string studyAreas = "&studyareas=[{\"geometry\":{\"x\":" + currentLocation.X + ",\"y\":" + currentLocation.Y + "}}]";
      string format = "&format=pdf";
      string studyareasoptions = string.Format("&studyareasoptions=%7B%22areaType%22%3A%22RingBuffer%22%2C%22bufferUnits%22%3A%22esriMiles%22%2C%22bufferRadii%22%3A%5B{0}%5D%7D", bufferRadius);
      string insr = string.Format("&inSR={0}", wkid);
      string f = "&f=bin";
      string reportGenerationUrl =
        @"http://geoenrich.arcgis.com/arcgis/rest/services/World/MapServer/exts/BAServer/Geoenrichment/CreateReport?" + token + studyAreas + format + studyareasoptions + insr + f;
      
      #region Issue the request to generate a report
      WebResponse response;
      try
      {
        HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(reportGenerationUrl);
        webRequest.Timeout = 0xea60;
        response = await webRequest.GetResponseAsync();
        if (response == null || response.ContentLength == 0)
          return null;
      }
      catch
      {
        return null;
      } 
      #endregion

      #region Read the result into pdfLocation
      try
      {
        using (Stream responseStream = response.GetResponseStream())
        {
          using (FileStream fileStream = new FileStream(pdfLocation, FileMode.Create))
          {
            byte[] buffer = new byte[4096];
            int bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
            while (bytesRead > 0)
            {
              await fileStream.WriteAsync(buffer, 0, bytesRead);
              bytesRead = await responseStream.ReadAsync(buffer, 0, buffer.Length);
            }
          }
        }
      }
      catch
      {
        return null;
      } 
      #endregion

      return pdfLocation;
    }
  }
}

