//---------------------------------------------------------------------
// <copyright file="DataServiceHostSimulator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace AstoriaUnitTests.TDD.Tests.Server.Simulators
{
    using System;
    using System.Collections.Generic;
    using Microsoft.OData.Service;

    internal class DataServiceHostSimulator : IDataServiceHost
    {
        private readonly Dictionary<String, String> queryArguments;

        public DataServiceHostSimulator()
        {
            this.queryArguments = new Dictionary<string, string>();
        }

        public Uri AbsoluteRequestUri
        {
            get;
            set;
        }

        public Uri AbsoluteServiceUri
        {
            get;
            set;
        }

        public string RequestAccept
        {
            get;
            set;
        }

        public string RequestAcceptCharSet
        {
            get;
            set;
        }

        public string RequestContentType
        {
            get;
            set;
        }

        public string RequestHttpMethod
        {
            get;
            set;
        }

        public string RequestIfMatch
        {
            get;
            set;
        }

        public string RequestIfNoneMatch
        {
            get;
            set;
        }

        public string RequestMaxVersion
        {
            get;
            set;
        }

        public System.IO.Stream RequestStream
        {
            get;
            set;
        }

        public string RequestVersion
        {
            get;
            set;
        }

        public string ResponseCacheControl
        {
            get;
            set;
        }

        public string ResponseContentType
        {
            get;
            set;
        }

        public string ResponseETag
        {
            get;
            set;
        }

        public string ResponseLocation
        {
            get;
            set;
        }

        public int ResponseStatusCode
        {
            get;
            set;
        }

        public System.IO.Stream ResponseStream
        {
            get; 
            set;
        }

        public string ResponseVersion
        {
            get;
            set;
        }

        public void SetQueryStringItem(string key, string argument)
        {
            this.queryArguments[key] = argument;
        }

        public void ClearQueryArgument()
        {
            this.queryArguments.Clear();
        }

        public string GetQueryStringItem(string item)
        {
            String result;
            if (this.queryArguments.TryGetValue(item, out result))
            {
                return result;
            }

            return null;
        }

        public Action<HandleExceptionArgs> ProcessExceptionCallBack
        {
            get;
            set;
        }

        public void ProcessException(HandleExceptionArgs args)
        {
            if (this.ProcessExceptionCallBack != null)
            {
                this.ProcessExceptionCallBack(args);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
