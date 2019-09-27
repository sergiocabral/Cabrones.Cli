using Cli.Business.IO;

namespace Cli.Business.Module
{
    /// <summary>
    ///     Interface para módulos do sistema.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        ///     Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        string Context { get; }

        /// <summary>
        ///     Ordem de exibição dentro do contexto do módulo.
        /// </summary>
        int ContextOrder { get; }

        /// <summary>
        ///     Nome de apresentação.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Módulo escondido que não é exibido na lista de opções.
        /// </summary>
        bool Hidden { get; }

        /// <summary>
        ///     Traduções em formato JSON.
        /// </summary>
        string Translates { get; }

        /// <summary>
        ///     Define o output padrão.
        /// </summary>
        /// <param name="output">Instância.</param>
        void SetOutput(IOutput output);

        /// <summary>
        ///     Define o input padrão.
        /// </summary>
        /// <param name="input">Instância.</param>
        void SetInput(IInput input);

        /// <summary>
        ///     Execução do módulo.
        /// </summary>
        void Run();
    }
}