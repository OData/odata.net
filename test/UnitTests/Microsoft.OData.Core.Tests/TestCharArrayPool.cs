//---------------------------------------------------------------------
// <copyright file="TestCharArrayPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Buffers;

namespace Microsoft.OData.Tests
{
    public class TestCharArrayPool : ICharArrayPool
    {
        public char[] Buffer { get { return Rent(MinSize); } }
        
        public int MinSize { set; get; }
        
        public TestCharArrayPool(int minSize)
        {
            this.MinSize = minSize;
        }
        
        public char[] Rent(int minSize)
        {
            return new char[minSize];
        }

        public void Return(char[] array)
        {
            throw new NotImplementedException();
        }
    }
}