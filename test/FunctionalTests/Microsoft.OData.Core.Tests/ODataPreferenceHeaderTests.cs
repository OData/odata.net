//---------------------------------------------------------------------
// <copyright file="ODataPreferenceHeaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using Xunit;

namespace Microsoft.OData.Tests
{
    public class ODataPreferenceHeaderTests
    {
        private IODataRequestMessage requestMessage;
        private ODataPreferenceHeader preferHeader;
        private const string PreferHeaderName = "Prefer";
        private const string ReturnMinimalPreference = "return=minimal";
        private const string ReturnRepresentationPreference = "return=representation";
        private const string ODataAnnotationPreferenceToken = "odata.include-annotations";
        private const string AnnotationFilter = "*";
        private const string ODataAnnotationPreference = "odata.include-annotations=\"" + AnnotationFilter + "\"";
        private const string Preference1 = "preference1";
        private const string Preference2 = "preference2=value2";
        private const string Preference3 = "preference3=value3;p1=v1;p2";
        private const string Preference4 = "preference4;p1;p2=v2";
        private const string ExistingPreference = Preference1 + "," + Preference2 + "," + Preference3 + "," + Preference4;
        private const string RespondAyncPreference = "respond-async";
        private const string RespondAsyncAndWaitPreference = "respond-async,wait=10";
        private const string WaitPreference = "wait=10";
        private const string WaitPreferenceTokenName = "wait";
        private const string MaxPageSizePreference = "odata.maxpagesize";
        private const string TrackChangesPreference = "odata.track-changes";
        private const string ContinueOnErrorPreference = "odata.continue-on-error";

        public ODataPreferenceHeaderTests()
        {
            this.requestMessage = new InMemoryMessage();
            this.preferHeader = new ODataPreferenceHeader(this.requestMessage);
        }

        [Fact]
        public void SetReturnContentToNullShouldClearReturnContentPreference()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + Preference1);
            this.preferHeader.ReturnContent = null;
            Assert.Equal(Preference1, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetReturnContentToNullShouldClearReturnNoContentPreference()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + Preference1);
            this.preferHeader.ReturnContent = null;
            Assert.Equal(Preference1, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetReturnNoContentToNullPreferHeaderShouldAddHeader()
        {
            Assert.DoesNotContain(this.requestMessage.Headers, kvp => kvp.Key.Equals(PreferHeaderName));

            this.preferHeader.ReturnContent = false;
            Assert.Equal(ReturnMinimalPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetReturnContentToEmptyPreferHeaderShouldSetHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "");
            this.preferHeader.ReturnContent = true;
            Assert.Equal(ReturnRepresentationPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

#if NETCOREAPP3_1
        [Fact]
        public void SetAnnotationFilterToEmptyShouldThrow()
        {
            Action test = () => this.preferHeader.AnnotationFilter = "";
            test.Throws<ArgumentException>(Strings.ExceptionUtils_ArgumentStringEmpty + " (Parameter 'AnnotationFilter')");
        }
#else
        [Fact]
        public void SetAnnotationFilterToEmptyShouldThrow()
        {
            Action test = () => this.preferHeader.AnnotationFilter = "";
            test.Throws<ArgumentException>(Strings.ExceptionUtils_ArgumentStringEmpty + "\r\nParameter name: AnnotationFilter");
        }
#endif
        [Fact]
        public void SetAnnotationFilterToNullShouldNoOpIfODataAnnotationPreferenceIsMissing()
        {
            this.preferHeader.AnnotationFilter = null;
            Assert.Null(this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetAnnotationFilterToNullShouldClearODataAnnotationPreferenceIfItIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ODataAnnotationPreference + "," + Preference1);
            this.preferHeader.AnnotationFilter = null;
            Assert.Equal(Preference1, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetAnnotationFilterToExistingPreferHeaderShouldAppendHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference);
            this.preferHeader.AnnotationFilter = AnnotationFilter;
            var heads = this.requestMessage.GetHeader(PreferHeaderName).Split(new[] { ',' });
            Assert.Contains(Preference1, heads);
            Assert.Contains(Preference2, heads);
            Assert.Contains(Preference3, heads);
            Assert.Contains(Preference4, heads);
            Assert.Contains(ODataAnnotationPreference, heads);
        }

        [Fact]
        public void SetPreferenceShouldReplaceExistingPreferenceOfSameName()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ODataAnnotationPreference);
            this.preferHeader.AnnotationFilter = "ns.*";
            Assert.Equal(ODataAnnotationPreferenceToken + "=\"ns.*\"", this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetReturnNoContentShouldReplaceExistingReturnContent()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ReturnMinimalPreference);
            this.preferHeader.ReturnContent = false;
            Assert.Equal(ReturnMinimalPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetReturnContentShouldReplaceExistingReturnNoContent()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference);
            this.preferHeader.ReturnContent = true;
            Assert.Equal(ReturnRepresentationPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void ReturnContentShouldReturnNullOnNullHeader()
        {
            Assert.DoesNotContain(this.requestMessage.Headers, kvp => kvp.Key == PreferHeaderName);
            Assert.False(this.preferHeader.ReturnContent.HasValue);
        }

        [Fact]
        public void ReturnContentShouldReturnNullOnEmptyHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "");
            Assert.False(this.preferHeader.ReturnContent.HasValue);
        }

        [Fact]
        public void AnnotationFilterShouldReturnNullWhenODataAnnotationsPreferenceIsMissing()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference);
            Assert.Null(this.preferHeader.AnnotationFilter);
        }

        [Fact]
        public void AnnotationFilterShouldReturnNullWhenODataAnnotationsPreferenceValueIsNotSet()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference + "," + ODataAnnotationPreferenceToken);
            Assert.Null(this.preferHeader.AnnotationFilter);
        }

        [Fact]
        public void ReturnContentShouldReturnFalseWhenReturnRepresentationIsAfterReturnMinimalPreferencesAreInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + ReturnRepresentationPreference + "," + ExistingPreference);
            Assert.False(this.preferHeader.ReturnContent);
        }

        [Fact]
        public void ReturnContentShouldReturnTrueWhenReturnRepresentationIsBeforeReturnMinimalPreferencesAreInHeader2()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ReturnMinimalPreference + "," + ExistingPreference);
            Assert.True(this.preferHeader.ReturnContent);
        }



        [Fact]
        public void ReturnContentShouldReturnFalseWhenReturnNoContentPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + ExistingPreference);
            Assert.False(this.preferHeader.ReturnContent);
        }

        [Fact]
        public void ReturnContentShouldReturnTrueWhenReturnContentPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ExistingPreference);
            Assert.True(this.preferHeader.ReturnContent);
        }

        [Fact]
        public void AnnotationFilterShouldReturnFilterWhenODataAnnotationsPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference + "," + ODataAnnotationPreference);
            Assert.Equal("*", this.preferHeader.AnnotationFilter);
        }

        [Fact]
        public void RepeatSetFilterShouldSetHeaderToLastFilter()
        {
            this.preferHeader.AnnotationFilter = "ns.*";
            this.preferHeader.AnnotationFilter = "ns.name";
            this.preferHeader.AnnotationFilter = AnnotationFilter;
            Assert.Equal(ODataAnnotationPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }


        [Fact]
        public void SetRespondAsyncToTrueShouldAppendHeader()
        {
            this.preferHeader.RespondAsync = true;
            Assert.True(this.preferHeader.RespondAsync);
            Assert.Equal(RespondAyncPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetRespondAsyncToFalseShouldClearHeader()
        {
            this.preferHeader.RespondAsync = false;
            Assert.False(this.preferHeader.RespondAsync);
            Assert.Null(this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetWaitShouldAppendHeaderWhenRespondAsycIsTrue()
        {
            this.preferHeader.RespondAsync = true;
            this.preferHeader.Wait = 10;
            Assert.True(this.preferHeader.RespondAsync);
            Assert.Equal(10, this.preferHeader.Wait);
            Assert.Equal(RespondAsyncAndWaitPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetWaitShouldAppendHeader()
        {
            this.preferHeader.Wait = 10;
            Assert.Equal(10, this.preferHeader.Wait);
            Assert.Equal(WaitPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetWaitToNullShouldClearHeader()
        {
            this.preferHeader.RespondAsync = true;
            this.preferHeader.Wait = null;
            Assert.True(this.preferHeader.RespondAsync);
            Assert.Null(this.preferHeader.Wait);
            Assert.Equal(RespondAyncPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void ReturnWaitOfBadIntergerFormatShouldThrow()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "wait=abc");
            this.preferHeader = new ODataPreferenceHeader(this.requestMessage);
            int? wait;
            Action test = () => wait = this.preferHeader.Wait;
            test.Throws<ODataException>(
                string.Format(CultureInfo.InvariantCulture,
                "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                "abc", WaitPreferenceTokenName));
        }

        [Fact]
        public void SetContinueOnErrorToTrueShouldAppendHeader()
        {
            this.preferHeader.ContinueOnError = true;
            Assert.True(this.preferHeader.ContinueOnError);
            Assert.Equal(ContinueOnErrorPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetContinueOnErrorToFalseShouldClearHeader()
        {
            this.preferHeader.ContinueOnError = false;
            Assert.False(this.preferHeader.ContinueOnError);
            Assert.Null(this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetMaxPageSizeShouldAppendHeader()
        {
            const int MaxPageSize = 10;
            this.preferHeader.MaxPageSize = MaxPageSize;
            Assert.Equal(MaxPageSize, this.preferHeader.MaxPageSize);
            Assert.Equal(string.Format("{0}={1}", MaxPageSizePreference, MaxPageSize), this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetMaxPageSizeToNullShouldClearHeader()
        {
            this.preferHeader.MaxPageSize = null;
            Assert.Null(this.preferHeader.MaxPageSize);
            Assert.Null(this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void ReturnMaxPageSizeOfBadIntergerFormatShouldThrow()
        {
            this.requestMessage.SetHeader(PreferHeaderName, string.Format("{0}=abc", MaxPageSizePreference));
            this.preferHeader = new ODataPreferenceHeader(this.requestMessage);
            int? maxPageSize;
            Action test = () => maxPageSize = this.preferHeader.MaxPageSize;
            test.Throws<ODataException>(string.Format(CultureInfo.InvariantCulture,
                "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                "abc", MaxPageSizePreference));
        }

        [Fact]
        public void SetTrackChangesToTrueShouldAppendHeader()
        {
            this.preferHeader.TrackChanges = true;
            Assert.True(this.preferHeader.TrackChanges);
            Assert.Equal(TrackChangesPreference, this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void SetTrackChangesToFalseShouldClearHeader()
        {
            this.preferHeader.TrackChanges = false;
            Assert.False(this.preferHeader.TrackChanges);
            Assert.Null(this.requestMessage.GetHeader(PreferHeaderName));
        }

        [Fact]
        public void PreferHeaderShouldBeCaseInsensitive()
        {
            const int MaxPageSize = 10;
            this.preferHeader.MaxPageSize = MaxPageSize;
            Assert.Equal(MaxPageSize, this.preferHeader.MaxPageSize);
            string expected = $"{MaxPageSizePreference}={MaxPageSize}";
            Assert.Equal(expected, this.requestMessage.GetHeader(PreferHeaderName.ToLower()));
            Assert.Equal(expected, this.requestMessage.GetHeader(PreferHeaderName.ToUpper()));
            Assert.Equal(expected, this.requestMessage.GetHeader("pReFer"));
        }
    }
}
