namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _boundFunctionCallNoParensParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens> Instance { get; } = (_namespace_ʺx2Eʺ_entityFunctionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_entityColFunctionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_complexFunctionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_complexColFunctionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_primitiveFunctionParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens>(_namespace_ʺx2Eʺ_primitiveColFunctionParser.Instance);
        
        public static class _namespace_ʺx2Eʺ_entityFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _entityFunction_1 in __GeneratedOdata.Parsers.Rules._entityFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityFunction(_namespace_1, _ʺx2Eʺ_1, _entityFunction_1);
        }
        
        public static class _namespace_ʺx2Eʺ_entityColFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _entityColFunction_1 in __GeneratedOdata.Parsers.Rules._entityColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_entityColFunction(_namespace_1, _ʺx2Eʺ_1, _entityColFunction_1);
        }
        
        public static class _namespace_ʺx2Eʺ_complexFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _complexFunction_1 in __GeneratedOdata.Parsers.Rules._complexFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexFunction(_namespace_1, _ʺx2Eʺ_1, _complexFunction_1);
        }
        
        public static class _namespace_ʺx2Eʺ_complexColFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _complexColFunction_1 in __GeneratedOdata.Parsers.Rules._complexColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_complexColFunction(_namespace_1, _ʺx2Eʺ_1, _complexColFunction_1);
        }
        
        public static class _namespace_ʺx2Eʺ_primitiveFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _primitiveFunction_1 in __GeneratedOdata.Parsers.Rules._primitiveFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveFunction(_namespace_1, _ʺx2Eʺ_1, _primitiveFunction_1);
        }
        
        public static class _namespace_ʺx2Eʺ_primitiveColFunctionParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction> Instance { get; } = from _namespace_1 in __GeneratedOdata.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2EʺParser.Instance
from _primitiveColFunction_1 in __GeneratedOdata.Parsers.Rules._primitiveColFunctionParser.Instance
select new __GeneratedOdata.CstNodes.Rules._boundFunctionCallNoParens._namespace_ʺx2Eʺ_primitiveColFunction(_namespace_1, _ʺx2Eʺ_1, _primitiveColFunction_1);
        }
    }
    
}
