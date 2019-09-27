// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Cli.General.Cryptography
{
    /// <summary>
    ///     <para>
    ///         Disponibiliza funcionalidades relacionadas a criptografia de dados
    ///         com algoritmos de criptografia simétrica.
    ///     </para>
    /// </summary>
    /// <typeparam name="TSymmetricAlgorithmProvider">Tipo do algoritimo de criptografia simétrica.</typeparam>
    public class CryptographySymmetric<TSymmetricAlgorithmProvider>
        where TSymmetricAlgorithmProvider : SymmetricAlgorithm
    {
        /// <summary>
        ///     <para>Construtor padrão.</para>
        /// </summary>
        public CryptographySymmetric() : this(string.Empty, new byte[] { })
        {
        }

        /// <summary>
        ///     <para>Construtor padrão.</para>
        /// </summary>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        ///     <para>Bytes usados na derivação da chave de criptografia que foi informada pelo usuário.</para>
        /// </param>
        public CryptographySymmetric(string key, byte[] bytesSalt)
            : this(
                key,
                bytesSalt,
                new AlgorithmInfoCollection()[typeof(TSymmetricAlgorithmProvider)].BytesKey,
                new AlgorithmInfoCollection()[typeof(TSymmetricAlgorithmProvider)].BytesIv)
        {
        }

        /// <summary>
        ///     <para>Construtor padrão.</para>
        ///     <para>Informa o comprimento de bytes usado na criptografia.</para>
        /// </summary>
        /// <param name="bytesKey">
        ///     <para>Total de bytes usados na palavra chave (Key) do algoritmo de criptografia.</para>
        /// </param>
        /// <param name="bytesIv">
        ///     <para>Total de bytes usados no vetor de inicialização (IV) do algoritmo de criptografia.</para>
        /// </param>
        public CryptographySymmetric(int bytesKey, int bytesIv)
            : this(string.Empty, new byte[] { }, bytesKey, bytesIv)
        {
        }

        /// <summary>
        ///     <para>Construtor padrão.</para>
        ///     <para>Informa o comprimento de bytes usado na criptografia.</para>
        /// </summary>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        ///     <para>Bytes usados na derivação da chave de criptografia que foi informada pelo usuário.</para>
        /// </param>
        /// <param name="bytesKey">
        ///     <para>Total de bytes usados na palavra chave (Key) do algoritmo de criptografia.</para>
        /// </param>
        /// <param name="bytesIv">
        ///     <para>Total de bytes usados no vetor de inicialização (IV) do algoritmo de criptografia.</para>
        /// </param>
        public CryptographySymmetric(string key, byte[] bytesSalt, int bytesKey, int bytesIv)
        {
            Key = key;
            BytesSalt = bytesSalt;
            BytesKey = bytesKey;
            BytesIv = bytesIv;
        }

        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Armazena o total de bytes usado na palavra chave (Key).</para>
        /// </summary>
        public int BytesKey { get; }

        /// <summary>
        ///     <para>(Leitura)</para>
        ///     <para>Armazena o total de bytes usado no vetor de inicialização (IV).</para>
        /// </summary>
        public int BytesIv { get; }

        /// <summary>
        ///     <para>(Leitura/Escrita></para>
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        ///     <para>(Leitura/Escrita></para>
        ///     <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </summary>
        public byte[] BytesSalt { get; set; }

        /// <summary>
        ///     <para>Criptografa ou reverte uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <param name="text">
        ///     <para>Texto de entrada.</para>
        /// </param>
        /// <returns>
        ///     <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text)
        {
            return Apply(toCryptography, text, Key, BytesSalt);
        }

        /// <summary>
        ///     <para>Criptografa ou reverte uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <param name="text">
        ///     <para>Texto de entrada.</para>
        /// </param>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <returns>
        ///     <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text, string key)
        {
            return Apply(toCryptography, text, key, BytesSalt);
        }

        /// <summary>
        ///     <para>Criptografa ou reverte uma sequencia de texto.</para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <param name="text">
        ///     <para>Texto de entrada.</para>
        /// </param>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        ///     <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </param>
        /// <returns>
        ///     <para>Resulta no mesmo texto de entrada, porém, criptografado.</para>
        /// </returns>
        public string Apply(bool toCryptography, string text, string key, byte[] bytesSalt)
        {
            using (var ms = new MemoryStream())
            {
                var cryptoTransform = GetCryptoTransform(toCryptography, key, bytesSalt);
                var cryptoStream = new CryptoStream(ms, cryptoTransform, CryptoStreamMode.Write);

                if (toCryptography)
                {
                    cryptoStream.Write(Encoding.Default.GetBytes(text), 0, text.Length);
                }
                else
                {
                    var arrayTexto = Convert.FromBase64String(text);
                    cryptoStream.Write(arrayTexto, 0, arrayTexto.Length);
                }

                cryptoStream.FlushFinalBlock();

                return toCryptography ? Convert.ToBase64String(ms.ToArray()) : Encoding.Default.GetString(ms.ToArray());
            }
        }

        /// <summary>
        ///     <para>
        ///         Obtem uma instância de uma classe devidamente configurada
        ///         para realizar a des/criptografia.
        ///     </para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <returns>
        ///     <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform" />.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography)
        {
            return GetCryptoTransform(toCryptography, Key, BytesSalt);
        }

        /// <summary>
        ///     <para>
        ///         Obtem uma instância de uma classe devidamente configurada
        ///         para realizar a des/criptografia.
        ///     </para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <returns>
        ///     <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform" />.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography, string key)
        {
            return GetCryptoTransform(toCryptography, key, BytesSalt);
        }

        /// <summary>
        ///     <para>
        ///         Obtem uma instância de uma classe devidamente configurada
        ///         para realizar a des/criptografia.
        ///     </para>
        /// </summary>
        /// <param name="toCryptography">
        ///     <para>
        ///         Quando igual a <c>true</c>, define o processo como Criptografia.
        ///         Mas se for igual a <c>false</c>, define como reversão da criptografia.
        ///     </para>
        /// </param>
        /// <param name="key">
        ///     <para>Senha, ou chave de criptografia, usada para des/criptografar.</para>
        /// </param>
        /// <param name="bytesSalt">
        ///     <para>Bytes usados na derivação da chave de criptografia.</para>
        /// </param>
        /// <returns>
        ///     <para>Retorna uma classe que implementa a interface <see cref="ICryptoTransform" />.</para>
        /// </returns>
        public ICryptoTransform GetCryptoTransform(bool toCryptography, string key, byte[] bytesSalt)
        {
            var pdb = new Rfc2898DeriveBytes(key, bytesSalt);

            var method = typeof(TSymmetricAlgorithmProvider).GetMethod("Create", new Type[] { });
            var algoritmo =
                (TSymmetricAlgorithmProvider) method?.Invoke(typeof(TSymmetricAlgorithmProvider), new object[] { }) ??
                throw new NullReferenceException();

            algoritmo.Key = pdb.GetBytes(BytesKey);
            algoritmo.IV = pdb.GetBytes(BytesIv);

            return toCryptography ? algoritmo.CreateEncryptor() : algoritmo.CreateDecryptor();
        }
    }
}