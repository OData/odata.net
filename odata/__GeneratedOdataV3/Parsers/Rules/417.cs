namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _qcharⲻnoⲻAMPⲻDQUOTEParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE> Instance { get; } = (_qcharⲻunescapedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE>(_escape_ⲤescapeⳆquotationⲻmarkↃParser.Instance);
        
        public static class _qcharⲻunescapedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped> Parse(IInput<char>? input)
                {
                    var _qcharⲻunescaped_1 = __GeneratedOdataV3.Parsers.Rules._qcharⲻunescapedParser.Instance.Parse(input);
if (!_qcharⲻunescaped_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._qcharⲻunescaped(_qcharⲻunescaped_1.Parsed), _qcharⲻunescaped_1.Remainder);
                }
            }
        }
        
        public static class _escape_ⲤescapeⳆquotationⲻmarkↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ> Parse(IInput<char>? input)
                {
                    var _escape_1 = __GeneratedOdataV3.Parsers.Rules._escapeParser.Instance.Parse(input);
if (!_escape_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ)!, input);
}

var _ⲤescapeⳆquotationⲻmarkↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤescapeⳆquotationⲻmarkↃParser.Instance.Parse(_escape_1.Remainder);
if (!_ⲤescapeⳆquotationⲻmarkↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._qcharⲻnoⲻAMPⲻDQUOTE._escape_ⲤescapeⳆquotationⲻmarkↃ(_escape_1.Parsed, _ⲤescapeⳆquotationⲻmarkↃ_1.Parsed), _ⲤescapeⳆquotationⲻmarkↃ_1.Remainder);
                }
            }
        }
    }
    
}
