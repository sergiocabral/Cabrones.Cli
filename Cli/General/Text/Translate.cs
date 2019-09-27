// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Cli.General.Text
{
    /// <summary>
    ///     Tradução do sistema.
    /// </summary>
    public class Translate
    {
        /// <summary>
        ///     Construtor.
        /// </summary>
        /// <param name="language"></param>
        /// <param name="setAsDefault">Define esta classe como padrão para traduções.</param>
        public Translate(string language, bool setAsDefault = false)
        {
            Language = language;
            if (setAsDefault) Default = this;
        }

        /// <summary>
        ///     Idioma da instância.
        /// </summary>
        public string Language { get; }

        /// <summary>
        ///     Dicionário idioma com suas traduções .
        /// </summary>
        private static IDictionary<string, IDictionary<string, string>> Translates { get; } =
            new Dictionary<string, IDictionary<string, string>>();

        /// <summary>
        ///     Instância padrão.
        /// </summary>
        public static Translate Default { get; private set; } = new Translate(string.Empty);

        /// <summary>
        ///     Cria uma instância e define como padrão para traduções
        /// </summary>
        /// <param name="language"></param>
        public static void CreateDefault(string language)
        {
            var unused = new Translate(language, true);
        }

        /// <summary>
        ///     Troca o formato da organização dos dados.
        ///     Topo como Idioma ou topo como Frase.
        /// </summary>
        /// <param name="source">Traduções.</param>
        /// <param name="destination">Destino das traduções.</param>
        /// <returns>Traduções.</returns>
        private static IDictionary<string, IDictionary<string, string>> SwapFormat(
            IDictionary<string, IDictionary<string, string>> source,
            IDictionary<string, IDictionary<string, string>> destination = null)
        {
            destination = destination ?? new Dictionary<string, IDictionary<string, string>>();
            foreach (var (phraseKey, phraseValue) in source)
            foreach (var (translateKey, translateValue) in phraseValue)
            {
                if (!destination.ContainsKey(translateKey))
                    destination[translateKey] = new Dictionary<string, string>();
                destination[translateKey][phraseKey] = translateValue;
            }

            return destination;
        }

        /// <summary>
        ///     Carrega um conjunto de traduções de vários idioma.
        /// </summary>
        /// <param name="json">Traduções.</param>
        public static string LoadAll(string json = null)
        {
            var serializer = new DataContractJsonSerializer(typeof(IDictionary<string, IDictionary<string, string>>));

            if (json != null)
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    var translates = (IDictionary<string, IDictionary<string, string>>) serializer.ReadObject(stream);
                    SwapFormat(translates, Translates);
                }
            else
                using (var stream = new MemoryStream())
                {
                    var translates = SwapFormat(Translates);
                    serializer.WriteObject(stream, translates);
                    json = Encoding.UTF8.GetString(stream.ToArray());
                }

            return json;
        }

        /// <summary>
        ///     Carrega um conjunto de traduções de um idioma.
        /// </summary>
        /// <param name="language">Nome do idioma.</param>
        /// <param name="json">Traduções.</param>
        public static string Load(string language, string json = null)
        {
            var serializer = new DataContractJsonSerializer(typeof(IDictionary<string, string>));

            if (json != null)
                using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
                {
                    Translates[language] = (IDictionary<string, string>) serializer.ReadObject(stream);
                }
            else
                using (var stream = new MemoryStream())
                {
                    serializer.WriteObject(stream, Translates[language]);
                    json = Encoding.UTF8.GetString(stream.ToArray());
                }

            return json;
        }

        /// <summary>
        ///     Retorna a tradução de um texto.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="language">Nome do idioma.</param>
        /// <returns>Texto traduzido.</returns>
        public static string GetText(string text, string language = null)
        {
            language = language ?? Default?.Language ?? string.Empty;
            return Translates.ContainsKey(language) && Translates[language].ContainsKey(text)
                ? Translates[language][text]
                : text;
        }

        /// <summary>
        ///     Retorna a tradução de um texto.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Texto traduzido.</returns>
        public string Text(string text)
        {
            return GetText(text, Language);
        }

        /// <summary>
        ///     Mesmo que string.Format(), mas realiza a tradução do texto.
        /// </summary>
        public static string GetFormat(IFormatProvider provider, string format, params object[] args)
        {
            return string.Format(provider, GetText(format), args);
        }

        /// <summary>
        ///     Mesmo que string.Format(), mas realiza a tradução do texto.
        /// </summary>
        public string Format(IFormatProvider provider, string format, params object[] args)
        {
            return GetFormat(provider, format, args);
        }

        /// <summary>
        ///     Mesmo que string.Format(), mas realiza a tradução do texto.
        /// </summary>
        public static string GetFormat(string format, params object[] args)
        {
            return string.Format(GetText(format), args);
        }

        /// <summary>
        ///     Mesmo que string.Format(), mas realiza a tradução do texto.
        /// </summary>
        public string Format(string format, params object[] args)
        {
            return GetFormat(GetText(format), args);
        }
    }
}