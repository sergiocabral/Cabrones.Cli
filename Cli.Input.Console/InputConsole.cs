// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

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
        /// Faz a leitura de entrada do usuário.
        /// </summary>
        /// <param name="isSensitive">Indica se deve ser tratado como dado sensível.</param>
        /// <returns></returns>
        public static string ReadLine(bool isSensitive = false)
        {
            if (!isSensitive) return System.Console.ReadLine();

            const string sensitiveChar = "*";
            
            var answer = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = System.Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace && answer.Length > 0)
                {
                    //Processa o backspace.
                    answer = answer.Substring(0, answer.Length - 1);
                    if (System.Console.CursorLeft > 0)
                    {
                        System.Console.CursorLeft--;
                        System.Console.Write(@" ");
                        System.Console.CursorLeft--;
                    }
                    else
                    {
                        System.Console.CursorTop--;
                        System.Console.CursorLeft = System.Console.BufferWidth - 1;
                        System.Console.Write(@" ");
                        System.Console.CursorTop--;
                        System.Console.CursorLeft = System.Console.BufferWidth - 1;
                    }
                } else if (key.KeyChar >= 32)
                {
                    //Adiciona apenas caracteres válidos.
                    answer += key.KeyChar;
                    System.Console.Write(sensitiveChar);
                }
            } while (key.Key != ConsoleKey.Enter);

            System.Console.WriteLine();

            return answer;
        }
        
        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <param name="isSensitive">Indica se deve ser tratado como dado sensível.</param>
        /// <returns>Entrada do usuário</returns>
        public override string Read(bool isSensitive = false)
        {
            var cursorLeft = System.Console.CursorLeft;
            var answer = ReadLine(isSensitive) ?? string.Empty;
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