//---------------------------------------------------------------------
// <copyright file="EdmLibCsdlSchemaEnvelope.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace EdmLibTests.FunctionalUtilities
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains schema envlope information 
    /// </summary>
    public class EdmLibCsdlSchemaEnvelope
    {
        public List<string> EntityContainerNames
        {
            get;
            private set;
        }

        public EdmLibCsdlSchemaEnvelope()
        {
            ApplyAlias = false;
            EntityContainerNames = new List<string>();
        }

        public string Alias
        {
            get;
            set;
        }
        public bool ApplyAlias
        {
            get;
            set;
        }
        public string Namespace
        {
            get;
            set;
        }
        /* TODO: How to convert CSDLs of a version to another version in Taupo?  
         *        For example, can we extend or using a different CSDL version?
        public EdmVersion EdmVersion
        {
            get;
            set;
        }
        */
    }
}