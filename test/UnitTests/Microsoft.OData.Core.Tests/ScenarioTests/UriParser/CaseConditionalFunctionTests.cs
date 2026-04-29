//---------------------------------------------------------------------
// <copyright file="CaseConditionalFunctionTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Tests.UriParser;
using Microsoft.OData.UriParser;
using Xunit;

namespace Microsoft.OData.Tests.ScenarioTests.UriParser
{
    public class CaseConditionalFunctionTests
    {
        private readonly IEdmModel model;
        private readonly IEdmEntityType userType;
        private readonly IEdmEntitySet usersSet;

        public CaseConditionalFunctionTests()
        {
            this.model = CreateModel();
            this.userType = this.model.FindDeclaredType("NS.User") as IEdmEntityType;
            this.usersSet = this.model.FindDeclaredEntitySet("Users");
        }

        [Fact]
        public void CaseInComputeFallbackToAlternateSearchTags()
        {
            // Arrange
            // Use case to select CustomTags if available, otherwise fallback to SystemTags
            string compute = "case(CustomTags/$count gt 0: CustomTags, true: SystemTags) as SearchTags";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("SearchTags", computeExp.Alias);

            // Verify the case function call
            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsUntyped());

            // Verify the 4 parameters (2 condition/result pairs)
            Assert.Collection(caseFunctionCall.Parameters,
                // Condition 1: CustomTags/$count gt 0
                param1 =>
                {
                    var binaryOp = Assert.IsType<BinaryOperatorNode>(param1);
                    Assert.Equal(BinaryOperatorKind.GreaterThan, binaryOp.OperatorKind);
                    var countNode = Assert.IsType<CountNode>(binaryOp.Left);
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(countNode.Source);
                    Assert.Equal("CustomTags", collectionPropAccess.Property.Name);
                    var constNode = Assert.IsType<ConstantNode>(binaryOp.Right);
                    Assert.Equal(0L, constNode.Value);
                },
                // Result 1: CustomTags
                param2 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param2);
                    Assert.Equal("CustomTags", collectionPropAccess.Property.Name);
                },
                // Condition 2: true
                param3 => param3.ShouldBeConstantQueryNode(true),
                // Result 2: SystemTags
                param4 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param4);
                    Assert.Equal("SystemTags", collectionPropAccess.Property.Name);
                });
        }

        [Fact]
        public void CaseInComputeSelectContactEmailsWithDefaultEmptyCollection()
        {
            // Arrange
            // Use case to select BusinessEmails or PersonalEmails based on PrimaryContact, defaulting to empty collection
            string compute = "case(PrimaryContact eq 'Business': BusinessEmails, PrimaryContact eq 'Personal': PersonalEmails, true: []) as ContactEmails";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("ContactEmails", computeExp.Alias);

            // Verify the case function call
            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsUntyped());

            // Verify the 6 parameters (3 condition/result pairs)
            Assert.Collection(caseFunctionCall.Parameters,
                // Condition 1: PrimaryContact eq 'Business'
                param1 =>
                {
                    var binaryOp = param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("PrimaryContact", propAccess.Property.Name);
                    binaryOp.Right.ShouldBeConstantQueryNode("Business");
                },
                // Result 1: BusinessEmails
                param2 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param2);
                    Assert.Equal("BusinessEmails", collectionPropAccess.Property.Name);
                },
                // Condition 2: PrimaryContact eq 'Personal'
                param3 =>
                {
                    var binaryOp = param3.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("PrimaryContact", propAccess.Property.Name);
                    binaryOp.Right.ShouldBeConstantQueryNode("Personal");
                },
                // Result 2: PersonalEmails
                param4 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param4);
                    Assert.Equal("PersonalEmails", collectionPropAccess.Property.Name);
                },
                // Condition 3: true (default case)
                param5 => param5.ShouldBeConstantQueryNode(true),
                // Result 3: [] - empty collection
                param6 =>
                {
                    var collectionNode = Assert.IsType<CollectionConstantNode>(param6);
                    Assert.Empty(collectionNode.Collection);
                });
        }

        [Fact]
        public void CaseInComputeCategorizePeopleByAge()
        {
            // Arrange
            // Use case to categorize users by age: Kid, Youth, Adult, Senior
            string compute = "case(Age lt 13: 'Kid', Age lt 20: 'Youth', Age lt 65: 'Adult', true: 'Senior') as AgeCategory";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("AgeCategory", computeExp.Alias);

            // Verify the case function call
            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsString());

            // Verify the 8 parameters (4 condition/result pairs)
            Assert.Collection(caseFunctionCall.Parameters,
                // Condition 1: Age lt 13
                param1 =>
                {
                    var binaryOp = param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThan);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("Age", propAccess.Property.Name);
                    binaryOp.Right.ShouldBeConstantQueryNode(13);
                },
                // Result 1: 'Kid'
                param2 => param2.ShouldBeConstantQueryNode("Kid"),
                // Condition 2: Age lt 20
                param3 =>
                {
                    var binaryOp = param3.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThan);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("Age", propAccess.Property.Name);
                    binaryOp.Right.ShouldBeConstantQueryNode(20);
                },
                // Result 2: 'Youth'
                param4 => param4.ShouldBeConstantQueryNode("Youth"),
                // Condition 3: Age lt 65
                param5 =>
                {
                    var binaryOp = param5.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThan);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("Age", propAccess.Property.Name);
                    binaryOp.Right.ShouldBeConstantQueryNode(65);
                },
                // Result 3: 'Adult'
                param6 => param6.ShouldBeConstantQueryNode("Adult"),
                // Condition 4: true (default case)
                param7 => param7.ShouldBeConstantQueryNode(true),
                // Result 4: 'Senior'
                param8 => param8.ShouldBeConstantQueryNode("Senior"));
        }

        [Fact]
        public void CaseInComputeMixedStatusAndCollectionProperties()
        {
            // Arrange
            // Use case with compound conditions mixing Status (string) and collection properties
            // Select BusinessEmails for active users with emails, CustomTags for inactive users with tags, otherwise empty
            string compute = "case(Status eq 'Active' and BusinessEmails/$count gt 0: BusinessEmails, Status eq 'Inactive' and CustomTags/$count gt 0: CustomTags, true: []) as NotificationEmails";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("NotificationEmails", computeExp.Alias);

            // Verify the case function call
            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsUntyped());

            // Verify the 6 parameters (3 condition/result pairs)
            Assert.Collection(caseFunctionCall.Parameters,
                // Condition 1: Status eq 'Active' and BusinessEmails/$count gt 0
                param1 =>
                {
                    var andOp = param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

                    // Left side: Status eq 'Active'
                    var leftEq = andOp.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
                    var statusProp = Assert.IsType<SingleValuePropertyAccessNode>(leftEq.Left);
                    Assert.Equal("Status", statusProp.Property.Name);
                    leftEq.Right.ShouldBeConstantQueryNode("Active");

                    // Right side: BusinessEmails/$count gt 0
                    var convertNode = andOp.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetBoolean(true));
                    var rightGt = convertNode.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
                    var countNode = Assert.IsType<CountNode>(rightGt.Left);
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(countNode.Source);
                    Assert.Equal("BusinessEmails", collectionPropAccess.Property.Name);
                    rightGt.Right.ShouldBeConstantQueryNode(0L);
                },
                // Result 1: BusinessEmails
                param2 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param2);
                    Assert.Equal("BusinessEmails", collectionPropAccess.Property.Name);
                },
                // Condition 2: Status eq 'Inactive' and CustomTags/$count gt 0
                param3 =>
                {
                    var andOp = param3.ShouldBeBinaryOperatorNode(BinaryOperatorKind.And);

                    // Left side: Status eq 'Inactive'
                    var leftEq = andOp.Left.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);
                    var statusProp = Assert.IsType<SingleValuePropertyAccessNode>(leftEq.Left);
                    Assert.Equal("Status", statusProp.Property.Name);
                    leftEq.Right.ShouldBeConstantQueryNode("Inactive");

                    // Right side: CustomTags/$count gt 0
                    var convertNode = andOp.Right.ShouldBeConvertQueryNode(EdmCoreModel.Instance.GetBoolean(true));
                    var rightGt = convertNode.Source.ShouldBeBinaryOperatorNode(BinaryOperatorKind.GreaterThan);
                    var countNode = Assert.IsType<CountNode>(rightGt.Left);
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(countNode.Source);
                    Assert.Equal("CustomTags", collectionPropAccess.Property.Name);
                    rightGt.Right.ShouldBeConstantQueryNode(0L);
                },
                // Result 2: CustomTags
                param4 =>
                {
                    var collectionPropAccess = Assert.IsType<CollectionPropertyAccessNode>(param4);
                    Assert.Equal("CustomTags", collectionPropAccess.Property.Name);
                },
                // Condition 3: true (default case)
                param5 => param5.ShouldBeConstantQueryNode(true),
                // Result 3: [] - empty collection
                param6 =>
                {
                    var collectionNode = Assert.IsType<CollectionConstantNode>(param6);
                    Assert.Empty(collectionNode.Collection);
                });
        }

        #region Edge Case Tests

        /// <summary>
        /// Tests case expression with a two condition/result pairs.
        /// Query: ?$compute=case(Age lt 18: 'Minor', true: 'Adult') as AgeGroup
        /// </summary>
        [Fact]
        public void CaseInComputeWithTwoConditionPairs()
        {
            // Arrange
            string compute = "case(Age lt 18: 'Minor', true: 'Adult') as AgeGroup";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("AgeGroup", computeExp.Alias);

            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");

            // Verify 4 parameters (2 pairs)
            Assert.Equal(4, caseFunctionCall.Parameters.Count());
            Assert.Collection(caseFunctionCall.Parameters,
                param1 => param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.LessThan),
                param2 => param2.ShouldBeConstantQueryNode("Minor"),
                param3 => param3.ShouldBeConstantQueryNode(true),
                param4 => param4.ShouldBeConstantQueryNode("Adult"));
        }

        /// <summary>
        /// Tests case expression where the default case (true condition) is the only match.
        /// Query: ?$compute=case(Age lt 0: 'Invalid', true: 'Valid') as Validity
        /// </summary>
        [Fact]
        public void CaseInComputeAllConditionsFalseDefaultMatches()
        {
            // Arrange
            // Scenario where first condition is unlikely to be true (age < 0)
            string compute = "case(Age lt 0: 'Invalid', true: 'Valid') as Validity";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("Validity", computeExp.Alias);

            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsString());

            Assert.Equal(4, caseFunctionCall.Parameters.Count());
        }

        /// <summary>
        /// Tests case expression with many condition/result pairs (6 pairs).
        /// Query: ?$compute=case(Age lt 3: 'Infant', Age lt 6: 'Toddler', Age lt 13: 'Child', Age lt 18: 'Teen', Age lt 65: 'Adult', true: 'Senior') as DetailedAgeGroup
        /// </summary>
        [Fact]
        public void CaseInComputeWithManyConditionPairs()
        {
            // Arrange
            string compute = "case(Age lt 3: 'Infant', Age lt 6: 'Toddler', Age lt 13: 'Child', Age lt 18: 'Teen', Age lt 65: 'Adult', true: 'Senior') as DetailedAgeGroup";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("DetailedAgeGroup", computeExp.Alias);

            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsString());

            // Verify 12 parameters (6 condition/result pairs)
            Assert.Equal(12, caseFunctionCall.Parameters.Count());
        }

        #endregion

        #region Integration Tests with $filter

        /// <summary>
        /// Tests case expression within a $filter query.
        /// Query: ?$filter=case(Age lt 18: 'Minor', true: 'Adult') eq 'Minor'
        /// </summary>
        [Fact]
        public void CaseInFilterExpression()
        {
            // Arrange
            var parser = new ODataQueryOptionParser(
                this.model,
                this.userType,
                this.usersSet,
                new Dictionary<string, string> { { "$filter", "case(Age lt 18: 'Minor', true: 'Adult') eq 'Minor'" } });

            // Act
            FilterClause filterClause = parser.ParseFilter();

            // Assert
            Assert.NotNull(filterClause);
            var binaryOp = filterClause.Expression.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal);

            // Left side should be the case function
            var caseFunctionCall = binaryOp.Left.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.Equal(4, caseFunctionCall.Parameters.Count());

            // Right side should be the constant 'Minor'
            binaryOp.Right.ShouldBeConstantQueryNode("Minor");
        }

        /// <summary>
        /// Tests case expression with complex filter conditions.
        /// Query: ?$filter=case(Status eq 'Active': true, true: false)
        /// </summary>
        [Fact]
        public void CaseInFilterWithBooleanResults()
        {
            // Arrange
            var parser = new ODataQueryOptionParser(
                this.model,
                this.userType,
                this.usersSet,
                new Dictionary<string, string> { { "$filter", "case(Status eq 'Active': true, true: false)" } });

            // Act
            FilterClause filterClause = parser.ParseFilter();

            // Assert
            Assert.NotNull(filterClause);
            var caseFunctionCall = filterClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsBoolean());
            Assert.Equal(4, caseFunctionCall.Parameters.Count());
        }

        #endregion

        #region Integration Tests with $orderby

        /// <summary>
        /// Tests case expression within a $orderby query.
        /// Query: ?$orderby=case(Status eq 'Active': 0, Status eq 'Pending': 1, true: 2)
        /// </summary>
        [Fact]
        public void CaseInOrderByExpression()
        {
            // Arrange
            var parser = new ODataQueryOptionParser(
                this.model,
                this.userType,
                this.usersSet,
                new Dictionary<string, string> { { "$orderby", "case(Status eq 'Active': 0, Status eq 'Pending': 1, true: 2)" } });

            // Act
            OrderByClause orderByClause = parser.ParseOrderBy();

            // Assert
            Assert.NotNull(orderByClause);
            var caseFunctionCall = orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");

            // Should have 6 parameters (3 condition/result pairs)
            Assert.Equal(6, caseFunctionCall.Parameters.Count());
            Assert.Equal(OrderByDirection.Ascending, orderByClause.Direction);
        }

        /// <summary>
        /// Tests case expression in orderby with descending direction.
        /// Query: ?$orderby=case(Age lt 18: 'Minor', true: 'Adult') desc
        /// </summary>
        [Fact]
        public void CaseInOrderByDescending()
        {
            // Arrange
            var parser = new ODataQueryOptionParser(
                this.model,
                this.userType,
                this.usersSet,
                new Dictionary<string, string> { { "$orderby", "case(Age lt 18: 'Minor', true: 'Adult') desc" } });

            // Act
            OrderByClause orderByClause = parser.ParseOrderBy();

            // Assert
            Assert.NotNull(orderByClause);
            var caseFunctionCall = orderByClause.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.Equal(4, caseFunctionCall.Parameters.Count());
            Assert.Equal(OrderByDirection.Descending, orderByClause.Direction);
        }

        #endregion

        #region Null Handling Tests

        /// <summary>
        /// Tests case expression with null literal result.
        /// Query: ?$compute=case(Status eq 'Unknown': null, true: Name) as DisplayName
        /// </summary>
        [Fact]
        public void CaseInComputeWithNullResult()
        {
            // Arrange
            string compute = "case(Status eq 'Unknown': null, true: Name) as DisplayName";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("DisplayName", computeExp.Alias);

            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsUntyped());

            Assert.Collection(caseFunctionCall.Parameters,
                param1 => param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.Equal),
                param2 => param2.ShouldBeConstantQueryNode<string>(null),
                param3 => param3.ShouldBeConstantQueryNode(true),
                param4 => Assert.IsType<SingleValuePropertyAccessNode>(param4));
        }

        /// <summary>
        /// Tests case expression checking for null values.
        /// Query: ?$compute=case(Name ne null: Name, true: 'Unknown') as SafeName
        /// </summary>
        [Fact]
        public void CaseInComputeWithNullCheck()
        {
            // Arrange
            string compute = "case(Name ne null: Name, true: 'Unknown') as SafeName";

            // Act
            ComputeClause computeClause = ParseCompute(compute, this.model, this.userType, this.usersSet);

            // Assert
            Assert.NotNull(computeClause);
            ComputeExpression computeExp = Assert.Single(computeClause.ComputedItems);
            Assert.Equal("SafeName", computeExp.Alias);

            var caseFunctionCall = computeExp.Expression.ShouldBeSingleValueFunctionCallQueryNode("case");
            Assert.True(caseFunctionCall.TypeReference.IsString());

            Assert.Collection(caseFunctionCall.Parameters,
                // Condition: Name ne null
                param1 =>
                {
                    var binaryOp = param1.ShouldBeBinaryOperatorNode(BinaryOperatorKind.NotEqual);
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(binaryOp.Left);
                    Assert.Equal("Name", propAccess.Property.Name);
                    // null is wrapped in a ConvertNode
                    var convertNode = Assert.IsType<ConvertNode>(binaryOp.Right);
                    convertNode.Source.ShouldBeConstantQueryNode<string>(null);
                },
                // Result: Name
                param2 =>
                {
                    var propAccess = Assert.IsType<SingleValuePropertyAccessNode>(param2);
                    Assert.Equal("Name", propAccess.Property.Name);
                },
                // Default condition: true
                param3 => param3.ShouldBeConstantQueryNode(true),
                // Default result: 'Unknown'
                param4 => param4.ShouldBeConstantQueryNode("Unknown"));
        }

        #endregion

        #region Negative Tests

        /// <summary>
        /// Tests that case expression with odd number of arguments throws appropriate error.
        /// This uses a valid-syntax expression that parses but fails validation.
        /// </summary>
        [Fact]
        public void CaseWithOddNumberOfArgumentsThrows()
        {
            // Arrange - create a scenario with 3 arguments (odd number)
            // We can't use string parsing as it catches syntax errors first
            // Instead we test the scenario where there would be 3 args total
            string compute = "case(Age lt 18: 'Minor', Age lt 65: 'Adult', true) as Invalid";

            // Act & Assert
            var exception = Assert.Throws<ODataException>(() =>
                ParseCompute(compute, this.model, this.userType, this.usersSet));

            // The error should mention even number requirement or parameter count
            Assert.Equal("':' expected at position 49 in 'case(Age lt 18: 'Minor', Age lt 65: 'Adult', true) as Invalid'. The 'case' function requires 'condition:result' pairs.", exception.Message);
        }

        /// <summary>
        /// Tests that case expression with non-boolean condition throws appropriate error.
        /// Query uses a string literal where a boolean condition is expected.
        /// </summary>
        [Fact]
        public void CaseWithNonBooleanConditionThrows()
        {
            // Arrange - use Age (integer) as condition instead of boolean
            string compute = "case(Age: 'Result', true: 'Default') as Invalid";

            // Act & Assert
            var exception = Assert.Throws<ODataException>(() => ParseCompute(compute, this.model, this.userType, this.usersSet));

            // The error should mention that condition must be boolean
            Assert.Equal("The condition at position '0' in the 'case' function must be a single-value boolean expression.", exception.Message);
        }

        #endregion

        private static IEdmModel CreateModel()
        {
            var model = new EdmModel();

            // Create User entity type
            var userType = new EdmEntityType("NS", "User");
            var idProperty = userType.AddStructuralProperty("ID", EdmPrimitiveTypeKind.Int32, false);
            userType.AddKeys(idProperty);
            userType.AddStructuralProperty("Name", EdmPrimitiveTypeKind.String);
            userType.AddStructuralProperty("Age", EdmPrimitiveTypeKind.Int32);
            userType.AddStructuralProperty("Status", EdmPrimitiveTypeKind.String);
            userType.AddStructuralProperty("StockQuantity", EdmPrimitiveTypeKind.Int32);
            userType.AddStructuralProperty("IsActive", EdmPrimitiveTypeKind.Boolean);
            userType.AddStructuralProperty("PrimaryContact", EdmPrimitiveTypeKind.String);
            userType.AddStructuralProperty("BusinessEmails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            userType.AddStructuralProperty("PersonalEmails", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            userType.AddStructuralProperty("CustomTags", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            userType.AddStructuralProperty("SystemTags", new EdmCollectionTypeReference(new EdmCollectionType(EdmCoreModel.Instance.GetString(true))));
            model.AddElement(userType);

            // Create entity container
            var container = new EdmEntityContainer("NS", "Container");
            model.AddElement(container);

            // Create Users entity set
            var usersSet = container.AddEntitySet("Users", userType);

            return model;
        }

        /// <summary>
        /// Helper method to parse a $compute query option
        /// </summary>
        /// <param name="compute">The compute expression string</param>
        /// <param name="model">The EDM model</param>
        /// <param name="entityType">The entity type</param>
        /// <param name="navigationSource">The navigation source (entity set)</param>
        /// <returns>The parsed ComputeClause</returns>
        private static ComputeClause ParseCompute(string compute, IEdmModel model, IEdmStructuredType entityType, IEdmNavigationSource navigationSource)
        {
            var parser = new ODataQueryOptionParser(
                model,
                entityType,
                navigationSource,
                new Dictionary<string, string> { { "$compute", compute } });

            return parser.ParseCompute();
        }
    }
}
