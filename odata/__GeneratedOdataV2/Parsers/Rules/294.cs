namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _durationValueParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._durationValue> Instance { get; } = from _SIGN_1 in __GeneratedOdataV2.Parsers.Rules._SIGNParser.Instance.Optional()
from _ʺx50ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx50ʺParser.Instance
from _1ЖDIGIT_ʺx44ʺ_1 in __GeneratedOdataV2.Parsers.Inners._1ЖDIGIT_ʺx44ʺParser.Instance.Optional()
from _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1 in __GeneratedOdataV2.Parsers.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡Parser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Rules._durationValue(_SIGN_1.GetOrElse(null), _ʺx50ʺ_1, _1ЖDIGIT_ʺx44ʺ_1.GetOrElse(null), _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡_1.GetOrElse(null));
    }
    
}
