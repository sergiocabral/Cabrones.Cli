using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Cli.Business;

namespace Cli.General.IO
{
    /// <summary>
    /// Gerencia a execução de processos, etc.
    /// </summary>
    public static class Processes
    {
        /// <summary>
        /// Diretório inicial para execução dos processos.
        /// </summary>
        public static DirectoryInfo WorkingDirectory { get; set; }= Definitions.DirectoryForUserData;

        /// <summary>
        /// Executa um programa.
        /// </summary>
        /// <param name="program">Programa</param>
        /// <param name="arguments">Argumentos</param>
        /// <param name="changeWorkingDirectory">Diretório de execução.</param>
        /// <returns>Saída do output do programa</returns>
        public static IEnumerable<string> Run(string program, string arguments, DirectoryInfo changeWorkingDirectory = null)
        {
            if (changeWorkingDirectory != null) WorkingDirectory = changeWorkingDirectory;
            
            var processStartInfo = new ProcessStartInfo(program)
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory.FullName,
                Arguments = arguments
            };

            var process = Process.Start(processStartInfo);
            if (process == null) return null;

            do
            {
                process.Refresh();
                Thread.Sleep(100);
            } while (!process.HasExited);

            var output = new List<string>();
            output.AddRange(process.StandardError.ReadToEnd().Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            output.AddRange(process.StandardOutput.ReadToEnd().Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

            process.Close();
            process.Dispose();
            
            return output;
        }
    }
}