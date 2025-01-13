namespace __GeneratedOdata.Trancsribers.Rules
{
    public sealed class _preferenceTranscriber : GeneratorV3.ITranscriber<__GeneratedOdata.CstNodes.Rules._preference>
    {
        private _preferenceTranscriber()
        {
        }
        
        public static _preferenceTranscriber Instance { get; } = new _preferenceTranscriber();
        
        public void Transcribe(__GeneratedOdata.CstNodes.Rules._preference value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdata.CstNodes.Rules._preference.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._allowEntityReferencesPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._allowEntityReferencesPreferenceTranscriber.Instance.Transcribe(node._allowEntityReferencesPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._callbackPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._callbackPreferenceTranscriber.Instance.Transcribe(node._callbackPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._continueOnErrorPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._continueOnErrorPreferenceTranscriber.Instance.Transcribe(node._continueOnErrorPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._includeAnnotationsPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._includeAnnotationsPreferenceTranscriber.Instance.Transcribe(node._includeAnnotationsPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._maxpagesizePreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._maxpagesizePreferenceTranscriber.Instance.Transcribe(node._maxpagesizePreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._respondAsyncPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._respondAsyncPreferenceTranscriber.Instance.Transcribe(node._respondAsyncPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._returnPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._returnPreferenceTranscriber.Instance.Transcribe(node._returnPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._trackChangesPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._trackChangesPreferenceTranscriber.Instance.Transcribe(node._trackChangesPreference_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdata.CstNodes.Rules._preference._waitPreference node, System.Text.StringBuilder context)
            {
                __GeneratedOdata.Trancsribers.Rules._waitPreferenceTranscriber.Instance.Transcribe(node._waitPreference_1, context);

return default;
            }
        }
    }
    
}
