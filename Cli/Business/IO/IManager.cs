// ReSharper disable UnusedMemberInSuper.Global

namespace Cli.Business.IO
{
    /// <summary>
    ///     Interface para gerenciadores de Input ou Output.
    /// </summary>
    public interface IManager<in T>
    {
        /// <summary>
        ///     Adiciona um item gerenciado.
        /// </summary>
        /// <param name="item">Item gerenciado.</param>
        void Add(T item);
    }
}