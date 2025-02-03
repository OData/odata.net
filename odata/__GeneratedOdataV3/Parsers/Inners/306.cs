namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x65x63x6Fx6Ex64ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x65_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x63_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV3.Parsers.Inners._x64Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx73x65x63x6Fx6Ex64ʺ.Instance, _x64_1.Remainder);
            }
        }
    }
    
}
