//---------------------------------------------------------------------
// <copyright file="MockCharArrayPool.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Buffers;

namespace Microsoft.OData.Tests.Json
{
    public class MockCharArrayPool : ICharArrayPool
    {
        public Action<int> RentVerifier;
        public Action<char[]> ReturnVerifier;

        public char[] Rent(int minSize)
        {
            var charArray = new char[minSize];
            RentVerifier(minSize);
            
            return charArray;
        }

        public void Return(char[] array)
        {
            ReturnVerifier(array);
            // Do nothing else
        }
    }
}
