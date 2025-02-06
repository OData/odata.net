namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx72x65x74x75x72x6EʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ> Parse(IInput<char>? input)
            {
                var _x72_1 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(input);
if (!_x72_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x72_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

var _x74_1 = __GeneratedOdataV4.Parsers.Inners._x74Parser.Instance.Parse(_x65_1.Remainder);
if (!_x74_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

var _x75_1 = __GeneratedOdataV4.Parsers.Inners._x75Parser.Instance.Parse(_x74_1.Remainder);
if (!_x75_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

var _x72_2 = __GeneratedOdataV4.Parsers.Inners._x72Parser.Instance.Parse(_x75_1.Remainder);
if (!_x72_2.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x72_2.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx72x65x74x75x72x6Eʺ.Instance, _x6E_1.Remainder);
            }
        }
    }
    
}
