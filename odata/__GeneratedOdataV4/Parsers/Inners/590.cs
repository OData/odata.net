namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ> Parse(IInput<char>? input)
            {
                var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(input);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x70_1 = __GeneratedOdataV4.Parsers.Inners._x70Parser.Instance.Parse(_x65_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x72_2 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x70_1.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_2.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x65_2.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x65_3 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x73_1.Remainder);
if (!_x65_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x65_3.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x74_2 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x74_2.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x70x72x65x73x65x6Ex74x61x74x69x6Fx6Eʺ.Instance, _x6E_2.Remainder);
            }
        }
    }
    
}
