using System.Collections.Specialized;
using System.Text;

namespace Cli.General.Web
{
    /// <summary>
    ///     Extensão para: NameValueCollection
    /// </summary>
    public static class NameValueCollectionExtension
    {
        /// <summary>
        ///     Retorna o conteúdo como query string.
        /// </summary>
        public static string GetQuerystring(this NameValueCollection values)
        {
            var result = new StringBuilder();
            foreach (string key in values)
                if (values[key] != null && values[key].Contains(","))
                {
                    foreach (var value in values[key].Split(','))
                    {
                        if (result.Length > 0) result.Append("&");
                        result.Append($"{key}={value}");
                    }
                }
                else
                {
                    if (result.Length > 0) result.Append("&");
                    result.Append($"{key}={values[key]}");
                }

            return result.ToString();
        }
    }
}