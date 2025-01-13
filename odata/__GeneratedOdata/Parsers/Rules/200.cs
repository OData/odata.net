namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _complexPropertyInUriParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._complexPropertyInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _complexProperty_1 in __GeneratedOdata.Parsers.Rules._complexPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdata.Parsers.Rules._nameⲻseparatorParser.Instance
from _complexInUri_1 in __GeneratedOdata.Parsers.Rules._complexInUriParser.Instance
select new __GeneratedOdata.CstNodes.Rules._complexPropertyInUri(_quotationⲻmark_1, _complexProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _complexInUri_1);
    }
    
}
