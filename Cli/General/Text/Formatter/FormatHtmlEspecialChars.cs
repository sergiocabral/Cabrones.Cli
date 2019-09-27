// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Cli.General.Text.Formatter
{
    /// <summary>
    ///     <para>Implementa a interface <see cref="IFormatProvider" />.</para>
    ///     <para>Esta classe converte caracteres especiais do html para texto comum.</para>
    /// </summary>
    public class FormatHtmlEspecialChars : FormatNothingToDo
    {
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
            var result = base.Format(format, arg, formatProvider);
            foreach (var (key, value) in Chars) result = Regex.Replace(result, key, value);
            return result;
        }

        #endregion

        #region Public

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        public FormatHtmlEspecialChars()
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="formatProvider">
        ///     <para>Decora um FormatProvider já existente.</para>
        /// </param>
        public FormatHtmlEspecialChars(IFormatProvider formatProvider) : base(formatProvider)
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="chars">
        ///     <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </param>
        public FormatHtmlEspecialChars(params KeyValuePair<string, string>[] chars) : this(null, chars)
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="chars">
        ///     <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </param>
        /// <param name="formatProvider">
        ///     <para>Decora um FormatProvider já existente.</para>
        /// </param>
        public FormatHtmlEspecialChars(IFormatProvider formatProvider, params KeyValuePair<string, string>[] chars)
            : this(formatProvider)
        {
            if (chars == null) return;

            foreach (var (key, value) in chars) Chars[key] = value;
        }

        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Lista das tags HTML ou XML que devem ser removidas.</para>
        /// </summary>
        public IDictionary<string, string> Chars { get; } = new Dictionary<string, string>
        {
            {"&nbsp;", " "},
            {"&amp;", "&"}
        };

        #endregion
    }
}