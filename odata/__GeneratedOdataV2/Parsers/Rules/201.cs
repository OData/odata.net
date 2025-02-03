namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _annotationInUriParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._annotationInUri> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _AT_1 in __GeneratedOdataV2.Parsers.Rules._ATParser.Instance
from _namespace_1 in __GeneratedOdataV2.Parsers.Rules._namespaceParser.Instance
from _ʺx2Eʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx2EʺParser.Instance
from _termName_1 in __GeneratedOdataV2.Parsers.Rules._termNameParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1 in __GeneratedOdataV2.Parsers.Inners._ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._annotationInUri(_quotationⲻmark_1, _AT_1, _namespace_1, _ʺx2Eʺ_1, _termName_1, _quotationⲻmark_2, _nameⲻseparator_1, _ⲤcomplexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriↃ_1);
    }
    
}
