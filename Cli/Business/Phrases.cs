// ReSharper disable UnusedMember.Global
namespace Cli.Business
{
    /// <summary>
    ///     Coleção de frases de uso comum.
    /// </summary>
    public static class Phrases
    {
        /// <summary>
        ///     dd/MM/yyyy
        /// </summary>
        public const string FormatDate = "MM/dd/yyyy";
        
        /// <summary>
        ///     HH:mm
        /// </summary>
        public const string FormatTime = "hh:mm tt";
        
        /// <summary>
        ///     dd/MM/yyyy HH:mm
        /// </summary>
        public const string FormatDateTime = "MM/dd/yyyy hh:mm tt";
        
        /// <summary>
        ///     Módulo carregado: {0}
        /// </summary>
        public const string FileLoadedAssembly = "System component loaded: {0}";

        /// <summary>
        ///     Ocorreu um erro ao carregar o arquivo \"{0}\".
        /// </summary>
        public const string FileLoadError = "There was an error loading the file \"{0}\".";

        /// <summary>
        ///     O conteúdo do arquivo \"{0}\" não é válido.
        /// </summary>
        public const string FileContentInvalid = "The contents of the file \"{0}\" are not valid.";

        /// <summary>
        ///     Ocorreu um erro ao gravar o arquivo \"{0}\".
        /// </summary>
        public const string FileWriteError = "There was an error writing file \"{0}\".";

        /// <summary>
        ///     Ocorreu um erro ao apagar o arquivo \"{0}\".
        /// </summary>
        public const string FileDeleteError = "There was an error deleting file \"{0}\".";

        /// <summary>
        ///     Escolha um ou deixe em branco para sair:
        /// </summary>
        public const string ChooseOne = "Choose or input blank to exit: ";

        /// <summary>
        ///     Escolha um ou deixe em branco para sair (selecione vários com Regex):
        /// </summary>
        public const string ChooseMultiple = "Choose multiple with _Regex_ or input blank to exit: ";

        /// <summary>
        ///     Escolha errada, cara
        /// </summary>
        public const string ChooseWrong = "Wrong choice, dude.";

        /// <summary>
        ///     (em branco)
        /// </summary>
        public const string ChooseBlank = "(blank)";

        /// <summary>
        ///     Confirmado
        /// </summary>
        public const string Confirmed = "Confirmed";

        /// <summary>
        ///     Digite \"{0}\" para confirmar a seleção anterior:
        /// </summary>
        public const string ConfirmSelection = "Type \"{0}\" to confirm previous selection:";
            
        /// <summary>
        ///     Cancelado
        /// </summary>
        public const string Canceled = "Canceled";

        /// <summary>
        ///     Em desenvolvimento.
        /// </summary>
        public const string NotImplemented = "Under development.";

        /// <summary>
        ///     Pressione qualquer tecla para continuar.
        /// </summary>
        public const string PressAnyKey = "Press any key to continue.";

        /// <summary>
        ///     Recursos:
        /// </summary>
        public const string Resources = "Resources:";

        /// <summary>
        ///     Operações:
        /// </summary>
        public const string Operations = "Operations:";

        /// <summary>
        ///     Para pausar use [P], para parar use [ESC].
        /// </summary>
        public const string LoopControl = "To pause use [P], to stop use [ESC].";

        /// <summary>
        ///     Execução cancelada pelo usuário.
        /// </summary>
        public const string LoopCanceled = "Execution canceled by user.";

        /// <summary>
        ///     Execução pausada pelo usuário. Pressione qualquer tecla para continuar.
        /// </summary>
        public const string LoopPaused = "Execution paused by the user. Press any key to continue.";
    }
}