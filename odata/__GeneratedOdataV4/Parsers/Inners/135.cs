namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx66x69x6Cx74x65x72ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ> Parse(IInput<char>? input)
            {
                var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(input);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x66_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x69_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6C_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x74_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x65_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx66x69x6Cx74x65x72ʺ.Instance, _x72_1.Remainder);
            }
        }
    }
    
}
