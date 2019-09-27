using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

// ReSharper disable UnusedMember.Global

namespace Cli.General.Web
{
    /// <summary>
    ///     Classe com funcionalidade utilitárias para internet.
    /// </summary>
    public class WebClientWithCookie : WebClient
    {
        /// <summary>
        ///     Cookie para o WebRequest.
        /// </summary>
        private CookieContainer CookieContainer { get; set; } = new CookieContainer();

        /// <summary>
        ///     Retorna um WebRequest com cookies.
        /// </summary>
        /// <param name="url">Endereço.</param>
        /// <returns>WebRequest com cookies.</returns>
        protected override WebRequest GetWebRequest(Uri url)
        {
            var request = (HttpWebRequest) base.GetWebRequest(url) ?? throw new NullReferenceException();
            request.Timeout = 60 * 1000;
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/71.0.3578.98 Safari/537.36";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            request.KeepAlive = true;
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "pt-BR,pt;q=0.9,en;q=0.8,en-US;q=0.7,es;q=0.6");
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.CookieContainer = CookieContainer;
            request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            request.Referer = url.ToString();
            return request;
        }

        /// <summary>
        ///     Carrega um endereço.
        /// </summary>
        /// <param name="url">Endereço.</param>
        /// <param name="method">Opcional. Método "GET" ou "POST".</param>
        /// <param name="fields">Opcional. Campos submetidos.</param>
        /// <param name="encoding">Opcional. Encoding. Por padrão é: Encoding.Default</param>
        /// <returns>Resultado do carregamento.</returns>
        public WebClientWithCookieLoadResult Load(string url, string method = "GET", NameValueCollection fields = null,
            Encoding encoding = null)
        {
            if (encoding == null) encoding = Encoding.Default;
            var querystring = fields?.GetQuerystring();
            method = (method + "").ToUpper();
            if (method == "GET" && !string.IsNullOrWhiteSpace(querystring)) url += $"?{querystring}";

            var request = (HttpWebRequest) GetWebRequest(new Uri(url)) ?? throw new NullReferenceException();

            request.Method = method;

            if (request.Method == "POST")
            {
                request.ContentType = "application/x-www-form-urlencoded";
                if (!string.IsNullOrWhiteSpace(querystring))
                {
                    var querystringBytes = encoding.GetBytes(querystring);
                    request.ContentLength = querystringBytes.Length;
                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(querystringBytes, 0, querystringBytes.Length);
                    }
                }
            }

            try
            {
                using (var response = (HttpWebResponse) request.GetResponse())
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream ?? throw new NullReferenceException(),
                    Encoding.Default))
                {
                    return new WebClientWithCookieLoadResult(
                        url,
                        true,
                        reader.ReadToEnd(),
                        CookieContainer.GetCookieHeader(new Uri(url)));
                }
            }
            catch (Exception ex)
            {
                return new WebClientWithCookieLoadResult(url, ex);
            }
        }

        /// <summary>
        ///     Carregar (ou consulta) cookies previamente guardados.
        /// </summary>
        /// <param name="cookies">Conteúdo.</param>
        public string Cookies(string cookies = null)
        {
            var formatter = new BinaryFormatter();
            byte[] bytes;
            if (!string.IsNullOrWhiteSpace(cookies))
                try
                {
                    bytes = Convert.FromBase64String(cookies);
                    using (var stream = new MemoryStream(bytes))
                    {
                        CookieContainer = (CookieContainer) formatter.Deserialize(stream);
                        return cookies;
                    }
                }
                catch
                {
                    // Ignora se não for possível ler os cookies.
                }

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, CookieContainer);
                stream.Position = 0;
                bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                return Convert.ToBase64String(bytes);
            }
        }
    }
}