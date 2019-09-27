using System;
using System.Collections.Generic;
using System.Linq;

namespace Cli.Business.IO
{
    /// <summary>
    ///     Gerenciador de outputs
    /// </summary>
    public sealed class OutputManager : ManagerBase<IOutput>, IOutput
    {
        /// <summary>
        ///     Nível usado para exibir output.
        /// </summary>
        private OutputLevel _levelToShow = OutputLevel.Interactive;
        
        /// <summary>
        ///     Nível usado para exibir output.
        /// </summary>
        public OutputLevel LevelFilter {
            get
            {
                if (Items.Any(item => _levelToShow != item.LevelFilter)) throw new InvalidOperationException();
                return _levelToShow;
            }
            set
            {
                _levelToShow = value;
                foreach (var item in Items) item.LevelFilter = _levelToShow;
            } 
        }
        
        /// <summary>
        ///     Nível atual para escrita das mensagens.
        /// </summary>
        private OutputLevel _level = OutputLevel.Interactive;
        
        /// <summary>
        ///     Nível atual para escrita das mensagens.
        /// </summary>
        public OutputLevel Level {
            get
            {
                if (Items.Any(item => _level != item.Level)) throw new InvalidOperationException();
                return _level;
            }
            set
            {
                _level = value;
                foreach (var item in Items) item.Level = _level;
            } 
        }
        
        /// <summary>
        ///     Adiciona um item gerenciado.
        /// </summary>
        /// <param name="item">Item gerenciado.</param>
        public override void Add(IOutput item)
        {
            item.Level = _level;
            item.LevelFilter = _levelToShow;
            base.Add(item);
        }

        /// <summary>
        /// Estrutura para representar um texto que deve ser exibido.
        /// </summary>
        private struct TextInfo
        {
            /// <summary>
            /// Nível do texto.
            /// </summary>
            public readonly OutputLevel Level;
            
            /// <summary>
            /// Texto.
            /// </summary>
            public readonly string Text;
            
            /// <summary>
            /// Argumentos para a formatação do texto.
            /// </summary>
            public readonly object[] Args;

            /// <summary>
            /// Construtor.
            /// </summary>
            /// <param name="level">Nível do texto.</param>
            /// <param name="text">Texto.</param>
            /// <param name="args">Argumentos para a formatação do texto.</param>
            public TextInfo(OutputLevel level, string text, object[] args)
            {
                Level = level;
                Text = text;
                Args = args;
            }
        }
        
        /// <summary>
        ///     Fila de mensagens não exibidas por não haver output.
        /// </summary>
        private IList<TextInfo> Queue { get; } = new List<TextInfo>();

        /// <summary>
        ///     Evita o output.
        /// </summary>
        public bool Prevent { get; set; }

        /// <summary>
        ///     Sinaliza que não há itens para enviar (flush)
        /// </summary>
        public bool Flushed { get; private set; }

        /// <summary>
        ///     Escreve um texto formatado.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public IOutput Write(string format, params object[] args)
        {
            return Write(new TextInfo(Level, format, args));
        }

        /// <summary>
        ///     Escreve um texto formatado e adiciona nova uma linha.
        /// </summary>
        /// <param name="format">Formato</param>
        /// <param name="args">Argumentos.</param>
        /// <returns>Auto referência.</returns>
        public IOutput WriteLine(string format = "", params object[] args)
        {
            return Write($"{format}\n", args);
        }

        /// <summary>
        ///     Escreve um texto formatado.
        /// </summary>
        /// <param name="textInfo">Informações do texto.</param>
        /// <param name="force">Força a escrita. Ignora o Flushed</param>
        /// <returns>Auto referência.</returns>
        private IOutput Write(TextInfo textInfo, bool force = false)
        {
            if (!force && (Items.Count == 0 || Prevent))
            {
                Queue.Add(textInfo);
                Flushed = false;
            }
            else
            {
                foreach (var item in Items) item.Write(textInfo.Text, textInfo.Args);
            }

            return this;
        }

        /// <summary>
        ///     Solicita a escrita dos itens da fila de mensagens não exibidas.
        /// </summary>
        public void QueueFlush()
        {
            var level = Level;
            do
            {
                var count = Queue.Count;
                for (var i = 0; i < count; i++)
                {
                    Level = Queue[0].Level;
                    Write(Queue[0], true);
                    Level = level;
                    Queue.RemoveAt(0);
                }
            } while (Queue.Count > 0);

            Flushed = true;
        }
    }
}