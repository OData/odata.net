namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx62x69x6Ex61x72x79ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ> Parse(IInput<char>? input)
            {
                var _x62_1 = __GeneratedOdataV3.Parsers.Inners._x62Parser.Instance.Parse(input);
if (!_x62_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x62_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x61_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV3.Parsers.Inners._x79Parser.Instance.Parse(_x72_1.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx62x69x6Ex61x72x79ʺ.Instance, _x79_1.Remainder);
            }
        }
    }
    
}
