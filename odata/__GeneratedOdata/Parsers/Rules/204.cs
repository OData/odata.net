namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _singleNavPropInJSONParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._singleNavPropInJSON> Instance { get; } = from _quotationⲻmark_1 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _entityNavigationProperty_1 in __GeneratedOdata.Parsers.Rules._entityNavigationPropertyParser.Instance
from _quotationⲻmark_2 in __GeneratedOdata.Parsers.Rules._quotationⲻmarkParser.Instance
from _nameⲻseparator_1 in __GeneratedOdata.Parsers.Rules._nameⲻseparatorParser.Instance
from _rootExpr_1 in __GeneratedOdata.Parsers.Rules._rootExprParser.Instance
select new __GeneratedOdata.CstNodes.Rules._singleNavPropInJSON(_quotationⲻmark_1, _entityNavigationProperty_1, _quotationⲻmark_2, _nameⲻseparator_1, _rootExpr_1);
    }
    
}
