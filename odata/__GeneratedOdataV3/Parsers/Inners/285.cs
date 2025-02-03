namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx61x6Ex79ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ> Parse(IInput<char>? input)
            {
                var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(input);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x61_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV3.Parsers.Inners._x79Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx61x6Ex79ʺ.Instance, _x79_1.Remainder);
            }
        }
    }
    
}
