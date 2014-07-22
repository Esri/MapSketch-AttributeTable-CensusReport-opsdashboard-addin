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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace OperationsDashboardAddIns
{ 
  /// <summary>
  /// Token 
  /// </summary>
  [DataContract(Name = "Token")]
  public class Token
  {
    [DataMember(Name = "access_token")]
    public string AccessToken { get; set; }
  }

  public class TokenService
  {
    Token _token = null;

    internal async Task<Token> GenerateTokenAsync()
    {
      string tokenUrl = string.Format(@"https://www.arcgis.com/sharing/oauth2/token?client_id={0}&grant_type=client_credentials&client_secret={1}&f=pjson",
        "client_id",
        "client_secret");

      try
      {
        HttpWebRequest webRequest = (HttpWebRequest)System.Net.WebRequest.Create(tokenUrl);
        webRequest.Timeout = 0xea60;
        System.Net.WebResponse response = await webRequest.GetResponseAsync();
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Token));
        _token = (Token)serializer.ReadObject(response.GetResponseStream());
        return _token;
      }
      catch
      {
        return null;
      }
    }
  }
}
