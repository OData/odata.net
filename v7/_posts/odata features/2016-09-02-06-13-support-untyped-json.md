---
layout: post
title: "Support undeclared & untyped property value in ODataLib and Client"
description: ""
category: "5. OData Features"
---

In ODataV4 7.0, ODataMessageReader & Writer are able to read & write arbitrary JSON as raw string in the payload. The undeclared & untyped property is supported by ODataLib and OData Client in a slightly different way than that in ODataV3.

#### In ODataLib

The ODataMessageReaderSettings & ODataMessageWriterSettings' 'Validations' property can be set with ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType to enable reading/writing undeclared&untyped value properties in payload. 

The below messageWriterSettings will enable reading undeclared / untyped property. It reads an undeclared and untype JSON as ODataUntypedValue whose .RawJson has the raw JSON string.

{% highlight csharp %}

			private ODataMessageReaderSettings readerSettings = new ODataMessageReaderSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };

            const string payload = @"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredAddress1"":"
               + @"{""@odata.type"":""Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}";
            ODataResource entry = null;
            this.ReadEntryPayload(payload, this.serverEntitySet, this.serverEntityType, reader =>
            {
                entry = reader.Item as ODataResource;
            });

            entry.Properties.Count().Should().Be(2);
            entry.Properties.Last().Value.As<ODataUntypedValue>().RawValue
                .Should().Be(@"{""@odata.type"":""Server.NS.AddressInValid"",""Street"":""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":""No.10000000999,Zixing Rd Minhang""}");

{% highlight csharp %}

And this is how to write undeclared & untyped property:

{% highlight csharp %}

            private ODataMessageWriterSettings writerSettings = new ODataMessageWriterSettings
            {
                Validations = ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType
            };
            
            var entry = new ODataResource
            {
                TypeName = "Server.NS.ServerEntityType",
                Properties = new[]
                    {
                        new ODataProperty{Name = "Id", Value = new ODataPrimitiveValue(61880128)},
                        new ODataProperty{Name = "UndeclaredFloatId", Value = new ODataPrimitiveValue(12.3D)},
                        new ODataProperty{Name = "UndeclaredAddress1", Value = 
                            new ODataUntypedValue(){RawValue=@"{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}"
                        },
                    },
            };
            string result = this.WriteEntryPayload(this.serverEntitySet, this.serverEntityType, writer =>
            {
                writer.WriteStart(entry);
                writer.WriteEnd();
            });

            result.Should().Be(@"{""@odata.context"":""http://www.sampletest.com/$metadata#serverEntitySet/$entity"",""Id"":61880128,""UndeclaredFloatId"":12.3,""UndeclaredAddress1"":{""@odata.type"":""#Server.NS.AddressInValid"",'Street':""No.999,Zixing Rd Minhang"",""UndeclaredStreet"":'No.10000000999,Zixing Rd Minhang'}}");

	
{% endhighlight %}

#### In OData Client

By default, DataServiceContext doesn't eanble reading & writing undeclared & untyped property value, unless explicitly enabled via ResponsePipeline.OnMessageReaderSettingsCreated() and RequestPipeline.OnMessageWriterSettingsCreated(). The below shows how to do that:

{% highlight csharp %}

			TestClientContext.Format.UseJson(Model);
            TestClientContext.MergeOption = ODataClient.MergeOption.OverwriteChanges;
            TestClientContext.Configurations.RequestPipeline.OnMessageWriterSettingsCreated(wsa =>
            {
                // writer supports ODataUntypedValue
                wsa.Settings.Validations &= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });
            TestClientContext.Configurations.RequestPipeline.OnEntryStarting(ea =>
            {
                if (ea.Entity.GetType() == typeof(AccountInfo))
                {
                    var undeclaredOdataProperty = new ODataProperty()
                    {
                        Name = "UndecalredOpenProperty1",
                        Value = new ODataUntypedValue() { RawValue = "{ \"sender\": \"RSS\", \"senderImage\": \"https://exchangelabs.abcexample-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.ahaha.COM\", \"summary\": \"RSS is now connected to your mailbox\", \"title\": null }" }
                    };
                    var accountInfoComplexValueProperties = ea.Entry.Properties as List<ODataProperty>;
                    accountInfoComplexValueProperties.Add(undeclaredOdataProperty);
                }
            });
            var account = TestClientContext.Accounts.Where(a => a.AccountID == 101).First();
            TestClientContext.UpdateObject(account);
            TestClientContext.SaveChanges();

            TestClientContext.Configurations.ResponsePipeline.OnMessageReaderSettingsCreated(rsa =>
            {
                // reader supports undeclared property
                rsa.Settings.Validations ^= ~ValidationKinds.ThrowOnUndeclaredPropertyForNonOpenType;
            });
            ODataUntypedValue undeclaredOdataPropertyValue = null;
            TestClientContext.Configurations.ResponsePipeline.OnEntityMaterialized(rea =>
            {
                if (rea.Entity.GetType() == typeof(AccountInfo))
                {
                    var undeclaredOdataProperty = rea.Entry.Properties.FirstOrDefault(s => s.Name == "UndecalredOpenProperty1");
                    undeclaredOdataPropertyValue = (ODataUntypedValue)undeclaredOdataProperty.Value;
                }
            });

            var accountReturned = TestClientContext.Accounts.Where(a => a.AccountID == 101).First();
            Assert.AreEqual<string>(
                "{\"sender\":\"RSS\",\"senderImage\":\"https://exchangelabs.abcexample-int.com/connectors/content/images/feed-icon-128px.png?upn=admin%40tenant-EXHR-3837dom.EXTEST.ahaha.COM\",\"summary\":\"RSS is now connected to your mailbox\",\"title\":null}",
                undeclaredOdataPropertyValue.RawValue);


{% endhighlight %}
