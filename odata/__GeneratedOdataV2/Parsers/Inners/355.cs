namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _primitiveColProperty_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _primitiveColInUri_1 in __GeneratedOdataV2.Parsers.Rules._primitiveColInUriParser.Instance
select new __GeneratedOdataV2.CstNodes.Inners._quotationⲻmark_primitiveColProperty_quotationⲻmark_nameⲻseparator_primitiveColInUri(_quotationⲻmark_1, _primitiveColProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _primitiveColInUri_1);
    }
    
}
