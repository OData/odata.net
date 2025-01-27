namespace __GeneratedOdata.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUriParser
    {
        public static IParser<char, __GeneratedOdata.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _complexColProperty_1 in __GeneratedOdata.Parsers.Rules._complexColPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdata.Parsers.Rules._nameⲻseparatorParser.Instance
from _complexColInUri_1 in __GeneratedOdata.Parsers.Rules._complexColInUriParser.Instance
select new __GeneratedOdata.CstNodes.Inners._quotationⲻmark_complexColProperty_quotationⲻmark_nameⲻseparator_complexColInUri(_quotationⲻmark_1, _complexColProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _complexColInUri_1);
    }
    
}
