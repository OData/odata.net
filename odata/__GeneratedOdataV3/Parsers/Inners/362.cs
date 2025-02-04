namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUriParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri> Instance { get; } = (_complexInUriParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_complexColInUriParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveLiteralInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri>(_primitiveColInUriParser.Instance);
        
        public static class _complexInUriParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri> Parse(IInput<char>? input)
                {
                    var _complexInUri_1 = __GeneratedOdataV3.Parsers.Rules._complexInUriParser.Instance.Parse(input);
if (!_complexInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexInUri(_complexInUri_1.Parsed), _complexInUri_1.Remainder);
                }
            }
        }
        
        public static class _complexColInUriParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri> Parse(IInput<char>? input)
                {
                    var _complexColInUri_1 = __GeneratedOdataV3.Parsers.Rules._complexColInUriParser.Instance.Parse(input);
if (!_complexColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._complexColInUri(_complexColInUri_1.Parsed), _complexColInUri_1.Remainder);
                }
            }
        }
        
        public static class _primitiveLiteralInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON> Parse(IInput<char>? input)
                {
                    var _primitiveLiteralInJSON_1 = __GeneratedOdataV3.Parsers.Rules._primitiveLiteralInJSONParser.Instance.Parse(input);
if (!_primitiveLiteralInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveLiteralInJSON(_primitiveLiteralInJSON_1.Parsed), _primitiveLiteralInJSON_1.Remainder);
                }
            }
        }
        
        public static class _primitiveColInUriParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri> Parse(IInput<char>? input)
                {
                    var _primitiveColInUri_1 = __GeneratedOdataV3.Parsers.Rules._primitiveColInUriParser.Instance.Parse(input);
if (!_primitiveColInUri_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._complexInUriⳆcomplexColInUriⳆprimitiveLiteralInJSONⳆprimitiveColInUri._primitiveColInUri(_primitiveColInUri_1.Parsed), _primitiveColInUri_1.Remainder);
                }
            }
        }
    }
    
}
