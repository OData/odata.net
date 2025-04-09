namespace __GeneratedPartialV1.Deferred.CstNodes.Inners
{
    using System;
    using CombinatorParsingV3;
    
    public sealed class _Ⰳx00ⲻFFDeferred : IAstNode<char, _Ⰳx00ⲻFFRealized>
    {
        public _Ⰳx00ⲻFFDeferred(IFuture<IRealizationResult<char>> previousNodeRealizationResult)
        {
            //// TODO get constructor accessibility correct
            this.previousNodeRealizationResult = previousNodeRealizationResult;
            
            this.realizationResult = Future.Create(() => this.RealizeImpl());
        }
        public _Ⰳx00ⲻFFDeferred(IFuture<IRealizationResult<char, _Ⰳx00ⲻFFRealized>> realizationResult)
        {
            this.realizationResult = realizationResult;
        }
        
        private IFuture<IRealizationResult<char>> previousNodeRealizationResult { get; }
        private IFuture<IRealizationResult<char, _Ⰳx00ⲻFFRealized>> realizationResult { get; }
        
        public IRealizationResult<char, _Ⰳx00ⲻFFRealized> Realize()
        {
            return this.realizationResult.Value;
        }
        
        private IRealizationResult<char, _Ⰳx00ⲻFFRealized> RealizeImpl()
        {
            if (!this.previousNodeRealizationResult.Value.Success)
{
    return new RealizationResult<char, _Ⰳx00ⲻFFRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
}
var _00 = _Ⰳx00ⲻFFRealized._00.Create(this.previousNodeRealizationResult);
if (_00.Success)
{
    return _00;
}

var _01 = _Ⰳx00ⲻFFRealized._01.Create(this.previousNodeRealizationResult);
if (_01.Success)
{
    return _01;
}

var _02 = _Ⰳx00ⲻFFRealized._02.Create(this.previousNodeRealizationResult);
if (_02.Success)
{
    return _02;
}

var _03 = _Ⰳx00ⲻFFRealized._03.Create(this.previousNodeRealizationResult);
if (_03.Success)
{
    return _03;
}

var _04 = _Ⰳx00ⲻFFRealized._04.Create(this.previousNodeRealizationResult);
if (_04.Success)
{
    return _04;
}

var _05 = _Ⰳx00ⲻFFRealized._05.Create(this.previousNodeRealizationResult);
if (_05.Success)
{
    return _05;
}

var _06 = _Ⰳx00ⲻFFRealized._06.Create(this.previousNodeRealizationResult);
if (_06.Success)
{
    return _06;
}

var _07 = _Ⰳx00ⲻFFRealized._07.Create(this.previousNodeRealizationResult);
if (_07.Success)
{
    return _07;
}

var _08 = _Ⰳx00ⲻFFRealized._08.Create(this.previousNodeRealizationResult);
if (_08.Success)
{
    return _08;
}

var _09 = _Ⰳx00ⲻFFRealized._09.Create(this.previousNodeRealizationResult);
if (_09.Success)
{
    return _09;
}

var _0A = _Ⰳx00ⲻFFRealized._0A.Create(this.previousNodeRealizationResult);
if (_0A.Success)
{
    return _0A;
}

var _0B = _Ⰳx00ⲻFFRealized._0B.Create(this.previousNodeRealizationResult);
if (_0B.Success)
{
    return _0B;
}

var _0C = _Ⰳx00ⲻFFRealized._0C.Create(this.previousNodeRealizationResult);
if (_0C.Success)
{
    return _0C;
}

var _0D = _Ⰳx00ⲻFFRealized._0D.Create(this.previousNodeRealizationResult);
if (_0D.Success)
{
    return _0D;
}

var _0E = _Ⰳx00ⲻFFRealized._0E.Create(this.previousNodeRealizationResult);
if (_0E.Success)
{
    return _0E;
}

var _0F = _Ⰳx00ⲻFFRealized._0F.Create(this.previousNodeRealizationResult);
if (_0F.Success)
{
    return _0F;
}

var _10 = _Ⰳx00ⲻFFRealized._10.Create(this.previousNodeRealizationResult);
if (_10.Success)
{
    return _10;
}

var _11 = _Ⰳx00ⲻFFRealized._11.Create(this.previousNodeRealizationResult);
if (_11.Success)
{
    return _11;
}

var _12 = _Ⰳx00ⲻFFRealized._12.Create(this.previousNodeRealizationResult);
if (_12.Success)
{
    return _12;
}

var _13 = _Ⰳx00ⲻFFRealized._13.Create(this.previousNodeRealizationResult);
if (_13.Success)
{
    return _13;
}

var _14 = _Ⰳx00ⲻFFRealized._14.Create(this.previousNodeRealizationResult);
if (_14.Success)
{
    return _14;
}

var _15 = _Ⰳx00ⲻFFRealized._15.Create(this.previousNodeRealizationResult);
if (_15.Success)
{
    return _15;
}

var _16 = _Ⰳx00ⲻFFRealized._16.Create(this.previousNodeRealizationResult);
if (_16.Success)
{
    return _16;
}

var _17 = _Ⰳx00ⲻFFRealized._17.Create(this.previousNodeRealizationResult);
if (_17.Success)
{
    return _17;
}

var _18 = _Ⰳx00ⲻFFRealized._18.Create(this.previousNodeRealizationResult);
if (_18.Success)
{
    return _18;
}

var _19 = _Ⰳx00ⲻFFRealized._19.Create(this.previousNodeRealizationResult);
if (_19.Success)
{
    return _19;
}

var _1A = _Ⰳx00ⲻFFRealized._1A.Create(this.previousNodeRealizationResult);
if (_1A.Success)
{
    return _1A;
}

var _1B = _Ⰳx00ⲻFFRealized._1B.Create(this.previousNodeRealizationResult);
if (_1B.Success)
{
    return _1B;
}

var _1C = _Ⰳx00ⲻFFRealized._1C.Create(this.previousNodeRealizationResult);
if (_1C.Success)
{
    return _1C;
}

var _1D = _Ⰳx00ⲻFFRealized._1D.Create(this.previousNodeRealizationResult);
if (_1D.Success)
{
    return _1D;
}

var _1E = _Ⰳx00ⲻFFRealized._1E.Create(this.previousNodeRealizationResult);
if (_1E.Success)
{
    return _1E;
}

var _1F = _Ⰳx00ⲻFFRealized._1F.Create(this.previousNodeRealizationResult);
if (_1F.Success)
{
    return _1F;
}

var _20 = _Ⰳx00ⲻFFRealized._20.Create(this.previousNodeRealizationResult);
if (_20.Success)
{
    return _20;
}

var _21 = _Ⰳx00ⲻFFRealized._21.Create(this.previousNodeRealizationResult);
if (_21.Success)
{
    return _21;
}

var _22 = _Ⰳx00ⲻFFRealized._22.Create(this.previousNodeRealizationResult);
if (_22.Success)
{
    return _22;
}

var _23 = _Ⰳx00ⲻFFRealized._23.Create(this.previousNodeRealizationResult);
if (_23.Success)
{
    return _23;
}

var _24 = _Ⰳx00ⲻFFRealized._24.Create(this.previousNodeRealizationResult);
if (_24.Success)
{
    return _24;
}

var _25 = _Ⰳx00ⲻFFRealized._25.Create(this.previousNodeRealizationResult);
if (_25.Success)
{
    return _25;
}

var _26 = _Ⰳx00ⲻFFRealized._26.Create(this.previousNodeRealizationResult);
if (_26.Success)
{
    return _26;
}

var _27 = _Ⰳx00ⲻFFRealized._27.Create(this.previousNodeRealizationResult);
if (_27.Success)
{
    return _27;
}

var _28 = _Ⰳx00ⲻFFRealized._28.Create(this.previousNodeRealizationResult);
if (_28.Success)
{
    return _28;
}

var _29 = _Ⰳx00ⲻFFRealized._29.Create(this.previousNodeRealizationResult);
if (_29.Success)
{
    return _29;
}

var _2A = _Ⰳx00ⲻFFRealized._2A.Create(this.previousNodeRealizationResult);
if (_2A.Success)
{
    return _2A;
}

var _2B = _Ⰳx00ⲻFFRealized._2B.Create(this.previousNodeRealizationResult);
if (_2B.Success)
{
    return _2B;
}

var _2C = _Ⰳx00ⲻFFRealized._2C.Create(this.previousNodeRealizationResult);
if (_2C.Success)
{
    return _2C;
}

var _2D = _Ⰳx00ⲻFFRealized._2D.Create(this.previousNodeRealizationResult);
if (_2D.Success)
{
    return _2D;
}

var _2E = _Ⰳx00ⲻFFRealized._2E.Create(this.previousNodeRealizationResult);
if (_2E.Success)
{
    return _2E;
}

var _2F = _Ⰳx00ⲻFFRealized._2F.Create(this.previousNodeRealizationResult);
if (_2F.Success)
{
    return _2F;
}

var _30 = _Ⰳx00ⲻFFRealized._30.Create(this.previousNodeRealizationResult);
if (_30.Success)
{
    return _30;
}

var _31 = _Ⰳx00ⲻFFRealized._31.Create(this.previousNodeRealizationResult);
if (_31.Success)
{
    return _31;
}

var _32 = _Ⰳx00ⲻFFRealized._32.Create(this.previousNodeRealizationResult);
if (_32.Success)
{
    return _32;
}

var _33 = _Ⰳx00ⲻFFRealized._33.Create(this.previousNodeRealizationResult);
if (_33.Success)
{
    return _33;
}

var _34 = _Ⰳx00ⲻFFRealized._34.Create(this.previousNodeRealizationResult);
if (_34.Success)
{
    return _34;
}

var _35 = _Ⰳx00ⲻFFRealized._35.Create(this.previousNodeRealizationResult);
if (_35.Success)
{
    return _35;
}

var _36 = _Ⰳx00ⲻFFRealized._36.Create(this.previousNodeRealizationResult);
if (_36.Success)
{
    return _36;
}

var _37 = _Ⰳx00ⲻFFRealized._37.Create(this.previousNodeRealizationResult);
if (_37.Success)
{
    return _37;
}

var _38 = _Ⰳx00ⲻFFRealized._38.Create(this.previousNodeRealizationResult);
if (_38.Success)
{
    return _38;
}

var _39 = _Ⰳx00ⲻFFRealized._39.Create(this.previousNodeRealizationResult);
if (_39.Success)
{
    return _39;
}

var _3A = _Ⰳx00ⲻFFRealized._3A.Create(this.previousNodeRealizationResult);
if (_3A.Success)
{
    return _3A;
}

var _3B = _Ⰳx00ⲻFFRealized._3B.Create(this.previousNodeRealizationResult);
if (_3B.Success)
{
    return _3B;
}

var _3C = _Ⰳx00ⲻFFRealized._3C.Create(this.previousNodeRealizationResult);
if (_3C.Success)
{
    return _3C;
}

var _3D = _Ⰳx00ⲻFFRealized._3D.Create(this.previousNodeRealizationResult);
if (_3D.Success)
{
    return _3D;
}

var _3E = _Ⰳx00ⲻFFRealized._3E.Create(this.previousNodeRealizationResult);
if (_3E.Success)
{
    return _3E;
}

var _3F = _Ⰳx00ⲻFFRealized._3F.Create(this.previousNodeRealizationResult);
if (_3F.Success)
{
    return _3F;
}

var _40 = _Ⰳx00ⲻFFRealized._40.Create(this.previousNodeRealizationResult);
if (_40.Success)
{
    return _40;
}

var _41 = _Ⰳx00ⲻFFRealized._41.Create(this.previousNodeRealizationResult);
if (_41.Success)
{
    return _41;
}

var _42 = _Ⰳx00ⲻFFRealized._42.Create(this.previousNodeRealizationResult);
if (_42.Success)
{
    return _42;
}

var _43 = _Ⰳx00ⲻFFRealized._43.Create(this.previousNodeRealizationResult);
if (_43.Success)
{
    return _43;
}

var _44 = _Ⰳx00ⲻFFRealized._44.Create(this.previousNodeRealizationResult);
if (_44.Success)
{
    return _44;
}

var _45 = _Ⰳx00ⲻFFRealized._45.Create(this.previousNodeRealizationResult);
if (_45.Success)
{
    return _45;
}

var _46 = _Ⰳx00ⲻFFRealized._46.Create(this.previousNodeRealizationResult);
if (_46.Success)
{
    return _46;
}

var _47 = _Ⰳx00ⲻFFRealized._47.Create(this.previousNodeRealizationResult);
if (_47.Success)
{
    return _47;
}

var _48 = _Ⰳx00ⲻFFRealized._48.Create(this.previousNodeRealizationResult);
if (_48.Success)
{
    return _48;
}

var _49 = _Ⰳx00ⲻFFRealized._49.Create(this.previousNodeRealizationResult);
if (_49.Success)
{
    return _49;
}

var _4A = _Ⰳx00ⲻFFRealized._4A.Create(this.previousNodeRealizationResult);
if (_4A.Success)
{
    return _4A;
}

var _4B = _Ⰳx00ⲻFFRealized._4B.Create(this.previousNodeRealizationResult);
if (_4B.Success)
{
    return _4B;
}

var _4C = _Ⰳx00ⲻFFRealized._4C.Create(this.previousNodeRealizationResult);
if (_4C.Success)
{
    return _4C;
}

var _4D = _Ⰳx00ⲻFFRealized._4D.Create(this.previousNodeRealizationResult);
if (_4D.Success)
{
    return _4D;
}

var _4E = _Ⰳx00ⲻFFRealized._4E.Create(this.previousNodeRealizationResult);
if (_4E.Success)
{
    return _4E;
}

var _4F = _Ⰳx00ⲻFFRealized._4F.Create(this.previousNodeRealizationResult);
if (_4F.Success)
{
    return _4F;
}

var _50 = _Ⰳx00ⲻFFRealized._50.Create(this.previousNodeRealizationResult);
if (_50.Success)
{
    return _50;
}

var _51 = _Ⰳx00ⲻFFRealized._51.Create(this.previousNodeRealizationResult);
if (_51.Success)
{
    return _51;
}

var _52 = _Ⰳx00ⲻFFRealized._52.Create(this.previousNodeRealizationResult);
if (_52.Success)
{
    return _52;
}

var _53 = _Ⰳx00ⲻFFRealized._53.Create(this.previousNodeRealizationResult);
if (_53.Success)
{
    return _53;
}

var _54 = _Ⰳx00ⲻFFRealized._54.Create(this.previousNodeRealizationResult);
if (_54.Success)
{
    return _54;
}

var _55 = _Ⰳx00ⲻFFRealized._55.Create(this.previousNodeRealizationResult);
if (_55.Success)
{
    return _55;
}

var _56 = _Ⰳx00ⲻFFRealized._56.Create(this.previousNodeRealizationResult);
if (_56.Success)
{
    return _56;
}

var _57 = _Ⰳx00ⲻFFRealized._57.Create(this.previousNodeRealizationResult);
if (_57.Success)
{
    return _57;
}

var _58 = _Ⰳx00ⲻFFRealized._58.Create(this.previousNodeRealizationResult);
if (_58.Success)
{
    return _58;
}

var _59 = _Ⰳx00ⲻFFRealized._59.Create(this.previousNodeRealizationResult);
if (_59.Success)
{
    return _59;
}

var _5A = _Ⰳx00ⲻFFRealized._5A.Create(this.previousNodeRealizationResult);
if (_5A.Success)
{
    return _5A;
}

var _5B = _Ⰳx00ⲻFFRealized._5B.Create(this.previousNodeRealizationResult);
if (_5B.Success)
{
    return _5B;
}

var _5C = _Ⰳx00ⲻFFRealized._5C.Create(this.previousNodeRealizationResult);
if (_5C.Success)
{
    return _5C;
}

var _5D = _Ⰳx00ⲻFFRealized._5D.Create(this.previousNodeRealizationResult);
if (_5D.Success)
{
    return _5D;
}

var _5E = _Ⰳx00ⲻFFRealized._5E.Create(this.previousNodeRealizationResult);
if (_5E.Success)
{
    return _5E;
}

var _5F = _Ⰳx00ⲻFFRealized._5F.Create(this.previousNodeRealizationResult);
if (_5F.Success)
{
    return _5F;
}

var _60 = _Ⰳx00ⲻFFRealized._60.Create(this.previousNodeRealizationResult);
if (_60.Success)
{
    return _60;
}

var _61 = _Ⰳx00ⲻFFRealized._61.Create(this.previousNodeRealizationResult);
if (_61.Success)
{
    return _61;
}

var _62 = _Ⰳx00ⲻFFRealized._62.Create(this.previousNodeRealizationResult);
if (_62.Success)
{
    return _62;
}

var _63 = _Ⰳx00ⲻFFRealized._63.Create(this.previousNodeRealizationResult);
if (_63.Success)
{
    return _63;
}

var _64 = _Ⰳx00ⲻFFRealized._64.Create(this.previousNodeRealizationResult);
if (_64.Success)
{
    return _64;
}

var _65 = _Ⰳx00ⲻFFRealized._65.Create(this.previousNodeRealizationResult);
if (_65.Success)
{
    return _65;
}

var _66 = _Ⰳx00ⲻFFRealized._66.Create(this.previousNodeRealizationResult);
if (_66.Success)
{
    return _66;
}

var _67 = _Ⰳx00ⲻFFRealized._67.Create(this.previousNodeRealizationResult);
if (_67.Success)
{
    return _67;
}

var _68 = _Ⰳx00ⲻFFRealized._68.Create(this.previousNodeRealizationResult);
if (_68.Success)
{
    return _68;
}

var _69 = _Ⰳx00ⲻFFRealized._69.Create(this.previousNodeRealizationResult);
if (_69.Success)
{
    return _69;
}

var _6A = _Ⰳx00ⲻFFRealized._6A.Create(this.previousNodeRealizationResult);
if (_6A.Success)
{
    return _6A;
}

var _6B = _Ⰳx00ⲻFFRealized._6B.Create(this.previousNodeRealizationResult);
if (_6B.Success)
{
    return _6B;
}

var _6C = _Ⰳx00ⲻFFRealized._6C.Create(this.previousNodeRealizationResult);
if (_6C.Success)
{
    return _6C;
}

var _6D = _Ⰳx00ⲻFFRealized._6D.Create(this.previousNodeRealizationResult);
if (_6D.Success)
{
    return _6D;
}

var _6E = _Ⰳx00ⲻFFRealized._6E.Create(this.previousNodeRealizationResult);
if (_6E.Success)
{
    return _6E;
}

var _6F = _Ⰳx00ⲻFFRealized._6F.Create(this.previousNodeRealizationResult);
if (_6F.Success)
{
    return _6F;
}

var _70 = _Ⰳx00ⲻFFRealized._70.Create(this.previousNodeRealizationResult);
if (_70.Success)
{
    return _70;
}

var _71 = _Ⰳx00ⲻFFRealized._71.Create(this.previousNodeRealizationResult);
if (_71.Success)
{
    return _71;
}

var _72 = _Ⰳx00ⲻFFRealized._72.Create(this.previousNodeRealizationResult);
if (_72.Success)
{
    return _72;
}

var _73 = _Ⰳx00ⲻFFRealized._73.Create(this.previousNodeRealizationResult);
if (_73.Success)
{
    return _73;
}

var _74 = _Ⰳx00ⲻFFRealized._74.Create(this.previousNodeRealizationResult);
if (_74.Success)
{
    return _74;
}

var _75 = _Ⰳx00ⲻFFRealized._75.Create(this.previousNodeRealizationResult);
if (_75.Success)
{
    return _75;
}

var _76 = _Ⰳx00ⲻFFRealized._76.Create(this.previousNodeRealizationResult);
if (_76.Success)
{
    return _76;
}

var _77 = _Ⰳx00ⲻFFRealized._77.Create(this.previousNodeRealizationResult);
if (_77.Success)
{
    return _77;
}

var _78 = _Ⰳx00ⲻFFRealized._78.Create(this.previousNodeRealizationResult);
if (_78.Success)
{
    return _78;
}

var _79 = _Ⰳx00ⲻFFRealized._79.Create(this.previousNodeRealizationResult);
if (_79.Success)
{
    return _79;
}

var _7A = _Ⰳx00ⲻFFRealized._7A.Create(this.previousNodeRealizationResult);
if (_7A.Success)
{
    return _7A;
}

var _7B = _Ⰳx00ⲻFFRealized._7B.Create(this.previousNodeRealizationResult);
if (_7B.Success)
{
    return _7B;
}

var _7C = _Ⰳx00ⲻFFRealized._7C.Create(this.previousNodeRealizationResult);
if (_7C.Success)
{
    return _7C;
}

var _7D = _Ⰳx00ⲻFFRealized._7D.Create(this.previousNodeRealizationResult);
if (_7D.Success)
{
    return _7D;
}

var _7E = _Ⰳx00ⲻFFRealized._7E.Create(this.previousNodeRealizationResult);
if (_7E.Success)
{
    return _7E;
}

var _7F = _Ⰳx00ⲻFFRealized._7F.Create(this.previousNodeRealizationResult);
if (_7F.Success)
{
    return _7F;
}

var _80 = _Ⰳx00ⲻFFRealized._80.Create(this.previousNodeRealizationResult);
if (_80.Success)
{
    return _80;
}

var _81 = _Ⰳx00ⲻFFRealized._81.Create(this.previousNodeRealizationResult);
if (_81.Success)
{
    return _81;
}

var _82 = _Ⰳx00ⲻFFRealized._82.Create(this.previousNodeRealizationResult);
if (_82.Success)
{
    return _82;
}

var _83 = _Ⰳx00ⲻFFRealized._83.Create(this.previousNodeRealizationResult);
if (_83.Success)
{
    return _83;
}

var _84 = _Ⰳx00ⲻFFRealized._84.Create(this.previousNodeRealizationResult);
if (_84.Success)
{
    return _84;
}

var _85 = _Ⰳx00ⲻFFRealized._85.Create(this.previousNodeRealizationResult);
if (_85.Success)
{
    return _85;
}

var _86 = _Ⰳx00ⲻFFRealized._86.Create(this.previousNodeRealizationResult);
if (_86.Success)
{
    return _86;
}

var _87 = _Ⰳx00ⲻFFRealized._87.Create(this.previousNodeRealizationResult);
if (_87.Success)
{
    return _87;
}

var _88 = _Ⰳx00ⲻFFRealized._88.Create(this.previousNodeRealizationResult);
if (_88.Success)
{
    return _88;
}

var _89 = _Ⰳx00ⲻFFRealized._89.Create(this.previousNodeRealizationResult);
if (_89.Success)
{
    return _89;
}

var _8A = _Ⰳx00ⲻFFRealized._8A.Create(this.previousNodeRealizationResult);
if (_8A.Success)
{
    return _8A;
}

var _8B = _Ⰳx00ⲻFFRealized._8B.Create(this.previousNodeRealizationResult);
if (_8B.Success)
{
    return _8B;
}

var _8C = _Ⰳx00ⲻFFRealized._8C.Create(this.previousNodeRealizationResult);
if (_8C.Success)
{
    return _8C;
}

var _8D = _Ⰳx00ⲻFFRealized._8D.Create(this.previousNodeRealizationResult);
if (_8D.Success)
{
    return _8D;
}

var _8E = _Ⰳx00ⲻFFRealized._8E.Create(this.previousNodeRealizationResult);
if (_8E.Success)
{
    return _8E;
}

var _8F = _Ⰳx00ⲻFFRealized._8F.Create(this.previousNodeRealizationResult);
if (_8F.Success)
{
    return _8F;
}

var _90 = _Ⰳx00ⲻFFRealized._90.Create(this.previousNodeRealizationResult);
if (_90.Success)
{
    return _90;
}

var _91 = _Ⰳx00ⲻFFRealized._91.Create(this.previousNodeRealizationResult);
if (_91.Success)
{
    return _91;
}

var _92 = _Ⰳx00ⲻFFRealized._92.Create(this.previousNodeRealizationResult);
if (_92.Success)
{
    return _92;
}

var _93 = _Ⰳx00ⲻFFRealized._93.Create(this.previousNodeRealizationResult);
if (_93.Success)
{
    return _93;
}

var _94 = _Ⰳx00ⲻFFRealized._94.Create(this.previousNodeRealizationResult);
if (_94.Success)
{
    return _94;
}

var _95 = _Ⰳx00ⲻFFRealized._95.Create(this.previousNodeRealizationResult);
if (_95.Success)
{
    return _95;
}

var _96 = _Ⰳx00ⲻFFRealized._96.Create(this.previousNodeRealizationResult);
if (_96.Success)
{
    return _96;
}

var _97 = _Ⰳx00ⲻFFRealized._97.Create(this.previousNodeRealizationResult);
if (_97.Success)
{
    return _97;
}

var _98 = _Ⰳx00ⲻFFRealized._98.Create(this.previousNodeRealizationResult);
if (_98.Success)
{
    return _98;
}

var _99 = _Ⰳx00ⲻFFRealized._99.Create(this.previousNodeRealizationResult);
if (_99.Success)
{
    return _99;
}

var _9A = _Ⰳx00ⲻFFRealized._9A.Create(this.previousNodeRealizationResult);
if (_9A.Success)
{
    return _9A;
}

var _9B = _Ⰳx00ⲻFFRealized._9B.Create(this.previousNodeRealizationResult);
if (_9B.Success)
{
    return _9B;
}

var _9C = _Ⰳx00ⲻFFRealized._9C.Create(this.previousNodeRealizationResult);
if (_9C.Success)
{
    return _9C;
}

var _9D = _Ⰳx00ⲻFFRealized._9D.Create(this.previousNodeRealizationResult);
if (_9D.Success)
{
    return _9D;
}

var _9E = _Ⰳx00ⲻFFRealized._9E.Create(this.previousNodeRealizationResult);
if (_9E.Success)
{
    return _9E;
}

var _9F = _Ⰳx00ⲻFFRealized._9F.Create(this.previousNodeRealizationResult);
if (_9F.Success)
{
    return _9F;
}

var _A0 = _Ⰳx00ⲻFFRealized._A0.Create(this.previousNodeRealizationResult);
if (_A0.Success)
{
    return _A0;
}

var _A1 = _Ⰳx00ⲻFFRealized._A1.Create(this.previousNodeRealizationResult);
if (_A1.Success)
{
    return _A1;
}

var _A2 = _Ⰳx00ⲻFFRealized._A2.Create(this.previousNodeRealizationResult);
if (_A2.Success)
{
    return _A2;
}

var _A3 = _Ⰳx00ⲻFFRealized._A3.Create(this.previousNodeRealizationResult);
if (_A3.Success)
{
    return _A3;
}

var _A4 = _Ⰳx00ⲻFFRealized._A4.Create(this.previousNodeRealizationResult);
if (_A4.Success)
{
    return _A4;
}

var _A5 = _Ⰳx00ⲻFFRealized._A5.Create(this.previousNodeRealizationResult);
if (_A5.Success)
{
    return _A5;
}

var _A6 = _Ⰳx00ⲻFFRealized._A6.Create(this.previousNodeRealizationResult);
if (_A6.Success)
{
    return _A6;
}

var _A7 = _Ⰳx00ⲻFFRealized._A7.Create(this.previousNodeRealizationResult);
if (_A7.Success)
{
    return _A7;
}

var _A8 = _Ⰳx00ⲻFFRealized._A8.Create(this.previousNodeRealizationResult);
if (_A8.Success)
{
    return _A8;
}

var _A9 = _Ⰳx00ⲻFFRealized._A9.Create(this.previousNodeRealizationResult);
if (_A9.Success)
{
    return _A9;
}

var _AA = _Ⰳx00ⲻFFRealized._AA.Create(this.previousNodeRealizationResult);
if (_AA.Success)
{
    return _AA;
}

var _AB = _Ⰳx00ⲻFFRealized._AB.Create(this.previousNodeRealizationResult);
if (_AB.Success)
{
    return _AB;
}

var _AC = _Ⰳx00ⲻFFRealized._AC.Create(this.previousNodeRealizationResult);
if (_AC.Success)
{
    return _AC;
}

var _AD = _Ⰳx00ⲻFFRealized._AD.Create(this.previousNodeRealizationResult);
if (_AD.Success)
{
    return _AD;
}

var _AE = _Ⰳx00ⲻFFRealized._AE.Create(this.previousNodeRealizationResult);
if (_AE.Success)
{
    return _AE;
}

var _AF = _Ⰳx00ⲻFFRealized._AF.Create(this.previousNodeRealizationResult);
if (_AF.Success)
{
    return _AF;
}

var _B0 = _Ⰳx00ⲻFFRealized._B0.Create(this.previousNodeRealizationResult);
if (_B0.Success)
{
    return _B0;
}

var _B1 = _Ⰳx00ⲻFFRealized._B1.Create(this.previousNodeRealizationResult);
if (_B1.Success)
{
    return _B1;
}

var _B2 = _Ⰳx00ⲻFFRealized._B2.Create(this.previousNodeRealizationResult);
if (_B2.Success)
{
    return _B2;
}

var _B3 = _Ⰳx00ⲻFFRealized._B3.Create(this.previousNodeRealizationResult);
if (_B3.Success)
{
    return _B3;
}

var _B4 = _Ⰳx00ⲻFFRealized._B4.Create(this.previousNodeRealizationResult);
if (_B4.Success)
{
    return _B4;
}

var _B5 = _Ⰳx00ⲻFFRealized._B5.Create(this.previousNodeRealizationResult);
if (_B5.Success)
{
    return _B5;
}

var _B6 = _Ⰳx00ⲻFFRealized._B6.Create(this.previousNodeRealizationResult);
if (_B6.Success)
{
    return _B6;
}

var _B7 = _Ⰳx00ⲻFFRealized._B7.Create(this.previousNodeRealizationResult);
if (_B7.Success)
{
    return _B7;
}

var _B8 = _Ⰳx00ⲻFFRealized._B8.Create(this.previousNodeRealizationResult);
if (_B8.Success)
{
    return _B8;
}

var _B9 = _Ⰳx00ⲻFFRealized._B9.Create(this.previousNodeRealizationResult);
if (_B9.Success)
{
    return _B9;
}

var _BA = _Ⰳx00ⲻFFRealized._BA.Create(this.previousNodeRealizationResult);
if (_BA.Success)
{
    return _BA;
}

var _BB = _Ⰳx00ⲻFFRealized._BB.Create(this.previousNodeRealizationResult);
if (_BB.Success)
{
    return _BB;
}

var _BC = _Ⰳx00ⲻFFRealized._BC.Create(this.previousNodeRealizationResult);
if (_BC.Success)
{
    return _BC;
}

var _BD = _Ⰳx00ⲻFFRealized._BD.Create(this.previousNodeRealizationResult);
if (_BD.Success)
{
    return _BD;
}

var _BE = _Ⰳx00ⲻFFRealized._BE.Create(this.previousNodeRealizationResult);
if (_BE.Success)
{
    return _BE;
}

var _BF = _Ⰳx00ⲻFFRealized._BF.Create(this.previousNodeRealizationResult);
if (_BF.Success)
{
    return _BF;
}

var _C0 = _Ⰳx00ⲻFFRealized._C0.Create(this.previousNodeRealizationResult);
if (_C0.Success)
{
    return _C0;
}

var _C1 = _Ⰳx00ⲻFFRealized._C1.Create(this.previousNodeRealizationResult);
if (_C1.Success)
{
    return _C1;
}

var _C2 = _Ⰳx00ⲻFFRealized._C2.Create(this.previousNodeRealizationResult);
if (_C2.Success)
{
    return _C2;
}

var _C3 = _Ⰳx00ⲻFFRealized._C3.Create(this.previousNodeRealizationResult);
if (_C3.Success)
{
    return _C3;
}

var _C4 = _Ⰳx00ⲻFFRealized._C4.Create(this.previousNodeRealizationResult);
if (_C4.Success)
{
    return _C4;
}

var _C5 = _Ⰳx00ⲻFFRealized._C5.Create(this.previousNodeRealizationResult);
if (_C5.Success)
{
    return _C5;
}

var _C6 = _Ⰳx00ⲻFFRealized._C6.Create(this.previousNodeRealizationResult);
if (_C6.Success)
{
    return _C6;
}

var _C7 = _Ⰳx00ⲻFFRealized._C7.Create(this.previousNodeRealizationResult);
if (_C7.Success)
{
    return _C7;
}

var _C8 = _Ⰳx00ⲻFFRealized._C8.Create(this.previousNodeRealizationResult);
if (_C8.Success)
{
    return _C8;
}

var _C9 = _Ⰳx00ⲻFFRealized._C9.Create(this.previousNodeRealizationResult);
if (_C9.Success)
{
    return _C9;
}

var _CA = _Ⰳx00ⲻFFRealized._CA.Create(this.previousNodeRealizationResult);
if (_CA.Success)
{
    return _CA;
}

var _CB = _Ⰳx00ⲻFFRealized._CB.Create(this.previousNodeRealizationResult);
if (_CB.Success)
{
    return _CB;
}

var _CC = _Ⰳx00ⲻFFRealized._CC.Create(this.previousNodeRealizationResult);
if (_CC.Success)
{
    return _CC;
}

var _CD = _Ⰳx00ⲻFFRealized._CD.Create(this.previousNodeRealizationResult);
if (_CD.Success)
{
    return _CD;
}

var _CE = _Ⰳx00ⲻFFRealized._CE.Create(this.previousNodeRealizationResult);
if (_CE.Success)
{
    return _CE;
}

var _CF = _Ⰳx00ⲻFFRealized._CF.Create(this.previousNodeRealizationResult);
if (_CF.Success)
{
    return _CF;
}

var _D0 = _Ⰳx00ⲻFFRealized._D0.Create(this.previousNodeRealizationResult);
if (_D0.Success)
{
    return _D0;
}

var _D1 = _Ⰳx00ⲻFFRealized._D1.Create(this.previousNodeRealizationResult);
if (_D1.Success)
{
    return _D1;
}

var _D2 = _Ⰳx00ⲻFFRealized._D2.Create(this.previousNodeRealizationResult);
if (_D2.Success)
{
    return _D2;
}

var _D3 = _Ⰳx00ⲻFFRealized._D3.Create(this.previousNodeRealizationResult);
if (_D3.Success)
{
    return _D3;
}

var _D4 = _Ⰳx00ⲻFFRealized._D4.Create(this.previousNodeRealizationResult);
if (_D4.Success)
{
    return _D4;
}

var _D5 = _Ⰳx00ⲻFFRealized._D5.Create(this.previousNodeRealizationResult);
if (_D5.Success)
{
    return _D5;
}

var _D6 = _Ⰳx00ⲻFFRealized._D6.Create(this.previousNodeRealizationResult);
if (_D6.Success)
{
    return _D6;
}

var _D7 = _Ⰳx00ⲻFFRealized._D7.Create(this.previousNodeRealizationResult);
if (_D7.Success)
{
    return _D7;
}

var _D8 = _Ⰳx00ⲻFFRealized._D8.Create(this.previousNodeRealizationResult);
if (_D8.Success)
{
    return _D8;
}

var _D9 = _Ⰳx00ⲻFFRealized._D9.Create(this.previousNodeRealizationResult);
if (_D9.Success)
{
    return _D9;
}

var _DA = _Ⰳx00ⲻFFRealized._DA.Create(this.previousNodeRealizationResult);
if (_DA.Success)
{
    return _DA;
}

var _DB = _Ⰳx00ⲻFFRealized._DB.Create(this.previousNodeRealizationResult);
if (_DB.Success)
{
    return _DB;
}

var _DC = _Ⰳx00ⲻFFRealized._DC.Create(this.previousNodeRealizationResult);
if (_DC.Success)
{
    return _DC;
}

var _DD = _Ⰳx00ⲻFFRealized._DD.Create(this.previousNodeRealizationResult);
if (_DD.Success)
{
    return _DD;
}

var _DE = _Ⰳx00ⲻFFRealized._DE.Create(this.previousNodeRealizationResult);
if (_DE.Success)
{
    return _DE;
}

var _DF = _Ⰳx00ⲻFFRealized._DF.Create(this.previousNodeRealizationResult);
if (_DF.Success)
{
    return _DF;
}

var _E0 = _Ⰳx00ⲻFFRealized._E0.Create(this.previousNodeRealizationResult);
if (_E0.Success)
{
    return _E0;
}

var _E1 = _Ⰳx00ⲻFFRealized._E1.Create(this.previousNodeRealizationResult);
if (_E1.Success)
{
    return _E1;
}

var _E2 = _Ⰳx00ⲻFFRealized._E2.Create(this.previousNodeRealizationResult);
if (_E2.Success)
{
    return _E2;
}

var _E3 = _Ⰳx00ⲻFFRealized._E3.Create(this.previousNodeRealizationResult);
if (_E3.Success)
{
    return _E3;
}

var _E4 = _Ⰳx00ⲻFFRealized._E4.Create(this.previousNodeRealizationResult);
if (_E4.Success)
{
    return _E4;
}

var _E5 = _Ⰳx00ⲻFFRealized._E5.Create(this.previousNodeRealizationResult);
if (_E5.Success)
{
    return _E5;
}

var _E6 = _Ⰳx00ⲻFFRealized._E6.Create(this.previousNodeRealizationResult);
if (_E6.Success)
{
    return _E6;
}

var _E7 = _Ⰳx00ⲻFFRealized._E7.Create(this.previousNodeRealizationResult);
if (_E7.Success)
{
    return _E7;
}

var _E8 = _Ⰳx00ⲻFFRealized._E8.Create(this.previousNodeRealizationResult);
if (_E8.Success)
{
    return _E8;
}

var _E9 = _Ⰳx00ⲻFFRealized._E9.Create(this.previousNodeRealizationResult);
if (_E9.Success)
{
    return _E9;
}

var _EA = _Ⰳx00ⲻFFRealized._EA.Create(this.previousNodeRealizationResult);
if (_EA.Success)
{
    return _EA;
}

var _EB = _Ⰳx00ⲻFFRealized._EB.Create(this.previousNodeRealizationResult);
if (_EB.Success)
{
    return _EB;
}

var _EC = _Ⰳx00ⲻFFRealized._EC.Create(this.previousNodeRealizationResult);
if (_EC.Success)
{
    return _EC;
}

var _ED = _Ⰳx00ⲻFFRealized._ED.Create(this.previousNodeRealizationResult);
if (_ED.Success)
{
    return _ED;
}

var _EE = _Ⰳx00ⲻFFRealized._EE.Create(this.previousNodeRealizationResult);
if (_EE.Success)
{
    return _EE;
}

var _EF = _Ⰳx00ⲻFFRealized._EF.Create(this.previousNodeRealizationResult);
if (_EF.Success)
{
    return _EF;
}

var _F0 = _Ⰳx00ⲻFFRealized._F0.Create(this.previousNodeRealizationResult);
if (_F0.Success)
{
    return _F0;
}

var _F1 = _Ⰳx00ⲻFFRealized._F1.Create(this.previousNodeRealizationResult);
if (_F1.Success)
{
    return _F1;
}

var _F2 = _Ⰳx00ⲻFFRealized._F2.Create(this.previousNodeRealizationResult);
if (_F2.Success)
{
    return _F2;
}

var _F3 = _Ⰳx00ⲻFFRealized._F3.Create(this.previousNodeRealizationResult);
if (_F3.Success)
{
    return _F3;
}

var _F4 = _Ⰳx00ⲻFFRealized._F4.Create(this.previousNodeRealizationResult);
if (_F4.Success)
{
    return _F4;
}

var _F5 = _Ⰳx00ⲻFFRealized._F5.Create(this.previousNodeRealizationResult);
if (_F5.Success)
{
    return _F5;
}

var _F6 = _Ⰳx00ⲻFFRealized._F6.Create(this.previousNodeRealizationResult);
if (_F6.Success)
{
    return _F6;
}

var _F7 = _Ⰳx00ⲻFFRealized._F7.Create(this.previousNodeRealizationResult);
if (_F7.Success)
{
    return _F7;
}

var _F8 = _Ⰳx00ⲻFFRealized._F8.Create(this.previousNodeRealizationResult);
if (_F8.Success)
{
    return _F8;
}

var _F9 = _Ⰳx00ⲻFFRealized._F9.Create(this.previousNodeRealizationResult);
if (_F9.Success)
{
    return _F9;
}

var _FA = _Ⰳx00ⲻFFRealized._FA.Create(this.previousNodeRealizationResult);
if (_FA.Success)
{
    return _FA;
}

var _FB = _Ⰳx00ⲻFFRealized._FB.Create(this.previousNodeRealizationResult);
if (_FB.Success)
{
    return _FB;
}

var _FC = _Ⰳx00ⲻFFRealized._FC.Create(this.previousNodeRealizationResult);
if (_FC.Success)
{
    return _FC;
}

var _FD = _Ⰳx00ⲻFFRealized._FD.Create(this.previousNodeRealizationResult);
if (_FD.Success)
{
    return _FD;
}

var _FE = _Ⰳx00ⲻFFRealized._FE.Create(this.previousNodeRealizationResult);
if (_FE.Success)
{
    return _FE;
}

var _FF = _Ⰳx00ⲻFFRealized._FF.Create(this.previousNodeRealizationResult);
if (_FF.Success)
{
    return _FF;
}
return new RealizationResult<char, _Ⰳx00ⲻFFRealized>(false, default, this.previousNodeRealizationResult.Value.RemainingTokens);
        }
    }
    
}
