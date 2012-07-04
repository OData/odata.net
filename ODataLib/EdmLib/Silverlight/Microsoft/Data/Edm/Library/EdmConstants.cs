//   Copyright 2011 Microsoft Corporation
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;

namespace Microsoft.Data.Edm.Library
{
    /// <summary>
    /// Contains constant values that apply to the EDM model, regardless of source (for CSDL/EDMX specific constants see <see cref="Microsoft.Data.Edm.Csdl.CsdlConstants"/>).
    /// </summary>
    public static class EdmConstants
    {
        /// <summary>
        /// Version 1.0 of EDM. Corresponds to CSDL namespace "http://schemas.microsoft.com/ado/2006/04/edm".
        /// </summary>
        public static readonly Version EdmVersion1 = new Version(1, 0);
    
        /// <summary>
        /// Version 1.1 of EDM. Corresponds to CSDL namespace "http://schemas.microsoft.com/ado/2007/05/edm".
        /// </summary>
        public static readonly Version EdmVersion1_1 = new Version(1, 1);
    
        /// <summary>
        /// Version 1.2 of EDM. Corresponds to CSDL namespace "http://schemas.microsoft.com/ado/2008/01/edm".
        /// </summary>
        public static readonly Version EdmVersion1_2 = new Version(1, 2);
     
        /// <summary>
        /// Version 2.0 of EDM. Corresponds to CSDL namespaces "http://schemas.microsoft.com/ado/2008/09/edm" and "http://schemas.microsoft.com/ado/2009/08/edm".
        /// </summary>
        public static readonly Version EdmVersion2 = new Version(2, 0);
     
        /// <summary>
        /// Version 3.0 of EDM. Corresponds to CSDL namespace "http://schemas.microsoft.com/ado/2009/11/edm".
        /// </summary>
        public static readonly Version EdmVersion3 = new Version(3, 0);
     
        /// <summary>
        /// The current latest version of EDM.
        /// </summary>
        public static readonly Version EdmVersionLatest = EdmVersion3;

        internal const string EdmNamespace = "Edm";
        internal const string TransientNamespace = "Transient";

        internal const string XmlPrefix = "xml";
        internal const string XmlNamespacePrefix = "xmlns";

        /// <summary>
        /// The URI of annotations that will be serialized as documentation elements.
        /// </summary>
        internal const string DocumentationUri = "http://schemas.microsoft.com/ado/2011/04/edm/documentation";

        /// <summary>
        /// The local name of annotations that will be serialized as documentation elements.
        /// </summary>
        internal const string DocumentationAnnotation = "Documentation";

        /// <summary>
        /// The URI of annotations that are internal and will not be serialized.
        /// </summary>
        internal const string InternalUri = "http://schemas.microsoft.com/ado/2011/04/edm/internal";

        /// <summary>
        /// The local name of the annotation that stores the data services version attribute for EDMX serialization.
        /// </summary>
        internal const string DataServiceVersion = "DataServiceVersion";

        /// <summary>
        /// The local name of the annotation that stores the max data services version attribute for EDMX serialization.
        /// </summary>
        internal const string MaxDataServiceVersion = "MaxDataServiceVersion";

        /// <summary>
        /// The local name of the annotation that stores EDM version of a model.
        /// </summary>
        internal const string EdmVersionAnnotation = "EdmVersion";

        internal const string FacetName_Nullable = "Nullable";
        internal const string FacetName_Precision = "Precision";
        internal const string FacetName_Scale = "Scale";
        internal const string FacetName_MaxLength = "MaxLength";
        internal const string FacetName_FixedLength = "FixedLength";
        internal const string FacetName_Unicode = "Unicode";
        internal const string FacetName_Collation = "Collation";
        internal const string FacetName_Srid = "SRID";

        internal const string Value_UnknownType = "UnknownType";
        internal const string Value_UnnamedType = "UnnamedType";
        internal const string Value_Max = "Max";
        internal const string Value_SridVariable = "Variable";

        internal const string Type_Association = "Association";
        internal const string Type_Collection = "Collection";
        internal const string Type_Complex = "Complex";
        internal const string Type_Entity = "Entity";
        internal const string Type_EntityReference = "EntityReference";
        internal const string Type_Enum = "Enum";
        internal const string Type_Row = "Row";

        internal const string Type_Primitive = "Primitive";
        internal const string Type_Binary = "Binary";
        internal const string Type_Decimal = "Decimal";
        internal const string Type_String = "String";
        internal const string Type_Stream = "Stream";
        internal const string Type_Spatial = "Spatial";
        internal const string Type_Temporal = "Temporal";

        internal const string Type_Structured = "Structured";

        internal const int Max_Precision = Int32.MaxValue;
        internal const int Min_Precision = 0;
    }
}
