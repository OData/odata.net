//---------------------------------------------------------------------
// <copyright file="ValidationVocabularyTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using Microsoft.OData.Edm.Csdl;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public partial class ValidationVocabularyTests
    {
        [Fact]
        public async Task TestValidationVocabularyModel_Async()
        {
            #region Expected Text
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Validation.V1"" Alias=""Validation"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""SingleOrCollectionType"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The qualified name of a type in scope, optionally wrapped in `Collection()` to denote a collection of instances of the type"" />
  </TypeDefinition>
  <ComplexType Name=""AllowedValue"">
    <Property Name=""Value"" Type=""Edm.PrimitiveType"">
      <Annotation Term=""Core.Description"" String=""An allowed value for the property, parameter, or type definition"" />
    </Property>
    <Annotation Term=""Validation.ApplicableTerms"">
      <Collection>
        <String>Core.SymbolicName</String>
      </Collection>
    </Annotation>
  </ComplexType>
  <ComplexType Name=""ConstraintType"">
    <Property Name=""FailureMessage"" Type=""Edm.String"">
      <Annotation Term=""Core.IsLanguageDependent"" />
      <Annotation Term=""Core.Description"" String=""Human-readable message that can be shown to end users if the constraint is not fulfilled"" />
    </Property>
    <Property Name=""Condition"" Type=""Edm.Boolean"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Value MUST be a dynamic expression that evaluates to true if and only if the constraint is fulfilled"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ItemsOfType"">
    <Property Name=""path"" Type=""Edm.NavigationPropertyPath"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""A path to a single- or collection-valued navigation property"" />
    </Property>
    <Property Name=""target"" Type=""Edm.NavigationPropertyPath"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""A path to a collection-valued navigation property"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""Entities related via the single- or collection-valued navigation property identified by `path` are also related via the collection-valued navigation property identified by `target`."" />
  </ComplexType>
  <Term Name=""Pattern"" Type=""Edm.String"" AppliesTo=""Property Parameter Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The pattern that a string property, parameter, or term must match. This SHOULD be a valid regular expression, according to the ECMA 262 regular expression dialect."" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.String"" />
  </Term>
  <Term Name=""Minimum"" Type=""Edm.PrimitiveType"" AppliesTo=""Property Parameter Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Minimum value that a property, parameter, or term can have."" />
    <Annotation Term=""Validation.ApplicableTerms"">
      <Collection>
        <String>Validation.Exclusive</String>
      </Collection>
    </Annotation>
  </Term>
  <Term Name=""Maximum"" Type=""Edm.PrimitiveType"" AppliesTo=""Property Parameter Term"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Maximum value that a property, parameter, or term can have."" />
    <Annotation Term=""Validation.ApplicableTerms"">
      <Collection>
        <String>Validation.Exclusive</String>
      </Collection>
    </Annotation>
  </Term>
  <Term Name=""Exclusive"" Type=""Core.Tag"" DefaultValue=""true"" AppliesTo=""Annotation"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Tags a Minimum or Maximum as exclusive, i.e. an open interval boundary."" />
  </Term>
  <Term Name=""AllowedValues"" Type=""Collection(Validation.AllowedValue)"" AppliesTo=""Property Parameter TypeDefinition"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A collection of valid values for the annotated property, parameter, or type definition"" />
  </Term>
  <Term Name=""MultipleOf"" Type=""Edm.Decimal"" AppliesTo=""Property Parameter Term"" Nullable=""false"" Scale=""variable"">
    <Annotation Term=""Core.Description"" String=""The value of the annotated property, parameter, or term must be an integer multiple of this positive value. For temporal types, the value is measured in seconds."" />
  </Term>
  <Term Name=""Constraint"" Type=""Validation.ConstraintType"" AppliesTo=""Property EntityType ComplexType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Condition that the annotation target has to fulfill"" />
  </Term>
  <Term Name=""ItemsOf"" Type=""Collection(Validation.ItemsOfType)"" AppliesTo=""EntityType ComplexType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""A list of constraints describing that entities related via one navigation property MUST also be related via another, collection-valued navigation property. The same `path` value MUST NOT occur more than once."" />
    <Annotation Term=""Core.LongDescription"" String=""Example: entity type `Customer` has navigation properties `AllOrders`, `OpenOrders`, and `ClosedOrders`. &#xA;The term allows to express that items of `OpenOrders` and `ClosedOrders` are also items of the `AllOrders` navigation property,&#xA;even though they are defined in an `Orders` entity set."" />
  </Term>
  <Term Name=""OpenPropertyTypeConstraint"" Type=""Collection(Validation.SingleOrCollectionType)"" AppliesTo=""ComplexType EntityType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Dynamic properties added to the annotated open structured type are restricted to the listed types."" />
  </Term>
  <Term Name=""DerivedTypeConstraint"" Type=""Collection(Validation.SingleOrCollectionType)"" AppliesTo=""EntitySet Singleton NavigationProperty Property TypeDefinition Parameter ReturnType"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Values are restricted to types that are both identical to or derived from the declared type and a type listed in this collection."" />
    <Annotation Term=""Core.LongDescription"" String=""This allows restricting values to certain sub-trees of an inheritance hierarchy,&#xA;          including hierarchies starting at the [Built-In Abstract Types](https://docs.oasis-open.org/odata/odata-csdl-json/v4.01/odata-csdl-json-v4.01.html#sec_BuiltInAbstractTypes).&#xA;          Types listed in this collection are ignored if they are not derived from the declared type of the annotated model element&#xA;          or would not be allowed as declared type of the annotated model element.&#xA;&#xA;          When applied to a collection-valued element, this annotation specifies the types allowed for members&#xA;          of the collection without mentioning the `Collection()` wrapper.&#xA;          The SingleOrCollectionType may only include the `Collection()` wrapper&#xA;          if the annotation is applied to an element with declared type `Edm.Untyped`."" />
  </Term>
  <Term Name=""AllowedTerms"" Type=""Collection(Core.QualifiedTermName)"" AppliesTo=""Term Property"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Annotate a term of type Edm.AnnotationPath, or a property of type Edm.AnnotationPath that is used within a structured term, to restrict the terms that can be targeted by the path."" />
    <Annotation Term=""Core.LongDescription"" String=""The annotation path expression is intended to end in a path segment with one of the listed terms. For forward compatibility, clients should be prepared for the annotation to reference terms besides those listed."" />
    <Annotation Term=""Core.RequiresType"" String=""Edm.AnnotationPath"" />
  </Term>
  <Term Name=""ApplicableTerms"" Type=""Collection(Core.QualifiedTermName)"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Names of specific terms that are applicable and may be applied in the current context. This annotation does not restrict the use of other terms."" />
  </Term>
  <Term Name=""MaxItems"" Type=""Edm.Int64"" AppliesTo=""Collection"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The annotated collection must have at most the specified number of items."" />
  </Term>
  <Term Name=""MinItems"" Type=""Edm.Int64"" AppliesTo=""Collection"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""The annotated collection must have at least the specified number of items."" />
  </Term>
</Schema>";
            #endregion

            var sw = new StringWriter();
            var settings = new XmlWriterSettings()
            {
                Indent = true,
                Encoding = System.Text.Encoding.UTF8,
                Async = true
            };

            XmlWriter xw = XmlWriter.Create(sw, settings);
            var (_, errors) = await this._validationModel.TryWriteSchemaAsync(xw).ConfigureAwait(false);
            await xw.FlushAsync().ConfigureAwait(false);
            xw.Close();
            string output = sw.ToString();

            Assert.True(!errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }
    }
}
