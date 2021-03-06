﻿using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Cli.General.Reflection
{
    /// <summary>
    ///     Extensão de métodos para: Assembly
    /// </summary>
    public static class AssemblyExtension
    {
        /// <summary>
        ///     Retorna o nome descritivo para um assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="shortName">Quando true retorna a última parte do nome, sem o namespace.</param>
        /// <returns>Nome descritivo.</returns>
        public static string GetDescription(this Assembly assembly, bool shortName = false)
        {
            var name = assembly.GetName();
            var nameText = !shortName ? name.Name : Regex.Replace(name.Name, @"^[\w\.]*\.", string.Empty);
            return $"{nameText} v{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}";
        }

        /// <summary>
        ///     Lê um recurso do assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="name">Nome do recurso.</param>
        /// <returns>Conteúdo do recurso.</returns>
        public static string GetResourceString(this Assembly assembly, string name)
        {
            string resource = null;
            foreach (var item in assembly.GetManifestResourceNames())
                if (item.IndexOf(name, StringComparison.Ordinal) >= 0)
                {
                    if (resource != null) throw new ArgumentException();
                    resource = item;
                }

            using (var stream = assembly.GetManifestResourceStream(resource))
            using (var reader = stream != null ? new StreamReader(stream) : null)
            {
                return reader?.ReadToEnd();
            }
        }

        /// <summary>
        ///     Lê um recurso do assembly.
        /// </summary>
        /// <param name="assembly">Assembly.</param>
        /// <param name="name">Nome do recurso.</param>
        /// <returns>Conteúdo do recurso.</returns>
        public static byte[] GetResourceBinary(this Assembly assembly, string name)
        {
            using (var stream = assembly.GetManifestResourceStream(name))
            using (var reader = stream != null ? new BinaryReader(stream) : null)
            {
                return reader?.ReadBytes((int) stream.Length);
            }
        }
    }
}