namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx61x73x63ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ> Parse(IInput<char>? input)
            {
                var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(input);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ)!, input);
}

var _x73_1 = __GeneratedOdataV4.Parsers.Inners._x73Parser.Instance.Parse(_x61_1.Remainder);
if (!_x73_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ)!, input);
}

var _x63_1 = __GeneratedOdataV4.Parsers.Inners._x63Parser.Instance.Parse(_x73_1.Remainder);
if (!_x63_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx61x73x63ʺ.Instance, _x63_1.Remainder);
            }
        }
    }
    
}
