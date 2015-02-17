//---------------------------------------------------------------------
// <copyright file="OracleDesign.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    public partial class Versioning
    {
        public static _Design Design
        {
            get
            {
                switch (AstoriaTestProperties.ClientVersion)
                {
                    case null:      return new _Design();
                    case "V1":      return new _V1Design();
                    case "V2":      return new _V2Design();
                    case "Dev10":   return new _Dev10Design();
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /DesignVersion parameter value");
                        return null;
                }
            }
        }

        // Default code generator.
        public class _Design
        {
            internal _Design() { }

            private static string _DataSvcUtilPath;
            public virtual string DataSvcUtilPath
            {
                get
                {
                    if (null == _DataSvcUtilPath)
                    {
                        _DataSvcUtilPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), @"..\V3.5"));
                    }

                    return _DataSvcUtilPath;
                }
            }

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
                        AstoriaTestLog.FailAndThrow("Unknown /DesignVersion parameter value");
                        return success == true;
                }
            }

            public virtual string AdditionalOptions { get { return "/DataServiceCollection /Version:2.0"; } }
        }

        // V1 code generator.
        public class _V1Design : _Design
        {
            internal _V1Design() { }

            public override string DataSvcUtilPath
            {
                get
                {
                    return @"""%WinDir%\Microsoft.NET\Framework\v3.5""";
                }
            }

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
                        AstoriaTestLog.FailAndThrow("Unknown /DesignVersion parameter value");
                        return success == true;
                }
            }

            public override string AdditionalOptions { get { return "/nologo"; } }
        }

        // Dev10 code generator.
        public class _Dev10Design : _Design
        {
            internal _Dev10Design() { }

            public override string DataSvcUtilPath
            {
                get
                {
                    return @"""%WinDir%\Microsoft.NET\Framework\v4.0""";
                }
            }

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
                        AstoriaTestLog.FailAndThrow("Unknown /DesignVersion parameter value");
                        return success == true;
                }
            }

            public override string AdditionalOptions { get { return "/DataServiceCollection /Version:2.0"; } }
        }

        // V2 code generator.
        public class _V2Design : _Design {}
    }
}
