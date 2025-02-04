namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ⲤcontainmentNavigationↃParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcontainmentNavigationↃ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcontainmentNavigationↃ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ⲤcontainmentNavigationↃ> Parse(IInput<char>? input)
            {
                var _containmentNavigation_1 = __GeneratedOdataV3.Parsers.Rules._containmentNavigationParser.Instance.Parse(input);
if (!_containmentNavigation_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ⲤcontainmentNavigationↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._ⲤcontainmentNavigationↃ(_containmentNavigation_1.Parsed), _containmentNavigation_1.Remainder);
            }
        }
    }
    
}
