namespace __GeneratedOdata.Parsers.Rules
{
    using Sprache;
    
    public static class _primitiveValueParser
    {
        public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue> Instance { get; } = (_booleanValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_guidValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_durationValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_dateValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_dateTimeOffsetValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_timeOfDayValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_enumValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullCollectionLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullLineStringLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullMultiPointLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullMultiLineStringLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullMultiPolygonLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullPointLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_fullPolygonLiteralParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_decimalValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_doubleValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_singleValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_sbyteValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_byteValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_int16ValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_int32ValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_int64ValueParser.Instance).Or<__GeneratedOdata.CstNodes.Rules._primitiveValue>(_binaryValueParser.Instance);
        
        public static class _booleanValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._booleanValue> Instance { get; } = from _booleanValue_1 in __GeneratedOdata.Parsers.Rules._booleanValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._booleanValue(_booleanValue_1);
        }
        
        public static class _guidValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._guidValue> Instance { get; } = from _guidValue_1 in __GeneratedOdata.Parsers.Rules._guidValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._guidValue(_guidValue_1);
        }
        
        public static class _durationValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._durationValue> Instance { get; } = from _durationValue_1 in __GeneratedOdata.Parsers.Rules._durationValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._durationValue(_durationValue_1);
        }
        
        public static class _dateValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._dateValue> Instance { get; } = from _dateValue_1 in __GeneratedOdata.Parsers.Rules._dateValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._dateValue(_dateValue_1);
        }
        
        public static class _dateTimeOffsetValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._dateTimeOffsetValue> Instance { get; } = from _dateTimeOffsetValue_1 in __GeneratedOdata.Parsers.Rules._dateTimeOffsetValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._dateTimeOffsetValue(_dateTimeOffsetValue_1);
        }
        
        public static class _timeOfDayValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._timeOfDayValue> Instance { get; } = from _timeOfDayValue_1 in __GeneratedOdata.Parsers.Rules._timeOfDayValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._timeOfDayValue(_timeOfDayValue_1);
        }
        
        public static class _enumValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._enumValue> Instance { get; } = from _enumValue_1 in __GeneratedOdata.Parsers.Rules._enumValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._enumValue(_enumValue_1);
        }
        
        public static class _fullCollectionLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullCollectionLiteral> Instance { get; } = from _fullCollectionLiteral_1 in __GeneratedOdata.Parsers.Rules._fullCollectionLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullCollectionLiteral(_fullCollectionLiteral_1);
        }
        
        public static class _fullLineStringLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullLineStringLiteral> Instance { get; } = from _fullLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullLineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullLineStringLiteral(_fullLineStringLiteral_1);
        }
        
        public static class _fullMultiPointLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiPointLiteral> Instance { get; } = from _fullMultiPointLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiPointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiPointLiteral(_fullMultiPointLiteral_1);
        }
        
        public static class _fullMultiLineStringLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral> Instance { get; } = from _fullMultiLineStringLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiLineStringLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiLineStringLiteral(_fullMultiLineStringLiteral_1);
        }
        
        public static class _fullMultiPolygonLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral> Instance { get; } = from _fullMultiPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._fullMultiPolygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullMultiPolygonLiteral(_fullMultiPolygonLiteral_1);
        }
        
        public static class _fullPointLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullPointLiteral> Instance { get; } = from _fullPointLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPointLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullPointLiteral(_fullPointLiteral_1);
        }
        
        public static class _fullPolygonLiteralParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._fullPolygonLiteral> Instance { get; } = from _fullPolygonLiteral_1 in __GeneratedOdata.Parsers.Rules._fullPolygonLiteralParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._fullPolygonLiteral(_fullPolygonLiteral_1);
        }
        
        public static class _decimalValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._decimalValue> Instance { get; } = from _decimalValue_1 in __GeneratedOdata.Parsers.Rules._decimalValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._decimalValue(_decimalValue_1);
        }
        
        public static class _doubleValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._doubleValue> Instance { get; } = from _doubleValue_1 in __GeneratedOdata.Parsers.Rules._doubleValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._doubleValue(_doubleValue_1);
        }
        
        public static class _singleValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._singleValue> Instance { get; } = from _singleValue_1 in __GeneratedOdata.Parsers.Rules._singleValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._singleValue(_singleValue_1);
        }
        
        public static class _sbyteValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._sbyteValue> Instance { get; } = from _sbyteValue_1 in __GeneratedOdata.Parsers.Rules._sbyteValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._sbyteValue(_sbyteValue_1);
        }
        
        public static class _byteValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._byteValue> Instance { get; } = from _byteValue_1 in __GeneratedOdata.Parsers.Rules._byteValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._byteValue(_byteValue_1);
        }
        
        public static class _int16ValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._int16Value> Instance { get; } = from _int16Value_1 in __GeneratedOdata.Parsers.Rules._int16ValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._int16Value(_int16Value_1);
        }
        
        public static class _int32ValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._int32Value> Instance { get; } = from _int32Value_1 in __GeneratedOdata.Parsers.Rules._int32ValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._int32Value(_int32Value_1);
        }
        
        public static class _int64ValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._int64Value> Instance { get; } = from _int64Value_1 in __GeneratedOdata.Parsers.Rules._int64ValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._int64Value(_int64Value_1);
        }
        
        public static class _binaryValueParser
        {
            public static Parser<__GeneratedOdata.CstNodes.Rules._primitiveValue._binaryValue> Instance { get; } = from _binaryValue_1 in __GeneratedOdata.Parsers.Rules._binaryValueParser.Instance
select new __GeneratedOdata.CstNodes.Rules._primitiveValue._binaryValue(_binaryValue_1);
        }
    }
    
}
