namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _pctⲻencodedⲻunescapedParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped> Instance { get; } = (_ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIGParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped>(_ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped>(_ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃParser.Instance);
        
        public static class _ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIGParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG> Parse(IInput<char>? input)
                {
                    var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

var _HEXDIG_1 = __GeneratedOdataV3.Parsers.Rules._HEXDIGParser.Instance.Parse(_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Remainder);
if (!_HEXDIG_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_HEXDIG(_ʺx25ʺ_1.Parsed, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Parsed, _HEXDIG_1.Parsed), _HEXDIG_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ> Parse(IInput<char>? input)
                {
                    var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

var _ʺx32ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx32ʺParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_ʺx32ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

var _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1 = __GeneratedOdataV3.Parsers.Inners._Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃParser.Instance.Parse(_ʺx32ʺ_1.Remainder);
if (!_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx32ʺ_Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ(_ʺx25ʺ_1.Parsed, _ʺx32ʺ_1.Parsed, _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Parsed), _Ⲥʺx30ʺⳆʺx31ʺⳆʺx33ʺⳆʺx34ʺⳆʺx35ʺⳆʺx36ʺⳆʺx37ʺⳆʺx38ʺⳆʺx39ʺⳆAⲻtoⲻFↃ_1.Remainder);
                }
            }
        }
        
        public static class _ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ> Parse(IInput<char>? input)
                {
                    var _ʺx25ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx25ʺParser.Instance.Parse(input);
if (!_ʺx25ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ)!, input);
}

var _ʺx35ʺ_1 = __GeneratedOdataV3.Parsers.Inners._ʺx35ʺParser.Instance.Parse(_ʺx25ʺ_1.Remainder);
if (!_ʺx35ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ)!, input);
}

var _ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃParser.Instance.Parse(_ʺx35ʺ_1.Remainder);
if (!_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._pctⲻencodedⲻunescaped._ʺx25ʺ_ʺx35ʺ_ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ(_ʺx25ʺ_1.Parsed, _ʺx35ʺ_1.Parsed, _ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ_1.Parsed), _ⲤDIGITⳆʺx41ʺⳆʺx42ʺⳆʺx44ʺⳆʺx45ʺⳆʺx46ʺↃ_1.Remainder);
                }
            }
        }
    }
    
}
