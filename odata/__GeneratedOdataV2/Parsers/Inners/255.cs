namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fʺ_selectListPropertyParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_selectListProperty> Instance { get; } = from _ʺx2Fʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2FʺParser.Instance
from _selectListProperty_1 in __GeneratedOdataV2.Parsers.Rules._selectListPropertyParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._ʺx2Fʺ_selectListProperty(_ʺx2Fʺ_1, _selectListProperty_1);
    }
    
}
