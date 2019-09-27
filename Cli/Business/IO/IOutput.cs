namespace Cli.Business.IO
{
    /// <summary>
    ///     Interface para exibidor de informações para o usuário.
    /// </summary>
    public interface IOutput
    {
        /// <summary>
        ///     Nível usado para exibir output.
        /// </summary>
        OutputLevel LevelFilter { get; set; }
        
        /// <summary>
        ///     Nível atual para escrita das mensagens.
        /// </summary>
        OutputLevel Level { get; set; }
        
        /// <summary>
        ///     Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        IOutput Write(string format, params object[] args);

        /// <summary>
        ///     Escreve um texto formatado e adiciona nova uma linha.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        IOutput WriteLine(string format = "", params object[] args);
    }
}