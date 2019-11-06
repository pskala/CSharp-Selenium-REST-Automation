using Microsoft.VisualStudio.TestTools.UnitTesting;
using mySpaceName.Helpers.WebDriver;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp.Extensions;
using System;

namespace mySpaceName.Helpers
{
    public class WebApi
    {
        public static string URLtoken = "/connect/token";

        public SeleniumSettings settings;
        public string AuthToken;
        public string RefreshToken;
        public string TokenType;

        List<string> performanceOutputList;

        public WebApi(SeleniumSettings Settings, List<string> PerformanceOutputList)
        {
            settings = Settings;
            performanceOutputList = PerformanceOutputList;
            //SetTokens(settings.UserName, settings.Password, out AuthToken, out RefreshToken, out TokenType);
        }
        /**
         * OAuth example - transformed user and password to token what used all request under login
         **/
        public void SetTokens(string username, string password, out string access_token, out string refresh_token, out string token_type)
        {
            try
            {
                var request = new RestRequest(Method.POST);

                request.AddHeader("cache-control", "no-cache");
                request.AddParameter("grant_type", "password");
                request.AddParameter("username", username);
                request.AddParameter("password", password);
                request.AddParameter("client_id", "yourClientID");
                request.AddParameter("client_secret", "secret");
                request.AddParameter("scope", "openid profile offline_access");

                var response = Execute(request, URLtoken);

                var responseJSON = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);

                if (!responseJSON.TryGetValue("access_token", out access_token))
                {
                    throw new System.ArgumentException("access_token cannot be empty for testcases for username " + username, "access_token");
                }
                if (!responseJSON.TryGetValue("refresh_token", out refresh_token))
                {
                    throw new System.ArgumentException("refresh_token cannot be empty for testcases for username " + username, "refresh_token");
                }
                if (!responseJSON.TryGetValue("token_type", out token_type))
                {
                    throw new System.ArgumentException("token_type cannot be empty for testcases for username " + username, "token_type");
                }
            } catch(Exception e)
            {
                Assert.Fail("Cannot possible login for username='" + username + "' Exception: " + e.Message);
                access_token = "";
                refresh_token = "";
                token_type = "";
            }
        }
        /**
         * send file to server as multipart/form-data
         **/
        public dynamic SendFileRequest(string urlPath, string pathFile, HttpStatusCode testOnStatusCode = HttpStatusCode.Accepted, string authToken = null)
        {
            var URL = settings.AppURL + urlPath;
            var authorizationToken = authToken == null ? AuthToken : authToken;
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", TokenType + " " + authorizationToken);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddFile("content", pathFile);
            var response = Execute(request, urlPath);
            Assert.AreEqual(testOnStatusCode, response.StatusCode, "Send file request with " + urlPath + " failed! Return status code is " + response.StatusCode + " but expected " + testOnStatusCode + ". Response is " + response.Content);
            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
        /**
         * download file from server 
         **/
        public void DownloadFileRequest(string urlPath, string filename, string authToken = null)
        {
            var URL = settings.AppURL + urlPath;
            var authorizationToken = authToken == null ? AuthToken : authToken;
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("authorization", TokenType + " " + authorizationToken);

            Execute(request, urlPath, filename);
        }
        /**
         * send GET request and return string text (json, error message, ..)
         **/
        public string SendGETRequestText(string urlPath, HttpStatusCode testOnStatusCode = HttpStatusCode.OK, string authToken = null)
        {
            IRestResponse response = GetResponse(urlPath, Method.GET, null, testOnStatusCode, authToken);

            Assert.AreEqual(testOnStatusCode, response.StatusCode, "Get request with " + urlPath + " failed! Return status code is " + response.StatusCode + " but expected " + testOnStatusCode + ". Response is " + response.Content);

            return response.Content;
        }
        /**
         * send GET request and return JSON object
         **/
        public dynamic SendGETRequest(string urlPath, HttpStatusCode testOnStatusCode = HttpStatusCode.OK, string authToken = null)
        {
            var content = SendGETRequestText(urlPath, testOnStatusCode, authToken);
            return JsonConvert.DeserializeObject<dynamic>(content);
        }
        /**
         * send POST request and return string text (json, error message, ..)
         **/
        public string SendPOSTRequestText(string urlPath, dynamic RequestBody = null, HttpStatusCode testOnStatusCode = HttpStatusCode.Created, string authToken = null)
        {
            IRestResponse response = GetResponse(urlPath, Method.POST, RequestBody, testOnStatusCode, authToken);

            Assert.AreEqual(testOnStatusCode, response.StatusCode, "Post request with " + urlPath + " and request body " + RequestBody + " failed! Return status code is " + response.StatusCode + " but expected " + testOnStatusCode + ". Response is " + response.Content);

            return response.Content;
        }
        /**
         * send POST request and return JSON object
         **/
        public dynamic SendPOSTRequest(string urlPath, dynamic RequestBody = null, HttpStatusCode testOnStatusCode = HttpStatusCode.Created, string authToken = null)
        {
            return JsonConvert.DeserializeObject<dynamic>(SendPOSTRequestText(urlPath, RequestBody, testOnStatusCode, authToken));
        }
        /**
         * send POST request and return JSON object
         **/
        public dynamic SendPUTRequest(string urlPath, dynamic RequestBody = null, HttpStatusCode testOnStatusCode = HttpStatusCode.NoContent, string authToken = null)
        {
            IRestResponse response = GetResponse(urlPath, Method.PUT, RequestBody, testOnStatusCode, authToken);

            Assert.AreEqual(testOnStatusCode, response.StatusCode, "Put request with " + urlPath + " and request body " + RequestBody + " failed! Return status code is " + response.StatusCode + " but expected " + testOnStatusCode + ". Response is " + response.Content);

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }
        /**
         * send DELETE request and return JSON object
         **/
        public dynamic SendDELETERequest(string urlPath, HttpStatusCode testOnStatusCode = HttpStatusCode.NoContent, string authToken = null)
        {
            IRestResponse response = GetResponse(urlPath, Method.DELETE, null, testOnStatusCode, authToken);

            Assert.AreEqual(testOnStatusCode, response.StatusCode, "Delete request with " + urlPath + " failed! Return status code is " + response.StatusCode + " but expected " + testOnStatusCode + ". Response is " + response.Content);

            return JsonConvert.DeserializeObject<dynamic>(response.Content);
        }

        /**
         * private procedures section
         **/
        private IRestResponse GetResponse(string urlPath, RestSharp.Method method, dynamic RequestBody = null, HttpStatusCode testOnStatusCode = HttpStatusCode.OK, string authToken = null)
        {
            var authorizationToken = authToken == null ? AuthToken : authToken;

            var request = new RestRequest(method);
            request.AddHeader("cache-control", "no-cache");
            if ((method != Method.GET) || (method != Method.DELETE))
            {
                var requestBody = RequestBody == null ? "{}" : RequestBody.ToString();
                request.AddHeader("content-type", "application/json");
                request.AddParameter("application/json", requestBody, ParameterType.RequestBody);
            }
            request.AddHeader("authorization", TokenType + " " + authorizationToken);

            return Execute(request, urlPath);
        }
        private IRestResponse Execute(RestRequest request, string urlPath, string filename = null)
        {
            var client = new RestClient(settings.AppURL + urlPath);
            DateTime stopPerformance;
            IRestResponse response;
            var startPerformance = DateTime.Now;
            if (filename != null)
            {
                client.DownloadData(request).SaveAs(settings.DownloadPathDir + filename);
                stopPerformance = DateTime.Now;
                WritePerformance(startPerformance, stopPerformance, urlPath, request.Method, "DownloadFile");
                return null;
            }
            else
            {
                response = client.Execute(request);
                stopPerformance = DateTime.Now;
                WritePerformance(startPerformance, stopPerformance, urlPath, request.Method, response.StatusCode.ToString());
                return response;
            }
        }
        private void WritePerformance(DateTime start, DateTime stop, string url, RestSharp.Method method, string statusCode)
        {
            var SEPARATOR = ";";
            var line = settings.TestContext.TestName + SEPARATOR + method.ToString() + SEPARATOR + url + SEPARATOR + statusCode + SEPARATOR;
            line = line + start.ToString() + SEPARATOR + stop.ToString() + SEPARATOR + (stop - start) + SEPARATOR;
            performanceOutputList.Add(line);
        }
    }
}
