namespace __GeneratedOdataV4.Trancsribers.Inners
{
    public sealed class _searchPhraseⳆsearchWordTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord>
    {
        private _searchPhraseⳆsearchWordTranscriber()
        {
        }
        
        public static _searchPhraseⳆsearchWordTranscriber Instance { get; } = new _searchPhraseⳆsearchWordTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord value, System.Text.StringBuilder builder)
        {
            Visitor.Instance.Visit(value, builder);
        }
        
        private sealed class Visitor : __GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord.Visitor<Root.Void, System.Text.StringBuilder>
        {
            private Visitor()
            {
            }
            
            public static Visitor Instance { get; } = new Visitor();
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchPhrase node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._searchPhraseTranscriber.Instance.Transcribe(node._searchPhrase_1, context);

return default;
            }
            
            protected internal override Root.Void Accept(__GeneratedOdataV4.CstNodes.Inners._searchPhraseⳆsearchWord._searchWord node, System.Text.StringBuilder context)
            {
                __GeneratedOdataV4.Trancsribers.Rules._searchWordTranscriber.Instance.Transcribe(node._searchWord_1, context);

return default;
            }
        }
    }
    
}
