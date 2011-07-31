using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Web;
using WPFLib.Updates;
using System.Net;

namespace WPFLib.Database
{
    public static class DbCommonUtils
    {
        public static IDbDataParameter AddParameter(this IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
            return parameter;
        }

        public static IDbDataParameter AddParameter(this IDbCommand command, string name, object value, DbType type)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            parameter.DbType = type;
            command.Parameters.Add(parameter);
            return parameter;
        }

        /// <summary>
        /// Для коллекции дает строчку, которую можно подставить в sql-запросы в конструкцию IN
        /// </summary>
        public static string CreateXmlParam<T>(IDbCommand command, string paramName, string dbNativeType, IEnumerable<T> valueList)
        {
            string xmlVal = BuildItemsXml(valueList);

            command.AddParameter(paramName, xmlVal, DbType.Xml);

            return string.Format("SELECT ParamValues.Item.value('.','{0}') FROM {1}.nodes('/ROOT/Item') as ParamValues(Item)", dbNativeType, paramName);

        }

        /// <summary>
        /// По списку значений создает xml-строчку для подстановки в запросы
        /// </summary>
        public static string BuildItemsXml<T>(IEnumerable<T> valueList)
        {
            StringBuilder sb = new StringBuilder("<ROOT>");
            foreach (T item in valueList)
            {
                var param = WebUtility.HtmlEncode(item.ToString());
                sb.Append("<Item>").Append(param).Append("</Item>");
            }
            sb.Append("</ROOT>");
            return sb.ToString();
        }

        public static ParameterValue BuildItemsXmlParam<T>(IEnumerable<T> valueList)
        {
            return new ParameterValue
            {
                Value = BuildItemsXml(valueList),
                Type = DbType.Xml
            };
        }
    }
}
