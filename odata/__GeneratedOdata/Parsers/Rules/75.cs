namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _topParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._top> Instance { get; } = from _Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃParser.Instance
from _EQ_1 in __GeneratedOdata.Parsers.Rules._EQParser.Instance
from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._top(_Ⲥʺx24x74x6Fx70ʺⳆʺx74x6Fx70ʺↃ_1, _EQ_1, _DIGIT_1);
    }
    
}
