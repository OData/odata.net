//---------------------------------------------------------------------
// <copyright file="CSDLUtility.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace System.Data.Test.Astoria
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public interface IMetadataItem
    {
        string ToCSDL();
    }
    public class EDMAnnotation : IMetadataItem
    {
        public bool KeepInContent { get; set; }
        public string PropertyName { get; set; }

        #region IMetadataItem Members

        public virtual string ToCSDL()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class CustomPropertyAnnotation : EDMAnnotation
    {

        public string TargetPath { get; set; }
        public string TargetNamespacePrefix { get; set; }
        public string TargetNamespaceUri { get; set; }

        #region IMetadataItem Members

        public override string ToCSDL()
        {
            return String.Format(" d3p1:FC_TargetPath=\"{0}\" d3p1:FC_SourcePath=\"{1}\" d3p1:FC_NsUri=\"{2}\" d3p1:FC_NsPrefix=\"{3}\" d3p1:FC_KeepInContent=\"{4}\" xmlns:d3p1=\"http://docs.oasis-open.org/odata/ns/metadata\"  ",
                TargetPath, PropertyName, TargetNamespaceUri, TargetNamespacePrefix, KeepInContent.ToString().ToLower());
        }

        #endregion
    }

    public class EDMProperty : IMetadataItem
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public bool IsKey { get; set; }
        public bool IsNullable { get; set; }

        public CustomPropertyAnnotation CustomAnnotation { get; set; }

        #region IMetadataItem Members

        const string _csdlTemplate = "<Property Name=\"{0}\" Type=\"{1}\" Nullable=\"{2}\"";
        const string _csdlKeyTemplate = "<Key><PropertyRef Name=\"{0}\" /></Key>";
        string IMetadataItem.ToCSDL()
        {
            StringBuilder sbCSDL = new StringBuilder();
            if (this.IsKey)
            {
                sbCSDL.AppendFormat(_csdlKeyTemplate, Name);
            }
            sbCSDL.AppendFormat(_csdlTemplate, Name, Type.FullName.Replace("System", "Edm"), this.IsNullable.ToString().ToLower());
            #region Append any annotations available

            if (CustomAnnotation != null)
            {
                IMetadataItem im = CustomAnnotation as IMetadataItem;
                sbCSDL.Append(im.ToCSDL());
            }
            #endregion
            sbCSDL.Append(" />");
            return sbCSDL.ToString();
        }
        #endregion
    }

    public class EDMComplexType : IMetadataItem
    {
        public string Namespace { get; set; }
        public EDMComplexType BaseType { get; set; }
        public string Name { get; set; }
        public List<EDMProperty> Properties { get; set; }
        public List<EDMAnnotation> Annotations { get; set; }

        public EDMComplexType(string name)
        {
            Name = name;
        }
        public EDMComplexType()
        {
        }

        #region IMetadataItem Members
        const string _entityTypeTemplate = "<ComplexType Name=\"{0}\">";
        const string _entityTypeWithBaseTypeTemplate = "<ComplexType Name=\"{0}\" BaseType=\"{1}\">";


        public string ToCSDL()
        {
            StringBuilder sbCsdl = new StringBuilder();
            #region Write the <EntityType> element
            if (this.BaseType != null)
            {
                sbCsdl.AppendFormat(_entityTypeWithBaseTypeTemplate, Name, this.BaseType.Name);
            }
            else
            {
                sbCsdl.AppendFormat(_entityTypeTemplate, Name);
            }

            #endregion Write the <EntityType> element

            #region Write the <Properties>

            foreach (EDMProperty EDMProperty in this.Properties)
            {
                IMetadataItem im = EDMProperty as IMetadataItem;
                sbCsdl.Append(im.ToCSDL());
            }

            #endregion

            #region Write the </EDMEntityType>
            sbCsdl.Append("</ComplexType>");
            #endregion
            return sbCsdl.ToString();
        }

        #endregion
    }
    public class EDMEntityType : IMetadataItem
    {
        public EDMEntityType BaseType { get; set; }
        public string Namespace { get; set; }
        public List<EDMProperty> Properties { get; set; }
        public List<EDMComplexType> ComplexTypeProperties { get; set; }
        public List<EDMAnnotation> ComplexPropertyAnnotations { get; set; }
        private string _entitySetName = "";
        public string Name
        {
            get { return _entitySetName; }
            set { _entitySetName = value; }
        }
        public EDMEntityType(string entitySetName)
        {
            _entitySetName = entitySetName;
        }

        #region IMetadataItem Members
        const string _entityTypeTemplate = "<EntityType Name=\"{0}\">";
        const string _entityTypeWithBaseTypeTemplate = "<EntityType Name=\"{0}\" BaseType=\"{1}.{2}\">";

        public string ToCSDL()
        {
            StringBuilder sbCsdl = new StringBuilder();
            #region Write the <EntityType> element
            if (this.BaseType != null)
            {
                sbCsdl.AppendFormat(_entityTypeWithBaseTypeTemplate, Name, this.BaseType.Namespace, this.BaseType.Name);
            }
            else
            {
                sbCsdl.AppendFormat(_entityTypeTemplate, Name);
            }

            #endregion Write the <EntityType> element

            #region Write the <Properties>

            foreach (EDMProperty EDMProperty in this.Properties)
            {
                IMetadataItem im = EDMProperty as IMetadataItem;
                sbCsdl.Append(im.ToCSDL());
            }

            if (this.ComplexTypeProperties != null)
            {
                foreach (EDMComplexType EDMProperty in this.ComplexTypeProperties)
                {
                    sbCsdl.AppendFormat("<Property Name=\"{0}\" Nullable=\"false\" Type=\"{1}.{2}\" />",
                        EDMProperty.Name, EDMProperty.Namespace, EDMProperty.Name);
                }
            }

            #endregion

            #region Write the </EDMEntityType>
            sbCsdl.Append("</EntityType>");
            #endregion
            return sbCsdl.ToString();
        }

        #endregion
    }

    public static class CSDLUtility
    {
        public static void WriteSchemaStart(string nameSpaceName, StringBuilder sbCsdl)
        {
            sbCsdl.AppendFormat("<Schema Namespace=\"{0}\" Alias=\"Self\" xmlns=\"http://docs.oasis-open.org/odata/ns/edm\" xmlns:c=\"http://christro.test/metadata\" xmlns:m=\"http://docs.oasis-open.org/odata/ns/metadata\">", nameSpaceName);
        }
        public static void WriteSchemaEnd(StringBuilder sbCsdl)
        {
            sbCsdl.Append("</Schema>");
        }
        public static void WriteEntityContainerStart(string entityContainerName, StringBuilder sbCsdl)
        {
            sbCsdl.AppendFormat("<EntityContainer Name=\"{0}\">", entityContainerName);
        }
        public static void WriteEntityContainerEnd(StringBuilder sbCsdl)
        {
            sbCsdl.Append("</EntityContainer>");
        }
        public static void WriteEntitySet(string entitySetName, string entityTypeName, StringBuilder sbCsdl)
        {
            sbCsdl.AppendFormat("<EntitySet Name=\"{0}\" EntityType=\"{1}\" />", entitySetName, entityTypeName);
        }
        public static void WriteEntityType(EDMEntityType EDMEntityType, StringBuilder sbCsdl)
        {
            IMetadataItem im = EDMEntityType as IMetadataItem;
            sbCsdl.Append(im.ToCSDL());
        }
        public static void WriteComplexType(EDMComplexType EDMComplexType, StringBuilder sbCsdl)
        {
            IMetadataItem im = EDMComplexType as IMetadataItem;
            sbCsdl.Append(im.ToCSDL());

        }

    }
}
