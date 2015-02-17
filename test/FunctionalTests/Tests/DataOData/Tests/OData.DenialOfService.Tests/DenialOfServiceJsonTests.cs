//---------------------------------------------------------------------
// <copyright file="DenialOfServiceJsonTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Reliability.ODataSecurityTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Execution;

    public partial class ODataSecurityTestModule
    {
        [TestCase]
        public class DenialOfServiceJsonTests : TestCase
        {
            [Variation]
            public void EntryWithUnlimitedIdValue()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"";
                var cycleString = "UnlimitedId";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedEditOrReadLink()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"uri\":\"http://";
                var cycleString = "UnlimitedEditOrReadLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedETag()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"etag\":\"";
                var cycleString = "UnlimitedETag";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedTypeName()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"type\":\"";
                var cycleString = "UnlimitedTypeName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedContentType()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"media_src\":\"http://readlink/\",\"content_type\":\"";
                var cycleString = "UnlimitedMediaResourceContentType";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedEditOrReadLink()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"edit_media\":\"";
                var cycleString = "UnlimitedMediaResourceEditOrReadLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedETag()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"edit_media\":\"http://editlink/\",\"media_etag\":\"";
                var cycleString = "UnlimitedMediaResourceETag";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedMetadata()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"actions\":{\"http://";
                var cycleString = "UnlimitedActionMetadata";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedTarget()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"actions\":{\"http://actionmetadata/\":[{\"target\":\"http://";
                var cycleString = "UnlimitedActionTarget";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedTitle()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"actions\":{\"http://actionmetadata/\":[{\"title\":\"";
                var cycleString = "UnlimitedActionTitle";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumbersOfTheSameActions()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"actions\":{\"http://actionmetadata/\":[";
                var cycleString = "{\"title\":\"ActionTitle\",\"target\":\"http://actiontarget/\"},";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumbersOfActions()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"actions\":{";
                var cycleString = "\"http://actionmetadata[Count]/\":[{\"title\":\"ActionTitle[Count]\",\"target\":\"http://actiontarget[Count]/\"}],";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithAssociationLinkWithUnlimitedName()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"properties\":{\"";
                var cycleString = "UnlimitedAssociationLinkName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithAssociationLinkWithUnlimitedUrl()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"properties\":{\"AssociationLinkName\":{\"associationuri\":\"http://";
                var cycleString = "UnlimitedAssociationLinkUrl";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfAssociationLinks()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"properties\":{";
                var cycleString = "\"AssociationLinkName[Count]\":{\"associationuri\":\"http://associationlinknameurl[Count]/\"},";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithFunctionWithUnlimitedMetadata()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"functions\":{\"http://";
                var cycleString = "FunctionUnlimitedMetadata";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithFunctionWithUnlimitedTitle()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"functions\":{\"http://functionmetadata/\":[{\"title\":\"";
                var cycleString = "FunctionUnlimitedTitle";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithFunctionWithUnlimitedTarget()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"functions\":{\"http://functionmetadata/\":[{\"title\":\"FunctionTitle\",\"target\":\"http://";
                var cycleString = "FunctionUnlimitedTarget";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfTheSameFunctions()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"functions\":{\"http://functionmetadata/\":[";
                var cycleString = "{\"title\":\"FunctionTitle\",\"target\":\"http://functiontarget/\"},";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfFunctions()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\",\"functions\":{";
                var cycleString = "\"http://functionmetadata[Count]/\":[{\"title\":\"FunctionTitle[Count]\",\"target\":\"http://functiontarget[Count]/\"}],";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithPropertyWithUnlimitedName()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\"},\"";
                var cycleString = "PropertyUnlimitedName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithPropertyWithUnlimitedValue()
            {
                var startingString = "{\"d\":{\"__metadata\":{\"id\":\"Id\"},\"PropertyName\":\"";
                var cycleString = "PropertyUnlimitedValue";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            ////[Variation]
            ////public void EntryWithUnlimitedNumberOfProperties()
            ////{
            ////    var startingString = string.Empty;
            ////    var cycleString = "<d:ProperyName[Count]>PropertyValue[Count]</d:ProperyName[Count]>";

            ////    this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            ////}

            [Variation]
            public void FeedWithUnlimitedNextPageLink()
            {
                var startingString = "{\"d\":{\"results\":[],\"__next\":\"http://";
                var cycleString = "UnlimitedNextPageLink";

                this.CreateAndReadResponseMessageWithFeed(startingString, cycleString);
            }

            [Variation]
            public void FeedWithUnlimitedNumberOfEntries()
            {
                var startingString = "{\"d\":{\"results\":[";
                var cycleString = "{\"__metadata\":{\"id\":\"Id\",\"type\":\"DenialOfServiceTests.Product\"},\"Id\":\"[Count]\"},";

                this.CreateAndReadResponseMessageWithFeed(startingString, cycleString);
            }

            private void CreateAndReadUnlimitedResponseMessageWithEntry(string startingString, string cycleString)
            {
                var model = this.GetModel();
                if (model != null)
                {
                    var responseMessage = new ODataUnlimitedResponseMessage(startingString, cycleString);
                    responseMessage.SetHeader("Content-Type", "application/json");
                    responseMessage.SetHeader("DataServiceVersion", "3.0");

                    var readLimitReached = false;
                    try
                    {

                        using (var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), model))
                        {
                            var entryReader = messageReader.CreateODataEntryReader();
                            entryReader.Read();
                            var result = entryReader.Item;
                            Console.WriteLine(result);
                        }
                    }
                    catch (ODataException ex)
                    {
                        if (ex.Message.StartsWith("The maximum number of bytes allowed to be read from the stream has been exceeded."))
                        {
                            readLimitReached = true;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (!readLimitReached)
                    {
                        throw new InvalidOperationException("Expecting exception, but it was not thrown.");
                    }
                }
            }

            private void CreateAndReadResponseMessageWithFeed(string startingString, string cycleString)
            {
                var model = this.GetModel();
                if (model != null)
                {
                    var responseMessage = new ODataUnlimitedResponseMessage(startingString, cycleString);
                    responseMessage.SetHeader("Content-Type", "application/json");
                    responseMessage.SetHeader("DataServiceVersion", "3.0");

                    var readLimitReached = false;
                    try
                    {

                        using (var messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), model))
                        {
                            var feedReader = messageReader.CreateODataFeedReader();
                            while (feedReader.Read())
                            {
                                switch (feedReader.State)
                                {
                                    case ODataReaderState.FeedStart:
                                        Console.WriteLine("FeedStart: " + (ODataFeed)feedReader.Item);
                                        break;

                                    case ODataReaderState.FeedEnd:
                                        Console.WriteLine("FeedEnd: " + (ODataFeed)feedReader.Item);
                                        break;

                                    case ODataReaderState.EntryStart:
                                        Console.WriteLine("EntryStart: " + (ODataEntry)feedReader.Item);
                                        break;

                                    case ODataReaderState.EntryEnd:
                                        Console.WriteLine("EntryEnd: " + (ODataEntry)feedReader.Item);
                                        break;

                                    default:
                                        Console.WriteLine("ODataItem: " + feedReader.Item);
                                        break;
                                }
                            }
                        }
                    }
                    catch (ODataException ex)
                    {
                        if (ex.Message.StartsWith("The maximum number of bytes allowed to be read from the stream has been exceeded."))
                        {
                            readLimitReached = true;
                        }
                        else
                        {
                            throw;
                        }
                    }

                    if (!readLimitReached)
                    {
                        throw new InvalidOperationException("Expecting exception, but it was not thrown.");
                    }
                }
            }

            private IEdmModel GetModel()
            {
                //use a fake model just to by-pass initial validation
                var modelString = "<Schema xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" Namespace=\"DenialOfServiceTests\"><EntityContainer Name=\"DenialOfServiceTestsContext\"><EntitySet Name=\"Products\" EntityType=\"DenialOfServiceTests.ProductProduct\" /></EntityContainer><EntityType Name=\"Product\"><Key><PropertyRef Name=\"Id\"/></Key><Property Name=\"Id\" Nullable=\"false\" Type=\"Edm.Int32\"/></EntityType></Schema>";
                var xmlReader = XmlReader.Create(new StringReader(modelString));

                IEdmModel model;
                IEnumerable<EdmError> errors;
                if (CsdlReader.TryParse(new[] { xmlReader }, out model, out errors))
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
