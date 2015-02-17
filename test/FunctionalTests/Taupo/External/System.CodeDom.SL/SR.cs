//---------------------------------------------------------------------
// <copyright file="SR.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Net;

namespace System.CodeDom
{
    public class SR
    {
        public const string CodeDomProvider_NotDefined = "CodeDomProvider Not Defined";
        public const string NotSupported_CodeDomAPI = "NotSupported_CodeDomAPI";
        public const string CodeGenOutputWriter = "CodeGenOutputWriter";
        public const string InvalidElementType = "InvalidElementType";
        public const string DuplicateFileName = "DuplicateFileName";
        public const string CodeGenReentrance = "CodeGenReentrance";
        public const string InvalidRegion = "InvalidRegion";
        public const string InvalidNullEmptyArgument = "InvalidNullEmptyArgument";
        public const string ArityDoesntMatch = "ArityDoesntMatch";
        public const string InvalidTypeName = "InvalidTypeName";
        public const string InvalidLanguageIdentifier = "InvalidLanguageIdentifier";
        public const string InvalidPathCharsInChecksum = "InvalidPathCharsInChecksum";
        public const string AutoGen_Comment_Line1 = "AutoGen_Comment_Line1";
        public const string AutoGen_Comment_Line2 = "AutoGen_Comment_Line2";
        public const string AutoGen_Comment_Line3 = "AutoGen_Comment_Line3";
        public const string AutoGen_Comment_Line4 = "AutoGen_Comment_Line4";
        public const string AutoGen_Comment_Line5 = "AutoGen_Comment_Line5";
        public const string InvalidPrimitiveType = "InvalidPrimitiveType";
        public const string Argument_NullComment = "Argument_NullComment";
        public const string InvalidIdentifier = "InvalidIdentifier";

        public static string GetString(string constant)
        {
            return null;
        }
        public static string GetString(string constant, params object[] parameters)
        {
            return null;
        }
    }
}
