namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavPropInJSONParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._collectionNavPropInJSON> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _entityColNavigationProperty_1 in __GeneratedOdataV2.Parsers.Rules._entityColNavigationPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdataV2.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdataV2.Parsers.Rules._nameⲻseparatorParser.Instance
from _rootExprCol_1 in __GeneratedOdataV2.Parsers.Rules._rootExprColParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._collectionNavPropInJSON(_quotationⲻmark_1, _entityColNavigationProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _rootExprCol_1);
    }
    
}
