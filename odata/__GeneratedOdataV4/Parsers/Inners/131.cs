namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx6Dx61x78ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ> Parse(IInput<char>? input)
            {
                var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(input);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x6D_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ)!, input);
}

var _x78_1 = __GeneratedOdataV4.Parsers.Inners._x78Parser.Instance.Parse(_x61_1.Remainder);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx6Dx61x78ʺ.Instance, _x78_1.Remainder);
            }
        }
    }
    
}
