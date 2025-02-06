namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx75x72x6CʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ> Parse(IInput<char>? input)
            {
                var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(input);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x75_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x72_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx75x72x6Cʺ.Instance, _x6C_1.Remainder);
            }
        }
    }
    
}
