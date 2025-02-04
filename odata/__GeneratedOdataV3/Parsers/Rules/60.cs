namespace __GeneratedOdataV3.Parsers.Rules
{
    using CombinatorParsingV2;
    
    public static class _systemQueryOptionParser
    {
        public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption> Instance { get; } = (_computeParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_deltatokenParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_expandParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_filterParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_formatParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_idParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_inlinecountParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_orderbyParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_schemaversionParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_searchParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_selectParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_skipParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_skiptokenParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_topParser.Instance).Or<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption>(_indexParser.Instance);
        
        public static class _computeParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._compute> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._compute>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._compute> Parse(IInput<char>? input)
                {
                    var _compute_1 = __GeneratedOdataV3.Parsers.Rules._computeParser.Instance.Parse(input);
if (!_compute_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._compute)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._compute(_compute_1.Parsed), _compute_1.Remainder);
                }
            }
        }
        
        public static class _deltatokenParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._deltatoken> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._deltatoken>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._deltatoken> Parse(IInput<char>? input)
                {
                    var _deltatoken_1 = __GeneratedOdataV3.Parsers.Rules._deltatokenParser.Instance.Parse(input);
if (!_deltatoken_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._deltatoken)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._deltatoken(_deltatoken_1.Parsed), _deltatoken_1.Remainder);
                }
            }
        }
        
        public static class _expandParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._expand> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._expand>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._expand> Parse(IInput<char>? input)
                {
                    var _expand_1 = __GeneratedOdataV3.Parsers.Rules._expandParser.Instance.Parse(input);
if (!_expand_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._expand)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._expand(_expand_1.Parsed), _expand_1.Remainder);
                }
            }
        }
        
        public static class _filterParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._filter> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._filter>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._filter> Parse(IInput<char>? input)
                {
                    var _filter_1 = __GeneratedOdataV3.Parsers.Rules._filterParser.Instance.Parse(input);
if (!_filter_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._filter)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._filter(_filter_1.Parsed), _filter_1.Remainder);
                }
            }
        }
        
        public static class _formatParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._format> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._format>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._format> Parse(IInput<char>? input)
                {
                    var _format_1 = __GeneratedOdataV3.Parsers.Rules._formatParser.Instance.Parse(input);
if (!_format_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._format)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._format(_format_1.Parsed), _format_1.Remainder);
                }
            }
        }
        
        public static class _idParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._id> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._id>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._id> Parse(IInput<char>? input)
                {
                    var _id_1 = __GeneratedOdataV3.Parsers.Rules._idParser.Instance.Parse(input);
if (!_id_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._id)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._id(_id_1.Parsed), _id_1.Remainder);
                }
            }
        }
        
        public static class _inlinecountParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._inlinecount> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._inlinecount>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._inlinecount> Parse(IInput<char>? input)
                {
                    var _inlinecount_1 = __GeneratedOdataV3.Parsers.Rules._inlinecountParser.Instance.Parse(input);
if (!_inlinecount_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._inlinecount)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._inlinecount(_inlinecount_1.Parsed), _inlinecount_1.Remainder);
                }
            }
        }
        
        public static class _orderbyParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._orderby> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._orderby>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._orderby> Parse(IInput<char>? input)
                {
                    var _orderby_1 = __GeneratedOdataV3.Parsers.Rules._orderbyParser.Instance.Parse(input);
if (!_orderby_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._orderby)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._orderby(_orderby_1.Parsed), _orderby_1.Remainder);
                }
            }
        }
        
        public static class _schemaversionParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._schemaversion> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._schemaversion>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._schemaversion> Parse(IInput<char>? input)
                {
                    var _schemaversion_1 = __GeneratedOdataV3.Parsers.Rules._schemaversionParser.Instance.Parse(input);
if (!_schemaversion_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._schemaversion)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._schemaversion(_schemaversion_1.Parsed), _schemaversion_1.Remainder);
                }
            }
        }
        
        public static class _searchParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._search> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._search>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._search> Parse(IInput<char>? input)
                {
                    var _search_1 = __GeneratedOdataV3.Parsers.Rules._searchParser.Instance.Parse(input);
if (!_search_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._search)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._search(_search_1.Parsed), _search_1.Remainder);
                }
            }
        }
        
        public static class _selectParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._select> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._select>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._select> Parse(IInput<char>? input)
                {
                    var _select_1 = __GeneratedOdataV3.Parsers.Rules._selectParser.Instance.Parse(input);
if (!_select_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._select)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._select(_select_1.Parsed), _select_1.Remainder);
                }
            }
        }
        
        public static class _skipParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skip> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skip>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skip> Parse(IInput<char>? input)
                {
                    var _skip_1 = __GeneratedOdataV3.Parsers.Rules._skipParser.Instance.Parse(input);
if (!_skip_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skip)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skip(_skip_1.Parsed), _skip_1.Remainder);
                }
            }
        }
        
        public static class _skiptokenParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skiptoken> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skiptoken>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skiptoken> Parse(IInput<char>? input)
                {
                    var _skiptoken_1 = __GeneratedOdataV3.Parsers.Rules._skiptokenParser.Instance.Parse(input);
if (!_skiptoken_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skiptoken)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._skiptoken(_skiptoken_1.Parsed), _skiptoken_1.Remainder);
                }
            }
        }
        
        public static class _topParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._top> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._top>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._top> Parse(IInput<char>? input)
                {
                    var _top_1 = __GeneratedOdataV3.Parsers.Rules._topParser.Instance.Parse(input);
if (!_top_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._top)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._top(_top_1.Parsed), _top_1.Remainder);
                }
            }
        }
        
        public static class _indexParser
        {
            public static IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._index> Instance { get; } = new Parser();
            
            private sealed class Parser : IParser<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._index>
            {
                public Parser()
                {
                }
                
                public IOutput<char, __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._index> Parse(IInput<char>? input)
                {
                    var _index_1 = __GeneratedOdataV3.Parsers.Rules._indexParser.Instance.Parse(input);
if (!_index_1.Success)
{
    return Output.Create(false, default(__GeneratedOdataV3.CstNodes.Rules._systemQueryOption._index)!, input);
}

return Output.Create(true, new __GeneratedOdataV3.CstNodes.Rules._systemQueryOption._index(_index_1.Parsed), _index_1.Remainder);
                }
            }
        }
    }
    
}
