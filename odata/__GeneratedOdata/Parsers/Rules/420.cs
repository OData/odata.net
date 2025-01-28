namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _ALPHAParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA> Instance { get; } = (_Ⰳx41ⲻ5AParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._ALPHA>(_Ⰳx61ⲻ7AParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA> Instance { get; } = _Ⰳx61ⲻ7AParser.Instance;

        public static class _Ⰳx41ⲻ5AParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A> Instance { get; } = from _Ⰳx41ⲻ5A_1 in __GeneratedOdata.Parsers.Inners._Ⰳx41ⲻ5AParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx41ⲻ5A(_Ⰳx41ⲻ5A_1);
        }
        
        public static class _Ⰳx61ⲻ7AParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Instance { get; } = from _Ⰳx61ⲻ7A_1 in __GeneratedOdata.Parsers.Inners._Ⰳx61ⲻ7AParser.Instance
select new __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(_Ⰳx61ⲻ7A_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A>
            {
                public IOutput<char, _ALPHA._Ⰳx61ⲻ7A> Parse(IInput<char>? input)
                {
                    var _Ⰳx61ⲻ7A_1 = __GeneratedOdata.Parsers.Inners._Ⰳx61ⲻ7AParser.Instance.Parse(input);
                    if (!_Ⰳx61ⲻ7A_1.Success)
                    {
                        return Output.Create(
                            false,
                            default(_ALPHA._Ⰳx61ⲻ7A)!,
                            input);
                    }

                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Rules._ALPHA._Ⰳx61ⲻ7A(_Ⰳx61ⲻ7A_1.Parsed),
                        _Ⰳx61ⲻ7A_1.Remainder);
                }
            }
        }
    }
    
}
