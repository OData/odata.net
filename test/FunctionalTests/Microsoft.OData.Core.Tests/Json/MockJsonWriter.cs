//---------------------------------------------------------------------
// <copyright file="MockJsonWriter.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using FluentAssertions;
using Microsoft.OData.Core.Json;
using Microsoft.OData.Edm.Library;

namespace Microsoft.OData.Core.Tests.Json
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
            throw new System.NotImplementedException();
        }

        public void EndPaddingFunctionScope()
        {
            throw new System.NotImplementedException();
        }

        public void StartObjectScope()
        {
            throw new System.NotImplementedException();
        }

        public void EndObjectScope()
        {
            throw new System.NotImplementedException();
        }

        public void StartArrayScope()
        {
            throw new System.NotImplementedException();
        }

        public void EndArrayScope()
        {
            throw new System.NotImplementedException();
        }

        public void WriteName(string name)
        {
            this.WriteNameVerifier.Should().NotBeNull();
            this.WriteNameVerifier(name);
        }

        public void WritePaddingFunctionName(string functionName)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(bool value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(int value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(float value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(short value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(long value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(double value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(System.Guid value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(decimal value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(Date value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(System.DateTimeOffset value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(System.TimeSpan value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(TimeOfDay value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(byte value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(sbyte value)
        {
            throw new System.NotImplementedException();
        }

        public void WriteValue(string value)
        {
            this.WriteValueVerifier.Should().NotBeNull();
            this.WriteValueVerifier(value);
        }

        public void WriteValue(byte[] value)
        {
            this.WriteValueVerifier.Should().NotBeNull();
            this.WriteValueVerifier(Convert.ToBase64String(value));
        }

        public void WriteRawValue(string rawValue)
        {
            this.WriteValueVerifier.Should().NotBeNull();
            this.WriteValueVerifier(rawValue);
        }

        public void Flush()
        {
            throw new System.NotImplementedException();
        }
    }
}