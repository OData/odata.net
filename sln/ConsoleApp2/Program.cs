namespace ConsoleApp2
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    using Microsoft.OData.Edm;
    using Microsoft.OData.Edm.Csdl;
    using Microsoft.OData.UriParser;

    class Program
    {
        static IEdmModel Read(string path)
        {
            using (var csdl = File.OpenRead(path))
            {
                using (var xmlReader = XmlReader.Create(csdl))
                {
                    return CsdlReader.Parse(xmlReader);
                }
            }
        }

        static void Write(string path, IEdmModel edmModel)
        {
            using (var file = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
            {
                using (var writer = XmlWriter.Create(file))
                {
                    var written = CsdlWriter.TryWriteCsdl(edmModel, writer, CsdlTarget.OData, out var errors);
                }
            }
        }

        static void Main(string[] args)
        {
            var @out = TextWriter.Null;
            if (!Array.Exists(args, element => string.Equals(element, "disableout", StringComparison.OrdinalIgnoreCase)))
            {
                @out = Console.Out;
            }

            var error = Console.Error;

            @out.WriteLine("Welcome. To disable these messages, please provide the \"disableout\" argument.");

            var csdlPathIndex = Array.IndexOf(args, "csdlpath");
            if (csdlPathIndex + 1 >= args.Length)
            {
                error.WriteLine("The \"csdlpath\" argument was provided without a path");
                return;
            }

            @out.WriteLine("Using the CSDL file at {0}", args[csdlPathIndex + 1]);

            //// TODO
            /*var model = new EdmModel();
            model.SetNamespaceAlias("microsoft.graph", "graph");
            var container = model.AddEntityContainer("graph2", "GraphService2");
            container.AddEntitySet()

            model.AddEntityContainer("graph", "GraphService");
            Write(@"C:\github\odata.net\sln\ConsoleApp2\bin\Debug\netcoreapp2.0\csdl2.xml", model);*/

            var provider = new RequestProvider(Read(args[csdlPathIndex + 1]));

            while (true)
            {
                @out.WriteLine();
                @out.WriteLine("Enter the HTTP verb followed by the relative URI of the resource.");
                var resource = Console.ReadLine().Trim();
                var seperatorIndex = resource.IndexOf(" ");
                if (seperatorIndex < 0)
                {
                    error.WriteLine("The request must have a verb and a URI");
                    continue;
                }

                var verb = resource.Substring(0, seperatorIndex);

                resource = resource.Substring(seperatorIndex).Trim();
                if (resource.Length == 0)
                {
                    error.WriteLine("The request must have a URI");
                    continue;
                }

                Uri uri;
                try
                {
                    uri = new Uri(resource, UriKind.Relative);
                }
                catch (Exception e)
                {
                    error.WriteLine("The resource is not a valid URI: {0}", e.Message);
                    continue;
                }

                @out.WriteLine("Enter the body of the request. Enter two empty lines to indicate the end of the body.");
                var body = new StringBuilder();
                var lastLine = Console.ReadLine();
                body.AppendLine(lastLine);
                string line;
                while ((line = Console.ReadLine()).Length != 0 || lastLine.Length != 0)
                {
                    body.AppendLine(line);
                    lastLine = line;
                }

                var request = new Request(verb, uri, body.ToString());
                Console.WriteLine(provider.ProvideRequest(request));
            }
        }

        public sealed class RequestProvider : IRequestProvider
        {
            private readonly IEdmModel model;

            public RequestProvider(IEdmModel model)
            {
                if (model == null)
                {
                    throw new ArgumentNullException(nameof(model));
                }

                this.model = model;
            }

            public string ProvideRequest(Request request)
            {
                var parser = new Microsoft.OData.UriParser.ODataUriParser(model, request.Resource);

                var odataUri = parser.ParseUri();
                /*var validation = odataUri.Filter.Expression.Accept(Validator.Instance);

                //// TODO GET /recommendations?$filter=recommendationResources/any(o:o/type eq 'asdf')
                //// TODO GET /recommendations?$filter=displayName eq 'gdebruin'
                if (validation.Recommendation && validation.Resource)
                {
                    throw new InvalidOperationException("cannot filter on both recommendation and recommendationresource");
                }*/

                return $"The {request.Verb} request to {request.Resource} with body {request.Body.Replace(Environment.NewLine, string.Empty)} was processed";
            }

            private sealed class Validator : QueryNodeVisitor<Validation>
            {
                private Validator()
                {
                }

                public static Validator Instance { get; } = new Validator();

                /// <summary>
                /// Visit an AllNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(AllNode nodeIn)
                {
                    /*validate(nodeIn);
                    ValidateNode(nodeIn.Body);
                    return true;*/

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit an AnyNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(AnyNode nodeIn)
                {
                    var recommendation = nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendation";
                    var resource = nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendationResource";

                    var validation = nodeIn.Body.Accept(this);
                    return new Validation(
                        recommendation || validation.Recommendation, 
                        resource || validation.Resource);
                }

                /// <summary>
                /// Visit a BinaryOperatorNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(BinaryOperatorNode nodeIn)
                {
                    var recommendation = nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendation";
                    var resource = nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendationResource";

                    var leftValidation = nodeIn.Left.Accept(this);
                    var rightValidation = nodeIn.Right.Accept(this);
                    return new Validation(
                        recommendation || leftValidation.Recommendation || rightValidation.Recommendation,
                        resource || leftValidation.Resource || leftValidation.Resource);
                }

                /// <summary>
                /// Visit a CountNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CountNode nodeIn)
                {
                    //validate(nodeIn);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a CollectionNavigationNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionNavigationNode nodeIn)
                {
                    var recommendation =
                        nodeIn.ItemType.Definition.FullTypeName() == "microsoft.graph.recommendation" ||
                        (nodeIn.ItemType.IsCollection() && nodeIn.ItemType.Definition.AsElementType().FullTypeName() == "microsoft.graph.recommendation");
                    var resource =
                        nodeIn.ItemType.Definition.FullTypeName() == "microsoft.graph.recommendationResource" ||
                        (nodeIn.ItemType.IsCollection() && nodeIn.ItemType.Definition.AsElementType().FullTypeName() == "microsoft.graph.recommendationResource");

                    var validation = nodeIn.Source.Accept(this);
                    return new Validation(
                        recommendation || validation.Recommendation,
                        resource || validation.Resource);
                }

                /// <summary>
                /// Visit a CollectionPropertyAccessNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionPropertyAccessNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.Property);

                    //// don't validate TypeReferences, only types, as nullability is only meaningful in context of model element
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //validate(nodeIn.ItemType.Definition);
                    //return true;

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a CollectionOpenPropertyAccessNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionOpenPropertyAccessNode nodeIn)
                {
                    //validate(nodeIn);

                    //// don't validate TypeReferences, only types, as nullability is only meaningful in context of model element
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //validate(nodeIn.ItemType.Definition);
                    //return true;

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a ConstantNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(ConstantNode nodeIn)
                {
                    var recommendation =
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendation";
                    var resource =
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendationResource";
                    return new Validation(recommendation, resource);
                }

                /// <summary>
                /// Visit a CollectionConstantNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionConstantNode nodeIn)
                {
                    //validate(nodeIn);
                    //return true;

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a ConvertNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(ConvertNode nodeIn)
                {
                    var recommendation = 
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendation" ||
                        (nodeIn.TypeReference.IsCollection() && nodeIn.TypeReference.Definition.AsElementType().FullTypeName() == "microsoft.graph.recommendation");
                    var resource = 
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendationResource" ||
                        (nodeIn.TypeReference.IsCollection() && nodeIn.TypeReference.Definition.AsElementType().FullTypeName() == "microsoft.graph.recommendationResource");

                    var validation = nodeIn.Source.Accept(this);
                    return new Validation(
                        recommendation || validation.Recommendation,
                        resource || validation.Resource);
                }

                /// <summary>
                /// Visit an CollectionResourceCastNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionResourceCastNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //validate(nodeIn.ItemType.Definition);
                    //return true;

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit an ResourceRangeVariableReferenceNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(ResourceRangeVariableReferenceNode nodeIn)
                {
                    var recommendation = 
                        nodeIn.RangeVariable.Name != "$it" && 
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendation";
                    var resource =
                        nodeIn.RangeVariable.Name != "$it" &&
                        nodeIn.TypeReference.Definition.FullTypeName() == "microsoft.graph.recommendationResource";

                    var validation = nodeIn.RangeVariable.CollectionResourceNode?.Accept(this) ?? new Validation();
                    return new Validation(
                        recommendation || validation.Recommendation,
                        resource || validation.Resource);
                }

                /// <summary>
                /// Visit a NonEntityRangeVariableNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(NonResourceRangeVariableReferenceNode nodeIn)
                {
                    //// todo: do we even need to call for a range variable?
                    //validate(nodeIn);
                    //return true;

                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleResourceCastNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleResourceCastNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleNavigationNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleNavigationNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.NavigationProperty);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleResourceFunctionCallNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleResourceFunctionCallNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.TypeReference.Definition);
                    //foreach (IEdmFunction function in nodeIn.Functions)
                    //{
                    //    validate(function);
                    //}

                    //foreach (QueryNode param in nodeIn.Parameters)
                    //{
                    //    ValidateNode(param);
                    //}

                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleValueFunctionCallNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleValueFunctionCallNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.TypeReference.Definition);
                    //foreach (IEdmFunction function in nodeIn.Functions)
                    //{
                    //    validate(function);
                    //}

                    //foreach (QueryNode param in nodeIn.Parameters)
                    //{
                    //    ValidateNode(param);
                    //}

                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a CollectionResourceFunctionCallNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionResourceFunctionCallNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.ItemType.Definition);
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //foreach (IEdmFunction function in nodeIn.Functions)
                    //{
                    //    validate(function);
                    //}

                    //foreach (QueryNode param in nodeIn.Parameters)
                    //{
                    //    ValidateNode(param);
                    //}

                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a CollectionFunctionCallNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionFunctionCallNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.ItemType.Definition);
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //foreach (IEdmFunction function in nodeIn.Functions)
                    //{
                    //    validate(function);
                    //}

                    //foreach (QueryNode param in nodeIn.Parameters)
                    //{
                    //    ValidateNode(param);
                    //}

                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleValueOpenPropertyAccessNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleValueOpenPropertyAccessNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleValuePropertyAccessNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleValuePropertyAccessNode nodeIn)
                {
                    var recommendation = nodeIn.Property.DeclaringType.FullTypeName() == "microsoft.graph.recommendation";
                    var resource = nodeIn.Property.DeclaringType.FullTypeName() == "microsoft.graph.recommendationResource";

                    var validation = nodeIn.Source.Accept(this);
                    return new Validation(
                        recommendation || validation.Recommendation,
                        resource || validation.Resource);
                }

                /// <summary>
                /// Visit a UnaryOperatorNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(UnaryOperatorNode nodeIn)
                {
                    //validate(nodeIn);
                    //ValidateNode(nodeIn.Operand);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a NamedFunctionParameterNode.
                /// </summary>
                /// <param name="nodeIn">The node to visit.</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(NamedFunctionParameterNode nodeIn)
                {
                    //validate(nodeIn);
                    //ValidateNode(nodeIn.Value);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a ParameterAliasNode
                /// </summary>
                /// <param name="nodeIn">The node to visit</param>
                /// <returns>The translated expression</returns>
                public override Validation Visit(ParameterAliasNode nodeIn)
                {
                    //// todo: do we even need to call for a parameter alias?
                    //validate(nodeIn);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SearchTermNode
                /// </summary>
                /// <param name="nodeIn">The node to visit</param>
                /// <returns>The translated expression</returns>
                public override Validation Visit(SearchTermNode nodeIn)
                {
                    //validate(nodeIn);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleComplexNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleComplexNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.Property);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a CollectionComplexNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(CollectionComplexNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.Property);
                    //validate(nodeIn.CollectionType.CollectionDefinition());
                    //validate(nodeIn.ItemType.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit a SingleValueCastNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(SingleValueCastNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit an AggregatedCollectionPropertyNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(AggregatedCollectionPropertyNode nodeIn)
                {
                    //validate(nodeIn);
                    //validate(nodeIn.Property);
                    //validate(nodeIn.TypeReference.Definition);
                    //return true;
                    throw new NotImplementedException();
                }

                /// <summary>
                /// Visit an InNode
                /// </summary>
                /// <param name="nodeIn">the node to visit</param>
                /// <returns>true, indicating that the node has been visited.</returns>
                public override Validation Visit(InNode nodeIn)
                {
                    //validate(nodeIn);
                    //ValidateNode(nodeIn.Left);
                    //ValidateNode(nodeIn.Right);
                    //return true;
                    throw new NotImplementedException();
                }
            }

            private struct Validation
            {
                public Validation(bool recommendation, bool resource)
                {
                    this.Recommendation = recommendation;
                    this.Resource = resource;
                }

                public bool Recommendation { get; }

                public bool Resource { get; }
            }
        }

        public interface IRequestProvider
        {
            string ProvideRequest(Request request);
        }

        public sealed class Request
        {
            private readonly string verb;

            private readonly Uri resource;

            private readonly string body;

            public Request(string verb, Uri resource, string body)
            {
                if (verb == null)
                {
                    throw new ArgumentNullException(nameof(verb));
                }

                if (verb.Length == 0)
                {
                    throw new ArgumentNullException(nameof(verb));
                }

                if (resource == null)
                {
                    throw new ArgumentNullException(nameof(resource));
                }

                if (body == null)
                {
                    throw new ArgumentNullException(nameof(body));
                }

                this.verb = verb;
                this.resource = resource;
                this.body = body;
            }

            public string Verb
            {
                get
                {
                    return this.verb;
                }
            }

            public Uri Resource
            {
                get
                {
                    return this.resource;
                }
            }

            public string Body
            {
                get
                {
                    return this.body;
                }
            }
        }
    }
}
