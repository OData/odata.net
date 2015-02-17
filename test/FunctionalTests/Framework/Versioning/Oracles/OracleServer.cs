//---------------------------------------------------------------------
// <copyright file="OracleServer.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    public partial class Versioning
    {
        public static string DefaultAstoriaVersion = "Live";

        public static _Server Server
        {
            get
            {
                switch (AstoriaTestProperties.ServerVersion)
                {
                    case null:      return new _LiveServer(); // THIS IS SPECIFIC TO THE BRANCH, DO NOT OVERWRITE WHEN PORTING CHANGES
                    case "V1":      return new _V1Server();
                    case "V2":      return new _V2Server();
                    case "Dev10":   return new _Dev10Server();
                    case "Live":    return new _LiveServer();
                    default:
                        AstoriaTestLog.FailAndThrow("Unknown /ServerVersion parameter value");
                        return null;
                }
            }
        }

        // Default server
        public abstract class _Server
        {
            internal _Server() { }
            public abstract void AdjustWebConfig(WebConfig wc);
            public abstract bool SupportsV2Features { get; }
            public abstract string DataServiceVersion { get; }
            public abstract bool SupportsEF4 { get; }
            public abstract bool SupportsLiveFeatures { get; }

            #region bug fixes
            public abstract bool BugFixed_NullETagWhenTypeHasNoConcurrency { get; }
            public abstract bool BugFixed_JsonPayloadExtraCharacters { get; }
            public abstract bool BugFixed_JsonReaderEscapeCharacters { get; }
            public abstract bool BugFixed_LinksUriElementInWrongNamespace { get; }
            public abstract bool BugFixed_NonNumericContentIDHasIncorrectError { get; }
            public abstract bool BugFixed_CannotInvokeServiceOperationsInBatch { get; }
            public abstract bool BugFixed_NotCheckingETagValuesOnPropertyUpdate { get; }
            public abstract bool BugFixed_ServiceRejectsUppercaseContentTypes { get; }
            public abstract bool BugFixed_JsonSerializerEscapesSingleQuotes { get; }
            #endregion
        }

        // V1 server.
        public class _V1Server : _Server
        {
            internal _V1Server() { }
            public override void AdjustWebConfig(WebConfig wc)
            {
                wc._AssemblyDomain = "System";
                wc._CompilerVersion = "3.5";
                wc._AssemblyVersion = "3.5.0.0";
                wc._CompilerAssemblyVersion = "2.0.0.0";
            }
            public override bool SupportsV2Features { get { return false; } }
            public override string DataServiceVersion { get { return "1.0"; } }
            public override bool SupportsEF4 { get { return false; } }
            public override bool SupportsLiveFeatures { get { return false; } }
            
            #region bug fixes
            public override bool  BugFixed_NullETagWhenTypeHasNoConcurrency
            {
                get
                {
                    Version osVersion = null;
                    string machineName = AstoriaTestProperties.HostMachineName;
                    if (machineName.Equals("localhost", StringComparison.InvariantCultureIgnoreCase) || machineName.Equals(Environment.MachineName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        osVersion = Environment.OSVersion.Version;
                    }

                    // this bug is only fixed in the hotfix for Win7
                    if (osVersion.Major == 6 && osVersion.Minor == 1)
                        return true;
                    return false;
                }
            }

            public override bool BugFixed_JsonPayloadExtraCharacters { get { return false; } }
            public override bool BugFixed_JsonReaderEscapeCharacters { get { return false; } }
            public override bool BugFixed_LinksUriElementInWrongNamespace { get { return false; } }
            public override bool BugFixed_NonNumericContentIDHasIncorrectError { get { return false; } }
            public override bool BugFixed_CannotInvokeServiceOperationsInBatch { get { return false; } }
            public override bool BugFixed_NotCheckingETagValuesOnPropertyUpdate { get { return false; } }
            public override bool BugFixed_ServiceRejectsUppercaseContentTypes { get { return false; } }
            public override bool BugFixed_JsonSerializerEscapesSingleQuotes { get { return false; } }
            #endregion
        }
        
        // V2 server.
        public class _V2Server : _Server
        {
            public override void AdjustWebConfig(WebConfig wc) { }
            public override bool SupportsV2Features { get { return true; } }
            public override string DataServiceVersion { get { return "2.0"; } }
            public override bool SupportsEF4 { get { return false; } }
            public override bool SupportsLiveFeatures { get { return false; } }

            #region bug fixes
            public override bool BugFixed_NullETagWhenTypeHasNoConcurrency { get { return true; } }
            public override bool BugFixed_JsonPayloadExtraCharacters { get { return true; } }
            public override bool BugFixed_JsonReaderEscapeCharacters { get { return true; } }
            public override bool BugFixed_LinksUriElementInWrongNamespace { get { return true; } }
            public override bool BugFixed_NonNumericContentIDHasIncorrectError { get { return true; } }
            public override bool BugFixed_CannotInvokeServiceOperationsInBatch { get { return true; } }
            public override bool BugFixed_NotCheckingETagValuesOnPropertyUpdate { get { return true; } }
            public override bool BugFixed_ServiceRejectsUppercaseContentTypes { get { return false; } }
            public override bool BugFixed_JsonSerializerEscapesSingleQuotes { get { return false; } }
            #endregion
        }

        // Dev10 server.
        public class _Dev10Server : _Server
        {
            internal _Dev10Server() { }
            public override void AdjustWebConfig(WebConfig wc)
            {
                wc._AssemblyDomain = "System";
                wc._AssemblyDomainVersion = DataFxAssemblyRef.FXAssemblyVersion;
                wc._AssemblyPublicKeyToken = DataFxAssemblyRef.EcmaPublicKeyToken;
                wc._AssemblyVersion = "4.0.0.0";
                wc._CompilerAssemblyVersion = "4.0.0.0";
                wc._CompilerVersion = "4.0";
            }

            public override bool SupportsV2Features { get { return true; } }
            public override string DataServiceVersion { get { return "2.0"; } }
            public override bool SupportsEF4 { get { return true; } }
            public override bool SupportsLiveFeatures { get { return false; } }

            #region bug fixes
            public override bool BugFixed_NullETagWhenTypeHasNoConcurrency { get { return true; } }
            public override bool BugFixed_JsonPayloadExtraCharacters { get { return true; } }
            public override bool BugFixed_JsonReaderEscapeCharacters { get { return true; } }
            public override bool BugFixed_LinksUriElementInWrongNamespace { get { return true; } }
            public override bool BugFixed_NonNumericContentIDHasIncorrectError { get { return true; } }
            public override bool BugFixed_CannotInvokeServiceOperationsInBatch { get { return true; } }
            public override bool BugFixed_NotCheckingETagValuesOnPropertyUpdate { get { return true; } }
            public override bool BugFixed_ServiceRejectsUppercaseContentTypes { get { return true; } }
            public override bool BugFixed_JsonSerializerEscapesSingleQuotes { get { return true; } }
            #endregion
        }

        public class _LiveServer : _Server
        {
            internal _LiveServer() { }
            public override void AdjustWebConfig(WebConfig wc)
            {
                wc._AssemblyDomain = "Microsoft";
                wc._AssemblyDomainVersion = DataFxAssemblyRef.DataFxAssemblyVersion;
                wc._AssemblyPublicKeyToken = DataFxAssemblyRef.SharedLibPublicKeyToken;
                wc._AssemblyVersion = "4.0.0.0";
                wc._CompilerAssemblyVersion = "4.0.0.0";
                wc._CompilerVersion = "4.0";
            }

            public override bool SupportsV2Features { get { return true; } }
            public override string DataServiceVersion { get { return "4.0"; } }
            public override bool SupportsEF4 { get { return true; } }
            public override bool SupportsLiveFeatures { get { return true; } }

            #region bug fixes
            public override bool BugFixed_NullETagWhenTypeHasNoConcurrency { get { return true; } }
            public override bool BugFixed_JsonPayloadExtraCharacters { get { return true; } }
            public override bool BugFixed_JsonReaderEscapeCharacters { get { return true; } }
            public override bool BugFixed_LinksUriElementInWrongNamespace { get { return true; } }
            public override bool BugFixed_NonNumericContentIDHasIncorrectError { get { return true; } }
            public override bool BugFixed_CannotInvokeServiceOperationsInBatch { get { return true; } }
            public override bool BugFixed_NotCheckingETagValuesOnPropertyUpdate { get { return true; } }
            public override bool BugFixed_ServiceRejectsUppercaseContentTypes { get { return true; } }
            public override bool BugFixed_JsonSerializerEscapesSingleQuotes { get { return true; } }
            #endregion
        }
    }
}
