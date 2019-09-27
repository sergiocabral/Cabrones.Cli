// ReSharper disable UnusedMember.Global

using System;
using System.Web;

namespace Cli.General.Text.Formatter
{
    /// <summary>
    ///     <para>Implementa a interface <see cref="IFormatProvider" />.</para>
    ///     <para>
    ///         Esta classe extrai de um código HTML ou XML o texto real sem as tags
    ///         e sem códigos para caracteres especiais.
    ///     </para>
    /// </summary>
    public class FormatExtractTextFromHtml : FormatNothingToDo
    {
        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        public FormatExtractTextFromHtml()
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="formatProvider">
        ///     <para>Decora um FormatProvider já existente.</para>
        /// </param>
        public FormatExtractTextFromHtml(IFormatProvider formatProvider) : base(formatProvider)
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
            var semHtmlTag = string.Format(new FormatRemoveTags(), "{0}", base.Format(format, arg, formatProvider));
            return HttpUtility.HtmlDecode(semHtmlTag);
        }

        #endregion
    }
}