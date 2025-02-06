namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx50x72x65x66x65x72ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ> Parse(IInput<char>? input)
            {
                var _x50_1 = __GeneratedOdataV4.Parsers.Inners._x50Parser.Instance.Parse(input);
if (!_x50_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x50_1.Remainder);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

var _x66_1 = __GeneratedOdataV4.Parsers.Inners._x66Parser.Instance.Parse(_x65_1.Remainder);
if (!_x66_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

var _x65_2 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x66_1.Remainder);
if (!_x65_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

var _x72_2 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x65_2.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx50x72x65x66x65x72ʺ.Instance, _x72_2.Remainder);
            }
        }
    }
    
}
