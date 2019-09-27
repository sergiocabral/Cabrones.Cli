// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.IO;
using Cli.Business;
using Cli.Business.IO;
using Cli.General.IO;
using Cli.General.Reflection;

namespace Cli.Output.File
{
    /// <summary>
    ///     Output para janela de console.
    /// </summary>
    public class OutputFile : OutputBaseWithFormatter
    {
        /// <summary>
        ///     Construtor.
        /// </summary>
        public OutputFile()
        {
            Filename =
                new FileInfo(Path.Combine(
                    Definitions.DirectoryForUserData.FullName,
                    $"{GetType().GetNamespace()}.{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log"));
            Filename.CreateEmpty();
        }

        /// <summary>
        ///     Arquivo de saída.
        /// </summary>
        public FileInfo Filename { get; }

        /// <summary>
        ///     Solicita a escrita imediata.
        /// </summary>
        /// <param name="text">Texto.</param>
        /// <param name="mark">Marcador.</param>
        protected override void WriteNow(string text, char mark = '\0')
        {
            if (!CanWrite) return;
            
            using (var stream = new FileStream(Filename.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.None))
            {
                stream.Position = stream.Length;
                foreach (var ch in text)
                    if (ch == '\b')
                    {
                        if (stream.Position <= 0) continue;

                        stream.Position--;
                        var chr = (char) stream.ReadByte();
                        if (chr != '\n') stream.Position--;
                    }
                    else
                    {
                        stream.WriteByte((byte) ch);
                    }

                stream.SetLength(stream.Position);
            }
        }
    }
}