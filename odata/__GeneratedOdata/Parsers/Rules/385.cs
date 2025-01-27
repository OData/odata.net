namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _schemeParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._scheme> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
from _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1 in Inners._ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._scheme(_ALPHA_1, _ⲤALPHAⳆDIGITⳆʺx2BʺⳆʺx2DʺⳆʺx2EʺↃ_1);
    }
    
}
