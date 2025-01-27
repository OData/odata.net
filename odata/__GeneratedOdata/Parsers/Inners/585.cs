namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx23ʺ_odataIdentifierParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._ʺx23ʺ_odataIdentifier> Instance { get; } = from _ʺx23ʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx23ʺParser.Instance
from _odataIdentifier_1 in __GeneratedOdata.Parsers.Rules._odataIdentifierParser.Instance
select new __GeneratedOdata.CstNodes.Inners._ʺx23ʺ_odataIdentifier(_ʺx23ʺ_1, _odataIdentifier_1);
    }
    
}
