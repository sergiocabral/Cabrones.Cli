// ReSharper disable UnusedMember.Global

using System;
using System.ComponentModel;

namespace Cli.General.Data
{
    /// <summary>
    ///     Extensão de métodos para enum
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        ///     Obter descrição amigável.
        /// </summary>
        /// <param name="genericEnum">Valor do enum</param>
        /// <returns>Descrição</returns>
        public static string GetDescription(this Enum genericEnum)
        {
            var type = genericEnum.GetType();
            var memberInfo = type.GetMember(genericEnum.ToString());

            if (memberInfo.Length <= 0) return genericEnum.ToString();

            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0
                ? ((DescriptionAttribute) attributes.GetValue(0)).Description
                : genericEnum.ToString();
        }
    }
}