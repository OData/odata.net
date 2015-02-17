//---------------------------------------------------------------------
// <copyright file="OracleClient.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    public partial class Versioning
    {
        public static _Client Client
        {
            get
            {
                switch (AstoriaTestProperties.ClientVersion)
                {
                    case null: return new _Client();
                    case "V1": return new _V1Client();
                    case "V2": return new _V2Client();
                    case "Dev10": return new _Dev10Client();
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /ClientVersion parameter value");
                        return null;
                }
            }
        }

        // Current client.
        public class _Client
        {
            internal _Client() { }

            // Version of .NET framework required to compile and run client application.
            public virtual string FrameworkVersion { get { return "3.5"; } }

            // Returns true if build results (success or failure) was unexpected.
            public virtual bool VerifyResults(bool success, string log)
            {
                switch (AstoriaTestProperties.DesignVersion)
                {
                    case null:
                    case "V1":
                    case "V2":
                    case "Dev10":
                        return success == true;
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /ClientVersion parameter value");
                        return success == true;
                }
            }

            // Returns assembly references necessary to build client applications.
            public virtual string AssemblyReferences
            {
                get
                {
                    return
                        DataFxAssemblyRef.File.DataServices + "," +
                        DataFxAssemblyRef.File.DataServicesClient + "," +
                        @"WindowsBase.dll";
                }
            }
        }

        // V1 client.
        public class _V1Client : _Client
        {
            internal _V1Client() { }
            public override string FrameworkVersion { get { return "3.5"; } }

            public override bool VerifyResults(bool success, string log)
            {
                switch (AstoriaTestProperties.DesignVersion)
                {
                    case null:
                    case "V1":
                    case "V2":
                    case "Dev10":
                        return success == true;
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /ClientVersion parameter value");
                        return success == true;
                }
            }

            public override string AssemblyReferences { get { return DataFxAssemblyRef.File.DataServices + "," + DataFxAssemblyRef.File.DataServicesClient; } }
        }

        // Dev10 client.
        public class _Dev10Client : _Client
        {
            internal _Dev10Client() { }
            public override string FrameworkVersion { get { return "4.0"; } }

            public override bool VerifyResults(bool success, string log)
            {
                switch (AstoriaTestProperties.DesignVersion)
                {
                    case null:
                    case "V1":
                    case "V2":
                    case "Dev10":
                        return success == true;
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /ClientVersion parameter value");
                        return success == true;
                }
            }

            public override string AssemblyReferences
            {
                get
                {
                    return DataFxAssemblyRef.File.DataServices + "," +
                           DataFxAssemblyRef.File.DataServicesClient + "," +
                           "WindowsBase.dll";
                }
            }
        }

        // V2 client.
        public class _V2Client : _Client { }
    }
}
