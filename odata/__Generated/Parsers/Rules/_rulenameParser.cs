namespace __Generated.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _rulenameParser
    {
        public static IParser<char, __Generated.CstNodes.Rules._rulename> Instance { get; } = from _ALPHA_1 in __Generated.Parsers.Rules._ALPHAParser.Instance
from _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1 in Inners._ⲤALPHAⳆDIGITⳆʺx2DʺↃParser.Instance.Many()
select new __Generated.CstNodes.Rules._rulename(_ALPHA_1, _ⲤALPHAⳆDIGITⳆʺx2DʺↃ_1);
    }
    
}
