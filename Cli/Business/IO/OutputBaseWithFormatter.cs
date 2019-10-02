namespace Cli.Business.IO
{
    /// <summary>
    ///     Classe base para exibidor de informações para o usuário.
    /// </summary>
    public abstract class OutputBaseWithFormatter : IOutput
    {
        /// <summary>
        ///     Nível usado para exibir output.
        /// </summary>
        public OutputLevel LevelFilter { get; set; }
        
        /// <summary>
        ///     Nível atual para escrita das mensagens.
        /// </summary>
        public OutputLevel Level { get; set; }

        /// <summary>
        ///     Determina se um texto pode ser escrito.
        /// </summary>
        protected bool CanWrite => (Level & LevelFilter) == LevelFilter;
        
        /// <summary>
        ///     Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public IOutput Write(string format, params object[] args)
        {
            var text = string.Format(format, args);

            FormatterHelper.Output(text, WriteNow);

            return this;
        }

        /// <summary>
        ///     Escreve um texto formatado e adiciona nova uma linha.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public IOutput WriteLine(string format = "", params object[] args)
        {
            return Write($"{format}\n", args);
        }

        /// <summary>
        ///     Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected abstract void WriteNow(string text, char mark = (char) 0);
    }
}