//   OData .NET Libraries ver. 5.6.2
//   Copyright (c) Microsoft Corporation. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Library.Internal;

namespace Microsoft.Data.Edm.Internal
{
    internal static class RegistrationHelper
    {
        internal static void RegisterSchemaElement(IEdmSchemaElement element, Dictionary<string, IEdmSchemaType> schemaTypeDictionary, Dictionary<string, IEdmValueTerm> valueTermDictionary, Dictionary<string, object> functionGroupDictionary, Dictionary<string, IEdmEntityContainer> containerDictionary)
        {
            string qualifiedName = element.FullName();
            switch (element.SchemaElementKind)
            {
                case EdmSchemaElementKind.Function:
                    AddFunction((IEdmFunction)element, qualifiedName, functionGroupDictionary);
                    break;
                case EdmSchemaElementKind.TypeDefinition:
                    AddElement((IEdmSchemaType)element, qualifiedName, schemaTypeDictionary, CreateAmbiguousTypeBinding);
                    break;
                case EdmSchemaElementKind.ValueTerm:
                    AddElement((IEdmValueTerm)element, qualifiedName, valueTermDictionary, CreateAmbiguousValueTermBinding);
                    break;
                case EdmSchemaElementKind.EntityContainer:
                    // Add EntityContainers to the dictionary twice to maintian backwards compat with Edms that did not consider EntityContainers to be schema elements.
                    AddElement((IEdmEntityContainer)element, qualifiedName, containerDictionary, CreateAmbiguousEntityContainerBinding);
                    AddElement((IEdmEntityContainer)element, element.Name, containerDictionary, CreateAmbiguousEntityContainerBinding);
                    break;
                case EdmSchemaElementKind.None:
                    throw new InvalidOperationException(Edm.Strings.EdmModel_CannotUseElementWithTypeNone);
                default:
                    throw new InvalidOperationException(Edm.Strings.UnknownEnumVal_SchemaElementKind(element.SchemaElementKind));
            }
        }

        internal static void RegisterProperty(IEdmProperty element, string name, Dictionary<string, IEdmProperty> dictionary)
        {
            AddElement(element, name, dictionary, CreateAmbiguousPropertyBinding);
        }

        internal static void AddElement<T>(T element, string name, Dictionary<string, T> elementDictionary, Func<T, T, T> ambiguityCreator) where T : class, IEdmElement
        {
            T preexisting;
            if (elementDictionary.TryGetValue(name, out preexisting))
            {
                elementDictionary[name] = ambiguityCreator(preexisting, element);
            }
            else
            {
                elementDictionary[name] = element;
            }
        }

        internal static void AddFunction<T>(T function, string name, Dictionary<string, object> functionListDictionary) where T : class, IEdmFunctionBase
        {
            object preexisting = null;
            if (functionListDictionary.TryGetValue(name, out preexisting))
            {
                List<T> functionList = preexisting as List<T>;
                if (functionList == null)
                {
                    T existingFunction = (T)preexisting;
                    functionList = new List<T>();
                    functionList.Add(existingFunction);
                    functionListDictionary[name] = functionList;
                }

                functionList.Add(function);
            }
            else
            {
                functionListDictionary[name] = function;
            }
        }

        internal static IEdmSchemaType CreateAmbiguousTypeBinding(IEdmSchemaType first, IEdmSchemaType second)
        {
            var ambiguous = first as AmbiguousTypeBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousTypeBinding(first, second);
        }

        internal static IEdmValueTerm CreateAmbiguousValueTermBinding(IEdmValueTerm first, IEdmValueTerm second)
        {
            var ambiguous = first as AmbiguousValueTermBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousValueTermBinding(first, second);
        }

        internal static IEdmEntitySet CreateAmbiguousEntitySetBinding(IEdmEntitySet first, IEdmEntitySet second)
        {
            var ambiguous = first as AmbiguousEntitySetBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousEntitySetBinding(first, second);
        }

        internal static IEdmEntityContainer CreateAmbiguousEntityContainerBinding(IEdmEntityContainer first, IEdmEntityContainer second)
        {
            var ambiguous = first as AmbiguousEntityContainerBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousEntityContainerBinding(first, second);
        }

        private static IEdmProperty CreateAmbiguousPropertyBinding(IEdmProperty first, IEdmProperty second)
        {
            var ambiguous = first as AmbiguousPropertyBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousPropertyBinding(first.DeclaringType, first, second);
        }
    }
}
