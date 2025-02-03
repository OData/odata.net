namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6EʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ> Parse(IInput<char>? input)
            {
                var _x73_1 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(input);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x73_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x68_1 = __GeneratedOdataV3.Parsers.Inners._x68Parser.Instance.Parse(_x63_1.Remainder);
if (!_x68_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x68_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x65_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x76_1 = __GeneratedOdataV3.Parsers.Inners._x76Parser.Instance.Parse(_x61_1.Remainder);
if (!_x76_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x65_2 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x76_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x65_2.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x73_2 = __GeneratedOdataV3.Parsers.Inners._x73Parser.Instance.Parse(_x72_1.Remainder);
if (!_x73_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x73_2.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x69_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x6F_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx73x63x68x65x6Dx61x76x65x72x73x69x6Fx6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
