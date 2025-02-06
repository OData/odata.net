namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡Parser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡> Instance { get; } = new Parser();
        
        private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡>
        {
            public Parser()
            {
            }
            
            public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡> Parse(IInput<char>? input)
            {
                var _ʺx54ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx54ʺParser.Instance.Parse(input);
if (!_ʺx54ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡)!, input);
}

var _1ЖDIGIT_ʺx48ʺ_1 = __GeneratedOdataV4.Parsers.Inners._1ЖDIGIT_ʺx48ʺParser.Instance.Optional().Parse(_ʺx54ʺ_1.Remainder);
if (!_1ЖDIGIT_ʺx48ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡)!, input);
}

var _1ЖDIGIT_ʺx4Dʺ_1 = __GeneratedOdataV4.Parsers.Inners._1ЖDIGIT_ʺx4DʺParser.Instance.Optional().Parse(_1ЖDIGIT_ʺx48ʺ_1.Remainder);
if (!_1ЖDIGIT_ʺx4Dʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡)!, input);
}

var _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1 = __GeneratedOdataV4.Parsers.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺParser.Instance.Optional().Parse(_1ЖDIGIT_ʺx4Dʺ_1.Remainder);
if (!_1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡(_ʺx54ʺ_1.Parsed, _1ЖDIGIT_ʺx48ʺ_1.Parsed.GetOrElse(null), _1ЖDIGIT_ʺx4Dʺ_1.Parsed.GetOrElse(null), _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1.Parsed.GetOrElse(null)), _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1.Remainder);
            }
        }
    }
    
}
