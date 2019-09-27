// ReSharper disable UnusedMember.Global

using System;
using System.Globalization;

namespace Cli.General.Text.Formatter
{
    /// <summary>
    ///     <para>Implementa a interface <see cref="IFormatProvider" />.</para>
    ///     <para>Formata a exibição do tamanho de arquivo.</para>
    /// </summary>
    public class FormatFileSize : FormatNothingToDo
    {
        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        public FormatFileSize()
        {
        }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="formatProvider">
        ///     <para>Decora um FormatProvider já existente.</para>
        /// </param>
        public FormatFileSize(IFormatProvider formatProvider) : base(formatProvider)
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
            var size = Convert.ToInt64(base.Format(format, arg, formatProvider));

            var culture = CultureInfo.GetCultureInfo("en-US");

            var result =
                size < 1024 ? string.Format(culture, "{0:n} B", size) :
                size / 1024 < 1024 ? string.Format(culture, "{0:n} KB", (double) size / 1024) :
                size / 1024 / 1024 < 1024 ? string.Format(culture, "{0:n} MB", (double) size / 1024 / 1024) :
                string.Format(culture, "{0:n} GB", (double) size / 1024 / 1024 / 1024);

            return result;
        }

        #endregion
    }
}