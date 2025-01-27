namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _topParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._top> Instance { get; } = from _Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Repeat(1, null)
select new __GeneratedOdata.CstNodes.Rules._top(_Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1, _EQ_1, new __GeneratedOdata.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdata.CstNodes.Rules._DIGIT>(_DIGIT_1));
    }
    
}
