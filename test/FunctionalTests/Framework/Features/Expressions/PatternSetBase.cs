//---------------------------------------------------------------------
// <copyright file="PatternSetBase.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Data.Test.Astoria.Expressions
{
    //Enum to define the different query components the Astoria service supports
    //Each of the value in this enum should have a corresponding verification class defined below
    //NOTE: Do not remove or add any item after the LastEnumElement.
    public enum QueryComponent
    {
        Top = 0,
        Skip,
        OrderBy,
        SelectSingleEntity,
        SimpleProjection
    };

    //Factory to identify and generate the correct verification class that matches the QueryComponent
    //This is a dictionary of verification classes.
    public abstract class AbstractPatternSet
    {
        public abstract AbstractExprTreePattern GetPattern(QueryComponent queryComponent);
    }

    //Implementation of factory for the dictionary that returns the exact Pattern verification class for a given QueryComponent
    public class ConcretePatternDictionary : AbstractPatternSet
    {
        public override AbstractExprTreePattern GetPattern(QueryComponent queryComponent)
        {
            switch (queryComponent)
            {
                case QueryComponent.Top: //top
                    {
                        return new PatternTopSkipOrderBy();
                    }
                case QueryComponent.Skip: //skip
                    {
                        return new PatternTopSkipOrderBy();
                    }
                case QueryComponent.OrderBy: //OrderBy
                    {
                        return new PatternTopSkipOrderBy();
                    }
                case QueryComponent.SelectSingleEntity: //select single entity
                    {
                        return new PatternSelectSingleEntity();
                    }
                case QueryComponent.SimpleProjection: //Simple projection
                    {
                        return new PatternSimpleProjection();
                    }
                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }

    //Abstract class from which every QueryComponent verification class derive from
    public abstract class AbstractExprTreePattern
    {
        public virtual bool Compare(QueryTreeInfo queryTreeInfo)
        {
            bool result = false;
            result = VerifyNodes(queryTreeInfo);
            result = VerifyMetaData(queryTreeInfo);
            result = VerifyLambda(queryTreeInfo);
            return result;
        }

        public abstract void Build(QueryTreeInfo input);

        //Compare will in turn call these individual comparison functions
        //Verify the structure of the tree
        protected abstract bool VerifyNodes(QueryTreeInfo queryTreeInfo);

        //Verify the meta data info in the nodes
        protected abstract bool VerifyMetaData(QueryTreeInfo queryTreeInfo);

        //Verify the lambda expressions where applicable - in cases like select node
        protected abstract bool VerifyLambda(QueryTreeInfo queryTreeInfo);
    }
}
