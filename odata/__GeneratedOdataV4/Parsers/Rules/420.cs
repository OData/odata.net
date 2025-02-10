namespace __GeneratedOdataV4.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _ALPHAParser
    {
        public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._ALPHA> Instance2 { get; } = new Parser2();

        private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._ALPHA>
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            public __GeneratedOdataV4.CstNodes.Rules._ALPHA Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
            {
                var _val = _Ⰳx61ⲻ7AParser.Instance2.Parse(input, start, out newStart);

                return default;
            }
        }

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
            public static CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Instance2 { get; } = new Parser2();

            private sealed class Parser2 : CombinatorParsingV3.IParser<char, CombinatorParsingV3.ParserExtensions.StringAdapter, __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A>
            {
                [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
                public __GeneratedOdataV4.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A Parse(CombinatorParsingV3.ParserExtensions.StringAdapter input, int start, out int newStart)
                {
                    var _Ⰳx61ⲻ7A_1 = __GeneratedOdataV4.Parsers.Inners._Ⰳx61ⲻ7AParser.Instance2.Parse(input, start, out newStart);

                    return default;
                }
            }

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
