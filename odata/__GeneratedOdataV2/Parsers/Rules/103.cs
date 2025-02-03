namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _customNameParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._customName> Instance { get; } = from _qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1 in __GeneratedOdataV2.Parsers.Rules._qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLARParser.Instance
from _ⲤqcharⲻnoⲻAMPⲻEQↃ_1 in Inners._ⲤqcharⲻnoⲻAMPⲻEQↃParser.Instance.Many()
select new __GeneratedOdataV2.CstNodes.Rules._customName(_qcharⲻnoⲻAMPⲻEQⲻATⲻDOLLAR_1, _ⲤqcharⲻnoⲻAMPⲻEQↃ_1);
    }
    
}
