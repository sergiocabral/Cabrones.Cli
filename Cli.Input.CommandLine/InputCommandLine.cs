// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Collections.Generic;
using Cli.Business.IO;

namespace Cli.Input.CommandLine
{
    /// <summary>
    ///     Input pela linha de comando.
    /// </summary>
    public class InputCommandLine : InputBase
    {
        /// <summary>
        ///     Última resposta. Quando != null retorna texto vazio para forçar saída do programa.
        /// </summary>
        private string _lastAnswer;

        /// <summary>
        ///     Construtor.
        /// </summary>
        public InputCommandLine()
        {
            CommandLineArgs = new List<string>(Environment.GetCommandLineArgs());
            CommandLineArgs.RemoveAt(0);
        }

        /// <summary>
        ///     Argumentos na fila.
        /// </summary>
        public List<string> CommandLineArgs { get; }

        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public override string Read()
        {
            if (_lastAnswer != null) return string.Empty;

            if (CommandLineArgs.Count == 0) return null;

            var answer = CommandLineArgs[0];
            CommandLineArgs.RemoveAt(0);

            if (answer != InputAnswerToExit && answer != InputAnswerToRoot) return answer;
            _lastAnswer = answer;
            return string.Empty;
        }

        /// <summary>
        ///     Verifica se possui resposta prévia.
        /// </summary>
        public override bool HasRead()
        {
            // ReSharper disable once ArrangeRedundantParentheses
            if (_lastAnswer == InputAnswerToExit || (_lastAnswer == InputAnswerToRoot && !IsModuleRoot)) return true;
            _lastAnswer = null;

            return CommandLineArgs.Count > 0;
        }

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public override char ReadKey()
        {
            return (char) 0;
        }
    }
}