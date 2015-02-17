//---------------------------------------------------------------------
// <copyright file="UpdatePayloadBuilder.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Data.Test.Astoria.Util;
    using System.Linq;

    public abstract class UpdatePayloadBuilder 
    {
#if !SilverlightTestFramework
        public static UpdatePayloadBuilder CreateUpdatePayloadBuilder(Workspace w, SerializationFormatKind format, RequestVerb requestVerb)
        {
            switch (format)
            {
                case SerializationFormatKind.Atom:
                case SerializationFormatKind.Default:
                case SerializationFormatKind.PlainXml:
                    return new AtomUpdatePayloadBuilder(w, requestVerb);
                case SerializationFormatKind.JSON:
                    return new JsonUpdatePayloadBuilder(w, requestVerb);
                default:
                    throw new ArgumentException("Could not create a payload builder for format: " + format.ToString());
            }
        }
#endif

        RequestVerb _requestVerb;
        private Workspace _workspace;

        public UpdatePayloadBuilder(Workspace workspace, RequestVerb requestVerb)
        {
            _workspace = workspace;
            _requestVerb = requestVerb;
        }
        public RequestVerb RequestVerb
        {
            get { return _requestVerb; }
        }
        public Workspace Workspace
        {
            get { return _workspace; }
        }
        #region TraceLog code
        #endregion 
        public abstract string Build(ExpNode node);
        
        protected string CreateCanonicalUri(ResourceInstanceKey key)
        {
            UriQueryBuilder builder = new UriQueryBuilder(Workspace, Workspace.ServiceUri);
            builder.UseBinaryFormatForDates = false;
            builder.CleanUpSpecialCharacters = false;

            KeyExpression keyExpression = ResourceInstanceUtil.ConvertToKeyExpression(key, Workspace);
            if (keyExpression != null)
            {
                QueryNode query = ContainmentUtil.BuildCanonicalQuery(keyExpression);
                return builder.Build(query);
            }
             
            IEnumerable<ResourceInstanceSimpleProperty> properties = key.KeyProperties.OfType<ResourceInstanceSimpleProperty>();

            string keyString = builder.CreateKeyString(properties.Select(p => p.Name).ToArray(), properties.Select(p => p.PropertyValue).ToArray());
            string uri = Workspace.ServiceUri + "/" + key.ResourceSetName + "(" + keyString + ")";

            return uri;
        }
    }
}

// "{ __metadata: { type: \"northwind.Region\" }, RegionId: 55, RegionDesctripyion: \"foo\" }"