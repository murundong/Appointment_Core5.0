using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace CoreBaseClass
{
    public class HttpHelper
    {
        private const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.109 Safari/537.36";
        private static readonly Encoding DefaultRequestEncoding = Encoding.GetEncoding("utf-8");


        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="timeOut">超时时间</param>
        /// <param name="encoding">编码格式默认为Encoding.UTF8</param>
        /// <returns></returns>
        public static string Get(string url, Encoding encoding = null, long buffSize = 256000)
        {
            try
            {
                encoding = encoding ?? Encoding.UTF8;

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("user-agent", DefaultUserAgent);
                    client.MaxResponseContentBufferSize = buffSize;
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    if (!response.IsSuccessStatusCode)
                    {
                        //Log.ErrorFormat("HttpGet URL:{0},StatusCode", url, response.StatusCode.ToString());
                        try
                        {
                            if (response.Content != null)
                            {
                                return response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    if (response.Content.Headers.ContentType != null)
                    {
                        response.Content.Headers.ContentType.CharSet = encoding.BodyName;
                    }
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw ;
                ////Log.ErrorFormat("HttpGet service error", e);
                //return string.Empty;
            }
        }


        public static string Post(string url, Dictionary<string, string> data, Encoding encoding = null)
        {
            try
            {
                encoding = encoding ?? Encoding.UTF8;

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                    client.MaxResponseContentBufferSize = 256000;

                    var content = new FormUrlEncodedContent(data);

                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        return string.Empty;
                    }
                    response.Content.Headers.ContentType.CharSet = encoding.BodyName;
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw;
                //Log.ErrorFormat("HttpGet service error URL:{0},ex{1}", url, e);
                //return string.Empty;
            }
        }

        public static string PostJson(string url, string json, Encoding encoding = null)
        {
            try
            {
                encoding = encoding ?? Encoding.UTF8;

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                    client.MaxResponseContentBufferSize = 256000;

                    HttpContent content = new StringContent(json, encoding, "application/json");

                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            if (response.Content != null)
                            {
                                return response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    if (response.Content.Headers.ContentType != null)
                    {
                        response.Content.Headers.ContentType.CharSet = encoding.BodyName;
                    }
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw;
                //Log.ErrorFormat("HttpPost service error URL:{0},data:{1},ex{2}", url, json, e);
                //return string.Empty;
            }
        }

        public static string PostJsonGZip(string url, string json, Encoding encoding = null)
        {
            try
            {
                encoding = encoding ?? Encoding.UTF8;

                var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                    client.DefaultRequestHeaders.TransferEncodingChunked = true;
                    client.MaxResponseContentBufferSize = 256000;

                    byte[] array = Encoding.UTF8.GetBytes(json);
                    var g = GZipHelper.Compress(array);

                    HttpContent content = new ByteArrayContent(g);
                    content.Headers.Add("Content-Encoding", "gzip");
                    content.Headers.Add("Content-Type", "application/json");
                    HttpResponseMessage response = client.PostAsync(url, content).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        try
                        {
                            if (response.Content != null)
                            {
                                return response.Content.ReadAsStringAsync().Result;
                            }
                            else
                            {
                                return string.Empty;
                            }
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
                    if (response.Content.Headers.ContentType != null)
                    {
                        response.Content.Headers.ContentType.CharSet = encoding.BodyName;
                    }
                    return response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (Exception e)
            {
                throw;
                //Log.ErrorFormat("HttpPost service error URL:{0},data:{1},ex{2}", url, json, e);
                //return string.Empty;
            }
        }


        public static string PostFile(string url, byte[] bytes, Dictionary<string, string> dicParam = null)
        {
            try
            {
                var boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
                var boundarybytes = Encoding.ASCII.GetBytes(string.Format("\r\n--{0}\r\n", boundary));
                var req = WebRequest.Create(url);
                req.Method = "POST";
                req.Credentials = CredentialCache.DefaultCredentials;
                req.ContentType = string.Format("multipart/form-data; boundary={0}", boundary);


                var dataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

                var strHeader = "Content-Disposition:form-data; name=\"{0}\";filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";

                strHeader = string.Format(strHeader, "filedata", string.Empty, string.Empty);

                byte[] byteHeader = System.Text.ASCIIEncoding.ASCII.GetBytes(strHeader);




                using (var stream = req.GetRequestStream())
                {
                    if (dicParam != null)
                    {
                        foreach (var kv in dicParam)
                        {
                            stream.Write(boundarybytes, 0, boundarybytes.Length);

                            var formBytes = Encoding.UTF8.GetBytes(string.Format(dataTemplate, kv.Key, kv.Value));
                            stream.Write(formBytes, 0, formBytes.Length);
                        }
                    }
                    stream.Write(boundarybytes, 0, boundarybytes.Length);

                    stream.Write(byteHeader, 0, byteHeader.Length);
                    stream.Write(bytes, 0, bytes.Length);
                    byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                    stream.Write(trailer, 0, trailer.Length);
                    stream.Close();
                }
                var result = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        result = reader.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                return e.ToString();
            }
        }

        public static string UploadFile(string upload_url, string filePath, Dictionary<string, string> param = null)
        {
            var url = upload_url;

            var result = string.Empty;
            byte[] bytes = null;
            try
            {
                using (var fs = new FileStream(filePath, FileMode.Open))
                {
                    bytes = new byte[fs.Length];
                    fs.Read(bytes, 0, bytes.Length);
                }
                result = PostFile(url, bytes, param);
            }
            catch (Exception e)
            {
                result = string.Empty;
            }
            return result;
        }


    }
}
