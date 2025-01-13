namespace __GeneratedOdata.Parsers.Inners
{
    using Sprache;
    
    public static class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdata.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _contextPropertyPath_1 in __GeneratedOdata.Parsers.Rules._contextPropertyPathParser.Instance
select new __GeneratedOdata.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _ʺx2Fʺ_1, _contextPropertyPath_1);
    }
    
}
