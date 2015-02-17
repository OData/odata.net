//---------------------------------------------------------------------
// <copyright file="StronglyTypedCodeLayerBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria;

namespace System.Data.Test.Astoria.ReflectionProvider
{
    public class StronglyTypedCodeLayerBuilder : CodeLayerBuilderBase
    {
        protected internal StronglyTypedCodeLayerBuilder(Workspace workspace, WorkspaceLanguage language, string codefilePath)
            :base(workspace,language,codefilePath)
        {
          
        }
        public override void Build()
        {
            //Write file header
            WriteFileHeader();

            //Write usings
            //WriteUsings()
            CodeBuilder.WriteUsing("System");
            CodeBuilder.WriteUsing("System.Linq");
            CodeBuilder.WriteUsing("System.Runtime.Serialization");
            CodeBuilder.WriteUsing("Microsoft.OData.Service");
            CodeBuilder.WriteUsing("Microsoft.OData.Client");
            CodeBuilder.WriteUsing("System.Collections.Generic");
            CodeBuilder.WriteUsing("System.Data.Test.Astoria.InMemoryLinq");

            CodeBuilder.WriteLine();

            //Write namespace
            CodeBuilder.WriteStartNamespace(Workspace.ContextNamespace);

            //Write DataContext Class
            WriteDataContextClass(this.Workspace.ServiceContainer);

            //Write ComplexType classes
            foreach (ComplexType t in Workspace.ServiceContainer.ComplexTypes)
            {
                WriteComplexTypeClass(t);
            }

            //Write EntityType classes
            foreach (ResourceType resType in Workspace.ServiceContainer.ResourceTypes)
            {
                WriteEntityTypeClass(resType);
            }

            CodeBuilder.WriteEndNamespace(Workspace.ContextNamespace);
        }

        protected virtual void WriteDataContextClass(ServiceContainer container)
        {
            foreach (ResourceAttribute att in container.Facets.Attributes)
                att.Apply(CodeBuilder);

            if (!Versioning.Server.SupportsV2Features && container.Workspace.ServiceModifications.Interfaces.IServiceProvider.Services.Any())
            {
                // we need to do something special, as V1 did not ask for IUpdatable from the service provider
#if !ClientSKUFramework
                // we include all interfaces defined in the V1 namespace that are not updatable (really this is just for the expand provider)
                CodeBuilder.WriteLine(ProviderWrapperGenerator.GenerateDerivedWrapper("InMemoryContextWrapper", typeof(InMemoryLinq.InMemoryContext),
                    container.Workspace.ServiceModifications.Interfaces.IServiceProvider.Services.Keys.ToArray()));
#endif
                CodeBuilder.WriteBeginClass(container.Workspace.ContextTypeName, "InMemoryContextWrapper", "Microsoft.OData.Service.IUpdatable", false);
            }
            else
            {
                if (container.Workspace.Settings.UpdatableImplementation == UpdatableImplementation.DataServiceUpdateProvider)
                    CodeBuilder.WriteBeginClass(container.Workspace.ContextTypeName, "InMemoryContext", "Microsoft.OData.Service.Providers.IDataServiceUpdateProvider", false);
                else
                    CodeBuilder.WriteBeginClass(container.Workspace.ContextTypeName, "InMemoryContext", "Microsoft.OData.Service.IUpdatable", false);
            }

            //Create the IQueryables
            foreach(ResourceContainer c in container.ResourceContainers)
            {
                if (c is ServiceOperation)
                    continue;
                CodeBuilder.CreateIQuerableGetProperty(c);
            }

            CodeBuilder.WriteEndClass(container.Name);
        }
        
        protected void WriteEntityTypeClass(ResourceType rt)
        {
            // write the key attribute
            CodeBuilder.WriteDataKeyAttribute(rt.Properties.OfType<ResourceProperty>().Where(rp => rp.PrimaryKey != null));

            // write other attributes
            foreach (ResourceAttribute att in rt.Facets.Attributes)
                att.Apply(CodeBuilder);

            //// friendly feeds attributes are sometimes on the properties!
            //foreach (NodeProperty p in rt.Properties)
            //{
            //    FriendlyFeedsAttribute attribute;
            //    if (FriendlyFeedsAttribute.TryGetAttribute(rt, p, out attribute))
            //    {
            //        FriendlyFeedsAttribute fromBase;
            //        if (rt.BaseType == null || !FriendlyFeedsAttribute.TryGetAttribute(rt.BaseType, p, out fromBase) || attribute != fromBase)
            //        {
            //            attribute.Apply(CodeBuilder);
            //        }
            //    }
            //}

            string baseType = null;
            if (rt.BaseType != null)
                baseType = rt.BaseType.Name;
            CodeBuilder.WriteBeginClass(rt.Name, baseType, null, rt.Facets.AbstractType);

            List<string> incrementers = new List<string>();
            foreach(ResourceProperty rp in rt.Properties.OfType<ResourceProperty>().Where(p => p.Facets.ServerGenerated))
            {
                if (rp.Type != Clr.Types.Int32)
                    AstoriaTestLog.FailAndThrow("Currently, we only code-gen server-generated ints");

                CodeBuilder.WriteLine("private static int _" + rp.Name + "_Counter = 0;");
                incrementers.Add(rp.Name);
            }

            if (incrementers.Any())
            {
                CodeBuilder.WriteLine("    public " + rt.Name + "()");
                CodeBuilder.WriteLine("    {");
                foreach(string propertyName in incrementers)
                    CodeBuilder.WriteLine("    this." + propertyName + " = _" + propertyName + "_Counter++;");
                CodeBuilder.WriteLine("    }");
            }

            // write out the properties
            foreach (ResourceProperty rp in rt.Properties.OfType<ResourceProperty>())
            {
                // if this property is not defined on the base type
                if (rt.BaseType == null || !rt.BaseType.Properties.Any(p => p.Name == rp.Name))
                {
                    if (rp.Facets.IsDeclaredProperty)
                    {
                        if (rp.IsNavigation)
                            CreateNavigationProperty(rp);
                        else
                            CreateValueProperty(rp);
                    }
                }
            }
            //Create a BiDirectionalRelationshipMap
            CodeBuilder.CreateNavigationMapMethod(rt);
            CodeBuilder.WriteEndClass(rt.Name);
            CodeBuilder.WriteLine();
        }

        protected void WriteComplexTypeClass(ComplexType ct)
        {
            CodeBuilder.WriteBeginClass(ct.Name, null, null, ct.Facets.AbstractType);

            foreach (ResourceProperty rp in ct.Properties.OfType<ResourceProperty>())
            {
                if(rp.Facets.IsDeclaredProperty)
                    CreateValueProperty(rp);
            }

            CodeBuilder.WriteEndClass(ct.Name);
        }

        protected void CreateValueProperty(ResourceProperty property)
        {
            string propertyType = null;
            if (property.Type.ClrType != null)
                propertyType = property.Type.ClrType.Namespace + "." + property.Type.ClrType.Name;
            else
                propertyType = property.Type.Name;

            if (property.Facets.Nullable && (property.Type.ClrType != null && (property.Type.ClrType.IsPrimitive || property.Type.ClrType.IsValueType)))
                propertyType = "Nullable<" + propertyType + ">";

            CodeBuilder.CreateFieldBackedProperty("_" + property.Name, property.Name, propertyType);
        }

        protected void CreateNavigationProperty(ResourceProperty navigationProperty)
        {
            string typeName="";
            if (navigationProperty.Type is ResourceType)
            {
                typeName = navigationProperty.Type.Name;
                CodeBuilder.CreateFieldBackedProperty("_" + navigationProperty.Name, navigationProperty.Name, typeName);
            }
            else
            {

                CollectionType collectionType = navigationProperty.Type as CollectionType;
                typeName = CodeBuilder.CreateListOfTType(collectionType.SubType.Name);
                CodeBuilder.CreateFieldBackedProperty(typeName, navigationProperty.Name, true);
            }
        }
    }
}
