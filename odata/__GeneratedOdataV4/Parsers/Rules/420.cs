namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ALPHAParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA> Instance { get; } = (_Ⰳx41ⲻ5AParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA>(_Ⰳx61ⲻ7AParser.Instance);
        
        public static class _Ⰳx41ⲻ5AParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A> Parse(IInput<char>? input)
                {
                    var _Ⰳx41ⲻ5A_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx41ⲻ5AParser.Instance.Parse(input);
if (!_Ⰳx41ⲻ5A_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A(_Ⰳx41ⲻ5A_1.Parsed), _Ⰳx41ⲻ5A_1.Remainder);
                }
            }
        }
        
        public static class _Ⰳx61ⲻ7AParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Parse(IInput<char>? input)
                {
                    var _Ⰳx61ⲻ7A_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx61ⲻ7AParser.Instance.Parse(input);
if (!_Ⰳx61ⲻ7A_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(_Ⰳx61ⲻ7A_1.Parsed), _Ⰳx61ⲻ7A_1.Remainder);
                }
            }
        }
    }
    
}
