namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx38ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx38ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx38ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx38ʺ> Parse(IInput<char>? input)
            {
                var _x38_1 = __GeneratedOdataV4.Parsers.Inners._x38Parser.Instance.Parse(input);
if (!_x38_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx38ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx38ʺ.Instance, _x38_1.Remainder);
            }
        }
    }
    
}
