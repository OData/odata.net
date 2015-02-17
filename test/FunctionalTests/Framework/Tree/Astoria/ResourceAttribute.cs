//---------------------------------------------------------------------
// <copyright file="ResourceAttribute.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Collections;               //IEnumerator
using System.Data.Test.Astoria.ReflectionProvider;
using System.Xml;

namespace System.Data.Test.Astoria
{
    public abstract class ResourceAttribute : Node
    {
        private static List<ResourceAttribute> allAttributes = new List<ResourceAttribute>();
        public static IEnumerable<ResourceAttribute> GetAllAttributes()
        {
            return allAttributes.AsEnumerable();
        }

        protected List<string> orderedParams;
        protected Dictionary<string,string> namedParams;

        protected ResourceAttribute(string name)
            : base(name)
        {
            orderedParams = new List<string>();
            namedParams = new Dictionary<string, string>();
            allAttributes.Add(this);
        }

        // Apply as code
        public virtual void Apply(CSharpCodeLanguageHelper codeHelper)
        {
            codeHelper.WriteAttribute(this.Name, namedParams, orderedParams.ToArray());
        }

        // for IDSQ/MP
        public virtual string InstantiationCode
        {
            get
            {
                return "new " + this.Name + "(" + string.Join(",", this.orderedParams.ToArray()) + ")";
            }
        }

        // Apply as csdl
        public abstract void Apply(XmlDocument csdl);
    }

    public class ResourceAttributeFacet : NodeFacet
    {
        public ResourceAttributeFacet(ResourceAttribute att)
            : base(FacetKind.Attributes, new NodeValue(att, null))
        { }
    }

    ////////////////////////////////////////////////////////
    // Specific feature attributes
    // TODO: seperate file? in with the feature-specific tests?
    //       it might get annoying to keep them all here
    //
    ////////////////////////////////////////////////////////  
    //public class FriendlyFeedsAttribute : ResourceAttribute
    //{
    //    public string EntityName
    //    {
    //        get;
    //        private set;
    //    }
    //    public string PropertyName
    //    {
    //        get;
    //        private set;
    //    }
    //    public string TargetName
    //    {
    //        get;
    //        private set;
    //    }
    //    public bool KeepInContent
    //    {
    //        get;
    //        private set;
    //    }
    //    public FriendlyFeedsAttribute(string entityName, string propertyName, string targetName, bool keepInContent)
    //        : base("FriendlyFeeds")
    //    {
    //        this.EntityName = entityName;
    //        this.PropertyName = propertyName;
    //        this.TargetName = targetName;
    //        this.KeepInContent = keepInContent;
    //    }

    //    public override void Apply(XmlDocument csdl)
    //    {
    //        string searchString = null;

    //        searchString = String.Format("//csdl:EntityType[@Name='{0}']", this.EntityName);
    //        searchString = String.Concat(searchString, String.Format("//csdl:Property[@Name='{0}']", this.PropertyName));

    //        System.Xml.XmlNode propNode = TestUtil.AssertSelectSingleElement(csdl, searchString);
    //        propNode.ToString();

    //        return;
    //    }
    //}

    // This class is not working at the moment, please do not remove it or uncomment it
    //// these will be inferred from the service container where possible
    //public class PrimaryKeyAttribute : ResourceAttribute
    //{
    //    private List<string> keyPropertyNames;
    //    private bool inferredFromType;
    //    private ResourceType type;

    //    private PrimaryKeyAttribute(ResourceType type, bool inferred, IEnumerable<string> propertyNames)
    //        : base("DataServiceKey")
    //    {
    //        this.inferredFromType = inferred;
    //        this.type = type;
    //        this.keyPropertyNames = propertyNames.ToList();

    //        if (keyPropertyNames.Count() > 1)
    //        {
    //            string[] orignalOrder = keyPropertyNames.ToArray();
    //            for (int i = 1; i < orignalOrder.Length; i++)
    //                if (String.Compare(orignalOrder[i - 1], orignalOrder[i], false, Globalization.CultureInfo.InvariantCulture) > 0)
    //                {
    //                    AstoriaTestLog.WriteLine("Warning: Compound key properties on type '{0}' are not in alphabetical order, this may cause problems", type.Name);
    //                    break;
    //                }
    //        }

    //        this.orderedParams = keyPropertyNames.Select(n => "\"" + n + "\"").ToList();
    //    }

    //    public PrimaryKeyAttribute(ResourceType type)
    //        : this(type, true, type.Key.Properties.Select(p => p.Name))
    //    { }

    //    // This might not work, provided for completeness
    //    public PrimaryKeyAttribute(ResourceType type, IEnumerable<string> names)
    //        : this(type, false, names)
    //    {
    //        type.Key = new PrimaryKey("PK_" + type.Name, type.Properties.Where(p => names.Contains(p.Name)).ToArray());
    //    }

    //    // This might not work, provided for completeness
    //    public PrimaryKeyAttribute(ResourceType type, params string[] names)
    //        : this(type, names.AsEnumerable())
    //    { }

    //    // This might not work, provided for completeness
    //    public override void Apply(XmlDocument csdl)
    //    {
    //        if (!inferredFromType)
    //        {
    //            XmlElement keyNode = TestUtil.AssertSelectSingleElement(csdl, String.Format("//csdl:EntityType[@Name='{0}']/csdl:Key"));

    //            IEnumerable<string> present = keyNode.ChildNodes.OfType<XmlElement>().Select(x => x.GetAttribute("Name"));
    //            IEnumerable<string> toAdd = keyPropertyNames.Except(present);
    //            IEnumerable<string> toRemove = present.Except(keyPropertyNames);

    //            foreach (string remove in toRemove)
    //                keyNode.RemoveChild(TestUtil.AssertSelectSingleElement(keyNode, String.Format("//csdl:PropertyRef[@Name='{0}']", remove)));

    //            foreach (string add in toAdd)
    //            {
    //                XmlElement propRef = csdl.CreateElement("csdl:PropertyRef");
    //                propRef.SetAttribute("Name", add);
    //                keyNode.AppendChild(propRef);
    //            }
    //        }
    //    }
    //}
}
