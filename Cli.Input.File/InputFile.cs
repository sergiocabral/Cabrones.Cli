// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System.Diagnostics;
using System.IO;
using System.Linq;
using Cli.Business;
using Cli.Business.IO;
using Cli.General.IO;
using Cli.General.Reflection;

namespace Cli.Input.File
{
    /// <summary>
    ///     Input pela janela de console.
    /// </summary>
    public class InputFile : InputBase
    {
        /// <summary>
        ///     Última resposta. Quando != null retorna texto vazio para forçar saída do programa.
        /// </summary>
        private string _lastAnswer;

        /// <summary>
        ///     Resultado da última verificação de HasRead()
        /// </summary>
        private bool _lastHasRead;

        /// <summary>
        ///     Construtor.
        /// </summary>
        public InputFile()
        {
            Filename = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName,
                $"{GetType().GetNamespace()}.txt"));
            Filename.CreateEmpty();
            Filename.OpenWrite().Close();
        }

        public FileInfo Filename { get; }

        /// <summary>
        ///     Intervalo entre verificações de HasRead(). (para evitar acesso a disco muito frequente).
        /// </summary>
        public int Interval { get; set; } = 1000;

        /// <summary>
        ///     Temporizador para evitar tentativas muito próximas.
        /// </summary>
        private Stopwatch Stopwatch { get; } = new Stopwatch();

        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <param name="isSensitive">Indica se deve ser tratado como dado sensível.</param>
        /// <returns>Entrada do usuário</returns>
        public override string Read(bool isSensitive = false)
        {
            if (_lastAnswer != null) return string.Empty;

            Stopwatch.Stop();
            Filename.Refresh();

            if (!Filename.Exists || Filename.Length <= 0) return null;

            var lines = System.IO.File.ReadAllLines(Filename.FullName).ToList();

            if (lines.Count <= 0) return null;

            var answer = lines[0];
            lines.RemoveAt(0);
            System.IO.File.WriteAllLines(Filename.FullName, lines.ToArray());

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

            if (Stopwatch.IsRunning && Stopwatch.ElapsedMilliseconds <= Interval) return _lastHasRead;

            Stopwatch.Restart();
            Filename.Refresh();
            if (Filename.Exists && Filename.Length > 0)
                _lastHasRead = System.IO.File.ReadAllLines(Filename.FullName).Any();
            else
                _lastHasRead = false;

            return _lastHasRead;
        }

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public override char ReadKey()
        {
            var read = Read();
            return read.Length > 0 ? read[0] : (char) 0;
        }
    }
}