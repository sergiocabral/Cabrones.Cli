// ReSharper disable UnusedMethodReturnValue.Global

using System.IO;

namespace Cli.General.IO
{
    /// <summary>
    ///     Extensão para FileInfo
    /// </summary>
    public static class FileInfoExtension
    {
        /// <summary>
        ///     Cria um arquivo vazio.
        /// </summary>
        /// <param name="file">Arquivo.</param>
        /// <param name="createDirectories">Força cria a estrutura de diretórios.</param>
        /// <returns>Sucesso na criação.</returns>
        public static bool CreateEmpty(this FileInfo file, bool createDirectories = true)
        {
            file.Refresh();

            if (file.Directory != null && !file.Directory.Exists && !createDirectories) return false;

            if (createDirectories) file.Directory.ForceCreate();

            file.AppendText().Close();

            return true;
        }
    }
}