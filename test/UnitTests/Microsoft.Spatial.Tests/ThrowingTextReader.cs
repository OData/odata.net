//---------------------------------------------------------------------
// <copyright file="ThrowingTextReader.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.IO;

namespace Microsoft.Spatial.Tests
{
    class ThrowingTextReader : TextReader 
    {
        public override int Read(char[] buffer, int index, int count)
        {
            throw new NotImplementedException();            
        }
    }
}
