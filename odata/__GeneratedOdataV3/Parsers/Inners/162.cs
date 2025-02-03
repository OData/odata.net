namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x66x6Fx72x6Dx61x74ʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV3.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV3.Parsers.Inners._x66Parser.Instance.Parse(_x24_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x6F_1 = __GeneratedOdataV3.Parsers.Inners._x6FParser.Instance.Parse(_x66_1.Remainder);
if (!_x6F_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV3.Parsers.Inners._x72Parser.Instance.Parse(_x6F_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x72_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

var _x74_1 = __GeneratedOdataV3.Parsers.Inners._x74Parser.Instance.Parse(_x61_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx24x66x6Fx72x6Dx61x74ʺ.Instance, _x74_1.Remainder);
            }
        }
    }
    
}
