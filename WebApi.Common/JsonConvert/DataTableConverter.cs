using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Common.JsonConvert
{
    public class DataTableConverter : JsonConverter<DataTable>
    {
        public override DataTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, DataTable value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (DataRow row in value.Rows)
            {
                writer.WriteStartObject();
                foreach (DataColumn col in value.Columns)
                {
                    if (row[col.ColumnName] is DBNull || row[col.ColumnName] == null)
                    {
                        writer.WriteNull(col.ColumnName);
                        continue;
                    }

                    if (col.DataType == typeof(DateTime)) //处理日期型
                    {
                        var v = (DateTime)row[col.ColumnName];
                        writer.WriteString(col.ColumnName, v.ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (col.DataType == typeof(bool))//处理布尔型
                    {
                        writer.WriteBoolean(col.ColumnName, (bool)row[col.ColumnName]);
                    }
                    else //当字符串
                    {
                        writer.WriteString(col.ColumnName, row[col.ColumnName].ToString());
                    }
                }
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}
