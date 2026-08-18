//---------------------------------------------------------------------
// <copyright file="ODataUriFunctionsParsingTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.UriParser
{
    /// <summary>
    /// Unit tests for Uri functions parsing tests.
    /// </summary>
    public class ODataUriFunctionsParsingTests : IDisposable
    {
        private const string CalculateBenefitFunctionName = "calculateBenefit";
        private const string CalculateTaxablePayFunctionName = "calculateTaxablePay";
        private const string CalculateAdjustedSalaryFunctionName = "calculateAdjustedSalary";
        private const string CalculateSalaryBandFunctionName = "calculateSalaryBand";

        private EdmModel model;

        public ODataUriFunctionsParsingTests()
        {
            InitializeModel();

            // Function with single-valued parameter
            CustomUriFunctions.AddCustomUriFunction(
                CalculateBenefitFunctionName,
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetDecimal(false)));
            // Function with collection-valued parameter
            CustomUriFunctions.AddCustomUriFunction(
                CalculateTaxablePayFunctionName,
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false))));
            // Function with single-valued literal parameter
            CustomUriFunctions.AddCustomUriFunction(
                CalculateAdjustedSalaryFunctionName,
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetDecimal(false)));
            // Function with collection-valued literal parameter
            CustomUriFunctions.AddCustomUriFunction(
                CalculateSalaryBandFunctionName,
                new FunctionSignatureWithReturnType(
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.Instance.GetDecimal(false),
                    EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false))));
        }

        public void Dispose()
        {
            CustomUriFunctions.RemoveCustomUriFunction(CalculateBenefitFunctionName);
            CustomUriFunctions.RemoveCustomUriFunction(CalculateTaxablePayFunctionName);
            CustomUriFunctions.RemoveCustomUriFunction(CalculateAdjustedSalaryFunctionName);
            CustomUriFunctions.RemoveCustomUriFunction(CalculateSalaryBandFunctionName);
        }

        [Fact]
        public void ParseUri_WithEdmFunctionInCompute_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees(1)?$compute=Default.CalculateBonus(salary=Salary)%20as%20Bonus&$select=Name,Salary,Bonus"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var bonusItem = computedItems[0];
            Assert.Equal("Bonus", bonusItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(bonusItem.Expression);
            Assert.Equal("Default.CalculateBonus", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Single(parameters);
            var salaryParameter = Assert.IsType<NamedFunctionParameterNode>(parameters[0]);
            var salaryParameterValue = Assert.IsType<SingleValuePropertyAccessNode>(salaryParameter.Value);
            Assert.Equal("Salary", salaryParameterValue.Property.Name);
            Assert.NotNull(parsedResult.SelectAndExpand);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Salary", selectedProps);
            Assert.Contains("Bonus", selectedProps);
        }

        [Fact]
        public void ParseUri_WithEdmFunctionInCompute_CollectionValuedParameter_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees(1)?$compute=Default.CalculateOvertimePay(salary=Salary,overtimeHours=OvertimeHours)%20as%20OvertimePay&$select=Name,Salary,OvertimePay"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var overtimePayItem = computedItems[0];
            Assert.Equal("OvertimePay", overtimePayItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(overtimePayItem.Expression);
            Assert.Equal("Default.CalculateOvertimePay", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Equal(2, parameters.Count);
            var salaryParameter = Assert.IsType<NamedFunctionParameterNode>(parameters[0]);
            var overtimeHoursParameter = Assert.IsType<NamedFunctionParameterNode>(parameters[1]);
            var salaryParameterValue = Assert.IsType<SingleValuePropertyAccessNode>(salaryParameter.Value);
            var overtimeHoursParameterValue = Assert.IsType<CollectionPropertyAccessNode>(overtimeHoursParameter.Value);
            Assert.Equal("Salary", salaryParameterValue.Property.Name);
            Assert.Equal("OvertimeHours", overtimeHoursParameterValue.Property.Name);
            Assert.NotNull(parsedResult.SelectAndExpand);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Salary", selectedProps);
            Assert.Contains("OvertimePay", selectedProps);
        }

        [Fact]
        public void ParseUri_WithEdmFunctionInCompute_SingleValuedLiteral_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees(1)?$compute=Default.GetScore(maxScore=100)%20as%20Score&$select=Name,Score"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var scoreItem = computedItems[0];
            Assert.Equal("Score", scoreItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(scoreItem.Expression);
            Assert.Equal("Default.GetScore", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Single(parameters);
            var maxScoreParameter = Assert.IsType<NamedFunctionParameterNode>(parameters[0]);
            var maxScoreLiteral = Assert.IsType<ConstantNode>(maxScoreParameter.Value);
            Assert.Equal(100, maxScoreLiteral.Value);
            Assert.NotNull(parsedResult.SelectAndExpand);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Score", selectedProps);
        }

        [Fact]
        public void ParseUri_WithEdmFunctionInCompute_CollectionValuedLiteral_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees(1)?$compute=Default.GetRating(scale=[1,2,3,4,5])%20as%20Rating&$select=Name,Rating"));
            
            // Act
            var parsedResult = odataUriParser.ParseUri();
            
            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var ratingItem = computedItems[0];
            Assert.Equal("Rating", ratingItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(ratingItem.Expression);
            Assert.Equal("Default.GetRating", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Single(parameters);
            var scaleParameter = Assert.IsType<NamedFunctionParameterNode>(parameters[0]);
            var scaleLiteral = Assert.IsType<ConstantNode>(scaleParameter.Value);
            Assert.Equal("[1,2,3,4,5]", scaleLiteral.LiteralText);
            Assert.NotNull(parsedResult.SelectAndExpand);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Rating", selectedProps);
        }

        [Fact]
        public void ParseUri_WithCustomFunctionInCompute_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees?$compute=calculateBenefit(Salary)%20as%20Benefit&$select=Name,Salary,Benefit"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var benefitItem = computedItems[0];
            Assert.Equal("Benefit", benefitItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(benefitItem.Expression);
            Assert.Equal("calculateBenefit", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Single(parameters);
            var salaryParameter = Assert.IsType<SingleValuePropertyAccessNode>(parameters[0]);
            Assert.Equal("Salary", salaryParameter.Property.Name);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Salary", selectedProps);
            Assert.Contains("Benefit", selectedProps);
        }

        [Fact]
        public void ParseUri_WithCustomFunctionInCompute_CollectionValuedParameter_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees?$compute=calculateTaxablePay(Salary,OvertimeHours)%20as%20TaxablePay&$select=Name,Salary,TaxablePay"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var taxablePayItem = computedItems[0];
            Assert.Equal("TaxablePay", taxablePayItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(taxablePayItem.Expression);
            Assert.Equal("calculateTaxablePay", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Equal(2, parameters.Count);
            var salaryParameter = Assert.IsType<SingleValuePropertyAccessNode>(parameters[0]);
            Assert.Equal("Salary", salaryParameter.Property.Name);
            var overtimeHoursParameter = Assert.IsType<CollectionPropertyAccessNode>(parameters[1]);
            Assert.Equal("OvertimeHours", overtimeHoursParameter.Property.Name);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("Salary", selectedProps);
            Assert.Contains("TaxablePay", selectedProps);
        }

        [Fact]
        public void ParseUri_WithCustomFunctionInCompute_SingleValuedLiteral_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees?$compute=calculateAdjustedSalary(Salary,1%2E5)%20as%20AdjustedSalary&$select=Name,AdjustedSalary"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var adjustedSalaryItem = computedItems[0];
            Assert.Equal("AdjustedSalary", adjustedSalaryItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(adjustedSalaryItem.Expression);
            Assert.Equal("calculateAdjustedSalary", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Equal(2, parameters.Count);
            var salaryParameter = Assert.IsType<SingleValuePropertyAccessNode>(parameters[0]);
            Assert.Equal("Salary", salaryParameter.Property.Name);
            var literalParameter = Assert.IsType<ConstantNode>(parameters[1]);
            Assert.Equal(1.5m, literalParameter.Value);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("AdjustedSalary", selectedProps);
        }

        [Fact]
        public void ParseUri_WithCustomFunctionInCompute_CollectionValuedLiteral_ParsesSelectAndComputeSuccessfully()
        {
            // Arrange
            var odataUriParser = new ODataUriParser(
                this.model,
                new Uri("http://tempuri.org"),
                new Uri($"http://tempuri.org/Employees?$compute=calculateSalaryBand(Salary,[5000,7000,9000])%20as%20SalaryBand&$select=Name,SalaryBand"));

            // Act
            var parsedResult = odataUriParser.ParseUri();

            // Assert
            Assert.NotNull(parsedResult);
            Assert.NotNull(parsedResult.Compute);
            var computedItems = parsedResult.Compute.ComputedItems.ToList();
            Assert.Single(computedItems);
            var salaryBandItem = computedItems[0];
            Assert.Equal("SalaryBand", salaryBandItem.Alias);
            var funcCall = Assert.IsType<SingleValueFunctionCallNode>(salaryBandItem.Expression);
            Assert.Equal("calculateSalaryBand", funcCall.Name);
            var parameters = funcCall.Parameters.ToList();
            Assert.Equal(2, parameters.Count);
            var salaryParameter = Assert.IsType<SingleValuePropertyAccessNode>(parameters[0]);
            Assert.Equal("Salary", salaryParameter.Property.Name);
            var literalParameter = Assert.IsType<CollectionConstantNode>(parameters[1]);
            var bandValues = literalParameter.Collection.OfType<ConstantNode>().Select(n => (decimal)n.Value).ToList();
            Assert.Equal(new decimal[] { 5000m, 7000m, 9000m }, bandValues);
            var selectedProps = parsedResult.SelectAndExpand.SelectedItems.OfType<PathSelectItem>().Select(i => i.SelectedPath.LastSegment.Identifier).ToList();
            Assert.Contains("Name", selectedProps);
            Assert.Contains("SalaryBand", selectedProps);
        }

        private void InitializeModel()
        {
            this.model = new EdmModel();
            var employeeEntityType = this.model.AddEntityType("NS", "Employee");
            var idProperty = employeeEntityType.AddStructuralProperty("ID", EdmCoreModel.Instance.GetInt32(false));
            employeeEntityType.AddKeys(idProperty);
            employeeEntityType.AddStructuralProperty("Name", EdmCoreModel.Instance.GetString(false));
            employeeEntityType.AddStructuralProperty("Salary", EdmCoreModel.Instance.GetDecimal(false));
            employeeEntityType.AddStructuralProperty("OvertimeHours", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false)));
            this.model.AddElement(employeeEntityType);

            // Function with single-valued parameter
            var calculateBonusEdmFunction = new EdmFunction(
                "Default",
                "CalculateBonus",
                EdmCoreModel.Instance.GetDecimal(false),
                true,
                null,
                false);
            calculateBonusEdmFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(employeeEntityType, false));
            calculateBonusEdmFunction.AddParameter("salary", EdmCoreModel.Instance.GetDecimal(false));
            this.model.AddElement(calculateBonusEdmFunction);

            // Function with collection-valued parameter
            var calculateOvertimePayEdmFunction = new EdmFunction(
                "Default",
                "CalculateOvertimePay",
                EdmCoreModel.Instance.GetDecimal(false),
                true,
                null,
                false);
            calculateOvertimePayEdmFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(employeeEntityType, false));
            calculateOvertimePayEdmFunction.AddParameter("salary", EdmCoreModel.Instance.GetDecimal(false));
            calculateOvertimePayEdmFunction.AddParameter("overtimeHours", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetDecimal(false)));
            this.model.AddElement(calculateOvertimePayEdmFunction);

            // Function with single-valued literal parameter
            var getScoreEdmFunction = new EdmFunction(
                "Default",
                "GetScore",
                EdmCoreModel.Instance.GetInt32(false),
                true,
                null,
                false);
            getScoreEdmFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(employeeEntityType, false));
            getScoreEdmFunction.AddParameter("maxScore", EdmCoreModel.Instance.GetInt32(false));
            this.model.AddElement(getScoreEdmFunction);

            // Function with collection-valued literal parameter
            var getRatingsEdmFunction = new EdmFunction(
                "Default",
                "GetRating",
                EdmCoreModel.Instance.GetInt32(false),
                true,
                null,
                false);
            getRatingsEdmFunction.AddParameter("bindingParameter", new EdmEntityTypeReference(employeeEntityType, false));
            getRatingsEdmFunction.AddParameter("scale", EdmCoreModel.GetCollection(EdmCoreModel.Instance.GetInt32(false)));
            this.model.AddElement(getRatingsEdmFunction);

            var container = this.model.AddEntityContainer("Default", "Container");
            container.AddEntitySet("Employees", employeeEntityType);
            container.AddFunctionImport("CalculateBonus", calculateBonusEdmFunction);
            container.AddFunctionImport("CalculateOvertimePay", calculateOvertimePayEdmFunction);
        }
    }
}
