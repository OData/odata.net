namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _singleNavPropInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON)!, input);
}

var _entityNavigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._entityNavigationPropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_entityNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_entityNavigationProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON)!, input);
}

var _rootExpr_1 = __GeneratedOdataV3.Parsers.Rules._rootExprParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_rootExpr_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._singleNavPropInJSON(_quotationⲻmark_1.Parsed, _entityNavigationProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed,  _rootExpr_1.Parsed), _rootExpr_1.Remainder);
            }
        }
    }
    
}
