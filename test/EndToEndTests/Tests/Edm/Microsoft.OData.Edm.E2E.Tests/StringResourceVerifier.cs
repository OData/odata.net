﻿//---------------------------------------------------------------------
// <copyright file="StringResourceVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Globalization;

namespace Microsoft.OData.Edm.E2E.Tests;

/// <summary>
/// Helpers for verifying strings that are pulled from a localized resources
/// </summary>
public class StringResourceVerifier
{
    private IResourceLookup lookup;

    /// <summary>
    /// Initializes a new instance of the StringResourceVerifier class.
    /// </summary>
    /// <param name="lookup">Lookup to be used for locating string resources</param>
    public StringResourceVerifier(IResourceLookup lookup)
    {
        this.lookup = lookup;
    }

    /// <summary>
    /// Determines if the supplied string is an instance of the string defined in localized resources
    /// </summary>
    /// <param name="expectedResourceKey">The key of the resource string to match against</param>
    /// <param name="actualMessage">String value to be verified</param>
    /// <param name="stringParameters">
    /// Expected values for string.Format placeholders in the resource string
    /// If none are supplied then any values for placeholders in the resource string will count as a match
    /// </param>
    /// <returns>True if the string matches, false otherwise</returns>
    public bool IsMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters)
    {
        return this.IsMatch(expectedResourceKey, actualMessage, true, stringParameters);
    }

    /// <summary>
    /// Determines if the supplied string is an instance of the string defined in localized resources
    /// </summary>
    /// <param name="expectedResourceKey">The key of the resource string to match against</param>
    /// <param name="actualMessage">String value to be verified</param>
    /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
    /// <param name="stringParameters">
    /// Expected values for string.Format placeholders in the resource string
    /// If none are supplied then any values for placeholders in the resource string will count as a match
    /// </param>
    /// <returns>True if the string matches, false otherwise</returns>
    public bool IsMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters)
    {
        string messageFromResources;
        return this.IsMatch(expectedResourceKey, actualMessage, isExactMatch, stringParameters, out messageFromResources);
    }

    /// <summary>
    /// Verified the supplied string is an instance of the string defined in resources
    /// </summary>
    /// <param name="expectedResourceKey">The key of the resource string to match against</param>
    /// <param name="actualMessage">String value to be verified</param>
    /// <param name="stringParameters">
    /// Expected values for string.Format placeholders in the resource string
    /// If none are supplied then any values for placeholders in the resource string will count as a match
    /// </param>
    /// <exception cref="DataComparisonException">Thrown if the string does not match</exception>
    public void VerifyMatch(string expectedResourceKey, string actualMessage, params object[] stringParameters)
    {
        this.VerifyMatch(expectedResourceKey, actualMessage, true, stringParameters);
    }

    /// <summary>
    /// Determines if the supplied string is an instance of the string defined in localized resources
    /// If the string in the resource file contains string.Format place holders then the actual message can contain any values for these
    /// </summary>
    /// <param name="expectedResourceKey">The key of the resource string to match against</param>
    /// <param name="actualMessage">String value to be verified</param>
    /// <param name="isExactMatch">Determines whether the exception message must be exact match of the message in the resource file, or just contain it.</param>
    /// <param name="stringParameters">
    /// Expected values for string.Format placeholders in the resource string
    /// If none are supplied then any values for placeholders in the resource string will count as a match
    /// </param>
    public void VerifyMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, params object[] stringParameters)
    {
        string messageFromResources;
        if (!this.IsMatch(expectedResourceKey, actualMessage, isExactMatch, stringParameters, out messageFromResources))
        {
            throw new ArgumentException($"Actual string does not match the message defined in resources. ExpectedValue={messageFromResources} : ActualValue={actualMessage}");
        }
    }

    private static bool IsMatchWithAnyPlaceholderValues(string expectedMessage, string actualMessage, bool isExactMatch)
    {
        // Find the sections of the Exception message seperated by {x} tags
        var sections = FindMessageSections(expectedMessage);

        // Check that each section is present in the actual message in correct order
        int indexToCheckFrom = 0;
        foreach (var section in sections)
        {
            // Check if it is present in the actual message
            int index = actualMessage.IndexOf(section, indexToCheckFrom, StringComparison.Ordinal);
            if (index < 0)
            {
                return false;
            }

            if (section.Length == 0 && section == sections.Last())
            {
                // If the last section is a zero-length string
                // then this indicates that the placeholder is the
                // last thing in the resource. Thus every section
                // matched and the placeholder takes up the rest of string
                // from the actual message.
                return true;
            }
            else
            {
                // continue checking from the end of this section
                // (Ensures sections are in the correct order)
                indexToCheckFrom = index + section.Length;
            }
        }

        // If we reach the end then everything is a match
        return isExactMatch ? indexToCheckFrom == actualMessage.Length : indexToCheckFrom <= actualMessage.Length;
    }

    private static IEnumerable<string> FindMessageSections(string messageFromResources)
    {
        // Find the start and end index of each section of the string
        var sections = new List<StringSection>();

        // Start with a section that spans the whole string
        // While there are still place holders shorten the previous section to end at the next { and start a new section from the following }
        sections.Add(new StringSection { StartIndex = 0, EndIndex = messageFromResources.Length });
        int indexToCheckFrom = 0;
        while (messageFromResources.IndexOf("{", indexToCheckFrom, StringComparison.Ordinal) >= 0)
        {
            // Find the indexes to split the new section around
            var previous = sections.Last();
            int previousEndIndex = messageFromResources.IndexOf("{", indexToCheckFrom, StringComparison.Ordinal);
            int nextStartIndex = messageFromResources.IndexOf("}", previousEndIndex, StringComparison.Ordinal) + 1;

            // If there are no remaining closing tags then we are done
            if (nextStartIndex == 0)
            {
                break;
            }
            else
            {
                // Contents of place holder must be integer
                int temp;
                bool intContents = int.TryParse(messageFromResources.Substring(previousEndIndex + 1, nextStartIndex - previousEndIndex - 2), out temp);

                // Place holder must not be escaped (i.e. {{0}})
                bool escaped = messageFromResources[previousEndIndex] == '{'
                    && nextStartIndex < messageFromResources.Length
                    && messageFromResources[nextStartIndex] == '}';

                if (!intContents || escaped)
                {
                    indexToCheckFrom++;
                    continue;
                }
            }

            // Shorten the previous section to end at the {
            previous.EndIndex = previousEndIndex;

            // Add the remaining string after the } into a new section,
            // even if the '}' is the last character in the string. This
            // helps verification ensure that there is actually a wildcard
            // at this point in the string instead of the string ending
            // without a wildcard.
            sections.Add(new StringSection { StartIndex = nextStartIndex, EndIndex = messageFromResources.Length });
            indexToCheckFrom = nextStartIndex;
        }

        // Pull out the sections
        return sections.Select(s => messageFromResources.Substring(s.StartIndex, s.EndIndex - s.StartIndex).Replace("{{", "{").Replace("}}", "}"));
    }

    private bool IsMatch(string expectedResourceKey, string actualMessage, bool isExactMatch, object[] stringParameters, out string messageFromResources)
    {
        ExceptionUtilities.CheckStringArgumentIsNotNullOrEmpty(expectedResourceKey, "expectedResourceKey");
        ExceptionUtilities.CheckArgumentNotNull(actualMessage, "actualMessage");

        messageFromResources = this.lookup.LookupString(expectedResourceKey);

        if (stringParameters.Length == 0)
        {
            return IsMatchWithAnyPlaceholderValues(messageFromResources, actualMessage, isExactMatch);
        }
        else
        {
            messageFromResources = string.Format(CultureInfo.InvariantCulture, messageFromResources, stringParameters);

            return isExactMatch ? actualMessage == messageFromResources : actualMessage.Contains(messageFromResources);
        }
    }

    /// <summary>
    /// Represents a section of a string
    /// </summary>
    private class StringSection
    {
        /// <summary>
        /// Gets or sets the index the section starts at
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the index the section ends at
        /// </summary>
        public int EndIndex { get; set; }
    }
}
