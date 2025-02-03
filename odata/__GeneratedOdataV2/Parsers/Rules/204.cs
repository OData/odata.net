namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavPropInJSONParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._singleNavPropInJSON> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _entityNavigationProperty_1 in __GeneratedOdataV2.Parsers.Rules._entityNavigationPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _rootExpr_1 in __GeneratedOdataV2.Parsers.Rules._rootExprParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._singleNavPropInJSON(_quotationⲻmark_1, _entityNavigationProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _rootExpr_1);
    }
    
}
