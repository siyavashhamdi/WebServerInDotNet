using System;
using System.Net;
using System.Text;
using System.Threading;

namespace WebServerTest
{
    public class LocalWebServer
    {
        private readonly HttpListener _listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> _responderCallbackMethod;

        public LocalWebServer(string prefix, Func<HttpListenerRequest, string> callbackMethod)
        {
            if (!HttpListener.IsSupported)
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");

            // URI prefixes are required, for example "http://localhost:8080/index/"
            var isPrefixValid = !string.IsNullOrWhiteSpace(prefix);
            if (!isPrefixValid)
                throw new ArgumentException("URI prefixes are required, for example");

            // A responder method is required
            if (callbackMethod == null)
                throw new ArgumentException("A call-back method is required");

            _listener.Prefixes.Add(prefix);

            _responderCallbackMethod = callbackMethod;
            _listener.Start();
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                //Console.WriteLine(@"Webserver running...");
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            var ctx = c as HttpListenerContext;

                            if (ctx == null)
                                throw new Exception("Context is null");

                            try
                            {
                                var rstr = _responderCallbackMethod(ctx.Request);
                                var buf = Encoding.UTF8.GetBytes(rstr);

                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch
                            {
                                // suppress any exceptions
                            }
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, _listener.GetContext());
                    }
                }
                catch
                {
                    // suppress any exceptions
                }
            });
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }
    }
}
