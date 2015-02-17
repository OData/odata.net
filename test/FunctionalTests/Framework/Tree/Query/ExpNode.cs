//---------------------------------------------------------------------
// <copyright file="ExpNode.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using System.Collections.Generic;       //IEnumerable<T>
using System.Reflection;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // ExpNode
    //
    ////////////////////////////////////////////////////////   
    public abstract class ExpNode : Node
    {
        //Constructor
        public ExpNode(String name)
            : base(name)
        {
        }

        //Accessors
        public abstract NodeType Type
        {
            get;
        }

        //Overrides
        public override Object Clone()
        {
            //TODO: Override in each derived exp class
            ExpNode clone = (ExpNode)base.Clone();
            return clone;
        }
    }

    ////////////////////////////////////////////////////////
    // QueryNode
    //
    ////////////////////////////////////////////////////////   
    public abstract class QueryNode : ExpNode
    {
        //Data
        protected ExpNode _input;

        //Constructor
        public QueryNode(ExpNode input)
            : base(null)
        {
            _input = input;
        }

        //Accessors
        public virtual ExpNode Input
        {
            get { return _input; }
        }
    }

    ////////////////////////////////////////////////////////
    // ConstantExpression
    //
    ////////////////////////////////////////////////////////   
    public class ConstantExpression : ExpNode
    {
        //Data
        protected NodeValue _value;

        //Constructor
        public ConstantExpression(NodeValue value)
            : base(null)
        {
            _value = value;
        }

        //Accessors
        public override NodeType Type
        {
            get { return this.Value.Type; }
        }

        public virtual NodeValue Value
        {
            get { return _value; }
        }

        //Overloads
        public override String ToString()
        {
            return this.Value.ToString();
        }
    }
    public class TypedMemberExpression : ExpNode
    {
        public Type EntityType { get; set; }
        public string PropertyName { get; set; }
        public TypedMemberExpression(Type entityType, string propertyName)
            : base("MemberExpression")
        {
            EntityType = entityType;
            PropertyName = propertyName;
        }

        public override NodeType Type
        {
            get { return null; }
        }
    }
    ////////////////////////////////////////////////////////
    // PropertyExpression
    //
    ////////////////////////////////////////////////////////   
    public class PropertyExpression : ExpNode
    {
        //Data
        protected NodeProperty _property;
        protected bool _valueOnly;

        //Constructor
        public PropertyExpression(NodeProperty property)
            : base(property.Name)
        {
            _property = property;
        }

        //Constructor
        public PropertyExpression(NodeProperty property, bool valueOnly)
            : base(property.Name)
        {
            _property = property;
            _valueOnly = valueOnly;
        }

        //Accessors
        public override NodeType Type
        {
            get { return this.Property.Type; }
        }

        public virtual NodeProperty Property
        {
            get { return _property; }
        }

        public virtual bool ValueOnly
        {
            get { return _valueOnly; }
        }
    }

    ////////////////////////////////////////////////////////
    // NestedPropertyExpression
    //
    ////////////////////////////////////////////////////////   
    public class NestedPropertyExpression : PropertyExpression
    {
        //Data
        protected PropertyExpression[] _propertyExpressions;

        //Constructor
        public NestedPropertyExpression(NodeProperty property, PropertyExpression[] propertyExpressions)
            : base(property)
        {
            _propertyExpressions = propertyExpressions;
        }

        //Constructor
        public NestedPropertyExpression(NodeProperty property, PropertyExpression[] propertyExpressions, bool valueOnly)
            : base(property, valueOnly)
        {
            _propertyExpressions = propertyExpressions;
        }

        public virtual PropertyExpression[] PropertyExpressions
        {
            get { return _propertyExpressions; }
        }
    }

    public class NullablePropertyExpression : PropertyExpression
    {
        public NullablePropertyExpression(NodeProperty property)
            : base(property)
        { }

        public NullablePropertyExpression(NodeProperty property, bool valueOnly)
            : base(property, valueOnly)
        { }
    }


    public class ServiceOpExpression : ExpNode
    {
        //Data
        protected Node _variable;
        protected NodeType _type = null;

        //Constructor
        public ServiceOpExpression(Node node)
            : base(null)
        {
            _variable = node;
        }

        public ServiceOpExpression(Node node, NodeType type)
            : base(null)
        {
            _type = type;
            _variable = node;
        }

        //Accessors
        public override NodeType Type
        {
            //TODO:
            get { return _type; }
        }

        public virtual Node Variable
        {
            get { return _variable; }
        }

    }


    ////////////////////////////////////////////////////////
    // VariableExpression
    //
    ////////////////////////////////////////////////////////   
    public class VariableExpression : ExpNode
    {
        //Data
        protected Node _variable;
        protected NodeType _type = null;

        //Constructor
        public VariableExpression(Node node)
            : base(null)
        {
            _variable = node;
        }

        public VariableExpression(Node node, NodeType type)
            : base(null)
        {
            _type = type;
            _variable = node;
        }

        //Accessors
        public override NodeType Type
        {
            //TODO:
            get { return _type; }
        }

        public virtual Node Variable
        {
            get { return _variable; }
        }
    }

    ////////////////////////////////////////////////////////
    // BinaryExpression
    //
    ////////////////////////////////////////////////////////   
    public class BinaryExpression : ExpNode
    {
        //Data
        protected ExpNode _left;
        protected ExpNode _right;

        //Constructor
        public BinaryExpression(ExpNode left, ExpNode right)
            : base(null)
        {
            _left = left;
            _right = right;
        }

        //Accessors
        public override NodeType Type
        {
            get { return this.Left.Type; }
        }

        public virtual ExpNode Left
        {
            get { return _left; }
        }

        public virtual ExpNode Right
        {
            get { return _right; }
        }
    }

    ////////////////////////////////////////////////////////
    // ComparisonOperator
    //
    ////////////////////////////////////////////////////////   
    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual
    };

    ////////////////////////////////////////////////////////
    // ComparisonExpression
    //
    ////////////////////////////////////////////////////////   
    public class ComparisonExpression : BinaryExpression
    {
        protected ComparisonOperator _operator;

        //Constructor
        public ComparisonExpression(ExpNode left, ExpNode right, ComparisonOperator op)
            : base(left, right)
        {
            _operator = op;
        }

        //Accessors
        public virtual ComparisonOperator Operator
        {
            get { return _operator; }
        }
    }

    public class IsOfExpression : ExpNode
    {
        //Constructor
        internal IsOfExpression(ExpNode target, NodeType targetType)
            : base(null)
        {
            Target = target;
            TargetType = targetType;
        }

        //Accessors
        public virtual NodeType TargetType
        {
            get;
            private set;
        }

        public virtual ExpNode Target
        {
            get;
            private set;
        }

        public override NodeType Type
        {
            get { return this.Target.Type; }
        }
    }

    public class CastExpression : QueryNode
    {
        //Constructor
        internal CastExpression(ExpNode target, NodeType targetType)
            : base(target)
        {
            TargetType = targetType;
        }

        //Accessors
        public virtual NodeType TargetType
        {
            get;
            private set;
        }

        public virtual ExpNode Target
        {
            get
            {
                return this.Input;
            }
        }

        public override NodeType Type
        {
            get { return this.Target.Type; }
        }
    }

    ////////////////////////////////////////////////////////
    // LogicalOperator
    //
    ////////////////////////////////////////////////////////   
    public enum LogicalOperator
    {
        And,
        Or,
        Not
    };

    ////////////////////////////////////////////////////////
    // LogicalExpression
    //
    ////////////////////////////////////////////////////////   
    public class LogicalExpression : BinaryExpression
    {
        protected LogicalOperator _operator;

        //Constructor
        public LogicalExpression(ExpNode left, ExpNode right, LogicalOperator op)
            : base(left, right)
        {
            _operator = op;
        }

        //Accessors
        public virtual LogicalOperator Operator
        {
            get { return _operator; }
        }
    }

    ////////////////////////////////////////////////////////
    // ArithmeticOperator
    //
    ////////////////////////////////////////////////////////   
    public enum ArithmeticOperator
    {
        Add,
        Sub,
        Mult,
        Div,
        Mod
    };

    ////////////////////////////////////////////////////////
    // ArithmeticExpression
    //
    ////////////////////////////////////////////////////////   
    public class ArithmeticExpression : BinaryExpression
    {
        protected ArithmeticOperator _operator;

        //Constructor
        public ArithmeticExpression(ExpNode left, ExpNode right, ArithmeticOperator op)
            : base(left, right)
        {
            _operator = op;
        }

        //Accessors
        public virtual ArithmeticOperator Operator
        {
            get { return _operator; }
        }
    }

    ////////////////////////////////////////////////////////
    // NegateExpression
    //
    ////////////////////////////////////////////////////////   
    public class NegateExpression : ExpNode
    {
        public NegateExpression(ExpNode argument)
            : base("Negate")
        {
            this.Argument = argument;
        }

        public ExpNode Argument
        {
            get;
            private set;
        }

        public override NodeType Type
        {
            get
            {
                return null;
            }
        }
    }

    ////////////////////////////////////////////////////////
    // MethodExpression
    //
    ////////////////////////////////////////////////////////   
    public class MethodExpression : ExpNode
    {
        //Data
        protected ExpNode[] _arguments = null;
        protected ExpNode _caller = null;
        protected MethodInfo _methodinfo = null;
        protected NodeType _type;

        //Constructor
        internal MethodExpression(ExpNode caller, MethodInfo method, ExpNode[] args)
            : base(method.Name)
        {
            _caller = caller;
            _arguments = args;
            _methodinfo = method;
        }

        //Accessors
        public virtual ExpNode Caller
        {
            get { return _caller; }
        }

        public virtual ExpNode[] Arguments
        {
            get { return _arguments; }
        }

        public virtual MethodInfo MethodInfo
        {
            get { return _methodinfo; }
        }

        //Overrides
        public override NodeType Type
        {
            get { return _type; }
        }

    }

    ////////////////////////////////////////////////////////
    // MethodExpression
    //
    ////////////////////////////////////////////////////////   
    public class MemberExpression : ExpNode
    {
        //Data
        protected ExpNode[] _arguments = null;
        protected ExpNode _caller = null;
        protected PropertyInfo _property = null;
        protected NodeType _type;

        //Constructor
        internal MemberExpression(ExpNode caller, PropertyInfo property, ExpNode[] args)
            : base(property.Name)
        {
            _caller = caller;
            _arguments = args;
            _property = property;
        }

        //Accessors
        public virtual ExpNode Caller
        {
            get { return _caller; }
        }

        public virtual ExpNode[] Arguments
        {
            get { return _arguments; }
        }

        public virtual PropertyInfo Property
        {
            get { return _property; }
        }

        //Overrides
        public override NodeType Type
        {
            get { return _type; }
        }

    }

    ////////////////////////////////////////////////////////
    // PathExpression
    //
    ////////////////////////////////////////////////////////   
    public class PathExpression : ExpNode
    {
        protected Nodes<StepExpression> _steps;
        protected Nodes<NameValuePair> _queryString;

        //Constructor
        public PathExpression(StepExpression[] steps, NameValuePair[] queryString) :
            base(null)
        {
            _steps = new Nodes<StepExpression>(this, steps);
            _queryString = new Nodes<NameValuePair>(this, queryString);
        }

        public ResourceContainer Container
        {
            get
            {
                ResourceContainer container = null;
                if (Steps.Count > 0)
                {
                    ContainerStepExpression step = Steps[0] as ContainerStepExpression;

                    if (step != null)
                    {
                        container = step.Container;
                    }
                }
                return container;
            }
        }

        public Nodes<StepExpression> Steps
        {
            get { return _steps; }
        }

        public Nodes<NameValuePair> QueryString
        {
            get { return _queryString; }
        }

        //Need to implement
        public override NodeType Type
        {
            get { throw new NotImplementedException(); }
        }
    }

    ////////////////////////////////////////////////////////
    // StepExpression
    //
    ////////////////////////////////////////////////////////   
    public abstract class StepExpression : ExpNode
    {
        protected Node _segment;
        protected PredicateExpression _predicate;

        public StepExpression(Node segment, PredicateExpression predicate) :
            base(null)
        {
            _segment = segment;
            _predicate = predicate;
        }

        public Node Segment
        {
            get { return _segment; }
        }

        public PredicateExpression Predicate
        {
            get { return _predicate; }
        }
    }


    ////////////////////////////////////////////////////////
    // PropertyStepExpression
    //
    ////////////////////////////////////////////////////////   
    public class PropertyStepExpression : StepExpression
    {
        public PropertyStepExpression(PropertyExpression property, PredicateExpression predicate) :
            base(property, predicate)
        {
        }

        public override NodeType Type
        {
            get { return Property.Type; }
        }

        public PropertyExpression Property
        {
            get { return (PropertyExpression)this.Segment; }
        }

    }

    ////////////////////////////////////////////////////////
    // ContainerStepExpression
    //
    ////////////////////////////////////////////////////////   
    public class ContainerStepExpression : StepExpression
    {
        public ContainerStepExpression(ResourceContainer container, PredicateExpression predicate) :
            base(container, predicate)
        {
        }

        public override NodeType Type
        {
            get { return null; }
        }

        public ResourceContainer Container
        {
            get { return (ResourceContainer)this.Segment; }
        }
    }

    ////////////////////////////////////////////////////////
    // KeyExpression
    //
    ////////////////////////////////////////////////////////   
    public class KeyExpression : ExpNode
    {
        private ResourceContainer _resourceContainer;
        private ResourceType _resourceType;
        private PropertyExpression[] _properties;
        private ConstantExpression[] _values;
        private bool[] _include;

        public KeyExpression(ResourceContainer container, ResourceType resourceType, PropertyExpression[] properties,
            ConstantExpression[] values, bool[] includeInKey)
            : base(null)
        {
            if (properties == null || values == null || properties.Length != values.Length || (includeInKey != null && includeInKey.Length != properties.Length))
            {
                throw new Exception("Error, Invalid Key Expression.  " +
                    "Each item in the properties array must have a corresponing " +
                    "value in the values array");
            }

            _properties = properties;
            _values = values;
            _resourceType = resourceType;
            _resourceContainer = container; // originally, this would not happen if using constructor without type
            _include = includeInKey;
            ETag = null;
            KnownPropertyValues = new Dictionary<string, object>();

            for (int i = 0; i < properties.Length; i++)
            {
                KnownPropertyValues[properties[i].Name] = values[i].Value.ClrValue;
            }

            if (_include == null)
            {
                _include = new bool[properties.Length];
                for (int i = 0; i < _include.Length; i++)
                    _include[i] = true;
            }

            if (properties.Length > 1)
            {
                // reorder to match the order on the type
                // to avoid a linq translator bug that generates filters
                int new_pos = 0;
                foreach (ResourceProperty keyProp in resourceType.Key.Properties)
                {
                    // because we always push the wrongly placed properties towards the back
                    // we can assume the beginning is ok
                    // also, in the case when they're equal, we don't do any extra work (beyond checking that theyre equal)
                    for (int old_pos = new_pos; old_pos < _properties.Length; old_pos++)
                    {
                        if (keyProp.Name.Equals(_properties[old_pos].Name))
                        {
                            if (old_pos != new_pos)
                                SwapPositions(old_pos, new_pos);
                            break;
                        }
                    }
                    new_pos++;
                }
            }
        }

        private void SwapPositions(int old_pos, int new_pos)
        {
            SwapPositions(_properties, old_pos, new_pos);
            SwapPositions(_values, old_pos, new_pos);
            SwapPositions(_include, old_pos, new_pos);
        }

        private static void SwapPositions<T>(T[] array, int old_pos, int new_pos)
        {
            T temp = array[new_pos];
            array[new_pos] = array[old_pos];
            array[old_pos] = temp;
        }

        public KeyExpression(ResourceContainer container, PropertyExpression[] properties, ConstantExpression[] values)
            : this(container, container.BaseType, properties, values, null) { }

        public KeyExpression(ResourceContainer container, ResourceType resourceType, PropertyExpression[] properties, ConstantExpression[] values)
            : this(container, resourceType, properties, values, null) { }

        public KeyExpression(ResourceContainer container, ResourceType resourceType, IDictionary<string, object> properties)
            : base(null)
        {
            // populate key and ETag from properties
            List<NodeProperty> keyProperties = resourceType.Key.Properties.ToList();

            _properties = new PropertyExpression[keyProperties.Count];
            _values = new ConstantExpression[keyProperties.Count];
            _include = new bool[keyProperties.Count];
            _resourceType = resourceType;
            _resourceContainer = container; // originally, this would not happen if using constructor without type

            for (int i = 0; i < keyProperties.Count; i++)
            {
                NodeProperty property = keyProperties[i];
                _properties[i] = new PropertyExpression(property);
                _values[i] = new ConstantExpression(new NodeValue(properties[property.Name], property.Type));
                _include[i] = true;
            }

            if (resourceType.Properties.Any(p => p.Facets.ConcurrencyModeFixed))
                ETag = ConcurrencyUtil.ConstructETag(container, properties);
            else
                ETag = null;

            this.KnownPropertyValues = properties;
        }

        public override NodeType Type
        {
            get { return Clr.Types.Boolean; }
        }

        public virtual NodeValue[] Values
        {
            get { return _values.Select(c => c.Value).ToArray(); }
            set { this.Values = value; }
        }

        public virtual NodeProperty[] Properties
        {
            get { return _properties.Select(p => p.Property).ToArray(); }
        }

        public virtual bool[] IncludeInUri
        {
            get { return _include; }
            set { this._include = value; }
        }

        public ResourceType ResourceType
        {
            get { return _resourceType; }
        }
        public ResourceContainer ResourceContainer
        {
            get { return _resourceContainer; }
        }
        //Build an equivalent comparison expression for the key expression
        //  because we don't know which will be included and which won't,
        //  build a list of equality tests first, then AND them all together
        public virtual ExpNode Predicate
        {
            get
            {
                // TODO: revert this function
                if (_properties.Length == 0)
                    return null;

                List<ExpNode> tests = new List<ExpNode>();
                for (int i = 0; i < _properties.Length; i++)
                {
                    tests.Add(Exp.Equal(_properties[i], _values[i]));
                }

                switch (tests.Count)
                {
                    case 0:
                        return null;

                    case 1:
                        return tests[0];

                    default:
                        // at least 2, AND them together
                        ExpNode predicate = tests[0];
                        for (int i = 1; i < tests.Count; i++)
                            predicate = Exp.And(predicate, tests[i]);
                        return predicate;
                }
            }
        }
        public override bool Equals(object obj)
        {
            KeyExpression keyExp = obj as KeyExpression;
            if (keyExp == null)
                return false;
            ResourceProperty[] resProperties = keyExp.Properties.OfType<ResourceProperty>().ToArray();
            ResourceProperty[] thisResProperties = keyExp.Properties.OfType<ResourceProperty>().ToArray();
            if (resProperties.Length != thisResProperties.Length)
                return false;
            if (this.Values.Length != keyExp.Values.Length)
                return false;
            if (this.IncludeInUri.Length != keyExp.IncludeInUri.Length)
                return false;
            for (int i = 0; i < keyExp.Values.Length; i++)
            {
                if (!this.Values[i].ClrValue.Equals(keyExp.Values[i].ClrValue))
                    return false;
                if (!resProperties[i].Name.Equals(thisResProperties[i].Name))
                    return false;
                if (this.IncludeInUri[i] != keyExp.IncludeInUri[i])
                    return false;
            }
            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public Nullable<bool> _isApproxKeyValue = null;
        public bool IsApproxKeyValue()
        {
            if (_isApproxKeyValue == null)
            {
                _isApproxKeyValue = false;
                List<ResourceProperty> approxPrecisionKeys = new List<ResourceProperty>();
                bool approxPrecisionFound = false;
                //If any of the key primitive types sometimes have inaccurate precision (ie: float)
                //Remove from collection, only want valid keys out of here
                foreach (ResourceProperty keyProp in this.Properties)
                {
                    if ((keyProp.Type as PrimitiveType).IsApproxPrecision)
                    {
                        approxPrecisionFound = true;
                    }
                }

                if (approxPrecisionFound)
                {
                    for (int i = 0; i < this.Properties.Length; i++)
                    {
                        NodeValue val = this.Values[i];
                        ResourceProperty rp = this.Properties[i] as ResourceProperty;
                        if ((rp.Type as PrimitiveType).IsApproxPrecision)
                        {
                            bool isApproxComparable = (rp.Type as PrimitiveType).IsApproxValueComparable(val);
                            if (!isApproxComparable)
                            {
                                _isApproxKeyValue = true;
                                break;
                            }

                        }
                    }

                }
            }
            return _isApproxKeyValue.Value;
        }

        public IEnumerable<KeyValuePair<PropertyExpression, ConstantExpression>> EnumerateIncludedPairs()
        {
            for (int i = 0; i < _properties.Length; i++)
            {
                if (_include[i])
                    yield return new KeyValuePair<PropertyExpression, ConstantExpression>(_properties[i], _values[i]);
            }
        }

        public string ETag
        {
            get;
            private set;
        }

        public IDictionary<string, object> KnownPropertyValues
        {
            get;
            private set;
        }
    }
    public class KeyExpressions : fxList<KeyExpression>
    {
        public KeyExpressions()
            : base()
        {
        }
        public KeyExpressions(params KeyExpression[] exps)
            : base(exps)
        {
        }
        public KeyExpressions(IEnumerable<KeyExpression> exps)
            : base(exps)
        {
        }
        public KeyExpressions Diff(KeyExpressions keyExpressions)
        {
            KeyExpressions diffKeyExpressions = new KeyExpressions();
            foreach (KeyExpression keyExp in keyExpressions)
            {
                bool found = false;
                foreach (KeyExpression keyExp2 in this)
                {
                    if (keyExp2.Equals(keyExp))
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                    diffKeyExpressions.Add(keyExp);
            }
            return diffKeyExpressions;
        }
    }

    public class NameValuePair : Node
    {
        protected NodeValue _value;

        public NameValuePair(string name, NodeValue value) :
            base(name)
        {
            _value = value;
        }

        public NodeValue Value
        {
            get { return _value; }
        }
    }




}
