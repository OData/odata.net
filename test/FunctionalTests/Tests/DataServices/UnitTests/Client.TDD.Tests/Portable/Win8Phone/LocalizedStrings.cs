//---------------------------------------------------------------------
// <copyright file="LocalizedStrings.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using DSClient.Win8Phone.Delta.UnitTests.Resources;

namespace DSClient.Win8Phone.Delta.UnitTests
{
    /// <summary>
    /// Provides access to string resources.
    /// </summary>
    public class LocalizedStrings
    {
        private static AppResources _localizedResources = new AppResources();

        public AppResources LocalizedResources { get { return _localizedResources; } }
    }
}