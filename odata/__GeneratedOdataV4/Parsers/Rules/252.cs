namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _navigationPropertyParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty> Instance { get; } = (_entityNavigationPropertyParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty>(_entityColNavigationPropertyParser.Instance);
        
        public static class _entityNavigationPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityNavigationProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityNavigationProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityNavigationProperty> Parse(IInput<char>? input)
                {
                    var _entityNavigationProperty_1 = __GeneratedOdataV4.Parsers.Rules._entityNavigationPropertyParser.Instance.Parse(input);
if (!_entityNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityNavigationProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityNavigationProperty(_entityNavigationProperty_1.Parsed), _entityNavigationProperty_1.Remainder);
                }
            }
        }
        
        public static class _entityColNavigationPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityColNavigationProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityColNavigationProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityColNavigationProperty> Parse(IInput<char>? input)
                {
                    var _entityColNavigationProperty_1 = __GeneratedOdataV4.Parsers.Rules._entityColNavigationPropertyParser.Instance.Parse(input);
if (!_entityColNavigationProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityColNavigationProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._navigationProperty._entityColNavigationProperty(_entityColNavigationProperty_1.Parsed), _entityColNavigationProperty_1.Remainder);
                }
            }
        }
    }
    
}
