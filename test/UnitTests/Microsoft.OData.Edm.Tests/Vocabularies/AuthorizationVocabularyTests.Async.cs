//---------------------------------------------------------------------
// <copyright file="AuthorizationVocabularyTests.Async.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.OData.Edm.Csdl;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Vocabularies
{
    public partial class AuthorizationVocabularyTests
    {
        [Fact]
        public async Task TestAuthorizationVocabularyModel_Async()
        {
            const string expectedText = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Schema Namespace=""Org.OData.Authorization.V1"" Alias=""Auth"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"">
  <TypeDefinition Name=""SchemeName"" UnderlyingType=""Edm.String"">
    <Annotation Term=""Core.Description"" String=""The name of the authorization scheme."" />
  </TypeDefinition>
  <ComplexType Name=""SecurityScheme"">
    <Property Name=""Authorization"" Type=""Auth.SchemeName"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The name of a required authorization scheme"" />
    </Property>
    <Property Name=""RequiredScopes"" Type=""Collection(Edm.String)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""The names of scopes required from this authorization scheme"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""Authorization"" Abstract=""true"">
    <Property Name=""Name"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Name that can be used to reference the authorization scheme"" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Description of the authorization scheme"" />
    </Property>
    <Annotation Term=""Core.Description"" String=""Base type for all Authorization types"" />
  </ComplexType>
  <ComplexType Name=""OpenIDConnect"" BaseType=""Auth.Authorization"">
    <Property Name=""IssuerUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Issuer location for the OpenID Provider. Configuration information can be obtained by appending `/.well-known/openid-configuration` to this Url."" />
      <Annotation Term=""Core.IsURL"" />
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
    <Property Name=""Scopes"" Type=""Collection(Auth.AuthorizationScope)"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Available scopes"" />
    </Property>
    <Property Name=""RefreshUrl"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Refresh Url"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2ClientCredentials"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2Implicit"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""AuthorizationUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Authorization URL"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2Password"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""OAuth2AuthCode"" BaseType=""Auth.OAuthAuthorization"">
    <Property Name=""AuthorizationUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Authorization URL"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
    <Property Name=""TokenUrl"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Token Url"" />
      <Annotation Term=""Core.IsURL"" />
    </Property>
  </ComplexType>
  <ComplexType Name=""AuthorizationScope"">
    <Property Name=""Scope"" Type=""Edm.String"" Nullable=""false"">
      <Annotation Term=""Core.Description"" String=""Scope name"" />
    </Property>
    <Property Name=""Grant"" Type=""Edm.String"">
      <Annotation Term=""Core.Description"" String=""Identity that has access to the scope or can grant access to the scope."" />
    </Property>
    <Property Name=""Description"" Type=""Edm.String"" Nullable=""false"">
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
  <Term Name=""SecuritySchemes"" Type=""Collection(Auth.SecurityScheme)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""At least one of the specified security schemes are required to make a request against the service"" />
  </Term>
  <Term Name=""Authorizations"" Type=""Collection(Auth.Authorization)"" AppliesTo=""EntityContainer"" Nullable=""false"">
    <Annotation Term=""Core.Description"" String=""Lists the methods supported by the service to authorize access"" />
  </Term>
</Schema>";

            var sw = new StringWriter();
            XmlWriterSettings settings = new XmlWriterSettings() { Async = true };
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;

            XmlWriter xw = XmlWriter.Create(sw, settings);
            var (_, errors) = await this._authorizationModel.TryWriteSchemaAsync(xw).ConfigureAwait(false);
            await xw.FlushAsync().ConfigureAwait(false);

            xw.Close();
            string output = sw.ToString();

            Assert.True(!errors.Any(), "No Errors");
            Assert.Equal(expectedText, output);
        }
    }
}
