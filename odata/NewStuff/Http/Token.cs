namespace NewStuff.Http
{
    using NewStuff.Http.Inners;
    using System.Collections.Generic;

    /// <summary>
    /// https://www.rfc-editor.org/rfc/rfc2616#section-2.2
    /// </summary>
    public sealed class Token
    {
        public Token(TokenChar firstCharacter, IEnumerable<TokenChar> subsequentCharacters)
        {
            FirstCharacter = firstCharacter;
            SubsequentCharacters = subsequentCharacters;
        }

        public TokenChar FirstCharacter { get; }
        public IEnumerable<TokenChar> SubsequentCharacters { get; }
    }
}
