//-----------------------------------------------------------------------------
// <copyright file="WebODataControllerFeatureProvider.cs" company=".NET Foundation">
//      Copyright (c) .NET Foundation and Contributors. All rights reserved.
//      See License.txt in the project root for license information.
// </copyright>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;

namespace Microsoft.OData.Client.E2E.TestCommon
{
    public class WebODataControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>, IApplicationFeatureProvider
    {
        private Type[] _controllers;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebODataControllerFeatureProvider"/> class.
        /// </summary>
        /// <param name="controllers">The controllers</param>
        public WebODataControllerFeatureProvider(params Type[] controllers)
        {
            _controllers = controllers;
        }

        /// <summary>
        /// Updates the feature instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="ApplicationPart" /> instances in the application.</param>
        /// <param name="feature">The controller feature.</param>
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (_controllers == null)
            {
                return;
            }

            feature.Controllers.Clear();
            foreach (var type in _controllers)
            {
                feature.Controllers.Add(type.GetTypeInfo());
            }
        }
    }
}
