// ReSharper disable UnusedMember.Global

using System;
using Cli.Business.IO;

namespace Cli.Input.Console
{
    /// <summary>
    ///     Output para janela de console.
    /// </summary>
    public class InputConsole : InputBase
    {
        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
            var cursorLeft = System.Console.CursorLeft;
            var answer = System.Console.ReadLine() ?? string.Empty;
            var cursorTop = System.Console.CursorTop - 1;

            var lengthExtra = answer.Length - (System.Console.BufferWidth - cursorLeft);

            if (lengthExtra > 0) cursorTop -= (int) Math.Floor((double) lengthExtra / System.Console.BufferWidth) + 1;

            System.Console.SetCursorPosition(cursorLeft, cursorTop);
            System.Console.WriteLine(new string(' ', answer.Length));
            System.Console.SetCursorPosition(cursorLeft, cursorTop);

            return answer;
        }

        /// <summary>
        ///     Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead()
        {
            return System.Console.KeyAvailable;
        }

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public override char ReadKey()
        {
            return System.Console.ReadKey(true).KeyChar;
        }
    }
}