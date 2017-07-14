using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Web;
using System.Drawing;

namespace TwitchAccountCreator
{
    static class HttpRequests
    {
        public static bool HttpPostRequest(string URL, Dictionary<string, string> postParameters, ref string response)
        {
            string postData = "";

            foreach (string key in postParameters.Keys)
            {
                postData += HttpUtility.UrlEncode(key) + "="
                      + HttpUtility.UrlEncode(postParameters[key]) + "&";
            }

            HttpWebRequest myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            myHttpWebRequest.CookieContainer = new CookieContainer();

            myHttpWebRequest.Method = "POST";

            byte[] data = Encoding.ASCII.GetBytes(postData);

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:24.0) Gecko/20100101 Firefox/24.0";
            myHttpWebRequest.Timeout = 5000;
            myHttpWebRequest.ContentLength = data.Length;
            myHttpWebRequest.Referer = "http://www.twitch.tv";

            Stream requestStream = myHttpWebRequest.GetRequestStream();
            requestStream.Write(data, 0, data.Length);
            requestStream.Close();

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                response = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();
                myHttpWebResponse.Close();

                
                foreach (Cookie c in myHttpWebResponse.Cookies)
                {
                    if (c.Name.Equals("login"))
                        return true;
                }
                

                return false;
            }
            else
            {
                return false;
            }
        }

        public static bool HttpGetPictureRequest(string URL, Dictionary<string, string> getParameters, ref Bitmap picture)
        {
            string getParams = "?";
            HttpWebRequest myHttpWebRequest;

            if (getParameters != null)
            {
                foreach (string key in getParameters.Keys)
                {
                    getParams += HttpUtility.UrlEncode(key) + "="
                          + HttpUtility.UrlEncode(getParameters[key]) + "&";
                }

                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL + getParams);
            }
            else
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            }

            myHttpWebRequest.Method = "GET";

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:24.0) Gecko/20100101 Firefox/24.0";
            myHttpWebRequest.Timeout = 5000;

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                picture = new Bitmap(responseStream);

                responseStream.Close();
                myHttpWebResponse.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool HttpGetRequest(string URL, Dictionary<string, string> getParameters, ref string response)
        {
            string getParams = "?";
            HttpWebRequest myHttpWebRequest;

            if (getParameters != null)
            {
                foreach (string key in getParameters.Keys)
                {
                    getParams += HttpUtility.UrlEncode(key) + "="
                          + HttpUtility.UrlEncode(getParameters[key]) + "&";
                }

                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL + getParams);
            }
            else
            {
                myHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create(URL);
            }

            myHttpWebRequest.Method = "GET";

            myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
            myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:24.0) Gecko/20100101 Firefox/24.0";
            myHttpWebRequest.Timeout = 5000;

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();

            if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
            {
                Stream responseStream = myHttpWebResponse.GetResponseStream();
                StreamReader myStreamReader = new StreamReader(responseStream, Encoding.Default);

                response = myStreamReader.ReadToEnd();

                myStreamReader.Close();
                responseStream.Close();
                myHttpWebResponse.Close();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
