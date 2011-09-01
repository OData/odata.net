//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Data.Edm.Csdl.Internal.Serialization
{
    internal class EdmModelSchemaSeparationSerializationVisitor: EdmModelVisitor
    {
        private bool visitCompleted = false;
        private Dictionary<string, EdmSchema> modelSchemas = new Dictionary<string, EdmSchema>();
        private IEdmModel model;
        private EdmSchema activeSchema;

        public EdmModelSchemaSeparationSerializationVisitor(IEdmModel visitedModel)
        {
            this.model = visitedModel;
        }

        public IEnumerable<EdmSchema> GetSchemas()
        {
            if (!this.visitCompleted)
            {
                this.Visit();
            }

            return this.modelSchemas.Values;
        }

        protected void Visit()
        {
            this.VisitEdmModel(this.model);
            this.visitCompleted = true;
        }

        protected override void ProcessSchemaElement(IEdmSchemaElement element)
        {
            EdmSchema schema;
            if (!this.modelSchemas.TryGetValue(element.Namespace, out schema))
            {
                schema = new EdmSchema(element.Namespace);
                this.modelSchemas.Add(element.Namespace, schema);
            }
            
            schema.AddSchemaElement(element);
            this.activeSchema = schema;
            
            base.ProcessSchemaElement(element);
        }

        /// <summary>
        /// When we see an entity container, we see if it has <see cref="CsdlConstants.EntityContainerSchemaNamespaceAnnotation"/>.
        /// If it does, then we attach it to that schema, otherwise we attached to the first existing schema.
        /// If there are no schemas, we create the one named "Default" and attach container to it.
        /// </summary>
        /// <param name="element">The entity container being processed.</param>
        protected override void ProcessEntityContainer(IEdmEntityContainer element)
        {
            var containerSchemaNamespace = element.GetEntityContainerSchemaNamespace() ?? this.modelSchemas.Select(s => s.Key).FirstOrDefault() ?? "Default";

            EdmSchema containerSchema;
            if (!this.modelSchemas.TryGetValue(containerSchemaNamespace, out containerSchema))
            {
                containerSchema = new EdmSchema(containerSchemaNamespace);
                this.modelSchemas.Add(containerSchema.Namespace, containerSchema);
            }

            containerSchema.AddEntityContainer(element);
            this.activeSchema = containerSchema;

            base.ProcessEntityContainer(element);
        }

        protected override void ProcessComplexTypeReference(IEdmComplexTypeReference element)
        {
            this.CheckNamespaceReference(element.ComplexDefinition());
        }

        protected override void ProcessEntityTypeReference(IEdmEntityTypeReference element)
        {
            this.CheckNamespaceReference(element.EntityDefinition());
        }

        protected override void ProcessEntityReferenceTypeReference(IEdmEntityReferenceTypeReference element)
        {
            this.CheckNamespaceReference(element.EntityType());
        }

        protected override void ProcessEnumTypeReference(IEdmEnumTypeReference element)
        {
            this.CheckNamespaceReference(element.EnumDefinition());
        }

        protected override void ProcessEntityTypeDefinition(IEdmEntityType element)
        {
            base.ProcessEntityTypeDefinition(element);
            if (element.BaseType != null)
            {
                this.CheckNamespaceReference(element.BaseEntityType());
            }
        }

        protected override void ProcessComplexTypeDefinition(IEdmComplexType element)
        {
            base.ProcessComplexTypeDefinition(element);
            if (element.BaseType != null)
            {
                this.CheckNamespaceReference(element.BaseComplexType());
            }
        }

        protected override void ProcessEnumTypeDefinition(IEdmEnumType element)
        {
            base.ProcessEnumTypeDefinition(element);
            this.CheckNamespaceReference(element.UnderlyingType);
        }

        protected override void ProcessAssociationEnd(IEdmAssociationEnd element)
        {
            this.CheckNamespaceReference(element.EntityType);
        }

        protected override void ProcessAssociationSet(IEdmAssociationSet element)
        {
            this.CheckNamespaceReference(element.Association);
            base.ProcessAssociationSet(element);
        }

        protected override void ProcessAssociationSetEnd(IEdmAssociationSetEnd element)
        {
            this.CheckNamespaceReference(element.EntitySet.ElementType);
        }

        protected void CheckNamespaceReference(IEdmSchemaElement element)
        {
            if (this.activeSchema != null)
            {
                this.activeSchema.AddUsing(element.Namespace);
            }
        }
    }
}
