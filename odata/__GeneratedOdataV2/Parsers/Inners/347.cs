namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_complexInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_complexInUri> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._valueⲻseparatorParser.Instance
from _complexInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._valueⲻseparator_complexInUri(_valueⲻseparator_1, _complexInUri_1);
    }
    
}
