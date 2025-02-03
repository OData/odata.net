namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Dx69x6Ex69x6Dx61x6CʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ> Parse(IInput<char>? input)
            {
                var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(input);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV3.Parsers.Inners._x6EParser.Instance.Parse(_x69_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x69_2 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x69_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x6D_2 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x69_2.Remainder);
if (!_x6D_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6D_2.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx6Dx69x6Ex69x6Dx61x6Cʺ.Instance, _x6C_1.Remainder);
            }
        }
    }
    
}
