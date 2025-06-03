using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.OData.Core.NewWriter;

internal interface IAnnotationsWriter
{
    void WriteCount(object value, ODataWriterState state);
    /*
    void WriteNextLink(object value, ODataWriterState state);
    void WriteDeltaLink(object value, ODataWriterState state);
    void WriteId(object value, ODataWriterState state);
    void WriteEditLink(object value, ODataWriterState state);
    void WriteReadLink(object value, ODataWriterState state);
    */
}

internal interface IAnnotationsWriter<T> : IAnnotationsWriter
{
    void WriteCount(T value, ODataWriterState state);
    /*
    void WriteNextLink(T value, ODataWriterState state);
    void WriteDeltaLink(T value, ODataWriterState state);
    void WriteId(T value, ODataWriterState state);
    void WriteEditLink(T value, ODataWriterState state);
    void WriteReadLink(T value, ODataWriterState state);
    */
}