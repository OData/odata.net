namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx65x71ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ> Parse(IInput<char>? input)
            {
                var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(input);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ)!, input);
}

var _x71_1 = __GeneratedOdataV3.Parsers.Inners._x71Parser.Instance.Parse(_x65_1.Remainder);
if (!_x71_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx65x71ʺ.Instance, _x71_1.Remainder);
            }
        }
    }
    
}
