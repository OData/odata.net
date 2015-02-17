//---------------------------------------------------------------------
// <copyright file="KeyAttWrapper.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using Microsoft.OData.Client;


namespace System.Data.Test.Astoria
{
    public class KeyAttWrapper
    {
        private Microsoft.OData.Client.KeyAttribute _KeyAttribute;

        public KeyAttWrapper()
        {
            this._KeyAttribute = new Microsoft.OData.Client.KeyAttribute();
        }

        public object TypeId
        {
            get
            {
                return this._KeyAttribute.TypeId;
            }
        }
    }
}
