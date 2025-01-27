namespace __GeneratedOdata.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataUriParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._odataUri> Instance { get; } = from _serviceRoot_1 in __GeneratedOdata.Parsers.Rules._serviceRootParser.Instance
from _odataRelativeUri_1 in __GeneratedOdata.Parsers.Rules._odataRelativeUriParser.Instance.Optional()
select new __GeneratedOdata.CstNodes.Rules._odataUri(_serviceRoot_1, _odataRelativeUri_1.GetOrElse(null));
    }
    
}
