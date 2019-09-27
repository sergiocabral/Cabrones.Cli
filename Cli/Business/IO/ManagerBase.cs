using System.Collections.Generic;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Classe base para gerenciadores de Input ou Output.
    /// </summary>
    public abstract class ManagerBase<T> : IManager<T>
    {
        protected IList<T> Items { get; } = new List<T>();

        /// <summary>
        ///     Adiciona um item gerenciado.
        /// </summary>
        /// <param name="item">Item gerenciado.</param>
        public virtual void Add(T item)
        {
            Items.Add(item);
        }
    }
}