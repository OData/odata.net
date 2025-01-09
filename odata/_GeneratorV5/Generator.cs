namespace _GeneratorV5
{
    using System.Collections.Generic;
    using System.Linq;

    using _GeneratorV4.Abnf.CstNodes;
    using AbnfParserGenerator;

    public sealed class Generator
    {
        private readonly string ruleCstNodesNamespace;
        private readonly string innerCstNodesNamespace;

        private readonly _GeneratorV4.Generator generator;

        public Generator(string ruleCstNodesNamespace, string innerCstNodesNamespace)
        {

            this.ruleCstNodesNamespace = ruleCstNodesNamespace;
            this.innerCstNodesNamespace = innerCstNodesNamespace;
            this.generator = new _GeneratorV4.Generator(ruleCstNodesNamespace);
        }

        public (Namespace RuleCstNodes, Namespace InnerCstNodes) Generate(_rulelist rulelist)
        {
            var cstNodes = this
                .generator
                .Generate(rulelist).ToList();
            var ruleNodes = cstNodes.Where(node => node.Name != "Inners");
            var innerNode = cstNodes.Where(node => node.Name == "Inners").Single();

            return 
                (
                    new Namespace(
                        this.ruleCstNodesNamespace, 
                        ruleNodes
                            .Select(
                                @class =>
                                    FullyQualify(@class))),
                    new Namespace(
                        this.innerCstNodesNamespace,
                        innerNode
                            .NestedClasses
                            .Select(
                                @class =>
                                    FullyQualify(@class)))
                );
        }

        private static Class FullyQualify(Class @class)
        {
            return new Class(
                @class.AccessModifier,
                @class.ClassModifier,
                @class.Name,
                @class.GenericTypeParameters,
                @class.BaseType, //// TODO does this ever need qualifying?
                @class
                    .Constructors
                    .Select(
                        constructor =>
                            new ConstructorDefinition(
                                constructor.AccessModifier,
                                constructor
                                    .Parameters
                                    .Select(
                                        parameter =>
                                            new MethodParameter(
                                                QualifyType(parameter.Type),
                                                parameter.Name)),
                                constructor.Body)),
                @class
                    .Methods
                    .Select(
                        method =>
                            new MethodDefinition(
                                method.AccessModifier,
                                method.ClassModifier,
                                method.IsOverride,
                                method.ReturnType, //// TODO does this need qualifying?
                                method.GenericTypeParameters,
                                method.Name,
                                method
                                    .Parameters
                                    .Select(
                                        parameter =>
                                            new MethodParameter(
                                                QualifyType(parameter.Type),
                                                parameter.Name)),
                                method.Body)),
                @class
                    .NestedClasses
                    .Select(
                        nestedClass =>
                            FullyQualify(nestedClass)),
                @class
                    .Properties
                    .Select(
                        property =>
                            new PropertyDefinition(
                                property.AccessModifier,
                                property.IsStatic,
                                QualifyType(property.Type),
                                property.Name,
                                property.HasGet,
                                property.HasSet,
                                property.Initializer)));
        }

        private static string QualifyType(string type)
        {
            if (type.StartsWith("IEnumerable<"))
            {
                return $"System.Collections.Generic.{type}";
            }

            return type;
        }
    }
}
