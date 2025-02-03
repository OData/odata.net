namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPathParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath> Instance { get; } = from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _contextPropertyPath_1 in __GeneratedOdataV2.Parsers.Rules._contextPropertyPathParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._꘡ʺx2Fʺ_qualifiedComplexTypeName꘡_ʺx2Fʺ_contextPropertyPath(_ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null), _ʺx2Fʺ_1, _contextPropertyPath_1);
    }
    
}
