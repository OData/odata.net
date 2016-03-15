//---------------------------------------------------------------------
// <copyright file="ODataPreferenceHeaderTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Globalization;
using FluentAssertions;
using Xunit;

namespace Microsoft.OData.Core.Tests
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
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(Preference1);
        }

        [Fact]
        public void SetReturnContentToNullShouldClearReturnNoContentPreference()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + Preference1);
            this.preferHeader.ReturnContent = null;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(Preference1);
        }

        [Fact]
        public void SetReturnNoContentToNullPreferHeaderShouldAddHeader()
        {
            this.requestMessage.Headers.Should().NotContain(kvp => kvp.Key.Equals(PreferHeaderName));
            this.preferHeader.ReturnContent = false;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ReturnMinimalPreference);
        }

        [Fact]
        public void SetReturnContentToEmptyPreferHeaderShouldSetHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "");
            this.preferHeader.ReturnContent = true;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ReturnRepresentationPreference);
        }

        [Fact]
        public void SetAnnotationFilterToEmptyShouldThrow()
        {
            Action test = () => this.preferHeader.AnnotationFilter = "";
            test.ShouldThrow<ArgumentException>().WithMessage(Strings.ExceptionUtils_ArgumentStringEmpty + "\r\nParameter name: AnnotationFilter");
        }

        [Fact]
        public void SetAnnotationFilterToNullShouldNoOpIfODataAnnotationPreferenceIsMissing()
        {
            this.preferHeader.AnnotationFilter = null;
            this.requestMessage.GetHeader(PreferHeaderName).Should().BeNull();
        }

        [Fact]
        public void SetAnnotationFilterToNullShouldClearODataAnnotationPreferenceIfItIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ODataAnnotationPreference + "," + Preference1);
            this.preferHeader.AnnotationFilter = null;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(Preference1);
        }

        [Fact]
        public void SetAnnotationFilterToExistingPreferHeaderShouldAppendHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference);
            this.preferHeader.AnnotationFilter = AnnotationFilter;
            this.requestMessage.GetHeader(PreferHeaderName).Split(new[] { ',' })
                .Should().Contain(Preference1)
                .And.Contain(Preference2)
                .And.Contain(Preference3)
                .And.Contain(Preference4)
                .And.Contain(ODataAnnotationPreference);
        }

        [Fact]
        public void SetPreferenceShouldReplaceExistingPreferenceOfSameName()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ODataAnnotationPreference);
            this.preferHeader.AnnotationFilter = "ns.*";
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ODataAnnotationPreferenceToken + "=\"ns.*\"");
        }

        [Fact]
        public void SetReturnNoContentShouldReplaceExistingReturnContent()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ReturnMinimalPreference);
            this.preferHeader.ReturnContent = false;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ReturnMinimalPreference);
        }

        [Fact]
        public void SetReturnContentShouldReplaceExistingReturnNoContent()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference);
            this.preferHeader.ReturnContent = true;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ReturnRepresentationPreference);
        }

        [Fact]
        public void ReturnContentShouldReturnNullOnNullHeader()
        {
            this.requestMessage.Headers.Should().NotContain(kvp => kvp.Key == PreferHeaderName);
            this.preferHeader.ReturnContent.HasValue.Should().BeFalse();
        }

        [Fact]
        public void ReturnContentShouldReturnNullOnEmptyHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "");
            this.preferHeader.ReturnContent.HasValue.Should().BeFalse();
        }

        [Fact]
        public void AnnotationFilterShouldReturnNullWhenODataAnnotationsPreferenceIsMissing()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference);
            this.preferHeader.AnnotationFilter.Should().BeNull();
        }

        [Fact]
        public void ReturnContentShouldReturnFalseWhenReturnRepresentationIsAfterReturnMinimalPreferencesAreInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + ReturnRepresentationPreference + "," + ExistingPreference);
            this.preferHeader.ReturnContent.Should().BeFalse();
        }

        [Fact]
        public void ReturnContentShouldReturnTrueWhenReturnRepresentationIsBeforeReturnMinimalPreferencesAreInHeader2()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ReturnMinimalPreference + "," + ExistingPreference);
            this.preferHeader.ReturnContent.Should().BeTrue();
        }



        [Fact]
        public void ReturnContentShouldReturnFalseWhenReturnNoContentPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnMinimalPreference + "," + ExistingPreference);
            this.preferHeader.ReturnContent.Should().BeFalse();
        }

        [Fact]
        public void ReturnContentShouldReturnTrueWhenReturnContentPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ReturnRepresentationPreference + "," + ExistingPreference);
            this.preferHeader.ReturnContent.Should().BeTrue();
        }

        [Fact]
        public void AnnotationFilterShouldReturnFilterWhenODataAnnotationsPreferenceIsInHeader()
        {
            this.requestMessage.SetHeader(PreferHeaderName, ExistingPreference + "," + ODataAnnotationPreference);
            this.preferHeader.AnnotationFilter.Should().Be("*");
        }

        [Fact]
        public void RepeatSetFilterShouldSetHeaderToLastFilter()
        {
            this.preferHeader.AnnotationFilter = "ns.*";
            this.preferHeader.AnnotationFilter = "ns.name";
            this.preferHeader.AnnotationFilter = AnnotationFilter;
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ODataAnnotationPreference);
        }


        [Fact]
        public void SetRespondAsyncToTrueShouldAppendHeader()
        {
            this.preferHeader.RespondAsync = true;
            this.preferHeader.RespondAsync.Should().BeTrue();
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(RespondAyncPreference);
        }

        [Fact]
        public void SetRespondAsyncToFalseShouldClearHeader()
        {
            this.preferHeader.RespondAsync = false;
            this.preferHeader.RespondAsync.Should().BeFalse();
            this.requestMessage.GetHeader(PreferHeaderName).Should().BeNull();
        }

        [Fact]
        public void SetWaitShouldAppendHeaderWhenRespondAsycIsTrue()
        {
            this.preferHeader.RespondAsync = true;
            this.preferHeader.Wait = 10;
            this.preferHeader.RespondAsync.Should().BeTrue();
            this.preferHeader.Wait.Should().Be(10);
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(RespondAsyncAndWaitPreference);
        }

        [Fact]
        public void SetWaitShouldAppendHeader()
        {
            this.preferHeader.Wait = 10;
            this.preferHeader.Wait.Should().Be(10);
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(WaitPreference);
        }

        [Fact]
        public void SetWaitToNullShouldClearHeader()
        {
            this.preferHeader.RespondAsync = true;
            this.preferHeader.Wait = null;
            this.preferHeader.RespondAsync.Should().BeTrue();
            this.preferHeader.Wait.Should().Be(null);
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(RespondAyncPreference);
        }

        [Fact]
        public void ReturnWaitOfBadIntergerFormatShouldThrow()
        {
            this.requestMessage.SetHeader(PreferHeaderName, "wait=abc");
            this.preferHeader = new ODataPreferenceHeader(this.requestMessage);
            int? wait;
            Action test = () => wait = this.preferHeader.Wait;
            test.ShouldThrow<ODataException>().WithMessage(
                string.Format(CultureInfo.InvariantCulture,
                "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                "abc", WaitPreferenceTokenName));
        }

        [Fact]
        public void SetContinueOnErrorToTrueShouldAppendHeader()
        {
            this.preferHeader.ContinueOnError = true;
            this.preferHeader.ContinueOnError.Should().BeTrue();
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(ContinueOnErrorPreference);
        }

        [Fact]
        public void SetContinueOnErrorToFalseShouldClearHeader()
        {
            this.preferHeader.ContinueOnError = false;
            this.preferHeader.ContinueOnError.Should().BeFalse();
            this.requestMessage.GetHeader(PreferHeaderName).Should().BeNull();
        }

        [Fact]
        public void SetMaxPageSizeShouldAppendHeader()
        {
            const int MaxPageSize = 10;
            this.preferHeader.MaxPageSize = MaxPageSize;
            this.preferHeader.MaxPageSize.Should().Be(MaxPageSize);
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(string.Format("{0}={1}", MaxPageSizePreference, MaxPageSize));
        }

        [Fact]
        public void SetMaxPageSizeToNullShouldClearHeader()
        {
            this.preferHeader.MaxPageSize = null;
            this.preferHeader.MaxPageSize.Should().Be(null);
            this.requestMessage.GetHeader(PreferHeaderName).Should().BeNull();
        }

        [Fact]
        public void ReturnMaxPageSizeOfBadIntergerFormatShouldThrow()
        {
            this.requestMessage.SetHeader(PreferHeaderName, string.Format("{0}=abc", MaxPageSizePreference));
            this.preferHeader = new ODataPreferenceHeader(this.requestMessage);
            int? maxPageSize;
            Action test = () => maxPageSize = this.preferHeader.MaxPageSize;
            test.ShouldThrow<ODataException>().WithMessage(
                string.Format(CultureInfo.InvariantCulture,
                "Invalid value '{0}' for {1} preference header found. The {1} preference header requires an integer value.",
                "abc", MaxPageSizePreference));
        }

        [Fact]
        public void SetTrackChangesToTrueShouldAppendHeader()
        {
            this.preferHeader.TrackChanges = true;
            this.preferHeader.TrackChanges.Should().BeTrue();
            this.requestMessage.GetHeader(PreferHeaderName).Should().Be(TrackChangesPreference);
        }

        [Fact]
        public void SetTrackChangesToFalseShouldClearHeader()
        {
            this.preferHeader.TrackChanges = false;
            this.preferHeader.TrackChanges.Should().BeFalse();
            this.requestMessage.GetHeader(PreferHeaderName).Should().BeNull();
        }
    }
}
