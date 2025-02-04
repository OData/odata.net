namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _collectionNavPropInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON> Parse(IInput<char>? input)
            {
                var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON)!, input);
}

var _entityColNavigationProperty_1 = __GeneratedOdataV3.Parsers.Rules._entityColNavigationPropertyParser.Instance.Parse(_quotationⲻmark_1.Remainder);
if (!_entityColNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON)!, input);
}

var _quotationⲻmark_2 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(_entityColNavigationProperty_1.Remainder);
if (!_quotationⲻmark_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON)!, input);
}

var _nameⲻseparator_1 = __GeneratedOdataV3.Parsers.Rules._nameⲻseparatorParser.Instance.Parse(_quotationⲻmark_2.Remainder);
if (!_nameⲻseparator_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON)!, input);
}

var _rootExprCol_1 = __GeneratedOdataV3.Parsers.Rules._rootExprColParser.Instance.Parse(_nameⲻseparator_1.Remainder);
if (!_rootExprCol_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._collectionNavPropInJSON(_quotationⲻmark_1.Parsed, _entityColNavigationProperty_1.Parsed, _quotationⲻmark_2.Parsed, _nameⲻseparator_1.Parsed, _rootExprCol_1.Parsed), _rootExprCol_1.Remainder);
            }
        }
    }
    
}
