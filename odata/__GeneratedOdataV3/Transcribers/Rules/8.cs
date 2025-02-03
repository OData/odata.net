namespace __GeneratedOdataV3.Trancsribers.Rules
{
    public sealed class _simpleKeyTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV3.CstNodes.Rules._simpleKey>
    {
        private _simpleKeyTranscriber()
        {
        }
        
        public static _simpleKeyTranscriber Instance { get; } = new _simpleKeyTranscriber();
        
        public void Transcribe(__GeneratedOdataV3.CstNodes.Rules._simpleKey value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV3.Trancsribers.Rules._OPENTranscriber.Instance.Transcribe(value._OPEN_1, builder);
__GeneratedOdataV3.Trancsribers.Inners._ⲤparameterAliasⳆkeyPropertyValueↃTranscriber.Instance.Transcribe(value._ⲤparameterAliasⳆkeyPropertyValueↃ_1, builder);
__GeneratedOdataV3.Trancsribers.Rules._CLOSETranscriber.Instance.Transcribe(value._CLOSE_1, builder);

        }
    }
    
}
