namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _unreservedⳆpctⲻencodedⳆsubⲻdelimsParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims> Instance { get; } = (_unreservedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_pctⲻencodedParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims>(_subⲻdelimsParser.Instance);
        
        public static class _unreservedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV4.Parsers.Rules._unreservedParser.Instance.Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._unreserved(_unreserved_1.Parsed), _unreserved_1.Remainder);
                }
            }
        }
        
        public static class _pctⲻencodedParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded> Parse(IInput<char>? input)
                {
                    var _pctⲻencoded_1 = __GeneratedOdataV4.Parsers.Rules._pctⲻencodedParser.Instance.Parse(input);
if (!_pctⲻencoded_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._pctⲻencoded(_pctⲻencoded_1.Parsed), _pctⲻencoded_1.Remainder);
                }
            }
        }
        
        public static class _subⲻdelimsParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims> Parse(IInput<char>? input)
                {
                    var _subⲻdelims_1 = __GeneratedOdataV4.Parsers.Rules._subⲻdelimsParser.Instance.Parse(input);
if (!_subⲻdelims_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._unreservedⳆpctⲻencodedⳆsubⲻdelims._subⲻdelims(_subⲻdelims_1.Parsed), _subⲻdelims_1.Remainder);
                }
            }
        }
    }
    
}
