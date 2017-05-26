//---------------------------------------------------------------------
// <copyright file="StubRecordExpression.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.VocabularyStubs
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Vocabularies;

    public class StubRecordExpression : StubNonAnnotatedElement, IEdmRecordExpression
    {
        private List<IEdmPropertyConstructor> properties = new List<IEdmPropertyConstructor>();

        public IEdmStructuredTypeReference DeclaredType
        {
            get; set;
        }

        public IEnumerable<IEdmPropertyConstructor> Properties
        {
            get { return properties; }
        }

        public EdmExpressionKind ExpressionKind
        {
            get { return EdmExpressionKind.Record; }
        }

        public void AddProperty(string propertyName, IEdmExpression expression)
        {
            properties.Add(new StubPropertyConstructor(propertyName, expression));
        }

        public override bool Equals(object obj)
        {
            var expression = obj as IEdmRecordExpression;
            if (expression == null || (this.properties.Count != expression.Properties.Count()))
            {
                return false;
            }
            foreach (var expected in this.properties)
            {
                var actual = expression.Properties.Single(n => n.Name == expected.Name);
                if (!expected.Value.Equals(actual.Value))
                {
                    return false;
                }
            }
            return true; 
        }

        public override int GetHashCode()
        {
            return this.properties.GetHashCode();
        }
    }
}
