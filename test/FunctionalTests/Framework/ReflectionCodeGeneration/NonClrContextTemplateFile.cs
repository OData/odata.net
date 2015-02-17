//---------------------------------------------------------------------
// <copyright file="NonClrContextTemplateFile.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria;
using System.Reflection;
using DSP = Microsoft.OData.Service.Providers;
using System.Collections;
using System.Data.Test.Astoria.NonClr;

namespace System.Data.Test.Astoria.ReflectionProvider
{
    public class NonClrContextTemplateFile : TemplateFile
    {
        private Workspace _workspace;

        public NonClrContextTemplateFile(string destinationPath)
            : this(null, destinationPath)
        {
        }

        public NonClrContextTemplateFile(Workspace workspace, string destinationPath)
            : base("Microsoft.Data.Test.ReflectionCodeGeneration.Templates.NonClrContextTemplate.cs", destinationPath)
        {
            _workspace = workspace;

            this.ReplaceCustomTokens += GenerateServiceOperations;
            this.ReplaceCustomTokens += GenerateMetadata;

            this.AddDefaultWorkspaceTokens(workspace);
            this.Tokens.Add("[[Inheritance]]", "NonClrContext");
        }

        public void GenerateMetadata(TemplateFile templateFile)
        {
            StringBuilder complexTypes = new StringBuilder();
            StringBuilder complexTypeProperties = new StringBuilder();

            StringBuilder resourceTypes = new StringBuilder();
            StringBuilder resourceTypeProperties = new StringBuilder();

            StringBuilder resourceSets = new StringBuilder();

            StringBuilder setDictionary = new StringBuilder();
            StringBuilder typeDictionary = new StringBuilder();

            StringBuilder clrTypeClasses = new StringBuilder();

            // Complex Types
            foreach (ComplexType complexType in _workspace.ServiceContainer.ComplexTypes)
            {
                complexTypes.AppendLine(CreateType(complexType));
                complexTypes.AppendLine();

                // properties
                foreach (ResourceProperty property in complexType.Properties)
                {
                    if (complexType.BaseType == null || !complexType.BaseTypes.Any(baseType => baseType.Properties.Any(p => p.Name == property.Name)))
                        complexTypeProperties.AppendLine(CreateProperty(complexType, property));
                }

                // type Dictionary
                typeDictionary.AppendLine(String.Format("types.Add({0});", complexType.Name.ToLowerInvariant()));
            }
            typeDictionary.AppendLine();

            // Resource Types
            foreach (ResourceType type in _workspace.ServiceContainer.ResourceTypes)
            {
                bool lazyLoad = false;

                if (_workspace.Settings.UseLazyPropertyLoading)
                {
                    Random r = AstoriaTestProperties.Random;
                    if (r.Next(0, 2) == 0)
                    {
                        lazyLoad = true;
                        type.Facets.IsLazyLoaded = true;
                    }
                }

                resourceTypes.AppendLine(CreateType(type, lazyLoad));
                typeDictionary.AppendLine(String.Format("types.Add({0});", type.Name.ToLowerInvariant()));

                if (type.Facets.IsClrType)
                    clrTypeClasses.Append(CreateClrTypeClass(type));


                foreach (ResourceProperty property in type.Properties.Where(p => p.Facets.IsDeclaredProperty))
                {
                    if (type.BaseType == null || !type.BaseTypes.Any(baseType => baseType.Properties.Any(p => p.Name == property.Name)))
                        resourceTypeProperties.AppendLine(CreateProperty(type, property, lazyLoad));
                }
            }

            // Resource Sets
            foreach (ResourceContainer container in _workspace.ServiceContainer.ResourceContainers)
            {
                if (container is ServiceOperation)
                    continue;

                // resource sets
                string setName = container.Name;
                string setVariableName = container.Name + "EntitySet";
                string resourceTypeName = container.BaseType.Name.ToLowerInvariant();

                resourceSets.AppendLine(String.Format("ResourceSet {0} = new ResourceSet(\"{1}\", {2});", setVariableName, setName, resourceTypeName));

                if (container.Facets.MestTag != null)
                    resourceSets.AppendLine(String.Format("{0}.CustomState = \"{1}\";", setVariableName, container.Facets.MestTag));

                resourceSets.AppendLine();

                // Add to containers
                setDictionary.AppendLine(String.Format("containers.Add({0});", setVariableName));
            }

            StringBuilder completeCode = new StringBuilder();

            // Order is important!
            completeCode.Append(complexTypes.ToString());
            completeCode.Append(complexTypeProperties.ToString());
            completeCode.Append(resourceTypes.ToString());
            completeCode.Append(resourceSets.ToString());
            completeCode.Append(resourceTypeProperties.ToString());
            completeCode.Append(typeDictionary.ToString());
            completeCode.Append(setDictionary.ToString());

            templateFile.FileText = templateFile.FileText.Replace("[[GeneratedMetadata]]", completeCode.ToString());
            templateFile.FileText = templateFile.FileText.Replace("[[ClrBackingTypes]]", clrTypeClasses.ToString());
        }

        private string CreateType(ComplexType type)
        {
            return CreateType(type, false);
        }

        private string CreateType(ComplexType type, bool lazyLoad)
        {
            StringBuilder resourceType = new StringBuilder();

            string resourceTypeDeclarationName = type.Name.ToLowerInvariant();

            string backingTypeParam;
            if (type.Facets.IsClrType)
                backingTypeParam = "typeof(" + type.Name + ")";
            else if (type is ResourceType)
                backingTypeParam = "typeof(RowEntityType)";
            else
                backingTypeParam = "typeof(RowComplexType)";

            string typeKindParam;
            if (type is ResourceType)
                typeKindParam = "ResourceTypeKind.EntityType";
            else
                typeKindParam = "ResourceTypeKind.ComplexType";

            string baseTypeParam;
            if (type.BaseType == null)
                baseTypeParam = "null";
            else
                baseTypeParam = type.BaseType.Name.ToLowerInvariant();

            string typeNamespaceParam = String.Format("\"{0}\"", type.Namespace);
            string typeNameParam = String.Format("\"{0}\"", type.Name);

            string abstractTypeParam;
            if (type.Facets.AbstractType)
                abstractTypeParam = "true";
            else
                abstractTypeParam = "false";

            string typeParams = String.Join(", ", new string[] { backingTypeParam, typeKindParam, baseTypeParam, typeNamespaceParam, typeNameParam, abstractTypeParam });

            if (lazyLoad)
                resourceType.AppendLine(String.Format("LazyResourceType {0} = new LazyResourceType({1});", resourceTypeDeclarationName, typeParams));
            else
                resourceType.AppendLine(String.Format("ResourceType {0} = new ResourceType({1});", resourceTypeDeclarationName, typeParams));

            // Set IsMediaLinkEntry to true whenever BlobAttribute is added to Facets.
            if (type.Facets.HasStream)
                resourceType.AppendLine(String.Format("{0}.IsMediaLinkEntry = true;", resourceTypeDeclarationName));

            // add calls to AddNamedStream for each named stream on the type
            foreach (string namedStream in type.Facets.NamedStreams)
            {
                if (type.BaseType == null || !type.BaseType.Facets.NamedStreams.Contains(namedStream))
                {
                    resourceType.AppendLine(String.Format("{0}.AddProperty(new ResourceProperty(\"{1}\", ResourcePropertyKind.Stream, ResourceType.GetPrimitiveResourceType(typeof(global::System.IO.Stream))));", resourceTypeDeclarationName, namedStream));
                }
            }

            // Set open type kind (default being not open)
            if (type.Facets.IsOpenType)
            {
                // let derived open types automatically pick up the setting
                if (type.BaseType == null || !type.BaseType.Facets.IsOpenType)
                    resourceType.AppendLine(String.Format("{0}.IsOpenType = true;", resourceTypeDeclarationName));
            }
            else
                resourceType.AppendLine(String.Format("{0}.IsOpenType = false;", resourceTypeDeclarationName));

            if (type.Facets.IsClrType)
                resourceType.AppendLine(String.Format("{0}.CanReflectOnInstanceType = true;", resourceTypeDeclarationName));
            else
                resourceType.AppendLine(String.Format("{0}.CanReflectOnInstanceType = false;", resourceTypeDeclarationName));

            return resourceType.ToString();
        }

        private class PropertyReuseInfo
        {
            public string DeclarationName
            {
                get;
                set;
            }

            public string PropertyParams
            {
                get;
                set;
            }

            public bool IsClr
            {
                get;
                set;
            }

        }

        private List<PropertyReuseInfo> _propertyReuseInfo = new List<PropertyReuseInfo>();

        private string ExistingPropertyName(string propertyParams, bool isClr, string propertyDeclarationName)
        {
            PropertyReuseInfo propertyReuseInfo = _propertyReuseInfo
                .SingleOrDefault(pri => pri.PropertyParams == propertyParams && pri.IsClr == isClr);

            if (propertyReuseInfo == null)
            {
                propertyReuseInfo = new PropertyReuseInfo();
                propertyReuseInfo.IsClr = isClr;
                propertyReuseInfo.PropertyParams = propertyParams;
                propertyReuseInfo.DeclarationName = propertyDeclarationName;

                _propertyReuseInfo.Add(propertyReuseInfo);

                return null;
            }
            else
                return propertyReuseInfo.DeclarationName;
        }

        private string CreateProperty(ComplexType type, ResourceProperty property)
        {
            return CreateProperty(type, property, false);
        }

        private string CreateProperty(ComplexType type, ResourceProperty property, bool lazyLoad)
        {
            StringBuilder propertyCode = new StringBuilder();

            string typeName = type.Name.ToLowerInvariant();

            string propertyDeclarationName = type.Name + property.Name + "Property";

            string propertyNameParam = String.Format("\"{0}\"", property.Name);
            string propertyKindParam = GetPropertyKindString(property);
            string propertyTypeParam = GetPropertyTypeString(property);

            string propertyParams = string.Join(", ", new string[] { propertyNameParam, propertyKindParam, propertyTypeParam });

            string existingName = ExistingPropertyName(propertyParams, property.Facets.IsClrProperty, propertyDeclarationName);
            if (existingName != null && !property.IsComplexType && !property.IsNavigation)
            {
                if (lazyLoad)
                    propertyCode.AppendLine(String.Format("{0}.AddLazyProperty({1});", typeName, existingName));
                else
                    propertyCode.AppendLine(String.Format("{0}.AddProperty({1});", typeName, existingName));
            }
            else
            {
                propertyCode.AppendLine(String.Format("ResourceProperty {0} = new ResourceProperty({1});", propertyDeclarationName, propertyParams));

                if (property.Facets.IsClrProperty)
                    propertyCode.AppendLine(String.Format("{0}.CanReflectOnInstanceTypeProperty = true;", propertyDeclarationName));
                else
                    propertyCode.AppendLine(String.Format("{0}.CanReflectOnInstanceTypeProperty = false;", propertyDeclarationName));

                if (property.IsNavigation)
                {
                    if (property.OtherSideNavigationProperty != null)
                        propertyCode.AppendLine(String.Format("{0}.CustomState = \"{1}\";", propertyDeclarationName, property.OtherSideNavigationProperty.ResourceAssociation.Name));
                    else if (property.Facets.MestTag != null)
                        propertyCode.AppendLine(String.Format("{0}.CustomState = \"{1}\";", propertyDeclarationName, property.Facets.MestTag));
                }
                else if (property.Facets.ServerGenerated)
                {
                    if (property.Type != Clr.Types.Int32)
                        AstoriaTestLog.FailAndThrow("Server-generated values only supported for ints");
                    propertyCode.AppendLine(String.Format("{0}.CustomState = \"{1}\";", propertyDeclarationName, NonClr.NonClrContext.ServerGeneratedCustomState));
                }

                if (lazyLoad)
                    propertyCode.AppendLine(String.Format("{0}.AddLazyProperty({1});", typeName, propertyDeclarationName));
                else
                    propertyCode.AppendLine(String.Format("{0}.AddProperty({1});", typeName, propertyDeclarationName));
            }

            return propertyCode.ToString();
        }

        private string GetPropertyTypeString(ResourceProperty property)
        {
            if (property.IsComplexType || property.IsNavigation)
                return property.Type.Name.ToLowerInvariant().Replace("[]", "");
            else
            {
                string propertyType = property.Type.ClrType.Name;

                if (property.Facets.Nullable && (property.Type.ClrType != null && (property.Type.ClrType.IsPrimitive || property.Type.ClrType.IsValueType)))
                    propertyType = "Nullable<" + propertyType + ">";

                return String.Format("ResourceType.GetPrimitiveResourceType(typeof({0}))", propertyType);
            }
        }

        private string GetPropertyKindString(ResourceProperty property)
        {
            List<DSP.ResourcePropertyKind> propertyKinds = new List<DSP.ResourcePropertyKind>();

            if (property.PrimaryKey != null)
            {
                //TODO: is it safe to assume keys are always primitive? we have been doing so, but the API doesn't enforce it
                propertyKinds.Add(DSP.ResourcePropertyKind.Key);
                propertyKinds.Add(DSP.ResourcePropertyKind.Primitive);
            }
            else if (property.IsComplexType)
            {
                propertyKinds.Add(DSP.ResourcePropertyKind.ComplexType);
            }
            else if (property.IsNavigation)
            {
                if (property.Type is CollectionType)
                    propertyKinds.Add(DSP.ResourcePropertyKind.ResourceSetReference);
                else
                    propertyKinds.Add(DSP.ResourcePropertyKind.ResourceReference);
            }
            else
            {
                propertyKinds.Add(DSP.ResourcePropertyKind.Primitive);
            }

            if (property.Facets.ConcurrencyModeFixed)
                propertyKinds.Add(DSP.ResourcePropertyKind.ETag);

            return String.Join(" | ", propertyKinds.Select(pk => pk.GetType().Name + "." + pk.ToString()).ToArray());
        }

        private string CreateClrTypeClass(ResourceType resourceType)
        {
            StringBuilder clrTypeClass = new StringBuilder();

            if (resourceType.BaseType != null && resourceType.BaseType.Facets.IsClrType)
                clrTypeClass.AppendLine("public class " + resourceType.Name + " : " + resourceType.BaseType.Name);
            else
                clrTypeClass.AppendLine("public class " + resourceType.Name + " : RowEntityType");

            clrTypeClass.AppendLine("{");
            clrTypeClass.AppendLine("    public " + resourceType.Name + "(string containerName) : base(containerName, \"" + resourceType.Namespace + "." + resourceType.Name + "\")");
            clrTypeClass.AppendLine("    {");
            clrTypeClass.AppendLine("    }");
            clrTypeClass.AppendLine();

            // provide a protected constructor for derived types
            if (resourceType.DerivedTypes.Any(t => t.Facets.IsClrType))
            {
                clrTypeClass.AppendLine("    protected " + resourceType.Name + "(string containerName, string typeName) : base(containerName, typeName)");
                clrTypeClass.AppendLine("    {");
                clrTypeClass.AppendLine("    }");
            }

            foreach (ResourceProperty property in resourceType.Properties.Where(rp => rp.Facets.IsClrProperty && rp.Facets.IsDeclaredProperty))
            {
                // skip those defined on a base type
                if (resourceType.BaseType != null && resourceType.BaseTypes.Any(baseType => baseType.Properties.Any(p => p.Name == property.Name && p.Facets.IsClrProperty)))
                    continue;

                string typeName, getBody;

                if (property.IsNavigation)
                {
                    if (property.OtherAssociationEnd.ResourceType.Facets.IsClrType)
                        typeName = property.OtherAssociationEnd.ResourceType.Name;
                    else
                        typeName = "RowEntityType";

                    if (property.Type is CollectionType)
                    {
                        if (typeName != "RowEntityType")
                        {
                            getBody = string.Join(Environment.NewLine, new string[]
                            {
                                "if( value == null )",
                                "   return new List<" + typeName + ">();",
                                "else",
                                "   return ((List<RowEntityType>)value).Cast<" + typeName + ">();"
                            });
                        }
                        else
                        {
                            getBody = string.Join(Environment.NewLine, new string[]
                            {
                                "if( value == null )",
                                "   return new List<RowEntityType>();",
                                "else",
                                "   return (List<RowEntityType>)value;"
                            });
                        }
                        typeName = "IEnumerable<" + typeName + ">";
                    }
                    else
                    {
                        getBody = string.Join(Environment.NewLine, new string[]
                        {
                            "if( value == null )",
                            "   return null;",
                            "else",
                            "   return (" + typeName + ")value;"
                        });
                    }
                }
                else
                {
                    if (property.Facets.Nullable && (property.Type.ClrType != null && (property.Type.ClrType.IsPrimitive || property.Type.ClrType.IsValueType)))
                    {
                        typeName = "Nullable<" + property.Type.ClrType.Name + ">";
                        getBody = string.Join(Environment.NewLine, new string[]
                        {
                            "if( value == null )",
                            "   return null;",
                            "else",
                            "   return (" + typeName + ")value;"
                        });
                    }
                    else
                    {
                        typeName = property.Type.ClrType.Name;
                        getBody = string.Join(Environment.NewLine, new string[]
                        {
                            "if( value == null )",
                            "   return this.GetDefaultValue<" + typeName + ">();",
                            "else",
                            "   return (" + typeName + ")value;"
                        });
                    }
                }

                clrTypeClass.AppendLine("    public " + typeName + " " + property.Name);
                clrTypeClass.AppendLine("    {");
                clrTypeClass.AppendLine("        get");
                clrTypeClass.AppendLine("        {");
                clrTypeClass.AppendLine("            object value =  this.GetValue(\"" + property.Name + "\");");
                clrTypeClass.AppendLine("            " + getBody);
                clrTypeClass.AppendLine("        }");
                clrTypeClass.AppendLine("    }");
                clrTypeClass.AppendLine();
            }

            clrTypeClass.AppendLine("}");

            return clrTypeClass.ToString();
        }

        public void GenerateServiceOperations(TemplateFile templateFile)
        {
            StringBuilder serviceOps = new StringBuilder();

            foreach (MethodInfo method in typeof(NonClrTestWebService<>).GetMethods())
            {
                object[] attribs = method.GetCustomAttributes(false);

                bool isSingleResult = false;
                foreach (object attrib in attribs)
                {
                    if (attrib is Microsoft.OData.Service.SingleResultAttribute)
                    {
                        isSingleResult = true;
                        break;
                    }
                }

                foreach (object attrib in attribs)
                {
                    if (attrib is System.ServiceModel.Web.WebGetAttribute || attrib is System.ServiceModel.Web.WebInvokeAttribute)
                    {
                        string kind;
                        if (typeof(IQueryable).IsAssignableFrom(method.ReturnType))
                        {
                            if (isSingleResult)
                            {
                                kind = "ServiceOperationResultKind.QueryWithSingleResult";
                            }
                            else
                            {
                                kind = "ServiceOperationResultKind.QueryWithMultipleResults";
                            }
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(method.ReturnType)
                            && !typeof(RowComplexType).IsAssignableFrom(method.ReturnType)
                            && method.ReturnType != typeof(string) && method.ReturnType != typeof(byte[]))
                        {
                            // this will be incorrect for complex types which derive from IEnumerable, but its the best we can do for now
                            kind = "ServiceOperationResultKind.Enumeration";
                        }
                        else if (typeof(void).IsAssignableFrom(method.ReturnType))
                        {
                            kind = "ServiceOperationResultKind.Void";
                        }
                        else
                        {
                            // either primitive or complex
                            kind = "ServiceOperationResultKind.DirectValue";
                        }

                        string verb;
                        if (attrib is System.ServiceModel.Web.WebGetAttribute)
                            verb = "GET";
                        else
                            verb = (attrib as System.ServiceModel.Web.WebInvokeAttribute).Method;

                        serviceOps.AppendLine("serviceOpCreateParams.Add(new ServiceOperationCreationParams(\"" + method.Name + "\", " + kind + ", \"\", \"" + verb + "\"));");
                        break;
                    }
                }
            }

            foreach (ServiceOperation op in _workspace.ServiceContainer.ResourceContainers.OfType<ServiceOperation>())
            {
                if (!op.ServiceOperationResultKind.HasValue)
                    AstoriaTestLog.FailAndThrow("Cannot generate code for service operation because the result kind is not set");
                if (op.ExpectedTypeName == null)
                    AstoriaTestLog.FailAndThrow("Cannot generate code for service operation because the expected type name is not set");

                string kind = "ServiceOperationResultKind." + op.ServiceOperationResultKind.Value.ToString();
                serviceOps.AppendLine("serviceOpCreateParams.Add(new ServiceOperationCreationParams(\"" + op.Name + "\", " + kind + ", \"" + op.BaseType.Name + "\", \"" + op.Verb.ToHttpMethod() + "\"));");
            }

            templateFile.FileText = templateFile.FileText.Replace("[[GeneratedServiceOperations]]", serviceOps.ToString());
        }
    }
}
