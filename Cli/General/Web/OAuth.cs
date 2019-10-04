using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using Cli.General.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace Cli.General.Web
{
    /// <summary>
    ///     Agrupa funcionalidades para realizar o processo de autenticação via OAuth
    /// </summary>
    public static class OAuth
    {
        /// <summary>
        /// Porta do servidor Web quando iniciado.
        /// </summary>
        public static int ServerPort { get; }= 2109; 
        
        /// <summary>
        /// Tempo (em segundos) que o servidor de escuta para redirect_uri fica ligado.
        /// </summary>
        private const int ServerWaitSeconds = 300;

        /// <summary>
        /// Título na página HTML para processamento do OAuth.
        /// </summary>
        public static string HtmlTitle { get; set; } = "OAuth";

        /// <summary>
        /// Conteúdo na página HTML durante o processamento do OAuth.
        /// </summary>
        public static string HtmlContent { get; set; } = $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>{HtmlTitle}</title></head><body>{"OAuth has been processed. This window can now be closed.".Translate()}</body></html>";

        /// <summary>
        /// Servidor web.
        /// </summary>
        private static WebServer _webServer;

        /// <summary>
        /// Método chamado quando o token for respondido.
        /// </summary>
        private static Action<string, string> _response;
            
        /// <summary>
        /// Retorna a url que deverá ser usada no campo redirect_uri da url de autorização OAuth. 
        /// </summary>
        /// <param name="name">Um nome identificador qualquer</param>
        /// <returns>Url</returns>
        public static string FactoryRedirectUrl(string name)
        {
            return $"http://localhost:{ServerPort}/{name.Slug()}/";
        }

        /// <summary>
        /// Inicia o processo de autorização OAuth.
        /// </summary>
        /// <param name="urlForAuthorization">Url de autorização OAuth.</param>
        /// <param name="response">Action quando houver a resposta com o token.</param>
        public static void StartAuthentication(string urlForAuthorization, Action<string, string> response)
        {
            _response = response;
            OpenUrl(urlForAuthorization);
            StartServer();
        }

        /// <summary>
        /// Abre uma url no navegador do computador.
        /// </summary>
        /// <param name="url">Url</param>
        private static void OpenUrl(string url)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new NotImplementedException("The OAuth authentication process only works on Windows.");
               
            var pathWindows = Environment.GetEnvironmentVariable("WINDIR");
            var pathCmd = $"{pathWindows}\\System32\\cmd.exe";
            Process.Start(pathCmd, $"/c start {url.Replace("&", "^&")}");
        }

        /// <summary>
        /// Inicia o servidor localhost para escutar o RedirectUrl.
        /// </summary>
        private static void StartServer()
        {
            StopServer();
            
            ThreadPool.QueueUserWorkItem(state =>
            {
                _webServer = new WebServer(ServerPort, Response).Start();
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                while (stopwatch.ElapsedMilliseconds / 1000 < ServerWaitSeconds && _webServer != null && _webServer.Started) { }

                StopServer();
            });
        }

        /// <summary>
        /// Para o servidor localhost para escutar o RedirectUrl.
        /// </summary>
        private static void StopServer()
        {
            if (_webServer == null) return;
            _webServer.Stop();
            _webServer = null;
        }

        /// <summary>
        /// Flag que indica que o OAuth ainda está sendo processado.
        /// </summary>
        private static bool _processing;

        /// <summary>
        /// Processa a responsa do servidor web.
        /// </summary>
        /// <param name="request">Requisição http.</param>
        /// <returns>Retorna HTML.</returns>
        private static string Response(HttpListenerRequest request)
        {
            if (_processing)
            {
                _processing = false;
                return HtmlContent;
            }

            var hasToken = request.Url.ToString().Contains("?");
            var html = hasToken ? 
                HtmlContent + @"<script>!location.search || (location.href = location.href.replace(location.search, ''))</script>" : 
                $"<!DOCTYPE html><html><head><meta charset=\"UTF-8\"><title>{HtmlTitle}</title></head><body><script>location.href = location.href.replace('#', '?');</script></body></html>";

            if (!hasToken) return html;

            _processing = true;

            _response(request.Url.Query, Regex.Replace(request.Url.AbsolutePath, @"(^/|/$)", string.Empty));
            ThreadPool.QueueUserWorkItem(state =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
                StopServer();
            }, null);
            
            return html;
        }
    }
}