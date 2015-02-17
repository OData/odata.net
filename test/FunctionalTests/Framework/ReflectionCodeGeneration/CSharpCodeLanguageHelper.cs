//---------------------------------------------------------------------
// <copyright file="CSharpCodeLanguageHelper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace System.Data.Test.Astoria.ReflectionProvider
{
    public class CSharpCodeLanguageHelper : IDisposable
    {
        private StreamWriter _writer;

        protected internal CSharpCodeLanguageHelper(StreamWriter writer)
        {
            _writer = writer;
        }

        protected StreamWriter CodeWriter
        {
            get { return _writer; }
        }

        public void WriteUsing(string usingNamespace)
        {
            this.CodeWriter.WriteLine("using " + usingNamespace + ";");
        }

        public void WriteLine(string line)
        {
            this.CodeWriter.WriteLine(line);
        }

        public void WriteLine()
        {
            WriteLine(string.Empty);
        }

        public void WriteLines(IEnumerable<string> lines)
        {
            foreach (string line in lines)
                WriteLine(line);
        }

        public void WriteLines(params string[] lines)
        {
            WriteLines(lines.AsEnumerable());
        }

        public void CreateFieldBackedProperty(string fieldName, string propertyName, string propertyType)
        {
            CreateFieldBackedProperty(fieldName, propertyName, propertyType, null);
        }

        public void CreateFieldBackedProperty(string fieldName, string propertyName, string propertyType, string initialValue)
        {
            if(initialValue != null)
                initialValue = " = " + initialValue;

            this.WriteLines(
                "           internal " + propertyType + " " + fieldName + initialValue + ";",
                "           public " + propertyType + " " + propertyName,
                "           {",
                "               get",
                "               {",
                "                   return this." + fieldName + ";",
                "               }",
                "               set",
                "               {",
                "                   this." + fieldName + " = value;",
                "               }",
                "           }"
            );
        }

        public void CreateFieldBackedProperty(string typeName, string propertyName, bool instantiate)
        {
            CreateFieldBackedProperty("_" + propertyName, propertyName, typeName, (instantiate ? "new " + typeName + "()" : null));
        }

        public void CreateIQuerableGetProperty(ResourceContainer container)
        {
            string baseType = container.BaseType.Name;
            this.WriteLines(
                "           public IQueryable<" + baseType + "> " + container.Name,
                "           {",
                "               get",
                "               {",
                "                   return EntitySetDictionary.GetEntitySet<" + baseType + ">();",
                "               }",
                "           }"
            );
        }

        public void WriteStartNamespace(string typeNamespace)
        {
            this.CodeWriter.WriteLine(String.Format("namespace {0}", typeNamespace));
            this.CodeWriter.WriteLine("{");
        }

        public void WriteEndNamespace(string typeNamespace)
        {
            this.CodeWriter.WriteLine("}");
            this.CodeWriter.WriteLine("// end " + typeNamespace);
        }


        public void WriteBeginClass(string className, string baseClassName, string interfaceName, bool isAbstract)
        {
            this.CodeWriter.Write("\tpublic ");
            if (isAbstract)
                this.CodeWriter.Write("abstract ");

            this.CodeWriter.Write(String.Format(" partial class {0}", className));

            if (baseClassName != null)
                this.CodeWriter.Write(String.Format(":{0}", baseClassName));
            if (interfaceName != null && baseClassName == null)
                this.CodeWriter.Write(String.Format(":{0}", interfaceName));
            else if (interfaceName != null && baseClassName != null)
                this.CodeWriter.Write(String.Format(",{0}", interfaceName));
            this.CodeWriter.WriteLine("");
            this.CodeWriter.WriteLine("\t{");
        }

        public void WriteEndClass(string className)
        {
            this.CodeWriter.WriteLine("    }");
            this.CodeWriter.WriteLine("    // end " + className);
        }

        public void WriteCommentLine(string comment)
        {
            this.CodeWriter.WriteLine("//" + comment);
        }

        public string CreateListOfTType(string itemType)
        {
            return "List<" + itemType + ">";
        }

        public void WriteDataKeyAttribute(IEnumerable<ResourceProperty> keys)
        {
            //[DataServiceKey("GameID")]
            string keysStr = "";
            foreach (ResourceProperty key in keys)
            {
                if (keysStr.Length == 0)
                    keysStr = "\"" + key.Name + "\"";
                else
                    keysStr = keysStr + ",\"" + key.Name + "\"";
            }
            this.CodeWriter.WriteLine(String.Format("    [DataServiceKey({0})]", keysStr));
        }

        public void CreateNavigationMapMethod(ResourceType resourceType)
        {

            //Create pattern below
            //internal string GetOtherSideNavigationProperty(string currentSide)
            /*{
                if (currentSide.Equals("Run"))
                {
                    return "Tasks";
                }
                else if (currentSide.Equals("Foo"))
                    return "bar";
                else
                    return null;
            }*/
            if (resourceType.BaseType == null)
                this.CodeWriter.WriteLine(String.Format("\t\t\tprotected internal virtual string GetOtherSideNavigationProperty(string currentSide)"));
            else
                this.CodeWriter.WriteLine(String.Format("\t\t\tprotected internal override string GetOtherSideNavigationProperty(string currentSide)"));
            this.CodeWriter.WriteLine("\t\t\t{");
            int i = 0;
            foreach (ResourceProperty rp in resourceType.Properties.OfType<ResourceProperty>().Where(rp => rp.IsNavigation == true).OrderBy(rp2 => rp2.Name))
            {
                ResourceProperty otherRp = rp.OtherSideNavigationProperty;
                if (i == 0)
                {
                    this.CodeWriter.WriteLine(String.Format("\t\t\t\tif (currentSide.Equals(\"{0}\"))", rp.Name));
                    if (otherRp != null)
                        this.CodeWriter.WriteLine(String.Format("\t\t\t\t\treturn \"{0}\";", otherRp.Name));
                    else
                        this.CodeWriter.WriteLine(String.Format("\t\t\t\t\treturn null;"));
                }
                else if (i > 0)
                {
                    this.CodeWriter.WriteLine(String.Format("\t\t\t\telse if (currentSide.Equals(\"{0}\"))", rp.Name));
                    if (otherRp != null)
                        this.CodeWriter.WriteLine(String.Format("\t\t\t\t\treturn \"{0}\";", otherRp.Name));
                    else
                        this.CodeWriter.WriteLine(String.Format("\t\t\t\t\treturn null;"));
                }
                i++;

            }

            this.CodeWriter.WriteLine(String.Format("\t\t\t\treturn null;"));
            this.CodeWriter.WriteLine("\t\t\t}");
        }

        public void WriteAttribute(string name, IDictionary<string, string> namedValues, params string[] orderedValues)
        {
            string ordered = null;
            string named = null;
            string parameters = null;

            if (orderedValues.Length > 0)
                ordered = String.Join(",", orderedValues);

            if (namedValues != null && namedValues.Count > 0)
                named = String.Join(",", namedValues.Select(pair => pair.Key + "=" + pair.Value).ToArray());

            if (named != null && ordered != null)
                parameters = ordered + "," + named;
            else if (named != null)
                parameters = named;
            else if (ordered != null)
                parameters = ordered;

            this.CodeWriter.WriteLine("\t[{0}({1})]", name, parameters);
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Flush();
                _writer.Close();
            }
        }

        #endregion
    }
}
