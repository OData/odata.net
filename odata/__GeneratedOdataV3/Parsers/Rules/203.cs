namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _navigationPropertyInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri> Instance { get; } = (_singleNavPropInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri>(_collectionNavPropInJSONParser.Instance);
        
        public static class _singleNavPropInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON> Parse(IInput<char>? input)
                {
                    var _singleNavPropInJSON_1 = __GeneratedOdataV3.Parsers.Rules._singleNavPropInJSONParser.Instance.Parse(input);
if (!_singleNavPropInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._singleNavPropInJSON(_singleNavPropInJSON_1.Parsed), _singleNavPropInJSON_1.Remainder);
                }
            }
        }
        
        public static class _collectionNavPropInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON> Parse(IInput<char>? input)
                {
                    var _collectionNavPropInJSON_1 = __GeneratedOdataV3.Parsers.Rules._collectionNavPropInJSONParser.Instance.Parse(input);
if (!_collectionNavPropInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._navigationPropertyInUri._collectionNavPropInJSON(_collectionNavPropInJSON_1.Parsed), _collectionNavPropInJSON_1.Remainder);
                }
            }
        }
    }
    
}
