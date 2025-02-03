namespace __GeneratedOdataV2.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡Parser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡> Instance { get; } = from _ʺx54ʺ_1 in __GeneratedOdataV2.Parsers.Inners._ʺx54ʺParser.Instance
from _1ЖDIGIT_ʺx48ʺ_1 in __GeneratedOdataV2.Parsers.Inners._1ЖDIGIT_ʺx48ʺParser.Instance.Optional()
from _1ЖDIGIT_ʺx4Dʺ_1 in __GeneratedOdataV2.Parsers.Inners._1ЖDIGIT_ʺx4DʺParser.Instance.Optional()
from _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1 in __GeneratedOdataV2.Parsers.Inners._1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺParser.Instance.Optional()
select new __GeneratedOdataV2.CstNodes.Inners._ʺx54ʺ_꘡1ЖDIGIT_ʺx48ʺ꘡_꘡1ЖDIGIT_ʺx4Dʺ꘡_꘡1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ꘡(_ʺx54ʺ_1, _1ЖDIGIT_ʺx48ʺ_1.GetOrElse(null), _1ЖDIGIT_ʺx4Dʺ_1.GetOrElse(null), _1ЖDIGIT_꘡ʺx2Eʺ_1ЖDIGIT꘡_ʺx53ʺ_1.GetOrElse(null));
    }
    
}
