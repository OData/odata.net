using __GeneratedOdata.CstNodes.Inners;

namespace CombinatorParsingV2
{
    //// TODO covariance and contravariance

    //// TODO stuff that you did to improve performance:
    //// charparser

    //// TODO update generator with stuff that *definitely* worked:
    //// remove selectmany
    //// more singletons

    //// TODO you need to make structs and ref structs v3 and v4
    //// TODO structs made performance worse and in made it even worse
    //// TODO ref structs for parser types
    //// TODO structs for node types?
    
    public interface IParser<TToken, out TParsed>
    {
        IOutput<TToken, TParsed> Parse(IInput<TToken>? input); //// TODO would it make sense to have a TInput and a IIndex? that way the index can be as small as an int, so there's no trade-off with using a struct? iindex would have a `TToken current(TInput)` method or something
    }

    public interface IInput<out TToken> //// TODO make a struct an use `in` parameter for `iparser.parse` method?
    {
        TToken Current { get; }

        IInput<TToken>? Next();
    }

    public interface IOutput<out TToken, out TParsed> //// TODO make a struct and use `in` parameter whereever applicable?
    {
        bool Success { get; }

        public TParsed Parsed { get; }

        IInput<TToken>? Remainder { get; }
    }
}
