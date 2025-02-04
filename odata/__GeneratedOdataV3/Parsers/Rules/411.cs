namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pctⲻencodedⲻnoⲻSQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE> Instance { get; } = (_ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIGParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE>(_ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance);
        
        public static class _ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIGParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG> Parse(IInput<char>? input)
                {
                    var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

var _HEXDIG_1 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Parse(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Remainder);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG(_ʺx25ʺ_1.Parsed, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Parsed,  _HEXDIG_1.Parsed), _HEXDIG_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ> Parse(IInput<char>? input)
                {
                    var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

var _ʺx32ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx32ʺParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance.Parse(_ʺx32ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻnoⲻSQUOTE._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ(_ʺx25ʺ_1.Parsed, _ʺx32ʺ_1.Parsed,  _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Parsed), _Ⲥʺx30ʺⳆʺx31ʺⳆʺx32ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Remainder);
                }
            }
        }
    }
    
}
