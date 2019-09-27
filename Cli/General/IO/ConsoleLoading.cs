// ReSharper disable UnusedMethodReturnValue.Global

using System;
using System.Timers;

namespace Cli.General.IO
{
    /// <summary>
    ///     Animação de "carregando..." para console.
    /// </summary>
    public static class ConsoleLoading
    {
        /// <summary>
        ///     Caracteres de cada sprite de animação.
        /// </summary>
        private const string Sprites = @"-\|/";

        /// <summary>
        ///     Posição do sprite atual.
        /// </summary>
        private static int _position;

        /// <summary>
        ///     Timer.
        /// </summary>
        private static readonly Timer Timer;

        /// <summary>
        ///     Construtor.
        /// </summary>
        static ConsoleLoading()
        {
            Timer = new Timer(100);
            Timer.Elapsed += Animation;
        }

        /// <summary>
        ///     Processa a animação.
        /// </summary>
        /// <param name="sender">Origem do evento.</param>
        /// <param name="e">Informações sobre o evento.</param>
        private static void Animation(object sender, ElapsedEventArgs e)
        {
            if (_position >= Sprites.Length) _position = 0;
            try
            {
                const string format = "{0}\b";
                Console.Write(format, Sprites[_position++]);
            }
            catch
            {
                // Ignora qualquer erro na exibição do sprite, porque logo depois vem o próximo.
            }
        }

        /// <summary>
        ///     Define (ou verifica) o console com animação de carregando.
        /// </summary>
        /// <param name="mode">Opcional. Ativa ou desativa. Se não informado não altera o estado atual.</param>
        /// <returns>Estado atual.</returns>
        public static bool Active(bool? mode = null)
        {
            if (!mode.HasValue) return Timer.Enabled;

            if (mode.Value && !Timer.Enabled)
            {
                Timer.Start();
            }
            else if (!mode.Value && Timer.Enabled)
            {
                Timer.Stop();

                const string format = "\b \b";
                Console.Write(format);
                _position = 0;
            }

            return Timer.Enabled;
        }
    }
}