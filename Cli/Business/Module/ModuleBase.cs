// ReSharper disable PublicConstructorInAbstractClass
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable VirtualMemberNeverOverridden.Global

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Cli.Business.IO;
using Cli.General.IO;
using Cli.General.Reflection;
using Cli.General.Text;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Cli.Business.Module
{
    /// <summary>
    ///     Class base para módulos do sistema.
    /// </summary>
    public abstract class ModuleBase : IModule
    {
        /// <summary>
        ///     Nome para a tradução que na verdade contem os comandos das opções.
        /// </summary>
        public const string KnowCommandTranslate = "command";
            
        /// <summary>
        ///     Indica se este é o módulo Root.
        /// </summary>
        public static IModule ModuleRoot { get; set; }

        /// <summary>
        ///     Evento chamado quando é exibido opções no módulo root.
        /// </summary>
        public static event Action OnModuleRootChooseOptionEnter;

        /// <summary>
        ///     Evento chamado quando é exibido opções de outro módulo que não seja root.
        /// </summary>
        public static event Action OnModuleRootChooseOptionLeave;

        /// <summary>
        ///     Construtor.
        /// </summary>
        public ModuleBase()
        {
            if (ModuleRoot == null)
            {
                ModuleRoot = this;
            }

            AllModules.Add(this);
        }

        /// <summary>
        ///     Lista de todos os módulos.
        /// </summary>
        public static IList<IModule> AllModules { get; } = new List<IModule>();

        /// <summary>
        ///     Módulos raiz, ou seja, com contexto vazio.
        /// </summary>
        public static IList<IModule> RootModules => AllModules.Where(a => a.Context == string.Empty).ToList();

        /// <summary>
        ///     Referência para o assembly da instância.
        /// </summary>
        public abstract Assembly ClassAssembly { get; }

        /// <summary>
        ///     Output padrão.
        /// </summary>
        public IOutput Output { get; private set; }

        /// <summary>
        ///     Input padrão.
        /// </summary>
        public IInput Input { get; private set; }

        private IniFile _iniFile;
        /// <summary>
        ///     Banco de dados.
        /// </summary>
        public IniFile IniFile
        {
            get
            {
                if (_iniFile != null) return _iniFile;

                var file = new FileInfo(Path.Combine(Definitions.DirectoryForUserData.FullName,
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.ini"));
                _iniFile = new IniFile(file.FullName, GetType().FullName);

                return _iniFile;
            }
            set => _iniFile = value;
        }

        /// <summary>
        ///     Contexto do módulo. Apenas com Context vazio aparecem na listagem inicial do programa.
        /// </summary>
        public virtual string Context => string.Empty;

        /// <summary>
        ///     Ordem de exibição dentro do contexto do módulo.
        /// </summary>
        public virtual int ContextOrder { get; } = 0;

        /// <summary>
        ///     Módulo escondido que não é exibido na lista de opções.
        /// </summary>
        public virtual bool Hidden { get; } = false;

        private string _name;
        /// <summary>
        ///     Nome de apresentação.
        /// </summary>
        public virtual string Name =>
            _name ?? (_name = Regex.Match(GetType().FullName ?? string.Empty, @"[^\.]*(?=\.[^\.]*$)").Value);

        private string _translates;
        /// <summary>
        ///     Traduções em formato JSON.
        /// </summary>
        public virtual string Translates =>
            _translates ?? (_translates = ClassAssembly.GetResourceString("Translates.json"));

        /// <summary>
        ///     Define o output padrão.
        /// </summary>
        /// <param name="output">Instância.</param>
        public void SetOutput(IOutput output)
        {
            Output = output;
        }

        /// <summary>
        ///     Define o input padrão.
        /// </summary>
        /// <param name="input">Instância.</param>
        public void SetInput(IInput input)
        {
            Input = input;
        }

        /// <summary>
        ///     Execução do módulo.
        /// </summary>
        public abstract void Run();

        /// <summary>
        ///     Mensagem pronta para "Em desenvolvimento"
        /// </summary>
        protected void NotImplemented()
        {
            Output.WriteLine($"#{Phrases.NotImplemented.Translate()}").WriteLine();
            Output.Write($"?{Phrases.PressAnyKey.Translate()}");
            Input.ReadKey();
            Output.WriteLine().WriteLine();
        }
        
        /// <summary>
        ///     Extrai o nome de um objeto.
        /// </summary>
        /// <param name="obj">Objeto</param>
        /// <returns>Nome como texto.</returns>
        private static string GetName(object obj)
        {
            var knowProperties = new List<string>
            {
                "Name",
                "Description",
                "Text",
                "Value",
                "Key"
            };
            try
            {
                foreach (var propertyInfo in knowProperties
                    .Select(property => obj.GetType().GetProperty(property))
                    .Where(propertyInfo => propertyInfo != null))
                {
                    return Convert.ToString(propertyInfo.GetValue(obj)) + string.Empty;
                }
            }
            catch
            {
                //Ignora em caso de erro e retorna a conversão comum ToString() 
            }
            return Convert.ToString(obj) + string.Empty;
        }
        
        /// <summary>
        ///     Formata uma lista para exibição.
        /// </summary>
        /// <param name="options">Lista de opções.</param>
        /// <returns>Lista como texto.</returns>
        public static string Options<T>(IList<T> options)
        {
            var padding = options.Count.ToString().Length;
            var list = new List<List<string>>();
            var i = 0;
            foreach (var option in options)
            {
                string text = null;
                if (typeof(T) != typeof(string) && option is IEnumerable enumerable)
                    foreach (var first in enumerable)
                    {
                        text = GetName(first);
                        break;
                    }
                else
                    text = GetName(option);

                list.Add(new List<string>
                {
                    (++i).ToString().PadLeft(padding), 
                    text,
                    text.Translate(), 
                    text.Translate(KnowCommandTranslate)
                });
            }

            var result = new StringBuilder();
            
            const int indexNumber = 0;
            const int indexOriginal = 1;
            const int indexTranslated = 2;
            const int indexCommand = 3;
            
            padding = list.Where(a => a[indexOriginal] != a[indexCommand]).Select(a => a[indexCommand].Length).OrderByDescending(a => a).FirstOrDefault();
            foreach (var item in list)
            {
                if (padding == 0)
                {
                    result.Append($" {item[indexNumber].EscapeForOutput()}) {item[indexTranslated].EscapeForOutput()}\n");
                }
                else
                {
                    var command = item[indexOriginal] != item[indexCommand] ? $"{item[indexCommand]}:" : string.Empty;
                    result.Append($" {item[indexNumber]}) #{command.PadLeft(padding + 1).EscapeForOutput()}# {item[indexTranslated].EscapeForOutput()}\n");
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Exibe um lista para seleção.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Lista de opções.</param>
        /// <param name="title">Título.</param>
        /// <returns>Resposta com índice e opção selecionada. Índice -1 para nenhuma seleção.</returns>
        public KeyValuePair<int, T> ChooseOption<T>(IList<T> options, string title)
        {
            var result = ChooseOptions(options, title, false);
            return result.Length > 0 ? result[0] : new KeyValuePair<int, T>(-1, default);
        }
        
        /// <summary>
        ///     Exibe um lista para seleção. Seleção múltipla com regex.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Lista de opções.</param>
        /// <param name="title">Título.</param>
        /// <returns>Resposta com índice e opção selecionada. Índice -1 para nenhuma seleção.</returns>
        public KeyValuePair<int, T>[] ChooseOptions<T>(IList<T> options, string title)
        {
            return ChooseOptions(options, title, true);
        }

        /// <summary>
        ///     Exibe um lista para seleção.
        /// </summary>
        /// <typeparam name="T">Tipo do conteúdo da lista.</typeparam>
        /// <param name="options">Lista de opções.</param>
        /// <param name="title">Título.</param>
        /// <param name="multipleWithRegex">Seleção múltipla com Regex</param>
        /// <returns>Resposta com índice e opção selecionada. Índice -1 para nenhuma seleção.</returns>
        private KeyValuePair<int, T>[] ChooseOptions<T>(IList<T> options, string title, bool multipleWithRegex)
        {
            IList<string> GetPossiblesToSelect(T option)
            {
                var values = new List<string>();
                if (typeof(T) != typeof(string) && option is IEnumerable enumerable)
                {
                    values.AddRange(
                        from object name in enumerable
                        select GetName(name).Translate());
                }
                else
                {
                    values.Add(GetName(option).Translate());
                }

                return values;
            }

            var optionsToShow = options;
            if (options.Count > 0 && options[0] is IModule)
            {
                options = options.OrderBy(a => ((IModule) a).Hidden).ToList();
                optionsToShow = options.Where(a => !((IModule) a).Hidden).ToList();
            }
            
            var optionsAsText = Options(optionsToShow);
            var result = new List<KeyValuePair<int, T>>();
            string answer;
            do
            {
                if (this == ModuleRoot)
                {
                    OnModuleRootChooseOptionEnter?.Invoke();
                }
                else
                {
                    OnModuleRootChooseOptionLeave?.Invoke();
                }
                
                Output.WriteLine(title.Translate());
                Output.WriteLine(optionsAsText);
                Output.Write($"?{(multipleWithRegex ? Phrases.ChooseMultiple : Phrases.ChooseOne).Translate()}");

                answer = Input.Read();

                if (!string.IsNullOrWhiteSpace(answer))
                {
                    if (Regex.IsMatch(answer, @"^[0-9]*$") && int.Parse(answer) > 0 &&
                        int.Parse(answer) <= options.Count)
                    {
                        result.Add(new KeyValuePair<int, T>(int.Parse(answer) - 1, options[int.Parse(answer) - 1]));
                    }
                    else if (multipleWithRegex)
                    {
                        if (answer == "*") answer = ".*";
                        for (var i = 0; i < options.Count; i++)
                        {
                            var possibles = GetPossiblesToSelect(options[i]);
                            possibles.Add((i + 1).ToString());
                            possibles.Add(GetName(options[i]).Translate(KnowCommandTranslate));

                            try
                            {
                                if (possibles.Any(possible => Regex.IsMatch(possible, answer, RegexOptions.IgnoreCase)))
                                {
                                    result.Add(new KeyValuePair<int, T>(i, options[i]));
                                }
                            }
                            catch
                            {
                                //Ignora em caso de erro na expressão regular.
                            }
                        }
                    }
                    else
                    {
                        var index = new List<int>();
                        var answerSlug = answer.Slug();
                        for (var i = 0; i < options.Count; i++)
                        {
                            if (string.Equals(GetName(options[i]).Translate(KnowCommandTranslate), answer,
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                index.Clear();
                                index.Add(i);
                                break;
                            }

                            if (GetPossiblesToSelect(options[i]).Count(a =>
                                    a.Slug().IndexOf(answerSlug, StringComparison.Ordinal) == 0) > 0)
                            {
                                index.Add(i);
                            }
                        }

                        if (index.Count == 1) result.Add(new KeyValuePair<int, T>(index[0], options[index[0]]));
                    }

                    switch (result.Count)
                    {
                        case 0:
                            Output.WriteLine($"@{answer.EscapeForOutput()}").WriteLine();
                            Output.WriteLine($"!{Phrases.ChooseWrong.Translate()}").WriteLine();
                            break;
                        case 1:
                            Output.WriteLine("@{1}) {0}", GetPossiblesToSelect(options[result[0].Key])[0].EscapeForOutput(), result[0].Key + 1).WriteLine();
                            break;
                        default:
                        {
                            foreach (var (optionId, option) in result)
                            {
                                Output.Write("@{1}) {0} ", GetPossiblesToSelect(option)[0].EscapeForOutput(), optionId + 1);
                            }
                            Output.WriteLine().WriteLine();
                            break;
                        }
                    }
                }
                else
                {
                    Output.WriteLine($"_{Phrases.ChooseBlank.Translate()}").WriteLine();
                }
            } while (!string.IsNullOrWhiteSpace(answer) && result.Count == 0);

            return result.ToArray();
        }

        /// <summary>
        ///     Recebe uma confirmação do usuário de Sim ou não.
        /// </summary>
        /// <param name="title">Título.</param>
        /// <param name="response">Resposta para confirmação.</param>
        public bool InputConfirm(string title = Phrases.ConfirmSelection, string response = "Ok")
        {
            Output.WriteLine($"?{title.Translate()}", response);
            Output.Write("?> ");

            var answer =  string.Equals(Input.Read(), response, StringComparison.CurrentCultureIgnoreCase);

            Output.WriteLine(answer
                ? $"@{Phrases.Confirmed.Translate()}"
                : $"_{Phrases.Canceled.Translate()}").WriteLine();

            return answer;
        }

        /// <summary>
        ///     Recebe um texto do usuário.
        /// </summary>
        /// <param name="title">Título.</param>
        public string InputText(string title)
        {
            Output.WriteLine($"?{title.Translate()}");
            Output.Write("?> ");

            var answer = Input.Read();

            Output.WriteLine(!string.IsNullOrWhiteSpace(answer)
                ? $"@{answer.EscapeForOutput()}"
                : $"_{Phrases.ChooseBlank.Translate()}").WriteLine();

            return answer;
        }

        /// <summary>
        ///     Exibe uma lista para seleção.
        /// </summary>
        /// <param name="options">Opções.</param>
        /// <param name="title">Título.</param>
        public void ChooseOption(IDictionary<string, Action> options, string title = Phrases.Operations)
        {
            do
            {
                var (key, value) = ChooseOption(options.Select(a => a.Key).ToArray(), title);
                if (key == -1) return;
                options[value]();
            } while (true);
        }

        /// <summary>
        ///     Exibe uma lista de módulos para seleção.
        /// </summary>
        /// <param name="context">Contexto dos módulos para exibição.</param>
        /// <param name="title">Título.</param>
        /// <param name="autoSelectForOneRootModule">Auto seleção houver apenas um módulo raiz (i.e., com contexto em branco).</param>
        public void ChooseModule(string context, string title = Phrases.Resources, bool autoSelectForOneRootModule = true)
        {
            var modules = AllModules
                .Where(a => a.Context == context)
                .OrderBy(a => a.ContextOrder)
                .ThenBy(a => a.Name)
                .ToList();

            var isOneRootModule = context == string.Empty && modules.Count == 1;
            if (autoSelectForOneRootModule && isOneRootModule)
            {
                ModuleRoot = AllModules.Single(a => a == modules[0] && a.Context == string.Empty);
                ModuleRoot.Run();
            }
            else
            {
                do
                {
                    var (key, value) = ChooseOption(modules, title);

                    if (key == -1) return;
                    AllModules.Single(a => a == value).Run();
                } while (true);
            }
        }

        /// <summary>
        ///     Inicia um loop.
        /// </summary>
        /// <param name="loop">Função do loop. Se retorna null o loop finaliza.</param>
        /// <param name="control">Argumentos passados e recebidos do loop.</param>
        /// <returns>Retorna true quando finaliza, e false quando o usuário cancela.</returns>
        protected bool Loop<T>(Func<T, T> loop, T control)
        {
            if (Output.LevelFilter <= OutputLevel.Interactive) Output.WriteLine($"_{Phrases.LoopControl.Translate()}").WriteLine();
            do
            {
                control = loop(control);

                if (control == null) continue;

                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (Output.LevelFilter <= OutputLevel.Interactive && Input.HasRead() ? Input.ReadKey() : (char) 0)
                {
                    case (char) ConsoleKey.Escape:
                        ConsoleLoading.Active(false);
                        if (Output.LevelFilter <= OutputLevel.Interactive) Output.WriteLine().WriteLine($"_{Phrases.LoopCanceled.Translate()}");
                        return false;
                    case 'p':
                    case 'P':
                        ConsoleLoading.Active(false);
                        if (Output.LevelFilter <= OutputLevel.Interactive) Output.WriteLine().WriteLine($"_{Phrases.LoopPaused.Translate()}").WriteLine();
                        Input.ReadKey();
                        break;
                }
            } while (control != null);

            return true;
        }
    }
}