// ReSharper disable MemberCanBeProtected.Global
using Cli.Business.Module;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Classe base para recebedor de informações do usuário.
    /// </summary>
    public abstract class InputBase : IInput
    {
        /// <summary>
        ///     Construtor.
        /// </summary>
        static InputBase()
        {
            ModuleBase.OnModuleRootChooseOptionEnter += () => IsModuleRoot = true;
            ModuleBase.OnModuleRootChooseOptionLeave += () => IsModuleRoot = false;
        }

        /// <summary>
        ///     Indica se um módulo root com opções em exibição.
        /// </summary>
        protected static bool IsModuleRoot { get; private set; }

        /// <summary>
        ///     Texto que finaliza o programa pressionando enter até o final.
        /// </summary>
        public const string InputAnswerToExit = "$";
        
        /// <summary>
        ///     Texto que retorna para a tela inicial do programa pressionando enter até chegar.
        /// </summary>
        public const string InputAnswerToRoot = "/";

        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public abstract string Read();

        /// <summary>
        ///     Verifica se possui resposta prévia.
        /// </summary>
        public abstract bool HasRead();

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public abstract char ReadKey();
    }
}