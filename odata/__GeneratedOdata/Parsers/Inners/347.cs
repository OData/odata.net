namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _valueⲻseparator_complexInUriParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._valueⲻseparator_complexInUri> Instance { get; } = from _valueⲻseparator_1 in __GeneratedOdata.Parsers.Rules._valueⲻseparatorParser.Instance
from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdata.CstNodes.Inners._valueⲻseparator_complexInUri(_valueⲻseparator_1, _complexInUri_1);
    }
    
}
