//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamBuffer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.OData
{
    #region Namespaces
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    #endregion Namespaces

    /// <summary>
    /// This class represents the internal buffer of the <see cref="ODataBatchReaderStream"/>.
    /// </summary>
    internal sealed class ODataBatchReaderStreamBuffer
    {
        /// <summary>The size of the look-ahead buffer.</summary>
        internal const int BufferLength = 8000;

        /// <summary>Length of the longest supported line terminator character sequence; makes the code easier to read.</summary>
        private const int MaxLineFeedLength = 2;

        /// <summary>The length of two '-' characters to make the code easier to read.</summary>
        private const int TwoDashesLength = 2;

        /// <summary>The byte array storing the actual bytes of the buffer.</summary>
        private readonly byte[] bytes = new byte[BufferLength];

        /// <summary>The current position inside the buffer.</summary>
        /// <remarks>This is the position of the byte that is the next to be read.</remarks>
        private int currentReadPosition = 0;

        /// <summary>The number of (not yet consumed) bytes currently in the buffer.</summary>
        private int numberOfBytesInBuffer;

        /// <summary>
        /// The byte array that acts as the actual storage of the buffered data.
        /// </summary>
        internal byte[] Bytes
        {
            get
            {
                return this.bytes;
            }
        }

        /// <summary>
        /// The current position inside the buffer.
        /// </summary>
        /// <remarks>This is the position of the byte that is the next to be read.</remarks>
        internal int CurrentReadPosition
        {
            get
            {
                return this.currentReadPosition;
            }
        }

        /// <summary>
        /// The number of (not yet consumed) bytes currently in the buffer.
        /// </summary>
        internal int NumberOfBytesInBuffer
        {
            get
            {
                return this.numberOfBytesInBuffer;
            }
        }

        /// <summary>
        /// Indexer into the byte buffer.
        /// </summary>
        /// <param name="index">The position in the buffer to get.</param>
        /// <returns>The byte at position <paramref name="index"/> in the buffer.</returns>
        internal byte this[int index]
        {
            get
            {
                return this.bytes[index];
            }
        }

        /// <summary>
        /// Skip to the specified position in the buffer.
        /// Adjust the current position and the number of bytes in the buffer.
        /// </summary>
        /// <param name="newPosition">The position to skip to.</param>
        internal void SkipTo(int newPosition)
        {
            Debug.Assert(newPosition >= this.currentReadPosition, "Can only skip forward.");
            Debug.Assert(newPosition <= this.currentReadPosition + this.numberOfBytesInBuffer, "Cannot skip beyond end of buffer.");

            int diff = newPosition - this.currentReadPosition;
            this.currentReadPosition = newPosition;
            this.numberOfBytesInBuffer -= diff;
        }

        /// <summary>
        /// Refills the buffer from the specified stream.
        /// </summary>
        /// <param name="stream">The stream to refill the buffer from.</param>
        /// <param name="preserveFrom">The index in the current buffer starting from which the
        /// currently buffered data should be preserved.</param>
        /// <returns>true if the underlying stream got exhausted while refilling.</returns>
        /// <remarks>This method will first shift any data that is to be preserved to the beginning
        /// of the buffer and then refill the rest of the buffer from the <paramref name="stream"/>.</remarks>
        internal bool RefillFrom(Stream stream, int preserveFrom)
        {
            Debug.Assert(stream != null, "stream != null");
            Debug.Assert(preserveFrom >= this.currentReadPosition, "preserveFrom must be at least as big as the current position in the buffer.");

            this.ShiftToBeginning(preserveFrom);
            int numberOfBytesToRead = ODataBatchReaderStreamBuffer.BufferLength - this.numberOfBytesInBuffer;
            int numberOfBytesRead = stream.Read(this.bytes, this.numberOfBytesInBuffer, numberOfBytesToRead);
            this.numberOfBytesInBuffer += numberOfBytesRead;

            // return true if the underlying stream is exhausted
            return numberOfBytesRead == 0;
        }

        /// <summary>
        /// Scans the current buffer for a line end.
        /// </summary>
        /// <param name="lineEndStartPosition">The start position of the line terminator or -1 if not found.</param>
        /// <param name="lineEndEndPosition">The end position of the line terminator or -1 if not found.</param>
        /// <returns>An enumeration value indicating whether the line termintor was found completely, partially or not at all.</returns>
        internal ODataBatchReaderStreamScanResult ScanForLineEnd(out int lineEndStartPosition, out int lineEndEndPosition)
        {
            bool endOfBufferReached;
            return this.ScanForLineEnd(
                this.currentReadPosition,
                BufferLength,
                /*allowLeadingWhitespaceOnly*/ false,
                out lineEndStartPosition,
                out lineEndEndPosition,
                out endOfBufferReached);
        }

        /// <summary>
        /// Scans the current buffer for the specified boundary.
        /// </summary>
        /// <param name="boundaries">The boundary strings to search for; this enumerable is sorted from the inner-most boundary
        /// to the top-most boundary. The boundary strings don't include the leading line terminator or the leading dashes.</param>
        /// <param name="maxDataBytesToScan">Stop if no boundary (or boundary start) is found after this number of bytes.</param>
        /// <param name="boundaryStartPosition">The start position of the boundary or -1 if not found.
        /// Note that the start position is the first byte of the leading line terminator.</param>
        /// <param name="boundaryEndPosition">The end position of the boundary or -1 if not found.
        /// Note that the end position is the last byte of the trailing line terminator.</param>
        /// <param name="isEndBoundary">true if the boundary is an end boundary (followed by two dashes); otherwise false.</param>
        /// <param name="isParentBoundary">true if the detected boundary is the parent boundary; otherwise false.</param>
        /// <returns>An enumeration value indicating whether the boundary was completely, partially or not found in the buffer.</returns>
        internal ODataBatchReaderStreamScanResult ScanForBoundary(
            IEnumerable<string> boundaries,
            int maxDataBytesToScan,
            out int boundaryStartPosition,
            out int boundaryEndPosition,
            out bool isEndBoundary,
            out bool isParentBoundary)
        {
            Debug.Assert(boundaries != null, "boundaries != null");

            boundaryStartPosition = -1;
            boundaryEndPosition = -1;
            isEndBoundary = false;
            isParentBoundary = false;

            int lineScanStartIndex = this.currentReadPosition;

            while (true)
            {
                // NOTE: a boundary has to start with a line terminator followed by two dashes ('-'),
                //       the actual boundary string, another two dashes (for an end boundary),
                //       optional whitespace characters and another line terminator.
                // NOTE: for WCF DS compatibility we do not require the leading line terminator.
                int lineEndStartPosition, boundaryDelimiterStartPosition;
                ODataBatchReaderStreamScanResult lineEndScanResult = this.ScanForBoundaryStart(
                    lineScanStartIndex,
                    maxDataBytesToScan,
                    out lineEndStartPosition,
                    out boundaryDelimiterStartPosition);

                switch (lineEndScanResult)
                {
                    case ODataBatchReaderStreamScanResult.NoMatch:
                        // Did not find a line end or boundary delimiter in the buffer or after reading maxDataBytesToScan bytes.
                        // Report no boundary match.
                        return ODataBatchReaderStreamScanResult.NoMatch;

                    case ODataBatchReaderStreamScanResult.PartialMatch:
                        // Found a partial line end or boundary delimiter at the end of the buffer but before reading maxDataBytesToScan.
                        // Report a partial boundary match.
                        boundaryStartPosition = lineEndStartPosition < 0 ? boundaryDelimiterStartPosition : lineEndStartPosition;
                        return ODataBatchReaderStreamScanResult.PartialMatch;

                    case ODataBatchReaderStreamScanResult.Match:
                        // Found a full line end or boundary delimiter start ('--') before reading maxDataBytesToScan or
                        // hitting the end of the buffer. Start matching the boundary delimiters.
                        //
                        // Start with the expected boundary (the first one in the enumerable):
                        // * if we find a full match - return match because we are done
                        // * if we find a partial match - return partial match because we have to continue checking the expected boundary.
                        // * if we find no match - we know that it is not the expected boundary; check the parent boundary (if it exists).
                        isParentBoundary = false;
                        foreach (string boundary in boundaries)
                        {
                            ODataBatchReaderStreamScanResult boundaryMatch = this.MatchBoundary(
                                lineEndStartPosition,
                                boundaryDelimiterStartPosition,
                                boundary,
                                out boundaryStartPosition,
                                out boundaryEndPosition,
                                out isEndBoundary);
                            switch (boundaryMatch)
                            {
                                case ODataBatchReaderStreamScanResult.Match:
                                    return ODataBatchReaderStreamScanResult.Match;

                                case ODataBatchReaderStreamScanResult.PartialMatch:
                                    boundaryEndPosition = -1;
                                    isEndBoundary = false;
                                    return ODataBatchReaderStreamScanResult.PartialMatch;

                                case ODataBatchReaderStreamScanResult.NoMatch:
                                    // try the parent boundary (if any) or continue scanning
                                    boundaryStartPosition = -1;
                                    boundaryEndPosition = -1;
                                    isEndBoundary = false;
                                    isParentBoundary = true;
                                    break;

                                default:
                                    throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
                            }
                        }

                        // If we could not match the boundary, try again starting at the current boundary start index. Or if we already did that
                        // move one character ahead.
                        lineScanStartIndex = lineScanStartIndex == boundaryDelimiterStartPosition
                            ? boundaryDelimiterStartPosition + 1
                            : boundaryDelimiterStartPosition;

                        break;
                    default:
                        throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
                }
            }
        }

        /// <summary>
        /// Scans the current buffer for a boundary start, which is either a line resource set or two dashes (since we don't require the leading line resource set).
        /// </summary>
        /// <param name="scanStartIx">The index at which to start scanning for the boundary start.</param>
        /// <param name="maxDataBytesToScan">Stop if no boundary start was found after this number of non end-of-line bytes.</param>
        /// <param name="lineEndStartPosition">The start position of the line end or -1 if not found.</param>
        /// <param name="boundaryDelimiterStartPosition">The start position of the boundary delimiter or -1 if not found.</param>
        /// <returns>An enumeration value indicating whether the boundary start was completely, partially or not found in the buffer.</returns>
        private ODataBatchReaderStreamScanResult ScanForBoundaryStart(
            int scanStartIx,
            int maxDataBytesToScan,
            out int lineEndStartPosition,
            out int boundaryDelimiterStartPosition)
        {
            int lastByteToCheck = this.currentReadPosition + Math.Min(maxDataBytesToScan, this.numberOfBytesInBuffer) - 1;

            for (int i = scanStartIx; i <= lastByteToCheck; ++i)
            {
                char ch = (char)this.bytes[i];

                // Note the following common line resource set chars:
                // \n - UNIX   \r\n - DOS   \r - Mac
                if (ch == '\r' || ch == '\n')
                {
                    // We found a line end; now we have to check whether it
                    // consists of a second character or not.
                    lineEndStartPosition = i;

                    // We only want to report PartialMatch if we ran out of bytes in the buffer;
                    // so even if we found the start of a line end in the last byte to check (and
                    // have more bytes in the buffer) we still look ahead by one.
                    if (ch == '\r' && i == lastByteToCheck && maxDataBytesToScan >= this.numberOfBytesInBuffer)
                    {
                        boundaryDelimiterStartPosition = i;
                        return ODataBatchReaderStreamScanResult.PartialMatch;
                    }

                    // If the next char is '\n' we found a line end consisting of two characters; adjust the end position.
                    boundaryDelimiterStartPosition = (ch == '\r' && (char)this.bytes[i + 1] == '\n')  ? i + 2 : i + 1;
                    return ODataBatchReaderStreamScanResult.Match;
                }
                else if (ch == '-')
                {
                    lineEndStartPosition = -1;

                    // We found a potential start of a boundary; we only want to report PartialMatch
                    // if we ran out of bytes in the buffer.
                    // So even if we found the start of a potential boundary start in the last byte to check (and
                    // have more bytes in the buffer) we still look ahead by one.
                    if (i == lastByteToCheck && maxDataBytesToScan >= this.numberOfBytesInBuffer)
                    {
                        boundaryDelimiterStartPosition = i;
                        return ODataBatchReaderStreamScanResult.PartialMatch;
                    }

                    if ((char)this.bytes[i + 1] == '-')
                    {
                        boundaryDelimiterStartPosition = i;
                        return ODataBatchReaderStreamScanResult.Match;
                    }
                }
            }

            lineEndStartPosition = -1;
            boundaryDelimiterStartPosition = -1;
            return ODataBatchReaderStreamScanResult.NoMatch;
        }

        /// <summary>
        /// Scans the current buffer for a line end.
        /// </summary>
        /// <param name="scanStartIx">The index at which to start scanning for the line terminator.</param>
        /// <param name="maxDataBytesToScan">Stop if no line end (or beginning of line end) was found after this number of non end-of-line bytes.</param>
        /// <param name="allowLeadingWhitespaceOnly">true if only whitespace data bytes are expected before the end-of-line characters; otherwise false.</param>
        /// <param name="lineEndStartPosition">The start position of the line terminator or -1 if not found.</param>
        /// <param name="lineEndEndPosition">The end position of the line terminator or -1 if not found.</param>
        /// <param name="endOfBufferReached">true if the end of the buffer was reached while scanning for the line end; otherwise false.</param>
        /// <returns>An enumeration value indicating whether the line termintor was found completely, partially or not at all.</returns>
        /// <remarks>This method only returns <see cref="ODataBatchReaderStreamScanResult.PartialMatch"/> if we found the start
        /// of a line terminator at the last character in the buffer.</remarks>
        private ODataBatchReaderStreamScanResult ScanForLineEnd(
            int scanStartIx,
            int maxDataBytesToScan,
            bool allowLeadingWhitespaceOnly,
            out int lineEndStartPosition,
            out int lineEndEndPosition,
            out bool endOfBufferReached)
        {
            endOfBufferReached = false;

            int lastByteToCheck = this.currentReadPosition + Math.Min(maxDataBytesToScan, this.numberOfBytesInBuffer) - 1;

            for (int i = scanStartIx; i <= lastByteToCheck; ++i)
            {
                char ch = (char)this.bytes[i];

                // Note the following common line resource set chars:
                // \n - UNIX   \r\n - DOS   \r - Mac
                if (ch == '\r' || ch == '\n')
                {
                    // We found a line end; now we have to check whether it
                    // consists of a second character or not.
                    lineEndStartPosition = i;

                    // We only want to report PartialMatch if we ran out of bytes in the buffer;
                    // so if we found the start of a line end in the last byte to check we still
                    // look ahead by one.
                    if (ch == '\r' && i == lastByteToCheck && maxDataBytesToScan >= this.numberOfBytesInBuffer)
                    {
                        lineEndEndPosition = -1;
                        return ODataBatchReaderStreamScanResult.PartialMatch;
                    }

                    lineEndEndPosition = lineEndStartPosition;

                    // If the next char is '\n' we found a line end consisting of two characters; adjust the end position.
                    if (ch == '\r' && (char)this.bytes[i + 1] == '\n')
                    {
                        lineEndEndPosition++;
                    }

                    return ODataBatchReaderStreamScanResult.Match;
                }
                else if (allowLeadingWhitespaceOnly && !char.IsWhiteSpace(ch))
                {
                    // We found a non-whitespace character but only whitespace characters are allowed
                    lineEndStartPosition = -1;
                    lineEndEndPosition = -1;
                    return ODataBatchReaderStreamScanResult.NoMatch;
                }
            }

            endOfBufferReached = true;
            lineEndStartPosition = -1;
            lineEndEndPosition = -1;
            return ODataBatchReaderStreamScanResult.NoMatch;
        }

        /// <summary>
        /// Check whether the bytes in the buffer at the specified start index match the expected boundary string.
        /// </summary>
        /// <param name="lineEndStartPosition">The start of the line resource set preceding the boundary (if present).</param>
        /// <param name="boundaryDelimiterStartPosition">The start position of the boundary delimiter.</param>
        /// <param name="boundary">The boundary string to check for.</param>
        /// <param name="boundaryStartPosition">If a match is detected, the start of the boundary delimiter,
        /// i.e., either the start of the leading line resource set or of the leading dashes.</param>
        /// <param name="boundaryEndPosition">If a match is detected, the position of the boundary end; otherwise -1.</param>
        /// <param name="isEndBoundary">true if the detected boundary is an end boundary; otherwise false.</param>
        /// <returns>An <see cref="ODataBatchReaderStreamScanResult"/> indicating whether a match, a partial match or no match was found.</returns>
        private ODataBatchReaderStreamScanResult MatchBoundary(
            int lineEndStartPosition,
            int boundaryDelimiterStartPosition,
            string boundary,
            out int boundaryStartPosition,
            out int boundaryEndPosition,
            out bool isEndBoundary)
        {
            boundaryStartPosition = -1;
            boundaryEndPosition = -1;

            int bufferLastByte = this.currentReadPosition + this.numberOfBytesInBuffer - 1;
            int boundaryEndPositionAfterLineFeed =
                boundaryDelimiterStartPosition + TwoDashesLength + boundary.Length + TwoDashesLength - 1;

            // NOTE: for simplicity reasons we require that the full end (!) boundary plus the maximum size
            //       of the line terminator fits into the buffer to get a non-partial match.
            //       By doing so we can reliably detect whether we are dealing with an end boundary or not.
            //       Otherwise the logic to figure out whether we found a boundary or not is riddled with
            //       corner cases that only complicate the code.
            bool isPartialMatch;
            int matchLength;
            if (bufferLastByte < boundaryEndPositionAfterLineFeed + MaxLineFeedLength)
            {
                isPartialMatch = true;
                matchLength = Math.Min(bufferLastByte, boundaryEndPositionAfterLineFeed) - boundaryDelimiterStartPosition + 1;
            }
            else
            {
                isPartialMatch = false;
                matchLength = boundaryEndPositionAfterLineFeed - boundaryDelimiterStartPosition + 1;
            }

            if (this.MatchBoundary(boundary, boundaryDelimiterStartPosition, matchLength, out isEndBoundary))
            {
                boundaryStartPosition = lineEndStartPosition < 0 ? boundaryDelimiterStartPosition : lineEndStartPosition;

                if (isPartialMatch)
                {
                    isEndBoundary = false;
                    return ODataBatchReaderStreamScanResult.PartialMatch;
                }
                else
                {
                    // If we fully matched the boundary compute the boundary end position
                    boundaryEndPosition = boundaryDelimiterStartPosition + TwoDashesLength + boundary.Length - 1;
                    if (isEndBoundary)
                    {
                        boundaryEndPosition += TwoDashesLength;
                    }

                    // Once we could match all the characters and delimiters of the boundary string
                    // (and the optional trailing '--') we now have to continue reading until the next
                    // line resource set that terminates the boundary. Only whitespace characters may exist
                    // after the boundary and before the line resource set.
                    int terminatingLineResourceSetStartPosition, terminatingLineFeedEndPosition;
                    bool endOfBufferReached;
                    ODataBatchReaderStreamScanResult terminatingLineFeedScanResult =
                        this.ScanForLineEnd(
                        boundaryEndPosition + 1,
                        int.MaxValue,
                        /*allowLeadingWhitespaceOnly*/true,
                        out terminatingLineResourceSetStartPosition,
                        out terminatingLineFeedEndPosition,
                        out endOfBufferReached);

                    switch (terminatingLineFeedScanResult)
                    {
                        case ODataBatchReaderStreamScanResult.NoMatch:
                            if (endOfBufferReached)
                            {
                                // Reached the end of the buffer and only found whitespaces.
                                if (boundaryStartPosition == 0)
                                {
                                    // If the boundary starts at the first position in the buffer
                                    // and we still could not find the end of it because there are
                                    // so many whitespaces before the terminating line resource set - fail
                                    // (security limit on the whitespaces)
                                    throw new ODataException(Strings.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached(BufferLength));
                                }

                                // Report a partial match.
                                isEndBoundary = false;
                                return ODataBatchReaderStreamScanResult.PartialMatch;
                            }
                            else
                            {
                                // Found a different character than whitespace or end-of-line
                                // so we did not match the boundary.
                                break;
                            }

                        case ODataBatchReaderStreamScanResult.PartialMatch:
                            // Found only a partial line end at the end of the buffer.
                            if (boundaryStartPosition == 0)
                            {
                                // If the boundary starts at the first position in the buffer
                                // and we still could not find the end of it because there are
                                // so many whitespaces before the terminating line resource set - fail
                                // (security limit on the whitespaces)
                                throw new ODataException(Strings.ODataBatchReaderStreamBuffer_BoundaryLineSecurityLimitReached(BufferLength));
                            }

                            // Report a partial match.
                            isEndBoundary = false;
                            return ODataBatchReaderStreamScanResult.PartialMatch;

                        case ODataBatchReaderStreamScanResult.Match:
                            // At this point we only found whitespace characters and then the terminating line resource set;
                            // adjust the boundary end position
                            boundaryEndPosition = terminatingLineFeedEndPosition;
                            return ODataBatchReaderStreamScanResult.Match;

                        default:
                            throw new ODataException(Strings.General_InternalError(InternalErrorCodes.ODataBatchReaderStreamBuffer_ScanForBoundary));
                    }
                }
            }

            return ODataBatchReaderStreamScanResult.NoMatch;
        }

        /// <summary>
        /// Try to match the specified boundary string starting at the specified position.
        /// </summary>
        /// <param name="boundary">The boundary string to search for; this does not include
        /// the leading line terminator or the leading dashes.</param>
        /// <param name="startIx">The index at which to start matching the boundary.</param>
        /// <param name="matchLength">The number of characters to match.</param>
        /// <param name="isEndBoundary">true if the boundary string is used in an end boundary; otherwise false.</param>
        /// <returns>true if it was established that the buffer starting at <paramref name="startIx"/>
        /// matches the <paramref name="boundary"/>; otherwise false.</returns>
        /// <remarks>This method also returns false if the boundary string was completly matched against the
        /// buffer but it could not be determined whether it is used in an end boundary or not.</remarks>
        private bool MatchBoundary(string boundary, int startIx, int matchLength, out bool isEndBoundary)
        {
            Debug.Assert(!string.IsNullOrEmpty(boundary), "!string.IsNullOrEmpty(boundary)");
            Debug.Assert(matchLength >= 0, "Match length must not be negative.");
            Debug.Assert(matchLength <= boundary.Length + 4, "Must never try to match more than the boundary string and the delimiting '--' on each side.");
            Debug.Assert(startIx + matchLength <= this.currentReadPosition + this.numberOfBytesInBuffer, "Match length must not exceed buffer.");

            isEndBoundary = false;

            if (matchLength == 0)
            {
                return true;
            }

            int trailingDashes = 0;
            int currentIx = startIx;

            // Shift the range by 2 so that 'i' will line up with the index of the character in the boundary
            // string. 'i < 0' means check for a leading '-', 'i >= boundary.Length' means check for a trailing '-'.
            for (int i = -TwoDashesLength; i < matchLength - TwoDashesLength; ++i)
            {
                if (i < 0)
                {
                    // Check for a leading '-'
                    if (this.bytes[currentIx] != '-')
                    {
                        return false;
                    }
                }
                else if (i < boundary.Length)
                {
                    // Compare against the character in the boundary string
                    if (this.bytes[currentIx] != boundary[i])
                    {
                        return false;
                    }
                }
                else
                {
                    // Check for a trailing '-'
                    if (this.bytes[currentIx] != '-')
                    {
                        // We matched the full boundary; return true but it is not an end boundary.
                        return true;
                    }

                    trailingDashes++;
                }

                currentIx++;
            }

            Debug.Assert(trailingDashes <= TwoDashesLength, "Should never look for more than " + TwoDashesLength + " trailing dashes.");
            isEndBoundary = trailingDashes == TwoDashesLength;
            return true;
        }

        /// <summary>
        /// Shifts all bytes in the buffer after a specified start index to the beginning of the buffer.
        /// </summary>
        /// <param name="startIndex">The start index where to start shifting.</param>
        private void ShiftToBeginning(int startIndex)
        {
            Debug.Assert(startIndex >= this.currentReadPosition, "Start of shift must be greater or equal than current position.");

            int bytesToShift = this.currentReadPosition + this.numberOfBytesInBuffer - startIndex;

            this.currentReadPosition = 0;

            if (bytesToShift <= 0)
            {
                // Nothing to shift; start index is too large
                this.numberOfBytesInBuffer = 0;
            }
            else
            {
                this.numberOfBytesInBuffer = bytesToShift;
                Buffer.BlockCopy(this.bytes, startIndex, this.bytes, 0, bytesToShift);
            }
        }
    }
}
