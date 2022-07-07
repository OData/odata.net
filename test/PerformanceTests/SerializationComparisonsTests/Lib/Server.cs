using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExperimentsLib
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;

    /// <summary>
    /// A basic HTTP server that serializes JSON
    /// responses based on the given data
    /// using the specified <see cref="IServerWriter{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Server<T> : IDisposable
    {
        IServerWriter<T> _writer;
        T _data;
        HttpListener _server;
        bool _running = false;
        Task _runningTask;
        string _contentType;
        public Server(IServerWriter<T> responseWriter, T data, string charset="utf-8")
        {
            _writer = responseWriter;
            _data = data;
            _contentType = $"application/json; charset={charset}";
        }

        public void Dispose()
        {
            Stop().Wait();
        }

        public void Start(string baseUrl)
        {
            if (_running)
            {
                return;
            }

            _server = new HttpListener();
            _server.Prefixes.Add(baseUrl.EndsWith("/") ? baseUrl : $"{baseUrl}/");
            _server.Start();
           
            _running = true;
            _runningTask = HandleRequests();
        }

        public void Start(int port)
        {
            try
            {
                Start($"http://*:{port}/");
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"Failed to start server on 'http://*:{port}': {ex.Message}");
                Console.WriteLine($"Falling back to 'http://localhost:{port}");
                Start($"http://localhost:{port}");
            }
        }

        public async Task Stop()
        {
            if (!_running)
            {
                return;
            }

            _running = false;
            _server.Stop();
            await _runningTask;
        }

        private async Task HandleRequests()
        {
            while (_running)
            {
                HttpListenerResponse resp = null;

                try
                {
                    HttpListenerContext ctx = await _server.GetContextAsync();
                    HttpListenerRequest req = ctx.Request;
                    resp = ctx.Response;
                    Console.WriteLine("Request received: {0}", req.Url.ToString());

                    T responseData = default(T);

                    resp.ContentType = _contentType;

                    // allow use to specify data size with ?count=<count> query param
                    //if (!string.IsNullOrEmpty(req.Url.Query))
                    //{
                    //    var query = HttpUtility.ParseQueryString(req.Url.Query);
                    //    string rawCount = query.Get("count");
                    //    if (!string.IsNullOrEmpty(rawCount))
                    //    {
                    //        int count = int.Parse(rawCount);
                    //        responseData = DataSet.GetCustomers(count);
                    //    }
                    //}

                    responseData ??= _data;
                    var sw = new Stopwatch();
                    sw.Start();
                    await _writer.WritePayload(responseData, resp.OutputStream);
                    resp.Close();
                    sw.Stop();
                    // disable console logging cause it seems to affect CPU time, likely blocks for I/O
                    //Console.WriteLine("Response time {0}ms, request {1}", sw.ElapsedMilliseconds, req.Url.ToString());
                }
                catch (Exception e)
                {
                    if (resp != null)
                    {
                        resp.StatusCode = 500;
                    }

                    using var errorWriter = new StreamWriter(resp.OutputStream);
                    errorWriter.Write($"{{\"error\": \"{e.Message}\"}}");
                    
                }
            }
        }
    }
}

