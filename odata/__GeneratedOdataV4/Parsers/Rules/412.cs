namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_otherⲻdelimsParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3AʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx40ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx2FʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3FʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx24ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx27ʺParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP>(_ʺx3DʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV4.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded> Parse(IInput<char>? input)
                {
                    var _pctⲻencoded_1 = __GeneratedOdataV4.Parsers.Rules._pctⲻencodedParser.Instance.Parse(input);
if (!_pctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._pctⲻencoded(_pctⲻencoded_1.Parsed), _pctⲻencoded_1.Remainder);
                }
            }
        }
        
        public static class _otherⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims> Parse(IInput<char>? input)
                {
                    var _otherⲻdelims_1 = __GeneratedOdataV4.Parsers.Rules._otherⲻdelimsParser.Instance.Parse(input);
if (!_otherⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._otherⲻdelims(_otherⲻdelims_1.Parsed), _otherⲻdelims_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx40ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx40ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx40ʺParser.Instance.Parse(input);
if (!_ʺx40ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx40ʺ.Instance, _ʺx40ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx2Fʺ.Instance, _ʺx2Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3FʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3FʺParser.Instance.Parse(input);
if (!_ʺx3Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Fʺ.Instance, _ʺx3Fʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx24ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx24ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx24ʺParser.Instance.Parse(input);
if (!_ʺx24ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx24ʺ.Instance, _ʺx24ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx27ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx27ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx27ʺParser.Instance.Parse(input);
if (!_ʺx27ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx27ʺ.Instance, _ʺx27ʺ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3DʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3DʺParser.Instance.Parse(input);
if (!_ʺx3Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Rules._qcharⲻnoⲻAMP._ʺx3Dʺ.Instance, _ʺx3Dʺ_1.Remainder);
                }
            }
        }
    }
    
}
