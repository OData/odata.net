namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _identifierLeadingCharacterParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter> Instance { get; } = (_ALPHAParser.Instance).Or<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter>(_ʺx5FʺParser.Instance);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter> Instance { get; } = _ALPHAParser.Instance;

        public static class _ALPHAParser
        {
            /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA> Instance { get; } = from _ALPHA_1 in __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA(_ALPHA_1);
            */
            //// PERF
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA> Instance { get; } = new Parser();

            private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA>
            {
                public IOutput<char, _identifierLeadingCharacter._ALPHA> Parse(IInput<char>? input)
                {
                    var _ALPHA_1 = __GeneratedOdata.Parsers.Rules._ALPHAParser.Instance.Parse(input);
                    return Output.Create(
                        true,
                        new __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ALPHA(_ALPHA_1.Parsed),
                        _ALPHA_1.Remainder);
                }
            }
        }
        
        public static class _ʺx5FʺParser
        {
            public static IParser<char, __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ> Instance { get; } = from _ʺx5Fʺ_1 in __GeneratedOdata.Parsers.Inners._ʺx5FʺParser.Instance
select new __GeneratedOdata.CstNodes.Rules._identifierLeadingCharacter._ʺx5Fʺ(_ʺx5Fʺ_1);
        }
    }
    
}
