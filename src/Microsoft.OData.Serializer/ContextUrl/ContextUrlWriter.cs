using Microsoft.OData.UriParser;
using System.Diagnostics;
using System.Text.Json;

namespace Microsoft.OData.Serializer;

internal static class ContextUrlWriter
{
    // TODO: should consult spec https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_ContextURL
    // and reference implementation in old OData library to make sure we cover all cases correctly
    public static void WriteContextUrlProperty(ODataPayloadKind payloadKind, ODataUri uri, Utf8JsonWriter jsonWriter)
    {
        switch (payloadKind)
        {
            case ODataPayloadKind.ResourceSet:
                WriteResourceSetContextUrl(uri, new ODataPathRange(uri.Path, uri.Path.Count), jsonWriter);
                break;
            case ODataPayloadKind.Resource:
                WriteResourceContextUrl(uri, jsonWriter);
                break;

            default:
                FallbackWriteContextUrl(uri, new ODataPathRange(uri.Path, uri.Path.Count), jsonWriter);
                throw new NotImplementedException();
        }
    }

    internal static void WriteResourceSetContextUrl(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter jsonWriter, string appendFragment = "")
    {
        if (odataUri == null)
        {
            return;
        }

        if (path[^1] is EntitySetSegment)
        {
            if (TryWriteContextUrlPropertyForSimpleEntitySet(odataUri, path, jsonWriter, appendFragment))
            {
                return;
            }

            if (TryWriteContextUrlPropertyForEntitySetWithSimpleSelectExpand(odataUri, path, jsonWriter, appendFragment))
            {
                return;
            }
        }
        else if (TryWriteContextUrlPropertyForSimpleResourceSet(odataUri, path, jsonWriter, appendFragment))
        {
            return;
        }
        else if (TryWriteContextUrlPropertyForResourcesetSetWithSimpleSelectExpand(odataUri, path, jsonWriter, appendFragment))
        {
            return;
        }

        FallbackWriteContextUrl(odataUri, path, jsonWriter, appendFragment);
    }

    internal static void WriteResourceContextUrl(ODataUri odataUri, Utf8JsonWriter jsonWriter)
    {
        if (odataUri == null)
        {
            return;
        }

        if (odataUri.Path.Count == 0)
        {
            return;
        }

        // TODO: we only support entityCollection/{id} for now, not singletons
        if (odataUri.Path[^1] is not KeySegment keySegment)
        {
            // Perhaps we should write some "default" context URL?
            return;
        }

        // piggy back on resource set context URL writer
        ODataPathRange path = new(odataUri.Path, odataUri.Path.Count - 1);
        WriteResourceSetContextUrl(odataUri, path, jsonWriter, "/$entity");
    }

    internal static bool TryWriteContextUrlPropertyForSimpleEntitySet(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter jsonWriter, string appendFragment = "")
    {
        if (!IsSimpleEntitySet(odataUri, path))
        {
            return false;
        }

        Debug.Assert(odataUri.SelectAndExpand == null);
        Debug.Assert(odataUri.Apply == null);

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;

        const string metadata = "$metadata#";

        Debug.Assert(path.Count == 1);
        var segment = path[0] as EntitySetSegment;
        Debug.Assert(segment != null);

        int totalLength = absoluteUri.Length + metadata.Length + segment.EntitySet.Name.Length + appendFragment.Length;

        (
            string AbsoluteUri,
            string SegmentName,
            string AppendFragment,
            Utf8JsonWriter Writer) state = (absoluteUri, segment.EntitySet.Name, appendFragment, jsonWriter);

        // Cannot use a lambda action since we need to pass an in parameter
        // Parameter modifiers in lambdas are still in preview
        ShortLivedArrayHelpers.CreateCharArray(totalLength, state, WriteContextUrl);

        static void WriteContextUrl(Span<char> buffer, in (string AbsoluteUri, string SegmentName, string AppendFragment, Utf8JsonWriter Writer) state)
        {
            var builder = new FixedSpanStringBuilder(buffer);
            builder.Append(state.AbsoluteUri);
            builder.Append(metadata);
            builder.Append(state.SegmentName);
            builder.Append(state.AppendFragment);

            state.Writer.WriteString("@odata.context"u8, builder.WrittenSpan);
        }

        return true;
    }

    internal static bool TryWriteContextUrlPropertyForSimpleResourceSet(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter jsonWriter, string appendFragment = "")
    {
        if (odataUri.SelectAndExpand != null)
        {
            return false;
        }

        if (odataUri.Apply != null)
        {
            return false;
        }

        var segments = new InlineStringList10();
        int runningSegmentsLength = 0;

        // TODO: Here we're using the path in the URL as the context URL path,
        // but we should use the canonical path instead (e.g. in case this
        // is a non-contained navigation property with a navigation binding path)
        for (int i = 0; i < path.Count; i++)
        {
            var segment = path[i];

            // TODO: this method might allocate and should be refactored!
            var segmentText = GetTextForSegment(segment);
            if (!segments.TryAdd(segmentText))
            {
                return false;
            }

            runningSegmentsLength += segmentText.Length;
        }

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;

        const string metadata = "$metadata#";

        int totalLength = absoluteUri.Length + metadata.Length + appendFragment.Length;
        totalLength += runningSegmentsLength + (path.Count - 1); // segments and the separators between them

        // Cannot use a lambda action since we need to pass an in parameter
        // Parameter modifiers in lambdas are still in preview
        ShortLivedArrayHelpers.CreateCharArray(totalLength, (segments, absoluteUri, appendFragment, jsonWriter), WriteContextUrl);

        static void WriteContextUrl(
            Span<char> buffer,
            in (InlineStringList10 Segments, string AbsoluteUri, string AppendFragment, Utf8JsonWriter Writer) state)
        {
            var builder = new FixedSpanStringBuilder(buffer);
            builder.Append(state.AbsoluteUri);
            builder.Append(metadata);

            builder.Append(state.Segments[0]);
            for (int i = 1; i < state.Segments.Length; i++)
            {
                // Since we conditionally append '/',
                // we might end up using fewer chars than "totalLength",
                // but that's okay
                if (state.Segments[i][0] != '(') // don't add separator before key segment
                {
                    builder.Append('/');
                }

                builder.Append(state.Segments[i]);
            }

            builder.Append(state.AppendFragment);

            state.Writer.WriteString("@odata.context"u8, builder.WrittenSpan);
        }

        return true;
    }

    internal static bool TryWriteContextUrlPropertyForEntitySetWithSimpleSelectExpand(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter writer, string appendFragment = "")
    {
        // Simple select expand means:
        // We have <= X selects and <= X expands
        // No nested selects (e.g. selecting nested properties of a complex type)
        // No nested expands (no $select or $expand inside an $expand)
        // No duplicates (how do we check for duplicates)
        // wild card is supported

        if (path.Count != 1 || odataUri.Apply != null) 
        {
            return false;
        }

        if (path[0] is not EntitySetSegment entitySet)
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

                if (pathSelectItem.SelectAndExpand.SelectedItems.Any())
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

        int totalStringLength = absoluteUri.Length + entitySet.EntitySet.Name.Length + metadata.Length + appendFragment.Length;

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
            string AppendFragment,
            bool HasWildCard,
            InlineStringList10 SelectedProperties,
            InlineStringList10 ExpandedProperties,
            Utf8JsonWriter Writer
        ) state = (absoluteUri, entitySet.EntitySet.Name, appendFragment, hasWildcard, selectedProperties, expandedProperties, writer);

        ShortLivedArrayHelpers.CreateCharArray(totalStringLength, state, WriteContextUrl);

        static void WriteContextUrl(
            Span<char> buffer,
            in (
                string ServiceRoot,
                string EntitySet,
                string AppendFragment,
                bool HasWildCard,
                InlineStringList10 SelectedProperties,
                InlineStringList10 ExpandedProperties,
                Utf8JsonWriter Writer
            ) state)
        {
            var builder = new FixedSpanStringBuilder(buffer);
            builder.Append(state.ServiceRoot);
            builder.Append(metadata);
            builder.Append(state.EntitySet);

            bool hasSelect = state.HasWildCard || state.SelectedProperties.Length > 0;

            if (hasSelect || state.ExpandedProperties.Length > 0)
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

            builder.Append(state.AppendFragment);

            state.Writer.WriteString("@odata.context"u8, builder.WrittenSpan);
        }

        return true;
    }

    internal static bool TryWriteContextUrlPropertyForResourcesetSetWithSimpleSelectExpand(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter writer, string appendFragment = "")
    {
        // Simple select expand means:
        // We have <= X selects and <= X expands
        // No nested selects (e.g. selecting nested properties of a complex type)
        // No nested expands (no $select or $expand inside an $expand)
        // No duplicates (how do we check for duplicates)
        // wild card is supported

        if (odataUri.Apply != null)
        {
            return false;
        }

        Debug.Assert(path.Count > 0);

        //Debug.Assert(odataUri.SelectAndExpand);

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri ?? string.Empty;
        const string metadata = "$metadata#";
        var duplicateChecker = new PossibleDuplicateStringChecker();
        var selectedProperties = new InlineStringList10();
        var expandedProperties = new InlineStringList10();
        bool hasWildcard = false;
        int runningExpandLength = 0;
        int runningSelectLength = 0;

        var segments = new InlineStringList10();
        int runningSegmentsLength = 0;

        for (int i = 0; i < path.Count; i++)
        {
            var segment = path[i];

            // TODO: this method might allocate and should be refactored!
            var segmentText = GetTextForSegment(segment);
            if (!segments.TryAdd(segmentText))
            {
                return false;
            }

            runningSegmentsLength += segmentText.Length;
        }

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

                if (pathSelectItem.SelectAndExpand.SelectedItems.Any())
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

        int totalStringLength = absoluteUri.Length + metadata.Length + appendFragment.Length;
        totalStringLength += runningSegmentsLength + (segments.Length - 1); // segments and the separators between them

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

        // TODO: we're copying the InlineStrings here into the state, that could be expensive
        // since they're large. Is there a way we can pass their references instead?
        // That would be possible if we could make the state type allow ref struct, then
        // we create a custom ref struct state and store references to the properties lists.
        ShortLivedArrayHelpers.CreateCharArray(
            totalStringLength,
            (segments, selectedProperties, expandedProperties, absoluteUri, appendFragment, hasWildcard, writer),
            WriteContextUrl);

        static void WriteContextUrl(
            Span<char> buffer,
            in (
                InlineStringList10 Segments,
                InlineStringList10 SelectedProperties,
                InlineStringList10 ExpandedProperties,
                string ServiceRoot,
                string AppendFragment,
                bool HasWildCard,
                Utf8JsonWriter Writer
            ) state)
        {
            var builder = new FixedSpanStringBuilder(buffer);
            builder.Append(state.ServiceRoot);
            builder.Append(metadata);

            builder.Append(state.Segments[0]);
            for (int i = 1; i < state.Segments.Length; i++)
            {
                if (state.Segments[i][0] != '(') // don't add separator before key segment)
                {
                    builder.Append('/');
                }

                builder.Append(state.Segments[i]);
            }

            bool hasSelect = state.HasWildCard || state.SelectedProperties.Length > 0;

            if (hasSelect || state.ExpandedProperties.Length > 0)
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

            builder.Append(state.AppendFragment);

            state.Writer.WriteString("@odata.context"u8, builder.WrittenSpan);
        }

        return true;
    }

    private static void FallbackWriteContextUrl(ODataUri odataUri, ODataPathRange path, Utf8JsonWriter writer, string appendFragment = "")
    {
        // Fallback to generic context URL construction

        // TODO: THIS implementation is INCORRECT and incomplete.
        // We'll work on full parity with old library and OData spec later, after the preview.
        // TODO: This is a naive, incorrect, incomplete implementation of the context URL.
        // It's just a placeholder to show how it might be constructed.

        // TODO: in .NET 10 we can write consider Utf8JsonWriter.WriteStringValueSegment
        // to write the string in chunks

        var absoluteUri = odataUri.ServiceRoot?.AbsoluteUri;
        const string metadata = "$metadata#";

        // One pass through the segments to compute total size to avoid multiple array allocations (e.g. with StringBuilder)
        int totalSize = (absoluteUri?.Length ?? 0)
            + metadata.Length
            + appendFragment.Length;

        if (path.Count > 0)
        {
            totalSize += path[0].Identifier.Length;
        }

        for (int i = 1; i < path.Count; i++)
        {
            totalSize += path[i].Identifier.Length + 1; // +1 for the separator
        }

        ShortLivedArrayHelpers.CreateCharArray(totalSize, (path, absoluteUri, appendFragment, writer), WriteContextUrl);

        static void WriteContextUrl(
            Span<char> buffer,
            in (
                ODataPathRange Path,
                string? AbsoluteUri,
                string AppendFragment,
                Utf8JsonWriter Writer) state)
        {
            var builder = new FixedSpanStringBuilder(buffer);
            builder.Append(state.AbsoluteUri);
            builder.Append(metadata);

            if (state.Path.Count > 0)
            {
                builder.Append(state.Path[0].Identifier);
            }

            for (int i = 1; i < state.Path.Count; i++)
            {
                builder.Append('/');
                builder.Append(state.Path[i].Identifier);
            }

            builder.Append(state.AppendFragment);

            state.Writer.WriteString("@odata.context"u8, buffer);
        }
    }

    private static bool IsSimpleEntitySet(ODataUri odataUri, ODataPathRange path)
    {
        return path.Count == 1
            && path[0] is EntitySetSegment
            && odataUri.SelectAndExpand == null
            && odataUri.Apply == null;
    }

    private static string GetTextForSegment(ODataPathSegment segment)
    {
        return segment switch
        {
            EntitySetSegment entitySet => entitySet.EntitySet.Name,
            NavigationPropertySegment navSeg => navSeg.NavigationProperty.Name,
            PropertySegment prop => prop.Property.Name,
            SingletonSegment singleton => singleton.Singleton.Name,
            // TODO we support only single key for now
            // TODO: BAD, avoid this alloc. Might need refactor.
            // TODO: Need to make sure key is escaped properly
            KeySegment key => $"('{key.Keys.First().Value}')",
            _ => segment.Identifier
        };
    }
}
