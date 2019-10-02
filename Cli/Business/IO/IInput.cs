namespace Cli.Business.IO
{
    /// <summary>
    ///     Interface para recebedor de informações do usuário.
    /// </summary>
    public interface IInput
    {
        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <param name="isSensitive">Indica se deve ser tratado como dado sensível.</param>
        /// <returns>Entrada do usuário</returns>
        string Read(bool isSensitive = false);

        /// <summary>
        ///     Verifica se possui resposta prévia.
        /// </summary>
        bool HasRead();

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        char ReadKey();
    }
}