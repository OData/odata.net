//---------------------------------------------------------------------
// <copyright file="BaselineLogger.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.OData.Writer.Tests.WriterCombinatorialEngine
{
    using System.Text;
    using Microsoft.OData.Edm;
    using Microsoft.Test.Taupo.OData.Writer.Tests.Common;

    public class BaselineLogger
    {
        private StringBuilder builder;
        private WriterTestConfiguration config;
        private bool modelPresence;
        private int combination;


        public BaselineLogger()
        {
            builder = new StringBuilder();
        }

        public void LogConfiguration(WriterTestConfiguration config)
        {
            this.config = config;
        }

        public void LogModelPresence(IEdmModel model)
        {
            this.modelPresence = model != null;
        }

        public void LogCombination(int combination)
        {
            this.combination = combination;
        }

        public void LogPayload(string payload)
        {
            builder.AppendFormat("Combination: {0}; TestConfiguration = Format: {1}, Request: {2}, Synchronous: {3}", combination, config.Format, config.IsRequest, config.Synchronous);
            builder.AppendLine();
            builder.AppendFormat("Model Present: {0}", modelPresence ? "true" : "false");
            builder.AppendLine();
            builder.AppendLine(payload);
            builder.AppendLine();
        }

        public string GetBaseline()
        {
            return builder.ToString();
        }
    }
}
