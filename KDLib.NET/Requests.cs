using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Fluent;
using Formatting = Newtonsoft.Json.Formatting;

namespace KDLib
{
  public static class Requests
  {
    private static readonly HttpClient httpClient = new HttpClient();

    public class RequestOptions
    {
      public bool LoggingEnabled = true;
      public bool LogPayload = true;
      public bool AlwaysLogFailures = true;
    }

    public class Response
    {
      private HttpResponseMessage resp;
      private byte[] data;

      public Response(HttpResponseMessage resp, byte[] data)
      {
        this.resp = resp;
        this.data = data;
      }

      public string Text
      {
        get
        {
          var charset = resp.Content.Headers.ContentType.CharSet;
          return charset == null ? Encoding.ASCII.GetString(data) : Encoding.GetEncoding(charset).GetString(data);
        }
      }

      public JToken Json => Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(Text);

      public XmlDocument Xml
      {
        get
        {
          var d = new XmlDocument();
          d.LoadXml(Text);
          return d;
        }
      }

      public HttpStatusCode StatusCode => resp.StatusCode;
      public byte[] Data => data;
      public string ContentType => resp.Content?.Headers?.ContentType?.MediaType;

      public bool Success => ((int) StatusCode / 100).In(2, 3);

      public string FormatResponse()
      {
        switch (ContentType) {
          case "application/octet-stream":
            return $"<binary:{Data.Length}>";
          case "application/json":
            return Json.ToString(Formatting.Indented);
          case "text/plain":
          case "text/html":
            return Text;
          default:
            return $"<unknown:{ContentType}>";
        }
      }
    }

    public static Task<Response> Get(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        RequestOptions options = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequest(HttpMethod.Get, url, parameters, null, null, headers, authUser, authPass, authBearerToken, timeout, options: options, cancellationToken: cancellationToken);
    }

    public static Task<Response> Post(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        RequestOptions options = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequest(HttpMethod.Post, url, parameters, data, json, headers, authUser, authPass, authBearerToken, timeout, options: options, cancellationToken: cancellationToken);
    }

    public static Task<Response> Put(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        RequestOptions options = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequest(HttpMethod.Put, url, parameters, data, json, headers, authUser, authPass, authBearerToken, timeout, options: options, cancellationToken: cancellationToken);
    }

    private static async Task<Response> DoRequest(
        HttpMethod method,
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        RequestOptions options = null,
        CancellationToken cancellationToken = default)
    {
      if (options == null)
        options = new RequestOptions();


      url = UrlUtils.CreateUrl(url, parameters);

      var request = new HttpRequestMessage(method, url);

      if ((authUser != null && authPass == null) || (authUser == null && authPass != null))
        throw new Exception("Both authUser and authPass must be specified");

      if (authUser != null) {
        var auth = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{authUser}:{authPass}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", auth);
      }
      else if (authBearerToken != null) {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authBearerToken);
      }

      headers?.ToList().ForEach(x => request.Headers.Add(x.Key, x.Value));

      if (data != null) {
        request.Content = new FormUrlEncodedContent(data);
      }
      else if (json != null) {
        request.Content = new StringContent(json.ToString());
        request.Content.Headers.ContentType.MediaType = "application/json";
      }

      if (!timeout.HasValue)
        timeout = TimeSpan.FromSeconds(60);

      string requestId = StringUtils.GenerateRandomString(4);
      if (options.LoggingEnabled) {
        var b = LogManager.GetLogger("requests")
                          .Trace()
                          .Message($"Sending request ({requestId}) to {method} {url}")
                          .Property("request_id", requestId)
                          .Property("parameters", parameters == null ? null : JsonConvert.SerializeObject(parameters))
                          .Property("headers", headers == null ? null : JsonConvert.SerializeObject(headers));

        if (options.LogPayload) {
          b.Property("data", data == null ? null : JsonConvert.SerializeObject(data));
          b.Property("json", json);
        }

        b.Write();
      }

      var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      if (timeout != Timeout.InfiniteTimeSpan)
        cts.CancelAfter(timeout.Value);
      
      var fetchTask = Task.Run(async () =>
      {
        var response = await httpClient.SendAsync(request, cts.Token).ConfigureAwait(false);
        var respData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        return new Response(response, respData);
      }, cts.Token);

      var stopwatch = Stopwatch.StartNew();
      try {
        var resp = await AsyncUtils.FastCancellableTask(fetchTask, cts.Token);

        int codeGroup = (int) resp.StatusCode / 100;
        if (codeGroup.In(2, 3)) {
          if (options.LoggingEnabled) {
            var b = LogManager.GetLogger("requests")
                              .Trace()
                              .Message($"Received response ({requestId}) from {method} {url} ({stopwatch.ElapsedMilliseconds}ms)")
                              .Property("request_id", requestId);

            if (options.LogPayload) {
              b.Property("response", resp.FormatResponse());
            }

            b.Write();
          }
        }
        else {
          if (options.LoggingEnabled || options.AlwaysLogFailures) {
            var b = LogManager.GetLogger("requests")
                              .Warn()
                              .Message($"Request ({requestId}) to {method} {url} failed with code: {(int) resp.StatusCode} ({stopwatch.ElapsedMilliseconds}ms)")
                              .Property("response", resp.FormatResponse())
                              .Property("request_id", requestId);

            if (options.LogPayload) {
              b.Property("status_code", (int) resp.StatusCode);
            }

            b.Write();
          }
        }

        return resp;
      }
      catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
        if (options.LoggingEnabled || options.AlwaysLogFailures) {
          LogManager.GetLogger("requests")
                    .Warn()
                    .Message($"Request ({requestId}) to {method} {url} timed out: ({stopwatch.ElapsedMilliseconds}ms)")
                    .Property("request_id", requestId)
                    .Write();
        }

        throw new TimeoutException();
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
        throw new TaskCanceledException();
      }
      catch (Exception e) {
        if (options.LoggingEnabled || options.AlwaysLogFailures) {
          LogManager.GetLogger("requests")
                    .Warn()
                    .Message($"Request ({requestId}) to {method} {url} failed: {e.Message} ({stopwatch.ElapsedMilliseconds}ms)")
                    .Property("request_id", requestId)
                    .Exception(e)
                    .Write();
        }

        throw;
      }
    }
  }
}