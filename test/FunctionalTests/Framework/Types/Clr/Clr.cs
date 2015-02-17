//---------------------------------------------------------------------
// <copyright file="Clr.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
	// Clr
	//
	////////////////////////////////////////////////////////   
    public static class Clr
    {
        //Statics
        public static NodeValue             Value<T>(T value)
        {
            return new NodeValue(value, Clr.Types[typeof(T)]    );
        }

        private static NodeTypes _types = null;
        public static NodeTypes             Types
        {
            get
            {
                if (_types == null)
                {
                    _types = new NodeTypes(null,
                        new ClrInt32(),
                        new ClrInt16(),
                        new ClrString(),
                        new ClrBoolean(),
                        new ClrDateTime(),
                        new ClrDecimal(),
                        new ClrFloat(),
                        new ClrByte(),
                        new ClrDouble(),
                        new ClrBinary(),
                        new ClrGuid(),
                        new ClrInt64(),
                        new ClrSByte()
                        //TODO...
                        );
                }
                return _types;
            }
        }
    }
}
