namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_selectListPropertyParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_selectListProperty> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx2FʺParser.Instance
from _selectListProperty_1 in __GeneratedOdata.Parsers.Rules._selectListPropertyParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx2Fʺ_selectListProperty(_ʺx2Fʺ_1, _selectListProperty_1);
    }
    
}
