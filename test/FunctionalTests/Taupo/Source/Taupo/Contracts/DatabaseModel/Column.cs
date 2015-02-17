//---------------------------------------------------------------------
// <copyright file="Column.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.DatabaseModel
{
    using System.Collections;
    using System.Collections.Generic;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Database column.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    public class Column : IAnnotatedStoreItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the Column class without a name or type.
        /// </summary>
        public Column()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Column class with given name but without a type.
        /// </summary>
        /// <param name="name">Column name.</param>
        public Column(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Column class with given name and type.
        /// </summary>
        /// <param name="name">Column name</param>
        /// <param name="dataType">Column type</param>
        public Column(string name, DataType dataType)
        {
            this.Name = name;
            this.ColumnType = dataType;
            this.Annotations = new List<Annotation>();
        }

        /// <summary>
        /// Gets or sets column name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets column type.
        /// </summary>
        public DataType ColumnType { get; set; }

        /// <summary>
        /// Gets the list of column annotations.
        /// </summary>
        /// <value>The annotations.</value>
        public IList<Annotation> Annotations { get; private set; }

        /// <summary>
        /// Adds the specified annotation.
        /// </summary>
        /// <param name="annotation">The annotation.</param>
        public void Add(Annotation annotation)
        {
            this.Annotations.Add(annotation);
        }

        /// <summary>
        /// Marks the column as auto-generated identity column with default parameters.
        /// </summary>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public Column Identity()
        {
            return this.Identity(1, 1);
        }

        /// <summary>
        /// Marks the column as auto-generated identity column with given parameters.
        /// </summary>
        /// <param name="startValue">The start value.</param>
        /// <param name="incrementBy">The value to increment the counter by.</param>
        /// <returns>This object (useful for chaining multiple calls)</returns>
        public Column Identity(int startValue, int incrementBy)
        {
            this.Add(new IdentityColumnAnnotation(startValue, incrementBy));
            return this;
        }

        /// <summary>
        /// This method is not supported and throws <see cref="TaupoNotSupportedException"/>
        /// </summary>
        /// <returns>This method never returns a result.</returns>
        public IEnumerator GetEnumerator()
        {
            throw ExceptionUtilities.CreateIEnumerableNotImplementedException();
        }
    }
}
