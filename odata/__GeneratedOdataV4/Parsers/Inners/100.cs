namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx24x65x78x70x61x6Ex64ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ> Parse(IInput<char>? input)
            {
                var _x24_1 = __GeneratedOdataV4.Parsers.Inners._x24Parser.Instance.Parse(input);
if (!_x24_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x65_1 = __GeneratedOdataV4.Parsers.Inners._x65Parser.Instance.Parse(_x24_1.Remainder);
if (!_x65_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x78_1 = __GeneratedOdataV4.Parsers.Inners._x78Parser.Instance.Parse(_x65_1.Remainder);
if (!_x78_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x70_1 = __GeneratedOdataV4.Parsers.Inners._x70Parser.Instance.Parse(_x78_1.Remainder);
if (!_x70_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x61_1 = __GeneratedOdataV4.Parsers.Inners._x61Parser.Instance.Parse(_x70_1.Remainder);
if (!_x61_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x6E_1 = __GeneratedOdataV4.Parsers.Inners._x6EParser.Instance.Parse(_x61_1.Remainder);
if (!_x6E_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

var _x64_1 = __GeneratedOdataV4.Parsers.Inners._x64Parser.Instance.Parse(_x6E_1.Remainder);
if (!_x64_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._ʺx24x65x78x70x61x6Ex64ʺ.Instance, _x64_1.Remainder);
            }
        }
    }
    
}
