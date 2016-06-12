//---------------------------------------------------------------------
// <copyright file="NodeFacets.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // NodeFacet
    //
    ////////////////////////////////////////////////////////   
    public class NodeFacet : Node
    {
        //Data
        protected NodeValue _value;

        //Constructor
        public NodeFacet(String name, NodeValue value)
            : base(name)
        {
            _value = value;
            _desc = "Facet";
        }

        //Accessors
        public virtual NodeValue Value
        {
            get { return _value; }
        }

        public virtual Object ClrValue
        {
            get { return this.Value.ClrValue; }
            set { this.Value.ClrValue = value; }
        }

        //Statics
        public static NodeFacet Nullable(bool value)
        {
            return new NodeFacet(FacetKind.Nullable, Clr.Value(value));
        }
        public static NodeFacet ServerGenerated(bool value)
        {
            return new NodeFacet(FacetKind.ServerGenerated, Clr.Value(value));
        }
        public static NodeFacet MaxSize(int value)
        {
            return new NodeFacet(FacetKind.MaxSize, Clr.Value(value));
        }
        public static NodeFacet Precision(int value)
        {
            return new NodeFacet(FacetKind.Precision, Clr.Value(value));
        }
        public static NodeFacet Scale(int value)
        {
            return new NodeFacet(FacetKind.Scale, Clr.Value(value));
        }
        public static NodeFacet FixedLength(bool isfixedLength)
        {
            return new NodeFacet(FacetKind.FixedLength, Clr.Value(isfixedLength));
        }
        public static NodeFacet AbstractType()
        {
            return new NodeFacet(FacetKind.AbstractType, Clr.Value(true));
        }
        public static NodeFacet Sortable(bool value)
        {
            return new NodeFacet(FacetKind.Sortable, Clr.Value(value));
        }
        public static NodeFacet StoreBlob()
        {
            return IsStoreBlob(true);
        }
        public static NodeFacet IsStoreBlob(bool value)
        {
            return new NodeFacet(FacetKind.IsStoreBlob, Clr.Value(value));
        }
        public static NodeFacet IdentityKey()
        {
            return IsIdentityKey(true);
        }
        public static NodeFacet IsIdentityKey(bool value)
        {
            return new NodeFacet(FacetKind.IsIdentity, Clr.Value(value));
        }
        // Containment-related facets - mmeehan
        public static NodeFacet TopLevelAccess()
        {
            return TopLevelAccess(true);
        }
        public static NodeFacet TopLevelAccess(bool value)
        {
            return new NodeFacet(FacetKind.TopLevelAccess, Clr.Value(value));
        }
        public static NodeFacet CanonicalAccessPath(bool value)
        {
            return new NodeFacet(FacetKind.CanonicalAccessPath, Clr.Value(value));
        }
        public static NodeFacet FriendlyFeeds(string value)
        {
            return new NodeFacet(FacetKind.FriendlyFeeds, Clr.Value(value));
        }

        public static ResourceAttributeFacet Attribute(ResourceAttribute attribute)
        {
            return new ResourceAttributeFacet(attribute);
        }

        public static NodeFacet UnderlyingType(UnderlyingType value)
        {
            NodeValue nodeValue = new NodeValue(value, null);
            return new NodeFacet(FacetKind.UnderlyingType, nodeValue);
        }

        public static NodeFacet IsOpenType()
        {
            return IsOpenType(true);
        }

        public static NodeFacet IsOpenType(bool value)
        {
            return new NodeFacet(FacetKind.IsOpenType, Clr.Value(value));
        }

        public static NodeFacet IsDeclaredProperty(bool value)
        {
            return new NodeFacet(FacetKind.IsDeclaredProperty, Clr.Value(value));
        }

        public static NodeFacet IsClrProperty(bool value)
        {
            return new NodeFacet(FacetKind.IsClrProperty, Clr.Value(value));
        }

        public static NodeFacet IsClrType(bool value)
        {
            return new NodeFacet(FacetKind.IsClrType, Clr.Value(value));
        }

        public static NodeFacet HasStream(bool value)
        {
            return new NodeFacet(FacetKind.HasStream, Clr.Value(value));
        }

        public static NodeFacet UnsafeLinkOperations(params RequestVerb[] operations)
        {
            NodeValue nodeValue = new NodeValue(operations.ToList(), null);
            return new NodeFacet(FacetKind.UnsafeLinkOperations, nodeValue);
        }

        public static NodeFacet MinValue<T>(T value)
        {
            return new NodeFacet(FacetKind.MinValue, Clr.Value<T>(value));
        }

        public static NodeFacet MaxValue<T>(T value)
        {
            return new NodeFacet(FacetKind.MaxValue, Clr.Value<T>(value));
        }


        public static NodeFacet IsForeignKey(bool value)
        {
            return new NodeFacet(FacetKind.IsForeignKey, Clr.Value(value));
        }


        //public static ResourceAttributeFacet AccessPath(ResourceContainer parent, ResourceContainer child)
        //{
        //    return new ResourceAttributeFacet(new ContainmentAttribute(parent, child));
        //}

        //public static ResourceAttributeFacet CanonicalAccessPath(ResourceContainer parent, ResourceContainer child, bool topLevelAccess)
        //{
        //    return new ResourceAttributeFacet(new ContainmentAttribute(parent, child, topLevelAccess));
        //}

        //public static ResourceAttributeFacet ConcurrencyTokens(ResourceType type, IEnumerable<string> propertyNames)
        //{
        //    return new ResourceAttributeFacet(new ConcurrencyAttribute(type, propertyNames));
        //}

        //public static ResourceAttributeFacet ConcurrencyTokens(ResourceType type, params string[] propertyNames)
        //{
        //    return new ResourceAttributeFacet(new ConcurrencyAttribute(type, propertyNames));
        //}

        //public static ResourceAttributeFacet FriendlyFeeds(string entityName, string propertyName, string targetName, bool keepInContent)
        //{
        //    return new ResourceAttributeFacet(new FriendlyFeedsAttribute(entityName, propertyName, targetName, keepInContent));
        //}


        //Overrides
        public override String ToString()
        {
            return this.Name + " = " + this.Value.ToString();
        }

        public override Object Clone()
        {
            NodeFacet clone = (NodeFacet)base.Clone();
            clone._value = (NodeValue)this.Value.Clone();
            return clone;
        }
    }

    ////////////////////////////////////////////////////////
    // FacetKind
    //
    ////////////////////////////////////////////////////////   
    public static class FacetKind
    {
        public const String Nullable = "Nullable";
        public const String MaxSize = "MaxSize";
        public const String ConcurrencyMode = "ConcurrencyMode";
        public const String Precision = "Precision";
        public const String Scale = "Scale";
        public const String FixedLength = "FixedLength";
        public const String ServerGenerated = "ServerGenerated";
        public const String AbstractType = "AbstractType";
        public const String DateTimeMinValue = "DateTimeMinValue";
        public const String DateTimeMaxValue = "DateTimeMaxValue";
        public const String Sortable = "Sortable";
        public const String TopLevelAccess = "TopLevelAccess";
        public const String CanonicalAccessPath = "CanonicalAccessPath";
        public const String FriendlyFeeds = "FriendlyFeeds";
        public const String Attributes = "Attributes";
        public const String IsStoreBlob = "IsStoreBlob";
        public const String IsIdentity = "IsIdentity";
        public const String UnderlyingType = "UnderlyingType";
        public const String IsOpenType = "IsOpenType";
        public const String IsDeclaredProperty = "IsDeclaredProperty";
        public const String MinValue = "MinValue";
        public const String MaxValue = "MaxValue";
        public const String IsClrProperty = "IsClrProperty";
        public const String IsLazyLoaded = "IsLazyLoaded";
        public const String IsClrType = "IsClrType";
        public const String MestTag = "MestTag";
        public const String HasStream = "HasStream";
        public const String NamedStreams = "NamedStreams";
        public const String UnsafeLinkOperations = "UnsafeLinkOperations";
        public const String IsForeignKey = "IsForeignKey";
    };

    ////////////////////////////////////////////////////////
    // NodeFacets
    //
    ////////////////////////////////////////////////////////   
    public class NodeFacets : Nodes<NodeFacet>
    {
        //Constructor
        public NodeFacets(Node parent, IEnumerable<NodeFacet> facets)
            : base(parent, facets)
        {
        }

        //Accessors
        public virtual bool Nullable
        {
            get { return this.Get(FacetKind.Nullable, false); }
            set { this.Set(FacetKind.Nullable, value); }
        }

        public virtual bool FixedLength
        {
            get { return this.Get(FacetKind.FixedLength, false); }
            set { this.Set(FacetKind.FixedLength, value); }
        }

        public virtual bool IsStoreBlob
        {
            get { return this.Get(FacetKind.IsStoreBlob, false); }
            set { this.Set(FacetKind.IsStoreBlob, value); }
        }

        public virtual ConcurrencyMode ConcurrencyMode
        {
            get { return this.Get<ConcurrencyMode>(FacetKind.ConcurrencyMode, ConcurrencyMode.None); }
            set { this.Set<ConcurrencyMode>(FacetKind.ConcurrencyMode, value); }
        }

        public virtual bool ConcurrencyModeFixed
        {
            get { return this.ConcurrencyMode == ConcurrencyMode.Fixed; }
            set { this.ConcurrencyMode = (value ? ConcurrencyMode.Fixed : ConcurrencyMode.None); }
        }

        public virtual int?         MaxSize
        {
            get { return this.Get(FacetKind.MaxSize, (int?)null); }
            set { this.Set(FacetKind.MaxSize, value); }
        }

        public virtual int? Scale
        {
            get { return this.Get(FacetKind.Scale, (int?)null); }
            set { this.Set(FacetKind.Scale, value); }
        }

        public virtual int? Precision
        {
            get { return this.Get(FacetKind.Precision, (int?)null); }
            set { this.Set(FacetKind.Precision, value); }
        }

        public virtual bool ServerGenerated
        {
            get { return this.Get(FacetKind.ServerGenerated, false); }
            set { this.Set(FacetKind.ServerGenerated, value); }
        }

        public virtual bool AbstractType
        {
            get { return this.Get(FacetKind.AbstractType, false); }
            set { this.Set(FacetKind.AbstractType, value); }
        }

        public virtual bool Sortable
        {
            get { return this.Get(FacetKind.Sortable, true); }
            set { this.Set(FacetKind.Sortable, value); }
        }

        // containment functionality - mmeehan
        // by default, top-level unless specified
        public virtual bool TopLevelAccess
        {
            get { return this.Get(FacetKind.TopLevelAccess, true); }
            set { this.Set(FacetKind.TopLevelAccess, value); }
        }

        public virtual bool? CanonicalAccessPath
        {
            get { return this.Get(FacetKind.CanonicalAccessPath, (bool?)null); }
            set { this.Set(FacetKind.CanonicalAccessPath, value); }
        }

        //public virtual string FriendlyFeeds
        //{
        //    get { return this.Get(FacetKind.FriendlyFeeds, string.Empty); }
        //    set { this.Set(FacetKind.FriendlyFeeds, value); }
        //}

        public virtual UnderlyingType UnderlyingType
        {
            get { return this.Get(FacetKind.UnderlyingType, UnderlyingType.Same); }
            set { this.Set(FacetKind.UnderlyingType, value); }
        }

        public virtual bool IsOpenType
        {
            get { return this.Get(FacetKind.IsOpenType, false); }
            set { this.Set(FacetKind.IsOpenType, value); }
        }
        
        public virtual bool IsDeclaredProperty
        {
            get { return this.Get(FacetKind.IsDeclaredProperty, true); }
            set { this.Set(FacetKind.IsDeclaredProperty, value); }
        }

        // For NonClr only
        public virtual bool IsClrProperty
        {
            get { return this.Get(FacetKind.IsClrProperty, false); }
            set { this.Set(FacetKind.IsClrProperty, value); }
        }

        public virtual bool IsLazyLoaded
        {
            get { return this.Get(FacetKind.IsLazyLoaded, false); }
            set { this.Set(FacetKind.IsLazyLoaded, value); }
        }

        public virtual bool IsClrType
        {
            get { return this.Get(FacetKind.IsClrType, false); }
            set { this.Set(FacetKind.IsClrType, value); }
        }

        public virtual string MestTag
        {
            get { return this.Get(FacetKind.MestTag, (string)null); }
            set { this.Set(FacetKind.MestTag, value); }
        }

        public virtual bool IsIdentity
        {
            get { return this.Get(FacetKind.IsIdentity, false); }
            set { this.Set(FacetKind.IsIdentity, value); }
        }

        public virtual bool HasStream
        {
            get { return this.Get(FacetKind.HasStream, false); }
            set { this.Set(FacetKind.HasStream, value); }
        }

        public virtual IList<string> NamedStreams
        {
            get
            {
                List<string> list;
                if (!this.TryGet(FacetKind.NamedStreams, out list))
                {
                    list = new List<string>();
                    this.Add(new NodeFacet(FacetKind.NamedStreams, new NodeValue(list, null)));
                }

                return list;
            }
        }

        public virtual IList<RequestVerb> UnsafeLinkOperations
        {
            get 
            { 
                List<RequestVerb> list;
                if(!this.TryGet(FacetKind.UnsafeLinkOperations, out list))
                {
                    list = new List<RequestVerb>();
                    this.Add(new NodeFacet(FacetKind.UnsafeLinkOperations, new NodeValue(list, null)));
                }
                return list;
            }
        }

        public virtual ResourceAttribute[] Attributes
        {
            get
            {
                return this.OfType<ResourceAttributeFacet>()
                    .Select(f => (f.Value.ClrValue as ResourceAttribute))
                    .ToArray();
            }
        }

        public virtual bool TryGetMaxValue<T>(out T value)
        {
            return TryGet(FacetKind.MaxValue, out value);
        }

        public virtual bool TryGetMinValue<T>(out T value)
        {
            return TryGet(FacetKind.MinValue, out value);
        }

        public virtual bool TryGet<T>(String kind, out T value)
        {
            NodeFacet found = this[kind];
            if (found != null && (found.Value.Type == null || found.Value.Type.ClrType == null || typeof(T).IsAssignableFrom(found.Value.Type.ClrType)))
            {
                value = (T)found.ClrValue;
                return true;
            }

            value = default(T);
            return false;
        }

        public virtual T Get<T>(String kind, T defaultvalue)
        {
            T value;
            if (TryGet(kind, out value))
                return value;
            return defaultvalue;
        }

        public virtual void Set<T>(String kind, T value)
        {
            NodeFacet found = this[kind];
            if (found != null)
            {
                //Update
                found.ClrValue = value;
            }
            else
            {
                //Add (if doesn't exist)
                this.Add(new NodeFacet(kind, Clr.Value(value)));
            }
        }

        public virtual void RemoveAll(Func<NodeFacet,bool> filter)
        {
            List<NodeFacet> toRemove = this.Where(filter).ToList();
            this.Remove(toRemove);
        }
    }
    public enum ConcurrencyMode
    {
        Fixed, None
    }
    public enum UnderlyingType
    {
        Same = 0,
        Xml,
        Image,
        Timestamp
    }
}
