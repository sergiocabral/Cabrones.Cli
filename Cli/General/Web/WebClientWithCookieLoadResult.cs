// ReSharper disable MemberCanBePrivate.Global

using System;

namespace Cli.General.Web
{
    /// <summary>
    ///     Conjunto de dados retornados após um carregamento de página.
    /// </summary>
    public struct WebClientWithCookieLoadResult
    {
        /// <summary>
        ///     Construtor de inicialização.
        /// </summary>
        /// <param name="url">Endereço.</param>
        /// <param name="success">Indica sucesso.</param>
        /// <param name="html">Código html.</param>
        /// <param name="cookies">Cookies.</param>
        public WebClientWithCookieLoadResult(string url, bool success, string html, string cookies)
        {
            Url = url;
            Exception = null;
            Success = success;
            Html = html;
            Cookies = cookies;
        }

        /// <summary>
        ///     Construtor para indicar falha.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="exception"></param>
        public WebClientWithCookieLoadResult(string url, Exception exception)
        {
            Url = url;
            Exception = exception;
            Success = false;
            Html = null;
            Cookies = null;
        }

        /// <summary>
        ///     Endereço.
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public object Url { get; }

        /// <summary>
        ///     Informações sobre o erro.
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public Exception Exception { get; }

        /// <summary>
        ///     Indica sucesso.
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public bool Success { get; }

        /// <summary>
        ///     Código html.
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string Html { get; }

        /// <summary>
        ///     Cookies.
        /// </summary>
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public string Cookies { get; }
    }
}