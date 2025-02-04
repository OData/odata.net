namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _escapeⳆquotationⲻmarkParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark> Instance { get; } = (_escapeParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark>(_quotationⲻmarkParser.Instance);
        
        public static class _escapeParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._escape> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._escape>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._escape> Parse(IInput<char>? input)
                {
                    var _escape_1 = __GeneratedOdataV3.Parsers.Rules._escapeParser.Instance.Parse(input);
if (!_escape_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._escape)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._escape(_escape_1.Parsed), _escape_1.Remainder);
                }
            }
        }
        
        public static class _quotationⲻmarkParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark> Parse(IInput<char>? input)
                {
                    var _quotationⲻmark_1 = __GeneratedOdataV3.Parsers.Rules._quotationⲻmarkParser.Instance.Parse(input);
if (!_quotationⲻmark_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._escapeⳆquotationⲻmark._quotationⲻmark(_quotationⲻmark_1.Parsed), _quotationⲻmark_1.Remainder);
                }
            }
        }
    }
    
}
