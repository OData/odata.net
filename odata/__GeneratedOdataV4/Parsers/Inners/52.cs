namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx2Fx24x65x61x63x68ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ> Parse(IInput<char>? input)
            {
                var _x2F_1 = __GeneratedOdataV4.Parsers.Inners._x2FParser.Instance.Parse(input);
if (!_x2F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

var _x24_1 = __GeneratedOdataV4.Parsers.Inners._x24Parser.Instance.Parse(_x2F_1.Remainder);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x24_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x65_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x61_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

var _x68_1 = __GeneratedOdataV4.Parsers.Inners._x68Parser.Instance.Parse(_x63_1.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx2Fx24x65x61x63x68ʺ.Instance, _x68_1.Remainder);
            }
        }
    }
    
}
