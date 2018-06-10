//---------------------------------------------------------------------
// <copyright file="SingletonBatchRoundtripJsonLightTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Microsoft.OData.Edm;
using Microsoft.OData.JsonLight;
using Microsoft.OData.MultipartMixed;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.Roundtrip.JsonLight
{
    public class BatchRoundtripJsonLightTests
    {
        private enum BatchFormat
        {
            MultipartMIME,
            ApplicationJson
        };

        private const string batchContentTypeMultipartMime = "multipart/mixed; boundary=batch_cb48b61f-511b-48e6-b00a-77c847badfb9";
        private const string batchContentTypeApplicationJson = "application/json; odata.streaming=true";
        private const string serviceDocumentUri = "http://odata.org/test/";
        private const string dependsOnIdNotFound = "is not matching any of the request Id and atomic group Id seen so far. Forward reference is not allowed.";
        private const string changesetContainingQueryNotAllowed =
            "was detected for a request in a change set. Requests in change sets only support the HTTP methods 'POST', 'PUT', 'DELETE', and 'PATCH'.";
        private const string referenceIdNotIncludedInDependsOn = "is not found in effective depends-on-Ids";
        private readonly EdmEntityContainer defaultContainer;
        private readonly EdmModel userModel;
        private readonly EdmSingleton singleton;
        private readonly EdmEntityType webType;

        // GET and DELETE should contain extra empty line as content is empty, see RFC 2046 5.1.1
        private const string ExpectedRequestPayload1 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fd

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10,""Name"":""SingletonWeb""}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton/WebId HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fe

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fe
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

DELETE http://odata.org/test/MySingleton HTTP/1.1


--changeset_702fbcf5-653b-4217-bf4b-563aae4971fe--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedRequestPayload2 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9

--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

PATCH http://odata.org/test//MySingleton1 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10,""Name"":""SingletonWeb""}
--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PATCH http://odata.org/test//MySingleton2 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changeset_1493140c-7589-4a34-8d1e-7aa9b37567f9--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50

--changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

PATCH http://odata.org/test//MySingleton3 HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":30,""Name"":""SingletonWeb3""}
--changeset_adf4fbc4-7e45-4e28-8ec0-8d12875f6c50--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test//MySingleton3/WebId HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayload1 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea77
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

HTTP/1.1 500 Internal Server Error


--changeset_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayload2 = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238

--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_81de0931-dc7c-434e-9153-4ffb2a8ae238--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1

--changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_a1607419-e00a-47bd-bd84-9ddb3caca3d1--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedRequestPayloadVerifyDependsOnIds = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test/MySingleton HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_702fbcf5-653b-4217-bf4b-563aae4971fd

--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":9}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

PATCH $2B HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changeset_702fbcf5-653b-4217-bf4b-563aae4971fd--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changeset_ac35271a-a4f8-4cb2-9967-46c640c716ab

--changeset_ac35271a-a4f8-4cb2-9967-46c640c716ab
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3A

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-Version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":112}
--changeset_ac35271a-a4f8-4cb2-9967-46c640c716ab--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

GET http://odata.org/test/MySingleton HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedResponsePayloadVerifyDependsOnIds = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_63a002e0-5306-4415-9439-5e7288d1bcfe

--changesetresponse_63a002e0-5306-4415-9439-5e7288d1bcfe
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3A

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_63a002e0-5306-4415-9439-5e7288d1bcfe--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        // Multipart batch request, with top level request "3" referencing Uri "$1".
        private const string ActualMultipartRequestWithTopLevelRequestDependency = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

GET http://odata.org/test/MySingleton HTTP/1.1
Accept: application/json;odata.metadata=full


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetrequest_8568f244-a9a8-40bc-887f-2d9602374b70

--changesetrequest_8568f244-a9a8-40bc-887f-2d9602374b70
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":9}
--changesetrequest_8568f244-a9a8-40bc-887f-2d9602374b70
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

PATCH http://odata.org/test//MySingleton HTTP/1.1
OData-version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":10}
--changesetrequest_8568f244-a9a8-40bc-887f-2d9602374b70
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

PATCH $2B HTTP/1.1
OData-version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":111}
--changesetrequest_8568f244-a9a8-40bc-887f-2d9602374b70--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

PATCH $1 HTTP/1.1
OData-version: 4.0
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8

{""@odata.type"":""#NS.Web"",""WebId"":113}


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedMultipartResponseWithTopLevelRequestDependency = @"--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;
OData-Version: 4.0

{""@odata.context"":""http://odata.org/test/$metadata#MySingleton"",""WebId"":10,""Name"":""WebSingleton""}
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: multipart/mixed; boundary=changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76

--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2A

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2B

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2C

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--changesetresponse_6ddc5056-67cd-42e2-85d2-e6fdea46ea76--
--batch_cb48b61f-511b-48e6-b00a-77c847badfb9
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 204 No Content
Content-Type: application/json;odata.metadata=none


--batch_cb48b61f-511b-48e6-b00a-77c847badfb9--
";

        private const string ExpectedRequestPayloadUsingJson1 = @"
{
    ""requests"": [{
            ""id"": ""a05368c8-479d-4409-a2ee-9b54b133ec38"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""accept"": ""application/json;odata.metadata=full""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 10,
                ""Name"": ""SingletonWeb""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""f7de7314-2f3d-4422-b840-ada6d6de0f18"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 111
            }
        }, {
            ""id"": ""adec0e52-7647-4d9d-baac-9316f9dc6927"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton/WebId"",
            ""headers"": {
                ""accept"": ""application/json;odata.metadata=full""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""29873bf8-2d2c-478d-a7df-1e7b236faebf"",
            ""method"": ""DELETE"",
            ""url"": ""http://odata.org/test/MySingleton"",
            ""headers"": {}
        }
    ]
}";

        private const string ExpectedRequestPayloadUsingJson2 = @"
{
    ""requests"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""2ffeac98-237b-46a7-b97c-6a360d622aaa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton1"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 10,
                ""Name"": ""SingletonWeb""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""2ffeac98-237b-46a7-b97c-6a360d622aaa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton2"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 111
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""2ffabc98-257b-46a7-b17c-97650d6220aa"",
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//MySingleton3"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 30,
                ""Name"": ""SingletonWeb3""
            }
        }, {
            ""id"": ""0d24a607-3fb0-4a83-a8fb-ef33f8c7e4ba"",
            ""method"": ""GET"",
            ""url"": ""http://odata.org/test//MySingleton3/WebId"",
            ""headers"": {
                ""accept"": ""application/json;odata.metadata=full""
            }
        }
    ]
}";

        private const string ExpectedResponsePayloadUsingJson1 = @"
{
    ""responses"": [{
            ""id"": ""cbcdb345-afdc-4125-8832-fcd7e14a013f"",
            ""status"": 200,
            ""headers"": {
                ""content-type"": ""application/json;"",
                ""odata-version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }, {
            ""id"": ""1"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 204,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 204,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""fd811ff7-4c67-4a2f-bbe1-60b606093f14"",
            ""status"": 200,
            ""headers"": {
                ""content-type"": ""application/json;"",
                ""odata-version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""a2f8de08-ae49-4717-92c5-9dbc198acc71"",
            ""status"": 500,
            ""headers"": {}
        }
    ]
}";

        private const string ExpectedResponsePayloadUsingJson2 = @"
{
    ""responses"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""b749ce30-c81d-4a04-af0f-342da47218ec"",
            ""status"": 204,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""b749ce30-c81d-4a04-af0f-342da47218ec"",
            ""status"": 204,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""3"",
            ""atomicityGroup"": ""630855a9-c931-478f-a6d7-860bba586e5e"",
            ""status"": 204,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""81133a6f-c773-4add-963f-54b167adb747"",
            ""status"": 200,
            ""headers"": {
                ""content-type"": ""application/json;"",
                ""odata-version"": ""4.0""
            },
            ""body"": {
                ""@odata.context"": ""http://odata.org/test/$metadata#MySingleton"",
                ""WebId"": 10,
                ""Name"": ""WebSingleton""
            }
        }
    ]
}";

        private const string ExpectedReferenceUriRequestPayload = @"
{
    ""requests"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""11d431dd-cfee-48c8-95fb-da8491644fa6"",
            ""method"": ""PUT"",
            ""url"": ""http://odata.org/test//MySingleton"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 10,
                ""Name"": ""SingletonWebForRequestIdRefTest""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""11d431dd-cfee-48c8-95fb-da8491644fa6"",
            ""dependsOn"": [""1""],
            ""method"": ""PATCH"",
            ""url"": ""http://odata.org/test//$1"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 12
            }
        }, {
            ""id"": ""3"",
            ""dependsOn"": [""1"", ""2""],
            ""method"": ""PATCH"",
            ""url"": ""$1/alias"",
            ""headers"": {
                ""odata-version"": ""4.0"",
                ""content-type"": ""application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8""
            },
            ""body"": {
                ""@odata.type"": ""#NS.Web"",
                ""WebId"": 13
            }
        }
    ]
}";

        private const string ExpectedReferenceUriResponsePayload = @"
{
    ""responses"": [{
            ""id"": ""1"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 201,
            ""headers"": {
                ""content-type"": ""application/json;odata.metadata=none""
            }
        }, {
            ""id"": ""2"",
            ""atomicityGroup"": ""6e5679f2-2fb9-4097-84a9-623a0960a50f"",
            ""status"": 204,
            ""headers"": {}
        }, {
            ""id"": ""fd811ff7-4c67-4a2f-bbe1-60b606093f14"",
            ""status"": 204,
            ""headers"": {}
        }
    ]
}";

        private readonly string[] ExpectedRequestPayloads =
        {
            ExpectedRequestPayload1,
            ExpectedRequestPayload2
        };

        private readonly string[] ExpectedRequestPayloadsJson =
        {
            ExpectedRequestPayloadUsingJson1,
            ExpectedRequestPayloadUsingJson2
        };

        private readonly string[] ExpectedResponsePayloads =
        {
            ExpectedResponsePayload1,
            ExpectedResponsePayload2
        };

        private readonly string[] ExpectedResponsePayloadsJson =
        {
            ExpectedResponsePayloadUsingJson1,
            ExpectedResponsePayloadUsingJson2
        };

        public BatchRoundtripJsonLightTests()
        {
            this.userModel = new EdmModel();

            this.webType = new EdmEntityType("NS", "Web");
            this.webType.AddStructuralProperty("WebId", EdmPrimitiveTypeKind.Int32);
            this.webType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            this.userModel.AddElement(this.webType);

            this.defaultContainer = new EdmEntityContainer("NS", "DefaultContainer");
            this.userModel.AddElement(defaultContainer);

            this.singleton = new EdmSingleton(defaultContainer, "MySingleton", this.webType);
            this.defaultContainer.AddElement(this.singleton);
        }

        [Fact]
        public void MultipartBatchImplicitChangeSetDependsOnIdsTest()
        {
            // Verify reference of content id works when dependsOn Ids are correctly specified.
            string contentIdRef = "2B";

            foreach (ODataVersion odataVersion in new[] { ODataVersion.V4, ODataVersion.V401 })
            {
                byte[] requestPayload = ClientWriteRequestForMultipartBatchVerifyDependsOnIds(
                    contentIdRef, odataVersion);
                VerifyPayloadForMultipartBatch(requestPayload, ExpectedRequestPayloadVerifyDependsOnIds);

                byte[] responsePayload = this.ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(requestPayload, odataVersion);
                VerifyPayloadForMultipartBatch(responsePayload, ExpectedResponsePayloadVerifyDependsOnIds);

                ClientReadSingletonBatchResponse(responsePayload, batchContentTypeMultipartMime);
            }
        }

        [Fact]
        public void MultipartBatchImplicitChangeSetInvalidDependsOnIdShouldThrow()
        {
            string contentIdRef = "invalidId";
            foreach (ODataVersion odataVersion in new[] {ODataVersion.V4, ODataVersion.V401})
            {
                ODataException ode = Assert.Throws<ODataException>(
                    () => ClientWriteRequestForMultipartBatchVerifyDependsOnIds(
                        contentIdRef, odataVersion));

                Assert.True(ode.Message.Contains(referenceIdNotIncludedInDependsOn));
            }
        }

        [Fact]
        public void MultipartBatchVerifyDependsOnIdsForTopLevelRequestV4()
        {
            // In V4, referencing preceding top-level request Id from another request
            // after change set should throw.
            string topLevelContentId = "1";

            ODataException ode = Assert.Throws<ODataException>(
                    () => ClientWriteRequestForMultipartBatchVerifyDependsOnIdsForTopLevelRequest(
                        topLevelContentId, ODataVersion.V4));
            Assert.True(ode.Message.Contains(referenceIdNotIncludedInDependsOn));
        }

        [Fact]
        public void MultipartBatchVerifyDependsOnIdsForTopLevelRequestV401()
        {
            string topLevelContentId = "1";

            byte[] requestBytes = ClientWriteRequestForMultipartBatchVerifyDependsOnIdsForTopLevelRequest(
                topLevelContentId, ODataVersion.V401);

            Assert.NotNull(requestBytes);
            Assert.True(requestBytes.Length > 0);
        }

        [Fact]
        public void MultipartBatchImplicitTopLevelDependsOnIdsTest()
        {
            foreach (ODataVersion odataVersion in new[] {ODataVersion.V4, ODataVersion.V401 })
            {
                byte[] responsePayload =
                    this.ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(
                        System.Text.Encoding.UTF8.GetBytes(ActualMultipartRequestWithTopLevelRequestDependency), odataVersion);
                VerifyPayloadForMultipartBatch(responsePayload, ExpectedMultipartResponseWithTopLevelRequestDependency);
            }
        }

        [Fact]
        public void BatchJsonLightTest()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.MultipartMIME, 0);
        }

        [Fact]
        public void BatchJsonLightTestUsingJson()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.ApplicationJson, 0);
        }

        [Fact]
        public void BatchJsonLightTestChangesetsFollowedByQuery()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.MultipartMIME, 1);
        }

        [Fact]
        public void BatchJsonLightTestChangesetsFollowedByQueryUsingJson()
        {
            BatchJsonLightTestUsingBatchFormat(BatchFormat.ApplicationJson, 1);
        }

        [Fact]
        public void BatchJsonLightSelfReferenceUriTest()
        {
            bool expectedExceptionThrown = true;
            try
            {
            this.CreateSelfReferenceBatchRequest(BatchFormat.ApplicationJson);
                expectedExceptionThrown = false;
            }
            catch (Exception e)
            {
                Assert.True(e.Message.Contains(dependsOnIdNotFound));
            }
            Assert.True(expectedExceptionThrown, "Uri self-referencing with its Content-ID should not be allowed.");
        }

        [Fact]
        public void BatchJsonLightReferenceUriV4TestShouldThrow()
        {
            bool exceptionThrown = true;
            try
            {
                this.CreateReferenceUriBatchRequest(ODataVersion.V4);
                exceptionThrown = false;
            }
            catch (ODataException e)
            {
                e.Message.Should().Contain("is not matching any of the request Id and atomic group Id seen so far");
                return;
            }

            Assert.True(exceptionThrown, "An exception should have been thrown when trying to refer to the " +
                "content Id of the last request of a change set or atomic group in V4.");
        }

        [Fact]
        public void BatchJsonLightUseInvalidDependsOnIdsV401Test()
        {
            bool exceptionThrown = true;
            try
            {
                this.CreateReferenceUriBatchRequest(ODataVersion.V401,
                    true /*useInvalidDependsOnIds*/,
                    false  /*useRequestIdOfGroupForDependsOnIds*/);
                exceptionThrown = false;
            }
            catch (ODataException e)
            {
                e.Message.Should().Contain(dependsOnIdNotFound);
                return;
            }

            Assert.True(exceptionThrown, "An exception should have been thrown when trying to create " +
                "request with invalid dependsOn Ids in V401.");
        }

        [Fact]
        public void BatchJsonLightInvalidUseRequestIdOfGroupForDependsOnIdsV401Test()
        {
            bool exceptionThrown = true;
            try
            {
                this.CreateReferenceUriBatchRequest(ODataVersion.V401,
                    false /*useInvalidDependsOnIds*/,
                    true  /*useRequestIdOfGroupForDependsOnIds*/);
                exceptionThrown = false;
            }
            catch (ODataException e)
            {
                e.Message.Should().Contain("Therefore dependsOn property should refer to atomic group Id");
                return;
            }

            Assert.True(exceptionThrown, "An exception should have been thrown when trying to create " +
                "request with invalid dependsOn Ids in V401.");
        }

        [Fact]
        public void BatchJsonLightReferenceUriV401Test()
        {
            byte[] requestPayload = this.CreateReferenceUriBatchRequest(ODataVersion.V401);
            VerifyPayload(requestPayload, ExpectedReferenceUriRequestPayload);

            byte[] responsePayload = this.ServiceReadReferenceUriBatchRequestAndWriteResponse(requestPayload);
            VerifyPayload(responsePayload, ExpectedReferenceUriResponsePayload);
        }

        [Fact]
        public void BatchJsonLightTestUsingJsonCanGenerateRequest()
        {
            byte[] requestPayload =
                CreateBatchRequestWithChangesetFirstAndQueryLast(GetContentTypeHeader(BatchFormat.ApplicationJson));
            VerifyPayload(requestPayload, BatchFormat.ApplicationJson, true, 1);
        }

        [Fact]
        public void BatchJsonLightQueryInsideChangesetNotAllowedTest()
        {
            BatchFormat[] formats = new BatchFormat[] { BatchFormat.MultipartMIME, BatchFormat.ApplicationJson };
            foreach (BatchFormat format in formats)
            {
                bool expectedExceptionThrown = false;
                try
                {
                    this.CreateQueryInsideChangesetBatchRequest(format);
                }
                catch (Exception e)
                {
                    expectedExceptionThrown = e.Message.Contains(changesetContainingQueryNotAllowed);
                }
                Assert.True(expectedExceptionThrown, "change set containing query operation should not be allowed.");
            }
        }

        [Fact]
        public void BatchJsonLightContentIdUniquenessScopeForMultipartV4ShouldPass()
        {
            ODataMessageWriterSettings writerSettingsV4 = new ODataMessageWriterSettings {Version = ODataVersion.V4};

            // Same Content-ID value in different change set should be allowed in V4 since
            // Content-ID uniqueness scope is local (change set scope)
            byte[] request = ClientWriteMuitipartChangesetsWithSameContentId(writerSettingsV4, batchContentTypeMultipartMime);
            Assert.NotNull(request);
        }

        [Fact]
        public void BatchJsonLightContentIdUniquenessScopeForMultipartV401ShouldThrow()
        {
            ODataMessageWriterSettings writerSettingsV401 = new ODataMessageWriterSettings {Version = ODataVersion.V401};


            string expectedPartialErrorMesage =
                "was found more than once in the same change set or same batch request. Content "
              + "IDs have to be unique across all operations of a change set for OData V4.0 and have to be unique "
              + "across all operations in the whole batch request for OData V4.01.";

            ODataException ode = Assert.Throws<ODataException>(
                () => ClientWriteMuitipartChangesetsWithSameContentId(writerSettingsV401, batchContentTypeMultipartMime));
            Assert.Contains(expectedPartialErrorMesage, ode.Message);
        }

        [Fact]
        public void BatchJsonLightContentIdUniquenessScopeForJsonBatchV401ShouldThrow()
        {
            ODataMessageWriterSettings writerSettingsV401 = new ODataMessageWriterSettings { Version = ODataVersion.V401 };


            string expectedPartialErrorMesage =
                "was found more than once in the same change set or same batch request. Content "
              + "IDs have to be unique across all operations of a change set for OData V4.0 and have to be unique "
              + "across all operations in the whole batch request for OData V4.01.";

            ODataException ode = Assert.Throws<ODataException>(
                () => ClientWriteMuitipartChangesetsWithSameContentId(writerSettingsV401, batchContentTypeApplicationJson));
            Assert.Contains(expectedPartialErrorMesage, ode.Message);
        }

        private static void PopulateWebResourceId(ODataBatchOperationRequestMessage dataModificationRequestMessage, int webId)
        {
            // Use a new message writer to write the body of this operation.
            using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(dataModificationRequestMessage))
            {
                ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                ODataResource entry = new ODataResource
                {
                    TypeName = "NS.Web",
                    Properties = new[] { new ODataProperty { Name = "WebId", Value = webId } }
                };
                entryWriter.WriteStart(entry);
                entryWriter.WriteEnd();
            }
        }

        private void BatchJsonLightTestUsingBatchFormat(BatchFormat batchFormat, int idx)
        {
            byte[] requestPayload = null;
            switch (idx)
            {
                case 0:
                    requestPayload = ClientWriteSingletonBatchRequest(
                        GetContentTypeHeader(batchFormat));
                    break;

                case 1:
                    requestPayload = CreateBatchRequestWithChangesetFirstAndQueryLast(
                        GetContentTypeHeader(batchFormat));
                    break;

                default:
                    throw new ArgumentException("Unknown batch request index value", "idx");
            }

            VerifyPayload(requestPayload, batchFormat, true /*for request*/, idx);
            var responsePayload = this.ServiceReadSingletonBatchRequestAndWriterBatchResponse(requestPayload, GetContentTypeHeader(batchFormat));
            VerifyPayload(responsePayload, batchFormat, false /*for response*/, idx);
            this.ClientReadSingletonBatchResponse(responsePayload, GetContentTypeHeader(batchFormat));
        }

        private void VerifyPayload(byte[] payloadBytes, BatchFormat batchFormat, bool forRequest, int idx)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {

                string payload = sr.ReadToEnd();
                string expectedPayload = null;

                switch (batchFormat)
                {
                    case BatchFormat.MultipartMIME:
                        {
                            expectedPayload = GetNormalizedMultipartMimeMessage(
                                forRequest ? ExpectedRequestPayloads[idx] : ExpectedResponsePayloads[idx]);

                            payload = GetNormalizedMultipartMimeMessage(payload);
                        }
                        break;

                    case BatchFormat.ApplicationJson:
                        {
                            expectedPayload = GetNormalizedJsonMessage(forRequest ?
                                ExpectedRequestPayloadsJson[idx] : ExpectedResponsePayloadsJson[idx]);

                            payload = GetNormalizedJsonMessage(payload);
                        }
                        break;

                    default:
                        {
                            Assert.True(false, "Unknown BatchFormat value");
                        }
                        break;
                }
                Assert.Equal(expectedPayload, payload);
            }
        }

        private void VerifyPayload(byte[] payloadBytes, string expectedPayload)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {
                string normalizedPayload = GetNormalizedJsonMessage(sr.ReadToEnd());
                string normalizedExpectedPayload = GetNormalizedJsonMessage(expectedPayload);

                Assert.Equal(normalizedExpectedPayload, normalizedPayload);
            }
        }

        private void VerifyPayloadForMultipartBatch(byte[] payloadBytes, string expectedPayload)
        {
            using (MemoryStream stream = new MemoryStream(payloadBytes))
            using (StreamReader sr = new StreamReader(stream))
            {
                string normalizedPayload = GetNormalizedMultipartMimeMessage(sr.ReadToEnd());
                string normalizedExpectedPayload = GetNormalizedMultipartMimeMessage(expectedPayload);

                Assert.Equal(normalizedExpectedPayload, normalizedPayload);
            }
        }

        private string GetNormalizedMultipartMimeMessage(string message)
        {
            string normalizedMessage = Regex.Replace(message, "changeset.*$", "changeset_GUID\r", RegexOptions.Multiline);
            normalizedMessage = Regex.Replace(normalizedMessage, "OData-Version: .*$", "OData-version: myODataVer\r",
                RegexOptions.Multiline);
            return normalizedMessage;
        }

        private byte[] ClientWriteSingletonBatchRequest(string batchContentType)
        {
            var stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageWriter = new ODataMessageWriter(requestMessage))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a query operation.
                var queryOperationMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/MySingleton"), /*contentId*/ null);

                // Verify that: for multipart batch the content id of the request generated by the writer can be null;
                // for Json batch the content id of the request generated by the writer is not null.
                Assert.True(
                       (batchWriter is ODataJsonLightBatchWriter && queryOperationMessage.ContentId != null)
                    || (batchWriter is ODataMultipartMixedBatchWriter && queryOperationMessage.ContentId == null));

                // Header modification on inner payload.
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                // Write a change set with multi update operation.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                var updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "1");

                // Use a new message writer to write the body of this operation.
                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 10 }, new ODataProperty() { Name = "Name", Value = "SingletonWeb" } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "2");

                using (var operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    var entry = new ODataResource() { TypeName = "NS.Web", Properties = new[] { new ODataProperty() { Name = "WebId", Value = 111 } } };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Write a query operation.
                queryOperationMessage = batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "/MySingleton/WebId"), /*contentId*/ null);

                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                // DELETE singleton, invalid
                batchWriter.WriteStartChangeset();
                batchWriter.CreateOperationRequestMessage("DELETE", new Uri(serviceDocumentUri + "MySingleton"), "3");
                batchWriter.WriteEndChangeset();

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadSingletonBatchRequestAndWriterBatchResponse(byte[] requestPayload, string batchContentType)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (var messageReader = new ODataMessageReader(requestMessage, new ODataMessageReaderSettings(), this.userModel))
            {
                var responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentType);
                var messageWriter = new ODataMessageWriter(responseMessage);
                var batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                var batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            var operationMessage = batchReader.CreateOperationRequestMessage();

                            if (operationMessage.Method == "PATCH")
                            {
                                var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                Assert.NotNull(response.ContentId);
                                Assert.NotNull(response.GroupId);
                                response.StatusCode = 204;
                                response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                // Set the content-id of response to that of the corresponding request.
                                // This enables us to correlate request and response to the maximum extend.
                                // For multipart batch, it is still null.
                                // For Json batch, it utilizes the possible non-null value created during the
                                // writer's creating of the batch request.
                                var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", "application/json;");
                                var settings = new ODataMessageWriterSettings();
                                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (var operationMessageWriter = new ODataMessageWriter(response, settings, this.userModel))
                                {
                                    var entryWriter = operationMessageWriter.CreateODataResourceWriter(this.singleton, this.webType);
                                    var entry = new ODataResource()
                                    {
                                        TypeName = "NS.Web",
                                        Properties = new[]
                                        {
                                            new ODataProperty() { Name = "WebId", Value = 10 },
                                            new ODataProperty() { Name = "Name", Value = "WebSingleton" }
                                        }
                                    };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }
                            else if (operationMessage.Method == "DELETE")
                            {
                                var response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 500;
                            }

                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            batchWriter.WriteStartChangeset();
                            break;
                        case ODataBatchReaderState.ChangesetEnd:
                            batchWriter.WriteEndChangeset();
                            break;
                    }
                }

                batchWriter.WriteEndBatch();
                responseStream.Position = 0;
                return responseStream.ToArray();
            }
        }

        private void ClientReadSingletonBatchResponse(byte[] responsePayload, string batchContentType)
        {
            IODataResponseMessage responseMessage = new InMemoryMessage() { Stream = new MemoryStream(responsePayload) };
            responseMessage.SetHeader("Content-Type", batchContentType);
            using (ODataMessageReader messageReader = new ODataMessageReader(responseMessage, new ODataMessageReaderSettings(), this.userModel))
            {
                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            ODataBatchOperationResponseMessage operationMessage = batchReader.CreateOperationResponseMessage();
                            if (operationMessage.StatusCode == 200)
                            {
                                using (ODataMessageReader innerMessageReader = new ODataMessageReader(operationMessage, new ODataMessageReaderSettings(), this.userModel))
                                {
                                    ODataReader reader = innerMessageReader.CreateODataResourceReader();

                                    while (reader.Read())
                                    {
                                        if (reader.State == ODataReaderState.ResourceEnd)
                                        {
                                            ODataResource entry = reader.Item as ODataResource;
                                            Assert.Equal(10, entry.Properties.Single(p => p.Name == "WebId").Value);
                                            Assert.Equal("WebSingleton", entry.Properties.Single(p => p.Name == "Name").Value);
                                        }
                                    }
                                }

                                // The only two messages with HTTP-200 response codes are the two GET requests with content id value of null.
                                // Verify that: for multipart batch the content id of the response is matching that of the request;
                                // for Json batch the content id of the response is not null.
                                Assert.True(
                                       (batchReader is ODataJsonLightBatchReader && operationMessage.ContentId != null)
                                    || (batchReader is ODataMultipartMixedBatchReader && operationMessage.ContentId == null));
                            }
                            else
                            {
                                Assert.True(204 == operationMessage.StatusCode || 500 == operationMessage.StatusCode);

                                // The only message above with HTTP-500 response code has content id value of "3".
                                // Verify that the content id of the response is matching that of the request.
                                if (500 == operationMessage.StatusCode)
                                {
                                    Assert.True(operationMessage.ContentId.Equals("3"));
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Create an invalid batch request with operation containing self-referencing uri.
        /// </summary>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateSelfReferenceBatchRequest(BatchFormat format)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(format));

            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);
            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a change set with an operation with self-referencing uri.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set with uri referencing itself.
                string contentId = "1";
                string[] dependsOnIds = new string[] {contentId};
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/${1}", serviceDocumentUri, contentId)),
                    contentId,
                    BatchPayloadUriOption.AbsoluteUri,
                    dependsOnIds);

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource() { TypeName = "NS.Web" };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();
                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Create a batch request that contains cross-referencing requests.
        /// </summary>
        /// <param name="version">The OData version for request.</param>
        /// <param name="useInvalidDependsOnIds">
        /// Whether to use invalid id value as dependsOn ids for the test case. Set to true to trigger exception.</param>
        /// <param name="useRequestIdOfGroupForDependsOnIds">
        /// Whether to use request id belonging to atomic group as dependsOn id. Set to true to trigger exception.</param>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateReferenceUriBatchRequest(ODataVersion version, bool useInvalidDependsOnIds = false, bool useRequestIdOfGroupForDependsOnIds = false)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(BatchFormat.ApplicationJson));
            ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
            settings.BaseUri = new Uri(serviceDocumentUri);
            settings.Version = version;

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                batchWriter.WriteStartChangeset();

                // Create operation.
                string resourceSegment = "MySingleton";
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PUT",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, resourceSegment)),
                    "1");
                string firstGroupId = updateOperationMessage.GroupId;

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 10 },
                            new ODataProperty() { Name = "Name", Value = "SingletonWebForRequestIdRefTest" } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                // A PATCH operation that depends on the preceding PUT operation.
                IList<string> dependsOnIds = new List<string>{"1"};

                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, "$1")),
                    "2",
                    BatchPayloadUriOption.AbsoluteUri,
                    dependsOnIds);

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[]
                        {
                            new ODataProperty() { Name = "WebId", Value = 12 }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Another PATCH operation that depends on both operations above.
                if (useInvalidDependsOnIds)
                {
                    dependsOnIds = new string[] { "nonExistant" };
                }
                else if(useRequestIdOfGroupForDependsOnIds)
                {
                    dependsOnIds = new string[] {"1", "2"};
                }
                else
                {
                    dependsOnIds = new string[] {firstGroupId};
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH",
                    new Uri("$1/alias", UriKind.Relative),
                    "3",
                    BatchPayloadUriOption.RelativeUri,
                    dependsOnIds);

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[]
                        {
                            new ODataProperty() { Name = "WebId", Value = 13 }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadReferenceUriBatchRequestAndWriteResponse(byte[] requestPayload)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(BatchFormat.ApplicationJson));
            ODataMessageReaderSettings settings = new ODataMessageReaderSettings {BaseUri = new Uri(serviceDocumentUri)};

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage, settings, this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", GetContentTypeHeader(BatchFormat.ApplicationJson));
                ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage);
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            ODataBatchOperationRequestMessage operationMessage = batchReader.CreateOperationRequestMessage();

                            ODataBatchOperationResponseMessage response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                            if (operationMessage.Method == "PUT")
                            {
                                response.StatusCode = 201;
                                response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "PATCH")
                            {
                                response.StatusCode = 204;
                            }

                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            batchWriter.WriteStartChangeset();
                            break;
                        case ODataBatchReaderState.ChangesetEnd:
                            batchWriter.WriteEndChangeset();
                            break;
                    }
                }

                batchWriter.WriteEndBatch();
                responseStream.Position = 0;
                return responseStream.ToArray();
            }
        }

        /// <summary>
        /// Create a batch request with two batches and one query operation.
        /// </summary>
        /// <returns>Stream containing the batch request.</returns>
        private byte[] CreateBatchRequestWithChangesetFirstAndQueryLast(string batchContentType)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // First item is part of a change set.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton1"), "1");

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 10 },
                            new ODataProperty() { Name = "Name",  Value = "SingletonWeb" }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton2"), "2");

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] { new ODataProperty() { Name = "WebId", Value = 111 } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }

                batchWriter.WriteEndChangeset();

                // Second change set starts.
                batchWriter.WriteStartChangeset();

                // Create a update operation in the change set.
                updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PATCH", new Uri(serviceDocumentUri + "/MySingleton3"), "3");

                // Use a new message writer to write the body of this operation.
                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 30 },
                            new ODataProperty() { Name = "Name",  Value = "SingletonWeb3" }
                        }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }
                batchWriter.WriteEndChangeset();

                // Last item is a query operation.
                ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(serviceDocumentUri + "/MySingleton3/WebId"), /*contentId*/ null);

                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Create an invalid batch request with query operation inside a change set.
        /// </summary>
        /// <returns>Thrown exception.</returns>
        private byte[] CreateQueryInsideChangesetBatchRequest(BatchFormat format)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", GetContentTypeHeader(format));

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                // An invalid batch that has a change set containing query request
                batchWriter.WriteStartBatch();
                batchWriter.WriteStartChangeset();

                // Create a query operation in the change set with uri referencing itself.
                ODataBatchOperationRequestMessage queryOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "GET", new Uri(serviceDocumentUri + "/MySingletonInvalidRequest/WebId"), /*contentId*/ null);
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");

                batchWriter.WriteEndChangeset();

                // Create operation.
                string resourceSegment = "MySingleton";
                ODataBatchOperationRequestMessage updateOperationMessage = batchWriter.CreateOperationRequestMessage(
                    "PUT",
                    new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", serviceDocumentUri, resourceSegment)),
                    "1");
                string firstGroupId = updateOperationMessage.GroupId;

                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                {
                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                    ODataResource entry = new ODataResource()
                    {
                        TypeName = "NS.Web",
                        Properties = new[] {
                            new ODataProperty() { Name = "WebId", Value = 10 },
                            new ODataProperty() { Name = "Name", Value = "SingletonWebForRequestIdRefTest" } }
                    };
                    entryWriter.WriteStart(entry);
                    entryWriter.WriteEnd();
                }
                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ClientWriteMuitipartChangesetsWithSameContentId(ODataMessageWriterSettings settings, string batchContentType)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentType);

            string contentId = "contentIdUniqueInChangesetScope";

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage, settings))
            {
                var batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Create two identical changesets such that each has a request with same Content-ID.
                for (int i = 0; i < 2; i++)
                {
                    batchWriter.WriteStartChangeset();

                    ODataBatchOperationRequestMessage updateOperationMessage =
                        batchWriter.CreateOperationRequestMessage(
                            "PATCH", new Uri(serviceDocumentUri + "/MySingleton"), contentId);

                    // Use a new message writer to write the body of this operation.
                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        var entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        var entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties =
                                new[]
                                {
                                    new ODataProperty() {Name = "WebId", Value = 10}
                                }
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    batchWriter.WriteEndChangeset();
                }

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ClientWriteRequestForMultipartBatchVerifyDependsOnIds(string contentIdRef, ODataVersion version)
        {
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage,
                new ODataMessageWriterSettings
                {
                    Version = version,
                    BaseUri = new Uri(serviceDocumentUri)
                }))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // Write a query operation.
                ODataBatchOperationRequestMessage queryOperationMessage =
                    batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "MySingleton"), "1"/*not written*/);

                // Header modification on inner payload.
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                Assert.NotNull(queryOperationMessage.ContentId);

                // Write a change set with multi update operation.
                batchWriter.WriteStartChangeset();
                {
                    // Create a update operation in the change set.
                    ODataBatchOperationRequestMessage updateOperationMessage =
                        batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "2A" /*written*/);

                    // Use a new message writer to write the body of this operation.
                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource
                        {
                            TypeName = "NS.Web",
                            Properties = new[] {new ODataProperty {Name = "WebId", Value = 9}}
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    // Create another update operation in the change set.
                    updateOperationMessage =
                        batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "2B");

                    // Use a new message writer to write the body of this operation.
                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource
                        {
                            TypeName = "NS.Web",
                            Properties = new[] {new ODataProperty {Name = "WebId", Value = 10}}
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }

                    Uri referenceUri = new Uri("$" + contentIdRef, UriKind.Relative);
                    updateOperationMessage = batchWriter.CreateOperationRequestMessage("PATCH", referenceUri, "2C",
                        BatchPayloadUriOption.RelativeUri);

                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource()
                        {
                            TypeName = "NS.Web",
                            Properties = new[] {new ODataProperty() {Name = "WebId", Value = 111}}
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }
                }
                batchWriter.WriteEndChangeset();

                // Create another change set with only one request inside
                batchWriter.WriteStartChangeset();
                {
                    // Create another update operation in the change set.
                    ODataBatchOperationRequestMessage updateOperationMessage =
                        batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "3A");

                    // Use a new message writer to write the body of this operation.
                    using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(updateOperationMessage))
                    {
                        ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter();
                        ODataResource entry = new ODataResource
                        {
                            TypeName = "NS.Web",
                            Properties = new[] {new ODataProperty {Name = "WebId", Value = 112}}
                        };
                        entryWriter.WriteStart(entry);
                        entryWriter.WriteEnd();
                    }
                }
                batchWriter.WriteEndChangeset();

                // Write a query operation.
                queryOperationMessage =
                    batchWriter.CreateOperationRequestMessage("GET", new Uri(serviceDocumentUri + "MySingleton"), "4"/*not written*/);

                // Header modification on inner payload.
                queryOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                Assert.NotNull(queryOperationMessage.ContentId);

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ClientWriteRequestForMultipartBatchVerifyDependsOnIdsForTopLevelRequest(string contentIdRef, ODataVersion version)
        {
            // Batch consists of one top-level request, one change set, and one more top-level request referencing the first top-level request.
            MemoryStream stream = new MemoryStream();

            IODataRequestMessage requestMessage = new InMemoryMessage { Stream = stream };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);

            using (ODataMessageWriter messageWriter = new ODataMessageWriter(requestMessage,
                new ODataMessageWriterSettings
                {
                    Version = version,
                    BaseUri = new Uri(serviceDocumentUri)
                }))
            {
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();

                batchWriter.WriteStartBatch();

                // A create operation.
                ODataBatchOperationRequestMessage createOperationMessage =
                    batchWriter.CreateOperationRequestMessage("POST", new Uri(serviceDocumentUri + "MySingleton"), contentIdRef);
                PopulateWebResourceId(createOperationMessage, 8);

                // Header modification on inner payload.
//                createOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                Assert.NotNull(createOperationMessage.ContentId);

                // A change set with multi update operation.
                batchWriter.WriteStartChangeset();
                {
                    // Create a update operation in the change set.
                    ODataBatchOperationRequestMessage updateOperationMessage =
                        batchWriter.CreateOperationRequestMessage("PATCH", new Uri(serviceDocumentUri + "/MySingleton"), "2A" /*written*/);
                    PopulateWebResourceId(updateOperationMessage, 9);
                }
                batchWriter.WriteEndChangeset();

                // An update operation referencing the preceding create operation.
                Uri referenceUri = new Uri("$" + contentIdRef, UriKind.Relative);
                ODataBatchOperationRequestMessage topLevelOperationMessage =
                    batchWriter.CreateOperationRequestMessage("PATCH", referenceUri, "3", BatchPayloadUriOption.AbsoluteUri );
                PopulateWebResourceId(topLevelOperationMessage, 10);

                // Header modification on inner payload.
//                topLevelOperationMessage.SetHeader("Accept", "application/json;odata.metadata=full");
                Assert.NotNull(topLevelOperationMessage.ContentId);

                batchWriter.WriteEndBatch();

                stream.Position = 0;
                return stream.ToArray();
            }
        }

        private byte[] ServiceReadRequestAndWriterResponseForMultipartBatchVerifyDependsOnIds(byte[] requestPayload, ODataVersion odataVersion)
        {
            IODataRequestMessage requestMessage = new InMemoryMessage() { Stream = new MemoryStream(requestPayload) };
            requestMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);

            using (ODataMessageReader messageReader = new ODataMessageReader(requestMessage,
                new ODataMessageReaderSettings
                {
                    Version = odataVersion,
                    BaseUri = new Uri(serviceDocumentUri)
                },
                this.userModel))
            {
                MemoryStream responseStream = new MemoryStream();

                IODataResponseMessage responseMessage = new InMemoryMessage { Stream = responseStream };

                // Client is expected to receive the response message in the same format as that is used in the request sent.
                responseMessage.SetHeader("Content-Type", batchContentTypeMultipartMime);
                ODataMessageWriter messageWriter = new ODataMessageWriter(responseMessage);
                ODataBatchWriter batchWriter = messageWriter.CreateODataBatchWriter();
                batchWriter.WriteStartBatch();

                ODataBatchReader batchReader = messageReader.CreateODataBatchReader();
                while (batchReader.Read())
                {
                    switch (batchReader.State)
                    {
                        case ODataBatchReaderState.Operation:
                            // Encountered an operation (either top-level or in a change set)
                            ODataBatchOperationRequestMessage operationMessage = batchReader.CreateOperationRequestMessage();

                            if (operationMessage.Method == "PATCH")
                            {
                                ODataBatchOperationResponseMessage response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 204;
                                response.SetHeader("Content-Type", "application/json;odata.metadata=none");
                            }
                            else if (operationMessage.Method == "GET")
                            {
                                // Set the content-id of response to that of the corresponding request.
                                // This enables us to correlate request and response to the maximum extend.
                                // For multipart batch, it is still null.
                                // For Json batch, it utilizes the possible non-null value created during the
                                // writer's creating of the batch request.
                                ODataBatchOperationResponseMessage response = batchWriter.CreateOperationResponseMessage(operationMessage.ContentId);
                                response.StatusCode = 200;
                                response.SetHeader("Content-Type", "application/json;");
                                ODataMessageWriterSettings settings = new ODataMessageWriterSettings();
                                settings.SetServiceDocumentUri(new Uri(serviceDocumentUri));
                                using (ODataMessageWriter operationMessageWriter = new ODataMessageWriter(response, settings, this.userModel))
                                {
                                    ODataWriter entryWriter = operationMessageWriter.CreateODataResourceWriter(this.singleton, this.webType);
                                    ODataResource entry = new ODataResource
                                    {
                                        TypeName = "NS.Web",
                                        Properties = new[]
                                        {
                                            new ODataProperty() { Name = "WebId", Value = 10 },
                                            new ODataProperty() { Name = "Name", Value = "WebSingleton" }
                                        }
                                    };
                                    entryWriter.WriteStart(entry);
                                    entryWriter.WriteEnd();
                                }
                            }

                            break;
                        case ODataBatchReaderState.ChangesetStart:
                            batchWriter.WriteStartChangeset();
                            break;
                        case ODataBatchReaderState.ChangesetEnd:
                            batchWriter.WriteEndChangeset();
                            break;
                    }
                }

                batchWriter.WriteEndBatch();
                responseStream.Position = 0;
                return responseStream.ToArray();
            }
        }

        private static string GetContentTypeHeader(BatchFormat batchFormat)
        {
            return batchFormat == BatchFormat.MultipartMIME
                ? batchContentTypeMultipartMime
                : batchContentTypeApplicationJson;
        }

        /// <summary>
        /// Normalize the json message by replacing GUIDs and removing white spaces from formatted input.
        /// </summary>
        /// <param name="jsonMessage">The json message to be normalized.</param>
        /// <returns>The normalized message.</returns>
        private string GetNormalizedJsonMessage(string jsonMessage)
        {
            const string myIdProperty = @"""id"":""my_id_guid""";
            const string myAtomicGroupProperty = @"""atomicityGroup"":""my_groupid_guid""";
            const string myDependsOnProperty = @"""dependsOn"":""[my_ids]""";
            const string myODataVersionProperty = @"""odata-version"":""myODataVer""";

            string result = Regex.Replace(jsonMessage, @"\s*", "", RegexOptions.Multiline);
            result = Regex.Replace(result, "\"id\":\"[^\"]*\"", myIdProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"atomicityGroup\":\"[^\"]*\"", myAtomicGroupProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"dependsOn\":\\[\"[^\\]]*\\]", myDependsOnProperty, RegexOptions.Multiline);
            result = Regex.Replace(result, "\"odata-version\":\"[^\"]*\"", myODataVersionProperty, RegexOptions.Multiline);
            return result;
        }
    }
}
