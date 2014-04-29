//---------------------------------------------------------------------
// <copyright file="WebParseError.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <summary>
//      WebParseError type.
// </summary>
//
// @owner  markash
//---------------------------------------------------------------------

namespace Microsoft.OData.Service.Http
{
    /// <summary>Errors for parsing.</summary>
    internal enum WebParseErrorCode
    {
        /// <summary>Generic status constant.</summary>
        Generic,

        /// <summary>InvalidHeaderName status constant.</summary>
        InvalidHeaderName,

        /// <summary>InvalidContentLength status constant.</summary>
        InvalidContentLength,

        /// <summary>IncompleteHeaderLine status constant.</summary>
        IncompleteHeaderLine,

        /// <summary>CrLfError status constant.</summary>
        CrLfError,

        /// <summary>InvalidChunkFormat status constant.</summary>
        InvalidChunkFormat,

        /// <summary>UnexpectedServerResponse status constant.</summary>
        UnexpectedServerResponse
    }

    /// <summary>Section for a parsing error.</summary>
    internal enum WebParseErrorSection
    {
        /// <summary>Generic section constant.</summary>
        Generic,

        /// <summary>ResponseHeader section constant.</summary>
        ResponseHeader,

        /// <summary>ResponsesectionLine section constant.</summary>
        ResponsesectionLine,

        /// <summary>ResponseBody section constant.</summary>
        ResponseBody
    }

    /// <summary>Status of data parsing.</summary>
    internal enum DataParseStatus
    {
        /// <summary>NeedMoreData status constant.</summary>
        NeedMoreData,

        /// <summary>ContinueParsing status constant.</summary>
        ContinueParsing,

        /// <summary>Done status constant.</summary>
        Done,

        /// <summary>Invalid status constant.</summary>
        Invalid,

        /// <summary>DataTooBig status constant.</summary>
        DataTooBig
    }

    ////[StructLayout(LayoutKind.Sequential)]

    /// <summary>Use this type to describe a parsing error.</summary>
    internal struct WebParseError
    {
        /// <summary>Section for the error.</summary>
        public WebParseErrorSection Section;

        /// <summary>Error code.</summary>
        public WebParseErrorCode Code;
    }
}
