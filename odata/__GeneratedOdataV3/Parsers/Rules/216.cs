namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _charInJSONParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON> Instance { get; } = (_qcharⲻunescapedParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON>(_qcharⲻJSONⲻspecialParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON>(_escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃParser.Instance);
        
        public static class _qcharⲻunescapedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._qcharⲻunescaped> Instance { get; } = from _qcharⲻunescaped_1 in __GeneratedOdataV3.Parsers.Rules._qcharⲻunescapedParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._charInJSON._qcharⲻunescaped(_qcharⲻunescaped_1);
        }
        
        public static class _qcharⲻJSONⲻspecialParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._qcharⲻJSONⲻspecial> Instance { get; } = from _qcharⲻJSONⲻspecial_1 in __GeneratedOdataV3.Parsers.Rules._qcharⲻJSONⲻspecialParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._charInJSON._qcharⲻJSONⲻspecial(_qcharⲻJSONⲻspecial_1);
        }
        
        public static class _escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ> Parse(IInput<char>? input)
                {
                    var _escape_1 = __GeneratedOdataV3.Parsers.Rules._escapeParser.Instance.Parse(input);
if (!_escape_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ)!, input);
}

var _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1 = __GeneratedOdataV3.Parsers.Inners._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃParser.Instance.Parse(_escape_1.Remainder);
if (!_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ(_escape_1.Parsed,  _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1.Parsed), _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1.Remainder);
                }
            }
        }
    }
    
}
