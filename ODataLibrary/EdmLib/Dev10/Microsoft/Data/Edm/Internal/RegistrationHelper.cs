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

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Edm.Library;

namespace Microsoft.Data.Edm.Internal
{
    internal static class RegistrationHelper
    {
        internal static void RegisterSchemaElement(IEdmSchemaElement element, Dictionary<string, IEdmSchemaType> schemaTypeDictionary, Dictionary<string, IEdmValueTerm> valueTermDictionary, Dictionary<string, IEdmAssociation> associationDictionary, Dictionary<string, object> functionGroupDictionary)
        {
            string name = element.FullName();

            IEdmFunction function = element as IEdmFunction;
            if (function != null)
            {
                AddFunction(function, name, functionGroupDictionary);
            }
            else
            {
                IEdmSchemaType schemaType = element as IEdmSchemaType;
                if (schemaType != null)
                {
                    AddElement(schemaType, name, schemaTypeDictionary, CreateAmbiguousTypeBinding);
                }
                else
                {
                    IEdmAssociation association = element as IEdmAssociation;
                    if (association != null)
                    {
                        AddElement(association, name, associationDictionary, CreateAmbiguousAssociationBinding);
                    }
                    else
                    {
                        AddElement((IEdmValueTerm)element, name, valueTermDictionary, CreateAmbiguousValueTermBinding);
                    }
                }
            }
        }

        internal static void UnregisterSchemaElement(IEdmSchemaElement element, Dictionary<string, IEdmSchemaType> schemaTypeDictionary, Dictionary<string, IEdmValueTerm> valueTermDictionary, Dictionary<string, IEdmAssociation> associationDictionary, Dictionary<string, object> functionGroupDictionary)
        {
            string name = element.FullName();

            IEdmFunction function = element as IEdmFunction;
            if (function != null)
            {
                RemoveFunction(function, name, functionGroupDictionary);
            }
            else
            {
                IEdmSchemaType schemaType = element as IEdmSchemaType;
                if (schemaType != null)
                {
                    RemoveElement(schemaType, name, schemaTypeDictionary);
                }
                else
                {
                    IEdmAssociation association = element as IEdmAssociation;
                    if (association != null)
                    {
                        RemoveElement(association, name, associationDictionary);
                    }
                    else
                    {
                        RemoveElement((IEdmValueTerm)element, name, valueTermDictionary);
                    }
                }
            }
        }

        internal static void RegisterEntityContainerElement(IEdmEntityContainerElement element, string name, Dictionary<string, object> functionImportDictionary, Dictionary<string, IEdmEntitySet> entitySetDictionary, Dictionary<string, IEdmAssociationSet> associationSetDictionary)
        {
            IEdmFunctionImport functionImport = element as IEdmFunctionImport;
            if (functionImport != null)
            {
                AddFunction(functionImport, element.Name, functionImportDictionary);
            }
            else
            {
                IEdmEntitySet entitySet = element as IEdmEntitySet;
                if (entitySet != null)
                {
                    AddElement(entitySet, element.Name, entitySetDictionary, CreateAmbiguousEntitySetBinding);
                }
                else
                {
                    IEdmAssociationSet associationSet = element as IEdmAssociationSet;
                    if (entitySet != null)
                    {
                        AddElement(associationSet, element.Name, associationSetDictionary, CreateAmbiguousAssociationSetBinding);
                    }
                }
            }
        }

        internal static void UnregisterEntityContainerElement(IEdmEntityContainerElement element, string name, Dictionary<string, object> functionImportDictionary, Dictionary<string, IEdmEntitySet> entitySetDictionary, Dictionary<string, IEdmAssociationSet> associationSetDictionary)
        {
            IEdmFunctionImport functionImport = element as IEdmFunctionImport;
            if (functionImport != null)
            {
                RemoveFunction(functionImport, element.Name, functionImportDictionary);
            }
            else
            {
                IEdmEntitySet entitySet = element as IEdmEntitySet;
                if (entitySet != null)
                {
                    RemoveElement(entitySet, name, entitySetDictionary);
                }
                else
                {
                    IEdmAssociationSet associationSet = element as IEdmAssociationSet;
                    if (associationSet != null)
                    {
                        RemoveElement(associationSet, name, associationSetDictionary);
                    }
                }
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

        internal static void RemoveElement<T>(T element, string name, Dictionary<string, T> elementDictionary) where T : class, IEdmNamedElement
        {
            T preexisting;
            if (elementDictionary.TryGetValue(name, out preexisting))
            {
                AmbiguousBinding<T> ambiguous = preexisting as AmbiguousBinding<T>;
                if (ambiguous != null)
                {
                    elementDictionary[name] = ambiguous.RemoveBinding(element);
                }
                else
                {
                    elementDictionary.Remove(name);
                }
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

        internal static void RemoveFunction<T>(T function, string name, Dictionary<string, object> functionListDictionary) where T : class, IEdmFunctionBase
        {
            object preexisting = null;
            if (functionListDictionary.TryGetValue(name, out preexisting))
            {
                List<T> functionList = preexisting as List<T>;
                if (functionList != null)
                {
                    functionList.Remove(function);
                    if (functionList.Count == 1)
                    {
                        functionListDictionary[name] = functionList.First();
                    }
                }
                else
                {
                    functionListDictionary.Remove(name);
                }
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

        internal static IEdmAssociation CreateAmbiguousAssociationBinding(IEdmAssociation first, IEdmAssociation second)
        {
            var ambiguous = first as AmbiguousAssociationBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousAssociationBinding(first, second);
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

        internal static IEdmAssociationSet CreateAmbiguousAssociationSetBinding(IEdmAssociationSet first, IEdmAssociationSet second)
        {
            var ambiguous = first as AmbiguousAssociationSetBinding;
            if (ambiguous != null)
            {
                ambiguous.AddBinding(second);
                return ambiguous;
            }

            return new AmbiguousAssociationSetBinding(first, second);
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
