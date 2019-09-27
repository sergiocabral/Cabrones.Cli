using System.Linq;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Gerenciador de inputs
    /// </summary>
    public sealed class InputManager : ManagerBase<IInput>, IInput
    {
        /// <summary>
        ///     Recebe uma entrada do usuário.
        /// </summary>
        /// <returns>Entrada do usuário</returns>
        public string Read()
        {
            do
            {
                foreach (var item in Items)
                    if (item.HasRead())
                        return item.Read();
            } while (true);
        }

        /// <summary>
        ///     Verifica se possui resposta prévia.
        /// </summary>
        public bool HasRead()
        {
            return Items.Any(item => item.HasRead());
        }

        /// <summary>
        ///     Solicita uma tecla do usuário para continuar.
        /// </summary>
        /// <returns>Caracter recebido.</returns>
        public char ReadKey()
        {
            do
            {
                foreach (var item in Items)
                    if (item.HasRead())
                        return item.ReadKey();
            } while (true);
        }
    }
}