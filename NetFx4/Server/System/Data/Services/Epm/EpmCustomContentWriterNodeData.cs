//Copyright 2010 Microsoft Corporation
//
//Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 
//
//http://www.apache.org/licenses/LICENSE-2.0 
//
//Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
//"AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and limitations under the License.


namespace System.Data.Services.Common
{
    using System;
    using System.IO;
    using System.Xml;

#if !ASTORIA_CLIENT
    using System.ServiceModel.Syndication;
    using System.Data.Services.Serializers;
    using System.Data.Services.Providers;
#else
    using System.Data.Services.Client;
#endif

    internal sealed class EpmCustomContentWriterNodeData : IDisposable
    {
        private bool disposed;

#if ASTORIA_CLIENT
        internal EpmCustomContentWriterNodeData(EpmTargetPathSegment segment, object element)
#else
        internal EpmCustomContentWriterNodeData(EpmTargetPathSegment segment, object element, EpmContentSerializer.EpmNullValuedPropertyTree nullValuedProperties, DataServiceProviderWrapper provider)
#endif
        {
            this.XmlContentStream = new MemoryStream();
            XmlWriterSettings customContentWriterSettings = new XmlWriterSettings();
            customContentWriterSettings.OmitXmlDeclaration = true;
            customContentWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;
            this.XmlContentWriter = XmlWriter.Create(this.XmlContentStream, customContentWriterSettings);
#if ASTORIA_CLIENT
            this.PopulateData(segment, element);
#else
            this.PopulateData(segment, element, nullValuedProperties, provider);
#endif
        }

#if ASTORIA_CLIENT
        internal EpmCustomContentWriterNodeData(EpmCustomContentWriterNodeData parentData, EpmTargetPathSegment segment, object element)
#else
        internal EpmCustomContentWriterNodeData(EpmCustomContentWriterNodeData parentData, EpmTargetPathSegment segment, object element, EpmContentSerializer.EpmNullValuedPropertyTree nullValuedProperties, DataServiceProviderWrapper provider)
#endif
        {
            this.XmlContentStream = parentData.XmlContentStream;
            this.XmlContentWriter = parentData.XmlContentWriter;
#if ASTORIA_CLIENT
            this.PopulateData(segment, element);
#else
            this.PopulateData(segment, element, nullValuedProperties, provider);
#endif
        }

        internal MemoryStream XmlContentStream
        {
            get;
            private set;
        }

        internal XmlWriter XmlContentWriter
        {
            get;
            private set;
        }

        internal String Data
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.XmlContentWriter != null)
                {
                    this.XmlContentWriter.Close();
                    this.XmlContentWriter = null;
                }

                if (this.XmlContentStream != null)
                {
                    this.XmlContentStream.Dispose();
                    this.XmlContentStream = null;
                }

                this.disposed = true;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "XmlReader on MemoryStream does not require disposal")]
#if ASTORIA_CLIENT
        internal void AddContentToTarget(XmlWriter target)
#else
        internal void AddContentToTarget(SyndicationItem target)
#endif
        {
#if ASTORIA_CLIENT
            Util.CheckArgumentNull(target, "target");
#else
            WebUtil.CheckArgumentNull(target, "target");
#endif
            this.XmlContentWriter.Close();
            this.XmlContentWriter = null;
            this.XmlContentStream.Seek(0, SeekOrigin.Begin);
            XmlReaderSettings customContentReaderSettings = new XmlReaderSettings();
            customContentReaderSettings.ConformanceLevel = ConformanceLevel.Fragment;
            XmlReader reader = XmlReader.Create(this.XmlContentStream, customContentReaderSettings);
            this.XmlContentStream = null;
#if ASTORIA_CLIENT
            target.WriteNode(reader, false);
#else
            target.ElementExtensions.Add(reader);
#endif
        }

#if ASTORIA_CLIENT
        private void PopulateData(EpmTargetPathSegment segment, object element)
#else
        private void PopulateData(EpmTargetPathSegment segment, object element, EpmContentSerializer.EpmNullValuedPropertyTree nullValuedProperties, DataServiceProviderWrapper provider)
#endif
        {
            if (segment.EpmInfo != null)
            {
                Object propertyValue;

                try
                {
#if ASTORIA_CLIENT
                    propertyValue = segment.EpmInfo.ReadPropertyValue(element);
#else
                    propertyValue = segment.EpmInfo.ReadPropertyValue(element, provider);
#endif
                }
                catch 
#if ASTORIA_CLIENT
                (System.Reflection.TargetInvocationException)
#else
                (System.Reflection.TargetInvocationException e)
#endif
                {
#if !ASTORIA_CLIENT
                    ErrorHandler.HandleTargetInvocationException(e);
#endif
                    throw;
                }

#if ASTORIA_CLIENT
                this.Data = propertyValue == null ? String.Empty : ClientConvert.ToString(propertyValue, false);
#else
                if (propertyValue == null || propertyValue == DBNull.Value)
                {
                    this.Data = String.Empty;
                    nullValuedProperties.Add(segment.EpmInfo);
                }
                else
                {
                    this.Data = PlainXmlSerializer.PrimitiveToString(propertyValue);
                }
#endif
            }
        }
    }
}