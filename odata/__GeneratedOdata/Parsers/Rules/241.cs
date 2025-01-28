namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _identifierCharacterParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter>(_ʺx5FʺParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter>(_DIGITParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter> Instance { get; } = _ALPHAParser.Instance;

        public static class _ALPHAParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA>
            {
                public IOutput<char, _identifierCharacter._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance.Parse(input);
                    if (!_ALPHA_1.Success)
                    {
                        return Output.Create(
                            false,
                            default(__GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA)!,
                            input);
                    }

                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Rules._identifierCharacter._ALPHA(_ALPHA_1.Parsed),
                        _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
        
        public static class _DIGITParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierCharacter._DIGIT> Instance { get; } = from _DIGIT_1 in __GeneratedOdata.Parsers.Rules._DIGITParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierCharacter._DIGIT(_DIGIT_1);
        }
    }
    
}
