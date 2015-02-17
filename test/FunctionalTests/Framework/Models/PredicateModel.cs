//---------------------------------------------------------------------
// <copyright file="PredicateModel.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.ModuleCore;
using Microsoft.Test.KoKoMo;

namespace System.Data.Test.Astoria
{
    ////////////////////////////////////////////////////////
    //  Predicate Model
    //
    ////////////////////////////////////////////////////////   
    public class PredicateModel: ExpressionModel
    {
        protected ResourceContainer _resourceContainer;
        protected Workspace _workspace;
        protected ResourceType _resType;
        protected KeyExpression _key = null;
        protected ResourceProperty _prop;
        //Constructor
        public PredicateModel(Workspace w,ResourceContainer container, ResourceProperty p, KeyExpression parentKey, ResourceType resType)
        {
            _resourceContainer = container;
            _workspace = w;
            _resType = resType;
            _key = parentKey;
            _prop = p;
        }

        public Workspace Workspace
        {
            get { return _workspace; }
        }

        //Actions
        [ModelAction]    
        public void GetKeyExpression()
        {
            if (_resType.Name.Contains("Order_Details_Extended") || _resourceContainer.Name.Contains("Order_Details_Extended"))
            {
                this.Result = null;
                return;
            }

            if (_resType.Name != _resourceContainer.BaseType.Name)
            {
                KeyExpressions keyExp2 = _resourceContainer.Workspace.GetExistingAssociatedKeys(_resourceContainer, _prop, _key);
                if (null == keyExp2 || keyExp2.Count == 0) { /* no keys for resource type*/ return; }
                foreach (KeyExpression ke in keyExp2)
                {
                    bool bSpecial = SpecialChars(ke);
                    if (bSpecial)
                    {
                        this.Result = null;
                        break;
                    }
                    else
                    {
                        this.Result = ke;
                        AstoriaTestLog.WriteLineIgnore("Predicate from container :" + _resType.Name);
                        break;
                    }
                }
            }
            else if (_resourceContainer.Name == _resourceContainer.BaseType.Name)
            {
              
                KeyExpression keyExp = this.Workspace.GetRandomExistingKey(_resourceContainer);
                bool bSpecial = SpecialChars(keyExp);
                if (bSpecial)
                { this.Result = null; }
                else
                {
                    this.Result = keyExp;
                    AstoriaTestLog.WriteLineIgnore("Predicate from container :" + _resourceContainer.Name);
                }
            }
            else
                this.Result = null;

           
            this.Disabled = true;
        }

        public override ExpressionModel CreateModel()
        {
            return new PredicateModel(this.Workspace,this._resourceContainer,_prop,_key,_resType);
        }

        public static bool SpecialChars(KeyExpression keyExp)
        {
            // Workaround unsupported .:'/ in URI
            bool bSpecial = false;
            if (keyExp != null)
            {
                foreach (NodeValue key in keyExp.Values)
                {
                    string x = key.ClrValue.ToString();
                    if (0 <= x.IndexOfAny(CharsBadRequest) || (key.ClrValue is DateTime))
                    {
                        bSpecial = true;
                        break;
                    }
                }
                return bSpecial;
            }
            else
                return true;
        }

        private static readonly char[] CharsBadRequest = new char[] { ':', '\'', '/', '.' };

        public static string UrlEncodeString(string original)
        {
            string result = null;
            foreach (char c in original)
            {
                //if (c == ' ')
                //result += "%20";
                //else

                if (c == '<')
                    result += "%3C";
                else if (c == '>')
                    result += "%3E";
                else if (c == '#')
                    result += "%23";
                else if (c == '%')
                    result += "%25";
                else if (c == '{')
                    result += "%7B";
                else if (c == '}')
                    result += "%7D";
                else if (c == '|')
                    result += "%7C";
                // else if (c == '\\')
                //result += "%5C";
                else if (c == '^')
                    result += "%5E";
                else if (c == '~')
                    result += "%7E";
                else if (c == '[')
                    result += "%5B:";
                else if (c == ']')
                    result += "%5D";
                else if (c == '\'')
                    result += "''";
                else if (c == ';')
                    result += "%3B";
                //else if (c == '/')
                // result += "%2F";
                else if (c == '?')
                    result += "%3F";
                //else if (c == ':')
                // result += "%3A";
                else if (c == '@')
                    result += "%40";
                else if (c == '=')
                    result += "%3D";
                else if (c == '&')
                    result += "%26";
                else if (c == '$')
                    result += "%24";
                else if (c == '+')
                    result += "%2B";
                else
                    result += c;
            }
            if (original != null && original.Length == 0)
                result = "";
            return result;
        }

    }
}
