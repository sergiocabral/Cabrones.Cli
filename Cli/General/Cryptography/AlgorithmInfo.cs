namespace Cli.General.Cryptography
{
    /// <summary>
    ///     <para>Agrupa informações sobre um algoritmo de criptografia.</para>
    /// </summary>
    public struct AlgorithmInfo
    {
        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Nome que representa o algoritmo de criptografia.</para>
        /// </summary>
        public string Name { get; }

        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Quantidade de bytes usado na chave de criptografia.</para>
        /// </summary>
        public int BytesKey { get; }

        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Quantidade de bytes usado no Vetor de Inicialização (IV).</para>
        /// </summary>
        public int BytesIv { get; }

        /// <summary>
        ///     <para>Construtor.</para>
        /// </summary>
        /// <param name="name">
        ///     <para>
        ///         Nome que representa o algoritmo
        ///         de criptografia.
        ///     </para>
        /// </param>
        /// <param name="bytesKey">
        ///     <para>
        ///         Quantidade de bytes usado na
        ///         chave de criptografia.
        ///     </para>
        /// </param>
        /// <param name="bytesIv">
        ///     <para>
        ///         Quantidade de bytes usado no
        ///         Vetor de Inicialização (IV).
        ///     </para>
        /// </param>
        public AlgorithmInfo(string name, int bytesKey, int bytesIv)
        {
            Name = name;
            BytesKey = bytesKey;
            BytesIv = bytesIv;
        }
    }
}