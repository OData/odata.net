namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectPropertyParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty> Instance { get; } = (_OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty>(_ʺx2Fʺ_selectPropertyParser.Instance);
        
        public static class _OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE> Parse(IInput<char>? input)
                {
                    var _OPEN_1 = __GeneratedOdataV4.Parsers.Rules._OPENParser.Instance.Parse(input);
if (!_OPEN_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE)!, input);
}

var _selectOption_1 = __GeneratedOdataV4.Parsers.Rules._selectOptionParser.Instance.Parse(_OPEN_1.Remainder);
if (!_selectOption_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE)!, input);
}

var _ⲤSEMI_selectOptionↃ_1 = Inners._ⲤSEMI_selectOptionↃParser.Instance.Many().Parse(_selectOption_1.Remainder);
if (!_ⲤSEMI_selectOptionↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE)!, input);
}

var _CLOSE_1 = __GeneratedOdataV4.Parsers.Rules._CLOSEParser.Instance.Parse(_ⲤSEMI_selectOptionↃ_1.Remainder);
if (!_CLOSE_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSE(_OPEN_1.Parsed, _selectOption_1.Parsed, _ⲤSEMI_selectOptionↃ_1.Parsed, _CLOSE_1.Parsed), _CLOSE_1.Remainder);
                }
            }
        }
        
        public static class _ʺx2Fʺ_selectPropertyParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty> Parse(IInput<char>? input)
                {
                    var _ʺx2Fʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx2FʺParser.Instance.Parse(input);
if (!_ʺx2Fʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty)!, input);
}

var _selectProperty_1 = __GeneratedOdataV4.Parsers.Rules._selectPropertyParser.Instance.Parse(_ʺx2Fʺ_1.Remainder);
if (!_selectProperty_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._OPEN_selectOption_ЖⲤSEMI_selectOptionↃ_CLOSEⳆʺx2Fʺ_selectProperty._ʺx2Fʺ_selectProperty(_ʺx2Fʺ_1.Parsed, _selectProperty_1.Parsed), _selectProperty_1.Remainder);
                }
            }
        }
    }
    
}
