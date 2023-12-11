//---------------------------------------------------------------------
// <copyright file="CsdlWriterTests.TargetPath.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.OData.Edm.Csdl;
using Microsoft.OData.Edm.Vocabularies;
using Xunit;

namespace Microsoft.OData.Edm.Tests.Csdl
{
    public partial class CsdlWriterTests
    {
        [Fact]
        public void ShouldWriteAnnotationForEntitySetProperty()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(entitySet);
            model.AddElement(container);

            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();

            EdmTargetPath targetPath = new EdmTargetPath(container, entitySet, nameProperty);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(targetPath, term, new EdmStringConstant("Name OutOfLine MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
              "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                  "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                    "<EntityType Name=\"Customer\">" +
                      "<Key>" +
                        "<PropertyRef Name=\"Id\" />" +
                      "</Key>" +
                      "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                      "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<Term Name=\"MyTerm\" Type=\"Edm.String\" />" +
                    "<EntityContainer Name=\"Default\">" +
                       "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\" />" +
                    "</EntityContainer>" +
                    "<Annotations Target=\"NS.Default/Customers/Name\">" +
                      "<Annotation Term=\"NS.MyTerm\" String=\"Name OutOfLine MyTerm Value\" />" +
                    "</Annotations>" +
                  "</Schema>" +
                "</edmx:DataServices>" +
              "</edmx:Edmx>");

            // Act & Assert for JSON
            WriteAndVerifyJson(model, @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {}
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    },
    ""$Annotations"": {
      ""NS.Default/Customers/Name"": {
        ""@NS.MyTerm"": ""Name OutOfLine MyTerm Value""
      }
    }
  }
}");
        }

        [Fact]
        public void WriteInlineAnnotationForTargetPathThrows()
        {
            // Arrange
            EdmModel model = new EdmModel();
            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(entitySet);
            model.AddElement(container);

            IEdmProperty nameProperty = customer.DeclaredProperties.Where(x => x.Name == "Name").FirstOrDefault();

            EdmTargetPath targetPath = new EdmTargetPath(container, entitySet, nameProperty);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(targetPath, term, new EdmStringConstant("Name OutOfLine MyTerm Value"));

            // Act & Assert
            Action action = () => annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.Inline);

            InvalidOperationException exception = Assert.Throws<InvalidOperationException>(action);
            Assert.Equal(Strings.EdmVocabularyAnnotations_InvalidLocationForTargetPathAnnotation(annotation.TargetString()), exception.Message);
        }

        [Fact]
        public void ShouldWriteAnnotationForEntitySetComplexTypeProperty()
        {
            // Arrange
            EdmModel model = new EdmModel();

            EdmComplexType addressType = new EdmComplexType("NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String, isNullable: false);
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, isNullable: false);
            addressType.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.Int32, isNullable: false);
            model.AddElement(addressType);

            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            customer.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(entitySet);
            model.AddElement(container);

            IEdmProperty addressProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            IEdmProperty streetProperty = addressType.DeclaredProperties.Where(x => x.Name == "Street").FirstOrDefault();

            EdmTargetPath targetPath = new EdmTargetPath(container, entitySet, addressProperty, streetProperty);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(targetPath, term, new EdmStringConstant("Name OutOfLine MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
              "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                  "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                    "<ComplexType Name=\"Address\">" +
                      "<Property Name=\"Street\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"City\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"PostalCode\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "</ComplexType>" +
                    "<EntityType Name=\"Customer\">" +
                      "<Key>" +
                        "<PropertyRef Name=\"Id\" />" +
                      "</Key>" +
                      "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                      "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"Address\" Type=\"NS.Address\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<Term Name=\"MyTerm\" Type=\"Edm.String\" />" +
                    "<EntityContainer Name=\"Default\">" +
                       "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\" />" +
                    "</EntityContainer>" +
                    "<Annotations Target=\"NS.Default/Customers/Address/Street\">" +
                      "<Annotation Term=\"NS.MyTerm\" String=\"Name OutOfLine MyTerm Value\" />" +
                    "</Annotations>" +
                  "</Schema>" +
                "</edmx:DataServices>" +
              "</edmx:Edmx>");

            // Act & Assert for JSON
            WriteAndVerifyJson(model, @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""Street"": {},
      ""City"": {},
      ""PostalCode"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {},
      ""Address"": {
        ""$Type"": ""NS.Address""
      }
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    },
    ""$Annotations"": {
      ""NS.Default/Customers/Address/Street"": {
        ""@NS.MyTerm"": ""Name OutOfLine MyTerm Value""
      }
    }
  }
}");
        }

        [Fact]
        public void ShouldWriteAnnotationForSingletonComplexTypeProperty()
        {
            // Arrange
            EdmModel model = new EdmModel();

            EdmComplexType addressType = new EdmComplexType("NS", "Address");
            addressType.AddStructuralProperty("Street", EdmPrimitiveTypeKind.String, isNullable: false);
            addressType.AddStructuralProperty("City", EdmPrimitiveTypeKind.String, isNullable: false);
            addressType.AddStructuralProperty("PostalCode", EdmPrimitiveTypeKind.Int32, isNullable: false);
            model.AddElement(addressType);

            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            customer.AddStructuralProperty("Address", new EdmComplexTypeReference(addressType, isNullable: false));
            model.AddElement(customer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(entitySet);
            IEdmSingleton singleton = container.AddSingleton("SpecialCustomer", customer);
            model.AddElement(container);

            IEdmProperty addressProperty = customer.DeclaredProperties.Where(x => x.Name == "Address").FirstOrDefault();
            IEdmProperty streetProperty = addressType.DeclaredProperties.Where(x => x.Name == "Street").FirstOrDefault();

            EdmTargetPath targetPath = new EdmTargetPath(container, singleton, addressProperty, streetProperty);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(targetPath, term, new EdmStringConstant("Name OutOfLine MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
              "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                  "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                    "<ComplexType Name=\"Address\">" +
                      "<Property Name=\"Street\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"City\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"PostalCode\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                    "</ComplexType>" +
                    "<EntityType Name=\"Customer\">" +
                      "<Key>" +
                        "<PropertyRef Name=\"Id\" />" +
                      "</Key>" +
                      "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                      "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                      "<Property Name=\"Address\" Type=\"NS.Address\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<Term Name=\"MyTerm\" Type=\"Edm.String\" />" +
                    "<EntityContainer Name=\"Default\">" +
                       "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\" />" +
                       "<Singleton Name=\"SpecialCustomer\" Type=\"NS.Customer\" />" +
                    "</EntityContainer>" +
                    "<Annotations Target=\"NS.Default/SpecialCustomer/Address/Street\">" +
                      "<Annotation Term=\"NS.MyTerm\" String=\"Name OutOfLine MyTerm Value\" />" +
                    "</Annotations>" +
                  "</Schema>" +
                "</edmx:DataServices>" +
              "</edmx:Edmx>");

            // Act & Assert for JSON
            WriteAndVerifyJson(model, @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Address"": {
      ""$Kind"": ""ComplexType"",
      ""Street"": {},
      ""City"": {},
      ""PostalCode"": {
        ""$Type"": ""Edm.Int32""
      }
    },
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {},
      ""Address"": {
        ""$Type"": ""NS.Address""
      }
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      },
      ""SpecialCustomer"": {
        ""$Type"": ""NS.Customer""
      }
    },
    ""$Annotations"": {
      ""NS.Default/SpecialCustomer/Address/Street"": {
        ""@NS.MyTerm"": ""Name OutOfLine MyTerm Value""
      }
    }
  }
}");
        }

        [Fact]
        public void ShouldWriteAnnotationForEntitySetDerivedTypeProperty()
        {
            // Arrange
            EdmModel model = new EdmModel();

            EdmEntityType customer = new EdmEntityType("NS", "Customer");
            customer.AddKeys(customer.AddStructuralProperty("Id", EdmCoreModel.Instance.GetInt32(false)));
            customer.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(customer);

            EdmEntityType vipCustomer = new EdmEntityType("NS", "VipCustomer", customer);
            vipCustomer.AddStructuralProperty("VipNumber", EdmPrimitiveTypeKind.String, isNullable: false);
            model.AddElement(vipCustomer);

            EdmEntityContainer container = new EdmEntityContainer("NS", "Default");
            EdmEntitySet entitySet = new EdmEntitySet(container, "Customers", customer);
            container.AddElement(entitySet);
            model.AddElement(container);

            IEdmProperty vipNoProperty = vipCustomer.DeclaredProperties.Where(x => x.Name == "VipNumber").FirstOrDefault();

            EdmTargetPath targetPath = new EdmTargetPath(container, entitySet, vipCustomer, vipNoProperty);

            EdmTerm term = new EdmTerm("NS", "MyTerm", EdmCoreModel.Instance.GetString(true));
            model.AddElement(term);
            EdmVocabularyAnnotation annotation = new EdmVocabularyAnnotation(targetPath, term, new EdmStringConstant("Name OutOfLine MyTerm Value"));
            annotation.SetSerializationLocation(model, EdmVocabularyAnnotationSerializationLocation.OutOfLine);
            model.SetVocabularyAnnotation(annotation);

            WriteAndVerifyXml(model, "<?xml version=\"1.0\" encoding=\"utf-16\"?>" +
              "<edmx:Edmx Version=\"4.0\" xmlns:edmx=\"http://docs.oasis-open.org/odata/ns/edmx\">" +
                "<edmx:DataServices>" +
                  "<Schema Namespace=\"NS\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\">" +
                    "<EntityType Name=\"Customer\">" +
                      "<Key>" +
                        "<PropertyRef Name=\"Id\" />" +
                      "</Key>" +
                      "<Property Name=\"Id\" Type=\"Edm.Int32\" Nullable=\"false\" />" +
                      "<Property Name=\"Name\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<EntityType Name=\"VipCustomer\" BaseType=\"NS.Customer\">" +
                      "<Property Name=\"VipNumber\" Type=\"Edm.String\" Nullable=\"false\" />" +
                    "</EntityType>" +
                    "<Term Name=\"MyTerm\" Type=\"Edm.String\" />" +
                    "<EntityContainer Name=\"Default\">" +
                       "<EntitySet Name=\"Customers\" EntityType=\"NS.Customer\" />" +
                    "</EntityContainer>" +
                    "<Annotations Target=\"NS.Default/Customers/NS.VipCustomer/VipNumber\">" +
                      "<Annotation Term=\"NS.MyTerm\" String=\"Name OutOfLine MyTerm Value\" />" +
                    "</Annotations>" +
                  "</Schema>" +
                "</edmx:DataServices>" +
              "</edmx:Edmx>");

            // Act & Assert for JSON
            WriteAndVerifyJson(model, @"{
  ""$Version"": ""4.0"",
  ""$EntityContainer"": ""NS.Default"",
  ""NS"": {
    ""Customer"": {
      ""$Kind"": ""EntityType"",
      ""$Key"": [
        ""Id""
      ],
      ""Id"": {
        ""$Type"": ""Edm.Int32""
      },
      ""Name"": {}
    },
    ""VipCustomer"": {
      ""$Kind"": ""EntityType"",
      ""$BaseType"": ""NS.Customer"",
      ""VipNumber"": {}
    },
    ""MyTerm"": {
      ""$Kind"": ""Term"",
      ""$Nullable"": true
    },
    ""Default"": {
      ""$Kind"": ""EntityContainer"",
      ""Customers"": {
        ""$Collection"": true,
        ""$Type"": ""NS.Customer""
      }
    },
    ""$Annotations"": {
      ""NS.Default/Customers/NS.VipCustomer/VipNumber"": {
        ""@NS.MyTerm"": ""Name OutOfLine MyTerm Value""
      }
    }
  }
}");
        }
    }
}
