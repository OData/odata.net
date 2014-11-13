//   OData .NET Libraries ver. 5.6.3
//   Copyright (c) Microsoft Corporation
//   All rights reserved. 
//   MIT License
//   Permission is hereby granted, free of charge, to any person obtaining a copy of
//   this software and associated documentation files (the "Software"), to deal in
//   the Software without restriction, including without limitation the rights to use,
//   copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the
//   Software, and to permit persons to whom the Software is furnished to do so,
//   subject to the following conditions:

//   The above copyright notice and this permission notice shall be included in all
//   copies or substantial portions of the Software.

//   THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
//   FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
//   COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
//   IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
//   CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#if ASTORIA_SERVER 
namespace System.Data.Services.Providers
{
    using System.Data.Services;
#else
namespace System.Data.EntityModel.Emitters
{
    using System.Data.Services.Design;
    using System.Data.Services.Providers;
#endif
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Data.Services.Common;
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// This class contains code for translating epm information stored in Metadata properties to objects of EpmPropertyInformation class
    /// !!! THIS CODE IS USED BY System.Data.Services.Providers.ObjectContextProvider *AND* System.Data.EntityModel.Emitters CLASSES !!!
    /// </summary>
    internal static class EpmHelper
    {
        /// <summary>
        /// Gets EPM information for a property of a type
        /// </summary>
        /// <param name="edmMember">Member that could contain EPM information</param>
        /// <returns>IEnumerable of EPM information for the member, if any.</returns>
        internal static IEnumerable<EpmPropertyInformation> GetEpmInformationFromProperty(EdmMember edmMember)
        {
            return GetEpmPropertyInformation(edmMember, edmMember.DeclaringType.Name, edmMember.Name);
        }

        /// <summary>
        /// Gets EPM information for a type
        /// </summary>
        /// <param name="structuralType">Type that could contain EPM information</param>
        /// <returns>IEnumerable of EPM information for the type, if any.</returns>
        internal static IEnumerable<EpmPropertyInformation> GetEpmInformationFromType(StructuralType structuralType)
        {
            return GetEpmPropertyInformation(structuralType, structuralType.Name, null);
        }

        /// <summary>
        /// Obtains the epm information for a single property by reading csdl content
        /// </summary>
        /// <param name="metadataItem">StructuralType or EdmMember to get EPM information for</param>
        /// <param name="typeName">Type for which we are reading the metadata properties</param>
        /// <param name="memberName">Member for which we are reading the metadata properties. Can be null if reading from a type instead of a member.</param>
        /// <returns>EpmPropertyInformation corresponding to read metadata properties</returns>
        private static IEnumerable<EpmPropertyInformation> GetEpmPropertyInformation(MetadataItem metadataItem, string typeName, string memberName)
        {
            Debug.Assert(metadataItem is StructuralType || metadataItem is EdmMember, "Expected to search for EPM information only on StructuralType or EdmMember");

            EpmAttributeNameBuilder epmAttributeNameBuilder = new EpmAttributeNameBuilder();

            while (true)
            {
                bool pathGiven = true;

                // EpmTargetPath is the only non-optional EPM attribute. If it is declared we need to take care of mapping.
                MetadataProperty epmTargetPathProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmTargetPath);

                if (epmTargetPathProperty != null)
                {
                    // By default, we keep the copy in content for backwards compatibility
                    bool epmKeepInContent = true;

                    MetadataProperty epmKeepInContentProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmKeepInContent);
                    if (epmKeepInContentProperty != null)
                    {
                        if (!Boolean.TryParse(Convert.ToString(epmKeepInContentProperty.Value, CultureInfo.InvariantCulture), out epmKeepInContent))
                        {
                            throw new InvalidOperationException(memberName == null ?
                                    Strings.ObjectContext_InvalidValueForEpmPropertyType(epmAttributeNameBuilder.EpmKeepInContent, typeName) :
                                    Strings.ObjectContext_InvalidValueForEpmPropertyMember(epmAttributeNameBuilder.EpmKeepInContent, memberName, typeName));
                        }
                    }

                    MetadataProperty epmSourcePathProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmSourcePath);
                    String epmSourcePath;
                    if (epmSourcePathProperty == null)
                    {
                        if (memberName == null)
                        {
                            throw new InvalidOperationException(Strings.ObjectContext_MissingExtendedAttributeType(epmAttributeNameBuilder.EpmSourcePath, typeName));
                        }

                        pathGiven = false;
                        epmSourcePath = memberName;
                    }
                    else
                    {
                        epmSourcePath = Convert.ToString(epmSourcePathProperty.Value, CultureInfo.InvariantCulture);
                    }

                    String epmTargetPath = Convert.ToString(epmTargetPathProperty.Value, CultureInfo.InvariantCulture);

                    // if the property is not a sydication property MapEpmTargetPathToSyndicationProperty
                    // will return SyndicationItemProperty.CustomProperty 
                    SyndicationItemProperty targetSyndicationItem = EpmTranslate.MapEpmTargetPathToSyndicationProperty(epmTargetPath);

                    MetadataProperty epmContentKindProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmContentKind);

                    MetadataProperty epmNsPrefixProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmNsPrefix);

                    MetadataProperty epmNsUriProperty = EdmUtil.FindExtendedProperty(metadataItem, epmAttributeNameBuilder.EpmNsUri);

                    // ContentKind is mutually exclusive with NsPrefix and NsUri properties
                    if (epmContentKindProperty != null)
                    {
                        if (epmNsPrefixProperty != null || epmNsUriProperty != null)
                        {
                            string epmPropertyName = epmNsPrefixProperty != null ? epmAttributeNameBuilder.EpmNsPrefix : epmAttributeNameBuilder.EpmNsUri;

                            throw new InvalidOperationException(memberName == null ?
                                            Strings.ObjectContext_InvalidAttributeForNonSyndicationItemsType(epmPropertyName, typeName) :
                                            Strings.ObjectContext_InvalidAttributeForNonSyndicationItemsMember(epmPropertyName, memberName, typeName));
                        }
                    }
                    
                    // epmNsPrefixProperty and epmNsUriProperty can be non-null only for non-Atom mapping. Since they are optional we need to check
                    // if it was possible to map the target path to a syndication item name. if it was not (i.e. targetSyndicationItem == SyndicationItemProperty.CustomProperty) 
                    // this is a non-Atom kind of mapping.
                    if (epmNsPrefixProperty != null || epmNsUriProperty != null || targetSyndicationItem == SyndicationItemProperty.CustomProperty)
                    {
                        String epmNsPrefix = epmNsPrefixProperty != null ? Convert.ToString(epmNsPrefixProperty.Value, CultureInfo.InvariantCulture) : null;
                        String epmNsUri = epmNsUriProperty != null ? Convert.ToString(epmNsUriProperty.Value, CultureInfo.InvariantCulture) : null;
                        yield return new EpmPropertyInformation
                                            {
                                                IsAtom = false,
                                                KeepInContent = epmKeepInContent,
                                                SourcePath = epmSourcePath,
                                                PathGiven = pathGiven,
                                                TargetPath = epmTargetPath,
                                                NsPrefix = epmNsPrefix,
                                                NsUri = epmNsUri
                                            };
                    }
                    else
                    {
                        SyndicationTextContentKind syndicationContentKind;

                        if (epmContentKindProperty != null)
                        {
                            syndicationContentKind = EpmTranslate.MapEpmContentKindToSyndicationTextContentKind(
                                                        Convert.ToString(epmContentKindProperty.Value, CultureInfo.InvariantCulture),
                                                        typeName,
                                                        memberName);
                        }
                        else
                        {
                            syndicationContentKind = SyndicationTextContentKind.Plaintext;
                        }

                        yield return new EpmPropertyInformation
                                            {
                                                IsAtom = true,
                                                KeepInContent = epmKeepInContent,
                                                SourcePath = epmSourcePath,
                                                PathGiven = pathGiven,
                                                SyndicationItem = targetSyndicationItem,
                                                ContentKind = syndicationContentKind,
                                            };
                    }

                    epmAttributeNameBuilder.MoveNext();
                }
                else
                {
                    yield break;
                }
            }
        }

        /// <summary>
        /// Class for holding de-serialized Epm attribute from csdl file
        /// </summary>
        internal sealed class EpmPropertyInformation
        {
            /// <summary>Syndication mapping or custom mapping</summary>
            internal bool IsAtom
            {
                get;
                set;
            }

            /// <summary>KeepInContent</summary>
            internal bool KeepInContent
            {
                get;
                set;
            }

            /// <summary>SourcePath</summary>
            internal String SourcePath
            {
                get;
                set;
            }

            /// <summary>Was path provided or inferred</summary>
            internal bool PathGiven
            {
                get;
                set;
            }

            /// <summary>TargetPath</summary>
            internal String TargetPath
            {
                get;
                set;
            }

            /// <summary>Target syndication item when IsAtom is true</summary>
            internal SyndicationItemProperty SyndicationItem
            {
                get;
                set;
            }

            /// <summary>Target syndication item content kind when IsAtom is true</summary>
            internal SyndicationTextContentKind ContentKind
            {
                get;
                set;
            }

            /// <summary>Namespace prefix when IsAtom is false</summary>
            internal String NsPrefix
            {
                get;
                set;
            }

            /// <summary>Namespace Uri when IsAtom is false</summary>
            internal String NsUri
            {
                get;
                set;
            }
        }
    }
}
