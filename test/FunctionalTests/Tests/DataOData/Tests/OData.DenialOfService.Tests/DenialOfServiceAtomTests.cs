//---------------------------------------------------------------------
// <copyright file="DenialOfServiceAtomTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Reliability.ODataSecurityTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.OData.Core;
    using Microsoft.Test.Taupo.Execution;

    public partial class ODataSecurityTestModule
    {
        [TestCase]
        public class DenialOfServiceAtomTests : TestCase
        {
            [Variation]
            public void EntryWithUnlimitedIdValue()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>";
                var cycleString = "UnlimitedId";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedEditLink()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><link rel=\"edit\" href=\"http://";
                var cycleString = "UnlimitedEditLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedETag()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\" m:etag=\"";
                var cycleString = "UnlimitedETag";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedReadLink()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><link rel=\"self\" href=\"http://";
                var cycleString = "UnlimitedReadLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedTypeName()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><category term=\"";
                var cycleString = "UnlimitedTypeName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedContentType()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-19T23:47:49Z</updated><author><name /></author><content type=\"";
                var cycleString = "UnlimitedMediaResourceContentType";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedEditLink()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-19T23:53:57Z</updated><author><name /></author><link rel=\"edit-media\" href=\"http://";
                var cycleString = "UnlimitedMediaResourceEditLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedETag()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-19T23:57:08Z</updated><author><name /></author><link rel=\"edit-media\" href=\"http://editlink/\" m:etag=\"";
                var cycleString = "UnlimitedMediaResourceETag";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithMediaResourceWithUnlimitedReadLink()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-20T00:14:50Z</updated><author><name /></author><content type=\"MediaResourceContentValue\" src=\"http://";
                var cycleString = "UnlimitedMediaResourceReadLink";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedMetadata()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T00:22:33Z</updated><author><name /></author><m:action metadata=\"http://";
                var cycleString = "UnlimitedActionMetadata";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedTarget()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T00:22:33Z</updated><author><name /></author><m:action metadata=\"http://ActionMetadata/\" target=\"http://";
                var cycleString = "UnlimitedActionTarget";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithActionWithUnlimitedTitle()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T00:37:13Z</updated><author><name /></author><m:action metadata=\"http://ActionMetadata/\" title=\"";
                var cycleString = "UnlimitedActionTitle";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumbersOfTheSameActions()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T00:49:57Z</updated><author><name /></author>";
                var cycleString = "<m:action metadata=\"http://actionmetadata/\" title=\"ActionTitle\" target=\"http://actiontarget/\" />";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumbersOfActions()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T00:49:57Z</updated><author><name /></author>";
                var cycleString = "<m:action metadata=\"http://actionmetadata[Count]/\" title=\"ActionTitle[Count]\" target=\"http://actiontarget[Count]/\" />";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithAssociationLinkWithUnlimitedName()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-20T02:07:57Z</updated><author><name /></author><link rel=\"http://schemas.microsoft.com/ado/2007/08/dataservices/relatedlinks/";
                var cycleString = "UnlimitedAssociationLinkName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithAssociationLinkWithUnlimitedTitle()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-20T02:07:57Z</updated><author><name /></author><link rel=\"http://schemas.microsoft.com/ado/2007/08/dataservices/relatedlinks/AssociationLinkName\" type=\"application/xml\" title=\"";
                var cycleString = "UnlimitedAssociationLinkTitle";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithAssociationLinkWithUnlimitedUrl()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-20T02:07:57Z</updated><author><name /></author><link rel=\"http://schemas.microsoft.com/ado/2007/08/dataservices/relatedlinks/AssociationLinkName\" type=\"application/xml\" title=\"AssociationLinkName\" href=\"http://";
                var cycleString = "UnlimitedAssociationLinkUrl";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfAssociationLinks()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\"><id>Id</id><title /><updated>2011-12-20T02:07:57Z</updated><author><name /></author>";
                var cycleString = "<link rel=\"http://schemas.microsoft.com/ado/2007/08/dataservices/relatedlinks/AssociationLinkName[Count]\" type=\"application/xml\" title=\"AssociationLinkName[Count]\" href=\"http://AssociationLinkUrl[Count]/\" />";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithFunctionWithUnlimitedMetadata()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:31:13Z</updated><author><name /></author><m:function metadata=\"http://";
                var cycleString = "FunctionUnlimitedMetadata";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithFunctionWithUnlimitedTarget()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:31:13Z</updated><author><name /></author><m:function metadata=\"http://FunctionMetadata/\" target=\"http://";
                var cycleString = "FunctionUnlimitedTarget";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfTheSameFunctions()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:31:13Z</updated><author><name /></author>";
                var cycleString = "<m:function metadata=\"http://FunctionMetadata/\" target=\"http://FunctionTarget/\" />";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfFunctions()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:31:13Z</updated><author><name /></author>";
                var cycleString = "<m:function metadata=\"http://FunctionMetadata[Count]/\" target=\"http://FunctionTarget[Count]/\" />";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithPropertyWithUnlimitedName()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:43:37Z</updated><author><name /></author><content type=\"application/xml\"><m:properties><d:";
                var cycleString = "PropertyUnlimitedName";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithPropertyWithUnlimitedValue()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:43:37Z</updated><author><name /></author><content type=\"application/xml\"><m:properties><d:ProperyName>";
                var cycleString = "PropertyUnlimitedValue";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithUnlimitedNumberOfProperties()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:43:37Z</updated><author><name /></author><content type=\"application/xml\"><m:properties>";
                var cycleString = "<d:ProperyName[Count]>PropertyValue[Count]</d:ProperyName[Count]>";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void EntryWithPropertiesOfUnlimitedDepth()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><entry xmlns=\"http://www.w3.org/2005/Atom\" xmlns:d=\"http://schemas.microsoft.com/ado/2007/08/dataservices\" xmlns:m=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"><id>Id</id><title /><updated>2011-12-20T02:43:37Z</updated><author><name /></author><content type=\"application/xml\"><m:properties>";
                var cycleString = "<d:ProperyName[Count]>";

                this.CreateAndReadUnlimitedResponseMessageWithEntry(startingString, cycleString);
            }

            [Variation]
            public void FeedWithUnlimitedFeedId()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><feed xmlns=\"http://www.w3.org/2005/Atom\"><id>";
                var cycleString = "UnlimitedFeedId";

                this.CreateAndReadUnlimitedResponseMessageWithFeed(startingString, cycleString);
            }

            [Variation]
            public void FeedWithUnlimitedNextPageLink()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><feed xmlns=\"http://www.w3.org/2005/Atom\"><id>FeedId</id><title /><updated>2011-12-20T19:55:09Z</updated><author><name /></author><link rel=\"next\" href=\"http://";
                var cycleString = "UnlimitedNextPageLink";

                this.CreateAndReadUnlimitedResponseMessageWithFeed(startingString, cycleString);
            }

            [Variation]
            private void FeedWithUnlimitedNumberOfEntries()
            {
                var startingString = "<?xml version=\"1.0\" encoding=\"utf-8\"?><feed xmlns=\"http://www.w3.org/2005/Atom\"><id>FeedId</id><title /><updated>2011-12-20T21:34:39Z</updated>";
                var cycleString = "<entry><id>Id[Count]</id><title /><updated>2011-12-20T21:34:39Z</updated><author><name /></author><content type=\"application/xml\" /></entry>";

                this.CreateAndReadUnlimitedResponseMessageWithFeed(startingString, cycleString);
            }

            private void CreateAndReadUnlimitedResponseMessageWithEntry(string startingString, string cycleString)
            {
                var responseMessage = new ODataUnlimitedResponseMessage(startingString, cycleString);
                responseMessage.SetHeader("Content-Type", "application/atom+xml");
                responseMessage.SetHeader("DataServiceVersion", "3.0");

                var readLimitReached = false;
                try
                {
                    using (var messageReader = new ODataMessageReader(responseMessage))
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

            private void CreateAndReadUnlimitedResponseMessageWithFeed(string startingString, string cycleString)
            {
                var responseMessage = new ODataUnlimitedResponseMessage(startingString, cycleString);
                responseMessage.SetHeader("Content-Type", "application/atom+xml");
                responseMessage.SetHeader("DataServiceVersion", "3.0");

                var readLimitReached = false;
                try
                {
                    using (var messageReader = new ODataMessageReader(responseMessage))
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
    }
}