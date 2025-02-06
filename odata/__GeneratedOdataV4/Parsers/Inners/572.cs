namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx63x61x6Cx6Cx62x61x63x6BʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ> Parse(IInput<char>? input)
            {
                var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(input);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x63_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6C_1.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x62_1 = __GeneratedOdataV4.Parsers.Inners._x62Parser.Instance.Parse(_x6C_2.Remainder);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x61_2 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x62_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x63_2 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x61_2.Remainder);
if (!_x63_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

var _x6B_1 = __GeneratedOdataV4.Parsers.Inners._x6BParser.Instance.Parse(_x63_2.Remainder);
if (!_x6B_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx63x61x6Cx6Cx62x61x63x6Bʺ.Instance, _x6B_1.Remainder);
            }
        }
    }
    
}
