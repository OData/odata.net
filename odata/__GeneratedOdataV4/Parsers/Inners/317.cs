namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx63x65x69x6Cx69x6Ex67ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ> Parse(IInput<char>? input)
            {
                var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(input);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x63_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x65_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x69_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x69_2 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x69_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x69_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV4.Parsers.Inners._x67Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx63x65x69x6Cx69x6Ex67ʺ.Instance, _x67_1.Remainder);
            }
        }
    }
    
}
