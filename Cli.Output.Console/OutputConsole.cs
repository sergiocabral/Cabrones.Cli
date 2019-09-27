// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using Cli.Business.IO;

namespace Cli.Output.Console
{
    /// <summary>
    ///     Output para janela de console.
    /// </summary>
    public class OutputConsole : OutputBaseWithFormatter
    {
        /// <summary>
        ///     Valor inicial (padrão) para ForegroundColor.
        /// </summary>
        public ConsoleColor DefaultForegroundColor { get; } = System.Console.ForegroundColor;

        /// <summary>
        ///     Valor inicial (padrão) para BackgroundColor.
        /// </summary>
        public ConsoleColor DefaultBackgroundColor { get; } = System.Console.BackgroundColor;

        /// <summary>
        ///     Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected override void WriteNow(string text, char mark = '\0')
        {
            SetColors(mark);
            foreach (var ch in text)
                if (ch == '\b')
                {
                    const string format = "\b \b";
                    if (CanWrite) System.Console.Write(format);
                }
                else
                {
                    if (CanWrite) System.Console.Write(ch);
                }

            SetColors();
        }

        /// <summary>
        ///     Define as cores de acordo com a marcação.
        /// </summary>
        /// <param name="mark"></param>
        private void SetColors(char mark = (char) 0)
        {
            if (mark == FormatterHelper.CharTitle)
            {
                System.Console.ForegroundColor = ConsoleColor.White;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharHighlight)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkCyan;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharDetail)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkGray;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharHint)
            {
                System.Console.ForegroundColor = ConsoleColor.Magenta;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharError)
            {
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharQuestion)
            {
                System.Console.ForegroundColor = ConsoleColor.DarkYellow;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else if (mark == FormatterHelper.CharAnswer)
            {
                System.Console.ForegroundColor = ConsoleColor.Yellow;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
            else
            {
                System.Console.ForegroundColor = DefaultForegroundColor;
                System.Console.BackgroundColor = DefaultBackgroundColor;
            }
        }
    }
}