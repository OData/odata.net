namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _batchOptionsParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._batchOptions> Instance { get; } = from _batchOption_1 in __GeneratedOdata.Parsers.Rules._batchOptionParser.Instance
from _Ⲥʺx26ʺ_batchOptionↃ_1 in __GeneratedOdata.Parsers.Inners._Ⲥʺx26ʺ_batchOptionↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._batchOptions(_batchOption_1, _Ⲥʺx26ʺ_batchOptionↃ_1);
    }
    
}
