namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON> Instance { get; } = (_stringInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_numberInJSONParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx74x72x75x65ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx66x61x6Cx73x65ʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON>(_ʺx6Ex75x6Cx6CʺParser.Instance);
        
        public static class _stringInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON> Parse(IInput<char>? input)
                {
                    var _stringInJSON_1 = __GeneratedOdataV3.Parsers.Rules._stringInJSONParser.Instance.Parse(input);
if (!_stringInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._stringInJSON(_stringInJSON_1.Parsed), _stringInJSON_1.Remainder);
                }
            }
        }
        
        public static class _numberInJSONParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON> Parse(IInput<char>? input)
                {
                    var _numberInJSON_1 = __GeneratedOdataV3.Parsers.Rules._numberInJSONParser.Instance.Parse(input);
if (!_numberInJSON_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._numberInJSON(_numberInJSON_1.Parsed), _numberInJSON_1.Remainder);
                }
            }
        }
        
        public static class _ʺx74x72x75x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx74x72x75x65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx74x72x75x65ʺParser.Instance.Parse(input);
if (!_ʺx74x72x75x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx74x72x75x65ʺ(_ʺx74x72x75x65ʺ_1.Parsed), _ʺx74x72x75x65ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx66x61x6Cx73x65ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx66x61x6Cx73x65ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx66x61x6Cx73x65ʺParser.Instance.Parse(input);
if (!_ʺx66x61x6Cx73x65ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx66x61x6Cx73x65ʺ(_ʺx66x61x6Cx73x65ʺ_1.Parsed), _ʺx66x61x6Cx73x65ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx6Ex75x6Cx6CʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ> Parse(IInput<char>? input)
                {
                    var _ʺx6Ex75x6Cx6Cʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx6Ex75x6Cx6CʺParser.Instance.Parse(input);
if (!_ʺx6Ex75x6Cx6Cʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._primitiveLiteralInJSON._ʺx6Ex75x6Cx6Cʺ(_ʺx6Ex75x6Cx6Cʺ_1.Parsed), _ʺx6Ex75x6Cx6Cʺ_1.Remainder);
                }
            }
        }
    }
    
}
