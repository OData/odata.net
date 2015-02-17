//---------------------------------------------------------------------
// <copyright file="Query.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // Query
    //
    ////////////////////////////////////////////////////////   
    public static class Query
    {
        //DML
        public static ProjectExpression Select(this QueryNode input, params ExpNode[] projection)
        {
            return new ProjectExpression(input, projection);
        }

        public static PredicateExpression Where(this QueryNode input, ExpNode predicate)
        {
            return new PredicateExpression(input, predicate);
        }

        public static CountExpression Count(this QueryNode input, bool isInline)
        {
            return Count(input, "allpages", isInline);
        }

        public static CountExpression Count(this QueryNode input, string countOption)
        {
            bool isInline = countOption.Contains("inline");
            return Count(input, countOption, isInline);
        }

        public static CountExpression Count(this QueryNode input, string countValue, bool isInline)
        {
            QueryNode scaninput = input;

            //navigate up the tree to identify the right scan input
            while (!(scaninput is ScanExpression) && !(scaninput is PredicateExpression) && !(scaninput is NavigationExpression) && !(scaninput is OfTypeExpression))
            {
                scaninput = scaninput.Input as QueryNode;
            }

            if (input is ScanExpression || input is PredicateExpression || input is NavigationExpression || input is OfTypeExpression)
                return new CountExpression(input, countValue, input, isInline);
            else
                return new CountExpression(input, countValue, scaninput, isInline);
        }

        public static TopExpression Top(this QueryNode input, int topvalue)
        {
            QueryNode scaninput = input;
            while (!(scaninput is ScanExpression) && !(scaninput is PredicateExpression) && !(scaninput is NavigationExpression) && !(scaninput is OfTypeExpression))
            {
                scaninput = scaninput.Input as QueryNode;
            }

            if (input is ScanExpression || input is PredicateExpression || input is NavigationExpression || input is OfTypeExpression)
                return new TopExpression(input, topvalue, input);
            else
                return new TopExpression(input, topvalue, scaninput);
        }

        public static SkipExpression Skip(this QueryNode input, int skipvalue)
        {
            QueryNode scaninput = input;
            while (!(scaninput is ScanExpression) && !(scaninput is PredicateExpression) && !(scaninput is NavigationExpression))
            {
                scaninput = scaninput.Input as QueryNode;
            }
            if (input is ScanExpression || input is PredicateExpression || input is NavigationExpression)
                return new SkipExpression(input, skipvalue, input);
            else
                return new SkipExpression(input, skipvalue, scaninput);
        }

        public static OrderByExpression Sort(this QueryNode input, ExpNode[] properties, bool bAscDesc)
        {
            return Sort(input, properties, bAscDesc, false);
        }

        public static OrderByExpression Sort(this QueryNode input, ExpNode[] properties, bool bAscDesc, bool excludeFromUri)
        {
            QueryNode scaninput = input;
            while (!(scaninput is ScanExpression) && !(scaninput is PredicateExpression) && !(scaninput is NavigationExpression))
            {
                scaninput = scaninput.Input as QueryNode;
            }
            if (input is ScanExpression || input is PredicateExpression || input is NavigationExpression)
                return new OrderByExpression(input, properties, input, bAscDesc, excludeFromUri);
            else
                return new OrderByExpression(input, properties, scaninput, bAscDesc, excludeFromUri);
        }

        public static ThenByExpression ThenBy(this QueryNode input, PropertyExpression[] properties, bool bAscDesc)
        {
            return ThenBy(input, properties, bAscDesc, false);
        }

        public static ThenByExpression ThenBy(this QueryNode input, PropertyExpression[] properties, bool bAscDesc, bool excludeFromUri)
        {
            QueryNode scaninput = input;
            while (!(scaninput is ScanExpression) && !(scaninput is PredicateExpression) && !(scaninput is NavigationExpression))
            {
                scaninput = scaninput.Input as QueryNode;
            }
            if (input is ScanExpression || input is PredicateExpression || input is NavigationExpression)
                return new ThenByExpression(input, properties, input, bAscDesc, excludeFromUri);
            else
                return new ThenByExpression(input, properties, scaninput, bAscDesc, excludeFromUri);
        }


        public static ScanExpression From(this ExpNode input)
        {
            return new ScanExpression(input);
        }
        public static ServiceOperationExpression ServiceOperation(this ExpNode input, params ServiceOperationParameterExpression[] arguments)
        {
            return new ServiceOperationExpression(input, arguments);
        }

        public static InsertExpression Insert(this Node target, params ExpNode[] arguments)
        {
            return new InsertExpression(target, arguments);
        }

        public static UpdateExpression Update(this Node target, params ExpNode[] arguments)
        {
            return new UpdateExpression(target, arguments);
        }

        public static DeleteExpression Delete(this Node target)
        {
            return new DeleteExpression(target);
        }

        public static ScanExpression CreateQuery(this Node input)
        {
            return Query.From(
                            Exp.Variable(input)
                            );
        }
        public static NewExpression New(this QueryNode input, Type entityType, params ExpNode[] members)
        {
            return new NewExpression(input, members) { EntityType = entityType };
        }
        public static NewExpression New(this QueryNode input, params ExpNode[] members)
        {
            return new NewExpression(input, members);
        }


        public static NavigationExpression Nav(this QueryNode input, PropertyExpression property)
        {
            return new NavigationExpression(input, property);
        }
        public static NavigationExpression Nav(this QueryNode input, PropertyExpression property, bool isLink)
        {
            return new NavigationExpression(input, property, isLink);
        }
        public static ExpandExpression Expand(this QueryNode input, PropertyExpression[] properties)
        {
            //if(!properties.Any())
            return new ExpandExpression(input, properties);

            //ResourceType type = (ResourceType)properties[0].Property.Parent;
            //foreach (PropertyExpression p in properties)
            //{
            //    if (type != p.Property.Parent)
            //        return new ExpandExpression(input, properties);
            //}

            //QueryNode query = Sort(input, type.Key.Properties.Select(p => p.Property()).ToArray(), true, true);
            //return new ExpandExpression(query, properties);
        }

        public static QueryOptionExpression QueryOption(this QueryNode input, string optionName, string value)
        {
            return new QueryOptionExpression(input, optionName, value);
        }

        public static QueryNode ExpandIfApplicable(this QueryNode input, PropertyExpression[] properties, bool applicable)
        {
            return applicable ? Expand(input, properties) : input;
        }
        public static OfTypeExpression OfType(this QueryNode input, ResourceType resourceType)
        {
            return new OfTypeExpression(input, resourceType);
        }

        public static FirstExpression First(this QueryNode input, ResourceType resourceType)
        {
            return new FirstExpression(input, resourceType);
        }

        public static FirstOrDefaultExpression FirstOrDefault(this QueryNode input, ResourceType resourceType)
        {
            return new FirstOrDefaultExpression(input, resourceType);
        }

        public static SingleExpression Single(this QueryNode input, ResourceType resourceType)
        {
            return new SingleExpression(input, resourceType);
        }

        public static SingleOrDefaultExpression SingleOrDefault(this QueryNode input, ResourceType resourceType)
        {
            return new SingleOrDefaultExpression(input, resourceType);
        }
    }

    public static class PropertyBinder
    {
        /// <summary>
        /// This method produces a MemberBindExpression that binds properties of the same entity type
        /// </summary>
        /// <param name="resourceType">The type that contains the properties</param>
        /// <param name="sourcePropertyName">The source property to copy the value from </param>
        /// <param name="targetPropertyName">The target property to copy the value to </param>
        /// <returns></returns>
        public static MemberBindExpression Bind(this ResourceType resourceType, string LeftHandSide, string RightHandSide)
        {
            return BindProperty(resourceType, LeftHandSide, RightHandSide);
        }

        private static MemberBindExpression BindProperty(ResourceType resourceType, string LeftHandSide, string RightHandSide)
        {
            PropertyExpression sourceProperty = new PropertyExpression(resourceType.Properties[RightHandSide] as ResourceProperty);
            TypedMemberExpression targetProperty = new TypedMemberExpression(resourceType.ClientClrType, resourceType.Properties[LeftHandSide].Name);
            return new MemberBindExpression(sourceProperty, targetProperty);
        }
        public static MemberBindExpression Bind(this ResourceType resourceType, string LeftHandSide, ExpNode RightHandSide)
        {
            TypedMemberExpression targetProperty = new TypedMemberExpression(resourceType.ClientClrType, resourceType.Properties[LeftHandSide].Name);
            return new MemberBindExpression(RightHandSide, targetProperty);
        }
    }
    public class MemberBindExpression : ExpNode
    {
        public ExpNode SourceProperty { get; set; }
        public TypedMemberExpression TargetProperty { get; set; }
        public string TargetPropertyName { get; set; }
        public MemberBindExpression(ExpNode sourceProperty, TypedMemberExpression targetProperty)
            : base("MemberBindExpression")
        {
            SourceProperty = sourceProperty;
            TargetProperty = targetProperty;
            //SourceProperty = new PropertyExpression( 
        }
        public MemberBindExpression(TypedMemberExpression targetProperty)
            : base("MemberBindExpression")
        {
            TargetProperty = targetProperty;
            //SourceProperty = new PropertyExpression( 
        }
        //public MemberBindExpression( 
        public override NodeType Type
        {
            get { return null; }
        }
    }

    public class ServiceOperationExpression : QueryNode
    {
        public ServiceOperationParameterExpression[] Arguments { get; set; }
        public ServiceOperationExpression(ExpNode input, ServiceOperationParameterExpression[] arguments)
            : base(input)
        {
            this.Arguments = arguments;
        }

        public override NodeType Type
        {
            get { return null; }
        }
    }
    public class ServiceOperationParameterExpression : ExpNode
    {
        public string ParameterName { get; set; }
        public Type ParameterType { get; set; }
        public object ParameterValue { get; set; }
        public ServiceOperationParameterExpression(string parameterName, Type parameterType, object parameterValue)
            : base("ParameterExpression")
        {
            ParameterName = parameterName;
            ParameterType = parameterType;
            ParameterValue = parameterValue;
        }
        public override NodeType Type
        {
            get { return null; }
        }
    }

    ////////////////////////////////////////////////////////
    // ProjectExpression
    //
    ////////////////////////////////////////////////////////   
    public class ProjectExpression : QueryNode
    {
        //Data
        protected Nodes<ExpNode> _projections;

        //Constructor
        public ProjectExpression(QueryNode input, params ExpNode[] projections)
            : base(input)
        {
            _projections = new Nodes<ExpNode>(this, projections);
        }

        //Accessors
        public override NodeType Type
        {
            //TODO: Resulting type
            get { return null; }
        }

        public virtual Nodes<ExpNode> Projections
        {
            get { return _projections; }
        }
    }

    ////////////////////////////////////////////////////////
    // ScanExpression
    //
    ////////////////////////////////////////////////////////   
    public class ScanExpression : QueryNode
    {
        private NodeType _type;
        //Data

        //Constructor
        public ScanExpression(ExpNode input)
            : base(input)
        {
            _type = input.Type;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _type; }
        }
    }

    ////////////////////////////////////////////////////////
    // PredicateExpression
    //
    ////////////////////////////////////////////////////////   
    public class PredicateExpression : QueryNode
    {
        //Data
        protected ExpNode _predicate;
        private NodeType _type;

        //Constructor
        public PredicateExpression(QueryNode input, ExpNode predicate)
            : base(input)
        {
            _predicate = predicate;
            _type = input.Type;
        }

        //Accessors
        public override NodeType Type
        {
            //get { return Clr.Types.Boolean;                     }
            get { return _type; }
        }

        public virtual ExpNode Predicate
        {
            get { return _predicate; }
        }
    }

    ////////////////////////////////////////////////////////
    // NavigationExpression
    //
    ////////////////////////////////////////////////////////   
    public class NavigationExpression : QueryNode
    {
        private bool _isLink = false;
        PropertyExpression _property;

        public NavigationExpression(QueryNode input, PropertyExpression property)
            : this(input, property, false)
        {
        }
        public NavigationExpression(QueryNode input, PropertyExpression property, bool isLink)
            : base(input)
        {
            _isLink = isLink;
            _property = property;
        }
        public override NodeType Type
        {
            get { return _property.Property.Type; }
        }

        public NodeProperty Property
        {
            get { return _property.Property; }
        }

        public PropertyExpression PropertyExp
        {
            get { return _property; }
        }
        public bool IsLink
        {
            get { return _isLink; }
        }
    }

    ////////////////////////////////////////////////////////
    // NavigationExpression
    //
    ////////////////////////////////////////////////////////   
    public class ExpandExpression : QueryNode
    {
        private PropertyExpression[] _properties;

        public ExpandExpression(QueryNode input, PropertyExpression[] properties)
            : base(input)
        {
            _properties = properties;
        }

        public NodeType[] Types
        {
            get { return _properties.Select(p => p.Property.Type).ToArray(); }
        }

        public override NodeType Type
        {
            get { return null; }
        }

        public virtual NodeProperty[] Properties
        {
            get { return _properties.Select(p => p.Property).ToArray(); }
        }

        public PropertyExpression[] PropertiesExp
        {
            get { return _properties; }
        }
    }

    public class QueryOptionExpression : QueryNode
    {
        private string _optionName;
        private object _value;

        public QueryOptionExpression(QueryNode input, string name, object value)
            : base(input)
        {
            _optionName = name;
            _value = value;
        }

        public override NodeType Type
        {
            get { return null; }
        }

        public string OptionName
        {
            get { return _optionName; }
        }

        public object Value
        {
            get { return _value; }
        }
    }

    ////////////////////////////////////////////////////////
    // CountExpression
    //
    ////////////////////////////////////////////////////////   
    public class CountExpression : QueryNode
    {
        //Data
        protected string _countKind;
        protected QueryNode _scannode;
        private bool _isInline = false;
        private NodeType _type = null;

        //Constructor
        public CountExpression(QueryNode input, string countKind, QueryNode scannode)
            : base(input)
        {
            _countKind = countKind;
            _scannode = scannode;
        }

        public CountExpression(QueryNode input, string countKind, QueryNode scannode, bool isInline)
            : base(input)
        {
            _countKind = countKind;
            _scannode = scannode;
            _isInline = isInline;
            if (isInline)
            {
                _type = input.Type;
            }
        }

        //Accessors
        public override NodeType Type
        {
            get { return _type; }
        }

        public string CountKind
        {
            get { return _countKind; }
        }

        public QueryNode ScanNode
        {
            get { return _scannode; }
        }

        public bool IsInline
        {
            get { return _isInline; }
        }
    }

    ////////////////////////////////////////////////////////
    // TopExpression
    //
    ////////////////////////////////////////////////////////   
    public class TopExpression : QueryNode
    {
        //Data
        protected int _predicate;
        protected QueryNode _scannode;

        //Constructor
        public TopExpression(QueryNode input, int predicate, QueryNode scannode)
            : base(input)
        {
            _predicate = predicate;
            _scannode = scannode;
        }

        //Accessors
        public override NodeType Type
        {
            get { return Clr.Types.Int32; }
        }

        public int Predicate
        {
            get { return _predicate; }
        }

        public QueryNode ScanNode
        {
            get { return _scannode; }
        }
    }

    ////////////////////////////////////////////////////////
    // SkipExpression
    //
    ////////////////////////////////////////////////////////   
    public class SkipExpression : QueryNode
    {
        //Data
        protected int _predicate;
        protected QueryNode _scannode;

        //Constructor
        public SkipExpression(QueryNode input, int predicate, QueryNode scannode)
            : base(input)
        {
            _predicate = predicate;
            _scannode = scannode;
        }

        //Accessors
        public override NodeType Type
        {
            get { return Clr.Types.Int32; }
        }

        public int Predicate
        {
            get { return _predicate; }
        }

        public QueryNode ScanNode
        {
            get { return _scannode; }
        }
    }


    ////////////////////////////////////////////////////////
    // OrderByExpression
    //
    ////////////////////////////////////////////////////////   
    public class OrderByExpression : QueryNode
    {
        private ExpNode[] _properties;
        protected QueryNode _scannode;
        protected bool _bAscDesc = false;
        protected bool _excludeFromUri;

        public OrderByExpression(QueryNode input, ExpNode[] properties, QueryNode scannode, bool bAscDesc)
            : this(input, properties, scannode, bAscDesc, false)
        {
        }

        public OrderByExpression(QueryNode input, ExpNode[] properties, QueryNode scannode, bool bAscDesc, bool excludeFromUri)
            : base(input)
        {
            _properties = properties;
            _scannode = scannode;
            _bAscDesc = bAscDesc;
            _excludeFromUri = excludeFromUri;

            if (excludeFromUri)
                _properties = properties.OrderBy(p => p.Name).ToArray();
        }

        /*public  NodeType[] Types
         {
             get { return _properties.Select(p => p.Property.Type).ToArray(); }
         }*/

        public override NodeType Type
        {
            get { return null; }
        }

        /*public virtual NodeProperty[] Properties
        {
            get { return _properties.Select(p => p.Property).ToArray(); }
        }*/

        public ExpNode[] PropertiesExp
        {
            get { return _properties; }
        }

        public QueryNode ScanNode
        {
            get { return _scannode; }
        }

        public bool AscDesc
        {
            get { return _bAscDesc; }
        }

        public bool ExcludeFromUri
        {
            get { return _excludeFromUri; }
        }
    }

    ////////////////////////////////////////////////////////
    // ThenByExpression
    //
    ////////////////////////////////////////////////////////   
    public class ThenByExpression : QueryNode
    {
        private PropertyExpression[] _properties;
        protected QueryNode _scannode;
        protected bool _bAscDesc = false;
        protected bool _excludeFromUri;

        public ThenByExpression(QueryNode input, PropertyExpression[] properties, QueryNode scannode, bool bAscDesc)
            : this(input, properties, scannode, bAscDesc, false)
        {
        }

        public ThenByExpression(QueryNode input, PropertyExpression[] properties, QueryNode scannode, bool bAscDesc, bool excludeFromUri)
            : base(input)
        {
            _properties = properties;
            _scannode = scannode;
            _bAscDesc = bAscDesc;
            _excludeFromUri = excludeFromUri;

            if (excludeFromUri)
                _properties = properties.OrderBy(p => p.Name).ToArray();
        }

        public NodeType[] Types
        {
            get { return _properties.Select(p => p.Property.Type).ToArray(); }
        }

        public override NodeType Type
        {
            get { return null; }
        }

        public virtual NodeProperty[] Properties
        {
            get { return _properties.Select(p => p.Property).ToArray(); }
        }

        public PropertyExpression[] PropertiesExp
        {
            get { return _properties; }
        }

        public QueryNode ScanNode
        {
            get { return _scannode; }
        }

        public bool AscDesc
        {
            get { return _bAscDesc; }
        }

        public bool ExcludeFromUri
        {
            get { return _excludeFromUri; }
        }
    }

    ////////////////////////////////////////////////////////
    // NewExpression
    //
    ////////////////////////////////////////////////////////   
    public class NewExpression : QueryNode
    {
        //Data
        protected ExpNode _target;
        protected Nodes<ExpNode> _arguments;
        protected NodeType _type;
        public Type EntityType { get; set; }
        //Constructor
        public NewExpression(ExpNode input, params ExpNode[] arguments)
            : base(input)
        {
            _target = input;
            _arguments = new Nodes<ExpNode>(this, arguments);
            //_type = target.Type;
        }

        //Accessors
        public override NodeType Type
        {
            //TODO:
            get { return _type; }
        }

        public virtual Nodes<ExpNode> Arguments
        {
            get { return _arguments; }
        }
    }

    ////////////////////////////////////////////////////////
    // ModifyExpression
    //
    ////////////////////////////////////////////////////////   
    public abstract class ModifyExpression : ExpNode
    {
        //Data
        protected Node _target;
        protected Nodes<ExpNode> _arguments;

        //Constructor
        public ModifyExpression(Node target, params ExpNode[] arguments)
            : base(null)
        {
            _target = target;
            _arguments = new Nodes<ExpNode>(this, arguments);
        }

        //Accessors
        public override NodeType Type
        {
            //TODO:
            get { return null; }
        }

        public virtual Node Target
        {
            get { return _target; }
        }

        public virtual Nodes<ExpNode> Arguments
        {
            get { return _arguments; }
        }
    }

    ////////////////////////////////////////////////////////
    // InsertExpression
    //
    ////////////////////////////////////////////////////////   
    public class InsertExpression : ModifyExpression
    {
        //Constructor
        public InsertExpression(Node target, params ExpNode[] arguments)
            : base(target, arguments)
        {
        }
    }

    ////////////////////////////////////////////////////////
    // UpdateExpression
    //
    ////////////////////////////////////////////////////////   
    public class UpdateExpression : ModifyExpression
    {
        //Constructor
        public UpdateExpression(Node target, params ExpNode[] arguments)
            : base(target, arguments)
        {
        }
    }

    ////////////////////////////////////////////////////////
    // DeleteExpression
    //
    ////////////////////////////////////////////////////////   
    public class DeleteExpression : ModifyExpression
    {
        //Constructor
        public DeleteExpression(Node target, params ExpNode[] arguments)
            : base(target, arguments)
        {
        }
    }

    ////////////////////////////////////////////////////////
    // OfType Expression
    //
    ////////////////////////////////////////////////////////   
    public class OfTypeExpression : QueryNode
    {
        //Data
        protected ResourceType _resourceType;

        //Constructor
        public OfTypeExpression(QueryNode input, ResourceType resourceType)
            : base(input)
        {
            _resourceType = resourceType;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _resourceType; }
        }
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
        }
    }

    ////////////////////////////////////////////////////////
    // First Expression
    //
    ////////////////////////////////////////////////////////   
    public class FirstExpression : QueryNode
    {
        //Data
        protected ResourceType _resourceType;

        //Constructor
        public FirstExpression(QueryNode input, ResourceType resourceType)
            : base(input)
        {
            _resourceType = resourceType;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _resourceType; }
        }
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
        }
    }

    ////////////////////////////////////////////////////////
    // FirstOrDefault Expression
    //
    ////////////////////////////////////////////////////////   
    public class FirstOrDefaultExpression : QueryNode
    {
        //Data
        protected ResourceType _resourceType;

        //Constructor
        public FirstOrDefaultExpression(QueryNode input, ResourceType resourceType)
            : base(input)
        {
            _resourceType = resourceType;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _resourceType; }
        }
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
        }
    }

    ////////////////////////////////////////////////////////
    // Single Expression
    //
    ////////////////////////////////////////////////////////   
    public class SingleExpression : QueryNode
    {
        //Data
        protected ResourceType _resourceType;

        //Constructor
        public SingleExpression(QueryNode input, ResourceType resourceType)
            : base(input)
        {
            _resourceType = resourceType;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _resourceType; }
        }
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
        }
    }

    ////////////////////////////////////////////////////////
    // SingleOrDefault Expression
    //
    ////////////////////////////////////////////////////////   
    public class SingleOrDefaultExpression : QueryNode
    {
        //Data
        protected ResourceType _resourceType;

        //Constructor
        public SingleOrDefaultExpression(QueryNode input, ResourceType resourceType)
            : base(input)
        {
            _resourceType = resourceType;
        }

        //Accessors
        public override NodeType Type
        {
            get { return _resourceType; }
        }
        public virtual ResourceType ResourceType
        {
            get { return _resourceType; }
        }
    }


}

