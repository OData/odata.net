namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _complexPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._complexPropertyInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _complexProperty_1 in __GeneratedOdataV2.Parsers.Rules._complexPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _complexInUri_1 in __GeneratedOdataV2.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._complexPropertyInUri(_quotationⲻmark_1, _complexProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _complexInUri_1);
    }
    
}
