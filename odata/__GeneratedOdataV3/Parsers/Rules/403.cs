namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pcharParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pchar>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pchar>(_subⲻdelimsParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pchar>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pchar>(_ʺx40ʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pchar._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV3.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pchar._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pchar._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._pctⲻencoded> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._pctⲻencoded>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pchar._pctⲻencoded> Parse(IInput<char>? input)
                {
                    var _pctⲻencoded_1 = __GeneratedOdataV3.Parsers.Rules._pctⲻencodedParser.Instance.Parse(input);
if (!_pctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pchar._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pchar._pctⲻencoded(_pctⲻencoded_1.Parsed), _pctⲻencoded_1.Remainder);
                }
            }
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._subⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._subⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pchar._subⲻdelims> Parse(IInput<char>? input)
                {
                    var _subⲻdelims_1 = __GeneratedOdataV3.Parsers.Rules._subⲻdelimsParser.Instance.Parse(input);
if (!_subⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pchar._subⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pchar._subⲻdelims(_subⲻdelims_1.Parsed), _subⲻdelims_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pchar._ʺx3Aʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx3Aʺ(_ʺx3Aʺ_1.Parsed), _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx40ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx40ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx40ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx40ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx40ʺParser.Instance.Parse(input);
if (!_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pchar._ʺx40ʺ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pchar._ʺx40ʺ(_ʺx40ʺ_1.Parsed), _ʺx40ʺ_1.Remainder);
                }
            }
        }
    }
    
}
