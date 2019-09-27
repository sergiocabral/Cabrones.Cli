using System;
using System.IO;
using Cli.Properties;

namespace Cli.Business
{
    /// <summary>
    ///     Definições em geral de uso do programa.
    /// </summary>
    public static class Definitions
    {
        /// <summary>
        ///     Caminho do executável.
        /// </summary>
        public static DirectoryInfo DirectoryForExecutable => new DirectoryInfo(Environment.CurrentDirectory);

        /// <summary>
        ///     Caminho para arquivos gerados para o usuário.
        /// </summary>
        public static DirectoryInfo DirectoryForUserData =>
            new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, Resources.DirectoryNameUserData));

        /// <summary>
        ///     Máscara para localizar arquivos de tradução.
        /// </summary>
        public static string FileMaskForTranslates => Resources.FileMaskTranslate;

        /// <summary>
        ///     Máscara para localizar arquivos de Input.
        /// </summary>
        public static string FileMaskForInput => Resources.FileMaskInput;

        /// <summary>
        ///     Máscara para localizar arquivos de Output.
        /// </summary>
        public static string FileMaskForOutput => Resources.FileMaskOutput;

        /// <summary>
        ///     Máscara para localizar arquivos de módulos em geral.
        /// </summary>
        public static string FileMaskForModule => Resources.FileMaskModule;
    }
}