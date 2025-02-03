namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx44x65x63x69x6Dx61x6CʺParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ> Parse(IInput<char>? input)
            {
                var _x44_1 = __GeneratedOdataV3.Parsers.Inners._x44Parser.Instance.Parse(input);
if (!_x44_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x65_1 = __GeneratedOdataV3.Parsers.Inners._x65Parser.Instance.Parse(_x44_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x63_1 = __GeneratedOdataV3.Parsers.Inners._x63Parser.Instance.Parse(_x65_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x69_1 = __GeneratedOdataV3.Parsers.Inners._x69Parser.Instance.Parse(_x63_1.Remainder);
if (!_x69_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV3.Parsers.Inners._x6DParser.Instance.Parse(_x69_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x61_1 = __GeneratedOdataV3.Parsers.Inners._x61Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV3.Parsers.Inners._x6CParser.Instance.Parse(_x61_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV3.CstNodes.Inners._ʺx44x65x63x69x6Dx61x6Cʺ.Instance, _x6C_1.Remainder);
            }
        }
    }
    
}
