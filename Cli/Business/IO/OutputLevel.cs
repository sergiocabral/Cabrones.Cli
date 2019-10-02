using System;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Classificação do nível do texto para exibição.
    /// </summary>
    [Flags]
    public enum OutputLevel
    {
        /// <summary>
        /// Comum a todas as mensagens.
        /// </summary>
        Interactive = 0b_01,

        /// <summary>
        /// Nível prioritário. Usado para quando for exibir apenas o retorno via linha de comando.
        /// Corresponde a Common e mais um bit.
        /// </summary>
        CommandLine = 0b_11,
    }
}