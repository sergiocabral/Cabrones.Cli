// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Cli.General.Web
{
   /// <summary>
   /// Implementa um web server.
   /// </summary>
    public class WebServer
    {
        /// <summary>
        /// Indica se o servidor está lidado.
        /// </summary>
        public bool Started { get; private set; }
        
        /// <summary>
        /// Escuta de conexão HTTP.
        /// </summary>
        private readonly HttpListener _listener = new HttpListener();
        
        /// <summary>
        /// Função que responde a requisição HTTP.
        /// </summary>
        private readonly Func<HttpListenerRequest, string> _response;

        /// <summary>
        /// Codificação do texto recebido e enviado.
        /// </summary>
        public Encoding Encoding { get; } = Encoding.UTF8;

        /// <summary>
        /// Construtor.
        /// Configura o servidor como localhost na porta especificada.
        /// </summary>
        /// <param name="port">Porta do serviço HTTP.</param>
        /// <param name="response">Função que processa a resposta do servidor.</param>
        public WebServer(int port = 80, Func<HttpListenerRequest, string> response = null)
            :this(response, $"http://localhost:{port}/") { }

        /// <summary>
        /// Construtor.
        /// </summary>
        /// <param name="response">Função que processa a resposta do servidor.</param>
        /// <param name="prefixes">Prefixos da conexão, exemplo: http://localhost:8080/test/</param>
        public WebServer(Func<HttpListenerRequest, string> response, params string[] prefixes)
        {
            if (!HttpListener.IsSupported) throw new NotSupportedException("HttpListener not supported.");
            
            response = response ?? (request => request.Url.ToString());

            prefixes = prefixes != null && prefixes.Length > 0 ? prefixes : new[] { "http://localhost:80/" };

            foreach (var prefix in prefixes) _listener.Prefixes.Add(prefix);

            _response = response;
            
            _listener.Start();
        }
    
        /// <summary>
        /// Inicia o servidor.
        /// </summary>
        public WebServer Start()
        {
            ThreadPool.QueueUserWorkItem(state =>
            {
                try
                {
                    while (_listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(stateListenerContext =>
                        {
                            var httpListenerContext = stateListenerContext as HttpListenerContext;
                            try
                            {
                                if (httpListenerContext == null) return;

                                var html = _response(httpListenerContext.Request);
                                var htmlBytes = Encoding.GetBytes(html);
                                httpListenerContext.Response.ContentLength64 = htmlBytes.Length;
                                httpListenerContext.Response.OutputStream.Write(htmlBytes, 0, htmlBytes.Length);
                            }
                            catch
                            {
                                // Ignora em caso de erro.
                            }
                            finally
                            {
                                // Sempre fecha o stream
                                httpListenerContext?.Response.OutputStream.Close();
                            }
                        },_listener.GetContext());
                    }
                }
                catch
                {
                    // Ignora em caso de erro.
                }
            });
            
            Started = true;
            
            return this;
        }

        /// <summary>
        /// Interrompe o servidor.
        /// </summary>
        public WebServer Stop()
        {
            if (!Started) return this;
            
            _listener.Stop();
            _listener.Close();
            
            Started = false;

            return this;
        }
    }
}