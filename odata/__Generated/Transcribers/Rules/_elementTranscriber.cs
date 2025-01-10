namespace __Generated.Trancsribers.Rules
{
    public sealed class _elementTranscriber : GeneratorV3.ITranscriber<__Generated.CstNodes.Rules._element>
    {
        private _elementTranscriber()
        {
        }
        
        public static _elementTranscriber Instance { get; } = new _elementTranscriber();
        
        public void Transcribe(__Generated.CstNodes.Rules._element value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __Generated.CstNodes.Rules._element.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._rulename node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._rulenameTranscriber.Instance.Transcribe(node._rulename_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._group node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._groupTranscriber.Instance.Transcribe(node._group_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._option node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._optionTranscriber.Instance.Transcribe(node._option_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._charⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._charⲻvalTranscriber.Instance.Transcribe(node._charⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._numⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._numⲻvalTranscriber.Instance.Transcribe(node._numⲻval_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__Generated.CstNodes.Rules._element._proseⲻval node, System.Text.StringBuilder context)
            {
                __Generated.Trancsribers.Rules._proseⲻvalTranscriber.Instance.Transcribe(node._proseⲻval_1, context);

return default;
            }
        }
    }
    
}
