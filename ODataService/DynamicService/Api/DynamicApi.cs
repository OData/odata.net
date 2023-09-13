// Copyright (c) Microsoft Corporation.  All rights reserved.
// Licensed under the MIT License.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.OData.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.Edm;
using Microsoft.Restier.Core;
using Microsoft.Restier.Core.Model;
using Microsoft.Restier.Core.Submit;
//using Microsoft.Restier.EntityFramework;
using Microsoft.OData.Edm.Csdl;
using System.Xml;
using Microsoft.OData.Edm.Validation;


namespace DynamicService
{
    public class DynamicApi : ApiBase
    {
       public DynamicApi(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }

    public class DynamicModelBuilder : IModelBuilder
    { 
        public IModelBuilder InnerHandler { get; set; }

        public string DataSourceName { get; set; }
        public IEdmModel GetModel(ModelContext context)
        {
            IEdmModel model;
            IEnumerable<EdmError> errors;
            // todo: improve this logic
            DataSourceName = "NWind";
            var path = AppDomain.CurrentDomain.GetData("ContentRootPath") as String;
            var file = Path.Combine(path, DataSourceName + ".csdl");

            XmlReader xmlReader = XmlReader.Create(file);
            if (CsdlReader.TryParse(xmlReader, out model, out errors))
            {
                return model;
            }

            throw new Exception("Couldn't parse xml");
        }

        public async Task<IEdmModel> GetModelAsync(ModelContext context, CancellationToken cancellationToken)
        {
            return await Task.FromResult(GetModel(context));
        }

    }
}