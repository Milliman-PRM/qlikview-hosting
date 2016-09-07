using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SystemReporting.Utilities
{
    public static class ExtensionMethods
    {
        public static string ToPropertyName(this string original)
        {
            // Need to insert a '_' at the beginning, in case name starts with a number!
            var name = original.Trim().Replace(" ", "").Insert(0, "_");
            name = name.Replace("+", "");
            for (int i = 0; i < name.Length; i++)
            {
                var letter = name[i];
                if (!char.IsLetterOrDigit(letter))
                    name = name.Replace(letter, '_');
            }
            return name;

        }

        public static string ToMD5(this string original)
        {
            if (original != null)
            {
                var MD5ByteArray = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(original));
                var hashBuilder = new StringBuilder();
                foreach (var byght in MD5ByteArray)
                    hashBuilder.Append(byght.ToString("x2").ToLower());

                return hashBuilder.ToString();
            }
            else
                throw new ArgumentNullException("Cannot generate an MD5 hash of the given string because the given string was null.");
        }       

        /// <summary>
        /// Method to convert the list items to datatabale
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public static DataTable ConvertToDataTable<T>(List<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            var Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }               
        
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            DataTable table = null;
            if (list != null && list.Count > 0)
            {
                var properties = TypeDescriptor.GetProperties(typeof(T));

                var propList = new List<PropertyDescriptor[]>();

                table = new DataTable();
                foreach (PropertyDescriptor item in properties)
                {
                    AddColumnsForProperties(typeof(T).Namespace, table, (new[] { item }).ToList(), ref propList);
                }

                object[] values = new object[propList.Count];

                foreach (T item in list)
                {
                    for (int i = 0; i < values.Length; i++)
                        values[i] = GetValueFromProps(propList[i], item) ?? DBNull.Value;

                    table.Rows.Add(values);
                }
            }
            return table;
        }

        private static DataTable AddColumnsForProperties(string myNamespace, DataTable dt, List<PropertyDescriptor> p, ref List<PropertyDescriptor[]> properties)
        {
            var pLast = p.Last();

            if (pLast.PropertyType.Namespace.StartsWith(myNamespace))
            {
                var allProperties = pLast.GetChildProperties();
                foreach (PropertyDescriptor item in allProperties)
                {
                    var newP = p.ToList();
                    newP.Add(item);

                    if (item.PropertyType.Namespace.ToLower().StartsWith(myNamespace))
                        AddColumnsForProperties(myNamespace, dt, newP, ref properties);
                    else
                        if (!dt.Columns.Contains(item.Name))
                    {
                        dt.Columns.Add(item.Name, Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType);
                        properties.Add(newP.ToArray());
                    }
                }
            }
            else
                if (!dt.Columns.Contains(pLast.Name))
            {
                dt.Columns.Add(pLast.Name, Nullable.GetUnderlyingType(pLast.PropertyType) ?? pLast.PropertyType);
                properties.Add(p.ToArray());
            }
            return dt;
        }
        static object GetValueFromProps(PropertyDescriptor[] descriptors, object item)
        {
            var result = item;
            foreach (var descriptor in descriptors)
            {
                result = descriptor.GetValue(result);
            }
            return result;
        }

        public static string ExportDatatableToHtml(DataTable dt)
        {
            var strHTMLBuilder = new StringBuilder();
            strHTMLBuilder.Append("<html >");
            strHTMLBuilder.Append("<head>");
            strHTMLBuilder.Append("</head>");
            strHTMLBuilder.Append("<body>");
            strHTMLBuilder.Append("<table border='1px' cellpadding='1' cellspacing='1' bgcolor='lightyellow' style='font-family:Garamond; font-size:small'>");

            //Header Row
            strHTMLBuilder.Append("<tr>");
            foreach (DataColumn myColumn in dt.Columns)
            {
                strHTMLBuilder.Append("<td>");
                strHTMLBuilder.Append("<b>");
                strHTMLBuilder.Append(myColumn.ColumnName);
                strHTMLBuilder.Append("</b>");
                strHTMLBuilder.Append("</td>");
            }
            strHTMLBuilder.Append("</tr>");

            //Details Row
            foreach (DataRow myRow in dt.Rows)
            {
                strHTMLBuilder.Append("<tr >");
                foreach (DataColumn myColumn in dt.Columns)
                {
                    strHTMLBuilder.Append("<td >");
                    strHTMLBuilder.Append(myRow[myColumn.ColumnName].ToString());
                    strHTMLBuilder.Append("</td>");
                }
                strHTMLBuilder.Append("</tr>");
            }

            //Close tags.  
            strHTMLBuilder.Append("</table>");
            strHTMLBuilder.Append("</body>");
            strHTMLBuilder.Append("</html>");
            string htmlTableText = strHTMLBuilder.ToString();
            return htmlTableText;
        }
    }
}
