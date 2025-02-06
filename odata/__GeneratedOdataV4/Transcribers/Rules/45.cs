namespace __GeneratedOdataV4.Trancsribers.Rules
{
    public sealed class _functionParameterTranscriber : GeneratorV3.ITranscriber<__GeneratedOdataV4.CstNodes.Rules._functionParameter>
    {
        private _functionParameterTranscriber()
        {
        }
        
        public static _functionParameterTranscriber Instance { get; } = new _functionParameterTranscriber();
        
        public void Transcribe(__GeneratedOdataV4.CstNodes.Rules._functionParameter value, System.Text.StringBuilder builder)
        {
            __GeneratedOdataV4.Trancsribers.Rules._parameterNameTranscriber.Instance.Transcribe(value._parameterName_1, builder);
__GeneratedOdataV4.Trancsribers.Rules._EQTranscriber.Instance.Transcribe(value._EQ_1, builder);
__GeneratedOdataV4.Trancsribers.Inners._ⲤparameterAliasⳆprimitiveLiteralↃTranscriber.Instance.Transcribe(value._ⲤparameterAliasⳆprimitiveLiteralↃ_1, builder);

        }
    }
    
}
