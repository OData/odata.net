//---------------------------------------------------------------------
// <copyright file="CSharpDatabaseSchemaSerializer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.DatabaseModel
{
    using System.CodeDom.Compiler;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.DatabaseModel;

    /// <summary>
    /// Serializes DatabaseSchema to C# Code
    /// </summary>
    public class CSharpDatabaseSchemaSerializer : IDatabaseSchemaSerializer
    {
        /// <summary>
        /// Initializes a new instance of the CSharpDatabaseSchemaSerializer class to write to a given TextWriter
        /// </summary>
        /// <param name="writer">The TextWriter to write C# code to</param>
        /// <param name="typeConstructorGenerator">Code generator to use for type creation</param>
        /// <param name="specification">Level of specification to be used constructing SchemaObjects in the generated code</param>
        public CSharpDatabaseSchemaSerializer(SafeIndentedTextWriter writer, IDataTypeConstructorCodeGenerator typeConstructorGenerator, SchemaObjectSpecificationLevel specification)
        {
            this.Writer = writer;
            this.TypeConstructorGenerator = typeConstructorGenerator;
            this.SpecificationLevel = specification;
        }

        /// <summary>
        /// Gets the TextWriter that code is to be written to
        /// </summary>
        public SafeIndentedTextWriter Writer { get; private set; }

        /// <summary>
        /// Gets the IDataTypeConstructorGenerator used to generate type construction code
        /// </summary>
        public IDataTypeConstructorCodeGenerator TypeConstructorGenerator { get; private set; }

        /// <summary>
        /// Gets the level of specification to be used constructing SchemaObjects in the generated code
        /// </summary>
        public SchemaObjectSpecificationLevel SpecificationLevel { get; private set; }

        /// <summary>
        /// Serializes a DatabaseSchema graph to C# code that will re-construct the graph
        /// </summary>
        /// <param name="schema">Schema to be serialized</param>
        public void Serialize(DatabaseSchema schema)
        {
            IEnumerable<string> requiredNamespaces = new List<string>()
            {
                typeof(DatabaseSchema).Namespace,
            };
           requiredNamespaces = requiredNamespaces.Union(this.TypeConstructorGenerator.GetCodeNamespaces());

            this.Writer.WriteLine("namespace MyNamespace");
            this.Writer.WriteLine("{");
            this.Writer.Indent++;

            foreach (string ns in requiredNamespaces)
            {
                this.Writer.WriteLine("using {0};", ns);
            }

            this.Writer.WriteLine();

            this.Writer.WriteLine("public class Test");
            this.Writer.WriteLine("{");
            this.Writer.Indent++;

            this.Writer.WriteLine("public DatabaseSchema BuildSchema()");
            this.Writer.WriteLine("{");
            this.Writer.Indent++;

            this.Writer.WriteLine("DatabaseSchema schema = new DatabaseSchema()");
            this.Writer.WriteLine("{");
            this.Writer.Indent++;

            foreach (var table in schema.Tables.Concat(schema.Views))
            {
                this.Writer.WriteLine("new {0}({1})", table.GetType().Name, BuildSchemaObjectCSV(table, this.SpecificationLevel));
                this.Writer.WriteLine("{");
                this.Writer.Indent++;

                foreach (var column in table.Columns)
                {
                    this.Writer.WriteLine("new Column(\"{0}\", {1}),", column.Name, this.TypeConstructorGenerator.GetDataTypeConstructorCode(column.ColumnType));
                }

                if (table.PrimaryKey != null)
                {
                    this.Writer.WriteLine();
                    this.Writer.WriteLine("new PrimaryKey({0})", BuildSchemaObjectCSV(table.PrimaryKey, this.SpecificationLevel));
                    this.Writer.Indent++;
                    this.Writer.WriteLine(".WithKeyColumns({0}),", BuildColumnNameCSV(table.PrimaryKey.Columns));
                    this.Writer.Indent--;
                }

                if (table.UniqueConstraints.Count > 0)
                {
                    this.Writer.WriteLine();
                    foreach (var constraint in table.UniqueConstraints)
                    {
                        this.Writer.WriteLine("new UniqueConstraint({0})", BuildSchemaObjectCSV(constraint, this.SpecificationLevel));
                        this.Writer.Indent++;
                        this.Writer.WriteLine(".WithUniqueColumns({0}),", BuildColumnNameCSV(constraint.Columns));
                        this.Writer.Indent--;
                    }
                }

                if (table.ForeignKeys.Count > 0)
                {
                    this.Writer.WriteLine();
                    foreach (var fk in table.ForeignKeys)
                    {
                        this.Writer.WriteLine("new ForeignKey({0})", BuildSchemaObjectCSV(fk, this.SpecificationLevel));
                        this.Writer.Indent++;

                        if (fk.SourceColumns.Count == 1)
                        {
                            this.Writer.WriteLine(".WithSourceColumn(\"{0}\")", fk.SourceColumns[0].Name);
                        }
                        else
                        {
                            this.Writer.WriteLine(".WithSourceColumns({0})", BuildColumnNameCSV(fk.SourceColumns));
                        }

                        this.Writer.WriteLine(".References(new TableReference({0}), {1}),", BuildSchemaObjectCSV(fk.Target, this.SpecificationLevel), BuildColumnNameCSV(fk.TargetColumns));
                        this.Writer.Indent--;
                    }
                }

                this.Writer.Indent--;
                this.Writer.WriteLine("},"); // Table constructor
            }

            this.Writer.Indent--;
            this.Writer.WriteLine("}.Resolve();"); // DatabaseSchema constructor

            this.Writer.WriteLine();
            this.Writer.WriteLine("return schema;");

            this.Writer.Indent--;
            this.Writer.WriteLine("}"); // method

            this.Writer.Indent--;
            this.Writer.WriteLine("}"); // class

            this.Writer.Indent--;
            this.Writer.WriteLine("}"); // namespace
        }

        /// <summary>
        /// Builds a comma seperated list of column names enclosed in quotes
        /// </summary>
        /// <param name="columns">Columns to take names from</param>
        /// <returns>String containing the CSV</returns>
        private static string BuildColumnNameCSV(ICollection<Column> columns)
        {
            return string.Join(
                ", ", 
                columns.Select(c => string.Format(CultureInfo.InvariantCulture, "\"{0}\"", c.Name)).ToArray());
        }

        /// <summary>
        /// Builds arguments for a SchemaObjectConstructor for a given specification level
        /// </summary>
        /// <param name="obj">SchemaObject to be constructed</param>
        /// <param name="specification">Level at which object is to be specified</param>
        /// <returns>String representing constructor arguments</returns>
        private static string BuildSchemaObjectCSV(SchemaObject obj, SchemaObjectSpecificationLevel specification)
        {
            switch (specification)
            {
                case SchemaObjectSpecificationLevel.Catalog:
                    return string.Format(CultureInfo.InvariantCulture, "\"{0}\", \"{1}\", \"{2}\"", obj.Catalog, obj.Schema, obj.Name);

                case SchemaObjectSpecificationLevel.Schema:
                    return string.Format(CultureInfo.InvariantCulture, "\"{0}\", \"{1}\"", obj.Schema, obj.Name);

                default:
                    return string.Format(CultureInfo.InvariantCulture, "\"{0}\"", obj.Name);
            }
        }
    }
}
