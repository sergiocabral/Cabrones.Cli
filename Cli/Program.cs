using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Cli.Business;
using Cli.Business.IO;
using Cli.Business.Module;
using Cli.General.Reflection;
using Cli.General.Text;

namespace Cli
{
    /// <summary>
    ///     Classe principal.
    /// </summary>
    public sealed class Program : ModuleBase
    {
        /// <summary>
        ///     Construtor.
        /// </summary>
        private Program()
        {
            SetInput(new InputManager());
            SetOutput(new OutputManager
            {
                Prevent = true, 
                LevelFilter = Environment.GetCommandLineArgs().Any(a => a == InputBase.InputAnswerToExit) ? 
                    OutputLevel.CommandLine : 
                    OutputLevel.Interactive
            });
            
            LoadTranslate(Language);
            ExtractAssemblies();
            LoadModules();
            Run();
        }

        /// <summary>
        ///     Referência para o assembly da instância.
        /// </summary>
        public override Assembly ClassAssembly => Assembly.GetExecutingAssembly();

        /// <summary>
        ///     Recebedor de informações do usuário.
        /// </summary>
        private new InputManager Input => base.Input as InputManager;

        /// <summary>
        ///     Exibidor de informações para o usuário.
        /// </summary>
        private new OutputManager Output => base.Output as OutputManager;

        /// <summary>
        ///     Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public override string Context => "{2FE98BB0-1485-4146-9DC3-D67DDF309E3C}";

        /// <summary>
        ///     Método de entrada do sistema operacional.
        /// </summary>
        private static void Main()
        {
            var unused = new Program();
        }

        /// <summary>
        ///     Idioma de execução.
        /// </summary>
        public static string Language
        {
            get
            {
                var environment = Environment.GetEnvironmentVariable("CLI-LANG") ?? Environment.GetEnvironmentVariable("CLI_LANG") ?? string.Empty;

                environment =
                    environment == KnowCommandTranslate ? string.Empty : //Ignora o idioma usado para nomear comandos.
                    Regex.IsMatch(environment, @"^pt[^a-z]*", RegexOptions.IgnoreCase) ? "pt-BR" : //Facilita tentativas de usar o portugues
                    CultureInfo.CurrentUICulture.Name; //Usa o idioma padrão do sistema operacional.
                
                return environment;
            }
        }

        /// <summary>
        ///     Extrai assemblies obrigatórios para a mesma pasta do executável.
        /// </summary>
        private void ExtractAssemblies()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resources = assembly.GetManifestResourceNames();
            foreach (var resource in resources)
            {
                var match = Regex.Match(resource, @"(?<=Properties\.).*\.dll$", RegexOptions.IgnoreCase);
                if (!match.Success) continue;

                var file = new FileInfo(Path.Combine(Definitions.DirectoryForExecutable.FullName, match.Value));
                if (file.Exists && AssemblyHelper.Load(file.FullName) == null)
                    try
                    {
                        file.Delete();
                        file.Refresh();
                    }
                    catch
                    {
                        Output.WriteLine($"!{Phrases.FileDeleteError.Translate()}", file.Name);
                    }

                if (file.Exists) continue;

                var bytes = assembly.GetResourceBinary(resource);
                try
                {
                    File.WriteAllBytes(file.FullName, bytes);
                }
                catch
                {
                    Output.WriteLine($"!{Phrases.FileWriteError.Translate()}", file.Name);
                }
            }
        }

        /// <summary>
        ///     Carrega as traduções de texto.
        /// </summary>
        /// <param name="language">Idioma.</param>
        private void LoadTranslate(string language)
        {
            var resource = Assembly.GetExecutingAssembly().GetResourceString("Translates.json");
            Translate.LoadAll(resource);

            Translate.CreateDefault(language);

            var filename = Definitions.FileMaskForTranslates.Replace("*", Translate.Default.Language);
            var file = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName, filename));

            if (!file.Exists) return;

            try
            {
                var content = File.ReadAllText(file.FullName);
                try
                {
                    Translate.Load(Translate.Default.Language, content);
                }
                catch (Exception ex)
                {
                    Output.WriteLine($"!{Phrases.FileContentInvalid.Translate()}", file.Name);
                    Output.WriteLine($"!{ex.Message}");
                }
            }
            catch (Exception ex)
            {
                Output.WriteLine($"!{Phrases.FileLoadError.Translate()}", file.Name);
                Output.WriteLine($"!{ex.Message}");
            }
        }

        /// <summary>
        ///     Carrega um assembly de um arquivo e instancia a class.
        /// </summary>
        /// <typeparam name="T">Tipo para instanciar.</typeparam>
        /// <param name="fileMask">Filtro para localizar os arquivos.</param>
        /// <param name="verbose">Quando true, exibe mensagem sobre a criação da instância.</param>
        /// <param name="action">Método que recebe a instância.</param>
        private void LoadAndCreate<T>(string fileMask, bool verbose, Action<T> action)
        {
            foreach (var file in Definitions.DirectoryForExecutable.GetFiles(fileMask))
            {
                var assembly = AssemblyHelper.Load(file.FullName);
                try
                {
                    var instances = AssemblyHelper.Load(assembly, typeof(T));
                    if (instances.Length > 0)
                    {
                        foreach (var instance in instances) action((T) instance);
                        if (verbose)
                            Output.WriteLine($"#{Phrases.FileLoadedAssembly.Translate()}", assembly.GetDescription());
                    }
                    else if (verbose)
                    {
                        throw new Exception();
                    }
                }
                catch
                {
                    Output.WriteLine($"!{Phrases.FileLoadError.Translate()}", file.Name);
                }
            }
        }

        /// <summary>
        ///     Carrega os módulos de entrada e saída de informações de/para o usuário.
        /// </summary>
        private void LoadModules()
        {
            Output.WriteLine($"#{Phrases.FileLoadedAssembly.Translate()}", Assembly.GetExecutingAssembly().GetDescription());

            LoadAndCreate(Definitions.FileMaskForOutput, true, (IOutput instance) => Output.Add(instance));
            LoadAndCreate(Definitions.FileMaskForInput, true, (IInput instance) => Input.Add(instance));

            void LoadModule(IModule instance)
            {
                instance.SetOutput(Output);
                instance.SetInput(Input);
                if (!string.IsNullOrWhiteSpace(instance.Translates)) Translate.LoadAll(instance.Translates);
            }

            LoadAndCreate<IModule>(Definitions.FileMaskForOutput, false, LoadModule);
            LoadAndCreate<IModule>(Definitions.FileMaskForInput, false, LoadModule);
            LoadAndCreate<IModule>(Definitions.FileMaskForModule, true, LoadModule);
        }

        /// <summary>
        ///     Execução do módulo.
        /// </summary>
        public override void Run()
        {
            Welcome();

            OnModuleRootChooseOptionEnter += Welcome;
            IgnoreNextWelcome = true;
            
            ChooseModule(string.Empty, "Available modules:");
            
            Output.WriteLine("_{0}", "Finished.".Translate());
        }

        /// <summary>
        ///     Nome exibido na mensagem de boas vindas.
        /// </summary>
        /// <returns></returns>
        private static string WelcomeText()
        {
            var isRootModule = RootModules.Count == 1 && RootModules[0] is ModuleBase;

            return !isRootModule ? 
                Assembly.GetExecutingAssembly().GetDescription(true) : 
                ((ModuleBase) RootModules[0]).ClassAssembly.GetDescription(true);
        }

        /// <summary>
        ///     Sinaliza que a próxima exibição de Welcome deve ser ignorada.
        /// </summary>
        private bool IgnoreNextWelcome { get; set; }

        /// <summary>
        ///     Mensagem de boas-vindas.
        /// </summary>
        private void Welcome()
        {
            if (IgnoreNextWelcome)
            {
                IgnoreNextWelcome = false;
                return;
            }
            
            Output.Prevent = false;

            const char space = '#';
            var name = WelcomeText();
            name = $"{new string(space, 3)} {name} {new string(space, 3)}";
            var line = new string(space, Console.BufferWidth - name.Length - 2);

            Output.WriteLine(
                "^" + (new string(space, name.Length) + " " + line).Replace(space.ToString(),
                    space + space.ToString()));
            Output.WriteLine("^" + (name + " " + line).Replace(space.ToString(), space + space.ToString()));
            Output.WriteLine(
                "^" + (new string(space, name.Length) + " " + line).Replace(space.ToString(),
                    space + space.ToString()));
            Output.WriteLine();

            if (Output.Flushed) return;

            Output.QueueFlush();
            Output.WriteLine();
        }
    }
}