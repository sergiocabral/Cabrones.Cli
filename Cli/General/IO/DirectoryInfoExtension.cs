using System.IO;

namespace Cli.General.IO
{
    /// <summary>
    ///     Extensão para DirectoryInfo
    /// </summary>
    public static class DirectoryInfoExtension
    {
        /// <summary>
        ///     Cria a estrutura de diretórios.
        /// </summary>
        /// <param name="directory">Diretório.</param>
        public static void ForceCreate(this DirectoryInfo directory)
        {
            directory.Refresh();
            if (directory.Parent != null && !directory.Parent.Exists) directory.Parent.ForceCreate();
            directory.Create();
        }
    }
}