namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx78x6Dx6CʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ> Parse(IInput<char>? input)
            {
                var _x78_1 = __GeneratedOdataV4.Parsers.Inners._x78Parser.Instance.Parse(input);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ)!, input);
}

var _x6D_1 = __GeneratedOdataV4.Parsers.Inners._x6DParser.Instance.Parse(_x78_1.Remainder);
if (!_x6D_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ)!, input);
}

var _x6C_1 = __GeneratedOdataV4.Parsers.Inners._x6CParser.Instance.Parse(_x6D_1.Remainder);
if (!_x6C_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx78x6Dx6Cʺ.Instance, _x6C_1.Remainder);
            }
        }
    }
    
}
