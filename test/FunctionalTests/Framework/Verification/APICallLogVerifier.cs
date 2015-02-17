//---------------------------------------------------------------------
// <copyright file="APICallLogVerifier.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Test.Astoria.CallOrder;
using System.IO;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
{
    public static class APICallLogVerifier
    {
        public static List<string> ClassesToIgnore = new List<string>();

        public static bool Equal(APICallLogEntry expected, APICallLogEntry observed)
        {
            if (expected.MethodName != observed.MethodName)
                return false;

            if (expected.Arguments.Length != observed.Arguments.Length)
                return false;

            for (int i = 0; i < expected.Arguments.Length; i++)
            {
                if (expected.Arguments[i].Key != observed.Arguments[i].Key)
                {
                    return false;
                }

                string expectedValue = expected.Arguments[i].Value;
                string observedValue = observed.Arguments[i].Value;
                if (expectedValue != observedValue)
                {
                    double temp1, temp2;
                    if (double.TryParse(expectedValue, out temp1) && double.TryParse(observedValue, out temp2))
                        return temp1 == temp2;

                    // TODO: when else can we ignore this?
                    return false;
                }
            }

            return true;
        }

        public static void Verify(APICallLogBuilder callLogBuilder, AstoriaResponse response, params ComplexType[] types)
        {
            if (!response.Request.LogAPICalls)
                AstoriaTestLog.FailAndThrow("Cannot verify call order when the request did not log the calls");

            Verify(callLogBuilder, response, Equal, types);
        }

        public static void Verify(APICallLogBuilder callLogBuilder, AstoriaResponse response, Func<APICallLogEntry, APICallLogEntry, bool> compare, params ComplexType[] types)
        {
            if (!AstoriaTestProperties.IsLabRun)
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("------------------------------------");
                builder.AppendLine("");
                builder.AppendLine("Verifying call order");
                builder.AppendLine("");

                if(types.Length > 0)
                {
                    builder.AppendLine("Metadata:");
                    WriteTypesToLog(builder, types);
                    builder.AppendLine("");
                }

                builder.AppendLine("Request:");
                response.Request.LogRequest(builder, true, true);
                builder.AppendLine("");
                builder.AppendLine("Response:");
                response.LogResponse(builder, true, true);
                builder.AppendLine("");
                AstoriaTestLog.WriteLineIgnore(builder.ToString());
            }

            try
            {
                Verify(callLogBuilder.Entries, response.Request.APICallLogEntries, compare);
            }
            catch (Exception ex)
            {
                if (types.Length > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("Metadata:");
                    WriteTypesToLog(builder, types);
                    builder.AppendLine("");
                    AstoriaTestLog.WriteLineIgnore(builder.ToString());
                }

                ResponseVerification.LogFailure(response, ex);
            }
        }

        public static void Verify(List<APICallLogEntry> expectedCalls, List<APICallLogEntry> observedCalls)
        {
            Verify(expectedCalls, observedCalls, Equal);
        }

        public static void Verify(List<APICallLogEntry> expectedCalls, List<APICallLogEntry> observedCalls, Func<APICallLogEntry, APICallLogEntry, bool> compare)
        {
            observedCalls = observedCalls.Where(e => !ClassesToIgnore.Any(c => e.MethodName.StartsWith(c + "."))).ToList();

            List<int> mismatchedLines = new List<int>();

            AstoriaTestLog.WriteLineIgnore("Observed call order: ");
            for(int i = 0; i < observedCalls.Count; i++)
            {
                if (i >= expectedCalls.Count || !compare(expectedCalls[i], observedCalls[i]))
                {
                    AstoriaTestLog.WriteLine("(Observed) " + i + " - " + observedCalls[i].ToString());
                    mismatchedLines.Add(i);
                }
                else if(!AstoriaTestProperties.IsLabRun)
                {
                    AstoriaTestLog.WriteLineIgnore("(Observed) " + i + " - " + observedCalls[i].ToString());
                }
            }
            
            if (observedCalls.Count < expectedCalls.Count)
            {
                for (int i = observedCalls.Count; i < expectedCalls.Count; i++)
                {
                    mismatchedLines.Add(i);
                }
            }

            if (mismatchedLines.Any())
            {
                AstoriaTestLog.WriteLineIgnore("Expected call order: ");
                for (int i = 0; i < expectedCalls.Count; i++)
                {
                    if(mismatchedLines.Contains(i))
                        AstoriaTestLog.WriteLine("(Expected) " + i + " - " + expectedCalls[i].ToString());
                    else
                        AstoriaTestLog.WriteLineIgnore("(Expected) " + i + " - " + expectedCalls[i].ToString());
                }

                AstoriaTestLog.FailAndThrow("Observed call log did not match baseline");
            }
        }

        private static void WriteTypesToLog(StringBuilder builder, IEnumerable<ComplexType> types)
        {
            foreach(var type in types)
            {
                if(type.BaseType != null)
                {
                    WriteTypesToLog(builder, new[] { type.BaseType });
                }

                builder.Append("    " + type.FullName);
                if(type.BaseType != null)
                {
                    builder.AppendLine(" : " + type.BaseType.FullName);
                }
                else
                {
                    builder.AppendLine();
                }

                builder.AppendLine("    {");
                builder.AppendLine("      IsOpenType: " + type.Facets.IsOpenType);
                builder.AppendLine("      CanReflect: " + type.Facets.IsClrType);
                builder.AppendLine("      HasStream: " + type.Facets.HasStream);
                builder.AppendLine("      NamedStreams: " + string.Join(", ", type.Facets.NamedStreams.ToArray()));
                builder.AppendLine("      Properties:");
                builder.AppendLine("      {");
                foreach(var property in type.Properties.Cast<ResourceProperty>())
                {
                    builder.AppendLine("        " + WriteProperty(property));
                }

                builder.AppendLine("      }");
                builder.AppendLine("    }");
                builder.AppendLine("");
            }
        }

        private static string WriteProperty(ResourceProperty property)
        {
            List<string> facets = new List<string>();
            if (property.PrimaryKey != null)
            {
                facets.Add("Id");
            }

            if (property.Facets.ConcurrencyModeFixed)
            {
                facets.Add("ETag");
            }

            if (property.Facets.IsClrProperty)
            {
                facets.Add("CanReflect");
            }

            if (!property.Facets.IsDeclaredProperty)
            {
                facets.Add("Dynamic");
            }

            if (facets.Count > 0)
            {
                return string.Format("{0} {1} ({2})", property.Type.FullName, property.Name, string.Join(", ", facets.ToArray()));
            }
            else
            {
                return string.Format("{0} {1}", property.Type.FullName, property.Name);
            }
        }
    }
}
