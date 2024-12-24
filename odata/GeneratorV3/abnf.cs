namespace GeneratorV3.Abnf
{
    using System.Collections.Generic;
    
    public abstract class ALPHA
    {
        private ALPHA()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(ALPHA node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(ALPHA.percentxFOURONEⲻFIVEA node, TContext context);
            protected internal abstract TResult Accept(ALPHA.percentxSIXONEⲻSEVENA node, TContext context);
        }
        
        public sealed class percentxFOURONEⲻFIVEA : ALPHA
        {
            public percentxFOURONEⲻFIVEA(Inners.percentxFOURONEⲻFIVEA percentxFOURONEⲻFIVEA_1)
            {
                this.percentxFOURONEⲻFIVEA_1 = percentxFOURONEⲻFIVEA_1;
            }
            
            public Inners.percentxFOURONEⲻFIVEA percentxFOURONEⲻFIVEA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class percentxSIXONEⲻSEVENA : ALPHA
        {
            public percentxSIXONEⲻSEVENA(Inners.percentxSIXONEⲻSEVENA percentxSIXONEⲻSEVENA_1)
            {
                this.percentxSIXONEⲻSEVENA_1 = percentxSIXONEⲻSEVENA_1;
            }
            
            public Inners.percentxSIXONEⲻSEVENA percentxSIXONEⲻSEVENA_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class BIT
    {
        private BIT()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(BIT node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(BIT.doublequotex30doublequote node, TContext context);
            protected internal abstract TResult Accept(BIT.doublequotex31doublequote node, TContext context);
        }
        
        public sealed class doublequotex30doublequote : BIT
        {
            public doublequotex30doublequote(Inners.doublequotex30doublequote doublequotex30doublequote_1)
            {
                this.doublequotex30doublequote_1 = doublequotex30doublequote_1;
            }
            
            public Inners.doublequotex30doublequote doublequotex30doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex31doublequote : BIT
        {
            public doublequotex31doublequote(Inners.doublequotex31doublequote doublequotex31doublequote_1)
            {
                this.doublequotex31doublequote_1 = doublequotex31doublequote_1;
            }
            
            public Inners.doublequotex31doublequote doublequotex31doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class CHAR
    {
        public CHAR(Inners.percentxZEROONEⲻSEVENF percentxZEROONEⲻSEVENF_1)
        {
            this.percentxZEROONEⲻSEVENF_1 = percentxZEROONEⲻSEVENF_1;
        }
        
        public Inners.percentxZEROONEⲻSEVENF percentxZEROONEⲻSEVENF_1 { get; }
    }
    
    public sealed class CR
    {
        public CR(Inners.percentxZEROD percentxZEROD_1)
        {
            this.percentxZEROD_1 = percentxZEROD_1;
        }
        
        public Inners.percentxZEROD percentxZEROD_1 { get; }
    }
    
    public sealed class CRLF
    {
        public CRLF(GeneratorV3.Abnf.CR CR_1, GeneratorV3.Abnf.LF LF_1)
        {
            this.CR_1 = CR_1;
            this.LF_1 = LF_1;
        }
        
        public GeneratorV3.Abnf.CR CR_1 { get; }
        public GeneratorV3.Abnf.LF LF_1 { get; }
    }
    
    public abstract class CTL
    {
        private CTL()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(CTL node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(CTL.percentxZEROZEROⲻONEF node, TContext context);
            protected internal abstract TResult Accept(CTL.percentxSEVENF node, TContext context);
        }
        
        public sealed class percentxZEROZEROⲻONEF : CTL
        {
            public percentxZEROZEROⲻONEF(Inners.percentxZEROZEROⲻONEF percentxZEROZEROⲻONEF_1)
            {
                this.percentxZEROZEROⲻONEF_1 = percentxZEROZEROⲻONEF_1;
            }
            
            public Inners.percentxZEROZEROⲻONEF percentxZEROZEROⲻONEF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class percentxSEVENF : CTL
        {
            public percentxSEVENF(Inners.percentxSEVENF percentxSEVENF_1)
            {
                this.percentxSEVENF_1 = percentxSEVENF_1;
            }
            
            public Inners.percentxSEVENF percentxSEVENF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class DIGIT
    {
        public DIGIT(Inners.percentxTHREEZEROⲻTHREENINE percentxTHREEZEROⲻTHREENINE_1)
        {
            this.percentxTHREEZEROⲻTHREENINE_1 = percentxTHREEZEROⲻTHREENINE_1;
        }
        
        public Inners.percentxTHREEZEROⲻTHREENINE percentxTHREEZEROⲻTHREENINE_1 { get; }
    }
    
    public sealed class DQUOTE
    {
        public DQUOTE(Inners.percentxTWOTWO percentxTWOTWO_1)
        {
            this.percentxTWOTWO_1 = percentxTWOTWO_1;
        }
        
        public Inners.percentxTWOTWO percentxTWOTWO_1 { get; }
    }
    
    public abstract class HEXDIG
    {
        private HEXDIG()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(HEXDIG node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(HEXDIG.DIGIT node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex41doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex42doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex43doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex44doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex45doublequote node, TContext context);
            protected internal abstract TResult Accept(HEXDIG.doublequotex46doublequote node, TContext context);
        }
        
        public sealed class DIGIT : HEXDIG
        {
            public DIGIT(GeneratorV3.Abnf.DIGIT DIGIT_1)
            {
                this.DIGIT_1 = DIGIT_1;
            }
            
            public GeneratorV3.Abnf.DIGIT DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex41doublequote : HEXDIG
        {
            public doublequotex41doublequote(Inners.doublequotex41doublequote doublequotex41doublequote_1)
            {
                this.doublequotex41doublequote_1 = doublequotex41doublequote_1;
            }
            
            public Inners.doublequotex41doublequote doublequotex41doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex42doublequote : HEXDIG
        {
            public doublequotex42doublequote(Inners.doublequotex42doublequote doublequotex42doublequote_1)
            {
                this.doublequotex42doublequote_1 = doublequotex42doublequote_1;
            }
            
            public Inners.doublequotex42doublequote doublequotex42doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex43doublequote : HEXDIG
        {
            public doublequotex43doublequote(Inners.doublequotex43doublequote doublequotex43doublequote_1)
            {
                this.doublequotex43doublequote_1 = doublequotex43doublequote_1;
            }
            
            public Inners.doublequotex43doublequote doublequotex43doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex44doublequote : HEXDIG
        {
            public doublequotex44doublequote(Inners.doublequotex44doublequote doublequotex44doublequote_1)
            {
                this.doublequotex44doublequote_1 = doublequotex44doublequote_1;
            }
            
            public Inners.doublequotex44doublequote doublequotex44doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex45doublequote : HEXDIG
        {
            public doublequotex45doublequote(Inners.doublequotex45doublequote doublequotex45doublequote_1)
            {
                this.doublequotex45doublequote_1 = doublequotex45doublequote_1;
            }
            
            public Inners.doublequotex45doublequote doublequotex45doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class doublequotex46doublequote : HEXDIG
        {
            public doublequotex46doublequote(Inners.doublequotex46doublequote doublequotex46doublequote_1)
            {
                this.doublequotex46doublequote_1 = doublequotex46doublequote_1;
            }
            
            public Inners.doublequotex46doublequote doublequotex46doublequote_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class HTAB
    {
        public HTAB(Inners.percentxZERONINE percentxZERONINE_1)
        {
            this.percentxZERONINE_1 = percentxZERONINE_1;
        }
        
        public Inners.percentxZERONINE percentxZERONINE_1 { get; }
    }
    
    public sealed class LF
    {
        public LF(Inners.percentxZEROA percentxZEROA_1)
        {
            this.percentxZEROA_1 = percentxZEROA_1;
        }
        
        public Inners.percentxZEROA percentxZEROA_1 { get; }
    }
    
    public sealed class LWSP
    {
        public LWSP(IEnumerable<Inners.openWSPⳆCRLF_WSPↃ> openWSPⳆCRLF_WSPↃ_1)
        {
            this.openWSPⳆCRLF_WSPↃ_1 = openWSPⳆCRLF_WSPↃ_1;
        }
        
        public IEnumerable<Inners.openWSPⳆCRLF_WSPↃ> openWSPⳆCRLF_WSPↃ_1 { get; }
    }
    
    public sealed class OCTET
    {
        public OCTET(Inners.percentxZEROZEROⲻFF percentxZEROZEROⲻFF_1)
        {
            this.percentxZEROZEROⲻFF_1 = percentxZEROZEROⲻFF_1;
        }
        
        public Inners.percentxZEROZEROⲻFF percentxZEROZEROⲻFF_1 { get; }
    }
    
    public sealed class SP
    {
        public SP(Inners.percentxTWOZERO percentxTWOZERO_1)
        {
            this.percentxTWOZERO_1 = percentxTWOZERO_1;
        }
        
        public Inners.percentxTWOZERO percentxTWOZERO_1 { get; }
    }
    
    public sealed class VCHAR
    {
        public VCHAR(Inners.percentxTWOONEⲻSEVENE percentxTWOONEⲻSEVENE_1)
        {
            this.percentxTWOONEⲻSEVENE_1 = percentxTWOONEⲻSEVENE_1;
        }
        
        public Inners.percentxTWOONEⲻSEVENE percentxTWOONEⲻSEVENE_1 { get; }
    }
    
    public abstract class WSP
    {
        private WSP()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(WSP node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(WSP.SP node, TContext context);
            protected internal abstract TResult Accept(WSP.HTAB node, TContext context);
        }
        
        public sealed class SP : WSP
        {
            public SP(GeneratorV3.Abnf.SP SP_1)
            {
                this.SP_1 = SP_1;
            }
            
            public GeneratorV3.Abnf.SP SP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class HTAB : WSP
        {
            public HTAB(GeneratorV3.Abnf.HTAB HTAB_1)
            {
                this.HTAB_1 = HTAB_1;
            }
            
            public GeneratorV3.Abnf.HTAB HTAB_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class rulelist
    {
        public rulelist(IEnumerable<Inners.openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ> openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ_1)
        {
            this.openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ_1 = openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ_1;
        }
        
        public IEnumerable<Inners.openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ> openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ_1 { get; }
    }
    
    public sealed class rule
    {
        public rule(GeneratorV3.Abnf.rulename rulename_1, GeneratorV3.Abnf.definedⲻas definedⲻas_1, GeneratorV3.Abnf.elements elements_1, GeneratorV3.Abnf.cⲻnl cⲻnl_1)
        {
            this.rulename_1 = rulename_1;
            this.definedⲻas_1 = definedⲻas_1;
            this.elements_1 = elements_1;
            this.cⲻnl_1 = cⲻnl_1;
        }
        
        public GeneratorV3.Abnf.rulename rulename_1 { get; }
        public GeneratorV3.Abnf.definedⲻas definedⲻas_1 { get; }
        public GeneratorV3.Abnf.elements elements_1 { get; }
        public GeneratorV3.Abnf.cⲻnl cⲻnl_1 { get; }
    }
    
    public sealed class rulename
    {
        public rulename(GeneratorV3.Abnf.ALPHA ALPHA_1, IEnumerable<Inners.openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ> openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ_1)
        {
            this.ALPHA_1 = ALPHA_1;
            this.openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ_1 = openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ_1;
        }
        
        public GeneratorV3.Abnf.ALPHA ALPHA_1 { get; }
        public IEnumerable<Inners.openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ> openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ_1 { get; }
    }
    
    public sealed class definedⲻas
    {
        public definedⲻas(IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, Inners.opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2)
        {
            this.cⲻwsp_1 = cⲻwsp_1;
            this.opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ_1 = opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ_1;
            this.cⲻwsp_2 = cⲻwsp_2;
        }
        
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
        public Inners.opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2 { get; }
    }
    
    public sealed class elements
    {
        public elements(GeneratorV3.Abnf.alternation alternation_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1)
        {
            this.alternation_1 = alternation_1;
            this.cⲻwsp_1 = cⲻwsp_1;
        }
        
        public GeneratorV3.Abnf.alternation alternation_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
    }
    
    public abstract class cⲻwsp
    {
        private cⲻwsp()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(cⲻwsp node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(cⲻwsp.WSP node, TContext context);
            protected internal abstract TResult Accept(cⲻwsp.opencⲻnl_WSPↃ node, TContext context);
        }
        
        public sealed class WSP : cⲻwsp
        {
            public WSP(GeneratorV3.Abnf.WSP WSP_1)
            {
                this.WSP_1 = WSP_1;
            }
            
            public GeneratorV3.Abnf.WSP WSP_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class opencⲻnl_WSPↃ : cⲻwsp
        {
            public opencⲻnl_WSPↃ(Inners.opencⲻnl_WSPↃ opencⲻnl_WSPↃ_1)
            {
                this.opencⲻnl_WSPↃ_1 = opencⲻnl_WSPↃ_1;
            }
            
            public Inners.opencⲻnl_WSPↃ opencⲻnl_WSPↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class cⲻnl
    {
        private cⲻnl()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(cⲻnl node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(cⲻnl.comment node, TContext context);
            protected internal abstract TResult Accept(cⲻnl.CRLF node, TContext context);
        }
        
        public sealed class comment : cⲻnl
        {
            public comment(GeneratorV3.Abnf.comment comment_1)
            {
                this.comment_1 = comment_1;
            }
            
            public GeneratorV3.Abnf.comment comment_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class CRLF : cⲻnl
        {
            public CRLF(GeneratorV3.Abnf.CRLF CRLF_1)
            {
                this.CRLF_1 = CRLF_1;
            }
            
            public GeneratorV3.Abnf.CRLF CRLF_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class comment
    {
        public comment(Inners.doublequotex3Bdoublequote doublequotex3Bdoublequote_1, IEnumerable<Inners.openWSPⳆVCHARↃ> openWSPⳆVCHARↃ_1, GeneratorV3.Abnf.CRLF CRLF_1)
        {
            this.doublequotex3Bdoublequote_1 = doublequotex3Bdoublequote_1;
            this.openWSPⳆVCHARↃ_1 = openWSPⳆVCHARↃ_1;
            this.CRLF_1 = CRLF_1;
        }
        
        public Inners.doublequotex3Bdoublequote doublequotex3Bdoublequote_1 { get; }
        public IEnumerable<Inners.openWSPⳆVCHARↃ> openWSPⳆVCHARↃ_1 { get; }
        public GeneratorV3.Abnf.CRLF CRLF_1 { get; }
    }
    
    public sealed class alternation
    {
        public alternation(GeneratorV3.Abnf.concatenation concatenation_1, IEnumerable<Inners.openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ> openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ_1)
        {
            this.concatenation_1 = concatenation_1;
            this.openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ_1 = openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ_1;
        }
        
        public GeneratorV3.Abnf.concatenation concatenation_1 { get; }
        public IEnumerable<Inners.openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ> openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ_1 { get; }
    }
    
    public sealed class concatenation
    {
        public concatenation(GeneratorV3.Abnf.repetition repetition_1, IEnumerable<Inners.openONEasteriskcⲻwsp_repetitionↃ> openONEasteriskcⲻwsp_repetitionↃ_1)
        {
            this.repetition_1 = repetition_1;
            this.openONEasteriskcⲻwsp_repetitionↃ_1 = openONEasteriskcⲻwsp_repetitionↃ_1;
        }
        
        public GeneratorV3.Abnf.repetition repetition_1 { get; }
        public IEnumerable<Inners.openONEasteriskcⲻwsp_repetitionↃ> openONEasteriskcⲻwsp_repetitionↃ_1 { get; }
    }
    
    public sealed class repetition
    {
        public repetition(GeneratorV3.Abnf.repeat? repeat_1, GeneratorV3.Abnf.element element_1)
        {
            this.repeat_1 = repeat_1;
            this.element_1 = element_1;
        }
        
        public GeneratorV3.Abnf.repeat? repeat_1 { get; }
        public GeneratorV3.Abnf.element element_1 { get; }
    }
    
    public abstract class repeat
    {
        private repeat()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(repeat node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(repeat.ONEasteriskDIGIT node, TContext context);
            protected internal abstract TResult Accept(repeat.openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ node, TContext context);
        }
        
        public sealed class ONEasteriskDIGIT : repeat
        {
            public ONEasteriskDIGIT(IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1)
            {
                this.DIGIT_1 = DIGIT_1;
            }
            
            public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ : repeat
        {
            public openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ(Inners.openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ_1)
            {
                this.openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ_1 = openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ_1;
            }
            
            public Inners.openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public abstract class element
    {
        private element()
        {
        }
        
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
        
        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(element node, TContext context)
            {
                return node.Dispatch(this, context);
            }
            
            protected internal abstract TResult Accept(element.rulename node, TContext context);
            protected internal abstract TResult Accept(element.group node, TContext context);
            protected internal abstract TResult Accept(element.option node, TContext context);
            protected internal abstract TResult Accept(element.charⲻval node, TContext context);
            protected internal abstract TResult Accept(element.numⲻval node, TContext context);
            protected internal abstract TResult Accept(element.proseⲻval node, TContext context);
        }
        
        public sealed class rulename : element
        {
            public rulename(GeneratorV3.Abnf.rulename rulename_1)
            {
                this.rulename_1 = rulename_1;
            }
            
            public GeneratorV3.Abnf.rulename rulename_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class group : element
        {
            public group(GeneratorV3.Abnf.group group_1)
            {
                this.group_1 = group_1;
            }
            
            public GeneratorV3.Abnf.group group_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class option : element
        {
            public option(GeneratorV3.Abnf.option option_1)
            {
                this.option_1 = option_1;
            }
            
            public GeneratorV3.Abnf.option option_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class charⲻval : element
        {
            public charⲻval(GeneratorV3.Abnf.charⲻval charⲻval_1)
            {
                this.charⲻval_1 = charⲻval_1;
            }
            
            public GeneratorV3.Abnf.charⲻval charⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class numⲻval : element
        {
            public numⲻval(GeneratorV3.Abnf.numⲻval numⲻval_1)
            {
                this.numⲻval_1 = numⲻval_1;
            }
            
            public GeneratorV3.Abnf.numⲻval numⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
        
        public sealed class proseⲻval : element
        {
            public proseⲻval(GeneratorV3.Abnf.proseⲻval proseⲻval_1)
            {
                this.proseⲻval_1 = proseⲻval_1;
            }
            
            public GeneratorV3.Abnf.proseⲻval proseⲻval_1 { get; }
            
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
    
    public sealed class group
    {
        public group(Inners.doublequotex28doublequote doublequotex28doublequote_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, GeneratorV3.Abnf.alternation alternation_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2, Inners.doublequotex29doublequote doublequotex29doublequote_1)
        {
            this.doublequotex28doublequote_1 = doublequotex28doublequote_1;
            this.cⲻwsp_1 = cⲻwsp_1;
            this.alternation_1 = alternation_1;
            this.cⲻwsp_2 = cⲻwsp_2;
            this.doublequotex29doublequote_1 = doublequotex29doublequote_1;
        }
        
        public Inners.doublequotex28doublequote doublequotex28doublequote_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
        public GeneratorV3.Abnf.alternation alternation_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2 { get; }
        public Inners.doublequotex29doublequote doublequotex29doublequote_1 { get; }
    }
    
    public sealed class option
    {
        public option(Inners.doublequotex5Bdoublequote doublequotex5Bdoublequote_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, GeneratorV3.Abnf.alternation alternation_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2, Inners.doublequotex5Ddoublequote doublequotex5Ddoublequote_1)
        {
            this.doublequotex5Bdoublequote_1 = doublequotex5Bdoublequote_1;
            this.cⲻwsp_1 = cⲻwsp_1;
            this.alternation_1 = alternation_1;
            this.cⲻwsp_2 = cⲻwsp_2;
            this.doublequotex5Ddoublequote_1 = doublequotex5Ddoublequote_1;
        }
        
        public Inners.doublequotex5Bdoublequote doublequotex5Bdoublequote_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
        public GeneratorV3.Abnf.alternation alternation_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2 { get; }
        public Inners.doublequotex5Ddoublequote doublequotex5Ddoublequote_1 { get; }
    }
    
    public sealed class charⲻval
    {
        public charⲻval(GeneratorV3.Abnf.DQUOTE DQUOTE_1, IEnumerable<Inners.openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ> openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ_1, GeneratorV3.Abnf.DQUOTE DQUOTE_2)
        {
            this.DQUOTE_1 = DQUOTE_1;
            this.openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ_1 = openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ_1;
            this.DQUOTE_2 = DQUOTE_2;
        }
        
        public GeneratorV3.Abnf.DQUOTE DQUOTE_1 { get; }
        public IEnumerable<Inners.openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ> openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ_1 { get; }
        public GeneratorV3.Abnf.DQUOTE DQUOTE_2 { get; }
    }
    
    public sealed class numⲻval
    {
        public numⲻval(Inners.doublequotex25doublequote doublequotex25doublequote_1, Inners.openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1)
        {
            this.doublequotex25doublequote_1 = doublequotex25doublequote_1;
            this.openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 = openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1;
        }
        
        public Inners.doublequotex25doublequote doublequotex25doublequote_1 { get; }
        public Inners.openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ_1 { get; }
    }
    
    public sealed class binⲻval
    {
        public binⲻval(Inners.doublequotex62doublequote doublequotex62doublequote_1, IEnumerable<GeneratorV3.Abnf.BIT> BIT_1, Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ_1)
        {
            this.doublequotex62doublequote_1 = doublequotex62doublequote_1;
            this.BIT_1 = BIT_1;
            this.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ_1 = ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ_1;
        }
        
        public Inners.doublequotex62doublequote doublequotex62doublequote_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.BIT> BIT_1 { get; }
        public Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ_1 { get; }
    }
    
    public sealed class decⲻval
    {
        public decⲻval(Inners.doublequotex64doublequote doublequotex64doublequote_1, IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1, Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1)
        {
            this.doublequotex64doublequote_1 = doublequotex64doublequote_1;
            this.DIGIT_1 = DIGIT_1;
            this.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1 = ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1;
        }
        
        public Inners.doublequotex64doublequote doublequotex64doublequote_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1 { get; }
        public Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1 { get; }
    }
    
    public sealed class hexⲻval
    {
        public hexⲻval(Inners.doublequotex78doublequote doublequotex78doublequote_1, IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1, Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1)
        {
            this.doublequotex78doublequote_1 = doublequotex78doublequote_1;
            this.HEXDIG_1 = HEXDIG_1;
            this.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1 = ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1;
        }
        
        public Inners.doublequotex78doublequote doublequotex78doublequote_1 { get; }
        public IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1 { get; }
        public Inners.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ? ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1 { get; }
    }
    
    public sealed class proseⲻval
    {
        public proseⲻval(Inners.doublequotex3Cdoublequote doublequotex3Cdoublequote_1, IEnumerable<Inners.openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ> openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ_1, Inners.doublequotex3Edoublequote doublequotex3Edoublequote_1)
        {
            this.doublequotex3Cdoublequote_1 = doublequotex3Cdoublequote_1;
            this.openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ_1 = openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ_1;
            this.doublequotex3Edoublequote_1 = doublequotex3Edoublequote_1;
        }
        
        public Inners.doublequotex3Cdoublequote doublequotex3Cdoublequote_1 { get; }
        public IEnumerable<Inners.openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ> openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ_1 { get; }
        public Inners.doublequotex3Edoublequote doublequotex3Edoublequote_1 { get; }
    }
    
    public static class Inners
    {
        public sealed class FOUR
        {
            private FOUR()
            {
            }
            
            public static FOUR Instance { get; } = new FOUR();
        }
        
        public sealed class ONE
        {
            private ONE()
            {
            }
            
            public static ONE Instance { get; } = new ONE();
        }
        
        public sealed class TWO
        {
            private TWO()
            {
            }
            
            public static TWO Instance { get; } = new TWO();
        }
        
        public sealed class THREE
        {
            private THREE()
            {
            }
            
            public static THREE Instance { get; } = new THREE();
        }
        
        public sealed class FIVE
        {
            private FIVE()
            {
            }
            
            public static FIVE Instance { get; } = new FIVE();
        }
        
        public sealed class SIX
        {
            private SIX()
            {
            }
            
            public static SIX Instance { get; } = new SIX();
        }
        
        public sealed class SEVEN
        {
            private SEVEN()
            {
            }
            
            public static SEVEN Instance { get; } = new SEVEN();
        }
        
        public sealed class EIGHT
        {
            private EIGHT()
            {
            }
            
            public static EIGHT Instance { get; } = new EIGHT();
        }
        
        public sealed class NINE
        {
            private NINE()
            {
            }
            
            public static NINE Instance { get; } = new NINE();
        }
        
        public sealed class A
        {
            private A()
            {
            }
            
            public static A Instance { get; } = new A();
        }
        
        public sealed class B
        {
            private B()
            {
            }
            
            public static B Instance { get; } = new B();
        }
        
        public sealed class C
        {
            private C()
            {
            }
            
            public static C Instance { get; } = new C();
        }
        
        public sealed class D
        {
            private D()
            {
            }
            
            public static D Instance { get; } = new D();
        }
        
        public sealed class E
        {
            private E()
            {
            }
            
            public static E Instance { get; } = new E();
        }
        
        public sealed class F
        {
            private F()
            {
            }
            
            public static F Instance { get; } = new F();
        }
        
        public sealed class ZERO
        {
            private ZERO()
            {
            }
            
            public static ZERO Instance { get; } = new ZERO();
        }
        
        public abstract class percentxFOURONEⲻFIVEA
        {
            private percentxFOURONEⲻFIVEA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxFOURONEⲻFIVEA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxFOURONEⲻFIVEA.FIVEA node, TContext context);
            }
            
            public sealed class FOURONE : percentxFOURONEⲻFIVEA
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxFOURONEⲻFIVEA
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxFOURONEⲻFIVEA
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxFOURONEⲻFIVEA
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxFOURONEⲻFIVEA
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxFOURONEⲻFIVEA
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxFOURONEⲻFIVEA
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxFOURONEⲻFIVEA
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxFOURONEⲻFIVEA
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxFOURONEⲻFIVEA
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxFOURONEⲻFIVEA
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxFOURONEⲻFIVEA
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxFOURONEⲻFIVEA
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxFOURONEⲻFIVEA
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxFOURONEⲻFIVEA
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxFOURONEⲻFIVEA
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxFOURONEⲻFIVEA
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxFOURONEⲻFIVEA
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxFOURONEⲻFIVEA
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxFOURONEⲻFIVEA
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxFOURONEⲻFIVEA
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxFOURONEⲻFIVEA
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxFOURONEⲻFIVEA
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxFOURONEⲻFIVEA
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxFOURONEⲻFIVEA
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxFOURONEⲻFIVEA
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxSIXONEⲻSEVENA
        {
            private percentxSIXONEⲻSEVENA()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxSIXONEⲻSEVENA node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxSIXONEⲻSEVENA.SEVENA node, TContext context);
            }
            
            public sealed class SIXONE : percentxSIXONEⲻSEVENA
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxSIXONEⲻSEVENA
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxSIXONEⲻSEVENA
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxSIXONEⲻSEVENA
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxSIXONEⲻSEVENA
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxSIXONEⲻSEVENA
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxSIXONEⲻSEVENA
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxSIXONEⲻSEVENA
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxSIXONEⲻSEVENA
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxSIXONEⲻSEVENA
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxSIXONEⲻSEVENA
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxSIXONEⲻSEVENA
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxSIXONEⲻSEVENA
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxSIXONEⲻSEVENA
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxSIXONEⲻSEVENA
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxSIXONEⲻSEVENA
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxSIXONEⲻSEVENA
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxSIXONEⲻSEVENA
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxSIXONEⲻSEVENA
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxSIXONEⲻSEVENA
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxSIXONEⲻSEVENA
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxSIXONEⲻSEVENA
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxSIXONEⲻSEVENA
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxSIXONEⲻSEVENA
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxSIXONEⲻSEVENA
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxSIXONEⲻSEVENA
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class x30
        {
            private x30()
            {
            }
            
            public static x30 Instance { get; } = new x30();
        }
        
        public sealed class doublequotex30doublequote
        {
            public doublequotex30doublequote(Inners.x30 x30_1)
            {
                this.x30_1 = x30_1;
            }
            
            public Inners.x30 x30_1 { get; }
        }
        
        public sealed class x31
        {
            private x31()
            {
            }
            
            public static x31 Instance { get; } = new x31();
        }
        
        public sealed class doublequotex31doublequote
        {
            public doublequotex31doublequote(Inners.x31 x31_1)
            {
                this.x31_1 = x31_1;
            }
            
            public Inners.x31 x31_1 { get; }
        }
        
        public abstract class percentxZEROONEⲻSEVENF
        {
            private percentxZEROONEⲻSEVENF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROONEⲻSEVENF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.ONEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROONEⲻSEVENF.SEVENF node, TContext context);
            }
            
            public sealed class ZEROONE : percentxZEROONEⲻSEVENF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROONEⲻSEVENF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROONEⲻSEVENF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROONEⲻSEVENF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROONEⲻSEVENF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROONEⲻSEVENF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROONEⲻSEVENF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROONEⲻSEVENF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROONEⲻSEVENF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROONEⲻSEVENF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROONEⲻSEVENF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROONEⲻSEVENF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROONEⲻSEVENF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROONEⲻSEVENF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROONEⲻSEVENF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROONEⲻSEVENF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROONEⲻSEVENF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROONEⲻSEVENF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROONEⲻSEVENF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROONEⲻSEVENF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROONEⲻSEVENF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROONEⲻSEVENF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROONEⲻSEVENF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROONEⲻSEVENF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROONEⲻSEVENF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROONEⲻSEVENF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROONEⲻSEVENF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROONEⲻSEVENF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROONEⲻSEVENF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROONEⲻSEVENF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROONEⲻSEVENF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOZERO : percentxZEROONEⲻSEVENF
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxZEROONEⲻSEVENF
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxZEROONEⲻSEVENF
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxZEROONEⲻSEVENF
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxZEROONEⲻSEVENF
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxZEROONEⲻSEVENF
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxZEROONEⲻSEVENF
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxZEROONEⲻSEVENF
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxZEROONEⲻSEVENF
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxZEROONEⲻSEVENF
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxZEROONEⲻSEVENF
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxZEROONEⲻSEVENF
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxZEROONEⲻSEVENF
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxZEROONEⲻSEVENF
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxZEROONEⲻSEVENF
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxZEROONEⲻSEVENF
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxZEROONEⲻSEVENF
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxZEROONEⲻSEVENF
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxZEROONEⲻSEVENF
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxZEROONEⲻSEVENF
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxZEROONEⲻSEVENF
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxZEROONEⲻSEVENF
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxZEROONEⲻSEVENF
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxZEROONEⲻSEVENF
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxZEROONEⲻSEVENF
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxZEROONEⲻSEVENF
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxZEROONEⲻSEVENF
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxZEROONEⲻSEVENF
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxZEROONEⲻSEVENF
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxZEROONEⲻSEVENF
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxZEROONEⲻSEVENF
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxZEROONEⲻSEVENF
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxZEROONEⲻSEVENF
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxZEROONEⲻSEVENF
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxZEROONEⲻSEVENF
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxZEROONEⲻSEVENF
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxZEROONEⲻSEVENF
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxZEROONEⲻSEVENF
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxZEROONEⲻSEVENF
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxZEROONEⲻSEVENF
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxZEROONEⲻSEVENF
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxZEROONEⲻSEVENF
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxZEROONEⲻSEVENF
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxZEROONEⲻSEVENF
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxZEROONEⲻSEVENF
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxZEROONEⲻSEVENF
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxZEROONEⲻSEVENF
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxZEROONEⲻSEVENF
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxZEROONEⲻSEVENF
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxZEROONEⲻSEVENF
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxZEROONEⲻSEVENF
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxZEROONEⲻSEVENF
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxZEROONEⲻSEVENF
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxZEROONEⲻSEVENF
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxZEROONEⲻSEVENF
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxZEROONEⲻSEVENF
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxZEROONEⲻSEVENF
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxZEROONEⲻSEVENF
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxZEROONEⲻSEVENF
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxZEROONEⲻSEVENF
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxZEROONEⲻSEVENF
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxZEROONEⲻSEVENF
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxZEROONEⲻSEVENF
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxZEROONEⲻSEVENF
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxZEROONEⲻSEVENF
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxZEROONEⲻSEVENF
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxZEROONEⲻSEVENF
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxZEROONEⲻSEVENF
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxZEROONEⲻSEVENF
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxZEROONEⲻSEVENF
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxZEROONEⲻSEVENF
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxZEROONEⲻSEVENF
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxZEROONEⲻSEVENF
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxZEROONEⲻSEVENF
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxZEROONEⲻSEVENF
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxZEROONEⲻSEVENF
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxZEROONEⲻSEVENF
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxZEROONEⲻSEVENF
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxZEROONEⲻSEVENF
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxZEROONEⲻSEVENF
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxZEROONEⲻSEVENF
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxZEROONEⲻSEVENF
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxZEROONEⲻSEVENF
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxZEROONEⲻSEVENF
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxZEROONEⲻSEVENF
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxZEROONEⲻSEVENF
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxZEROONEⲻSEVENF
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxZEROONEⲻSEVENF
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxZEROONEⲻSEVENF
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxZEROONEⲻSEVENF
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxZEROONEⲻSEVENF
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxZEROONEⲻSEVENF
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxZEROONEⲻSEVENF
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxZEROONEⲻSEVENF
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxZEROONEⲻSEVENF
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENF : percentxZEROONEⲻSEVENF
            {
                public SEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxZEROD
        {
            public percentxZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
            {
                this.ZERO_1 = ZERO_1;
                this.D_1 = D_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.D D_1 { get; }
        }
        
        public abstract class percentxZEROZEROⲻONEF
        {
            private percentxZEROZEROⲻONEF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROZEROⲻONEF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻONEF.ONEF node, TContext context);
            }
            
            public sealed class ZEROZERO : percentxZEROZEROⲻONEF
            {
                public ZEROZERO(Inners.ZERO ZERO_1, Inners.ZERO ZERO_2)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ZERO_2 = ZERO_2;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ZERO ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROONE : percentxZEROZEROⲻONEF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROZEROⲻONEF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROZEROⲻONEF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROZEROⲻONEF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROZEROⲻONEF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROZEROⲻONEF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROZEROⲻONEF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROZEROⲻONEF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROZEROⲻONEF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROZEROⲻONEF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROZEROⲻONEF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROZEROⲻONEF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROZEROⲻONEF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROZEROⲻONEF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROZEROⲻONEF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROZEROⲻONEF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROZEROⲻONEF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROZEROⲻONEF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROZEROⲻONEF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROZEROⲻONEF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROZEROⲻONEF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROZEROⲻONEF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROZEROⲻONEF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROZEROⲻONEF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROZEROⲻONEF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROZEROⲻONEF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROZEROⲻONEF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROZEROⲻONEF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROZEROⲻONEF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROZEROⲻONEF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROZEROⲻONEF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxSEVENF
        {
            public percentxSEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
            {
                this.SEVEN_1 = SEVEN_1;
                this.F_1 = F_1;
            }
            
            public Inners.SEVEN SEVEN_1 { get; }
            public Inners.F F_1 { get; }
        }
        
        public abstract class percentxTHREEZEROⲻTHREENINE
        {
            private percentxTHREEZEROⲻTHREENINE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTHREEZEROⲻTHREENINE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEZEROⲻTHREENINE.THREENINE node, TContext context);
            }
            
            public sealed class THREEZERO : percentxTHREEZEROⲻTHREENINE
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTHREEZEROⲻTHREENINE
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTHREEZEROⲻTHREENINE
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTHREEZEROⲻTHREENINE
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTHREEZEROⲻTHREENINE
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTHREEZEROⲻTHREENINE
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTHREEZEROⲻTHREENINE
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTHREEZEROⲻTHREENINE
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTHREEZEROⲻTHREENINE
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTHREEZEROⲻTHREENINE
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxTWOTWO
        {
            public percentxTWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
            {
                this.TWO_1 = TWO_1;
                this.TWO_2 = TWO_2;
            }
            
            public Inners.TWO TWO_1 { get; }
            public Inners.TWO TWO_2 { get; }
        }
        
        public sealed class x41
        {
            private x41()
            {
            }
            
            public static x41 Instance { get; } = new x41();
        }
        
        public sealed class doublequotex41doublequote
        {
            public doublequotex41doublequote(Inners.x41 x41_1)
            {
                this.x41_1 = x41_1;
            }
            
            public Inners.x41 x41_1 { get; }
        }
        
        public sealed class x42
        {
            private x42()
            {
            }
            
            public static x42 Instance { get; } = new x42();
        }
        
        public sealed class doublequotex42doublequote
        {
            public doublequotex42doublequote(Inners.x42 x42_1)
            {
                this.x42_1 = x42_1;
            }
            
            public Inners.x42 x42_1 { get; }
        }
        
        public sealed class x43
        {
            private x43()
            {
            }
            
            public static x43 Instance { get; } = new x43();
        }
        
        public sealed class doublequotex43doublequote
        {
            public doublequotex43doublequote(Inners.x43 x43_1)
            {
                this.x43_1 = x43_1;
            }
            
            public Inners.x43 x43_1 { get; }
        }
        
        public sealed class x44
        {
            private x44()
            {
            }
            
            public static x44 Instance { get; } = new x44();
        }
        
        public sealed class doublequotex44doublequote
        {
            public doublequotex44doublequote(Inners.x44 x44_1)
            {
                this.x44_1 = x44_1;
            }
            
            public Inners.x44 x44_1 { get; }
        }
        
        public sealed class x45
        {
            private x45()
            {
            }
            
            public static x45 Instance { get; } = new x45();
        }
        
        public sealed class doublequotex45doublequote
        {
            public doublequotex45doublequote(Inners.x45 x45_1)
            {
                this.x45_1 = x45_1;
            }
            
            public Inners.x45 x45_1 { get; }
        }
        
        public sealed class x46
        {
            private x46()
            {
            }
            
            public static x46 Instance { get; } = new x46();
        }
        
        public sealed class doublequotex46doublequote
        {
            public doublequotex46doublequote(Inners.x46 x46_1)
            {
                this.x46_1 = x46_1;
            }
            
            public Inners.x46 x46_1 { get; }
        }
        
        public sealed class percentxZERONINE
        {
            public percentxZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
            {
                this.ZERO_1 = ZERO_1;
                this.NINE_1 = NINE_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.NINE NINE_1 { get; }
        }
        
        public sealed class percentxZEROA
        {
            public percentxZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
            {
                this.ZERO_1 = ZERO_1;
                this.A_1 = A_1;
            }
            
            public Inners.ZERO ZERO_1 { get; }
            public Inners.A A_1 { get; }
        }
        
        public abstract class WSPⳆCRLF_WSP
        {
            private WSPⳆCRLF_WSP()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(WSPⳆCRLF_WSP node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(WSPⳆCRLF_WSP.WSP node, TContext context);
                protected internal abstract TResult Accept(WSPⳆCRLF_WSP.CRLF_WSP node, TContext context);
            }
            
            public sealed class WSP : WSPⳆCRLF_WSP
            {
                public WSP(GeneratorV3.Abnf.WSP WSP_1)
                {
                    this.WSP_1 = WSP_1;
                }
                
                public GeneratorV3.Abnf.WSP WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CRLF_WSP : WSPⳆCRLF_WSP
            {
                public CRLF_WSP(GeneratorV3.Abnf.CRLF CRLF_1, GeneratorV3.Abnf.WSP WSP_1)
                {
                    this.CRLF_1 = CRLF_1;
                    this.WSP_1 = WSP_1;
                }
                
                public GeneratorV3.Abnf.CRLF CRLF_1 { get; }
                public GeneratorV3.Abnf.WSP WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openWSPⳆCRLF_WSPↃ
        {
            public openWSPⳆCRLF_WSPↃ(Inners.WSPⳆCRLF_WSP WSPⳆCRLF_WSP_1)
            {
                this.WSPⳆCRLF_WSP_1 = WSPⳆCRLF_WSP_1;
            }
            
            public Inners.WSPⳆCRLF_WSP WSPⳆCRLF_WSP_1 { get; }
        }
        
        public abstract class percentxZEROZEROⲻFF
        {
            private percentxZEROZEROⲻFF()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxZEROZEROⲻFF node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZERONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ZEROF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ONEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.SEVENF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EIGHTF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.NINEF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ATWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ATHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ASIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ASEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ANINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.AF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.BF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.CF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.DF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ETWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ESIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ENINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.ED node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.EF node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FZERO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FONE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FTWO node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FSIX node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FNINE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FA node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FB node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FC node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FD node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FE node, TContext context);
                protected internal abstract TResult Accept(percentxZEROZEROⲻFF.FF node, TContext context);
            }
            
            public sealed class ZEROZERO : percentxZEROZEROⲻFF
            {
                public ZEROZERO(Inners.ZERO ZERO_1, Inners.ZERO ZERO_2)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ZERO_2 = ZERO_2;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ZERO ZERO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROONE : percentxZEROZEROⲻFF
            {
                public ZEROONE(Inners.ZERO ZERO_1, Inners.ONE ONE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTWO : percentxZEROZEROⲻFF
            {
                public ZEROTWO(Inners.ZERO ZERO_1, Inners.TWO TWO_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROTHREE : percentxZEROZEROⲻFF
            {
                public ZEROTHREE(Inners.ZERO ZERO_1, Inners.THREE THREE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFOUR : percentxZEROZEROⲻFF
            {
                public ZEROFOUR(Inners.ZERO ZERO_1, Inners.FOUR FOUR_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROFIVE : percentxZEROZEROⲻFF
            {
                public ZEROFIVE(Inners.ZERO ZERO_1, Inners.FIVE FIVE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSIX : percentxZEROZEROⲻFF
            {
                public ZEROSIX(Inners.ZERO ZERO_1, Inners.SIX SIX_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROSEVEN : percentxZEROZEROⲻFF
            {
                public ZEROSEVEN(Inners.ZERO ZERO_1, Inners.SEVEN SEVEN_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROEIGHT : percentxZEROZEROⲻFF
            {
                public ZEROEIGHT(Inners.ZERO ZERO_1, Inners.EIGHT EIGHT_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZERONINE : percentxZEROZEROⲻFF
            {
                public ZERONINE(Inners.ZERO ZERO_1, Inners.NINE NINE_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROA : percentxZEROZEROⲻFF
            {
                public ZEROA(Inners.ZERO ZERO_1, Inners.A A_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROB : percentxZEROZEROⲻFF
            {
                public ZEROB(Inners.ZERO ZERO_1, Inners.B B_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROC : percentxZEROZEROⲻFF
            {
                public ZEROC(Inners.ZERO ZERO_1, Inners.C C_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROD : percentxZEROZEROⲻFF
            {
                public ZEROD(Inners.ZERO ZERO_1, Inners.D D_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROE : percentxZEROZEROⲻFF
            {
                public ZEROE(Inners.ZERO ZERO_1, Inners.E E_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ZEROF : percentxZEROZEROⲻFF
            {
                public ZEROF(Inners.ZERO ZERO_1, Inners.F F_1)
                {
                    this.ZERO_1 = ZERO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ZERO ZERO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEZERO : percentxZEROZEROⲻFF
            {
                public ONEZERO(Inners.ONE ONE_1, Inners.ZERO ZERO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEONE : percentxZEROZEROⲻFF
            {
                public ONEONE(Inners.ONE ONE_1, Inners.ONE ONE_2)
                {
                    this.ONE_1 = ONE_1;
                    this.ONE_2 = ONE_2;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.ONE ONE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETWO : percentxZEROZEROⲻFF
            {
                public ONETWO(Inners.ONE ONE_1, Inners.TWO TWO_1)
                {
                    this.ONE_1 = ONE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONETHREE : percentxZEROZEROⲻFF
            {
                public ONETHREE(Inners.ONE ONE_1, Inners.THREE THREE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFOUR : percentxZEROZEROⲻFF
            {
                public ONEFOUR(Inners.ONE ONE_1, Inners.FOUR FOUR_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEFIVE : percentxZEROZEROⲻFF
            {
                public ONEFIVE(Inners.ONE ONE_1, Inners.FIVE FIVE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESIX : percentxZEROZEROⲻFF
            {
                public ONESIX(Inners.ONE ONE_1, Inners.SIX SIX_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONESEVEN : percentxZEROZEROⲻFF
            {
                public ONESEVEN(Inners.ONE ONE_1, Inners.SEVEN SEVEN_1)
                {
                    this.ONE_1 = ONE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEEIGHT : percentxZEROZEROⲻFF
            {
                public ONEEIGHT(Inners.ONE ONE_1, Inners.EIGHT EIGHT_1)
                {
                    this.ONE_1 = ONE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONENINE : percentxZEROZEROⲻFF
            {
                public ONENINE(Inners.ONE ONE_1, Inners.NINE NINE_1)
                {
                    this.ONE_1 = ONE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEA : percentxZEROZEROⲻFF
            {
                public ONEA(Inners.ONE ONE_1, Inners.A A_1)
                {
                    this.ONE_1 = ONE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEB : percentxZEROZEROⲻFF
            {
                public ONEB(Inners.ONE ONE_1, Inners.B B_1)
                {
                    this.ONE_1 = ONE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEC : percentxZEROZEROⲻFF
            {
                public ONEC(Inners.ONE ONE_1, Inners.C C_1)
                {
                    this.ONE_1 = ONE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONED : percentxZEROZEROⲻFF
            {
                public ONED(Inners.ONE ONE_1, Inners.D D_1)
                {
                    this.ONE_1 = ONE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEE : percentxZEROZEROⲻFF
            {
                public ONEE(Inners.ONE ONE_1, Inners.E E_1)
                {
                    this.ONE_1 = ONE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ONEF : percentxZEROZEROⲻFF
            {
                public ONEF(Inners.ONE ONE_1, Inners.F F_1)
                {
                    this.ONE_1 = ONE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.ONE ONE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOZERO : percentxZEROZEROⲻFF
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxZEROZEROⲻFF
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxZEROZEROⲻFF
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxZEROZEROⲻFF
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxZEROZEROⲻFF
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxZEROZEROⲻFF
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxZEROZEROⲻFF
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxZEROZEROⲻFF
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxZEROZEROⲻFF
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxZEROZEROⲻFF
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxZEROZEROⲻFF
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxZEROZEROⲻFF
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxZEROZEROⲻFF
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxZEROZEROⲻFF
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxZEROZEROⲻFF
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxZEROZEROⲻFF
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxZEROZEROⲻFF
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxZEROZEROⲻFF
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxZEROZEROⲻFF
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxZEROZEROⲻFF
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxZEROZEROⲻFF
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxZEROZEROⲻFF
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxZEROZEROⲻFF
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxZEROZEROⲻFF
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxZEROZEROⲻFF
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxZEROZEROⲻFF
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxZEROZEROⲻFF
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxZEROZEROⲻFF
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxZEROZEROⲻFF
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxZEROZEROⲻFF
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxZEROZEROⲻFF
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxZEROZEROⲻFF
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxZEROZEROⲻFF
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxZEROZEROⲻFF
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxZEROZEROⲻFF
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxZEROZEROⲻFF
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxZEROZEROⲻFF
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxZEROZEROⲻFF
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxZEROZEROⲻFF
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxZEROZEROⲻFF
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxZEROZEROⲻFF
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxZEROZEROⲻFF
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxZEROZEROⲻFF
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxZEROZEROⲻFF
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxZEROZEROⲻFF
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxZEROZEROⲻFF
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxZEROZEROⲻFF
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxZEROZEROⲻFF
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxZEROZEROⲻFF
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxZEROZEROⲻFF
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxZEROZEROⲻFF
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxZEROZEROⲻFF
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxZEROZEROⲻFF
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxZEROZEROⲻFF
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxZEROZEROⲻFF
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxZEROZEROⲻFF
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxZEROZEROⲻFF
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxZEROZEROⲻFF
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxZEROZEROⲻFF
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxZEROZEROⲻFF
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxZEROZEROⲻFF
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxZEROZEROⲻFF
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxZEROZEROⲻFF
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxZEROZEROⲻFF
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxZEROZEROⲻFF
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxZEROZEROⲻFF
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxZEROZEROⲻFF
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxZEROZEROⲻFF
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxZEROZEROⲻFF
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxZEROZEROⲻFF
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxZEROZEROⲻFF
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxZEROZEROⲻFF
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxZEROZEROⲻFF
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxZEROZEROⲻFF
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxZEROZEROⲻFF
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxZEROZEROⲻFF
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxZEROZEROⲻFF
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxZEROZEROⲻFF
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxZEROZEROⲻFF
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxZEROZEROⲻFF
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxZEROZEROⲻFF
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxZEROZEROⲻFF
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxZEROZEROⲻFF
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxZEROZEROⲻFF
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxZEROZEROⲻFF
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxZEROZEROⲻFF
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxZEROZEROⲻFF
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxZEROZEROⲻFF
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxZEROZEROⲻFF
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxZEROZEROⲻFF
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxZEROZEROⲻFF
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxZEROZEROⲻFF
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxZEROZEROⲻFF
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxZEROZEROⲻFF
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxZEROZEROⲻFF
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENF : percentxZEROZEROⲻFF
            {
                public SEVENF(Inners.SEVEN SEVEN_1, Inners.F F_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTZERO : percentxZEROZEROⲻFF
            {
                public EIGHTZERO(Inners.EIGHT EIGHT_1, Inners.ZERO ZERO_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTONE : percentxZEROZEROⲻFF
            {
                public EIGHTONE(Inners.EIGHT EIGHT_1, Inners.ONE ONE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTTWO : percentxZEROZEROⲻFF
            {
                public EIGHTTWO(Inners.EIGHT EIGHT_1, Inners.TWO TWO_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTTHREE : percentxZEROZEROⲻFF
            {
                public EIGHTTHREE(Inners.EIGHT EIGHT_1, Inners.THREE THREE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTFOUR : percentxZEROZEROⲻFF
            {
                public EIGHTFOUR(Inners.EIGHT EIGHT_1, Inners.FOUR FOUR_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTFIVE : percentxZEROZEROⲻFF
            {
                public EIGHTFIVE(Inners.EIGHT EIGHT_1, Inners.FIVE FIVE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTSIX : percentxZEROZEROⲻFF
            {
                public EIGHTSIX(Inners.EIGHT EIGHT_1, Inners.SIX SIX_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTSEVEN : percentxZEROZEROⲻFF
            {
                public EIGHTSEVEN(Inners.EIGHT EIGHT_1, Inners.SEVEN SEVEN_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTEIGHT : percentxZEROZEROⲻFF
            {
                public EIGHTEIGHT(Inners.EIGHT EIGHT_1, Inners.EIGHT EIGHT_2)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.EIGHT_2 = EIGHT_2;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.EIGHT EIGHT_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTNINE : percentxZEROZEROⲻFF
            {
                public EIGHTNINE(Inners.EIGHT EIGHT_1, Inners.NINE NINE_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTA : percentxZEROZEROⲻFF
            {
                public EIGHTA(Inners.EIGHT EIGHT_1, Inners.A A_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.A_1 = A_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTB : percentxZEROZEROⲻFF
            {
                public EIGHTB(Inners.EIGHT EIGHT_1, Inners.B B_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.B_1 = B_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTC : percentxZEROZEROⲻFF
            {
                public EIGHTC(Inners.EIGHT EIGHT_1, Inners.C C_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.C_1 = C_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTD : percentxZEROZEROⲻFF
            {
                public EIGHTD(Inners.EIGHT EIGHT_1, Inners.D D_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.D_1 = D_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTE : percentxZEROZEROⲻFF
            {
                public EIGHTE(Inners.EIGHT EIGHT_1, Inners.E E_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.E_1 = E_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EIGHTF : percentxZEROZEROⲻFF
            {
                public EIGHTF(Inners.EIGHT EIGHT_1, Inners.F F_1)
                {
                    this.EIGHT_1 = EIGHT_1;
                    this.F_1 = F_1;
                }
                
                public Inners.EIGHT EIGHT_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEZERO : percentxZEROZEROⲻFF
            {
                public NINEZERO(Inners.NINE NINE_1, Inners.ZERO ZERO_1)
                {
                    this.NINE_1 = NINE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEONE : percentxZEROZEROⲻFF
            {
                public NINEONE(Inners.NINE NINE_1, Inners.ONE ONE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINETWO : percentxZEROZEROⲻFF
            {
                public NINETWO(Inners.NINE NINE_1, Inners.TWO TWO_1)
                {
                    this.NINE_1 = NINE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINETHREE : percentxZEROZEROⲻFF
            {
                public NINETHREE(Inners.NINE NINE_1, Inners.THREE THREE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEFOUR : percentxZEROZEROⲻFF
            {
                public NINEFOUR(Inners.NINE NINE_1, Inners.FOUR FOUR_1)
                {
                    this.NINE_1 = NINE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEFIVE : percentxZEROZEROⲻFF
            {
                public NINEFIVE(Inners.NINE NINE_1, Inners.FIVE FIVE_1)
                {
                    this.NINE_1 = NINE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINESIX : percentxZEROZEROⲻFF
            {
                public NINESIX(Inners.NINE NINE_1, Inners.SIX SIX_1)
                {
                    this.NINE_1 = NINE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINESEVEN : percentxZEROZEROⲻFF
            {
                public NINESEVEN(Inners.NINE NINE_1, Inners.SEVEN SEVEN_1)
                {
                    this.NINE_1 = NINE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEEIGHT : percentxZEROZEROⲻFF
            {
                public NINEEIGHT(Inners.NINE NINE_1, Inners.EIGHT EIGHT_1)
                {
                    this.NINE_1 = NINE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINENINE : percentxZEROZEROⲻFF
            {
                public NINENINE(Inners.NINE NINE_1, Inners.NINE NINE_2)
                {
                    this.NINE_1 = NINE_1;
                    this.NINE_2 = NINE_2;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.NINE NINE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEA : percentxZEROZEROⲻFF
            {
                public NINEA(Inners.NINE NINE_1, Inners.A A_1)
                {
                    this.NINE_1 = NINE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEB : percentxZEROZEROⲻFF
            {
                public NINEB(Inners.NINE NINE_1, Inners.B B_1)
                {
                    this.NINE_1 = NINE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEC : percentxZEROZEROⲻFF
            {
                public NINEC(Inners.NINE NINE_1, Inners.C C_1)
                {
                    this.NINE_1 = NINE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINED : percentxZEROZEROⲻFF
            {
                public NINED(Inners.NINE NINE_1, Inners.D D_1)
                {
                    this.NINE_1 = NINE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEE : percentxZEROZEROⲻFF
            {
                public NINEE(Inners.NINE NINE_1, Inners.E E_1)
                {
                    this.NINE_1 = NINE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class NINEF : percentxZEROZEROⲻFF
            {
                public NINEF(Inners.NINE NINE_1, Inners.F F_1)
                {
                    this.NINE_1 = NINE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.NINE NINE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AZERO : percentxZEROZEROⲻFF
            {
                public AZERO(Inners.A A_1, Inners.ZERO ZERO_1)
                {
                    this.A_1 = A_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AONE : percentxZEROZEROⲻFF
            {
                public AONE(Inners.A A_1, Inners.ONE ONE_1)
                {
                    this.A_1 = A_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ATWO : percentxZEROZEROⲻFF
            {
                public ATWO(Inners.A A_1, Inners.TWO TWO_1)
                {
                    this.A_1 = A_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ATHREE : percentxZEROZEROⲻFF
            {
                public ATHREE(Inners.A A_1, Inners.THREE THREE_1)
                {
                    this.A_1 = A_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AFOUR : percentxZEROZEROⲻFF
            {
                public AFOUR(Inners.A A_1, Inners.FOUR FOUR_1)
                {
                    this.A_1 = A_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AFIVE : percentxZEROZEROⲻFF
            {
                public AFIVE(Inners.A A_1, Inners.FIVE FIVE_1)
                {
                    this.A_1 = A_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ASIX : percentxZEROZEROⲻFF
            {
                public ASIX(Inners.A A_1, Inners.SIX SIX_1)
                {
                    this.A_1 = A_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ASEVEN : percentxZEROZEROⲻFF
            {
                public ASEVEN(Inners.A A_1, Inners.SEVEN SEVEN_1)
                {
                    this.A_1 = A_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AEIGHT : percentxZEROZEROⲻFF
            {
                public AEIGHT(Inners.A A_1, Inners.EIGHT EIGHT_1)
                {
                    this.A_1 = A_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ANINE : percentxZEROZEROⲻFF
            {
                public ANINE(Inners.A A_1, Inners.NINE NINE_1)
                {
                    this.A_1 = A_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AA : percentxZEROZEROⲻFF
            {
                public AA(Inners.A A_1, Inners.A A_2)
                {
                    this.A_1 = A_1;
                    this.A_2 = A_2;
                }
                
                public Inners.A A_1 { get; }
                public Inners.A A_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AB : percentxZEROZEROⲻFF
            {
                public AB(Inners.A A_1, Inners.B B_1)
                {
                    this.A_1 = A_1;
                    this.B_1 = B_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AC : percentxZEROZEROⲻFF
            {
                public AC(Inners.A A_1, Inners.C C_1)
                {
                    this.A_1 = A_1;
                    this.C_1 = C_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AD : percentxZEROZEROⲻFF
            {
                public AD(Inners.A A_1, Inners.D D_1)
                {
                    this.A_1 = A_1;
                    this.D_1 = D_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AE : percentxZEROZEROⲻFF
            {
                public AE(Inners.A A_1, Inners.E E_1)
                {
                    this.A_1 = A_1;
                    this.E_1 = E_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class AF : percentxZEROZEROⲻFF
            {
                public AF(Inners.A A_1, Inners.F F_1)
                {
                    this.A_1 = A_1;
                    this.F_1 = F_1;
                }
                
                public Inners.A A_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BZERO : percentxZEROZEROⲻFF
            {
                public BZERO(Inners.B B_1, Inners.ZERO ZERO_1)
                {
                    this.B_1 = B_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BONE : percentxZEROZEROⲻFF
            {
                public BONE(Inners.B B_1, Inners.ONE ONE_1)
                {
                    this.B_1 = B_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BTWO : percentxZEROZEROⲻFF
            {
                public BTWO(Inners.B B_1, Inners.TWO TWO_1)
                {
                    this.B_1 = B_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BTHREE : percentxZEROZEROⲻFF
            {
                public BTHREE(Inners.B B_1, Inners.THREE THREE_1)
                {
                    this.B_1 = B_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BFOUR : percentxZEROZEROⲻFF
            {
                public BFOUR(Inners.B B_1, Inners.FOUR FOUR_1)
                {
                    this.B_1 = B_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BFIVE : percentxZEROZEROⲻFF
            {
                public BFIVE(Inners.B B_1, Inners.FIVE FIVE_1)
                {
                    this.B_1 = B_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BSIX : percentxZEROZEROⲻFF
            {
                public BSIX(Inners.B B_1, Inners.SIX SIX_1)
                {
                    this.B_1 = B_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BSEVEN : percentxZEROZEROⲻFF
            {
                public BSEVEN(Inners.B B_1, Inners.SEVEN SEVEN_1)
                {
                    this.B_1 = B_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BEIGHT : percentxZEROZEROⲻFF
            {
                public BEIGHT(Inners.B B_1, Inners.EIGHT EIGHT_1)
                {
                    this.B_1 = B_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BNINE : percentxZEROZEROⲻFF
            {
                public BNINE(Inners.B B_1, Inners.NINE NINE_1)
                {
                    this.B_1 = B_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BA : percentxZEROZEROⲻFF
            {
                public BA(Inners.B B_1, Inners.A A_1)
                {
                    this.B_1 = B_1;
                    this.A_1 = A_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BB : percentxZEROZEROⲻFF
            {
                public BB(Inners.B B_1, Inners.B B_2)
                {
                    this.B_1 = B_1;
                    this.B_2 = B_2;
                }
                
                public Inners.B B_1 { get; }
                public Inners.B B_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BC : percentxZEROZEROⲻFF
            {
                public BC(Inners.B B_1, Inners.C C_1)
                {
                    this.B_1 = B_1;
                    this.C_1 = C_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BD : percentxZEROZEROⲻFF
            {
                public BD(Inners.B B_1, Inners.D D_1)
                {
                    this.B_1 = B_1;
                    this.D_1 = D_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BE : percentxZEROZEROⲻFF
            {
                public BE(Inners.B B_1, Inners.E E_1)
                {
                    this.B_1 = B_1;
                    this.E_1 = E_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class BF : percentxZEROZEROⲻFF
            {
                public BF(Inners.B B_1, Inners.F F_1)
                {
                    this.B_1 = B_1;
                    this.F_1 = F_1;
                }
                
                public Inners.B B_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CZERO : percentxZEROZEROⲻFF
            {
                public CZERO(Inners.C C_1, Inners.ZERO ZERO_1)
                {
                    this.C_1 = C_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CONE : percentxZEROZEROⲻFF
            {
                public CONE(Inners.C C_1, Inners.ONE ONE_1)
                {
                    this.C_1 = C_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CTWO : percentxZEROZEROⲻFF
            {
                public CTWO(Inners.C C_1, Inners.TWO TWO_1)
                {
                    this.C_1 = C_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CTHREE : percentxZEROZEROⲻFF
            {
                public CTHREE(Inners.C C_1, Inners.THREE THREE_1)
                {
                    this.C_1 = C_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CFOUR : percentxZEROZEROⲻFF
            {
                public CFOUR(Inners.C C_1, Inners.FOUR FOUR_1)
                {
                    this.C_1 = C_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CFIVE : percentxZEROZEROⲻFF
            {
                public CFIVE(Inners.C C_1, Inners.FIVE FIVE_1)
                {
                    this.C_1 = C_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CSIX : percentxZEROZEROⲻFF
            {
                public CSIX(Inners.C C_1, Inners.SIX SIX_1)
                {
                    this.C_1 = C_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CSEVEN : percentxZEROZEROⲻFF
            {
                public CSEVEN(Inners.C C_1, Inners.SEVEN SEVEN_1)
                {
                    this.C_1 = C_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CEIGHT : percentxZEROZEROⲻFF
            {
                public CEIGHT(Inners.C C_1, Inners.EIGHT EIGHT_1)
                {
                    this.C_1 = C_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CNINE : percentxZEROZEROⲻFF
            {
                public CNINE(Inners.C C_1, Inners.NINE NINE_1)
                {
                    this.C_1 = C_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CA : percentxZEROZEROⲻFF
            {
                public CA(Inners.C C_1, Inners.A A_1)
                {
                    this.C_1 = C_1;
                    this.A_1 = A_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CB : percentxZEROZEROⲻFF
            {
                public CB(Inners.C C_1, Inners.B B_1)
                {
                    this.C_1 = C_1;
                    this.B_1 = B_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CC : percentxZEROZEROⲻFF
            {
                public CC(Inners.C C_1, Inners.C C_2)
                {
                    this.C_1 = C_1;
                    this.C_2 = C_2;
                }
                
                public Inners.C C_1 { get; }
                public Inners.C C_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CD : percentxZEROZEROⲻFF
            {
                public CD(Inners.C C_1, Inners.D D_1)
                {
                    this.C_1 = C_1;
                    this.D_1 = D_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CE : percentxZEROZEROⲻFF
            {
                public CE(Inners.C C_1, Inners.E E_1)
                {
                    this.C_1 = C_1;
                    this.E_1 = E_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class CF : percentxZEROZEROⲻFF
            {
                public CF(Inners.C C_1, Inners.F F_1)
                {
                    this.C_1 = C_1;
                    this.F_1 = F_1;
                }
                
                public Inners.C C_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DZERO : percentxZEROZEROⲻFF
            {
                public DZERO(Inners.D D_1, Inners.ZERO ZERO_1)
                {
                    this.D_1 = D_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DONE : percentxZEROZEROⲻFF
            {
                public DONE(Inners.D D_1, Inners.ONE ONE_1)
                {
                    this.D_1 = D_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DTWO : percentxZEROZEROⲻFF
            {
                public DTWO(Inners.D D_1, Inners.TWO TWO_1)
                {
                    this.D_1 = D_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DTHREE : percentxZEROZEROⲻFF
            {
                public DTHREE(Inners.D D_1, Inners.THREE THREE_1)
                {
                    this.D_1 = D_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DFOUR : percentxZEROZEROⲻFF
            {
                public DFOUR(Inners.D D_1, Inners.FOUR FOUR_1)
                {
                    this.D_1 = D_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DFIVE : percentxZEROZEROⲻFF
            {
                public DFIVE(Inners.D D_1, Inners.FIVE FIVE_1)
                {
                    this.D_1 = D_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DSIX : percentxZEROZEROⲻFF
            {
                public DSIX(Inners.D D_1, Inners.SIX SIX_1)
                {
                    this.D_1 = D_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DSEVEN : percentxZEROZEROⲻFF
            {
                public DSEVEN(Inners.D D_1, Inners.SEVEN SEVEN_1)
                {
                    this.D_1 = D_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DEIGHT : percentxZEROZEROⲻFF
            {
                public DEIGHT(Inners.D D_1, Inners.EIGHT EIGHT_1)
                {
                    this.D_1 = D_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DNINE : percentxZEROZEROⲻFF
            {
                public DNINE(Inners.D D_1, Inners.NINE NINE_1)
                {
                    this.D_1 = D_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DA : percentxZEROZEROⲻFF
            {
                public DA(Inners.D D_1, Inners.A A_1)
                {
                    this.D_1 = D_1;
                    this.A_1 = A_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DB : percentxZEROZEROⲻFF
            {
                public DB(Inners.D D_1, Inners.B B_1)
                {
                    this.D_1 = D_1;
                    this.B_1 = B_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DC : percentxZEROZEROⲻFF
            {
                public DC(Inners.D D_1, Inners.C C_1)
                {
                    this.D_1 = D_1;
                    this.C_1 = C_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DD : percentxZEROZEROⲻFF
            {
                public DD(Inners.D D_1, Inners.D D_2)
                {
                    this.D_1 = D_1;
                    this.D_2 = D_2;
                }
                
                public Inners.D D_1 { get; }
                public Inners.D D_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DE : percentxZEROZEROⲻFF
            {
                public DE(Inners.D D_1, Inners.E E_1)
                {
                    this.D_1 = D_1;
                    this.E_1 = E_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DF : percentxZEROZEROⲻFF
            {
                public DF(Inners.D D_1, Inners.F F_1)
                {
                    this.D_1 = D_1;
                    this.F_1 = F_1;
                }
                
                public Inners.D D_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EZERO : percentxZEROZEROⲻFF
            {
                public EZERO(Inners.E E_1, Inners.ZERO ZERO_1)
                {
                    this.E_1 = E_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EONE : percentxZEROZEROⲻFF
            {
                public EONE(Inners.E E_1, Inners.ONE ONE_1)
                {
                    this.E_1 = E_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ETWO : percentxZEROZEROⲻFF
            {
                public ETWO(Inners.E E_1, Inners.TWO TWO_1)
                {
                    this.E_1 = E_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ETHREE : percentxZEROZEROⲻFF
            {
                public ETHREE(Inners.E E_1, Inners.THREE THREE_1)
                {
                    this.E_1 = E_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EFOUR : percentxZEROZEROⲻFF
            {
                public EFOUR(Inners.E E_1, Inners.FOUR FOUR_1)
                {
                    this.E_1 = E_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EFIVE : percentxZEROZEROⲻFF
            {
                public EFIVE(Inners.E E_1, Inners.FIVE FIVE_1)
                {
                    this.E_1 = E_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ESIX : percentxZEROZEROⲻFF
            {
                public ESIX(Inners.E E_1, Inners.SIX SIX_1)
                {
                    this.E_1 = E_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ESEVEN : percentxZEROZEROⲻFF
            {
                public ESEVEN(Inners.E E_1, Inners.SEVEN SEVEN_1)
                {
                    this.E_1 = E_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EEIGHT : percentxZEROZEROⲻFF
            {
                public EEIGHT(Inners.E E_1, Inners.EIGHT EIGHT_1)
                {
                    this.E_1 = E_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ENINE : percentxZEROZEROⲻFF
            {
                public ENINE(Inners.E E_1, Inners.NINE NINE_1)
                {
                    this.E_1 = E_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EA : percentxZEROZEROⲻFF
            {
                public EA(Inners.E E_1, Inners.A A_1)
                {
                    this.E_1 = E_1;
                    this.A_1 = A_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EB : percentxZEROZEROⲻFF
            {
                public EB(Inners.E E_1, Inners.B B_1)
                {
                    this.E_1 = E_1;
                    this.B_1 = B_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EC : percentxZEROZEROⲻFF
            {
                public EC(Inners.E E_1, Inners.C C_1)
                {
                    this.E_1 = E_1;
                    this.C_1 = C_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class ED : percentxZEROZEROⲻFF
            {
                public ED(Inners.E E_1, Inners.D D_1)
                {
                    this.E_1 = E_1;
                    this.D_1 = D_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EE : percentxZEROZEROⲻFF
            {
                public EE(Inners.E E_1, Inners.E E_2)
                {
                    this.E_1 = E_1;
                    this.E_2 = E_2;
                }
                
                public Inners.E E_1 { get; }
                public Inners.E E_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class EF : percentxZEROZEROⲻFF
            {
                public EF(Inners.E E_1, Inners.F F_1)
                {
                    this.E_1 = E_1;
                    this.F_1 = F_1;
                }
                
                public Inners.E E_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FZERO : percentxZEROZEROⲻFF
            {
                public FZERO(Inners.F F_1, Inners.ZERO ZERO_1)
                {
                    this.F_1 = F_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FONE : percentxZEROZEROⲻFF
            {
                public FONE(Inners.F F_1, Inners.ONE ONE_1)
                {
                    this.F_1 = F_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FTWO : percentxZEROZEROⲻFF
            {
                public FTWO(Inners.F F_1, Inners.TWO TWO_1)
                {
                    this.F_1 = F_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FTHREE : percentxZEROZEROⲻFF
            {
                public FTHREE(Inners.F F_1, Inners.THREE THREE_1)
                {
                    this.F_1 = F_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FFOUR : percentxZEROZEROⲻFF
            {
                public FFOUR(Inners.F F_1, Inners.FOUR FOUR_1)
                {
                    this.F_1 = F_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FFIVE : percentxZEROZEROⲻFF
            {
                public FFIVE(Inners.F F_1, Inners.FIVE FIVE_1)
                {
                    this.F_1 = F_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FSIX : percentxZEROZEROⲻFF
            {
                public FSIX(Inners.F F_1, Inners.SIX SIX_1)
                {
                    this.F_1 = F_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FSEVEN : percentxZEROZEROⲻFF
            {
                public FSEVEN(Inners.F F_1, Inners.SEVEN SEVEN_1)
                {
                    this.F_1 = F_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FEIGHT : percentxZEROZEROⲻFF
            {
                public FEIGHT(Inners.F F_1, Inners.EIGHT EIGHT_1)
                {
                    this.F_1 = F_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FNINE : percentxZEROZEROⲻFF
            {
                public FNINE(Inners.F F_1, Inners.NINE NINE_1)
                {
                    this.F_1 = F_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FA : percentxZEROZEROⲻFF
            {
                public FA(Inners.F F_1, Inners.A A_1)
                {
                    this.F_1 = F_1;
                    this.A_1 = A_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FB : percentxZEROZEROⲻFF
            {
                public FB(Inners.F F_1, Inners.B B_1)
                {
                    this.F_1 = F_1;
                    this.B_1 = B_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FC : percentxZEROZEROⲻFF
            {
                public FC(Inners.F F_1, Inners.C C_1)
                {
                    this.F_1 = F_1;
                    this.C_1 = C_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FD : percentxZEROZEROⲻFF
            {
                public FD(Inners.F F_1, Inners.D D_1)
                {
                    this.F_1 = F_1;
                    this.D_1 = D_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FE : percentxZEROZEROⲻFF
            {
                public FE(Inners.F F_1, Inners.E E_1)
                {
                    this.F_1 = F_1;
                    this.E_1 = E_1;
                }
                
                public Inners.F F_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FF : percentxZEROZEROⲻFF
            {
                public FF(Inners.F F_1, Inners.F F_2)
                {
                    this.F_1 = F_1;
                    this.F_2 = F_2;
                }
                
                public Inners.F F_1 { get; }
                public Inners.F F_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class percentxTWOZERO
        {
            public percentxTWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
            {
                this.TWO_1 = TWO_1;
                this.ZERO_1 = ZERO_1;
            }
            
            public Inners.TWO TWO_1 { get; }
            public Inners.ZERO ZERO_1 { get; }
        }
        
        public abstract class percentxTWOONEⲻSEVENE
        {
            private percentxTWOONEⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOONEⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxTWOONEⲻSEVENE.SEVENE node, TContext context);
            }
            
            public sealed class TWOONE : percentxTWOONEⲻSEVENE
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxTWOONEⲻSEVENE
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxTWOONEⲻSEVENE
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxTWOONEⲻSEVENE
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxTWOONEⲻSEVENE
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxTWOONEⲻSEVENE
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxTWOONEⲻSEVENE
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxTWOONEⲻSEVENE
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxTWOONEⲻSEVENE
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxTWOONEⲻSEVENE
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxTWOONEⲻSEVENE
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxTWOONEⲻSEVENE
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxTWOONEⲻSEVENE
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxTWOONEⲻSEVENE
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxTWOONEⲻSEVENE
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxTWOONEⲻSEVENE
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTWOONEⲻSEVENE
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTWOONEⲻSEVENE
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTWOONEⲻSEVENE
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTWOONEⲻSEVENE
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTWOONEⲻSEVENE
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTWOONEⲻSEVENE
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTWOONEⲻSEVENE
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTWOONEⲻSEVENE
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTWOONEⲻSEVENE
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxTWOONEⲻSEVENE
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxTWOONEⲻSEVENE
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxTWOONEⲻSEVENE
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxTWOONEⲻSEVENE
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxTWOONEⲻSEVENE
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxTWOONEⲻSEVENE
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxTWOONEⲻSEVENE
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxTWOONEⲻSEVENE
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxTWOONEⲻSEVENE
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxTWOONEⲻSEVENE
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxTWOONEⲻSEVENE
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxTWOONEⲻSEVENE
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxTWOONEⲻSEVENE
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxTWOONEⲻSEVENE
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxTWOONEⲻSEVENE
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxTWOONEⲻSEVENE
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxTWOONEⲻSEVENE
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxTWOONEⲻSEVENE
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxTWOONEⲻSEVENE
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxTWOONEⲻSEVENE
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxTWOONEⲻSEVENE
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxTWOONEⲻSEVENE
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxTWOONEⲻSEVENE
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxTWOONEⲻSEVENE
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxTWOONEⲻSEVENE
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxTWOONEⲻSEVENE
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxTWOONEⲻSEVENE
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxTWOONEⲻSEVENE
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxTWOONEⲻSEVENE
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxTWOONEⲻSEVENE
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxTWOONEⲻSEVENE
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxTWOONEⲻSEVENE
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxTWOONEⲻSEVENE
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxTWOONEⲻSEVENE
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxTWOONEⲻSEVENE
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxTWOONEⲻSEVENE
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxTWOONEⲻSEVENE
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxTWOONEⲻSEVENE
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxTWOONEⲻSEVENE
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxTWOONEⲻSEVENE
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxTWOONEⲻSEVENE
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxTWOONEⲻSEVENE
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxTWOONEⲻSEVENE
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxTWOONEⲻSEVENE
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxTWOONEⲻSEVENE
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxTWOONEⲻSEVENE
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxTWOONEⲻSEVENE
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxTWOONEⲻSEVENE
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxTWOONEⲻSEVENE
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxTWOONEⲻSEVENE
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxTWOONEⲻSEVENE
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxTWOONEⲻSEVENE
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxTWOONEⲻSEVENE
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxTWOONEⲻSEVENE
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxTWOONEⲻSEVENE
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxTWOONEⲻSEVENE
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxTWOONEⲻSEVENE
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxTWOONEⲻSEVENE
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxTWOONEⲻSEVENE
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxTWOONEⲻSEVENE
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxTWOONEⲻSEVENE
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxTWOONEⲻSEVENE
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxTWOONEⲻSEVENE
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxTWOONEⲻSEVENE
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxTWOONEⲻSEVENE
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxTWOONEⲻSEVENE
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxTWOONEⲻSEVENE
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxTWOONEⲻSEVENE
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxTWOONEⲻSEVENE
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class asteriskcⲻwsp_cⲻnl
        {
            public asteriskcⲻwsp_cⲻnl(IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, GeneratorV3.Abnf.cⲻnl cⲻnl_1)
            {
                this.cⲻwsp_1 = cⲻwsp_1;
                this.cⲻnl_1 = cⲻnl_1;
            }
            
            public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
            public GeneratorV3.Abnf.cⲻnl cⲻnl_1 { get; }
        }
        
        public sealed class openasteriskcⲻwsp_cⲻnlↃ
        {
            public openasteriskcⲻwsp_cⲻnlↃ(Inners.asteriskcⲻwsp_cⲻnl asteriskcⲻwsp_cⲻnl_1)
            {
                this.asteriskcⲻwsp_cⲻnl_1 = asteriskcⲻwsp_cⲻnl_1;
            }
            
            public Inners.asteriskcⲻwsp_cⲻnl asteriskcⲻwsp_cⲻnl_1 { get; }
        }
        
        public abstract class ruleⳆopenasteriskcⲻwsp_cⲻnlↃ
        {
            private ruleⳆopenasteriskcⲻwsp_cⲻnlↃ()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(ruleⳆopenasteriskcⲻwsp_cⲻnlↃ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(ruleⳆopenasteriskcⲻwsp_cⲻnlↃ.rule node, TContext context);
                protected internal abstract TResult Accept(ruleⳆopenasteriskcⲻwsp_cⲻnlↃ.openasteriskcⲻwsp_cⲻnlↃ node, TContext context);
            }
            
            public sealed class rule : ruleⳆopenasteriskcⲻwsp_cⲻnlↃ
            {
                public rule(GeneratorV3.Abnf.rule rule_1)
                {
                    this.rule_1 = rule_1;
                }
                
                public GeneratorV3.Abnf.rule rule_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class openasteriskcⲻwsp_cⲻnlↃ : ruleⳆopenasteriskcⲻwsp_cⲻnlↃ
            {
                public openasteriskcⲻwsp_cⲻnlↃ(Inners.openasteriskcⲻwsp_cⲻnlↃ openasteriskcⲻwsp_cⲻnlↃ_1)
                {
                    this.openasteriskcⲻwsp_cⲻnlↃ_1 = openasteriskcⲻwsp_cⲻnlↃ_1;
                }
                
                public Inners.openasteriskcⲻwsp_cⲻnlↃ openasteriskcⲻwsp_cⲻnlↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ
        {
            public openruleⳆopenasteriskcⲻwsp_cⲻnlↃↃ(Inners.ruleⳆopenasteriskcⲻwsp_cⲻnlↃ ruleⳆopenasteriskcⲻwsp_cⲻnlↃ_1)
            {
                this.ruleⳆopenasteriskcⲻwsp_cⲻnlↃ_1 = ruleⳆopenasteriskcⲻwsp_cⲻnlↃ_1;
            }
            
            public Inners.ruleⳆopenasteriskcⲻwsp_cⲻnlↃ ruleⳆopenasteriskcⲻwsp_cⲻnlↃ_1 { get; }
        }
        
        public sealed class x2D
        {
            private x2D()
            {
            }
            
            public static x2D Instance { get; } = new x2D();
        }
        
        public sealed class doublequotex2Ddoublequote
        {
            public doublequotex2Ddoublequote(Inners.x2D x2D_1)
            {
                this.x2D_1 = x2D_1;
            }
            
            public Inners.x2D x2D_1 { get; }
        }
        
        public abstract class ALPHAⳆDIGITⳆdoublequotex2Ddoublequote
        {
            private ALPHAⳆDIGITⳆdoublequotex2Ddoublequote()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(ALPHAⳆDIGITⳆdoublequotex2Ddoublequote node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(ALPHAⳆDIGITⳆdoublequotex2Ddoublequote.ALPHA node, TContext context);
                protected internal abstract TResult Accept(ALPHAⳆDIGITⳆdoublequotex2Ddoublequote.DIGIT node, TContext context);
                protected internal abstract TResult Accept(ALPHAⳆDIGITⳆdoublequotex2Ddoublequote.doublequotex2Ddoublequote node, TContext context);
            }
            
            public sealed class ALPHA : ALPHAⳆDIGITⳆdoublequotex2Ddoublequote
            {
                public ALPHA(GeneratorV3.Abnf.ALPHA ALPHA_1)
                {
                    this.ALPHA_1 = ALPHA_1;
                }
                
                public GeneratorV3.Abnf.ALPHA ALPHA_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class DIGIT : ALPHAⳆDIGITⳆdoublequotex2Ddoublequote
            {
                public DIGIT(GeneratorV3.Abnf.DIGIT DIGIT_1)
                {
                    this.DIGIT_1 = DIGIT_1;
                }
                
                public GeneratorV3.Abnf.DIGIT DIGIT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class doublequotex2Ddoublequote : ALPHAⳆDIGITⳆdoublequotex2Ddoublequote
            {
                public doublequotex2Ddoublequote(Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1)
                {
                    this.doublequotex2Ddoublequote_1 = doublequotex2Ddoublequote_1;
                }
                
                public Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ
        {
            public openALPHAⳆDIGITⳆdoublequotex2DdoublequoteↃ(Inners.ALPHAⳆDIGITⳆdoublequotex2Ddoublequote ALPHAⳆDIGITⳆdoublequotex2Ddoublequote_1)
            {
                this.ALPHAⳆDIGITⳆdoublequotex2Ddoublequote_1 = ALPHAⳆDIGITⳆdoublequotex2Ddoublequote_1;
            }
            
            public Inners.ALPHAⳆDIGITⳆdoublequotex2Ddoublequote ALPHAⳆDIGITⳆdoublequotex2Ddoublequote_1 { get; }
        }
        
        public sealed class x3D
        {
            private x3D()
            {
            }
            
            public static x3D Instance { get; } = new x3D();
        }
        
        public sealed class doublequotex3Ddoublequote
        {
            public doublequotex3Ddoublequote(Inners.x3D x3D_1)
            {
                this.x3D_1 = x3D_1;
            }
            
            public Inners.x3D x3D_1 { get; }
        }
        
        public sealed class x2F
        {
            private x2F()
            {
            }
            
            public static x2F Instance { get; } = new x2F();
        }
        
        public sealed class doublequotex3Dx2Fdoublequote
        {
            public doublequotex3Dx2Fdoublequote(Inners.x3D x3D_1, Inners.x2F x2F_1)
            {
                this.x3D_1 = x3D_1;
                this.x2F_1 = x2F_1;
            }
            
            public Inners.x3D x3D_1 { get; }
            public Inners.x2F x2F_1 { get; }
        }
        
        public abstract class doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote
        {
            private doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote.doublequotex3Ddoublequote node, TContext context);
                protected internal abstract TResult Accept(doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote.doublequotex3Dx2Fdoublequote node, TContext context);
            }
            
            public sealed class doublequotex3Ddoublequote : doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote
            {
                public doublequotex3Ddoublequote(Inners.doublequotex3Ddoublequote doublequotex3Ddoublequote_1)
                {
                    this.doublequotex3Ddoublequote_1 = doublequotex3Ddoublequote_1;
                }
                
                public Inners.doublequotex3Ddoublequote doublequotex3Ddoublequote_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class doublequotex3Dx2Fdoublequote : doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote
            {
                public doublequotex3Dx2Fdoublequote(Inners.doublequotex3Dx2Fdoublequote doublequotex3Dx2Fdoublequote_1)
                {
                    this.doublequotex3Dx2Fdoublequote_1 = doublequotex3Dx2Fdoublequote_1;
                }
                
                public Inners.doublequotex3Dx2Fdoublequote doublequotex3Dx2Fdoublequote_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ
        {
            public opendoublequotex3DdoublequoteⳆdoublequotex3Dx2FdoublequoteↃ(Inners.doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote_1)
            {
                this.doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote_1 = doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote_1;
            }
            
            public Inners.doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote doublequotex3DdoublequoteⳆdoublequotex3Dx2Fdoublequote_1 { get; }
        }
        
        public sealed class cⲻnl_WSP
        {
            public cⲻnl_WSP(GeneratorV3.Abnf.cⲻnl cⲻnl_1, GeneratorV3.Abnf.WSP WSP_1)
            {
                this.cⲻnl_1 = cⲻnl_1;
                this.WSP_1 = WSP_1;
            }
            
            public GeneratorV3.Abnf.cⲻnl cⲻnl_1 { get; }
            public GeneratorV3.Abnf.WSP WSP_1 { get; }
        }
        
        public sealed class opencⲻnl_WSPↃ
        {
            public opencⲻnl_WSPↃ(Inners.cⲻnl_WSP cⲻnl_WSP_1)
            {
                this.cⲻnl_WSP_1 = cⲻnl_WSP_1;
            }
            
            public Inners.cⲻnl_WSP cⲻnl_WSP_1 { get; }
        }
        
        public sealed class x3B
        {
            private x3B()
            {
            }
            
            public static x3B Instance { get; } = new x3B();
        }
        
        public sealed class doublequotex3Bdoublequote
        {
            public doublequotex3Bdoublequote(Inners.x3B x3B_1)
            {
                this.x3B_1 = x3B_1;
            }
            
            public Inners.x3B x3B_1 { get; }
        }
        
        public abstract class WSPⳆVCHAR
        {
            private WSPⳆVCHAR()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(WSPⳆVCHAR node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(WSPⳆVCHAR.WSP node, TContext context);
                protected internal abstract TResult Accept(WSPⳆVCHAR.VCHAR node, TContext context);
            }
            
            public sealed class WSP : WSPⳆVCHAR
            {
                public WSP(GeneratorV3.Abnf.WSP WSP_1)
                {
                    this.WSP_1 = WSP_1;
                }
                
                public GeneratorV3.Abnf.WSP WSP_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class VCHAR : WSPⳆVCHAR
            {
                public VCHAR(GeneratorV3.Abnf.VCHAR VCHAR_1)
                {
                    this.VCHAR_1 = VCHAR_1;
                }
                
                public GeneratorV3.Abnf.VCHAR VCHAR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openWSPⳆVCHARↃ
        {
            public openWSPⳆVCHARↃ(Inners.WSPⳆVCHAR WSPⳆVCHAR_1)
            {
                this.WSPⳆVCHAR_1 = WSPⳆVCHAR_1;
            }
            
            public Inners.WSPⳆVCHAR WSPⳆVCHAR_1 { get; }
        }
        
        public sealed class doublequotex2Fdoublequote
        {
            public doublequotex2Fdoublequote(Inners.x2F x2F_1)
            {
                this.x2F_1 = x2F_1;
            }
            
            public Inners.x2F x2F_1 { get; }
        }
        
        public sealed class asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation
        {
            public asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation(IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, Inners.doublequotex2Fdoublequote doublequotex2Fdoublequote_1, IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2, GeneratorV3.Abnf.concatenation concatenation_1)
            {
                this.cⲻwsp_1 = cⲻwsp_1;
                this.doublequotex2Fdoublequote_1 = doublequotex2Fdoublequote_1;
                this.cⲻwsp_2 = cⲻwsp_2;
                this.concatenation_1 = concatenation_1;
            }
            
            public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
            public Inners.doublequotex2Fdoublequote doublequotex2Fdoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_2 { get; }
            public GeneratorV3.Abnf.concatenation concatenation_1 { get; }
        }
        
        public sealed class openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ
        {
            public openasteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenationↃ(Inners.asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation_1)
            {
                this.asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation_1 = asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation_1;
            }
            
            public Inners.asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation asteriskcⲻwsp_doublequotex2Fdoublequote_asteriskcⲻwsp_concatenation_1 { get; }
        }
        
        public sealed class ONEasteriskcⲻwsp_repetition
        {
            public ONEasteriskcⲻwsp_repetition(IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1, GeneratorV3.Abnf.repetition repetition_1)
            {
                this.cⲻwsp_1 = cⲻwsp_1;
                this.repetition_1 = repetition_1;
            }
            
            public IEnumerable<GeneratorV3.Abnf.cⲻwsp> cⲻwsp_1 { get; }
            public GeneratorV3.Abnf.repetition repetition_1 { get; }
        }
        
        public sealed class openONEasteriskcⲻwsp_repetitionↃ
        {
            public openONEasteriskcⲻwsp_repetitionↃ(Inners.ONEasteriskcⲻwsp_repetition ONEasteriskcⲻwsp_repetition_1)
            {
                this.ONEasteriskcⲻwsp_repetition_1 = ONEasteriskcⲻwsp_repetition_1;
            }
            
            public Inners.ONEasteriskcⲻwsp_repetition ONEasteriskcⲻwsp_repetition_1 { get; }
        }
        
        public sealed class x2A
        {
            private x2A()
            {
            }
            
            public static x2A Instance { get; } = new x2A();
        }
        
        public sealed class doublequotex2Adoublequote
        {
            public doublequotex2Adoublequote(Inners.x2A x2A_1)
            {
                this.x2A_1 = x2A_1;
            }
            
            public Inners.x2A x2A_1 { get; }
        }
        
        public sealed class asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT
        {
            public asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT(IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1, Inners.doublequotex2Adoublequote doublequotex2Adoublequote_1, IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_2)
            {
                this.DIGIT_1 = DIGIT_1;
                this.doublequotex2Adoublequote_1 = doublequotex2Adoublequote_1;
                this.DIGIT_2 = DIGIT_2;
            }
            
            public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1 { get; }
            public Inners.doublequotex2Adoublequote doublequotex2Adoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_2 { get; }
        }
        
        public sealed class openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ
        {
            public openasteriskDIGIT_doublequotex2Adoublequote_asteriskDIGITↃ(Inners.asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT_1)
            {
                this.asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT_1 = asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT_1;
            }
            
            public Inners.asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT asteriskDIGIT_doublequotex2Adoublequote_asteriskDIGIT_1 { get; }
        }
        
        public sealed class x28
        {
            private x28()
            {
            }
            
            public static x28 Instance { get; } = new x28();
        }
        
        public sealed class doublequotex28doublequote
        {
            public doublequotex28doublequote(Inners.x28 x28_1)
            {
                this.x28_1 = x28_1;
            }
            
            public Inners.x28 x28_1 { get; }
        }
        
        public sealed class x29
        {
            private x29()
            {
            }
            
            public static x29 Instance { get; } = new x29();
        }
        
        public sealed class doublequotex29doublequote
        {
            public doublequotex29doublequote(Inners.x29 x29_1)
            {
                this.x29_1 = x29_1;
            }
            
            public Inners.x29 x29_1 { get; }
        }
        
        public sealed class x5B
        {
            private x5B()
            {
            }
            
            public static x5B Instance { get; } = new x5B();
        }
        
        public sealed class doublequotex5Bdoublequote
        {
            public doublequotex5Bdoublequote(Inners.x5B x5B_1)
            {
                this.x5B_1 = x5B_1;
            }
            
            public Inners.x5B x5B_1 { get; }
        }
        
        public sealed class x5D
        {
            private x5D()
            {
            }
            
            public static x5D Instance { get; } = new x5D();
        }
        
        public sealed class doublequotex5Ddoublequote
        {
            public doublequotex5Ddoublequote(Inners.x5D x5D_1)
            {
                this.x5D_1 = x5D_1;
            }
            
            public Inners.x5D x5D_1 { get; }
        }
        
        public abstract class percentxTWOZEROⲻTWOONE
        {
            private percentxTWOZEROⲻTWOONE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOZEROⲻTWOONE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOZEROⲻTWOONE.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTWOONE.TWOONE node, TContext context);
            }
            
            public sealed class TWOZERO : percentxTWOZEROⲻTWOONE
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxTWOZEROⲻTWOONE
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxTWOTHREEⲻSEVENE
        {
            private percentxTWOTHREEⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOTHREEⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxTWOTHREEⲻSEVENE.SEVENE node, TContext context);
            }
            
            public sealed class TWOTHREE : percentxTWOTHREEⲻSEVENE
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxTWOTHREEⲻSEVENE
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxTWOTHREEⲻSEVENE
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxTWOTHREEⲻSEVENE
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxTWOTHREEⲻSEVENE
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxTWOTHREEⲻSEVENE
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxTWOTHREEⲻSEVENE
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxTWOTHREEⲻSEVENE
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxTWOTHREEⲻSEVENE
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxTWOTHREEⲻSEVENE
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxTWOTHREEⲻSEVENE
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxTWOTHREEⲻSEVENE
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxTWOTHREEⲻSEVENE
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxTWOTHREEⲻSEVENE
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTWOTHREEⲻSEVENE
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTWOTHREEⲻSEVENE
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTWOTHREEⲻSEVENE
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTWOTHREEⲻSEVENE
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTWOTHREEⲻSEVENE
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTWOTHREEⲻSEVENE
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTWOTHREEⲻSEVENE
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTWOTHREEⲻSEVENE
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTWOTHREEⲻSEVENE
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxTWOTHREEⲻSEVENE
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxTWOTHREEⲻSEVENE
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxTWOTHREEⲻSEVENE
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxTWOTHREEⲻSEVENE
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEE : percentxTWOTHREEⲻSEVENE
            {
                public THREEE(Inners.THREE THREE_1, Inners.E E_1)
                {
                    this.THREE_1 = THREE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEF : percentxTWOTHREEⲻSEVENE
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxTWOTHREEⲻSEVENE
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxTWOTHREEⲻSEVENE
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxTWOTHREEⲻSEVENE
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxTWOTHREEⲻSEVENE
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxTWOTHREEⲻSEVENE
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxTWOTHREEⲻSEVENE
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxTWOTHREEⲻSEVENE
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxTWOTHREEⲻSEVENE
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxTWOTHREEⲻSEVENE
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxTWOTHREEⲻSEVENE
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxTWOTHREEⲻSEVENE
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxTWOTHREEⲻSEVENE
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxTWOTHREEⲻSEVENE
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxTWOTHREEⲻSEVENE
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxTWOTHREEⲻSEVENE
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxTWOTHREEⲻSEVENE
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxTWOTHREEⲻSEVENE
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxTWOTHREEⲻSEVENE
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxTWOTHREEⲻSEVENE
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxTWOTHREEⲻSEVENE
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxTWOTHREEⲻSEVENE
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxTWOTHREEⲻSEVENE
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxTWOTHREEⲻSEVENE
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxTWOTHREEⲻSEVENE
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxTWOTHREEⲻSEVENE
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxTWOTHREEⲻSEVENE
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxTWOTHREEⲻSEVENE
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxTWOTHREEⲻSEVENE
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxTWOTHREEⲻSEVENE
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxTWOTHREEⲻSEVENE
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxTWOTHREEⲻSEVENE
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxTWOTHREEⲻSEVENE
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxTWOTHREEⲻSEVENE
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxTWOTHREEⲻSEVENE
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxTWOTHREEⲻSEVENE
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxTWOTHREEⲻSEVENE
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxTWOTHREEⲻSEVENE
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxTWOTHREEⲻSEVENE
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxTWOTHREEⲻSEVENE
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxTWOTHREEⲻSEVENE
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxTWOTHREEⲻSEVENE
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxTWOTHREEⲻSEVENE
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxTWOTHREEⲻSEVENE
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxTWOTHREEⲻSEVENE
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxTWOTHREEⲻSEVENE
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxTWOTHREEⲻSEVENE
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxTWOTHREEⲻSEVENE
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxTWOTHREEⲻSEVENE
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxTWOTHREEⲻSEVENE
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxTWOTHREEⲻSEVENE
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxTWOTHREEⲻSEVENE
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxTWOTHREEⲻSEVENE
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxTWOTHREEⲻSEVENE
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxTWOTHREEⲻSEVENE
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxTWOTHREEⲻSEVENE
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxTWOTHREEⲻSEVENE
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxTWOTHREEⲻSEVENE
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxTWOTHREEⲻSEVENE
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxTWOTHREEⲻSEVENE
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxTWOTHREEⲻSEVENE
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxTWOTHREEⲻSEVENE
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxTWOTHREEⲻSEVENE
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxTWOTHREEⲻSEVENE
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE
        {
            private percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE.percentxTWOZEROⲻTWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE.percentxTWOTHREEⲻSEVENE node, TContext context);
            }
            
            public sealed class percentxTWOZEROⲻTWOONE : percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE
            {
                public percentxTWOZEROⲻTWOONE(Inners.percentxTWOZEROⲻTWOONE percentxTWOZEROⲻTWOONE_1)
                {
                    this.percentxTWOZEROⲻTWOONE_1 = percentxTWOZEROⲻTWOONE_1;
                }
                
                public Inners.percentxTWOZEROⲻTWOONE percentxTWOZEROⲻTWOONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class percentxTWOTHREEⲻSEVENE : percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE
            {
                public percentxTWOTHREEⲻSEVENE(Inners.percentxTWOTHREEⲻSEVENE percentxTWOTHREEⲻSEVENE_1)
                {
                    this.percentxTWOTHREEⲻSEVENE_1 = percentxTWOTHREEⲻSEVENE_1;
                }
                
                public Inners.percentxTWOTHREEⲻSEVENE percentxTWOTHREEⲻSEVENE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ
        {
            public openpercentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENEↃ(Inners.percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE_1)
            {
                this.percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE_1 = percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE_1;
            }
            
            public Inners.percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE percentxTWOZEROⲻTWOONEⳆpercentxTWOTHREEⲻSEVENE_1 { get; }
        }
        
        public sealed class x25
        {
            private x25()
            {
            }
            
            public static x25 Instance { get; } = new x25();
        }
        
        public sealed class doublequotex25doublequote
        {
            public doublequotex25doublequote(Inners.x25 x25_1)
            {
                this.x25_1 = x25_1;
            }
            
            public Inners.x25 x25_1 { get; }
        }
        
        public abstract class binⲻvalⳆdecⲻvalⳆhexⲻval
        {
            private binⲻvalⳆdecⲻvalⳆhexⲻval()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(binⲻvalⳆdecⲻvalⳆhexⲻval node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(binⲻvalⳆdecⲻvalⳆhexⲻval.binⲻval node, TContext context);
                protected internal abstract TResult Accept(binⲻvalⳆdecⲻvalⳆhexⲻval.decⲻval node, TContext context);
                protected internal abstract TResult Accept(binⲻvalⳆdecⲻvalⳆhexⲻval.hexⲻval node, TContext context);
            }
            
            public sealed class binⲻval : binⲻvalⳆdecⲻvalⳆhexⲻval
            {
                public binⲻval(GeneratorV3.Abnf.binⲻval binⲻval_1)
                {
                    this.binⲻval_1 = binⲻval_1;
                }
                
                public GeneratorV3.Abnf.binⲻval binⲻval_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class decⲻval : binⲻvalⳆdecⲻvalⳆhexⲻval
            {
                public decⲻval(GeneratorV3.Abnf.decⲻval decⲻval_1)
                {
                    this.decⲻval_1 = decⲻval_1;
                }
                
                public GeneratorV3.Abnf.decⲻval decⲻval_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class hexⲻval : binⲻvalⳆdecⲻvalⳆhexⲻval
            {
                public hexⲻval(GeneratorV3.Abnf.hexⲻval hexⲻval_1)
                {
                    this.hexⲻval_1 = hexⲻval_1;
                }
                
                public GeneratorV3.Abnf.hexⲻval hexⲻval_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ
        {
            public openbinⲻvalⳆdecⲻvalⳆhexⲻvalↃ(Inners.binⲻvalⳆdecⲻvalⳆhexⲻval binⲻvalⳆdecⲻvalⳆhexⲻval_1)
            {
                this.binⲻvalⳆdecⲻvalⳆhexⲻval_1 = binⲻvalⳆdecⲻvalⳆhexⲻval_1;
            }
            
            public Inners.binⲻvalⳆdecⲻvalⳆhexⲻval binⲻvalⳆdecⲻvalⳆhexⲻval_1 { get; }
        }
        
        public sealed class x62
        {
            private x62()
            {
            }
            
            public static x62 Instance { get; } = new x62();
        }
        
        public sealed class doublequotex62doublequote
        {
            public doublequotex62doublequote(Inners.x62 x62_1)
            {
                this.x62_1 = x62_1;
            }
            
            public Inners.x62 x62_1 { get; }
        }
        
        public sealed class x2E
        {
            private x2E()
            {
            }
            
            public static x2E Instance { get; } = new x2E();
        }
        
        public sealed class doublequotex2Edoublequote
        {
            public doublequotex2Edoublequote(Inners.x2E x2E_1)
            {
                this.x2E_1 = x2E_1;
            }
            
            public Inners.x2E x2E_1 { get; }
        }
        
        public sealed class doublequotex2Edoublequote_ONEasteriskBIT
        {
            public doublequotex2Edoublequote_ONEasteriskBIT(Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1, IEnumerable<GeneratorV3.Abnf.BIT> BIT_1)
            {
                this.doublequotex2Edoublequote_1 = doublequotex2Edoublequote_1;
                this.BIT_1 = BIT_1;
            }
            
            public Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.BIT> BIT_1 { get; }
        }
        
        public sealed class opendoublequotex2Edoublequote_ONEasteriskBITↃ
        {
            public opendoublequotex2Edoublequote_ONEasteriskBITↃ(Inners.doublequotex2Edoublequote_ONEasteriskBIT doublequotex2Edoublequote_ONEasteriskBIT_1)
            {
                this.doublequotex2Edoublequote_ONEasteriskBIT_1 = doublequotex2Edoublequote_ONEasteriskBIT_1;
            }
            
            public Inners.doublequotex2Edoublequote_ONEasteriskBIT doublequotex2Edoublequote_ONEasteriskBIT_1 { get; }
        }
        
        public sealed class doublequotex2Ddoublequote_ONEasteriskBIT
        {
            public doublequotex2Ddoublequote_ONEasteriskBIT(Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1, IEnumerable<GeneratorV3.Abnf.BIT> BIT_1)
            {
                this.doublequotex2Ddoublequote_1 = doublequotex2Ddoublequote_1;
                this.BIT_1 = BIT_1;
            }
            
            public Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.BIT> BIT_1 { get; }
        }
        
        public sealed class opendoublequotex2Ddoublequote_ONEasteriskBITↃ
        {
            public opendoublequotex2Ddoublequote_ONEasteriskBITↃ(Inners.doublequotex2Ddoublequote_ONEasteriskBIT doublequotex2Ddoublequote_ONEasteriskBIT_1)
            {
                this.doublequotex2Ddoublequote_ONEasteriskBIT_1 = doublequotex2Ddoublequote_ONEasteriskBIT_1;
            }
            
            public Inners.doublequotex2Ddoublequote_ONEasteriskBIT doublequotex2Ddoublequote_ONEasteriskBIT_1 { get; }
        }
        
        public abstract class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ
        {
            private ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃ node, TContext context);
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ.opendoublequotex2Ddoublequote_ONEasteriskBITↃ node, TContext context);
            }
            
            public sealed class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ
            {
                public ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃ(IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskBITↃ> opendoublequotex2Edoublequote_ONEasteriskBITↃ_1)
                {
                    this.opendoublequotex2Edoublequote_ONEasteriskBITↃ_1 = opendoublequotex2Edoublequote_ONEasteriskBITↃ_1;
                }
                
                public IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskBITↃ> opendoublequotex2Edoublequote_ONEasteriskBITↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class opendoublequotex2Ddoublequote_ONEasteriskBITↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskBITↃⳆopendoublequotex2Ddoublequote_ONEasteriskBITↃ
            {
                public opendoublequotex2Ddoublequote_ONEasteriskBITↃ(Inners.opendoublequotex2Ddoublequote_ONEasteriskBITↃ opendoublequotex2Ddoublequote_ONEasteriskBITↃ_1)
                {
                    this.opendoublequotex2Ddoublequote_ONEasteriskBITↃ_1 = opendoublequotex2Ddoublequote_ONEasteriskBITↃ_1;
                }
                
                public Inners.opendoublequotex2Ddoublequote_ONEasteriskBITↃ opendoublequotex2Ddoublequote_ONEasteriskBITↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class x64
        {
            private x64()
            {
            }
            
            public static x64 Instance { get; } = new x64();
        }
        
        public sealed class doublequotex64doublequote
        {
            public doublequotex64doublequote(Inners.x64 x64_1)
            {
                this.x64_1 = x64_1;
            }
            
            public Inners.x64 x64_1 { get; }
        }
        
        public sealed class doublequotex2Edoublequote_ONEasteriskDIGIT
        {
            public doublequotex2Edoublequote_ONEasteriskDIGIT(Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1, IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1)
            {
                this.doublequotex2Edoublequote_1 = doublequotex2Edoublequote_1;
                this.DIGIT_1 = DIGIT_1;
            }
            
            public Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1 { get; }
        }
        
        public sealed class opendoublequotex2Edoublequote_ONEasteriskDIGITↃ
        {
            public opendoublequotex2Edoublequote_ONEasteriskDIGITↃ(Inners.doublequotex2Edoublequote_ONEasteriskDIGIT doublequotex2Edoublequote_ONEasteriskDIGIT_1)
            {
                this.doublequotex2Edoublequote_ONEasteriskDIGIT_1 = doublequotex2Edoublequote_ONEasteriskDIGIT_1;
            }
            
            public Inners.doublequotex2Edoublequote_ONEasteriskDIGIT doublequotex2Edoublequote_ONEasteriskDIGIT_1 { get; }
        }
        
        public sealed class doublequotex2Ddoublequote_ONEasteriskDIGIT
        {
            public doublequotex2Ddoublequote_ONEasteriskDIGIT(Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1, IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1)
            {
                this.doublequotex2Ddoublequote_1 = doublequotex2Ddoublequote_1;
                this.DIGIT_1 = DIGIT_1;
            }
            
            public Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.DIGIT> DIGIT_1 { get; }
        }
        
        public sealed class opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ
        {
            public opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ(Inners.doublequotex2Ddoublequote_ONEasteriskDIGIT doublequotex2Ddoublequote_ONEasteriskDIGIT_1)
            {
                this.doublequotex2Ddoublequote_ONEasteriskDIGIT_1 = doublequotex2Ddoublequote_ONEasteriskDIGIT_1;
            }
            
            public Inners.doublequotex2Ddoublequote_ONEasteriskDIGIT doublequotex2Ddoublequote_ONEasteriskDIGIT_1 { get; }
        }
        
        public abstract class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ
        {
            private ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃ node, TContext context);
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ.opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ node, TContext context);
            }
            
            public sealed class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ
            {
                public ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃ(IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskDIGITↃ> opendoublequotex2Edoublequote_ONEasteriskDIGITↃ_1)
                {
                    this.opendoublequotex2Edoublequote_ONEasteriskDIGITↃ_1 = opendoublequotex2Edoublequote_ONEasteriskDIGITↃ_1;
                }
                
                public IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskDIGITↃ> opendoublequotex2Edoublequote_ONEasteriskDIGITↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskDIGITↃⳆopendoublequotex2Ddoublequote_ONEasteriskDIGITↃ
            {
                public opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ(Inners.opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1)
                {
                    this.opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1 = opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1;
                }
                
                public Inners.opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ opendoublequotex2Ddoublequote_ONEasteriskDIGITↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class x78
        {
            private x78()
            {
            }
            
            public static x78 Instance { get; } = new x78();
        }
        
        public sealed class doublequotex78doublequote
        {
            public doublequotex78doublequote(Inners.x78 x78_1)
            {
                this.x78_1 = x78_1;
            }
            
            public Inners.x78 x78_1 { get; }
        }
        
        public sealed class doublequotex2Edoublequote_ONEasteriskHEXDIG
        {
            public doublequotex2Edoublequote_ONEasteriskHEXDIG(Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1, IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1)
            {
                this.doublequotex2Edoublequote_1 = doublequotex2Edoublequote_1;
                this.HEXDIG_1 = HEXDIG_1;
            }
            
            public Inners.doublequotex2Edoublequote doublequotex2Edoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1 { get; }
        }
        
        public sealed class opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ
        {
            public opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ(Inners.doublequotex2Edoublequote_ONEasteriskHEXDIG doublequotex2Edoublequote_ONEasteriskHEXDIG_1)
            {
                this.doublequotex2Edoublequote_ONEasteriskHEXDIG_1 = doublequotex2Edoublequote_ONEasteriskHEXDIG_1;
            }
            
            public Inners.doublequotex2Edoublequote_ONEasteriskHEXDIG doublequotex2Edoublequote_ONEasteriskHEXDIG_1 { get; }
        }
        
        public sealed class doublequotex2Ddoublequote_ONEasteriskHEXDIG
        {
            public doublequotex2Ddoublequote_ONEasteriskHEXDIG(Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1, IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1)
            {
                this.doublequotex2Ddoublequote_1 = doublequotex2Ddoublequote_1;
                this.HEXDIG_1 = HEXDIG_1;
            }
            
            public Inners.doublequotex2Ddoublequote doublequotex2Ddoublequote_1 { get; }
            public IEnumerable<GeneratorV3.Abnf.HEXDIG> HEXDIG_1 { get; }
        }
        
        public sealed class opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ
        {
            public opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ(Inners.doublequotex2Ddoublequote_ONEasteriskHEXDIG doublequotex2Ddoublequote_ONEasteriskHEXDIG_1)
            {
                this.doublequotex2Ddoublequote_ONEasteriskHEXDIG_1 = doublequotex2Ddoublequote_ONEasteriskHEXDIG_1;
            }
            
            public Inners.doublequotex2Ddoublequote_ONEasteriskHEXDIG doublequotex2Ddoublequote_ONEasteriskHEXDIG_1 { get; }
        }
        
        public abstract class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ
        {
            private ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ.ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ node, TContext context);
                protected internal abstract TResult Accept(ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ.opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ node, TContext context);
            }
            
            public sealed class ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ
            {
                public ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ(IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ> opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ_1)
                {
                    this.opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ_1 = opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ_1;
                }
                
                public IEnumerable<Inners.opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ> opendoublequotex2Edoublequote_ONEasteriskHEXDIGↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ : ONEasteriskopendoublequotex2Edoublequote_ONEasteriskHEXDIGↃⳆopendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ
            {
                public opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ(Inners.opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1)
                {
                    this.opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1 = opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1;
                }
                
                public Inners.opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ opendoublequotex2Ddoublequote_ONEasteriskHEXDIGↃ_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class x3C
        {
            private x3C()
            {
            }
            
            public static x3C Instance { get; } = new x3C();
        }
        
        public sealed class doublequotex3Cdoublequote
        {
            public doublequotex3Cdoublequote(Inners.x3C x3C_1)
            {
                this.x3C_1 = x3C_1;
            }
            
            public Inners.x3C x3C_1 { get; }
        }
        
        public abstract class percentxTWOZEROⲻTHREED
        {
            private percentxTWOZEROⲻTHREED()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOZEROⲻTHREED node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWONINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOD node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.TWOF node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEA node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEB node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREEC node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREED.THREED node, TContext context);
            }
            
            public sealed class TWOZERO : percentxTWOZEROⲻTHREED
            {
                public TWOZERO(Inners.TWO TWO_1, Inners.ZERO ZERO_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOONE : percentxTWOZEROⲻTHREED
            {
                public TWOONE(Inners.TWO TWO_1, Inners.ONE ONE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTWO : percentxTWOZEROⲻTHREED
            {
                public TWOTWO(Inners.TWO TWO_1, Inners.TWO TWO_2)
                {
                    this.TWO_1 = TWO_1;
                    this.TWO_2 = TWO_2;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.TWO TWO_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOTHREE : percentxTWOZEROⲻTHREED
            {
                public TWOTHREE(Inners.TWO TWO_1, Inners.THREE THREE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFOUR : percentxTWOZEROⲻTHREED
            {
                public TWOFOUR(Inners.TWO TWO_1, Inners.FOUR FOUR_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOFIVE : percentxTWOZEROⲻTHREED
            {
                public TWOFIVE(Inners.TWO TWO_1, Inners.FIVE FIVE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSIX : percentxTWOZEROⲻTHREED
            {
                public TWOSIX(Inners.TWO TWO_1, Inners.SIX SIX_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOSEVEN : percentxTWOZEROⲻTHREED
            {
                public TWOSEVEN(Inners.TWO TWO_1, Inners.SEVEN SEVEN_1)
                {
                    this.TWO_1 = TWO_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOEIGHT : percentxTWOZEROⲻTHREED
            {
                public TWOEIGHT(Inners.TWO TWO_1, Inners.EIGHT EIGHT_1)
                {
                    this.TWO_1 = TWO_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWONINE : percentxTWOZEROⲻTHREED
            {
                public TWONINE(Inners.TWO TWO_1, Inners.NINE NINE_1)
                {
                    this.TWO_1 = TWO_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOA : percentxTWOZEROⲻTHREED
            {
                public TWOA(Inners.TWO TWO_1, Inners.A A_1)
                {
                    this.TWO_1 = TWO_1;
                    this.A_1 = A_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOB : percentxTWOZEROⲻTHREED
            {
                public TWOB(Inners.TWO TWO_1, Inners.B B_1)
                {
                    this.TWO_1 = TWO_1;
                    this.B_1 = B_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOC : percentxTWOZEROⲻTHREED
            {
                public TWOC(Inners.TWO TWO_1, Inners.C C_1)
                {
                    this.TWO_1 = TWO_1;
                    this.C_1 = C_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOD : percentxTWOZEROⲻTHREED
            {
                public TWOD(Inners.TWO TWO_1, Inners.D D_1)
                {
                    this.TWO_1 = TWO_1;
                    this.D_1 = D_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOE : percentxTWOZEROⲻTHREED
            {
                public TWOE(Inners.TWO TWO_1, Inners.E E_1)
                {
                    this.TWO_1 = TWO_1;
                    this.E_1 = E_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class TWOF : percentxTWOZEROⲻTHREED
            {
                public TWOF(Inners.TWO TWO_1, Inners.F F_1)
                {
                    this.TWO_1 = TWO_1;
                    this.F_1 = F_1;
                }
                
                public Inners.TWO TWO_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEZERO : percentxTWOZEROⲻTHREED
            {
                public THREEZERO(Inners.THREE THREE_1, Inners.ZERO ZERO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEONE : percentxTWOZEROⲻTHREED
            {
                public THREEONE(Inners.THREE THREE_1, Inners.ONE ONE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETWO : percentxTWOZEROⲻTHREED
            {
                public THREETWO(Inners.THREE THREE_1, Inners.TWO TWO_1)
                {
                    this.THREE_1 = THREE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREETHREE : percentxTWOZEROⲻTHREED
            {
                public THREETHREE(Inners.THREE THREE_1, Inners.THREE THREE_2)
                {
                    this.THREE_1 = THREE_1;
                    this.THREE_2 = THREE_2;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.THREE THREE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFOUR : percentxTWOZEROⲻTHREED
            {
                public THREEFOUR(Inners.THREE THREE_1, Inners.FOUR FOUR_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEFIVE : percentxTWOZEROⲻTHREED
            {
                public THREEFIVE(Inners.THREE THREE_1, Inners.FIVE FIVE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESIX : percentxTWOZEROⲻTHREED
            {
                public THREESIX(Inners.THREE THREE_1, Inners.SIX SIX_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREESEVEN : percentxTWOZEROⲻTHREED
            {
                public THREESEVEN(Inners.THREE THREE_1, Inners.SEVEN SEVEN_1)
                {
                    this.THREE_1 = THREE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEEIGHT : percentxTWOZEROⲻTHREED
            {
                public THREEEIGHT(Inners.THREE THREE_1, Inners.EIGHT EIGHT_1)
                {
                    this.THREE_1 = THREE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREENINE : percentxTWOZEROⲻTHREED
            {
                public THREENINE(Inners.THREE THREE_1, Inners.NINE NINE_1)
                {
                    this.THREE_1 = THREE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEA : percentxTWOZEROⲻTHREED
            {
                public THREEA(Inners.THREE THREE_1, Inners.A A_1)
                {
                    this.THREE_1 = THREE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEB : percentxTWOZEROⲻTHREED
            {
                public THREEB(Inners.THREE THREE_1, Inners.B B_1)
                {
                    this.THREE_1 = THREE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREEC : percentxTWOZEROⲻTHREED
            {
                public THREEC(Inners.THREE THREE_1, Inners.C C_1)
                {
                    this.THREE_1 = THREE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class THREED : percentxTWOZEROⲻTHREED
            {
                public THREED(Inners.THREE THREE_1, Inners.D D_1)
                {
                    this.THREE_1 = THREE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxTHREEFⲻSEVENE
        {
            private percentxTHREEFⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTHREEFⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.THREEF node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOUREIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURA node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURB node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURC node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURD node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FOURF node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVETWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVETHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVESIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVESEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVENINE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEA node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEB node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEC node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVED node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.FIVEF node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXA node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXB node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXC node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXD node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SIXF node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENZERO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENONE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENTWO node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENTHREE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENFOUR node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENFIVE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENSIX node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENSEVEN node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENEIGHT node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENNINE node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENA node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENB node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENC node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVEND node, TContext context);
                protected internal abstract TResult Accept(percentxTHREEFⲻSEVENE.SEVENE node, TContext context);
            }
            
            public sealed class THREEF : percentxTHREEFⲻSEVENE
            {
                public THREEF(Inners.THREE THREE_1, Inners.F F_1)
                {
                    this.THREE_1 = THREE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.THREE THREE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURZERO : percentxTHREEFⲻSEVENE
            {
                public FOURZERO(Inners.FOUR FOUR_1, Inners.ZERO ZERO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURONE : percentxTHREEFⲻSEVENE
            {
                public FOURONE(Inners.FOUR FOUR_1, Inners.ONE ONE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTWO : percentxTHREEFⲻSEVENE
            {
                public FOURTWO(Inners.FOUR FOUR_1, Inners.TWO TWO_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURTHREE : percentxTHREEFⲻSEVENE
            {
                public FOURTHREE(Inners.FOUR FOUR_1, Inners.THREE THREE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFOUR : percentxTHREEFⲻSEVENE
            {
                public FOURFOUR(Inners.FOUR FOUR_1, Inners.FOUR FOUR_2)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FOUR_2 = FOUR_2;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FOUR FOUR_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURFIVE : percentxTHREEFⲻSEVENE
            {
                public FOURFIVE(Inners.FOUR FOUR_1, Inners.FIVE FIVE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSIX : percentxTHREEFⲻSEVENE
            {
                public FOURSIX(Inners.FOUR FOUR_1, Inners.SIX SIX_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURSEVEN : percentxTHREEFⲻSEVENE
            {
                public FOURSEVEN(Inners.FOUR FOUR_1, Inners.SEVEN SEVEN_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOUREIGHT : percentxTHREEFⲻSEVENE
            {
                public FOUREIGHT(Inners.FOUR FOUR_1, Inners.EIGHT EIGHT_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURNINE : percentxTHREEFⲻSEVENE
            {
                public FOURNINE(Inners.FOUR FOUR_1, Inners.NINE NINE_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURA : percentxTHREEFⲻSEVENE
            {
                public FOURA(Inners.FOUR FOUR_1, Inners.A A_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURB : percentxTHREEFⲻSEVENE
            {
                public FOURB(Inners.FOUR FOUR_1, Inners.B B_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURC : percentxTHREEFⲻSEVENE
            {
                public FOURC(Inners.FOUR FOUR_1, Inners.C C_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURD : percentxTHREEFⲻSEVENE
            {
                public FOURD(Inners.FOUR FOUR_1, Inners.D D_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURE : percentxTHREEFⲻSEVENE
            {
                public FOURE(Inners.FOUR FOUR_1, Inners.E E_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FOURF : percentxTHREEFⲻSEVENE
            {
                public FOURF(Inners.FOUR FOUR_1, Inners.F F_1)
                {
                    this.FOUR_1 = FOUR_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FOUR FOUR_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEZERO : percentxTHREEFⲻSEVENE
            {
                public FIVEZERO(Inners.FIVE FIVE_1, Inners.ZERO ZERO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEONE : percentxTHREEFⲻSEVENE
            {
                public FIVEONE(Inners.FIVE FIVE_1, Inners.ONE ONE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETWO : percentxTHREEFⲻSEVENE
            {
                public FIVETWO(Inners.FIVE FIVE_1, Inners.TWO TWO_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVETHREE : percentxTHREEFⲻSEVENE
            {
                public FIVETHREE(Inners.FIVE FIVE_1, Inners.THREE THREE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFOUR : percentxTHREEFⲻSEVENE
            {
                public FIVEFOUR(Inners.FIVE FIVE_1, Inners.FOUR FOUR_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEFIVE : percentxTHREEFⲻSEVENE
            {
                public FIVEFIVE(Inners.FIVE FIVE_1, Inners.FIVE FIVE_2)
                {
                    this.FIVE_1 = FIVE_1;
                    this.FIVE_2 = FIVE_2;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.FIVE FIVE_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESIX : percentxTHREEFⲻSEVENE
            {
                public FIVESIX(Inners.FIVE FIVE_1, Inners.SIX SIX_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVESEVEN : percentxTHREEFⲻSEVENE
            {
                public FIVESEVEN(Inners.FIVE FIVE_1, Inners.SEVEN SEVEN_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEEIGHT : percentxTHREEFⲻSEVENE
            {
                public FIVEEIGHT(Inners.FIVE FIVE_1, Inners.EIGHT EIGHT_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVENINE : percentxTHREEFⲻSEVENE
            {
                public FIVENINE(Inners.FIVE FIVE_1, Inners.NINE NINE_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEA : percentxTHREEFⲻSEVENE
            {
                public FIVEA(Inners.FIVE FIVE_1, Inners.A A_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.A_1 = A_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEB : percentxTHREEFⲻSEVENE
            {
                public FIVEB(Inners.FIVE FIVE_1, Inners.B B_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.B_1 = B_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEC : percentxTHREEFⲻSEVENE
            {
                public FIVEC(Inners.FIVE FIVE_1, Inners.C C_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.C_1 = C_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVED : percentxTHREEFⲻSEVENE
            {
                public FIVED(Inners.FIVE FIVE_1, Inners.D D_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.D_1 = D_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEE : percentxTHREEFⲻSEVENE
            {
                public FIVEE(Inners.FIVE FIVE_1, Inners.E E_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.E_1 = E_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class FIVEF : percentxTHREEFⲻSEVENE
            {
                public FIVEF(Inners.FIVE FIVE_1, Inners.F F_1)
                {
                    this.FIVE_1 = FIVE_1;
                    this.F_1 = F_1;
                }
                
                public Inners.FIVE FIVE_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXZERO : percentxTHREEFⲻSEVENE
            {
                public SIXZERO(Inners.SIX SIX_1, Inners.ZERO ZERO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXONE : percentxTHREEFⲻSEVENE
            {
                public SIXONE(Inners.SIX SIX_1, Inners.ONE ONE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTWO : percentxTHREEFⲻSEVENE
            {
                public SIXTWO(Inners.SIX SIX_1, Inners.TWO TWO_1)
                {
                    this.SIX_1 = SIX_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXTHREE : percentxTHREEFⲻSEVENE
            {
                public SIXTHREE(Inners.SIX SIX_1, Inners.THREE THREE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFOUR : percentxTHREEFⲻSEVENE
            {
                public SIXFOUR(Inners.SIX SIX_1, Inners.FOUR FOUR_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXFIVE : percentxTHREEFⲻSEVENE
            {
                public SIXFIVE(Inners.SIX SIX_1, Inners.FIVE FIVE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSIX : percentxTHREEFⲻSEVENE
            {
                public SIXSIX(Inners.SIX SIX_1, Inners.SIX SIX_2)
                {
                    this.SIX_1 = SIX_1;
                    this.SIX_2 = SIX_2;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SIX SIX_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXSEVEN : percentxTHREEFⲻSEVENE
            {
                public SIXSEVEN(Inners.SIX SIX_1, Inners.SEVEN SEVEN_1)
                {
                    this.SIX_1 = SIX_1;
                    this.SEVEN_1 = SEVEN_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.SEVEN SEVEN_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXEIGHT : percentxTHREEFⲻSEVENE
            {
                public SIXEIGHT(Inners.SIX SIX_1, Inners.EIGHT EIGHT_1)
                {
                    this.SIX_1 = SIX_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXNINE : percentxTHREEFⲻSEVENE
            {
                public SIXNINE(Inners.SIX SIX_1, Inners.NINE NINE_1)
                {
                    this.SIX_1 = SIX_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXA : percentxTHREEFⲻSEVENE
            {
                public SIXA(Inners.SIX SIX_1, Inners.A A_1)
                {
                    this.SIX_1 = SIX_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXB : percentxTHREEFⲻSEVENE
            {
                public SIXB(Inners.SIX SIX_1, Inners.B B_1)
                {
                    this.SIX_1 = SIX_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXC : percentxTHREEFⲻSEVENE
            {
                public SIXC(Inners.SIX SIX_1, Inners.C C_1)
                {
                    this.SIX_1 = SIX_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXD : percentxTHREEFⲻSEVENE
            {
                public SIXD(Inners.SIX SIX_1, Inners.D D_1)
                {
                    this.SIX_1 = SIX_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXE : percentxTHREEFⲻSEVENE
            {
                public SIXE(Inners.SIX SIX_1, Inners.E E_1)
                {
                    this.SIX_1 = SIX_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SIXF : percentxTHREEFⲻSEVENE
            {
                public SIXF(Inners.SIX SIX_1, Inners.F F_1)
                {
                    this.SIX_1 = SIX_1;
                    this.F_1 = F_1;
                }
                
                public Inners.SIX SIX_1 { get; }
                public Inners.F F_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENZERO : percentxTHREEFⲻSEVENE
            {
                public SEVENZERO(Inners.SEVEN SEVEN_1, Inners.ZERO ZERO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ZERO_1 = ZERO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ZERO ZERO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENONE : percentxTHREEFⲻSEVENE
            {
                public SEVENONE(Inners.SEVEN SEVEN_1, Inners.ONE ONE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.ONE_1 = ONE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.ONE ONE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTWO : percentxTHREEFⲻSEVENE
            {
                public SEVENTWO(Inners.SEVEN SEVEN_1, Inners.TWO TWO_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.TWO_1 = TWO_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.TWO TWO_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENTHREE : percentxTHREEFⲻSEVENE
            {
                public SEVENTHREE(Inners.SEVEN SEVEN_1, Inners.THREE THREE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.THREE_1 = THREE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.THREE THREE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFOUR : percentxTHREEFⲻSEVENE
            {
                public SEVENFOUR(Inners.SEVEN SEVEN_1, Inners.FOUR FOUR_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FOUR_1 = FOUR_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FOUR FOUR_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENFIVE : percentxTHREEFⲻSEVENE
            {
                public SEVENFIVE(Inners.SEVEN SEVEN_1, Inners.FIVE FIVE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.FIVE_1 = FIVE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.FIVE FIVE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSIX : percentxTHREEFⲻSEVENE
            {
                public SEVENSIX(Inners.SEVEN SEVEN_1, Inners.SIX SIX_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SIX_1 = SIX_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SIX SIX_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENSEVEN : percentxTHREEFⲻSEVENE
            {
                public SEVENSEVEN(Inners.SEVEN SEVEN_1, Inners.SEVEN SEVEN_2)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.SEVEN_2 = SEVEN_2;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.SEVEN SEVEN_2 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENEIGHT : percentxTHREEFⲻSEVENE
            {
                public SEVENEIGHT(Inners.SEVEN SEVEN_1, Inners.EIGHT EIGHT_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.EIGHT_1 = EIGHT_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.EIGHT EIGHT_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENNINE : percentxTHREEFⲻSEVENE
            {
                public SEVENNINE(Inners.SEVEN SEVEN_1, Inners.NINE NINE_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.NINE_1 = NINE_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.NINE NINE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENA : percentxTHREEFⲻSEVENE
            {
                public SEVENA(Inners.SEVEN SEVEN_1, Inners.A A_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.A_1 = A_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.A A_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENB : percentxTHREEFⲻSEVENE
            {
                public SEVENB(Inners.SEVEN SEVEN_1, Inners.B B_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.B_1 = B_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.B B_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENC : percentxTHREEFⲻSEVENE
            {
                public SEVENC(Inners.SEVEN SEVEN_1, Inners.C C_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.C_1 = C_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.C C_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVEND : percentxTHREEFⲻSEVENE
            {
                public SEVEND(Inners.SEVEN SEVEN_1, Inners.D D_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.D_1 = D_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.D D_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class SEVENE : percentxTHREEFⲻSEVENE
            {
                public SEVENE(Inners.SEVEN SEVEN_1, Inners.E E_1)
                {
                    this.SEVEN_1 = SEVEN_1;
                    this.E_1 = E_1;
                }
                
                public Inners.SEVEN SEVEN_1 { get; }
                public Inners.E E_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public abstract class percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE
        {
            private percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE()
            {
            }
            
            protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);
            
            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE node, TContext context)
                {
                    return node.Dispatch(this, context);
                }
                
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE.percentxTWOZEROⲻTHREED node, TContext context);
                protected internal abstract TResult Accept(percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE.percentxTHREEFⲻSEVENE node, TContext context);
            }
            
            public sealed class percentxTWOZEROⲻTHREED : percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE
            {
                public percentxTWOZEROⲻTHREED(Inners.percentxTWOZEROⲻTHREED percentxTWOZEROⲻTHREED_1)
                {
                    this.percentxTWOZEROⲻTHREED_1 = percentxTWOZEROⲻTHREED_1;
                }
                
                public Inners.percentxTWOZEROⲻTHREED percentxTWOZEROⲻTHREED_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
            
            public sealed class percentxTHREEFⲻSEVENE : percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE
            {
                public percentxTHREEFⲻSEVENE(Inners.percentxTHREEFⲻSEVENE percentxTHREEFⲻSEVENE_1)
                {
                    this.percentxTHREEFⲻSEVENE_1 = percentxTHREEFⲻSEVENE_1;
                }
                
                public Inners.percentxTHREEFⲻSEVENE percentxTHREEFⲻSEVENE_1 { get; }
                
                protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
        
        public sealed class openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ
        {
            public openpercentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENEↃ(Inners.percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE_1)
            {
                this.percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE_1 = percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE_1;
            }
            
            public Inners.percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE percentxTWOZEROⲻTHREEDⳆpercentxTHREEFⲻSEVENE_1 { get; }
        }
        
        public sealed class x3E
        {
            private x3E()
            {
            }
            
            public static x3E Instance { get; } = new x3E();
        }
        
        public sealed class doublequotex3Edoublequote
        {
            public doublequotex3Edoublequote(Inners.x3E x3E_1)
            {
                this.x3E_1 = x3E_1;
            }
            
            public Inners.x3E x3E_1 { get; }
        }
    }
    
}
