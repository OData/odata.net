//---------------------------------------------------------------------
// <copyright file="MockJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using Microsoft.OData.Json;
using Microsoft.OData.Edm;
using Xunit;

namespace Microsoft.OData.Tests.Json
{
    /// <summary>
    /// This is intended to be a Mock for the JsonWriter.
    /// 
    /// So far only a a few things have been implemented.
    /// </summary>
    internal class MockJsonWriter : IJsonWriter
    {
        public Action<string> WriteNameVerifier;
        public Action<string> WriteValueVerifier;

        public void StartPaddingFunctionScope()
        {
            throw new NotImplementedException();
        }

        public void EndPaddingFunctionScope()
        {
            throw new NotImplementedException();
        }

        public void StartObjectScope()
        {
            throw new NotImplementedException();
        }

        public void EndObjectScope()
        {
            throw new NotImplementedException();
        }

        public void StartArrayScope()
        {
            throw new NotImplementedException();
        }

        public void EndArrayScope()
        {
            throw new NotImplementedException();
        }

        public void WriteName(string name)
        {
            Assert.NotNull(this.WriteNameVerifier);
            this.WriteNameVerifier(name);
        }

        public void WritePaddingFunctionName(string functionName)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(bool value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(int value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(float value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(short value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(long value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(double value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(Guid value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(decimal value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(Date value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(DateTimeOffset value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(TimeSpan value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(TimeOfDay value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(byte value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(sbyte value)
        {
            throw new NotImplementedException();
        }

        public void WriteValue(string value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(value);
        }

        public void WriteValue(byte[] value)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(Convert.ToBase64String(value));
        }

        public void WriteRawValue(string rawValue)
        {
            Assert.NotNull(this.WriteValueVerifier);
            this.WriteValueVerifier(rawValue);
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }
    }
}