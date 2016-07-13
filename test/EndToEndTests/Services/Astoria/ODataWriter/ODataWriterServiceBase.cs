//---------------------------------------------------------------------
// <copyright file="ODataWriterServiceBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.Services.ODataWriterService
{
    using System.Collections.Generic;
    using Microsoft.OData.Service;
    using System.IO;
    using System.Linq;
    using System.ServiceModel;
    using System.Xml;
#if TEST_ODATA_SERVICES_ASTORIA || TEST_ODATA_SERVICES_ASTORIA_NOPUBLICPROVIDER
    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.Edm.Validation;
#else
    using Microsoft.Data.Edm;
    using Microsoft.Data.Edm.Csdl;
    using Microsoft.Data.Edm.Validation;
#endif
    using Microsoft.OData;
    using BaseService = Microsoft.Test.OData.Services.AstoriaDefaultService.Service;

    [ServiceBehaviorAttribute(IncludeExceptionDetailInFaults = true)]
    public class ODataWriterServiceBase<TDataServiceODataWriter> : BaseService where TDataServiceODataWriter : DataServiceODataWriter
    {
        public ODataWriterServiceBase()
        {
            this.ODataWriterFactory =
                (odataWriter) =>
                {
                    var odataWriterConstructor = typeof(TDataServiceODataWriter).GetConstructors().Single(c => c.GetParameters().Count() == 1 && c.GetParameters().Single().ParameterType == typeof(ODataWriter));
                    var customODataWriter = (TDataServiceODataWriter)odataWriterConstructor.Invoke(new object[] { odataWriter });
                    return customODataWriter;
                };
        }

        public static new void InitializeService(DataServiceConfiguration config)
        {
            BaseService.InitializeService(config);

            config.AnnotationsBuilder =
                (model) =>
                {
                    var xmlReaders = new XmlReader[] { XmlReader.Create(new StringReader(@"
                        <Schema xmlns=""http://docs.oasis-open.org/odata/ns/edm"" Namespace=""Microsoft.Test.OData.Services.ODataWriterService"" >
                            <Annotations Target=""Microsoft.Test.OData.Services.AstoriaDefaultService.Customer"">
                                <Annotation Term=""CustomInstanceAnnotations.Term1"" Bool=""true"" />
                            </Annotations>
                        </Schema>
                        ")) };

                    IEdmModel annotationsModel;
                    IEnumerable<EdmError> errors;
                    bool parsed = SchemaReader.TryParse(xmlReaders, model, out annotationsModel, out errors);
                    if (!parsed)
                    {
                        throw new EdmParseException(errors);
                    }

                    return new IEdmModel[] { annotationsModel };
                };
        }
    }
}
