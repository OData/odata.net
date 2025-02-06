namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻJSONⲻspecialParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial> Instance { get; } = (_SPParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7BʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx7DʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5BʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial>(_ʺx5DʺParser.Instance);
        
        public static class _SPParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._SP> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._SP>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._SP> Parse(IInput<char>? input)
                {
                    var _SP_1 = __GeneratedOdataV4.Parsers.Rules._SPParser.Instance.Parse(input);
if (!_SP_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._SP)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._SP.Instance, _SP_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx7BʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx7Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx7BʺParser.Instance.Parse(input);
if (!_ʺx7Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Bʺ.Instance, _ʺx7Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx7DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx7Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx7DʺParser.Instance.Parse(input);
if (!_ʺx7Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx7Dʺ.Instance, _ʺx7Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5BʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Bʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5BʺParser.Instance.Parse(input);
if (!_ʺx5Bʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Bʺ.Instance, _ʺx5Bʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx5Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx5DʺParser.Instance.Parse(input);
if (!_ʺx5Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻJSONⲻspecial._ʺx5Dʺ.Instance, _ʺx5Dʺ_1.Remainder);
                }
            }
        }
    }
    
}
