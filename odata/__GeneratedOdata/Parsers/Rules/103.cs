namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _customNameParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._customName> Instance { get; } = from _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 in __GeneratedOdata.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance
from _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 in __GeneratedOdata.Parsers.Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many()
select new __GeneratedOdata.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1);
    }
    
}