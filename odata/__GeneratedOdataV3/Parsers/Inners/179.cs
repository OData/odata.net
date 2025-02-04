namespace __GeneratedOdataV3.Parsers.Inners
{
    using CombinatorParsingV2;
    
    public static class _STARⳆ1ЖunreservedParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved> Instance { get; } = (_STARParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved>(_1ЖunreservedParser.Instance);
        
        public static class _STARParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR> Parse(IInput<char>? input)
                {
                    var _STAR_1 = __GeneratedOdataV3.Parsers.Rules._STARParser.Instance.Parse(input);
if (!_STAR_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._STAR(_STAR_1.Parsed), _STAR_1.Remainder);
                }
            }
        }
        
        public static class _1ЖunreservedParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved> Parse(IInput<char>? input)
                {
                    var _unreserved_1 = __GeneratedOdataV3.Parsers.Rules._unreservedParser.Instance.Repeat(1, null).Parse(input);
if (!_unreserved_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Inners._STARⳆ1Жunreserved._1Жunreserved(new __GeneratedOdataV3.CstNodes.Inners.HelperRangedAtLeast1<__GeneratedOdataV3.CstNodes.Rules._unreserved>(_unreserved_1.Parsed)), _unreserved_1.Remainder);
                }
            }
        }
    }
    
}
