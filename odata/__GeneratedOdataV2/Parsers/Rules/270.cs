namespace __GeneratedOdataV2.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _primitiveLiteralParser
    {
        public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral> Instance { get; } = (_nullValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_booleanValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_guidValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_dateValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_dateTimeOffsetValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_timeOfDayValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_decimalValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_doubleValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_singleValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_sbyteValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_byteValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_int16ValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_int32ValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_int64ValueParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_stringParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_durationParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_enumParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_binaryParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyCollectionParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyLineStringParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyMultiLineStringParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyMultiPointParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyMultiPolygonParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyPointParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geographyPolygonParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryCollectionParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryLineStringParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryMultiLineStringParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryMultiPointParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryMultiPolygonParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryPointParser.Instance).Or<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral>(_geometryPolygonParser.Instance);
        
        public static class _nullValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._nullValue> Instance { get; } = from _nullValue_1 in __GeneratedOdataV2.Parsers.Rules._nullValueParser.Instance
select __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._nullValue.Instance;
        }
        
        public static class _booleanValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._booleanValue> Instance { get; } = from _booleanValue_1 in __GeneratedOdataV2.Parsers.Rules._booleanValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._booleanValue(_booleanValue_1);
        }
        
        public static class _guidValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._guidValue> Instance { get; } = from _guidValue_1 in __GeneratedOdataV2.Parsers.Rules._guidValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._guidValue(_guidValue_1);
        }
        
        public static class _dateValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._dateValue> Instance { get; } = from _dateValue_1 in __GeneratedOdataV2.Parsers.Rules._dateValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._dateValue(_dateValue_1);
        }
        
        public static class _dateTimeOffsetValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue> Instance { get; } = from _dateTimeOffsetValue_1 in __GeneratedOdataV2.Parsers.Rules._dateTimeOffsetValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._dateTimeOffsetValue(_dateTimeOffsetValue_1);
        }
        
        public static class _timeOfDayValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._timeOfDayValue> Instance { get; } = from _timeOfDayValue_1 in __GeneratedOdataV2.Parsers.Rules._timeOfDayValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._timeOfDayValue(_timeOfDayValue_1);
        }
        
        public static class _decimalValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._decimalValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdataV2.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._decimalValue(_decimalValue_1);
        }
        
        public static class _doubleValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._doubleValue> Instance { get; } = from _doubleValue_1 in __GeneratedOdataV2.Parsers.Rules._doubleValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._doubleValue(_doubleValue_1);
        }
        
        public static class _singleValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._singleValue> Instance { get; } = from _singleValue_1 in __GeneratedOdataV2.Parsers.Rules._singleValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._singleValue(_singleValue_1);
        }
        
        public static class _sbyteValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._sbyteValue> Instance { get; } = from _sbyteValue_1 in __GeneratedOdataV2.Parsers.Rules._sbyteValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._sbyteValue(_sbyteValue_1);
        }
        
        public static class _byteValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._byteValue> Instance { get; } = from _byteValue_1 in __GeneratedOdataV2.Parsers.Rules._byteValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._byteValue(_byteValue_1);
        }
        
        public static class _int16ValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int16Value> Instance { get; } = from _int16Value_1 in __GeneratedOdataV2.Parsers.Rules._int16ValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int16Value(_int16Value_1);
        }
        
        public static class _int32ValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int32Value> Instance { get; } = from _int32Value_1 in __GeneratedOdataV2.Parsers.Rules._int32ValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int32Value(_int32Value_1);
        }
        
        public static class _int64ValueParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int64Value> Instance { get; } = from _int64Value_1 in __GeneratedOdataV2.Parsers.Rules._int64ValueParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._int64Value(_int64Value_1);
        }
        
        public static class _stringParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._string> Instance { get; } = from _string_1 in __GeneratedOdataV2.Parsers.Rules._stringParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._string(_string_1);
        }
        
        public static class _durationParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._duration> Instance { get; } = from _duration_1 in __GeneratedOdataV2.Parsers.Rules._durationParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._duration(_duration_1);
        }
        
        public static class _enumParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._enum> Instance { get; } = from _enum_1 in __GeneratedOdataV2.Parsers.Rules._enumParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._enum(_enum_1);
        }
        
        public static class _binaryParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._binary> Instance { get; } = from _binary_1 in __GeneratedOdataV2.Parsers.Rules._binaryParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._binary(_binary_1);
        }
        
        public static class _geographyCollectionParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyCollection> Instance { get; } = from _geographyCollection_1 in __GeneratedOdataV2.Parsers.Rules._geographyCollectionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyCollection(_geographyCollection_1);
        }
        
        public static class _geographyLineStringParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyLineString> Instance { get; } = from _geographyLineString_1 in __GeneratedOdataV2.Parsers.Rules._geographyLineStringParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyLineString(_geographyLineString_1);
        }
        
        public static class _geographyMultiLineStringParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiLineString> Instance { get; } = from _geographyMultiLineString_1 in __GeneratedOdataV2.Parsers.Rules._geographyMultiLineStringParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiLineString(_geographyMultiLineString_1);
        }
        
        public static class _geographyMultiPointParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiPoint> Instance { get; } = from _geographyMultiPoint_1 in __GeneratedOdataV2.Parsers.Rules._geographyMultiPointParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiPoint(_geographyMultiPoint_1);
        }
        
        public static class _geographyMultiPolygonParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon> Instance { get; } = from _geographyMultiPolygon_1 in __GeneratedOdataV2.Parsers.Rules._geographyMultiPolygonParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyMultiPolygon(_geographyMultiPolygon_1);
        }
        
        public static class _geographyPointParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyPoint> Instance { get; } = from _geographyPoint_1 in __GeneratedOdataV2.Parsers.Rules._geographyPointParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyPoint(_geographyPoint_1);
        }
        
        public static class _geographyPolygonParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyPolygon> Instance { get; } = from _geographyPolygon_1 in __GeneratedOdataV2.Parsers.Rules._geographyPolygonParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geographyPolygon(_geographyPolygon_1);
        }
        
        public static class _geometryCollectionParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryCollection> Instance { get; } = from _geometryCollection_1 in __GeneratedOdataV2.Parsers.Rules._geometryCollectionParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryCollection(_geometryCollection_1);
        }
        
        public static class _geometryLineStringParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryLineString> Instance { get; } = from _geometryLineString_1 in __GeneratedOdataV2.Parsers.Rules._geometryLineStringParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryLineString(_geometryLineString_1);
        }
        
        public static class _geometryMultiLineStringParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiLineString> Instance { get; } = from _geometryMultiLineString_1 in __GeneratedOdataV2.Parsers.Rules._geometryMultiLineStringParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiLineString(_geometryMultiLineString_1);
        }
        
        public static class _geometryMultiPointParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiPoint> Instance { get; } = from _geometryMultiPoint_1 in __GeneratedOdataV2.Parsers.Rules._geometryMultiPointParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiPoint(_geometryMultiPoint_1);
        }
        
        public static class _geometryMultiPolygonParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon> Instance { get; } = from _geometryMultiPolygon_1 in __GeneratedOdataV2.Parsers.Rules._geometryMultiPolygonParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryMultiPolygon(_geometryMultiPolygon_1);
        }
        
        public static class _geometryPointParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryPoint> Instance { get; } = from _geometryPoint_1 in __GeneratedOdataV2.Parsers.Rules._geometryPointParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryPoint(_geometryPoint_1);
        }
        
        public static class _geometryPolygonParser
        {
            public static IParser<char, __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryPolygon> Instance { get; } = from _geometryPolygon_1 in __GeneratedOdataV2.Parsers.Rules._geometryPolygonParser.Instance
select new __GeneratedOdataV2.CstNodes.Rules._primitiveLiteral._geometryPolygon(_geometryPolygon_1);
        }
    }
    
}
