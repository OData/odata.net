namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _odataUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._odataUri> Instance { get; } = from _serviceRoot_1 in __GeneratedOdataV2.Parsers.Rules._serviceRootParser.Instance
from _odataRelativeUri_1 in __GeneratedOdataV2.Parsers.Rules._odataRelativeUriParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._odataUri(_serviceRoot_1, _odataRelativeUri_1.GetOrElse(null));
    }
    
}
