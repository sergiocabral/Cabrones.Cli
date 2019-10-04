// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Cli.General.Text.Formatter;

namespace Cli.General.Text
{
    /// <summary>
    ///     Extensões para string.
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        ///     Traduz um texto.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="language">Opcional. Especificar idioma.</param>
        /// <returns>Texto traduzido.</returns>
        public static string Translate(this string text, string language = null)
        {
            return Text.Translate.GetText(text, language ?? Text.Translate.Default.Language);
        }

        /// <summary>
        ///     Converte para formato slug
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Texto como slug.</returns>
        public static string Slug(this string text)
        {
            return string.Format(new FormatRemoveAccentuation(), "{0}", text).ToLower().Replace(" ", "-");
        }

        /// <summary>
        ///     Determina se é um endereço de e-mail válido.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns></returns>
        public static bool IsEmail(this string text)
        {
            return Regex.IsMatch(text,
                @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*@((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z");
        }

        /// <summary>
        ///     Determina se é um inteiro.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="min">Valor mínimo.</param>
        /// <param name="max">Valor máximo.</param>
        /// <returns></returns>
        public static bool IsIntegerPositive(this string text, int min = int.MinValue, int max = int.MaxValue)
        {
            try
            {
                var value = int.Parse(text);
                return value >= min && value <= max;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Converte um conjunto de valores em par para querystring.
        /// </summary>
        /// <param name="values">Valores</param>
        /// <returns>Querystring</returns>
        public static string ToQuerystring(this NameValueCollection values)
        {
            var result = new StringBuilder();
            foreach (string value in values)
            {
                result.Append($"{Uri.EscapeDataString(value)}={Uri.EscapeDataString(values[value])}&");
            }

            return result.Length == 0 ? string.Empty : result.ToString().Substring(0, result.Length - 1);
        }

        /// <summary>
        /// Converte uma querystring em conjunto de valores em par.
        /// </summary>
        /// <param name="querystring">Querystring</param>
        /// <returns>Valores</returns>
        public static NameValueCollection ToNameValueCollection(this string querystring)
        {
            var result = new NameValueCollection();
            querystring = querystring.Length > 0 && querystring[0] == '?' ? querystring.Substring(1) : querystring;
            var values = querystring.Split('&', StringSplitOptions.RemoveEmptyEntries);
            foreach (var value in values)
            {
                var keyValue = value.Split('=');
                result[Uri.UnescapeDataString(keyValue[0])] = Uri.UnescapeDataString(keyValue[1]);
            }

            return result;
        }
    }
}