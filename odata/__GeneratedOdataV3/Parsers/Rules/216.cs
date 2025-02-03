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
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ> Instance { get; } = from _escape_1 in __GeneratedOdataV3.Parsers.Rules._escapeParser.Instance
from _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1 in __GeneratedOdataV3.Parsers.Inners._ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._charInJSON._escape_ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ(_escape_1, _ⲤquotationⲻmarkⳆescapeⳆⲤʺx2FʺⳆʺx25x32x46ʺↃⳆʺx62ʺⳆʺx66ʺⳆʺx6EʺⳆʺx72ʺⳆʺx74ʺⳆʺx75ʺ_4HEXDIGↃ_1);
        }
    }
    
}
