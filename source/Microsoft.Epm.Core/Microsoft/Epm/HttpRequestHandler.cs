namespace Microsoft.Epm
{
    using System;
    using System.Net;
    using System.Text;

    public class HttpRequestHandler
    {
        public HttpRequestHandler()
        {
        }

        public void Listen()
        {
            using (var listener = new HttpListener())
            {
                listener.Prefixes.Add("http://+:8080/");
                listener.Start();

                while (true)
                {
                    Console.WriteLine("Listening");
                    var context = listener.GetContext(); //// TODO async
                    Console.WriteLine($"Received: TODO");

                    var response = context.Response;
                    response.OutputStream.Write(Encoding.ASCII.GetBytes("ack"));
                    response.OutputStream.Dispose();
                }
            }
        }
    }
}