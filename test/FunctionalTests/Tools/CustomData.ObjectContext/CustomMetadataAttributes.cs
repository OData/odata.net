//---------------------------------------------------------------------
// <copyright file="CustomMetadataAttributes.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria.CustomData.Metadata
{
    // --- CODE SNIPPET START ---

    //
    // CustomComplexType class
    //
    #region CustomComplexTypeAttribute class

    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class CustomComplexTypeAttribute : Attribute
    {
        #region Constructors

        public CustomComplexTypeAttribute()
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomTargetEntitySetAttribute class
    //
    #region CustomTargetEntitySetAttribute class

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class CustomTargetEntitySetAttribute : Attribute
    {
        #region Private fields

        private readonly ICollection<string> _entitySetNames;

        #endregion

        #region Constructors

        public CustomTargetEntitySetAttribute(string entitySetName)
            : this(new string[] { entitySetName })
        {
        }

        public CustomTargetEntitySetAttribute(params string[] entitySetNames)
        {
            _entitySetNames = new List<string>(entitySetNames).AsReadOnly();
        }

        #endregion

        #region Properties

        public ICollection<string> EntitySetNames
        {
            get { return _entitySetNames; }
        }

        #endregion

    }

    #endregion

    //
    // CustomMemberAttribute class
    //
    #region CustomMemberAttribute class

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public abstract class CustomMemberAttribute : Attribute
    {
        #region Private fields

        private bool _isNullable = false;

        private bool _isReadOnly = false;

        #endregion

        #region Constructors

        public CustomMemberAttribute()
        {
        }

        #endregion

        #region Properties

        public bool IsNullable
        {
            set { _isNullable = value; }
            get { return _isNullable; }
        }

        public bool IsReadOnly
        {
            set { _isReadOnly = value; }
            get { return _isReadOnly; }
        }

        #endregion
    }

    #endregion

    //
    // CustomPropertyAttribute class
    //
    #region CustomPropertyAttribute class

    public class CustomPropertyAttribute : CustomMemberAttribute
    {
        #region Private fields

        private bool _isStoreGenerated = false;

        private int? _maximumLength = null;

        #endregion

        #region Constructors

        public CustomPropertyAttribute()
        {
        }


        #endregion

        #region Properties

        public bool IsStoreGenerated
        {
            set { _isStoreGenerated = value; }
            get { return _isStoreGenerated; }
        }

        public int MaximumLength
        {
            set { _maximumLength = value; }
            get { return _maximumLength.Value; }
        }

        internal bool HasMaximumLength
        {
            get { return _maximumLength.HasValue; }
        }

        #endregion
    }

    #endregion

    //
    // CustomNavigationPropertyAttribute class
    //
    #region CustomNavigationPropertyAttribute class

    public abstract class CustomNavigationPropertyAttribute : CustomMemberAttribute
    {
        #region Private fields

        private bool _isPrimaryEnd;

        private string _entitySetName;

        private CustomMultiplicity _multiplicity;        

        private Type _relatedEndEntityType;

        private CustomMultiplicity _relatedEndMultiplicity;

        private string _relatedEndEntitySetName;

        private string _relatedEndPropertyName;

        #endregion

        #region Constructors

        public CustomNavigationPropertyAttribute(
                                bool isPrimaryEnd,
                                string entitySetName,
                                CustomMultiplicity multiplicity,
                                string relatedEndEntitySetName,
                                Type relatedEndEntityType,                                
                                CustomMultiplicity relatedEndMultiplicity
                        )
        {
            _isPrimaryEnd = isPrimaryEnd;
            _entitySetName = entitySetName;
            _multiplicity = multiplicity;
            _relatedEndEntitySetName = relatedEndEntitySetName;
            _relatedEndEntityType = relatedEndEntityType;            
            _relatedEndMultiplicity = relatedEndMultiplicity;
        }

        #endregion

        #region Properties

        public bool IsPrimaryEnd
        {
            get { return _isPrimaryEnd; }
            set { _isPrimaryEnd = value; }
        }

        public string EntitySetName
        {
            get { return _entitySetName; }
        }

        public CustomMultiplicity Multiplicity
        {
            get { return _multiplicity; }
        }

        public string RelatedEndEntitySetName
        {
            get { return _relatedEndEntitySetName; }
        }

        public Type RelatedEndEntityType
        {
            get { return _relatedEndEntityType; }
        }

        public CustomMultiplicity RelatedEndMultiplicity
        {
            get { return _relatedEndMultiplicity; }
        }

        public string RelatedEndPropertyName
        {
            get { return _relatedEndPropertyName; }
            set { _relatedEndPropertyName = value; }
        }

        #endregion
    }

    #endregion

    //
    // CustomOneToManyPropertyAttribute class
    //
    #region CustomOneToManyPropertyAttribute class

    public sealed class CustomOneToManyPropertyAttribute : CustomNavigationPropertyAttribute
    {
        #region Constructors

        public CustomOneToManyPropertyAttribute()
            : this(null, null, null)
        {
        }

        public CustomOneToManyPropertyAttribute(Type relatedEndEntityType)
            : this(null, relatedEndEntityType, null)
        {
        }

        public CustomOneToManyPropertyAttribute(
                        string entitySetName,                        
                        Type relatedEndEntityType,                      
                        string relatedEndEntitySetName
                    )
            : base(true, entitySetName, CustomMultiplicity.One, relatedEndEntitySetName,
                                                    relatedEndEntityType, CustomMultiplicity.Many)
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomManyToManyPropertyAttribute class
    //
    #region CustomManyToManyPropertyAttribute class

    public sealed class CustomManyToManyPropertyAttribute : CustomNavigationPropertyAttribute
    {
        #region Constructors

        public CustomManyToManyPropertyAttribute()
            : this(null, null, null)
        {
        }

        public CustomManyToManyPropertyAttribute(Type relatedEndEntityType)
            : this(null, relatedEndEntityType, null)
        {
        }

        public CustomManyToManyPropertyAttribute(
                        string entitySetName,
                        Type relatedEndEntityType,
                        string relatedEndEntitySetName
                    )
            : base(false, entitySetName, CustomMultiplicity.Many, relatedEndEntitySetName,
                            relatedEndEntityType, CustomMultiplicity.Many)
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomManyToOnePropertyAttribute class
    //
    #region CustomManyToOnePropertyAttribute class

    public sealed class CustomManyToOnePropertyAttribute : CustomNavigationPropertyAttribute
    {
        #region Constructors

        public CustomManyToOnePropertyAttribute()
            : this(null, null, null)
        {
        }

        public CustomManyToOnePropertyAttribute(Type relatedEndEntityType)
            : this(null, relatedEndEntityType, null)
        {
        }

        public CustomManyToOnePropertyAttribute(
                        string entitySetName,
                        Type relatedEndEntityType,
                        string relatedEndEntitySetName
                    )
            : base(false, entitySetName, CustomMultiplicity.Many, relatedEndEntitySetName,
                            relatedEndEntityType, CustomMultiplicity.One)
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomOneToOnePropertyAttribute class
    //
    #region CustomOneToOnePropertyAttribute class

    public sealed class CustomOneToOnePropertyAttribute : CustomNavigationPropertyAttribute
    {
        #region Constructors

        public CustomOneToOnePropertyAttribute()
            : this(null)
        {
        }

        public CustomOneToOnePropertyAttribute(Type relatedEndEntityType)
            : this(null, relatedEndEntityType, null)
        {
        }

        public CustomOneToOnePropertyAttribute(
                        string entitySetName,
                        Type relatedEndEntityType,
                        string relatedEndEntitySetName
                    )
            : base(false, entitySetName, CustomMultiplicity.One, relatedEndEntitySetName,
                                                        relatedEndEntityType, CustomMultiplicity.One)
        {
        }

        #endregion
    }

    #endregion

    //
    // CustomForeignKeyConstraintAttribute class
    //
    #region CustomForeignKeyConstraintAttribute class

    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class CustomForeignKeyConstraintAttribute : Attribute
    {
        #region Private fields
        
        private Type _targetEntityType;

        private string _targetKeyPropertyName; 

        #endregion

        #region Constructors

        public CustomForeignKeyConstraintAttribute(
                            Type targetEntityType,
                            string targetKeyPropertyName
                        )
        {
            _targetEntityType = targetEntityType;
            _targetKeyPropertyName = targetKeyPropertyName;
        }

        #endregion

        #region Properties

        public Type TargetEntityType
        {
            get { return _targetEntityType; }
        }

        public string TargetKeyPropertyName
        {
            get { return _targetKeyPropertyName; }
        }

        #endregion
    }

    #endregion

    // --- CODE SNIPPET END ---
}
