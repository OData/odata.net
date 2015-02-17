//---------------------------------------------------------------------
// <copyright file="ODataReflectionStreamProvider.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWCFService.DataSource
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class ODataReflectionStreamProvider : IODataStreamProvider
    {
        public Stream GetStream(object entity)
        {
            var mediaEntity = GetAsMediaEntity(entity);
            return ConvertToNonClosingStream(mediaEntity.Stream);
        }

        public void CreateStream(object entity, Stream stream, string contentType)
        {
            var mediaEntity = GetAsMediaEntity(entity);
            mediaEntity.Stream = ConvertToNonClosingStream(stream);
            mediaEntity.ContentType = contentType;
            mediaEntity.Id = DateTime.UtcNow.Ticks % 1000 + 1000; //TODO: [tiano]implement a key generator for all keys
            mediaEntity.ETagValue = DateTime.UtcNow.Ticks;
        }

        public void UpdateStream(object entity, Stream stream, string contentType)
        {
            var mediaEntity = GetAsMediaEntity(entity);
            mediaEntity.Stream.Dispose();
            mediaEntity.Stream = ConvertToNonClosingStream(stream);
            mediaEntity.ContentType = contentType;
            mediaEntity.ETagValue = DateTime.UtcNow.Ticks;
        }

        public void DeleteStream(object entity)
        {
            // just validate here, do nothing for in memory media entity stream
            GetAsMediaEntity(entity);
        }

        public string GetETag(object entity)
        {
            var mediaEntity = GetAsMediaEntity(entity);
            return mediaEntity.ETag;
        }

        public string GetContentType(object entity)
        {
            var mediaEntity = GetAsMediaEntity(entity);
            return mediaEntity.ContentType;
        }

        private static MediaEntity GetAsMediaEntity(object entity)
        {
            var result = entity as MediaEntity;

            if (result == null)
            {
                throw new InvalidOperationException("The entity is not a media entity.");
            }

            return result;
        }

        private static Stream ConvertToNonClosingStream(Stream source)
        {
            var result = new NonClosingStream();

            if (source.CanSeek)
            {
                source.Seek(0, SeekOrigin.Begin);
            }

            source.CopyTo(result);

            if (source.CanSeek)
            {
                source.Seek(0, SeekOrigin.Begin);
            }

            result.Seek(0, SeekOrigin.Begin);

            return result;
        }
    }
}
