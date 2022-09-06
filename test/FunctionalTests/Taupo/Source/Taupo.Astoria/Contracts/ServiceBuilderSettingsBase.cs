//---------------------------------------------------------------------
// <copyright file="ServiceBuilderSettingsBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Test.Taupo.Astoria.Contracts.WebServices.DataServiceBuilderService.DotNet;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Utilities;

    /// <summary>
    /// Provides base functionality for building settings strings (key-value pairs used to 
    /// communicate client requirements to the service builder server).
    /// </summary>
    public abstract class ServiceBuilderSettingsBase
    {
        /// <summary>
        /// Builds the settings string to be sent to the server.
        /// </summary>
        /// <returns>Settings string consisting of key-value pairs separated with semicolons.</returns>
        /// <remarks>
        /// The string can be parsed by methods in <see cref="InitStringUtilities"/> class.
        /// </remarks>
        public virtual IEnumerable<ServiceBuilderParameter> BuildSettings()
        {
            List<ServiceBuilderParameter> results = new List<ServiceBuilderParameter>();

            foreach (var prop in this.GetType().GetProperties().Where(c => !c.IsDefined(typeof(IgnoreDataMemberAttribute), false)))
            {
                object value = prop.GetValue(this, null);
                if (value != null)
                {
                    var valueType = value.GetType();
                    ExceptionUtilities.Assert(valueType != typeof(DateTime), "Serialization will result in lossy conversion, need to update serialization/deserialization if assert hit");
                    ExceptionUtilities.Assert(valueType != typeof(float), "Serialization will result in lossy conversion, need to update serialization/deserialization if assert hit");
                    ExceptionUtilities.Assert(valueType != typeof(double), "Serialization will result in lossy conversion, need to update serialization/deserialization if assert hit");
                    ExceptionUtilities.Assert(valueType != typeof(decimal), "Serialization will result in lossy conversion, need to update serialization/deserialization if assert hit");

                    ServiceBuilderParameter param = new ServiceBuilderParameter
                    {
                        Name = prop.Name,
                        Value = Convert.ToString(value, CultureInfo.InvariantCulture),
                    };

                    results.Add(param);
                }
            }

            return results;
        }
    }
}