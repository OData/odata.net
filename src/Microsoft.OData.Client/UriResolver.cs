//---------------------------------------------------------------------
// <copyright file="UriResolver.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData.Client
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// This immutable class is responsible for knowing how to correctly resolve base uri requests
    /// </summary>
    internal class UriResolver
    {
        /// <summary>The baseUri provided by the user. can be null.</summary>
        private readonly Uri baseUri;

        /// <summary>The function provided by the user to resolve the baseUri of entity sets. can be null</summary>
        private readonly Func<string, Uri> resolveEntitySet;

        /// <summary>base uri with guranteed trailing slash</summary>
        private readonly Uri baseUriWithSlash;

        /// <summary>
        /// Ctor for BaseUriResolver
        /// </summary>
        /// <param name="baseUri">The baseUri provided by the user.</param>
        /// <param name="resolveEntitySet">The function provider by the user to resolve the baseUri of the entity sets.</param>
        private UriResolver(Uri baseUri, Func<string, Uri> resolveEntitySet)
        {
            this.baseUri = baseUri;
            this.resolveEntitySet = resolveEntitySet;
            if (this.baseUri != null)
            {
                this.baseUriWithSlash = ForceSlashTerminatedUri(this.baseUri);
            }
        }

        /// <summary>Gets the ResolveEntitySet value.</summary>
        internal Func<string, Uri> ResolveEntitySet
        {
            get { return this.resolveEntitySet; }
        }

        /// <summary>
        /// This property returns the baseUri value with no validation at all
        ///
        /// NOTE: this property should only be used to show the user what the BaseUri value is, all
        ///       other access should go through the GetBaseUriWithSlash() method
        /// </summary>
        /// <value> the baseUri value </value>
        internal Uri RawBaseUriValue
        {
            get { return this.baseUri; }
        }

        /// <summary>
        /// Gets the base URI of the resolver regardless of whether or not it is null.
        /// </summary>
        internal Uri BaseUriOrNull
        {
            get { return this.baseUriWithSlash; }
        }

        /// <summary>
        /// Creates a UriResolver from a baseUri
        /// </summary>
        /// <param name="baseUri">The baseUri to use in the UriResolver</param>
        /// <param name="parameterName">The name of the paramter that the user passed the baseUri in from.</param>
        /// <returns>The new UriResolver using the passed in baseUri</returns>
        internal static UriResolver CreateFromBaseUri(Uri baseUri, string parameterName)
        {
            ConvertToAbsoluteAndValidateBaseUri(ref baseUri, parameterName);
            return new UriResolver(baseUri, null);
        }

        /// <summary>
        /// Creates a new BaseUriResolver with all the same values except a new BaseUri value
        /// </summary>
        /// <param name="overrideBaseUriValue">The new BaseUri value</param>
        /// <param name="parameterName">The name of the paramter that the user passed the baseUri in from.</param>
        /// <returns>A new BaseUriResolver with the BaseUri property set to the new value.</returns>
        internal UriResolver CloneWithOverrideValue(Uri overrideBaseUriValue, string parameterName)
        {
            ConvertToAbsoluteAndValidateBaseUri(ref overrideBaseUriValue, parameterName);
            return new UriResolver(overrideBaseUriValue, this.resolveEntitySet);
        }

        /// <summary>
        /// Creates a new BaseUriResolver with all the same values except a new ResolveEntitySet value
        /// </summary>
        /// <param name="overrideResolveEntitySetValue">The new BaseUri value</param>
        /// <returns>A new BaseUriResolver with the ResolveEntitySet property set to the new value.</returns>
        internal UriResolver CloneWithOverrideValue(Func<string, Uri> overrideResolveEntitySetValue)
        {
            return new UriResolver(this.baseUri, overrideResolveEntitySetValue);
        }

        /// <summary>base uri with no trailing slash</summary>
        /// <param name="entitySetName">the name of the entitSet whose Uri will be retrieved.</param>
        /// <returns>the baseUri ended with a slash for the entitySetName passed in.</returns>
        internal Uri GetEntitySetUri(string entitySetName)
        {
            Uri resolved = this.GetEntitySetUriFromResolver(entitySetName);
            if (resolved != null)
            {
                return ForceNonSlashTerminatedUri(resolved);
            }

            if (this.baseUriWithSlash != null)
            {
                return UriUtil.CreateUri(this.baseUriWithSlash, UriUtil.CreateUri(entitySetName, UriKind.Relative));
            }

            throw Error.InvalidOperation(Strings.Context_ResolveEntitySetOrBaseUriRequired(entitySetName));
        }

        /// <summary>
        /// returns the BaseUri property followed by a slash.
        ///
        /// if the BaseUri property is null, an InvalidOperationException is thrown
        /// </summary>
        /// <returns>The BaseUri property with a slash.</returns>
        internal Uri GetBaseUriWithSlash()
        {
            return this.GetBaseUriWithSlash(() => Strings.Context_BaseUriRequired);
        }

        /// <summary>
        /// If necessary will create an absolute uri by combining the BaseUri and requestUri
        /// </summary>
        /// <param name="requestUri">The uri specified by the user</param>
        /// <returns>An absolute Uri based on the requestUri and if nessesary the BaseUri</returns>
        internal Uri GetOrCreateAbsoluteUri(Uri requestUri)
        {
            Util.CheckArgumentNull(requestUri, "requestUri");
            if (!requestUri.IsAbsoluteUri)
            {
                return UriUtil.CreateUri(this.GetBaseUriWithSlash(() => Strings.Context_RequestUriIsRelativeBaseUriRequired), requestUri);
            }

            return requestUri;
        }

        /// <summary>
        /// Converts the baseUri passed in to an absolute Uri and then validates that it is
        /// usable by the system.
        /// </summary>
        /// <param name="baseUri">The user provided baseUri value.</param>
        /// <param name="parameterName">The name of the paramter that the user passed the baseUri in from.</param>
        private static void ConvertToAbsoluteAndValidateBaseUri(ref Uri baseUri, string parameterName)
        {
            baseUri = ConvertToAbsoluteUri(baseUri);
            if (!IsValidBaseUri(baseUri))
            {
                if (parameterName != null)
                {
                    throw Error.Argument(Strings.Context_BaseUri, parameterName);
                }

                throw Error.InvalidOperation(Strings.Context_BaseUri);
            }
        }

        /// <summary>
        /// Validates that the passed in BaseUri
        /// </summary>
        /// <param name="baseUri">the baseUri that needs to be validated</param>
        /// <returns>True if the baseUri is valid, otherwise false</returns>
        private static bool IsValidBaseUri(Uri baseUri)
        {
            if (baseUri == null)
            {
                return true;
            }

            if (!baseUri.IsAbsoluteUri ||
                !Uri.IsWellFormedUriString(UriUtil.UriToString(baseUri), UriKind.Absolute) ||
                !String.IsNullOrEmpty(baseUri.Query) ||
                !String.IsNullOrEmpty(baseUri.Fragment))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates a relative silverlight uri to an absolute uri
        /// </summary>
        /// <param name="baseUri">the uri passed by the client</param>
        /// <returns>the updated absolute uri</returns>
        private static Uri ConvertToAbsoluteUri(Uri baseUri)
        {
            if (baseUri == null)
            {
                return null;
            }

            return baseUri;
        }

        /// <summary>
        /// Returns a Uri that is not slash terminated
        ///
        /// Will be the passed in one if it is slash termination free, or a new one
        /// if the passed in one is slash terminated.
        /// </summary>
        /// <param name="uri">The Uri to be un slash terminated</param>
        /// <returns>A slash termination free version of the passed in Uri.</returns>
        private static Uri ForceNonSlashTerminatedUri(Uri uri)
        {
            Debug.Assert(uri.IsAbsoluteUri, "the uri must be an absolute uri");
            Debug.Assert(String.IsNullOrEmpty(uri.Query), "the uri must not have any query");
            Debug.Assert(String.IsNullOrEmpty(uri.Fragment), "the uri must not have any fragment");

            string uriString = UriUtil.UriToString(uri);
            if (uriString[uriString.Length - 1] == '/')
            {
                return UriUtil.CreateUri(uriString.Substring(0, uriString.Length - 1), UriKind.Absolute);
            }

            return uri;
        }

        /// <summary>
        /// Returns a slash terminated Uri.
        ///
        /// Will be the passed in one if it is already slash terminated, or a new one
        /// if the passed in one is not slash terminated.
        /// </summary>
        /// <param name="uri">The Uri to be slash terminated</param>
        /// <returns>A slash terminated version of the passed in Uri.</returns>
        private static Uri ForceSlashTerminatedUri(Uri uri)
        {
            Debug.Assert(uri.IsAbsoluteUri, "the uri must be an absolute uri");
            Debug.Assert(String.IsNullOrEmpty(uri.Query), "the uri must not have any query");
            Debug.Assert(String.IsNullOrEmpty(uri.Fragment), "the uri must not have any fragment");

            string uriString = UriUtil.UriToString(uri);
            if (uriString[uriString.Length - 1] != '/')
            {
                return UriUtil.CreateUri(uriString + "/", UriKind.Absolute);
            }

            return uri;
        }

        /// <summary>
        /// returns the BaseUri property followed by a slash.
        ///
        /// if the BaseUri property is null, an InvalidOperationException is thrown
        /// </summary>
        /// <param name="getErrorMessage">
        /// Returns the error message to use if the BaseUri is not available. Using a function so we only have to
        /// look up the resource if an error is actually thrown;
        /// </param>
        /// <returns>The BaseUri property with a slash.</returns>
        private Uri GetBaseUriWithSlash(Func<string> getErrorMessage)
        {
            if (this.baseUriWithSlash == null)
            {
                throw Error.InvalidOperation(getErrorMessage());
            }

            return this.baseUriWithSlash;
        }

        /// <summary>
        /// Gets a Uri from the ResolveEntitySet property if available
        /// </summary>
        /// <param name="entitySetName">The name of the entity set to resolve to a URI</param>
        /// <returns>An absolute URI for the entitySet or null</returns>
        private Uri GetEntitySetUriFromResolver(string entitySetName)
        {
            if (this.resolveEntitySet != null)
            {
                Uri resolved = this.resolveEntitySet(entitySetName);
                if (resolved != null)
                {
                    if (IsValidBaseUri(resolved))
                    {
                        return resolved;
                    }

                    throw Error.InvalidOperation(Strings.Context_ResolveReturnedInvalidUri);
                }
            }

            return null;
        }
    }
}