// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Cli.General.Text.Formatter
{
    /// <summary>
    ///     <para>Implementa a interface <see cref="IFormatProvider" />.</para>
    ///     <para>Converte um código html para o formato xml.</para>
    /// </summary>
    public class FormatHtmlToXml : FormatNothingToDo
    {
        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        public FormatHtmlToXml()
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="formatProvider">
        ///     <para>Decora um FormatProvider já existente.</para>
        /// </param>
        public FormatHtmlToXml(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        #region ICustomFormatter Members

        /// <summary>
        ///     <para>
        ///         Converte o valor de um objeto especificado para um representação
        ///         de sequência equivalente usando o formato especificado e
        ///         informações de formatação da região (culture-specific).
        ///     </para>
        /// </summary>
        /// <param name="format">
        ///     <para>
        ///         A sequência de formato que contém
        ///         especificações de formatação.
        ///     </para>
        /// </param>
        /// <param name="arg">
        ///     <para>O objeto a ser formatado.</para>
        /// </param>
        /// <param name="formatProvider">
        ///     <para>
        ///         Um objeto que fornece informações sobre
        ///         o formato da instância atual.
        ///     </para>
        /// </param>
        /// <returns>
        ///     <para>
        ///         A sequência de texto <paramref name="arg" /> formatada
        ///         conforme especificado pelos parâmetros <paramref name="format" /> e
        ///         <paramref name="formatProvider" />.
        ///     </para>
        /// </returns>
        public override string Format(string format, object arg, IFormatProvider formatProvider)
        {
            var result = new StringBuilder(base.Format(format, arg, formatProvider)
                .Replace("&", "&amp;")
            );

            var positions = new List<int>();
            foreach (Match match1 in Regex.Matches(result.ToString(),
                @"(?<=\<[a-z]+\s[^\>]*[a-z0-9]*\=)\""[^\>]*?\""(?=[\>\s])",
                RegexOptions.IgnoreCase | RegexOptions.Singleline))
                if (match1.Value.Length > 2 &&
                    match1.Value.IndexOf("\"", 1, StringComparison.Ordinal) < match1.Value.Length - 1)
                    foreach (Match match2 in Regex.Matches(match1.Value.Substring(1, match1.Value.Length - 2), @""""))
                        positions.Add(match1.Index + 1 + match2.Index);
            foreach (var position in positions.OrderByDescending(a => a))
            {
                result.Remove(position, 1);
                result.Insert(position, "&quot;");
            }

            return result.ToString();
        }

        #endregion
    }
}