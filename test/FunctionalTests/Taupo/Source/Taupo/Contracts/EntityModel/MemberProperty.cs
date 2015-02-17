//---------------------------------------------------------------------
// <copyright file="MemberProperty.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Contracts.EntityModel
{
    using System.Collections;
    using Microsoft.Test.Taupo.Contracts.Types;

    /// <summary>
    /// Represents a property of either <see cref="EntityType"/> or a <see cref="ComplexType"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "IEnumerable is just for easy construction. Type is not really a collection")]
    [System.Diagnostics.DebuggerDisplay("Property Name={this.Name} Type={this.PropertyType} PrimaryKey={this.IsPrimaryKey}")]
    public class MemberProperty : AnnotatedItem, IEnumerable
    {
        /// <summary>
        /// Initializes a new instance of the MemberProperty class without name or type.
        /// </summary>
        public MemberProperty()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MemberProperty class with a given name.
        /// </summary>
        /// <param name="name">Property name</param>
        public MemberProperty(string name)
            : this(name, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MemberProperty class with given name and <see cref="DataType"/>.
        /// </summary>
        /// <param name="name">Property name</param>
        /// <param name="type">Property type.</param>
        public MemberProperty(string name, DataType type)
        {
            this.Name = name;
            this.PropertyType = type;
        }

        /// <summary>
        /// Gets or sets name of the property.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets type of the property.
        /// </summary>
        public DataType PropertyType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the property is part of the containing entity's primary key. Default value is false.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the default value to which this property is set.
        /// </summary>
        /// <remarks>
        /// <para>This property is of type object to satisfy the wide number of types that could be here in CSDL. 
        /// The appropriate mappings in CSDL are as follows (specified in PrimitiveClrTypeFacet of the EdmDataType):</para>
        /// <para>Edm.Binary: System.Byte[]</para>
        /// <para>Edm.Boolean: System.Boolean</para>
        /// <para>Edm.Byte: System.Byte</para>
        /// <para>Edm.DateTime: System.DateTime</para>
        /// <para>Edm.DateTimeOffset: System.DateTimeOffset</para>
        /// <para>Edm.Decimal: System.Decimal</para>
        /// <para>Edm.Double: System.Double</para>
        /// <para>Edm.Guid: System.Guid</para>
        /// <para>Edm.Int16: System.Int16</para>
        /// <para>Edm.Int32: System.Int32</para>
        /// <para>Edm.Int64: System.Int64</para>
        /// <para>Edm.SByte: System.SByte</para>
        /// <para>Edm.Single: System.Single</para>
        /// <para>Edm.String: System.String</para>
        /// <para>Edm.Duration: System.TimeSpan</para>
        /// </remarks>
        public object DefaultValue { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="Microsoft.Test.Taupo.Contracts.EntityModel.MemberProperty"/>.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>The result of the conversion.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2225:OperatorOverloadsHaveNamedAlternates", Justification = "Not needed here.")]
        public static implicit operator MemberProperty(string propertyName)
        {
            return new MemberPropertyReference(propertyName);
        }
    }
}
