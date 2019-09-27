using System;
using System.Text.RegularExpressions;

namespace Cli.General.Reflection
{
    /// <summary>
    ///     Extensão de métodos para: Type
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        ///     Retorna o nome do namespace.
        /// </summary>
        /// <param name="type">Tipo.</param>
        /// <returns>Nome descritivo.</returns>
        public static string GetNamespace(this Type type)
        {
            return Regex.Match(type.FullName ?? throw new NullReferenceException(), $@".*(?=\.{type.Name}$)")
                .Value;
        }
    }
}