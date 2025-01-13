namespace __GeneratedOdata.CstNodes.Rules
{
    public abstract class _expandItem
    {
        private _expandItem()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(_expandItem node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(_expandItem._STAR_꘡refⳆOPEN_levels_CLOSE꘡ node, TContext context);
            protected internal abstract TResult Accept(_expandItem._ʺx24x76x61x6Cx75x65ʺ node, TContext context);
            protected internal abstract TResult Accept(_expandItem._expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡ node, TContext context);
        }
        
        public sealed class _STAR_꘡refⳆOPEN_levels_CLOSE꘡ : _expandItem
        {
            public _STAR_꘡refⳆOPEN_levels_CLOSE꘡(__GeneratedOdata.CstNodes.Rules._STAR _STAR_1, __GeneratedOdata.CstNodes.Inners._refⳆOPEN_levels_CLOSE? _refⳆOPEN_levels_CLOSE_1)
            {
                this._STAR_1 = _STAR_1;
                this._refⳆOPEN_levels_CLOSE_1 = _refⳆOPEN_levels_CLOSE_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._STAR _STAR_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._refⳆOPEN_levels_CLOSE? _refⳆOPEN_levels_CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _ʺx24x76x61x6Cx75x65ʺ : _expandItem
        {
            public _ʺx24x76x61x6Cx75x65ʺ(__GeneratedOdata.CstNodes.Inners._ʺx24x76x61x6Cx75x65ʺ _ʺx24x76x61x6Cx75x65ʺ_1)
            {
                this._ʺx24x76x61x6Cx75x65ʺ_1 = _ʺx24x76x61x6Cx75x65ʺ_1;
            }
            
            public __GeneratedOdata.CstNodes.Inners._ʺx24x76x61x6Cx75x65ʺ _ʺx24x76x61x6Cx75x65ʺ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class _expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡ : _expandItem
        {
            public _expandPath_꘡ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE꘡(__GeneratedOdata.CstNodes.Rules._expandPath _expandPath_1, __GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE? _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1)
            {
                this._expandPath_1 = _expandPath_1;
                this._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 = _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1;
            }
            
            public __GeneratedOdata.CstNodes.Rules._expandPath _expandPath_1 { get; }
            public __GeneratedOdata.CstNodes.Inners._ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE? _ref_꘡OPEN_expandRefOption_ЖⲤSEMI_expandRefOptionↃ_CLOSE꘡Ⳇcount_꘡OPEN_expandCountOption_ЖⲤSEMI_expandCountOptionↃ_CLOSE꘡ⳆOPEN_expandOption_ЖⲤSEMI_expandOptionↃ_CLOSE_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
}
