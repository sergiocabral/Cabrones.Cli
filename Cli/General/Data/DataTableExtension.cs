// ReSharper disable UnusedMember.Global

using System.Data;

namespace Cli.General.Data
{
    /// <summary>
    ///     Extensão de métodos para DataTable
    /// </summary>
    public static class DataTableExtension
    {
        /// <summary>
        ///     Localiza o índice de uma coluna por pesquisar o conteúdo.
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="search">Conteúdo a pesquisar</param>
        /// <returns>Índice ou -1 para não encontrado.</returns>
        public static int FindColumn(this DataTable dataTable, string search)
        {
            for (var i = 0; i < dataTable.Columns.Count; i++)
                if (dataTable.Columns[i].ColumnName.ToLower().Contains(search.ToLower()))
                    return i;
            return -1;
        }
    }
}