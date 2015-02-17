//---------------------------------------------------------------------
// <copyright file="PhpCodeGenerator.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.Taupo.Astoria.Php
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Microsoft.Test.Taupo.Astoria.Contracts;
    using Microsoft.Test.Taupo.Astoria.Contracts.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.Contracts.OData;
    using Microsoft.Test.Taupo.Astoria.LinqToAstoria;
    using Microsoft.Test.Taupo.Astoria.OData;
    using Microsoft.Test.Taupo.Common;
    using Microsoft.Test.Taupo.Contracts;
    using Microsoft.Test.Taupo.Contracts.Types;
    using Microsoft.Test.Taupo.Query.Common;
    using Microsoft.Test.Taupo.Query.Contracts;
    using Microsoft.Test.Taupo.Query.Contracts.CommonExpressions;
    using Microsoft.Test.Taupo.Query.Contracts.Linq.Expressions;

    /// <summary>
    /// Code Generator for PHP
    /// </summary>
    [ImplementationName(typeof(INonClrCodeGenerator), "Php")]
    public class PhpCodeGenerator : INonClrCodeGenerator
    {
        private PhpQueryOptions options; 
        private StringWriter phpCode;

        /// <summary>
        /// Gets or sets the service descriptor
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public IAstoriaServiceDescriptor Service { get; set; }

        /// <summary>
        /// Gets or sets the converter to use when getting PHP options for a query
        /// </summary>
        [InjectDependency(IsRequired = true)]
        public QueryExpressionToPhpOptionsConverter ExpressionConverter { get; set; }
        
        /// <summary>
        /// Method to generate the PHP code
        /// </summary>
        /// <param name="expression">Expression from which PHP code is generated</param>
        /// <returns>The generated PHP code as string</returns>
        public string GenerateCode(QueryExpression expression)
        {
            ExceptionUtilities.CheckAllRequiredDependencies(this);

            this.options = this.ExpressionConverter.Convert(expression);

            using (this.phpCode = new StringWriter(CultureInfo.InvariantCulture))
            {
                this.AddInitialTags();
                this.WrapInTryBlock(() =>
                {
                    this.GenerateDataServiceClassObject();
                    this.GenerateCodeFromQueryExpression();
                    this.AddClosingTags();
                });
                this.AddCatchBlock();
                this.phpCode.WriteLine(PhpConstants.ClosingPhpTag);
                return this.phpCode.ToString();
            }
        }

        private void WrapInTryBlock(Action executeAction)
        {
            this.phpCode.WriteLine("try");
            this.phpCode.WriteLine("{");
            executeAction();
            this.phpCode.WriteLine("}");
        }

        private void AddCatchBlock()
        {
            this.phpCode.WriteLine("catch (Exception $e) {");
            this.phpCode.WriteLine("echo 'Caught exception: ',  print_r($e);");
            this.phpCode.WriteLine("echo('Failed');");
            this.phpCode.WriteLine("}");
        }

        private void AddInitialTags()
        {
            this.phpCode.WriteLine(PhpConstants.OpeningPhpTag);
        }

        private void AddClosingTags()
        {
            this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "$statuscode = {0}->getStatusCode();", PhpConstants.DataServiceResponseObject));
            this.phpCode.WriteLine("if($statuscode == 200) { echo('Passed'); } ");
            this.phpCode.WriteLine("else { echo('Failed'); }");
        }

        private void GenerateCodeFromQueryExpression()
        {
            this.AddQueryOptions();
        }

        private void GenerateDataServiceClassObject()
        {
            this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}'{1}.php';", PhpConstants.IncludeDataServiceClassFile, this.options.EntityContainer));
            this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} = new {1}('{2}');", PhpConstants.DataServiceContainerObject, this.options.EntityContainer, this.Service.ServiceUri.OriginalString));
        }

        private void AddQueryOptions()
        {
            if (this.options.PrimaryKey != null)
            {
                this.AddPrimaryKeyOption();
            }
            else
            {
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} = {1}->{2}();", PhpConstants.DataServiceQueryObject, PhpConstants.DataServiceContainerObject, this.options.EntitySet));
                this.AddFilterOption();
                this.AddSelectOption();
                this.AddExpandOption();
                this.AddOrderByOption();
                this.AddSkipOption();
                this.AddTopOption();
                this.AddInlineCountOption();
                if (this.options.Count)
                {
                    this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} = {1}->{2}();", PhpConstants.DataServiceResponseObject, PhpConstants.DataServiceQueryObject, PhpConstants.CountOption));
                }
                else
                {
                    this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} = {1}->{2}();", PhpConstants.DataServiceResponseObject, PhpConstants.DataServiceQueryObject, PhpConstants.Execute));
                }
            }
        }

        private void AddPrimaryKeyOption()
        {
            this.phpCode.WriteLine(
                "{0} = {1}->{2}(\"{3}{4}\");",
                PhpConstants.DataServiceResponseObject, 
                PhpConstants.DataServiceContainerObject,
                PhpConstants.Execute,  
                this.options.EntitySet, 
                this.options.PrimaryKey);
        }

        private void AddFilterOption()
        {
            if (this.options.Filter != null)
            {
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}->{1}(\"{2}\");", PhpConstants.DataServiceQueryObject, PhpConstants.FilterOption, this.options.Filter));
            }
        }

        private void AddExpandOption()
        {
            if (this.options.Expand != null)
            {
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}->{1}('{2}');", PhpConstants.DataServiceQueryObject, PhpConstants.ExpandOption, this.options.Expand));
            }
        }

        private void AddOrderByOption()
        {
            if (this.options.OrderBy != null)
            {
                this.phpCode.Write(string.Format(CultureInfo.InvariantCulture, "{0}->", PhpConstants.DataServiceQueryObject));
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}('{1}');", PhpConstants.OrderByOption, this.options.OrderBy));
            }
        }

        private void AddSelectOption()
        {
            if (this.options.Select != null)
            {
                this.phpCode.Write(string.Format(CultureInfo.InvariantCulture, "{0}->", PhpConstants.DataServiceQueryObject));
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}('{1}');", PhpConstants.SelectOption, this.options.Select));
            }
        }

        private void AddSkipOption()
        {
            if (this.options.Skip != null)
            {
                this.phpCode.Write(string.Format(CultureInfo.InvariantCulture, "{0}->", PhpConstants.DataServiceQueryObject));
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}('{1}');", PhpConstants.SkipOption, this.options.Skip));
            }
        }

        private void AddTopOption()
        {
            if (this.options.Top != null)
            {
                this.phpCode.Write(string.Format(CultureInfo.InvariantCulture, "{0}->", PhpConstants.DataServiceQueryObject));
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}('{1}');", PhpConstants.TopOption, this.options.Top));
            }
        }

        private void AddInlineCountOption()
        {
            if (this.options.InlineCount)
            {
                this.phpCode.Write(string.Format(CultureInfo.InvariantCulture, "{0}->", PhpConstants.DataServiceQueryObject));
                this.phpCode.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}();", PhpConstants.InlineCountOption));
            }
        }
    }
}
