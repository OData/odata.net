namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _methodCallExprParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr> Instance { get; } = (_indexOfMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_toLowerMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_toUpperMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_trimMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_substringMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_concatMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_lengthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_yearMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_monthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_dayMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_hourMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_minuteMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_secondMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_fractionalsecondsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_totalsecondsMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_dateMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_timeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_roundMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_floorMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_ceilingMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_distanceMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_geoLengthMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_totalOffsetMinutesMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_minDateTimeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_maxDateTimeMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_nowMethodCallExprParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr>(_boolMethodCallExprParser.Instance);
        
        public static class _indexOfMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr> Instance { get; } = from _indexOfMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._indexOfMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._indexOfMethodCallExpr(_indexOfMethodCallExpr_1);
        }
        
        public static class _toLowerMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr> Instance { get; } = from _toLowerMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._toLowerMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toLowerMethodCallExpr(_toLowerMethodCallExpr_1);
        }
        
        public static class _toUpperMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr> Instance { get; } = from _toUpperMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._toUpperMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._toUpperMethodCallExpr(_toUpperMethodCallExpr_1);
        }
        
        public static class _trimMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr> Instance { get; } = from _trimMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._trimMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._trimMethodCallExpr(_trimMethodCallExpr_1);
        }
        
        public static class _substringMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr> Instance { get; } = from _substringMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._substringMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._substringMethodCallExpr(_substringMethodCallExpr_1);
        }
        
        public static class _concatMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr> Instance { get; } = from _concatMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._concatMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._concatMethodCallExpr(_concatMethodCallExpr_1);
        }
        
        public static class _lengthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr> Instance { get; } = from _lengthMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._lengthMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._lengthMethodCallExpr(_lengthMethodCallExpr_1);
        }
        
        public static class _yearMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr> Instance { get; } = from _yearMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._yearMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._yearMethodCallExpr(_yearMethodCallExpr_1);
        }
        
        public static class _monthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr> Instance { get; } = from _monthMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._monthMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._monthMethodCallExpr(_monthMethodCallExpr_1);
        }
        
        public static class _dayMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr> Instance { get; } = from _dayMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._dayMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dayMethodCallExpr(_dayMethodCallExpr_1);
        }
        
        public static class _hourMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr> Instance { get; } = from _hourMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._hourMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._hourMethodCallExpr(_hourMethodCallExpr_1);
        }
        
        public static class _minuteMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr> Instance { get; } = from _minuteMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._minuteMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minuteMethodCallExpr(_minuteMethodCallExpr_1);
        }
        
        public static class _secondMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr> Instance { get; } = from _secondMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._secondMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._secondMethodCallExpr(_secondMethodCallExpr_1);
        }
        
        public static class _fractionalsecondsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr> Instance { get; } = from _fractionalsecondsMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._fractionalsecondsMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._fractionalsecondsMethodCallExpr(_fractionalsecondsMethodCallExpr_1);
        }
        
        public static class _totalsecondsMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr> Instance { get; } = from _totalsecondsMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._totalsecondsMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalsecondsMethodCallExpr(_totalsecondsMethodCallExpr_1);
        }
        
        public static class _dateMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr> Instance { get; } = from _dateMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._dateMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._dateMethodCallExpr(_dateMethodCallExpr_1);
        }
        
        public static class _timeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr> Instance { get; } = from _timeMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._timeMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._timeMethodCallExpr(_timeMethodCallExpr_1);
        }
        
        public static class _roundMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr> Instance { get; } = from _roundMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._roundMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._roundMethodCallExpr(_roundMethodCallExpr_1);
        }
        
        public static class _floorMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr> Instance { get; } = from _floorMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._floorMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._floorMethodCallExpr(_floorMethodCallExpr_1);
        }
        
        public static class _ceilingMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr> Instance { get; } = from _ceilingMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._ceilingMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._ceilingMethodCallExpr(_ceilingMethodCallExpr_1);
        }
        
        public static class _distanceMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr> Instance { get; } = from _distanceMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._distanceMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._distanceMethodCallExpr(_distanceMethodCallExpr_1);
        }
        
        public static class _geoLengthMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr> Instance { get; } = from _geoLengthMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._geoLengthMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._geoLengthMethodCallExpr(_geoLengthMethodCallExpr_1);
        }
        
        public static class _totalOffsetMinutesMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr> Instance { get; } = from _totalOffsetMinutesMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._totalOffsetMinutesMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._totalOffsetMinutesMethodCallExpr(_totalOffsetMinutesMethodCallExpr_1);
        }
        
        public static class _minDateTimeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr> Instance { get; } = from _minDateTimeMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._minDateTimeMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._minDateTimeMethodCallExpr(_minDateTimeMethodCallExpr_1);
        }
        
        public static class _maxDateTimeMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr> Instance { get; } = from _maxDateTimeMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._maxDateTimeMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._maxDateTimeMethodCallExpr(_maxDateTimeMethodCallExpr_1);
        }
        
        public static class _nowMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr> Instance { get; } = from _nowMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._nowMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._nowMethodCallExpr(_nowMethodCallExpr_1);
        }
        
        public static class _boolMethodCallExprParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr> Instance { get; } = from _boolMethodCallExpr_1 in __GeneratedOdataV3.Parsers.Rules._boolMethodCallExprParser.Instance
select new __GeneratedOdataV3.CstNodes.Rules._methodCallExpr._boolMethodCallExpr(_boolMethodCallExpr_1);
        }
    }
    
}
