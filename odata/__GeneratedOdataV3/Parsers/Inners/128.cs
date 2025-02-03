namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Cx65x76x65x6Cx73ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ> Parse(IInput<char>? input)
            {
                var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(input);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

var _x76_1 = __GeneratedOdataV3.Parsers.Inners._x76Parser.Instance.Parse(_x65_1.Remainder);
if (!_x76_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x76_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

var _x6C_2 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x65_2.Remainder);
if (!_x6C_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x6C_2.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Cx65x76x65x6Cx73ʺ.Instance, _x73_1.Remainder);
            }
        }
    }
    
}
