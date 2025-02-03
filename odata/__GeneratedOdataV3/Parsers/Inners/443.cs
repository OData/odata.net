namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx53x69x6Ex67x6Cx65ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ> Parse(IInput<char>? input)
            {
                var _x53_1 = __GeneratedOdataV3.Parsers.Inners._x53Parser.Instance.Parse(input);
if (!_x53_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x53_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

var _x67_1 = __GeneratedOdataV3.Parsers.Inners._x67Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x67_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x67_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx53x69x6Ex67x6Cx65ʺ.Instance, _x65_1.Remainder);
            }
        }
    }
    
}
