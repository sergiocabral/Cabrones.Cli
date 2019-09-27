// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Auxilia na formatação de textos para output
    ///     Implementar esses marcadores ao escrever texto:
    ///     Pode iniciar a frase ("#Por exemplo") ou conter palavras ("Outro *exemplo* aqui.")
    ///     Título:     ^
    ///     Destacado:  *
    ///     Detalhe:    #
    ///     Dica:       _
    ///     Erro:       !
    ///     Pergunta:   ?
    ///     Resposta:   @
    ///     Nova linha: \n  (não é marcador, mas é usado para padronizar NewLine como Environment.NewLine)
    /// </summary>
    public static class FormatterHelper
    {
        private static char[] _chars;

        /// <summary>
        ///     Caracter marcado: Nova linha
        /// </summary>
        public static char CharNewLine { get; } = '\n';

        /// <summary>
        ///     Caracter marcado: Ignora formatação restante.
        /// </summary>
        public static char CharIgnore { get; } = '$';

        /// <summary>
        ///     Caracter marcado: Título
        /// </summary>
        public static char CharTitle { get; } = '^';

        /// <summary>
        ///     Caracter marcado: Destacado
        /// </summary>
        public static char CharHighlight { get; } = '*';

        /// <summary>
        ///     Caracter marcado: Detalhe
        /// </summary>
        public static char CharDetail { get; } = '#';

        /// <summary>
        ///     Caracter marcado: Dica
        /// </summary>
        public static char CharHint { get; } = '_';

        /// <summary>
        ///     Caracter marcado: Erro
        /// </summary>
        public static char CharError { get; } = '!';

        /// <summary>
        ///     Caracter marcado: Pergunta
        /// </summary>
        public static char CharQuestion { get; } = '?';

        /// <summary>
        ///     Caracter marcado: Resposta
        /// </summary>
        public static char CharAnswer { get; } = '@';

        /// <summary>
        ///     Lista de caracteres especiais.
        /// </summary>
        public static IEnumerable<char> Chars
        {
            get
            {
                if (_chars != null) return _chars;

                _chars = (
                    from property in typeof(FormatterHelper).GetProperties()
                    where typeof(char) == property.PropertyType
                    select (char) property.GetValue(null)
                    into ch
                    where ch != CharNewLine
                    select ch
                ).ToArray();

                return _chars;
            }
        }

        /// <summary>
        ///     Escapa o texto para exibir todos os caracteres de marcadores.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Texto.</returns>
        public static string Escape(string text)
        {
            var result = new StringBuilder(text);
            foreach (var item in Chars) result.Replace(item.ToString(), item + item.ToString());
            return result.ToString();
        }

        /// <summary>
        ///     Extensão para string.
        ///     Escapa o texto para exibir todos os caracteres de marcadores.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <returns>Texto.</returns>
        public static string EscapeForOutput(this string text)
        {
            return Escape(text);
        }

        /// <summary>
        ///     Solicita a escrita parte por parte formatada.
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="write">Função de escrita</param>
        /// <param name="raw">Ignora o formatador. Aplica apenas NewLine.</param>
        /// <param name="rawMark">Marcador padrão usado para o texto raw.</param>
        public static void Output(string text, Action<string, char> write, bool raw = false, char rawMark = (char) 0)
        {
            if (text == null) return;

            text = text.Replace("\r", string.Empty);

            const char noMark = (char) 0;
            var marks = new List<char>();
            var currentText = new StringBuilder();

            if (raw)
            {
                currentText.Append(Escape(text));
                text = string.Empty;
                marks.Add(rawMark);
            }

            char Chr(string str, int index)
            {
                return string.IsNullOrEmpty(str) || index >= str.Length ? noMark : str[index];
            }

            char LastMask()
            {
                return marks.Count == 0 ? noMark : marks[marks.Count - 1];
            }

            bool IsMark(char mark)
            {
                return Chars.Contains(mark);
            }

            var i = 0;
            while (i < text.Length)
            {
                var currentChar = text[i];

                if (currentChar == CharNewLine) //É marca de nova linha
                {
                    currentText.Append(Environment.NewLine);
                    write(currentText.ToString(), LastMask());
                    currentText.Clear();
                    marks.Clear();
                }
                else if (
                    IsMark(currentChar) && //É uma marca
                    currentChar == Chr(text, i + 1) //Marca duplicada
                )
                {
                    currentText.Append(currentChar);
                    i++;
                }
                else if (
                    IsMark(currentChar) //É uma marca
                )
                {
                    if (currentChar == CharIgnore)
                    {
                        //Ignora (raw)
                        currentText.Append(text.Substring(i + 1));
                        break;
                    }

                    if (currentText.Length > 0) //Tem texto pendente
                    {
                        write(currentText.ToString(), LastMask());
                        currentText.Clear();
                    }

                    if (currentChar != LastMask()) //Abre marcação
                        marks.Add(currentChar);
                    else //Fecha marcação em aberto
                        marks.RemoveAt(marks.Count - 1);
                }
                else
                {
                    currentText.Append(currentChar);
                }

                i++;
            }

            if (currentText.Length <= 0) return;

            write(currentText.ToString(), LastMask());
            currentText.Clear();
        }
    }
}