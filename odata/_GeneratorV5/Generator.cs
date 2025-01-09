namespace _GeneratorV5
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Runtime.CompilerServices;
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
                                    this.FullyQualify(@class))),
                    new Namespace(
                        this.innerCstNodesNamespace,
                        innerNode
                            .NestedClasses
                            .Select(
                                @class =>
                                    this.FullyQualify(@class)))
                );
        }

        private Class FullyQualify(Class @class)
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
                                                this.QualifyType(parameter.Type),
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
                                                this.QualifyType(parameter.Type),
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
                                this.QualifyType(property.Type),
                                property.Name,
                                property.HasGet,
                                property.HasSet,
                                property.Initializer)));
        }

        private string QualifyType(string type)
        {
            var enumerableDelimiter = "IEnumerable<";
            if (type.StartsWith(enumerableDelimiter))
            {
                return $"System.Collections.Generic.IEnumerable<{QualifyType(type.Substring(enumerableDelimiter.Length))}";
            }

            var innersDelimiter = "Inners.";
            if (type.StartsWith(innersDelimiter))
            {
                return $"{this.innerCstNodesNamespace}.{type.Substring(innersDelimiter.Length)}";
            }

            return type;
        }
    }
}
