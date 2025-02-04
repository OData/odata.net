namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx77ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx77ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx77ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx77ʺ> Parse(IInput<char>? input)
            {
                var _x77_1 = __GeneratedOdataV3.Parsers.Inners._x77Parser.Instance.Parse(input);
if (!_x77_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx77ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx77ʺ.Instance, _x77_1.Remainder);
            }
        }
    }
    
}
