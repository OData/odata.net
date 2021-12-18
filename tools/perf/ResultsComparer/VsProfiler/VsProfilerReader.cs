//---------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using ResultsComparer.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ResultsComparer.VsProfiler
{
    public class VsProfilerReader<T>: IReader<T> where T : new()
    {
        private TextReader reader;
        private string[] columns;
        private int rowNum = 0;
        private bool disposedValue;

        public VsProfilerReader(TextReader reader)
        {
            this.reader = reader;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        public bool ReadNext(out T value)
        {
            value = default(T);
            rowNum++;

            if (rowNum == 1)
            {
                if (!ReadHeaderRow())
                {
                    return false;
                }

                rowNum++;
            }

            if (!ReadRowValue(out value))
            {
                return false;
            }

            rowNum++;
            return true;
        }

        private IEnumerable<T> Enumerate()
        {
            while (ReadNext(out T value))
            {
                yield return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    reader.Dispose();
                }

                reader = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private bool ReadLine(out string line)
        {
            line = reader.ReadLine();
            return line != null;
        }

        private bool ReadHeaderRow()
        {
            if (!ReadLine(out string row))
            {
                return false;
            }

            ExtractColumns(row);
            return true;
        }

        private bool ReadRowValue(out T value)
        {
            value = default(T);

            if (!ReadLine(out string row))
            {
                return false;
            }

            string[] values = ExtractRowValues(row);
            value = ConvertRow(values);

            return true;
        }

        private void ExtractColumns(string row)
        {
            this.columns = row.Split("\t");
        }

        private string[] ExtractRowValues(string row)
        {
            string[] values = row.Split("\t");
            // the first column of data has characters used to represent the indentation of the
            // expanded tree nodes of the UI. Example:
            // ||||||| + System.Threading.Tasks.Task.RunContinuations(object)	1,526,055	2	112	system.private.corelib.il
            // ||||||||||||| -Microsoft.OData.ODataWriterCore.InterceptException<T>(System.Action<Microsoft.OData.ODataWriterCore, T>, T) 514,369 33  9,132   microsoft.odata.core
            // Let's clean any such columns

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = Regex.Replace(values[i], @"[\|\+\-]+", "").Trim();
            }

            return values;
        }

        private T ConvertRow(string[] values)
        {
            T obj = new();
            Type type = typeof(T);
            PropertyInfo[] properties = type.GetProperties();

            foreach (var property in properties)
            {
                var attribute = property.GetCustomAttributes<FieldNameAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    string fieldName = attribute.Name;
                    int colIndex = FindColumn(fieldName);

                    if (colIndex == -1)
                    {
                        throw new Exception($"Unable to find column '{fieldName}'");
                    }

                    string rawValue = values[colIndex];
                    object targetValue;
                    if (property.PropertyType == typeof(int))
                    {
                        targetValue = int.Parse(rawValue, NumberStyles.AllowThousands);
                    }
                    else if (property.PropertyType == typeof(long))
                    {
                        targetValue = long.Parse(rawValue, NumberStyles.AllowThousands);
                    }
                    else if (property.PropertyType == typeof(decimal))
                    {
                        targetValue = decimal.Parse(rawValue, NumberStyles.AllowThousands|NumberStyles.AllowDecimalPoint);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        targetValue = double.Parse(rawValue, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
                    }
                    else if (property.PropertyType == typeof(float))
                    {
                        targetValue = float.Parse(rawValue, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
                    }
                    else
                    {
                        targetValue = rawValue;
                    }

                    property.SetValue(obj, targetValue);
                }
            }

            return obj;
        }

        private int FindColumn(string name)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
