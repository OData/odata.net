//---------------------------------------------------------------------
// <copyright file="ForceRawMessageFormatBinding.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.WCFService
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.ServiceModel.Configuration;

    /// <summary>
    /// Custom WCF binding classes used to force raw content format. This allows us to use the same endpoint for both
    /// XML & JSON since we have our own deserializers.
    /// </summary>
    /// <remarks>See http://blogs.msdn.com/b/carlosfigueira/archive/2008/04/17/wcf-raw-programming-model-receiving-arbitrary-data.aspx for more information.</remarks>
    public class ForceRawMessageFormatBinding : CustomBinding
    {
        public ForceRawMessageFormatBinding() : base(new WebHttpBinding())
        {
            var encodingBindingElement = this.Elements.Find<WebMessageEncodingBindingElement>();
            encodingBindingElement.ContentTypeMapper = new ForceRawMessageFormatMapper();
        }
    }

    public class ForceRawMessageFormatBindingElement : StandardBindingElement
    {
        protected override Type BindingElementType
        {
            get { return typeof(ForceRawMessageFormatBinding); }
        }

        protected override void OnApplyConfiguration(Binding binding)
        {
        }
    }

    public class ForceRawMessageFormatBindingCollectionElement : StandardBindingCollectionElement<ForceRawMessageFormatBinding, ForceRawMessageFormatBindingElement>
    {
    }

    public class ForceRawMessageFormatMapper : WebContentTypeMapper
    {
        public override WebContentFormat GetMessageFormatForContentType(string contentType)
        {
            return WebContentFormat.Raw;
        }
    }
}