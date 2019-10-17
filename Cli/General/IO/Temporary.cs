// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMethodReturnValue.Global

using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace Cli.General.IO
{
    /// <summary>
    /// Manipulação de elementos (arquivos, diretórios, etc) temporários.
    /// </summary>
    public static class Temporary
    {
        /// <summary>
        /// Diretório que contem o diretório temporário.
        /// </summary>
        public static DirectoryInfo ParentTemporaryDirectory { get; set; } = new DirectoryInfo(Environment.GetEnvironmentVariable("TEMP") ?? @"./");

        /// <summary>
        /// Força a exclusão de um arquivo ou diretório. 
        /// </summary>
        /// <param name="target">Arquivo ou diretório</param>
        /// <returns>Indica exclusão completa. Caso false indica que algo estava bloqueado.</returns>
        public static bool ForceDelete(FileSystemInfo target)
        {
            if (target is DirectoryInfo asDirectory)
            { 
                foreach (var directory in asDirectory.GetDirectories().Union<FileSystemInfo>(asDirectory.GetFiles()))
                {
                    ForceDelete(directory);
                }

                asDirectory.Attributes = FileAttributes.Directory;
            }
            else
            {
                target.Attributes = FileAttributes.Archive;
            }

            try
            {
                target.Delete();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Apaga todos os diretórios temporários.
        /// </summary>
        /// <param name="directory">Diretório para exclusão</param>
        /// <param name="except">Exceto os desta lista.</param>
        public static void TryDeleteDirectory(DirectoryInfo directory, params string[] except)
        {
            if (directory == null) return;
            except = except ?? new string[] { };
                
            ThreadPool.QueueUserWorkItem(state =>
            {
                foreach (var child in directory
                    .GetDirectories().Where(a => !except.Contains(a.Name)).Union<FileSystemInfo>(directory
                        .GetFiles().Where(a => !except.Contains(a.Name))))
                    ForceDelete(child);

                if (!directory.GetFiles().Any() && !directory.GetDirectories().Any()) 
                    ForceDelete(directory);
            });
        }

        /// <summary>
        /// Retorna um diretório temporário já criado.
        /// </summary>
        /// <param name="name">Nome do diretório.</param>
        /// <param name="deleteOthers">Quando true apaga os demais diretórios temporários.</param>
        /// <param name="temporaryDirectoryName">Nome da pasta temporária</param>
        /// <returns>Diretório criado.</returns>
        public static DirectoryInfo CreateDirectory(string name = null, bool deleteOthers = true, string temporaryDirectoryName = "Temp")
        {
            var dirTemporary = new DirectoryInfo(Path.Combine(ParentTemporaryDirectory.FullName, temporaryDirectoryName));
            if (!dirTemporary.Exists) dirTemporary.Create();

            // ReSharper disable once StringLiteralTypo
            dirTemporary = dirTemporary.CreateSubdirectory(name ?? DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (deleteOthers) TryDeleteDirectory(dirTemporary.Parent, dirTemporary.Name);
            
            return dirTemporary;
        }
    }
}