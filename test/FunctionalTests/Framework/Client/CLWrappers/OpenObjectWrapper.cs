//---------------------------------------------------------------------
// <copyright file="OpenObjectWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.OData.Client;

#if ASTORIA_OPEN_OBJECT

namespace System.Data.Test.Astoria
{
    public class OpenObjectWrapper
    {
        private  Dictionary<string, object> propertySet = new Dictionary<string, object>();
        private Microsoft.OData.Client.OpenObject _OpenObject;

        public OpenObjectWrapper()
        {
            this._OpenObject = new Microsoft.OData.Client.OpenObject();
        }

       public System.Collections.Generic.Dictionary<string, object> OpenProperties
        {
            get
            {
                propertySet =  this._OpenObject.OpenProperties;
                return propertySet;
            }
        }

        public T Field<T>(String property)
        {
            return this._OpenObject.Field<T>(property);
        }

        public object this[string property]
        {
            get { return this.propertySet[property]; }
            set { this.propertySet[property] = value; }
        }
    }
}
#endif
