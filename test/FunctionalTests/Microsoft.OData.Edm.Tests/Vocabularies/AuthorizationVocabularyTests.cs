//---------------------------------------------------------------------
// <copyright file="AuthorizationVocabularyTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Validation;
using Microsoft.OData.Edm.Vocabularies.V1;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    /// <summary>
    /// Test authorization vocabulary
    /// </summary>
    public class AuthorizationVocabularyTests
    {
        private readonly IEdmModel _authorizationModel = AuthorizationVocabularyModel.Instance;

        [Fact]
        public void TestAuthorizationVocabularyModel()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Authorization.V1"" Alias=""Auth"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <ComplexType Name=""Authorization"" Abstract=""true"">
    <Property Name=""Name"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Name that can be used to reference the authorization flow."" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Description of the authorization method"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""Base type for all Authorization types"" />
  </ComplexType>
  <ComplexType Name=""OpenIDConnect"" BaseType=""Auth.Authorization"">
    <Property Name=""IssuerUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Issuer location for the OpenID Provider. Configuration information can be obtained by appending `/.well-known/openid-configuration` to this Url."" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""Http"" BaseType=""Auth.Authorization"">
    <Property Name=""Scheme"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""HTTP Authorization scheme to be used in the Authorization header, as per RFC7235"" />
    </Property>
    <Property Name=""BearerFormat"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Format of the bearer token"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuthAuthorization"" BaseType=""Auth.Authorization"" Abstract=""true"">
    <Property Name=""Scopes"" Type=""Collection(Auth.AuthorizationScope)"">
      <Annotation Term=""Core.Description"" String=""Available scopes"" />
    </Property>
    <Property Name=""RefreshUrl"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Refresh Url"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2ClientCredentials"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2Implicit"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""AuthorizationUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Authorization URL"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2Password"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2AuthCode"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""AuthorizationUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Authorization URL"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" Bool=""true"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""AuthorizationScope"">
    <Property Name=""Scope"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Scope name"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Description of the scope"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""ApiKey"" BaseType=""Auth.Authorization"">
    <Property Name=""KeyName"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The name of the header or query parameter"" />
    </Property>
    <Property Name=""Location"" Type=""Auth.KeyLocation"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Whether the API Key is passed in the header or as a query option"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""SecurityScheme"">
    <Property Name=""AuthorizationSchemeName"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""The name of a required authorization scheme"" />
    </Property>
    <Property Name=""RequiredScopes"" Type=""Collection(Edm.String)"">
      <Annotation Term=""Core.Description"" String=""The names of scopes required from this authorization scheme."" />
    </Property>
  </ComplexType>
  <EnumType Name=""KeyLocation"">
    <Member Name=""Header"">
      <Annotation Term=""Core.Description"" String=""API Key is passed in the header"" />
    </Member>
    <Member Name=""QueryOption"">
      <Annotation Term=""Core.Description"" String=""API Key is passed as a query option"" />
    </Member>
    <Member Name=""Cookie"">
      <Annotation Term=""Core.Description"" String=""API Key is passed as a cookie"" />
    </Member>
  </EnumType>
  <Term Name=""Authorizations"" Type=""Collection(Auth.Authorization)"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""Lists the methods available to authorize access to the annotated resource"" />
  </Term>
  <Term Name=""SecuritySchemes"" Type=""Collection(Auth.SecurityScheme)"" AppliesTo=""EntityContainer"">
    <Annotation Term=""Core.Description"" String=""At least one of the specified security schemes are required to make a request against the service."" />
  </Term>
</Schema>";

            StringWriter sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            IEnumerable<EdmError> errors;
            XmlWriter xw = XmlWriter.Create(sw, settings);
            this._authorizationModel.TryWriteSchema(xw, out errors);
            xw.Flush();
#if NETCOREAPP1_0
            xw.Dispose();
#else
            xw.Close();
#endif
            string output = sw.ToString();

            Assert.True(!errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }

        [Fact]
        public void TestAuthorizationVocabularyAuthorizationsTerm()
        {
            var term = this._authorizationModel.FindDeclaredTerm("Org.OData.Authorization.V1.Authorizations");
            Assert.NotNull(term);

            Assert.NotNull(term.Type);
            Assert.Equal(EdmTypeKind.Collection, term.Type.Definition.TypeKind);
            Assert.Equal("Collection(Org.OData.Authorization.V1.Authorization)", term.Type.Definition.FullTypeName());

            Assert.Equal("EntityContainer", term.AppliesTo);
        }

        [Fact]
        public void TestAuthorizationVocabularySecuritySchemesTerm()
        {
            var term = this._authorizationModel.FindDeclaredTerm("Org.OData.Authorization.V1.SecuritySchemes");
            Assert.NotNull(term);

            Assert.NotNull(term.Type);
            Assert.Equal(EdmTypeKind.Collection, term.Type.Definition.TypeKind);
            Assert.Equal("Collection(Org.OData.Authorization.V1.SecurityScheme)", term.Type.Definition.FullTypeName());

            Assert.Equal("EntityContainer", term.AppliesTo);
        }

        [Theory]
        [InlineData("Authorization", null, "Name|Description", true)]
        [InlineData("OpenIDConnect", "Authorization", "IssuerUrl", false)]
        [InlineData("Http", "Authorization", "Scheme|BearerFormat", false)]
        [InlineData("OAuthAuthorization", "Authorization", "Scopes|RefreshUrl", true)]
        [InlineData("OAuth2ClientCredentials", "OAuthAuthorization", "TokenUrl", false)]
        [InlineData("OAuth2Implicit", "OAuthAuthorization", "AuthorizationUrl", false)]
        [InlineData("OAuth2Password", "OAuthAuthorization", "TokenUrl", false)]
        [InlineData("OAuth2AuthCode", "OAuthAuthorization", "AuthorizationUrl|TokenUrl", false)]
        [InlineData("AuthorizationScope", null, "Scope|Description", false)]
        [InlineData("ApiKey", "Authorization", "KeyName|Location", false)]
        [InlineData("SecurityScheme", null, "AuthorizationSchemeName|RequiredScopes", false)]
        public void TestAuthorizationVocabularyComplexType(string typeName, string baseName, string properties,
            bool isAbstract)
        {
            var schemaType = this._authorizationModel.FindDeclaredType("Org.OData.Authorization.V1." + typeName);
            Assert.NotNull(schemaType);

            Assert.Equal(EdmTypeKind.Complex, schemaType.TypeKind);
            IEdmComplexType complex = (IEdmComplexType) (schemaType);

            Assert.Equal(isAbstract, complex.IsAbstract);
            Assert.False(complex.IsOpen);

            if (baseName != null)
            {
                Assert.NotNull(complex.BaseType);
                var baseType = this._authorizationModel.FindDeclaredType("Org.OData.Authorization.V1." + baseName);
                Assert.NotNull(baseType);
                Assert.Same(baseType, complex.BaseType);
            }
            else
            {
                Assert.Null(complex.BaseType);
            }

            Assert.NotEmpty(complex.DeclaredProperties);
            Assert.Equal(properties, string.Join("|", complex.DeclaredProperties.Select(e => e.Name)));
            Assert.Empty(complex.DeclaredNavigationProperties());
        }

        [Fact]
        public void TestAuthorizationVocabularyKeyLocationEnumType()
        {
            var schemaType = this._authorizationModel.FindDeclaredType("Org.OData.Authorization.V1.KeyLocation");
            Assert.NotNull(schemaType);

            Assert.Equal(EdmTypeKind.Enum, schemaType.TypeKind);
            IEdmEnumType enumType = (IEdmEnumType)(schemaType);
            Assert.NotNull(enumType);
            Assert.Same(EdmCoreModel.Instance.GetPrimitiveType(EdmPrimitiveTypeKind.Int32), enumType.UnderlyingType);

            Assert.Equal("Header|QueryOption|Cookie", string.Join("|", enumType.Members.Select(e => e.Name)));
        }
    }
}
