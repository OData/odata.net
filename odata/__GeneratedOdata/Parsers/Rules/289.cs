namespace __GeneratedOdata.Parsers.Rules
{
    using __GeneratedOdata.CstNodes.Rules;
    using CombinatorParsingV2;
    
    public static class _stringParser
    {
        /*public static IParser<char, __GeneratedOdata.CstNodes.Rules._string> Instance { get; } = from _SQUOTE_1 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
from _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 in Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃParser.Instance.Many()
from _SQUOTE_2 in __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance
select new __GeneratedOdata.CstNodes.Rules._string(_SQUOTE_1, _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1, _SQUOTE_2);
        */
        //// PERF
        public static IParser<char, __GeneratedOdata.CstNodes.Rules._string> Instance { get; } = new Parser();

        private sealed class Parser : IParser<char, __GeneratedOdata.CstNodes.Rules._string>
        {
            public IOutput<char, _string> Parse(IInput<char>? input)
            {
                var _SQUOTE_1 = __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance.Parse(input);
                if (!_SQUOTE_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_string)!,
                        input);
                }

                var _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1 = Inners._ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃParser.Instance.Many().Parse(_SQUOTE_1.Remainder);
                if (!_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Success)
                {
                    return Output.Create(
                        false,
                        default(_string)!,
                        input);
                }

                var _SQUOTE_2 = __GeneratedOdata.Parsers.Rules._SQUOTEParser.Instance.Parse(_ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Remainder);
                if (!_SQUOTE_2.Success)
                {
                    return Output.Create(
                        false,
                        default(_string)!,
                        input);
                }

                return Output.Create(
                    true,
                    new __GeneratedOdata.CstNodes.Rules._string(_SQUOTE_1.Parsed, _ⲤSQUOTEⲻinⲻstringⳆpcharⲻnoⲻSQUOTEↃ_1.Parsed, _SQUOTE_2.Parsed),
                    _SQUOTE_2.Remainder);
            }
        }
    }
    
}
