// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Cli.General.Cryptography;

namespace Cli.General.IO
{
    /// <summary>
    ///     Manipula um arquivo .ini
    /// </summary>
    public class IniFile
    {
        /// <summary>
        ///     Seção padrão.
        /// </summary>
        public const string SectionDefault = "configuration";

        /// <summary>
        ///     Construtor.
        /// </summary>
        /// <param name="filename">Arquivo .ini</param>
        /// <param name="defaultSection">Seção padrão.</param>
        public IniFile(string filename, string defaultSection = SectionDefault)
        {
            var directory = new FileInfo(filename).Directory;
            if (directory != null && !directory.Exists) directory.Create();

            Filename = filename;
            DefaultSection = defaultSection;
        }

        /// <summary>
        ///     Seção padrão.
        /// </summary>
        public string DefaultSection { get; set; }

        /// <summary>
        ///     Arquivo .ini
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        ///     Prefixo que garante a criptografia do dados.
        /// </summary>
        public static string PrefixToCryptography { get; set; } = "encrypt:";

        /// <summary>
        ///     Senha padrão usada na criptografia simétrica.
        /// </summary>
        private static string KeyDefault
        {
            get
            {
                var key = Assembly.GetEntryAssembly()?.GetName().Name.ToCharArray() ?? string.Empty.ToCharArray();
                Array.Reverse(key);
                return new string(key.Length.ToString()[0], 1000).Insert(500, new string(key));
            }
        }

        /// <summary>
        ///     Instância de criptografia.
        /// </summary>
        private CryptographySymmetric<Rijndael> Cryptography { get; set; } =
            new CryptographySymmetric<Rijndael>(KeyDefault, new byte[] {1, 0, 8, 9, 2, 7, 6, 5, 2, 3});

        /// <summary>
        ///     Converte um arquivo em dicionário de dados.
        /// </summary>
        /// <param name="filename">Arquivo. Opcional. Por padrão é o arquivo informado no construtor.</param>
        /// <returns>Dicionário.</returns>
        private IDictionary<string, IDictionary<string, string>> FileToDictionary(string filename = null)
        {
            filename = filename ?? Filename;

            var needCryptography = false;

            var data = new Dictionary<string, IDictionary<string, string>>();

            string[] content;
            try
            {
                content = File.Exists(filename) ? File.ReadAllLines(filename) : new string[] { };
            }
            catch
            {
                content = new string[] { };
            }

            string section = null;

            foreach (var line in content)
                if (Regex.IsMatch(line.Trim(), @"^\[.*\]$"))
                {
                    section = line.Trim();
                    section = section.Substring(1, section.Length - 2).Trim();

                    if (!data.ContainsKey(section)) data[section] = new Dictionary<string, string>();
                }
                else if (section != null && Regex.IsMatch(line.Trim(), @"^[^\=]+\=.*$"))
                {
                    var key = line.Trim();
                    var value = key.Substring(key.IndexOf('=') + 1).Trim();
                    key = key.Substring(0, key.IndexOf('=')).Trim();

                    if (key.IndexOf(PrefixToCryptography, StringComparison.Ordinal) == 0)
                        try
                        {
                            value = Cryptography.Apply(false, value);
                        }
                        catch
                        {
                            needCryptography = true;
                        }

                    data[section][key] = value;
                }

            if (needCryptography) DictionaryToFile(data, filename);
            return data;
        }

        /// <summary>
        ///     Converte um arquivo em dicionário de dados.
        /// </summary>
        /// <param name="data">Dicionário.</param>
        /// <param name="filename">Arquivo. Opcional. Por padrão é o arquivo informado no construtor.</param>
        private void DictionaryToFile(IDictionary<string, IDictionary<string, string>> data, string filename = null)
        {
            var content = new StringBuilder();
            if (data != null)
                foreach (var (key, dictionary) in data)
                {
                    content.AppendLine($"[{key}]").AppendLine();
                    if (dictionary == null) continue;

                    foreach (var (dataKey, dataValue) in dictionary)
                    {
                        var value = dataValue + string.Empty;
                        if (dataKey.IndexOf(PrefixToCryptography, StringComparison.Ordinal) == 0)
                            try
                            {
                                Cryptography.Apply(false, value);
                            }
                            catch
                            {
                                value = Cryptography.Apply(true, value);
                            }

                        content.AppendLine($"{dataKey}={value}").AppendLine();
                    }
                }

            File.WriteAllText(filename ?? Filename, content.ToString());
        }

        /// <summary>
        ///     Lê uma chave.
        /// </summary>
        /// <param name="key">Chave.</param>
        /// <param name="section">Seção.</param>
        /// <returns>Valor.</returns>
        public string Read(string key, string section = null)
        {
            section = section ?? DefaultSection;
            var data = FileToDictionary(Filename);
            return data.ContainsKey(section) && data[section].ContainsKey(key) ? data[section][key] : null;
        }

        /// <summary>
        ///     Escreve um valor numa chave.
        /// </summary>
        /// <param name="key">Chave.</param>
        /// <param name="value">Valor.</param>
        /// <param name="section">Seção.</param>
        public void Write(string key, string value, string section = null)
        {
            section = section ?? DefaultSection;
            var data = FileToDictionary(Filename);
            if (!data.ContainsKey(section)) data[section] = new Dictionary<string, string>();
            data[section][key] = value;
            DictionaryToFile(data);
        }

        /// <summary>
        ///     Apaga uma chave.
        /// </summary>
        /// <param name="key">Chave.</param>
        /// <param name="section">Seção.</param>
        public void DeleteKey(string key, string section = null)
        {
            section = section ?? DefaultSection;
            var data = FileToDictionary(Filename);

            if (!data.ContainsKey(section)) return;

            data[section].Remove(key);
            DictionaryToFile(data);
        }

        /// <summary>
        ///     Apaga uma seção inteira.
        /// </summary>
        /// <param name="section">Seção.</param>
        public void DeleteSection(string section = null)
        {
            section = section ?? DefaultSection;
            var data = FileToDictionary(Filename);
            data.Remove(section);
            DictionaryToFile(data);
        }

        /// <summary>
        ///     Verifica se uma chave existe.
        /// </summary>
        /// <param name="key">Chave.</param>
        /// <param name="section">Seção.</param>
        /// <returns>Resultado da verificação.</returns>
        public bool KeyExists(string key, string section = null)
        {
            section = section ?? DefaultSection;
            var data = FileToDictionary(Filename);
            return data.ContainsKey(section) && data[section].ContainsKey(key);
        }

        /// <summary>
        ///     Localiza chaves com base numa expressão.
        /// </summary>
        /// <param name="pattern">Expressão.</param>
        /// <param name="options">Expressão.</param>
        /// <returns>Chaves encontradas.</returns>
        public string[] GetKeys(string pattern = null, RegexOptions options = RegexOptions.None)
        {
            var data = FileToDictionary(Filename);

            return (
                from section in data
                from value in section.Value
                where pattern == null || Regex.IsMatch(value.Key, pattern, options)
                select value.Key
            ).ToArray();
        }
    }
}