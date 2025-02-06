namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _unreservedⳆsubⲻdelimsⳆʺx3AʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ>(_subⲻdelimsParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ>(_ʺx3AʺParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV4.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims> Parse(IInput<char>? input)
                {
                    var _subⲻdelims_1 = __GeneratedOdataV4.Parsers.Rules._subⲻdelimsParser.Instance.Parse(input);
if (!_subⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._subⲻdelims(_subⲻdelims_1.Parsed), _subⲻdelims_1.Remainder);
                }
            }
        }
        
        public static class _ʺx3AʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ> Parse(IInput<char>? input)
                {
                    var _ʺx3Aʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx3AʺParser.Instance.Parse(input);
if (!_ʺx3Aʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆsubⲻdelimsⳆʺx3Aʺ._ʺx3Aʺ.Instance, _ʺx3Aʺ_1.Remainder);
                }
            }
        }
    }
    
}
