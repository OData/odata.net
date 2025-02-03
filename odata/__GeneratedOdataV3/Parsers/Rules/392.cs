namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _IPv6addressParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address> Instance { get; } = (_6Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16Parser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address>(_꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3AʺParser.Instance);
        
        public static class _6Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(6, 6)
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._6Ⲥh16_ʺx3AʺↃ_ls32(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly6<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _ls32_1);
        }
        
        public static class _ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(5, 5)
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._ʺx3Ax3Aʺ_5Ⲥh16_ʺx3AʺↃ_ls32(_ʺx3Ax3Aʺ_1, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly5<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _ls32_1);
        }
        
        public static class _꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = from _h16_1 in __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(4, 4)
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡h16꘡_ʺx3Ax3Aʺ_4Ⲥh16_ʺx3AʺↃ_ls32(_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly4<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _ls32_1);
        }
        
        public static class _꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = from _Ж1Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж1Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(3, 3)
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж1Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_3Ⲥh16_ʺx3AʺↃ_ls32(_Ж1Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly3<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _ls32_1);
        }
        
        public static class _꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32> Instance { get; } = from _Ж2Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж2Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _Ⲥh16_ʺx3AʺↃ_1 in __GeneratedOdataV3.Parsers.Inners._Ⲥh16_ʺx3AʺↃParser.Instance.Repeat(2, 2)
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж2Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_2Ⲥh16_ʺx3AʺↃ_ls32(_Ж2Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, new __GeneratedOdataV3.CstNodes.Inners.HelperRangedExactly2<__GeneratedOdataV3.CstNodes.Inners._Ⲥh16_ʺx3AʺↃ>(_Ⲥh16_ʺx3AʺↃ_1), _ls32_1);
        }
        
        public static class _꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32> Instance { get; } = from _Ж3Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж3Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _h16_1 in __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance
from _ʺx3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3AʺParser.Instance
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж3Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16_ʺx3Aʺ_ls32(_Ж3Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, _h16_1, _ʺx3Aʺ_1, _ls32_1);
        }
        
        public static class _꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32> Instance { get; } = from _Ж4Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж4Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _ls32_1 in __GeneratedOdataV3.Parsers.Rules._ls32Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж4Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_ls32(_Ж4Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, _ls32_1);
        }
        
        public static class _꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16Parser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16> Instance { get; } = from _Ж5Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж5Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
from _h16_1 in __GeneratedOdataV3.Parsers.Rules._h16Parser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж5Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ_h16(_Ж5Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1, _h16_1);
        }
        
        public static class _꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3AʺParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ> Instance { get; } = from _Ж6Ⲥh16_ʺx3AʺↃ_h16_1 in __GeneratedOdataV3.Parsers.Inners._Ж6Ⲥh16_ʺx3AʺↃ_h16Parser.Instance.Optional()
from _ʺx3Ax3Aʺ_1 in __GeneratedOdataV3.Parsers.Inners._ʺx3Ax3AʺParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._IPv6address._꘡Ж6Ⲥh16_ʺx3AʺↃ_h16꘡_ʺx3Ax3Aʺ(_Ж6Ⲥh16_ʺx3AʺↃ_h16_1.GetOrElse(null), _ʺx3Ax3Aʺ_1);
        }
    }
    
}
