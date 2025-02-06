namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx45x6Ex74x69x74x79x49x44ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ> Parse(IInput<char>? input)
            {
                var _x45_1 = __GeneratedOdataV4.Parsers.Inners._x45Parser.Instance.Parse(input);
if (!_x45_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x45_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x74_2 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x69_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x74_2.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x49_1 = __GeneratedOdataV4.Parsers.Inners._x49Parser.Instance.Parse(_x79_1.Remainder);
if (!_x49_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

var _x44_1 = __GeneratedOdataV4.Parsers.Inners._x44Parser.Instance.Parse(_x49_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx45x6Ex74x69x74x79x49x44ʺ.Instance, _x44_1.Remainder);
            }
        }
    }
    
}
