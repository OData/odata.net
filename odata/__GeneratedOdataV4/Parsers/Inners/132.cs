namespace __GeneratedOdataV4.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _oneToNine_ЖDIGITⳆʺx6Dx61x78ʺParser
    {
        public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ> Instance { get; } = (_oneToNine_ЖDIGITParser.Instance).Or<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ>(_ʺx6Dx61x78ʺParser.Instance);
        
        public static class _oneToNine_ЖDIGITParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT> Parse(IInput<char>? input)
                {
                    var _oneToNine_1 = __GeneratedOdataV4.Parsers.Rules._oneToNineParser.Instance.Parse(input);
if (!_oneToNine_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT)!, input);
}

var _DIGIT_1 = __GeneratedOdataV4.Parsers.Rules._DIGITParser.Instance.Many().Parse(_oneToNine_1.Remainder);
if (!_DIGIT_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT)!, input);
}

return Output.Create(true, new __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._oneToNine_ЖDIGIT(_oneToNine_1.Parsed, _DIGIT_1.Parsed), _DIGIT_1.Remainder);
                }
            }
        }
        
        public static class _ʺx6Dx61x78ʺParser
        {
            public static IParser<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ> Parse(IInput<char>? input)
                {
                    var _ʺx6Dx61x78ʺ_1 = __GeneratedOdataV4.Parsers.Inners._ʺx6Dx61x78ʺParser.Instance.Parse(input);
if (!_ʺx6Dx61x78ʺ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ)!, input);
}

return Output.Create(true, __GeneratedOdataV4.CstNodes.Inners._oneToNine_ЖDIGITⳆʺx6Dx61x78ʺ._ʺx6Dx61x78ʺ.Instance, _ʺx6Dx61x78ʺ_1.Remainder);
                }
            }
        }
    }
    
}
