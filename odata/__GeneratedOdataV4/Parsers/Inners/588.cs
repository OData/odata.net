namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ> Parse(IInput<char>? input)
            {
                var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(input);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x65_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV4.Parsers.Inners._x70Parser.Instance.Parse(_x73_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV4.Parsers.Inners._x6FParser.Instance.Parse(_x70_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x2D_1 = __GeneratedOdataV4.Parsers.Inners._x2DParser.Instance.Parse(_x64_1.Remainder);
if (!_x2D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x2D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x73_2 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x61_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x79_1 = __GeneratedOdataV4.Parsers.Inners._x79Parser.Instance.Parse(_x73_2.Remainder);
if (!_x79_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x6E_2 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x79_1.Remainder);
if (!_x6E_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x6E_2.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x73x70x6Fx6Ex64x2Dx61x73x79x6Ex63ʺ.Instance, _x63_1.Remainder);
            }
        }
    }
    
}
