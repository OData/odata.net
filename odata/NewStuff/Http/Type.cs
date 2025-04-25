namespace NewStuff.Http
{
    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-3.7
    /// </summary>
    public sealed class Type
    {
        public Type(Token token)
        {
            Token = token;
        }

        public Token Token { get; }
    }
}
