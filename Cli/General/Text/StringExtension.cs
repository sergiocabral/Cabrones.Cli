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
    }
}