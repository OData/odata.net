namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _subⲻdelimsParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims> Instance { get; } = (_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims>(_ʺx26ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims>(_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims>(_ʺx3DʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims>(_otherⲻdelimsParser.Instance);
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx24ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx24ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx24ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24ʺParser.Instance.Parse(input);
if (!_ʺx24ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx24ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx24ʺ.Instance, _ʺx24ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx26ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx26ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx26ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx26ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx26ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx26ʺParser.Instance.Parse(input);
if (!_ʺx26ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx26ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx26ʺ.Instance, _ʺx26ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx27ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx27ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx27ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx27ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx27ʺParser.Instance.Parse(input);
if (!_ʺx27ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx27ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx27ʺ.Instance, _ʺx27ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx3Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx3Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx3Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3DʺParser.Instance.Parse(input);
if (!_ʺx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx3Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._ʺx3Dʺ.Instance, _ʺx3Dʺ_1.Remainder);
                }
            }
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._otherⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._otherⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._otherⲻdelims> Parse(IInput<char>? input)
                {
                    var _otherⲻdelims_1 = __GeneratedOdataV4.Parsers.Rules._otherⲻdelimsParser.Instance.Parse(input);
if (!_otherⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._subⲻdelims._otherⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._subⲻdelims._otherⲻdelims(_otherⲻdelims_1.Parsed), _otherⲻdelims_1.Remainder);
                }
            }
        }
    }
    
}
