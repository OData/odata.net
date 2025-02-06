namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx34x2Ex30ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ> Parse(IInput<char>? input)
            {
                var _x34_1 = __GeneratedOdataV4.Parsers.Inners._x34Parser.Instance.Parse(input);
if (!_x34_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ)!, input);
}

var _x2E_1 = __GeneratedOdataV4.Parsers.Inners._x2EParser.Instance.Parse(_x34_1.Remainder);
if (!_x2E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ)!, input);
}

var _x30_1 = __GeneratedOdataV4.Parsers.Inners._x30Parser.Instance.Parse(_x2E_1.Remainder);
if (!_x30_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx34x2Ex30ʺ.Instance, _x30_1.Remainder);
            }
        }
    }
    
}
