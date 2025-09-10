using Microsoft.OData.Serializer.V3.Utils;
using Microsoft.OData.UriParser;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Microsoft.OData.Serializer.V3.ContextUrl;

internal static class ContextUrlHelper
{
    public static void WriteContextUrlProperty(ODataPayloadKind payloadKind, ODataUri uri, Utf8JsonWriter jsonWriter)
    {
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                WriteResourceSetContextUrl(uri, jsonWriter);
                break;

            default:
                // No context URL for other payload kinds
                throw new NotImplementedException();
        }
    }

    internal static void WriteResourceSetContextUrl(ODataUri odataUri, Utf8JsonWriter jsonWriter)
    {
        if (odataUri == null)
        {
            return;
        }

        if (TryWriteContextUrlPropertyForSimpleEntitySet(odataUri, jsonWriter))
        {
            return;
        }

        if (TryWriteContextUrlPropertyForEntitySetWithSimpleSelectExpand(odataUri, jsonWriter))
        {
            return;
        }

        var writer = jsonWriter;

        // TODO: This is a naive, incorrect, incomplete implementation of the context URL.
        // It's just a placeholder to show how it might be constructed.

        // TODO: in .NET 10 we can write consider Utf8JsonWriter.WriteStringValueSegment
        // to write the string in chunks

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri;
        const string metadata = "$metadata#";

        // One pass through the segments to compute total size to avoid multiple array allocations (e.g. with StringBuilder)
        int totalSize = (absoluteUri?.Length ?? 0)
            + metadata.Length;

        if (odataUri.Path.Count > 0)
        {
            totalSize += odataUri.Path[0].Identifier.Length;
        }

        for (int i = 1; i < odataUri.Path.Count; i++)
        {
            totalSize += odataUri.Path[i].Identifier.Length + 1; // +1 for the separator
        }

        char[]? array = null;
        Span<char> buffer = totalSize <= 128 ?
            stackalloc char[totalSize] :
            (array = ArrayPool<char>.Shared.Rent(totalSize));

        buffer = buffer[..totalSize]; // Slice the buffer in case we rented a larger array than needed
        int offset = 0;
        absoluteUri.AsSpan().CopyTo(buffer);
        offset += absoluteUri?.Length ?? 0;
        metadata.AsSpan().CopyTo(buffer[offset..]);
        offset += metadata.Length;

        if (odataUri.Path.Count > 0)
        {
            odataUri.Path[0].Identifier.AsSpan().CopyTo(buffer[offset..]);
            offset += odataUri.Path[0].Identifier.Length;
        }

        for (int i = 1; i < odataUri.Path.Count; i++)
        {
            buffer[offset] = '/';
            odataUri.Path[i].Identifier.AsSpan().CopyTo(buffer[(offset + 1)..]);
            offset += 1 + odataUri.Path[i].Identifier.Length;
        }

        writer.WriteString("@odata.context"u8, buffer);

        if (array != null)
        {
            ArrayPool<char>.Shared.Return(array);
        }
    }

    internal static bool TryWriteContextUrlPropertyForSimpleEntitySet(ODataUri odataUri, Utf8JsonWriter jsonWriter)
    {
        if (!IsSimpleEntitySet(odataUri))
        {
            return false;
        }

        Debug.Assert(odataUri.SelectAndExpand == null);
        Debug.Assert(odataUri.Apply == null);

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;

        const string metadata = "$metadata#";

        Debug.Assert(odataUri.Path.Count == 1);
        var segment = odataUri.Path[0] as EntitySetSegment;
        Debug.Assert(segment != null);

        int totalLength = absoluteUri.Length + metadata.Length + segment.EntitySet.Name.Length;

        (string AbsoluteUri, string SegmentName, Utf8JsonWriter Writer) state = (absoluteUri, segment.EntitySet.Name, jsonWriter);

        ShortLivedArrayHelpers.WriteCharArray(totalLength, state, static (uriStringBuffer, state) =>
        {
            var builder = new SpanStringBuilder(uriStringBuffer);
            builder.Append(state.AbsoluteUri);
            builder.Append(metadata);
            builder.Append(state.SegmentName);

            state.Writer.WriteString("@odata.context", builder.WrittenSpan);
        });

        return true;
    }

    internal static bool TryWriteContextUrlPropertyForEntitySetWithSimpleSelectExpand(ODataUri odataUri, Utf8JsonWriter writer)
    {
        // Simple select expand means:
        // We have <= X selects and <= X expands
        // No nested selects (e.g. selecting nested properties of a complex type)
        // No nested expands (no $select or $expand inside an $expand)
        // No duplicates (how do we check for duplicates)
        // wild card is supported

        if (odataUri.Path.Count != 1 || odataUri.Apply != null) 
        {
            return false;
        }

        if (odataUri.Path[0] is not EntitySetSegment entitySet)
        {
            return false;
        }

        //Debug.Assert(odataUri.SelectAndExpand);

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;
        const string metadata = "$metadata#";
        var duplicateChecker = new PossibleDuplicateStringChecker();
        var selectedProperties = new InlineStringList10();
        var expandedProperties = new InlineStringList10();
        bool hasWildcard = false;
        int runningExpandLength = 0;
        int runningSelectLength = 0;

        foreach (var selectItem in odataUri.SelectAndExpand.SelectedItems)
        {
            // Check for nested selects
            if (selectItem is ExpandedNavigationSelectItem expandedNavSelectItem)
            {
                // No nested expands
                if (expandedNavSelectItem.SelectAndExpand.SelectedItems.Any())
                {
                    return false;
                }

                if (expandedNavSelectItem.PathToNavigationProperty.Count != 1)
                {
                    return false;
                }

                var nav = expandedNavSelectItem.PathToNavigationProperty.LastSegment as NavigationPropertySegment;
                Debug.Assert(nav != null);

                var navPropertyName = nav.NavigationProperty.Name;
                if (!duplicateChecker.TryAdd(navPropertyName))
                {
                    return false;
                }

                if (!expandedProperties.TryAdd(navPropertyName))
                {
                    return false;
                }

                runningExpandLength += navPropertyName.Length;
            }
            else if (selectItem is PathSelectItem pathSelectItem)
            {
                // No nested selects
                if (pathSelectItem.SelectedPath.Count != 1)
                {
                    return false;
                }

                if (pathSelectItem.SelectAndExpand != null)
                {
                    return false;
                }

                var property = pathSelectItem.SelectedPath[0] as PropertySegment;
                Debug.Assert(property != null);


                // if we've seen a wildcard, then we don't need to worry about duplicates
                // or number of select properties because every selected item will be absorbed in the wildcard
                if (hasWildcard)
                {
                    continue;
                }

                if (!duplicateChecker.TryAdd(property.Property.Name))
                {
                    return false;
                }

                if (!selectedProperties.TryAdd(property.Property.Name))
                {
                    return false;
                }

                runningSelectLength += property.Property.Name.Length;
            }
            else if (selectItem is WildcardSelectItem)
            {
                hasWildcard = true;
            }
            else
            {
                return false;
            }
        }

        int totalStringLength = absoluteUri.Length + entitySet.EntitySet.Name.Length + metadata.Length;

        if (runningExpandLength + runningSelectLength > 0 || hasWildcard)
        {
            totalStringLength += 2; // we'll wrapp the selected/expanded properties between ( )
        }

        // Since no we don't need nested expands, then each expand will be written as ( )
        // We'll have comma between expands
        if (expandedProperties.Length > 0)
        {
            totalStringLength += 2 * expandedProperties.Length; // we'll wrap the expanded properties between ( ) and add commas
            totalStringLength += expandedProperties.Length - 1; // we'll add commas between expanded properties
            totalStringLength += runningExpandLength;
        }

        if (hasWildcard)
        {
            totalStringLength += 1;
        }
        else if (selectedProperties.Length > 0)
        {
            totalStringLength += selectedProperties.Length - 1; // we'll add commas between selected properties
            totalStringLength += runningSelectLength;
        }

        // if we have both select and expand, then we'll need a comma between the last select and first expand
        if ((hasWildcard || selectedProperties.Length > 0) && expandedProperties.Length > 0)
        {
            totalStringLength += 1;
        }

        (
            string ServiceRoot,
            string EntitySet,
            bool HasWildCard,
            InlineStringList10 SelectedProperties,
            InlineStringList10 ExpandedProperties,
            Utf8JsonWriter Writer
        ) state = (absoluteUri, entitySet.EntitySet.Name, hasWildcard, selectedProperties, expandedProperties, writer);

        ShortLivedArrayHelpers.WriteCharArray(totalStringLength, state, static (buffer, state) =>
        {
            var builder = new SpanStringBuilder(buffer);
            builder.Append(state.ServiceRoot);
            builder.Append(metadata);
            builder.Append(state.EntitySet);

            bool hasSelect = state.HasWildCard || state.SelectedProperties.Length > 0;

            if (hasSelect|| state.ExpandedProperties.Length > 0)
            {
                builder.Append('(');

                if (state.HasWildCard)
                {
                    builder.Append('*');
                }
                else if (state.SelectedProperties.Length > 0)
                {
                    builder.Append(state.SelectedProperties[0]);
                    for (int i = 1; i < state.SelectedProperties.Length; i++)
                    {
                        builder.Append(',');
                        builder.Append(state.SelectedProperties[i]);
                    }
                }

                if (state.ExpandedProperties.Length > 0)
                {
                    if (hasSelect)
                    {
                        builder.Append(',');
                    }

                    builder.Append(state.ExpandedProperties[0]);
                    builder.Append('(');
                    builder.Append(')');

                    for (int i = 1; i < state.ExpandedProperties.Length; i++)
                    {
                        builder.Append(')');
                        builder.Append(state.ExpandedProperties[i]);
                        builder.Append('(');
                        builder.Append(')');
                    }
                }

                builder.Append(')');
            }

            state.Writer.WriteString("@odata.context"u8, builder.WrittenSpan);
        });

        return true;
    }

    private static bool IsSimpleEntitySet(ODataUri odataUri)
    {
        return odataUri.Path.Count == 1
            && odataUri.Path[0] is EntitySetSegment
            && odataUri.SelectAndExpand == null
            && odataUri.Apply == null;
    }
}
