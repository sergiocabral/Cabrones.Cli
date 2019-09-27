// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global

using System;
using System.Diagnostics;
using System.Threading;

namespace Cli.General.IO
{
    public static class ConsoleReadLine
    {
        /// <summary>
        ///     Última resposta de Console.ReadLine()
        /// </summary>
        private static string _inputLast;

        /// <summary>
        ///     Thread que processa as respostas de Console.ReadLine() quando houver oportunidade.
        /// </summary>
        private static readonly Thread InputThread = new Thread(InputThreadAction) {IsBackground = true};

        /// <summary>
        ///     Notifica oportunidade para obter resposta de Console.ReadLine().
        /// </summary>
        private static readonly AutoResetEvent InputGet = new AutoResetEvent(false);

        /// <summary>
        ///     Notifica que foi obtido resposta de Console.ReadLine().
        /// </summary>
        private static readonly AutoResetEvent InputGot = new AutoResetEvent(false);

        /// <summary>
        ///     Construtor.
        /// </summary>
        static ConsoleReadLine()
        {
            InputThread.Start();
        }

        /// <summary>
        ///     Processamento da thread que processa as respostas de Console.ReadLine() quando houver oportunidade.
        /// </summary>
        private static void InputThreadAction()
        {
            while (true)
            {
                InputGet.WaitOne();
                _inputLast = Console.ReadLine();
                InputGot.Set();
            }

            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        ///     Obtem resposta de Console.ReadLine() com timeout.
        ///     Se estourar o tempo gera Exception.
        /// </summary>
        /// <param name="timeout">Opcional. Tempo limite de espera.</param>
        /// <returns>Resposta.</returns>
        public static string ReadLine(int timeout = Timeout.Infinite)
        {
            if (timeout == Timeout.Infinite) return Console.ReadLine();

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.ElapsedMilliseconds < timeout && !Console.KeyAvailable)
            {
                // Aguarda o tempo limite.
            }

            if (!Console.KeyAvailable) throw new TimeoutException("User did not provide input within the time limit.");

            InputGet.Set();
            InputGot.WaitOne();
            return _inputLast;
        }

        /// <summary>
        ///     Obtem resposta de Console.ReadLine() com timeout.
        /// </summary>
        /// <param name="timeout">Tempo limite de espera.</param>
        /// <param name="defaults">Resposta padrão se estourar o timeout.</param>
        /// <returns>Resposta.</returns>
        public static string ReadLine(int timeout, string defaults)
        {
            try
            {
                return ReadLine(timeout);
            }
            catch
            {
                return defaults;
            }
        }
    }
}