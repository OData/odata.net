//---------------------------------------------------------------------
// <copyright file="QueryModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Linq;
using System.Data.SqlClient;
using Microsoft.Test.KoKoMo;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.ModuleCore;

namespace System.Data.Test.Astoria
    {
    ////////////////////////////////////////////////////////
    //  Query Model
    //
    ////////////////////////////////////////////////////////   
    public class QueryModel : ExpressionModel
        {
        public enum LastAction
            {
            Init,
            From,
            Where,
            Select,
            Nav,
            OrderBy,
            Top,
            Skip,
            Count,
            Expand
            }

        public enum isValid
            {
            Yes,
            No
            }

        protected ExpNode _query = null;
        protected Workspace _workspace = null;
        protected ResourceType from = null;
        protected ResourceContainer _fromContainer = null;
        protected ResourceProperty property = null;
        protected PredicateExpression where = null;
        protected SerializationFormatKind _kind;
        protected bool bWhere = false;
        protected bool bSelect = false;
        protected bool bTop = false;
        protected bool bSkip = false;
        protected bool bSort = false;
        protected bool bNav = false;
        protected bool bExpand = false;
        protected bool bIsValid = false;
        protected bool bIsOption = false;
        protected int iRows = 0;
        protected int iTop = 0;
        protected string sLast = String.Empty;
        protected Type _pType = null;
        protected KeyExpression _parentKey = null;
        protected bool bFilter = false;

        //Constructor
        public QueryModel(Workspace workspace, SerializationFormatKind kind, QueryModel parent)
            : base( parent )
            {
            _workspace = workspace;
            _kind = kind;
            _parent = parent;
            }

        [ModelVariable]
        public bool IsCount { get; set; }

        [ModelVariable]
        public bool IsKey { get; set; }

        [ModelVariable]
        public bool RunningUnderScenarioModel { get; set; }

        [ModelVariable]
        public ExpNode QueryResult
            {
            get { return _query; }
            set
                {
                _query = value;
                }
            }

        public Workspace Workspace
            {
            get { return _workspace; }
            }

        [ModelVariable]
        public Type ResultType
            {
            get { return _pType; }
            }

        public SerializationFormatKind SerializationKind
            {
            get { return _kind; }
            }
        [ModelVariable]
        public KeyExpression ParentKey
            {
            get { return _parentKey; }
            set { _parentKey = value; }
            }

        [ModelVariable]
        public ResourceContainer ResContainer
            {
            get { return _fromContainer; }
            set
                {
                _fromContainer = value;
                from = _fromContainer.ResourceTypes.First();
                }
            }
        [ModelVariable]
        public ResourceType ResType
            {
            get { return from; }
            set { from = value; }
            }

        [ModelVariable]
        public KeyExpression ParentRelKey
            {
            get { return _parentKey; }
            set { _parentKey = value; }
            }


        // disable compiler warning 'The field is assigned but its value is never used'
        // since the fields are used by the model via reflection.
#pragma warning disable 414
        [ModelVariable]
        LastAction _action = LastAction.Init;

        [ModelVariable]
        isValid _isValid = isValid.No;
#pragma warning restore 414

        public int GetRowsNo(ResourceType resourceType)
            {
            string selectQuery = @"Select Count(*) from " + resourceType.Name;

            SqlConnection conn = new SqlConnection( ((EdmWorkspace)this.Workspace).Database.DatabaseConnectionString );
            conn.Open();
            SqlCommand cmd = new SqlCommand( selectQuery, conn );

            SqlDataReader myReader = cmd.ExecuteReader();
            try
                {
                while (myReader.Read())
                    {
                    iRows = myReader.GetInt32( 0 );
                    }
                }
            finally
                {
                myReader.Close();
                conn.Close();
                }

            return iRows;

            }

        private static int NextTopValue() { return AstoriaTestProperties.Random.Next( 1, 16 ); /* >=1 && < 16*/} // 10+ for 2 digits, top=0 is negative case
        private static int NextSkipValue() { return AstoriaTestProperties.Random.Next( 0, 16 ); /* >= 0 && < 16*/ } // skip=0 is a positive case

        //Overrides
        public override ExpressionModel CreateModel()
            {
            return new QueryModel( this.Workspace, this._kind, this );
            }

        //Actions
        [ModelAction( CallFirst = true, CallOnce = true )]
        public virtual void From()
            {
            if (RunningUnderScenarioModel)
                {
                _query = Query.From( Exp.Variable( ResContainer ) );
                _pType = ResContainer.BaseType.ClientClrType;
                AstoriaTestLog.WriteLineIgnore( "Query.From(" + ResContainer.Name + ")" );
                }
            else
                {
                ScanModel model = new ScanModel( _workspace );
                ModelEngine engine = new ModelEngine( this.Engine, model );
                engine.Run();

                this.ResContainer = model.ResultContainer;
                _query = Query.From( Exp.Variable( model.ResultContainer ) );
                _pType = model.ResultContainer.BaseType.ClientClrType;
                AstoriaTestLog.WriteLineIgnore( "Query.From(" + model.ResultContainer.Name + ")" );
                }


            _action = LastAction.From;
            IsCount = false;

            }

        [ModelAction( Weight = 1, CallLast = true )]
        [ModelRequirement(Variable = "bIsValid", Not = false)]
        public virtual void Select()
            {
           
            PropertyExpression ordervalues = new PropertyExpression( this.ResType.Properties.Choose() );

            if (_query is ScanExpression)
                _query = ((ScanExpression)_query).Select() as ProjectExpression;
            if (_query is NavigationExpression)
                _query = ((NavigationExpression)_query).Select() as ProjectExpression;
            if (_query is TopExpression)
                _query = ((TopExpression)_query).Select() as ProjectExpression;
            if (_query is SkipExpression)
                _query = ((SkipExpression)_query).Select() as ProjectExpression;
            if (_query is CountExpression)
                _query = ((CountExpression)_query).Select() as ProjectExpression;
            if (_query is ExpandExpression)
                _query = ((ExpandExpression)_query).Select() as ProjectExpression;

            if (_query is PredicateExpression)
                {
                int i = this.Engine.Options.Random.Next( 0, 2 );

                if (bFilter) { i = 0; }

                switch (i)
                    {
                    case 0:
                        _query = ((PredicateExpression)_query).Select() as ProjectExpression;
                        break;
                    case 1:

                        if (ordervalues.Type is CollectionType)
                            {
                            this.ResType = (ResourceType)((ResourceCollection)ordervalues.Type).SubType;
                            _pType = this.ResType.ClientClrType;
                            }
                        else if (ordervalues.Type is ResourceType)
                            {
                            this.ResType = (ResourceType)ordervalues.Type;
                            _pType = ordervalues.Type.ClrType;
                            }
                        else
                            _pType = ordervalues.Type.ClrType;

                        _query = ((PredicateExpression)_query).Select( ordervalues ) as ProjectExpression;

                        break;
                    }

                }

            if (_query != null)
                {
                bSelect = true;
                _action = LastAction.Select;
                _isValid = isValid.Yes;
                AstoriaTestLog.WriteLineIgnore( ".Select()" );
                this.Disabled = true;
                }
            }

        [ModelAction( Weight = 1000)]
        [ModelRequirement( Variable = "ResType", Not = null )]
        //[ModelRequirement(Variable = "_action", Value = new object[] { LastAction.From, LastAction.Nav})]
        [ModelRequirement( Variable = "_action", Not = LastAction.Where )]
        [ModelRequirement( Variable = "bFilter", Not = true )]
        [ModelRequirement(Variable = "bIsOption", Not = true)]
        public virtual void Where()
            {

                AstoriaTestLog.WriteLineIgnore("Calling Where");

            //Sub model - projections
            PredicateModel model = new PredicateModel( this.Workspace, this.ResContainer, this.property, this.ParentRelKey, this.ResType );
            ModelEngine engine = new ModelEngine( this.Engine, model );
            engine.Run();

            ExpNode e = model.Result;

            this.ParentRelKey = e as KeyExpression;
            if (null == _parentKey)
                {
                /* no keys for resource type*/
                this.Reload();
                return;
                }

            int i = this.Engine.Options.Random.Next( 0, 10 );
            if (i % 7 == 0)
                {
                e = ((KeyExpression)e).Predicate;
                bFilter = true;
                }

            if (e != null)
                {
                if (_query is ScanExpression)
                    _query = ((ScanExpression)_query).Where( e ) as PredicateExpression;
                else if (_query is NavigationExpression)
                    _query = ((NavigationExpression)_query).Where( e ) as PredicateExpression;
                
                bWhere = true;
                IsKey = true;
                _action = LastAction.Where;
                AstoriaTestLog.WriteLineIgnore( ".Where()" );
                }
            }

        [ModelAction( Weight = 1, CallOnce = true )]
        [ModelRequirement( Variable = "IsKey", Not = true )]
        [ModelRequirement( Variable = "bTop", Not = true )]
        public void Top()
            {
            int i = NextTopValue();
          
            if (_query is OrderByExpression)
                _query = ((OrderByExpression)_query).Top( i ) as TopExpression;
            else if (_query is CountExpression)
                _query = ((CountExpression)_query).Top(i) as TopExpression;
            else if (_query is ExpandExpression)
                _query = ((ExpandExpression)_query).Top(i) as TopExpression;
            else if (_query is NavigationExpression)
                _query = ((NavigationExpression)_query).Top(i) as TopExpression;
            else if (_query is SkipExpression)
                _query = ((SkipExpression)_query).Top(i) as TopExpression;
            else if (_query is PredicateExpression)
                _query = ((PredicateExpression)_query).Top(i) as TopExpression;
            else if (_query is ScanExpression)
                _query = ((ScanExpression)_query).Top(i) as TopExpression;

            iTop = i;
            bTop = true;
            _action = LastAction.Top;
            bIsOption = true;
            AstoriaTestLog.WriteLineIgnore( ".Top(" + i.ToString() + ")" );
            }

        [ModelAction( Weight = 1, CallOnce = true )]
        [ModelRequirement( Variable = "bSkip", Not = true )]
        [ModelRequirement(Variable = "_action", Value = LastAction.OrderBy)]
        public void Skip()
            {
            int i = NextSkipValue();
            if (_query is OrderByExpression)
                _query = ((OrderByExpression)_query).Skip( i ) as SkipExpression;
         
            bSkip = true;
            _action = LastAction.Skip;
            bIsOption = true;
            AstoriaTestLog.WriteLineIgnore(".Skip(" + i.ToString() + ")");
            }

        [ModelAction(Weight = 1)]
        [ModelRequirement(Variable = "IsCount", Not = true)]
        [ModelRequirement(Variable = "IsKey", Not = true)]
        public void Count()
        {
            if (_query is TopExpression)
                _query = ((TopExpression)_query).Count(true) as CountExpression;
            else if (_query is OrderByExpression)
                _query = ((OrderByExpression)_query).Count(true) as CountExpression;
            else if (_query is ScanExpression)
                _query = ((ScanExpression)_query).Count(true) as CountExpression;
            else if (_query is NavigationExpression)
                _query = ((NavigationExpression)_query).Count(true) as CountExpression;
            else if (_query is SkipExpression)
                _query = ((SkipExpression)_query).Count(true) as CountExpression;
            else if (_query is NavigationExpression)
                _query = ((NavigationExpression)_query).Count(true) as CountExpression;
            else if (_query is ExpandExpression)
                _query = ((ExpandExpression)_query).Count(true) as CountExpression;
            else if (_query is PredicateExpression)
                _query = ((PredicateExpression)_query).Count(true) as CountExpression;


            IsCount = true;
            _action = LastAction.Count;
            bIsOption = true;
            AstoriaTestLog.WriteLineIgnore(".Count");
        }

        [ModelAction(Weight = 1, CallOnce = true)]
        [ModelRequirement(Variable = "bSort", Not = true)]
        [ModelRequirement(Variable = "IsKey", Not = true)]
        [ModelParameter(Type = typeof(bool))]
        public void Sort(bool bAsc)
        {
            //Sub model - sort
            SortModel model = new SortModel(this._workspace, _fromContainer.BaseType);
            ModelEngine engine = new ModelEngine(this.Engine, model);
            engine.Run();

            PropertyExpression[] ordervalues = model.SortResult;

           if (_query is TopExpression)
                _query = ((TopExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
            else if (_query is SkipExpression)
                _query = ((SkipExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
            else if (_query is CountExpression)
                _query = ((CountExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
            else if (_query is ExpandExpression)
                _query = ((ExpandExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
            else if (_query is NavigationExpression)
                _query = ((NavigationExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
           else if (_query is PredicateExpression)
               _query = ((PredicateExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;
           else if (_query is ScanExpression)
               _query = ((ScanExpression)_query).Sort(ordervalues, bAsc) as OrderByExpression;

            bSort = true;
            _action = LastAction.OrderBy;
            bIsOption = true;
            AstoriaTestLog.WriteLineIgnore(".OrderBy(" + ordervalues.ToString() + ")");
        }

        public static PropertyExpression[] GetSortPropertyExpression(ResourceType resourceType)
            {
            List<PropertyExpression> anyProperties = new List<PropertyExpression>();
            int iCount = resourceType.Properties.OfType<ResourceProperty>().Where( p => p.IsNavigation && !p.IsComplexType && p.Facets.Sortable ).Count();

            int i = AstoriaTestProperties.Random.Next( 1, iCount );
            int j = 0;
            foreach (ResourceProperty resourceProperty in resourceType.Properties)
                {
                if (!resourceProperty.IsNavigation && !resourceProperty.IsComplexType && resourceProperty.Facets.Sortable)
                    {
                    anyProperties.Add( new PropertyExpression( resourceProperty ) );
                    j++;
                    }
                if (j > i) { break; }
                }

            return anyProperties.ToArray<PropertyExpression>();
            }


        [ModelAction( Weight = 100 )]
        [ModelRequirement( Variable = "_action", Value = LastAction.Where )]
        [ModelRequirement( Variable = "bNav", Not = true )]
        [ModelRequirement( Variable = "bFilter", Not = true )]
        public void Navigation()
            {
            bool bfound = false;
            int j = 0;
            ResourceType _navType = null;

            while (!bfound && j < from.Properties.Count)
                {
                int i = this.Engine.Options.Random.Next( 0, from.Properties.Count );

                ResourceProperty _property = (ResourceProperty)from.Properties[i];
                if (_property.IsNavigation && _property.Type is CollectionType && this.ResContainer.BaseType.Properties.Contains( _property ))
                    {
                    if (_query is PredicateExpression)
                        {
                        _query = ((PredicateExpression)_query).Nav( _property.Property() ) as NavigationExpression;
                        if (_property.Type is CollectionType)
                            {
                            _navType = (ResourceType)((ResourceCollection)_property.Type).SubType;
                            _pType = _navType.ClientClrType;
                            }
                        else if (_property.Type is ResourceType)
                            {
                            _navType = (ResourceType)_property.Type;
                            _pType = _property.Type.ClrType;
                            }

                        this.ResType = _navType;

                        bNav = true;
                        bfound = true;
                        _action = LastAction.Nav;
                        property = _property;
                        this.ResType = _navType; //update the resourceType

                        AstoriaTestLog.WriteLineIgnore( ".Nav(" + _property.Property().Name + ")" );
                        IsKey = false;
                        }
                    }

                j++;
                }
            }
        [ModelAction(Weight = 10, CallOnce = true)]
        [ModelRequirement(Variable = "bExpand", Not = true)]
        public void Expand()
        {
            string sExpand = String.Empty;
            PropertyExpression[] expandValues = new PropertyExpression[1];

            List<ResourceProperty> properties = new List<ResourceProperty>();
            foreach (ResourceProperty property in from.Properties)
            {
                if (property.IsNavigation && from.Properties.Contains(property) &&
                        !(from.Name == "BugDefectTrackingSet" || from.Name == "BugProjectTrackingSet"))
                    properties.Add(property);
            }

            if (properties.Count > 0)
            {
                expandValues[0] = new PropertyExpression(properties[0]);
                sExpand = expandValues[0].ToString();

                if (_query is TopExpression)
                    _query = ((TopExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is OrderByExpression)
                    _query = ((OrderByExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is ScanExpression)
                    _query = ((ScanExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is NavigationExpression)
                    _query = ((NavigationExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is SkipExpression)
                    _query = ((SkipExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is PredicateExpression)
                    _query = ((PredicateExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is CountExpression)
                    _query = ((CountExpression)_query).Expand(expandValues) as ExpandExpression;
                else if (_query is NavigationExpression)
                    _query = ((NavigationExpression)_query).Expand(expandValues) as ExpandExpression;
              
                bExpand = true;
                _action = LastAction.Expand;
                bIsOption = true;
                AstoriaTestLog.WriteLineIgnore(".Expand(" + sExpand + ")");
            }
        }

        }
    }
