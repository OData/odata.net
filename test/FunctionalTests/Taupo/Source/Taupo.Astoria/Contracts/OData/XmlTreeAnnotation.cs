//---------------------------------------------------------------------
// <copyright file="XmlTreeAnnotation.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts.OData
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;

    /// <summary>
    /// A simple representation of the xml trees that can be present in Atom payloads
    /// </summary>
    public class XmlTreeAnnotation : ODataPayloadElementEquatableAnnotation
    {
        /// <summary>
        /// Initializes a new instance of the XmlTreeAnnotation class
        /// </summary>
        public XmlTreeAnnotation()
        {
            this.Children = new List<XmlTreeAnnotation>();
            this.ValueEqualityFunc = ValueComparer.Instance.Equals;
        }

        /// <summary>
        /// Gets or sets the value of the mapped property represented by this node in the tree
        /// </summary>
        public string PropertyValue { get; set; }

        /// <summary>
        /// Gets or sets the namespace for the tree's root
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Gets or sets the namespace prefix for the tree's root
        /// </summary>
        public string NamespacePrefix { get; set; }

        /// <summary>
        /// Gets or sets the local name for the tree's root
        /// </summary>
        public string LocalName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not this node in the tree is an attribute
        /// </summary>
        public bool IsAttribute { get; set; }
        
        /// <summary>
        /// Gets the child notes of the this node
        /// </summary>
        public ICollection<XmlTreeAnnotation> Children { get; private set; }

        /// <summary>
        /// Gets a string representation of the annotation to make debugging easier
        /// </summary>
        public override string StringRepresentation
        {
            get
            {
                return string.Format(CultureInfo.InvariantCulture, "EPM:{0}", this.LocalName);
            }
        }

        internal Func<object, object, bool> ValueEqualityFunc { get; set; }

        /// <summary>
        ///  Helper method for creating atom entity-property-mapping trees
        /// </summary>
        /// <param name="localName">The local name</param>
        /// <param name="value">The property value</param>
        /// <param name="children">The children for the tree</param>
        /// <returns>A new tree with the given values</returns>
        public static XmlTreeAnnotation Atom(string localName, string value, params XmlTreeAnnotation[] children)
        {
            return new XmlTreeAnnotation()
            {
                LocalName = localName,
                NamespaceName = ODataConstants.AtomNamespaceName,
                NamespacePrefix = null,
                IsAttribute = false,
                PropertyValue = value,
                Children = children.ToList(),
            };
        }

        /// <summary>
        ///  Helper method for creating atom attribute entity-property-mapping trees
        /// </summary>
        /// <param name="localName">The local name</param>
        /// <param name="value">The property value</param>
        /// <returns>A new tree with the given values</returns>
        public static XmlTreeAnnotation AtomAttribute(string localName, string value)
        {
            return new XmlTreeAnnotation()
            {
                LocalName = localName,
                NamespaceName = string.Empty,
                NamespacePrefix = null,
                IsAttribute = true,
                PropertyValue = value,
                Children = new List<XmlTreeAnnotation>()
            };
        }

        /// <summary>
        ///  Helper method for creating custom (non-atom) entity-property-mapping trees
        /// </summary>
        /// <param name="localName">The local name</param>
        /// <param name="namespaceName">The namespace name</param>
        /// <param name="namespacePrefix">The namespace prefix</param>
        /// <param name="isAttribute">Whether or not it is an attribute</param>
        /// <param name="value">The property value</param>
        /// <param name="children">The children for the tree</param>
        /// <returns>A new tree with the given values</returns>
        public static XmlTreeAnnotation Custom(string localName, string namespaceName, string namespacePrefix, bool isAttribute, string value, params XmlTreeAnnotation[] children)
        {
            ExceptionUtilities.Assert(namespaceName != ODataConstants.AtomNamespaceName, "Should not be used for ATOM mappings");
            return new XmlTreeAnnotation()
            {
                LocalName = localName,
                NamespaceName = namespaceName,
                NamespacePrefix = namespacePrefix,
                IsAttribute = isAttribute,
                PropertyValue = value,
                Children = children.ToList(),
            };
        }

        /// <summary>
        /// Creates a clone of the annotation
        /// </summary>
        /// <returns>a clone of the annotation</returns>
        public override ODataPayloadElementAnnotation Clone()
        {
            var clone = new XmlTreeAnnotation
            {
                PropertyValue = this.PropertyValue,
                NamespaceName = this.NamespaceName,
                NamespacePrefix = this.NamespacePrefix,
                LocalName = this.LocalName,
                IsAttribute = this.IsAttribute,
                ValueEqualityFunc = this.ValueEqualityFunc,
            };

            clone.Children.AddRange(this.Children.Select(c => (XmlTreeAnnotation)c.Clone()).ToArray());
            return clone;
        }

        /// <summary>
        /// Returns whether or not the given annotation is equal to the current annotation
        /// </summary>
        /// <param name="other">The annotation to compare to</param>
        /// <returns>True if the annotations are equivalent, false otherwise</returns>
        public override bool Equals(ODataPayloadElementEquatableAnnotation other)
        {
            ExceptionUtilities.CheckObjectNotNull(this.ValueEqualityFunc, "Value equality callback must not be null");
            return this.CastAndCheckEquality<XmlTreeAnnotation>(
                other,
                tree =>
                {
                    if (this.IsAttribute != tree.IsAttribute)
                    {
                        return false;
                    }

                    if (this.LocalName != tree.LocalName)
                    {
                        return false;
                    }

                    if (this.NamespaceName != tree.NamespaceName)
                    {
                        return false;
                    }

                    if (this.NamespacePrefix != tree.NamespacePrefix)
                    {
                        return false;
                    }

                    if (!this.ValueEqualityFunc(this.PropertyValue, tree.PropertyValue))
                    {
                        return false;
                    }

                    if (this.Children.Count != tree.Children.Count)
                    {
                        return false;
                    }

                    var remainingChildren = tree.Children.ToList();
                    foreach (var child in this.Children)
                    {
                        var match = remainingChildren.FirstOrDefault(c => child.Equals(c));
                        if (match == null)
                        {
                            return false;
                        }

                        remainingChildren.Remove(match);
                    }

                    return true;
                });
        }
    }
}
