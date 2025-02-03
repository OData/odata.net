namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _selectPathParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._selectPath> Instance { get; } = from _ⲤcomplexPropertyⳆcomplexColPropertyↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤcomplexPropertyⳆcomplexColPropertyↃParser.Instance
from _ʺx2Fʺ_qualifiedComplexTypeName_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2Fʺ_qualifiedComplexTypeNameParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._selectPath(_ⲤcomplexPropertyⳆcomplexColPropertyↃ_1, _ʺx2Fʺ_qualifiedComplexTypeName_1.GetOrElse(null));
    }
    
}
