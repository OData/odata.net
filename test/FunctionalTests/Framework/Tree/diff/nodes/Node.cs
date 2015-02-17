//---------------------------------------------------------------------
// <copyright file="Node.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections;                   //IEnumerable
using System.Collections.Generic;
using System.Diagnostics;           //List<T>
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    // Node
    //
    ////////////////////////////////////////////////////////   
    public abstract class Node : fxBase, IComparable<Node>, ICloneable
    {
        //Data
        protected Node _parent;
        protected String _namespace;
        protected String _desc;
        protected NodeFacets _facets;
        internal Nodes<Node> _other; //TODO:

        internal static Node[] EmptyNodes = new Node[0];

        //Constructor
        public Node(String name, params Node[] facets)
            : base(name)
        {
            _namespace = name;
            _desc = this.GetType().Name;
            _facets = new NodeFacets(this, facets.OfType<NodeFacet>());
            _other = new Nodes<Node>(this, facets.Except(_facets.OfType<Node>()));
        }
        public Node(String name, string nodeNamespace, params Node[] facets)
            : base(name)
        {
            _namespace = nodeNamespace;
            _desc = this.GetType().Name;
            _facets = new NodeFacets(this, facets.OfType<NodeFacet>());
            _other = new Nodes<Node>(this, facets.Except(_facets.OfType<Node>()));
        }
        //Accessors
        public virtual String Namespace
        {
            [DebuggerStepThrough]
            get { return _namespace; }
        }

        public override String Name
        {
            [DebuggerStepThrough]
            get { return base.Name; }

            [DebuggerStepThrough]
            set { base.Name = value; }
        }

        public virtual String Desc
        {
            [DebuggerStepThrough]
            get { return _desc; }
        }

        public virtual Node Parent
        {
            [DebuggerStepThrough]
            get { return _parent; }

            [DebuggerStepThrough]
            set { _parent = value; }
        }

        public virtual NodeFacets Facets
        {
            [DebuggerStepThrough]
            get { return _facets; }
        }

        public virtual string FullName
        {
            get { return this.Namespace + "." + this.Name; }
        }

        /// <summary>Enumerates all nodes under <paramref name="node"/> including itself.</summary>
        /// <param name="node">Root node to enumerate.</param>
        /// <returns>An enumeration with <paramref name="node"/> and all its children, recursively.</returns>
        public static IEnumerable<Node> EnumerateNodes(Node node)
        {
            if (node != null)
            {
                yield return node;

                if (node is ExpNode)
                {
                    var e = (ExpNode)node;
                    foreach (Node n in EnumerateNodes(e.Type))
                    {
                        yield return n;
                    }
                }

                if (node is BinaryExpression)
                {
                    var b = (BinaryExpression)node;
                    foreach (Node n in EnumerateNodes(b.Left)) yield return n;
                    foreach (Node n in EnumerateNodes(b.Right)) yield return n;
                }

                if (node is ConstantExpression)
                {
                    var e = (ConstantExpression)node;
                    foreach (Node n in EnumerateNodes(e.Value)) yield return n;
                }

                if (node is KeyExpression)
                {
                    var e = (KeyExpression)node;
                    foreach (Node n in EnumerateNodes(e.Predicate)) yield return n;
                    foreach (NodeProperty p in e.Properties) foreach (Node n in EnumerateNodes(p)) yield return n;
                    foreach (NodeValue v in e.Values) foreach (Node n in EnumerateNodes(v)) yield return n;
                }

                if (node is ModifyExpression)
                {
                    var e = (ModifyExpression)node;
                    foreach (Node argument in e.Arguments) foreach (Node n in EnumerateNodes(argument)) yield return n;
                    foreach (Node n in EnumerateNodes(e.Target)) yield return n;
                }

                if (node is ResourceInstanceCollection)
                {
                    var e = (ResourceInstanceCollection)node;
                    foreach (ResourceBodyTree u in e.NodeList) foreach (Node n in EnumerateNodes(u)) yield return n;
                }

                if (node is ResourceInstanceComplexProperty)
                {
                    var e = (ResourceInstanceComplexProperty)node;
                    foreach (Node n in e.ComplexResourceInstance.Properties) yield return n;
                }

                if (node is ResourceInstanceKey)
                {
                    var e = (ResourceInstanceKey)node;
                    foreach (ResourceInstanceProperty p in e.KeyProperties) foreach (Node n in EnumerateNodes(p)) yield return n;
                }

                if (node is ResourceInstanceNavColProperty)
                {
                    var e = (ResourceInstanceNavColProperty)node;
                    foreach (Node n in EnumerateNodes(e.Collection)) yield return n;
                }

                if (node is ComplexResourceInstance)
                {
                    var e = (ComplexResourceInstance)node;
                    foreach (ResourceInstanceProperty p in e.Properties) foreach (Node n in EnumerateNodes(p)) yield return n;
                }

                if (node is KeyedResourceInstance)
                {
                    var e = (KeyedResourceInstance)node;
                    foreach (ResourceInstanceProperty p in e.Properties) foreach (Node n in EnumerateNodes(p)) yield return n;
                    foreach (Node n in EnumerateNodes(e.ResourceInstanceKey)) yield return n;
                }

                if (node is NewExpression)
                {
                    //var e = (NewExpression)node;
                    //foreach (ExpNode argument in e.Arguments) foreach (Node n in EnumerateNodes(argument)) yield return n;
                    //foreach (Node n in EnumerateNodes(e.Target)) yield return n;
                }

                if (node is PathExpression)
                {
                    var e = (PathExpression)node;
                    foreach (NameValuePair p in e.QueryString) foreach (Node n in EnumerateNodes(p)) yield return n;
                    foreach (StepExpression s in e.Steps) foreach (Node n in EnumerateNodes(s)) yield return n;
                }

                if (node is PropertyExpression)
                {
                    var e = (PropertyExpression)node;
                    foreach (Node n in EnumerateNodes(e.Property)) yield return n;
                }

                if (node is QueryNode)
                {
                    var e = (QueryNode)node;
                    foreach (Node n in EnumerateNodes(e.Input)) yield return n;
                }

                if (node is NodeType)
                {
                    var e = (NodeType)node;
                    foreach (Node n in EnumerateNodes(e.Default)) yield return n;
                    foreach (Node n in EnumerateNodes(e.Null)) yield return n;
                }

                if (node is CollectionType)
                {
                    var e = (CollectionType)node;
                    foreach (Node n in EnumerateNodes(e.SubType)) yield return n;
                }

                if (node is ComplexType)
                {
                    var e = (ComplexType)node;
                    foreach (Node n in EnumerateNodes(e.BaseType)) yield return n;
                    foreach (Node n in EnumerateNodes(e.Condition)) yield return n;
                    foreach (NodeProperty p in e.Properties) foreach (Node n in EnumerateNodes(p)) yield return n;
                    foreach (NodeRelation r in e.Relations) foreach (Node n in EnumerateNodes(r)) yield return n;
                }

                if (node is ResourceType)
                {
                    var e = (ResourceType)node;
                    foreach (ResourceAssociation a in e.Associations) foreach (Node n in EnumerateNodes(a)) yield return n;
                    foreach (Node n in EnumerateNodes(e.Key)) yield return n;
                }

                if (node is NodeValue)
                {
                    var e = (NodeValue)node;
                    foreach (Node n in EnumerateNodes(e.Type)) yield return n;
                }

                // TODO: handle ExpandExpression, NavigationExpression, OrderByExpression, PredicateExpression,
                // ProjectExpression, ScanExpression, SkipExpression, TopExpression,
                // StepExpression, ContainerStepExpression, PropertyStepExpression,
                // VariableExpression,
                // NameValuePair,
                // NodeFacet,
                // NodeProperty, ResourceProperty
                // NodeSet,
                // ResourceAssociationEnd
            }
        }

        /// <summary>
        /// Enumerates all trees generated by applying replacements given 
        /// by the specified <paramref name="mutator"/> on each node.
        /// </summary>
        /// <param name="rootNode">Root node of the tree.</param>
        /// <param name="mutator">Callback to return mutated nodes for replacement.</param>
        /// <returns>An enumeration of all mutated trees.</returns>
        public static IEnumerable<Node> MutateAllNodes(Node rootNode, Func<Node, IEnumerable<Node>> mutator)
        {
            if (rootNode == null)
            {
                throw new ArgumentNullException("rootNode");
            }
            if (mutator == null)
            {
                throw new ArgumentNullException("mutator");
            }

            foreach (Node candidateNode in EnumerateNodes(rootNode))
            {
                IEnumerable<Node> mutations = mutator(candidateNode);
                foreach (Node mutatedNode in mutations)
                {
                    yield return ReplaceNode(rootNode, candidateNode, mutatedNode);
                }
            }
        }

        /// <summary>Returns a new tree with a replaced node.</summary>
        /// <param name="rootNode">Node of the tree to replace.</param>
        /// <param name="oldNode">Old node to have replaced.</param>
        /// <param name="newNode">New node to replace with.</param>
        /// <returns>The root of the new tree.</returns>
        public static Node ReplaceNode(Node rootNode, Node oldNode, Node newNode)
        {
            if (oldNode == rootNode)
            {
                return newNode;
            }
            else
            {
                //Node newRootNode = newNode;
                //do
                //{
                //    newRootNode = RecreateNodeWithNewChild(oldNode.Parent, oldNode, newRootNode);
                //    oldNode = oldNode.Parent;
                //}
                //while (oldNode != rootNode);
                //return newRootNode;

                // Parent property isn't reliable on nodes, so we're only
                // making replacements for simple properties at the moment.
                if (rootNode is ComplexResourceInstance)
                {
                    var r = (ComplexResourceInstance)rootNode;
                    var newRoot = (ComplexResourceInstance)r.Clone();
                    bool replacementDone = false;
                    for (int i = 0; i < r.Properties.Count; i++)
                    {
                        if (r.Properties[i] == oldNode)
                        {
                            newRoot.Properties[i] = (ResourceInstanceProperty)newNode;
                            replacementDone = true;
                        }
                    }

                    if (!replacementDone)
                    {
                        throw new InvalidOperationException("No replacement done on '" + rootNode.ToString() +
                            "' from '" + oldNode.ToString() + "' to '" + newNode.ToString() + "'.");
                    }

                    return newRoot;
                }
                else
                {
                    throw new NotImplementedException("ReplaceNode not implemented for type '" + rootNode.GetType() + "'.");
                }
            }
        }

        //Overrides
        public override String ToString()
        {
            return this.Name;
        }

        public virtual int CompareTo(Node b)
        {
            return String.Compare(this.Name, b.Name);
        }

        public virtual Object Clone()
        {
            Node clone = (Node)base.MemberwiseClone();
            clone._facets = (NodeFacets)this._facets.Clone();
            return clone;
        }

        public string Xml
        {
            get
            {
                return this.Visit(null, this);
            }
        }

        private string Visit(Node caller, Node node)
        {
            if (node is ProjectExpression)
            {
                ProjectExpression e = (ProjectExpression)node;
                if (e.Projections.Count == 0)
                {
                    return String.Format("<ProjectExpression><Input>{0}</Input></ProjectExpression>", this.Visit(e, e.Input));
                }
                else if (e.Projections.Count == 1)
                {
                    //if (e.Projections[0] is PropertyExpression)
                    return String.Format("<ProjectExpression><Input>{0}</Input><ProjectionZero>{1}</ProjectionZero></ProjectExpression>", this.Visit(e, e.Input), this.Visit(e, e.Projections[0]));
                    //else if (e.Projections[0] is ConstantExpression)
                    //    return String.Format("<ProjectExpression><Input>{0}</Input>{1}</ProjectExpression>", this.Visit(e, e.Input), this.Visit(e, e.Projections[0]));
                    //else return String.Format("encountered {0}", e.Projections[0].Name);
                }
                else
                    throw new Exception("More than 1 projection, invalid");
            }
            else if (node is ComparisonExpression)
            {
                return WriteBinaryExpressionXml("ComparisonExpression", node);
            }
            else if (node is ArithmeticExpression)
            {
                return WriteBinaryExpressionXml("ArithmeticException", node);
            }
            else if (node is ScanExpression)
            {
                ScanExpression e = (ScanExpression)node;
                if (e.Input is VariableExpression && ((VariableExpression)e.Input).Variable is ResourceContainer)
                    return String.Format("<ScanExpression><Input>{0}</Input></ScanExpression>", this.Visit(e, e.Input));

                throw new Exception("Unsupported on in scan expression");
            }
            else if (node is PredicateExpression)
            {
                PredicateExpression e = (PredicateExpression)node;
                return String.Format("<PredicateExpression><Input>{0}</Input><Predicate>{1}</Predicate></PredicateExpression>",
                        this.Visit(e, e.Input), this.Visit(e, e.Predicate));
            }
            else if (node is NavigationExpression)
            {
                NavigationExpression e = (NavigationExpression)node;
                return String.Format("<NavigationExpression IsLink=\"{2}\"><Input>{0}</Input><PropertyExp>{1}</PropertyExp></NavigationExpression>", this.Visit(e, e.Input), this.Visit(e, e.PropertyExp), e.IsLink.ToString());
            }
            else if (node is KeyExpression)
            {
                string keyExpressionString = BuildKeyExpressionXml((KeyExpression)node);
                return String.Format("<KeyExpression>{0}</KeyExpression>", keyExpressionString);
                //UriQueryBuilder.CreateKeyString((KeyExpression)node, false));
            }
            else if (node is NestedPropertyExpression)
            {
                NestedPropertyExpression e = (NestedPropertyExpression)node;

                string nestedProperty = "";
                foreach (PropertyExpression p in e.PropertyExpressions)
                {
                    nestedProperty += this.Visit(e, p);
                }

                return String.Format("<NestedPropertyExpression>{0}</NestedPropertyExpression>", nestedProperty);
            }
            else if (node is PropertyExpression)
            {
                PropertyExpression e = (PropertyExpression)node;
                return String.Format("<PropertyExpression Name=\"{0}\" ValueOnly=\"{1}\" />", e.Property.Name, e.ValueOnly.ToString());
            }
            else if (node is VariableExpression)
            {
                VariableExpression e = (VariableExpression)node;
                ResourceContainer container = e.Variable as ResourceContainer;
                return String.Format("<VariableExpression Name=\"{0}\" Type=\"{1}\"/>", container.Name,
                    container.BaseType.ClrType != null ? container.BaseType.ClrType.FullName : container.BaseType.Name
                    );
            }
            else if (node is ConstantExpression)
            {
                ConstantExpression e = (ConstantExpression)node;
                return String.Format("<ConstantExpression Value=\"{0}\" Type=\"{1}\" />", e.Value.ClrValue, e.Value.ClrValue.GetType().FullName);
            }
            else if (node is ServiceContainer)
            {
                string containerString = null;

                ServiceContainer e = (ServiceContainer)node;
                foreach (ResourceContainer container in e.ResourceContainers)
                {
                    containerString += this.Visit(e, container);
                }

                return String.Format("<ServiceContainer Name=\"{0}\" Namespace=\"{1}\">{2}</ServiceContainer>", e.Name, e.Namespace, containerString);
            }
            else if (node is ResourceContainer)
            {
                string typeString = null;

                ResourceContainer e = (ResourceContainer)node;
                foreach (ResourceType type in e.ResourceTypes)
                {
                    typeString += this.Visit(e, type);
                }

                return String.Format("<ResourceContainer Name=\"{0}\" Namespace=\"{1}\">{2}</ResourceContainer>", e.Name, e.Namespace, typeString);
            }
            else if (node is ResourceType)
            {
                string propertyString = null;

                ResourceType e = (ResourceType)node;
                foreach (ResourceProperty property in e.Properties)
                {
                    propertyString += this.Visit(e, property);
                }

                return String.Format("<ResourceType Name=\"{0}\" Namespace=\"{1}\">{2}</ResourceType>", e.Name, e.Namespace, propertyString);
            }
            else if (node is ResourceProperty)
            {
                ResourceProperty e = (ResourceProperty)node;
                return String.Format("<ResourceProperty Name=\"{0}\" IsNavigation=\"{1}\" IsComplexType=\"{2}\" />", e.Name, e.IsNavigation, e.IsComplexType);
            }
            else 
            {
                return String.Format("<{0} name=\'{1}\'></{0}>", node.GetType().Name, node.Name);
            }
        }

        private string WriteBinaryExpressionXml(string expressionName, Node node)
        {
            return String.Format("<{0} operand=\"{1}\"><Left>{2}</Left><Right>{3}</Right></{0}>",
                                expressionName,
                                node is ArithmeticExpression ? ((ArithmeticExpression)node).Operator.ToString()
                                : ((ComparisonExpression)node).Operator.ToString()
                                , this.Visit(node, ((BinaryExpression)node).Left),
                                this.Visit(node, ((BinaryExpression)node).Right));
        }

        private string BuildKeyExpressionXml(KeyExpression keyExpression)
        {
            StringBuilder sbKeyExpression = new StringBuilder();
            for (int propertyIndex = 0; propertyIndex < keyExpression.Properties.Length; propertyIndex++)
            {
                sbKeyExpression.AppendFormat("<ArithmeticExpression><Equal><Left><PropertyExpression>{0}</PropertyExpression></Left><Right><ConstantExpression><Type>{1}</Type><Value>{2}</Value></ConstantExpression></Right></Equal></ArithmeticExpression>",
                    keyExpression.Properties[propertyIndex].Name, keyExpression.Values[propertyIndex].ClrValue.GetType().FullName,
                    keyExpression.Values[propertyIndex].ClrValue.ToString()
                    );
            }
            return sbKeyExpression.ToString();
        }
    }

    ////////////////////////////////////////////////////////
    // Nodes
    //
    ////////////////////////////////////////////////////////   
    public class Nodes<T> : fxList<T>, ICloneable where T : Node
    {
        //Data
        protected Node _parent;

        //Constructor
        public Nodes(Node parent, IEnumerable<T> array)
            : base()
        {
            _parent = parent;
            this.Add(array);
        }

        //Accessors
        public virtual T this[String name]
        {
            get { return this.Where(n => n.Name == name).FirstOrDefault(); }
            set { this[this.IndexOf(this[name])] = value; }
        }

        public virtual void Remove(String name)
        {
            base.Remove(this[name]);
        }

        //Overrides
        public override T Add(T item)
        {
            //Update the parent, although only if not already set
            if (item.Parent == null)
                item.Parent = _parent;
            return base.Add(item);
        }

        public virtual Object Clone()
        {
            Nodes<T> clone = (Nodes<T>)base.MemberwiseClone();

            //Deep clone the list of nodes
            clone._list = new List<T>();
            foreach (T item in this)
                clone.Add((T)item.Clone());

            return clone;
        }

        public override String ToString()
        {
            String format = null;
            foreach (T item in this)
            {
                if (format != null)
                    format += ", ";
                format += item.Name;
            }
            return format;

        }
    }

    ////////////////////////////////////////////////////////
    // NodeSet
    //
    ////////////////////////////////////////////////////////   
    public abstract class NodeSet : Node, IEnumerable<Node>
    {
        //Data

        //Constructor
        public NodeSet(String name)
            : base(name)
        {
            _desc = "Set";
        }

        //Abstract
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<Node>)this.GetEnumerator();
        }

        public abstract IEnumerator<Node> GetEnumerator();

        //Overrides
    }

    ////////////////////////////////////////////////////////
    // NodeSet<T>
    //
    ////////////////////////////////////////////////////////   
    public class NodeSet<T> : NodeSet where T : Node
    {
        //Data
        protected Nodes<T> _nodes;

        //Constructor
        public NodeSet(String name, params Node[] nodes)
            : base(name)
        {
            _nodes = new Nodes<T>(this, new T[] { });
            if (nodes != null)
            {
                // go through AddNode in case its been overridden
                foreach (T node in nodes.OfType<T>())
                    this.AddNode(node);
            }
        }

        public virtual void AddNode(T node)
        {
            _nodes.Add(node);
        }

        public virtual void AddNodes(IEnumerable<T> nodes)
        {
            _nodes.Add(nodes);
        }

        public virtual void RemoveNode(T node)
        {
            _nodes.Remove(node);
        }


        //Accessors
        public override IEnumerator<Node> GetEnumerator()
        {
            return _nodes.OfType<Node>().GetEnumerator();
        }

        public virtual Nodes<T> Nodes
        {
            [DebuggerStepThrough]
            get { return _nodes; }
        }

        //Overrides
        public override Object Clone()
        {
            NodeSet<T> clone = (NodeSet<T>)base.Clone();
            clone._nodes = (Nodes<T>)this._nodes.Clone();
            return clone;
        }

        public override String ToString()
        {
            return _nodes.ToString();
        }
    }
}
