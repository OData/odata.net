//---------------------------------------------------------------------
// <copyright file="ODataResourceMetadataBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

#if ODATA_CLIENT
namespace Microsoft.OData.Client
#else
namespace Microsoft.OData.Evaluation
#endif
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Microsoft.OData.JsonLight;
    #endregion

    /// <summary>
    /// Extensibility point for customizing how OData entity metadata (edit-links, IDs, ETags, etc) is built.
    /// </summary>
    internal abstract class ODataResourceMetadataBuilder
    {
#if !ODATA_CLIENT
        /// <summary>
        /// Gets an instance of the metadata builder which never returns anything other than nulls.
        /// </summary>
        internal static ODataResourceMetadataBuilder Null
        {
            get
            {
                return NullResourceMetadataBuilder.Instance;
            }
        }
#endif

        /// <summary>
        /// Gets instance of the metadata builder which belongs to the parent odata resource
        /// </summary>
        /// <returns>
        /// The metadata builder of the parent odata resource
        /// Or null if there is no parent odata resource
        /// </returns>
        internal ODataResourceMetadataBuilder ParentMetadataBuilder { get; set; }

        /// <summary>
        /// Whether the resource is from collection.
        /// </summary>
        internal bool IsFromCollection { get; set; }

        /// <summary>
        /// The name of property that current resource represents.
        /// </summary>
        internal string NameAsProperty { get; set; }

        /// <summary>
        /// Gets the edit link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the edit link for the entity.
        /// Or null if it is not possible to determine the edit link.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal abstract Uri GetEditLink();

        /// <summary>
        /// Gets the read link of the entity.
        /// </summary>
        /// <returns>
        /// The absolute URI of the read link for the entity.
        /// Or null if it is not possible to determine the read link.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal abstract Uri GetReadLink();

        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        /// <returns>
        /// The ID for the entity.
        /// Or null if it is not possible to determine the ID.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal abstract Uri GetId();

        /// <summary>
        /// Get the id that need to be written into wire
        /// </summary>
        /// <param name="id">The id return to the caller</param>
        /// <returns>
        /// If writer should write odata.id property into wire
        /// </returns>
        internal abstract bool TryGetIdForSerialization(out Uri id);

        /// <summary>
        /// Gets the ETag of the entity.
        /// </summary>
        /// <returns>
        /// The ETag for the entity.
        /// Or null if it is not possible to determine the ETag.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal abstract string GetETag();

#if !ODATA_CLIENT
        /// <summary>
        /// Gets the default media resource of the entity.
        /// </summary>
        /// <returns>
        /// The the default media resource of the entity.
        /// Or null if the entity is not an MLE.
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal virtual ODataStreamReferenceValue GetMediaResource()
        {
            return null;
        }

        /// <summary>
        /// Gets the entity properties.
        /// </summary>
        /// <param name="nonComputedProperties">Non-computed properties from the entity.</param>
        /// <returns>The the computed and non-computed entity properties.</returns>
        internal virtual IEnumerable<ODataProperty> GetProperties(IEnumerable<ODataProperty> nonComputedProperties)
        {
            return nonComputedProperties == null ? null : nonComputedProperties.Where(p => !(p.ODataValue is ODataStreamReferenceValue));
        }

        /// <summary>
        /// Gets the list of computed and non-computed actions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed actions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal virtual IEnumerable<ODataAction> GetActions()
        {
            return null;
        }

        /// <summary>
        /// Gets the list of computed and non-computed functions for the entity.
        /// </summary>
        /// <returns>The list of computed and non-computed functions for the entity.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal virtual IEnumerable<ODataFunction> GetFunctions()
        {
            return null;
        }

        /// <summary>
        /// Marks the given nested resource info as processed.
        /// </summary>
        /// <param name="navigationPropertyName">The nested resource info we've already processed.</param>
        internal virtual void MarkNestedResourceInfoProcessed(string navigationPropertyName)
        {
        }

        /// <summary>
        /// Returns the next unprocessed nested resource info or null if there's no more navigation links to process.
        /// </summary>
        /// <returns>Returns the next unprocessed nested resource info or null if there's no more navigation links to process.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "A method for consistency with the rest of the API.")]
        internal virtual ODataJsonLightReaderNestedResourceInfo GetNextUnprocessedNavigationLink()
        {
            return null;
        }
#endif

        /// <summary>
        /// Gets the edit link of a stream value.
        /// </summary>
        /// <param name="streamPropertyName">The name of the stream property the edit link is computed for;
        /// or null for the default media resource.</param>
        /// <returns>
        /// The absolute URI of the edit link for the specified stream property or the default media resource.
        /// Or null if it is not possible to determine the stream edit link.
        /// </returns>
        internal virtual Uri GetStreamEditLink(string streamPropertyName)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNotEmpty(streamPropertyName, "streamPropertyName");
#else
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");
#endif
            return null;
        }

        /// <summary>
        /// Gets the read link of a stream value.
        /// </summary>
        /// <param name="streamPropertyName">The name of the stream property the read link is computed for;
        /// or null for the default media resource.</param>
        /// <returns>
        /// The absolute URI of the read link for the specified stream property or the default media resource.
        /// Or null if it is not possible to determine the stream read link.
        /// </returns>
        internal virtual Uri GetStreamReadLink(string streamPropertyName)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNotEmpty(streamPropertyName, "streamPropertyName");
#else
            ExceptionUtils.CheckArgumentStringNotEmpty(streamPropertyName, "streamPropertyName");
#endif

            return null;
        }

        /// <summary>
        /// Gets the navigation link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the navigation link URI for.</param>
        /// <param name="navigationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasNestedResourceInfoUrl">true if the value of the <paramref name="navigationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null navigation link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The navigation link URI for the navigation property.
        /// null if its not possible to determine the navigation link for the specified navigation property.
        /// </returns>
        internal virtual Uri GetNavigationLinkUri(string navigationPropertyName, Uri navigationLinkUrl, bool hasNestedResourceInfoUrl)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNullAndEmpty(navigationPropertyName, "navigationPropertyName");
#else
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");
#endif
            return null;
        }

        /// <summary>
        /// Gets the association link URI for the specified navigation property.
        /// </summary>
        /// <param name="navigationPropertyName">The name of the navigation property to get the association link URI for.</param>
        /// <param name="associationLinkUrl">The value of the link URI as seen on the wire or provided explicitly by the user or previously returned by the metadata builder, which may be null.</param>
        /// <param name="hasAssociationLinkUrl">true if the value of the <paramref name="associationLinkUrl"/> was seen on the wire or provided explicitly by the user or previously returned by
        /// the metadata builder, false otherwise. This flag allows the metadata builder to determine whether a null association link url is an uninitialized value or a value that was set explicitly.</param>
        /// <returns>
        /// The association link URI for the navigation property.
        /// null if its not possible to determine the association link for the specified navigation property.
        /// </returns>
        internal virtual Uri GetAssociationLinkUri(string navigationPropertyName, Uri associationLinkUrl, bool hasAssociationLinkUrl)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNullAndEmpty(navigationPropertyName, "navigationPropertyName");
#else
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(navigationPropertyName, "navigationPropertyName");
#endif
            return null;
        }

        /// <summary>
        /// Get the operation target URI for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <param name="bindingParameterTypeName">The binding parameter type name to include in the target, or null/empty if there is none.</param>
        /// <param name="parameterNames">The parameter names to include in the target, or null/empty if there is none.</param>
        /// <returns>
        /// The target URI for the operation.
        /// null if it is not possible to determine the target URI for the specified operation.
        /// </returns>
        internal virtual Uri GetOperationTargetUri(string operationName, string bindingParameterTypeName, string parameterNames)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNullAndEmpty(operationName, "operationName");
#else
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");
#endif
            return null;
        }

        /// <summary>
        /// Get the operation title for the specified <paramref name="operationName"/>.
        /// </summary>
        /// <param name="operationName">The fully qualified name of the operation for which to get the target URI.</param>
        /// <returns>
        /// The title for the operation.
        /// null if it is not possible to determine the title for the specified operation.
        /// </returns>
        internal virtual string GetOperationTitle(string operationName)
        {
#if ODATA_CLIENT
            Util.CheckArgumentNullAndEmpty(operationName, "operationName");
#else
            ExceptionUtils.CheckArgumentStringNotNullOrEmpty(operationName, "operationName");
#endif
            return null;
        }

#if !ODATA_CLIENT
        /// <summary>
        /// Implementation of the metadata builder which only returns nulls.
        /// </summary>
        private sealed class NullResourceMetadataBuilder : ODataResourceMetadataBuilder
        {
            /// <summary>
            /// Singleton instance of the null metadata builder.
            /// </summary>
            internal static readonly NullResourceMetadataBuilder Instance = new NullResourceMetadataBuilder();

            /// <summary>
            /// Prevents a default instance of the <see cref="NullResourceMetadataBuilder"/> class from being created.
            /// </summary>
            private NullResourceMetadataBuilder()
            {
            }

            /// <summary>
            /// Gets the edit link of the entity.
            /// </summary>
            /// <returns>
            /// The absolute URI of the edit link for the entity.
            /// Or null if it is not possible to determine the edit link.
            /// </returns>
            internal override Uri GetEditLink()
            {
                return null;
            }

            /// <summary>
            /// Gets the read link of the entity.
            /// </summary>
            /// <returns>
            /// The absolute URI of the read link for the entity.
            /// Or null if it is not possible to determine the read link.
            /// </returns>
            internal override Uri GetReadLink()
            {
                return null;
            }

            /// <summary>
            /// Gets the ID of the entity.
            /// </summary>
            /// <returns>
            /// The ID for the entity.
            /// Or null if it is not possible to determine the ID.
            /// </returns>
            internal override Uri GetId()
            {
                return null;
            }

            /// <summary>
            /// Gets the ETag of the entity.
            /// </summary>
            /// <returns>
            /// The ETag for the entity.
            /// Or null if it is not possible to determine the ETag.
            /// </returns>
            internal override string GetETag()
            {
                return null;
            }

            /// <summary>
            /// Get the id that need to be written into wire
            /// </summary>
            /// <param name="id">The id return to the caller</param>
            /// <returns>
            /// If writer should write odata.id property into wire
            /// </returns>
            internal override bool TryGetIdForSerialization(out Uri id)
            {
                id = null;
                return false;
            }
        }
#endif
    }
}
