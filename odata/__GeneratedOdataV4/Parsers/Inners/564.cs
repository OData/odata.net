namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ> Parse(IInput<char>? input)
            {
                var _x4F_1 = __GeneratedOdataV4.Parsers.Inners._x4FParser.Instance.Parse(input);
if (!_x4F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x44_1 = __GeneratedOdataV4.Parsers.Inners._x44Parser.Instance.Parse(_x4F_1.Remainder);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x44_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x61_2 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x74_1.Remainder);
if (!_x61_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV4.Parsers.Inners._x2DParser.Instance.Parse(_x61_2.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x4D_1 = __GeneratedOdataV4.Parsers.Inners._x4DParser.Instance.Parse(_x2D_1.Remainder);
if (!_x4D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x61_3 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x4D_1.Remainder);
if (!_x61_3.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x78_1 = __GeneratedOdataV4.Parsers.Inners._x78Parser.Instance.Parse(_x61_3.Remainder);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x56_1 = __GeneratedOdataV4.Parsers.Inners._x56Parser.Instance.Parse(_x78_1.Remainder);
if (!_x56_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x56_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x65_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x72_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x69_1 = __GeneratedOdataV4.Parsers.Inners._x69Parser.Instance.Parse(_x73_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx4Fx44x61x74x61x2Dx4Dx61x78x56x65x72x73x69x6Fx6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
