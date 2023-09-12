// @ts-nocheck
/**
 * Converts OData CSDL XML 2.0 and 4.0x to OData CSDL JSON
 */

//TODO:
// - read vocabulary to find out type of term to correctly determine "empty" value

const sax = require("sax");

const VOCABULARIES = {
  Core: {
    Namespace: "Org.OData.Core.V1",
    Uri: "https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Core.V1.json",
  },
  Capabilities: {
    Namespace: "Org.OData.Capabilities.V1",
    Uri: "https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Capabilities.V1.json",
  },
  Validation: {
    Namespace: "Org.OData.Validation.V1",
    Uri: "https://oasis-tcs.github.io/odata-vocabularies/vocabularies/Org.OData.Validation.V1.json",
  },
};

module.exports.xml2json = function (
  xml,
  { lineNumbers = false, annotations = false, strict = false } = {}
) {
  const result = {};
  const alias = { Edm: "Edm", odata: "odata" };
  const namespace = {};
  const namespaceUri = {};

  const preV4 = {
    association: {},
    associationSets: [],
    referentialConstraints: [],
    entitySet: {},
    current: {
      association: null,
      associationSet: null,
      namespace: null,
    },
    needs: {},
    nonFilterable: {},
    filterRestrictions: {},
    requiredInFilter: {},
    nonSortable: {},
  };

  const preParser = sax.parser(true, { xmlns: true });

  preParser.onerror = function (e) {
    throw e;
  };

  preParser.onopentag = function (node) {
    switch (node.local) {
      case "Edmx":
        checkAttribute(node, "Version");
        setAttributes(result, node, ["Version"]);
        break;
      case "DataServices": {
        const version = Object.values(node.attributes).find(
          (attr) => attr.local == "DataServiceVersion"
        );
        //TODO: check node.uri for the correct XML namespace
        if (version) result.$Version = version.value;
        break;
      }
      case "Reference":
        checkAttribute(node, "Uri");
        namespaceUri.$current = node.attributes.Uri.value;
        break;
      case "Include":
        checkAttribute(node, "Namespace");
        namespaceUri[node.attributes.Namespace.value] = namespaceUri.$current;
        if (node.attributes.Alias)
          namespaceUri[node.attributes.Alias.value] = namespaceUri.$current;
      // fall through
      case "Schema":
        checkAttribute(node, "Namespace");
        if (node.attributes.Alias) {
          alias[node.attributes.Namespace.value] = node.attributes.Alias.value;
          alias[node.attributes.Alias.value] = node.attributes.Alias.value;
          namespace[node.attributes.Alias.value] =
            node.attributes.Namespace.value;
        } else {
          alias[node.attributes.Namespace.value] =
            node.attributes.Namespace.value;
        }
        namespace[node.attributes.Namespace.value] =
          node.attributes.Namespace.value;
        preV4.current.namespace = node.attributes.Namespace.value;
        break;
      case "Association": {
        checkAttribute(node, "Name");
        const name = preV4.current.namespace + "." + node.attributes.Name.value;
        preV4.current.association = preV4.association[name] || {};
        preV4.association[name] = preV4.current.association;
        break;
      }
      case "NavigationProperty":
        checkAttribute(node, "Name");
        if (result.$Version < "4.0") {
          checkAttribute(node, "Relationship");
          checkAttribute(node, "FromRole");
          const parts = nameParts(node.attributes.Relationship.value);
          const name = namespace[parts.qualifier] + "." + parts.name;
          const association = preV4.association[name] || {};
          const end = association[node.attributes.FromRole.value] || {};
          end.$Partner = node.attributes.Name.value;
          association[node.attributes.FromRole.value] = end;
          preV4.association[name] = association;
        }
        break;
      case "AssociationSet":
        checkAttribute(node, "Association");
        preV4.current.associationSet = {
          associationName: node.attributes.Association.value,
          ends: [],
        };
        preV4.associationSets.push(preV4.current.associationSet);
        break;
      case "End":
        checkAttribute(node, "Role");
        if (preV4.current.association) {
          checkAttribute(node, "Type");
          preV4.current.associationEnd =
            preV4.current.association[node.attributes.Role.value] || {};
          preV4.current.associationEnd.type = normalizeTarget(
            node.attributes.Type.value
          );
          preV4.current.associationEnd.multiplicity =
            node.attributes.Multiplicity.value;
          preV4.current.association[node.attributes.Role.value] =
            preV4.current.associationEnd;
        } else if (preV4.current.associationSet) {
          checkAttribute(node, "EntitySet");
          preV4.current.associationSet.ends.push({
            set: node.attributes.EntitySet.value,
            role: node.attributes.Role.value,
          });
        }
        break;
      case "OnDelete":
        checkAttribute(node, "Action");
        if (result.$Version < "4.0") {
          preV4.current.associationEnd.onDelete = node.attributes.Action.value;
        }
        break;
      case "ReferentialConstraint":
        if (result.$Version < "4.0") {
          preV4.current.referentialConstraint = {
            association: preV4.current.association,
            principalProperties: [],
            dependentProperties: [],
          };
          preV4.referentialConstraints.push(
            preV4.current.referentialConstraint
          );
        }
        break;
      case "Principal":
        preV4.current.referentialConstraint.properties =
          preV4.current.referentialConstraint.principalProperties;
        break;
      case "Dependent":
        preV4.current.referentialConstraint.properties =
          preV4.current.referentialConstraint.dependentProperties;
        preV4.current.referentialConstraint.dependentRole =
          node.attributes.Role.value;
        break;
      case "PropertyRef":
        if (preV4.current.referentialConstraint)
          preV4.current.referentialConstraint.properties.push(
            node.attributes.Name.value
          );
        break;
      case "Documentation":
        preV4.needs.Core = true;
        break;
    }
  };

  preParser.onclosetag = function (tag) {
    switch (tag) {
      case "Schema":
        preV4.current.namespace = null;
        break;
      case "Association":
        preV4.current.association = null;
        break;
      case "AssociationSet":
        preV4.current.associationSet = null;
        break;
      case "End":
        preV4.current.associationEnd = null;
        break;
      case "ReferentialConstraint":
        preV4.current.referentialConstraint = null;
        break;
      case "Principal":
      case "Dependent":
        delete preV4.current.referentialConstraint.properties;
        break;
    }
  };

  try {
    preParser.write(xml).close();
  } catch (e) {
    throw enrichedException(e, xml, preParser);
  }

  preV4.associationSets.forEach((associationSet) => {
    const parts = nameParts(associationSet.associationName);
    const assoc =
      preV4.association[namespace[parts.qualifier] + "." + parts.name];
    associationSet.ends.forEach((end, i) => {
      const other = associationSet.ends[1 - i];
      if (!preV4.entitySet[end.set]) preV4.entitySet[end.set] = [];
      if (assoc[end.role].$Partner) {
        preV4.entitySet[end.set].push({
          path: assoc[end.role].$Partner,
          type: assoc[end.role].type,
          target: other.set,
        });
      }
    });
  });

  preV4.referentialConstraints.forEach((constraint) => {
    const c = {};
    constraint.association[constraint.dependentRole].constraint = c;
    constraint.dependentProperties.forEach((property, i) => {
      c[property] = constraint.principalProperties[i];
    });
  });

  if (result.$Version < "4.0") {
    for (const [a, v] of Object.entries(VOCABULARIES)) {
      if (!alias[v.Namespace]) {
        alias[v.Namespace] = a;
        alias[a] = a;
      }
    }
  }

  const current = { annotatable: [], annotation: [], container: {} };
  const last = { target: null };
  const parser = sax.parser(true, { xmlns: true });

  // Note: parsing errors should already have been caught by preParser
  // parser.onerror = function (e) {
  //   throw e;
  // };

  parser.onopentag = function (node) {
    const attributeExpressions = [
      "Binary",
      "Bool",
      "Date",
      "DateTimeOffset",
      "Decimal",
      "Duration",
      "EnumMember",
      "Float",
      "Guid",
      "Int",
      "String",
      "TimeOfDay",
      "AnnotationPath",
      "ModelElementPath",
      "NavigationPropertyPath",
      "PropertyPath",
      "Path",
      "UrlRef",
    ];

    if (!checkElementAllowed(node)) return;

    let annotatable = { target: null, prefix: "" };
    let annotation = {};

    switch (node.local) {
      case "Edmx":
        setAttributes(null, node, [], ["Version"]);
        break;
      case "Reference": {
        setAttributes(null, node, [], ["Uri"]);
        current.reference = {};
        let uri = node.attributes.Uri.value;
        if (
          (uri.startsWith(
            "https://oasis-tcs.github.io/odata-vocabularies/vocabularies/"
          ) ||
            uri.startsWith(
              "https://sap.github.io/odata-vocabularies/vocabularies/"
            )) &&
          uri.endsWith(".xml")
        )
          uri = uri.substring(0, uri.length - 3) + "json";
        if (!result.$Reference) result.$Reference = {};
        result.$Reference[uri] = current.reference;
        annotatable.target = current.reference;
        break;
      }
      case "Include":
        current.include = {};
        setAttributes(current.include, node, ["Namespace", "Alias"]);
        if (!current.reference.$Include) current.reference.$Include = [];
        current.reference.$Include.push(current.include);
        annotatable.target = current.include;
        break;
      case "IncludeAnnotations":
        checkAttribute(node, "TermNamespace");
        current.includeAnnotations = {};
        setAttributes(current.includeAnnotations, node, [
          "TargetNamespace",
          "TermNamespace",
          "Qualifier",
        ]);
        if (!current.reference.$IncludeAnnotations)
          current.reference.$IncludeAnnotations = [];
        current.reference.$IncludeAnnotations.push(current.includeAnnotations);
        annotatable.target = current.includeAnnotations;
        break;
      case "Schema":
        current.schema = {};
        current.schemaName = node.attributes.Namespace.value;
        setAttributes(current.schema, node, ["Alias"], ["Namespace"]);
        v2Annotations(current.schema, node, SAP_V2_SCHEMA);
        result[node.attributes.Namespace.value] = current.schema;
        annotatable.target = current.schema;
        break;
      case "EntityType":
        checkAttribute(node, "Name");
        current.type = { $Kind: node.local };
        current.qualifiedTypeName =
          alias[current.schemaName] + "." + node.attributes.Name.value;
        addLineNumber(current.type);
        setAttributes(current.type, node, [
          "Abstract",
          "BaseType",
          "HasStream",
          "OpenType",
        ]);
        if (result.$Version < "4.0") {
          const hasStream = Object.values(node.attributes).find(
            (attr) => attr.local === "HasStream"
          );
          //TODO: check node.uri for the correct XML namespace
          if (hasStream && hasStream.value == "true")
            current.type.$HasStream = true;
        }
        current.schema[node.attributes.Name.value] = current.type;
        annotatable.target = current.type;
        break;
      case "Key":
        setAttributes(null, node, []);
        current.type.$Key = [];
        break;
      case "PropertyRef":
        checkAttribute(node, "Name");
        if (current.type) {
          const attr = {};
          setAttributes(attr, node, ["Name", "Alias"]);
          if (attr.$Alias) {
            const key = {};
            key[attr.$Alias] = attr.$Name;
            current.type.$Key.push(key);
          } else {
            current.type.$Key.push(attr.$Name);
          }
        }
        break;
      case "ComplexType":
        checkAttribute(node, "Name");
        current.type = { $Kind: node.local };
        current.qualifiedTypeName =
          alias[current.schemaName] + "." + node.attributes.Name.value;
        addLineNumber(current.type);
        setAttributes(current.type, node, ["Abstract", "BaseType", "OpenType"]);
        current.schema[node.attributes.Name.value] = current.type;
        annotatable.target = current.type;
        break;
      case "NavigationProperty":
        checkAttribute(node, "Name");
        current.property = { $Kind: node.local };
        addLineNumber(current.property);
        if (result.$Version < "4.0") {
          const parts = nameParts(node.attributes.Relationship.value);
          const association =
            preV4.association[namespace[parts.qualifier] + "." + parts.name];
          const toEnd = association[node.attributes.ToRole.value];
          current.property.$Type = toEnd.type;
          if (toEnd.multiplicity == "*") current.property.$Collection = true;
          if (toEnd.multiplicity == "0..1") current.property.$Nullable = true;
          if (toEnd.$Partner) current.property.$Partner = toEnd.$Partner;
          const fromEnd = association[node.attributes.FromRole.value];
          if (fromEnd.onDelete) current.property.$OnDelete = fromEnd.onDelete;
          if (fromEnd.constraint)
            current.property.$ReferentialConstraint = fromEnd.constraint;
        } else {
          checkAttribute(node, "Type");
          setAttributes(current.property, node, [
            "Type",
            "Nullable",
            "ContainsTarget",
            "Partner",
          ]);
        }
        current.type[node.attributes.Name.value] = current.property;
        annotatable.target = current.property;
        break;
      case "OnDelete":
        checkAttribute(node, "Action");
        setAttributes(null, node, [], ["Action"]);
        if (current.property) {
          current.property.$OnDelete = node.attributes.Action.value;
          annotatable.target = current.property;
          annotatable.prefix = "$OnDelete";
        }
        break;
      case "ReferentialConstraint":
        if (current.property) {
          checkAttribute(node, "Property");
          checkAttribute(node, "ReferencedProperty");
          setAttributes(null, node, [], ["Property", "ReferencedProperty"]);
          if (!current.property.$ReferentialConstraint)
            current.property.$ReferentialConstraint = {};
          current.property.$ReferentialConstraint[
            node.attributes.Property.value
          ] = node.attributes.ReferencedProperty.value;
          annotatable.target = current.property.$ReferentialConstraint;
          annotatable.prefix = node.attributes.Property.value;
        }
        break;
      case "Property":
        checkAttribute(node, "Name");
        checkAttribute(node, "Type");
        current.property = {};
        addLineNumber(current.property);
        setAttributes(
          current.property,
          node,
          [
            "Type",
            "Nullable",
            "MaxLength",
            "Unicode",
            "Precision",
            "Scale",
            "SRID",
            "DefaultValue",
          ],
          result.$Version < "4.0" ? ["ConcurrencyMode", "FixedLength"] : []
        );
        v2PropertyAnnotations(current.property, node);
        current.type[node.attributes.Name.value] = current.property;
        annotatable.target = current.property;
        break;
      case "EnumType":
        checkAttribute(node, "Name");
        current.type = { $Kind: node.local };
        addLineNumber(current.type);
        current.enumMemberValue = 0;
        setAttributes(current.type, node, ["UnderlyingType", "IsFlags"]);
        current.schema[node.attributes.Name.value] = current.type;
        annotatable.target = current.type;
        break;
      case "Member": {
        checkAttribute(node, "Name");
        setAttributes(null, node, [], ["Value"]);
        const member = node.attributes.Name.value;
        const value = Number(node.attributes.Value?.value);
        addLineNumber(current.type, member);
        current.type[member] = Number.isNaN(value)
          ? current.enumMemberValue
          : value;
        current.enumMemberValue++;
        annotatable.target = current.type;
        annotatable.prefix = node.attributes.Name.value;
        break;
      }
      case "TypeDefinition":
        checkAttribute(node, "Name");
        checkAttribute(node, "UnderlyingType");
        current.type = { $Kind: node.local };
        addLineNumber(current.type);
        setAttributes(current.type, node, [
          "UnderlyingType",
          "MaxLength",
          "Unicode",
          "Precision",
          "Scale",
          "SRID",
        ]);
        current.schema[node.attributes.Name.value] = current.type;
        annotatable.target = current.type;
        break;
      case "Action":
        checkAttribute(node, "Name");
        current.overload = { $Kind: node.local };
        addLineNumber(current.overload);
        setAttributes(current.overload, node, ["EntitySetPath", "IsBound"]);
        if (!current.schema[node.attributes.Name.value])
          current.schema[node.attributes.Name.value] = [];
        if (Array.isArray(current.schema[node.attributes.Name.value]))
          current.schema[node.attributes.Name.value].push(current.overload);
        annotatable.target = current.overload;
        break;
      case "Function":
        checkAttribute(node, "Name");
        current.overload = { $Kind: node.local };
        addLineNumber(current.overload);
        setAttributes(current.overload, node, [
          "EntitySetPath",
          "IsBound",
          "IsComposable",
        ]);
        if (!current.schema[node.attributes.Name.value])
          current.schema[node.attributes.Name.value] = [];
        if (Array.isArray(current.schema[node.attributes.Name.value]))
          current.schema[node.attributes.Name.value].push(current.overload);
        annotatable.target = current.overload;
        break;
      case "Parameter": {
        checkAttribute(node, "Name");
        checkAttribute(node, "Type");
        const parameter = { $Name: node.attributes.Name.value };
        addLineNumber(parameter);
        setAttributes(
          parameter,
          node,
          [
            "Type",
            "Nullable",
            "MaxLength",
            "Unicode",
            "Precision",
            "Scale",
            "SRID",
          ],
          result.$Version < "4.0" ? ["Mode"] : []
        );
        if (!current.overload) throw new Error("Unexpected element: Parameter");
        if (!current.overload.$Parameter) current.overload.$Parameter = [];
        current.overload.$Parameter.push(parameter);
        annotatable.target = parameter;
        break;
      }
      case "ReturnType":
        checkAttribute(node, "Type");
        current.overload.$ReturnType = {};
        addLineNumber(current.overload.$ReturnType);
        setAttributes(current.overload.$ReturnType, node, [
          "Type",
          "Nullable",
          "MaxLength",
          "Unicode",
          "Precision",
          "Scale",
          "SRID",
        ]);
        annotatable.target = current.overload.$ReturnType;
        break;
      case "EntityContainer":
        checkAttribute(node, "Name");
        result.$EntityContainer =
          current.schemaName + "." + node.attributes.Name.value;
        current.container = { $Kind: node.local };
        setAttributes(current.container, node, ["Extends"]);
        current.schema[node.attributes.Name.value] = current.container;
        annotatable.target = current.container;
        break;
      case "EntitySet": {
        checkAttribute(node, "Name");
        checkAttribute(node, "EntityType");
        current.containerChild = { $Collection: true };
        setAttributes(current.containerChild, node, [
          "EntityType",
          "IncludeInServiceDocument",
        ]);
        v2Annotations(current.containerChild, node, SAP_V2_ENTITY_SET);
        current.container[node.attributes.Name.value] = current.containerChild;
        annotatable.target = current.containerChild;
        const bindings = preV4.entitySet[node.attributes.Name.value];
        if (bindings && bindings.length > 0) {
          current.containerChild.$NavigationPropertyBinding = {};
          bindings.forEach((binding) => {
            const path =
              (binding.type != current.containerChild.$Type
                ? binding.type + "/"
                : "") + binding.path;
            current.containerChild.$NavigationPropertyBinding[path] =
              binding.target;
          });
        }
        break;
      }
      case "Singleton":
        checkAttribute(node, "Name");
        checkAttribute(node, "Type");
        current.containerChild = {};
        setAttributes(current.containerChild, node, ["Type", "Nullable"]);
        current.container[node.attributes.Name.value] = current.containerChild;
        annotatable.target = current.containerChild;
        break;
      case "ActionImport":
        checkAttribute(node, "Name");
        checkAttribute(node, "Action");
        current.containerChild = {};
        setAttributes(current.containerChild, node, ["Action", "EntitySet"]);
        current.container[node.attributes.Name.value] = current.containerChild;
        annotatable.target = current.containerChild;
        break;
      case "FunctionImport":
        checkAttribute(node, "Name");
        current.containerChild = {};
        if (result.$Version < "4.0") {
          const method = Object.values(node.attributes).find(
            (attr) => attr.local == "HttpMethod"
          );
          const operationName =
            current.schemaName + "." + node.attributes.Name.value;
          current.overload = {};
          if (method == null) {
            if (
              node.attributes.IsBindable &&
              node.attributes.IsBindable.value == "true"
            ) {
              current.overload.$IsBound = true;
              annotatable.target = current.overload;
            } else {
              current.container[node.attributes.Name.value] =
                current.containerChild;
              annotatable.target = current.containerChild;
            }
            if (
              node.attributes.IsSideEffecting &&
              node.attributes.IsSideEffecting.value == "false"
            ) {
              current.overload.$Kind = "Function";
              if (!current.overload.$IsBound)
                current.containerChild.$Function = operationName;
            } else {
              current.overload.$Kind = "Action";
              if (!current.overload.$IsBound) {
                current.containerChild.$Action = operationName;
              }
            }
          } else if (method.value == "GET") {
            current.containerChild.$Function = operationName;
            current.overload.$Kind = "Function";
            current.container[node.attributes.Name.value] =
              current.containerChild;
            annotatable.target = current.containerChild;
          } else {
            current.containerChild.$Action = operationName;
            current.overload.$Kind = "Action";
            current.container[node.attributes.Name.value] =
              current.containerChild;
            annotatable.target = current.containerChild;
          }
          setAttributes(
            current.containerChild,
            node,
            ["EntitySet"],
            ["ReturnType", "IsBindable", "IsSideEffecting"]
          );
          const returnType = node.attributes.ReturnType;
          if (returnType) {
            current.overload.$ReturnType = {};
            setAttributes(
              current.overload.$ReturnType,
              node,
              ["ReturnType"],
              ["EntitySet", "IsBindable", "IsSideEffecting"]
            );
          }
          current.schema[node.attributes.Name.value] = [current.overload];
        } else {
          checkAttribute(node, "Function");
          setAttributes(current.containerChild, node, [
            "Function",
            "EntitySet",
            "IncludeInServiceDocument",
          ]);
          current.container[node.attributes.Name.value] =
            current.containerChild;
          annotatable.target = current.containerChild;
        }
        break;
      case "NavigationPropertyBinding": {
        checkAttribute(node, "Path");
        checkAttribute(node, "Target");
        setAttributes(null, node, [], ["Path", "Target"]);
        if (!current.containerChild.$NavigationPropertyBinding)
          current.containerChild.$NavigationPropertyBinding = {};
        let target = normalizeTarget(node.attributes.Target.value);
        if (target.startsWith(normalizeTarget(result.$EntityContainer) + "/"))
          target = target.substring(target.indexOf("/") + 1);
        current.containerChild.$NavigationPropertyBinding[
          node.attributes.Path.value
        ] = target;
        break;
      }
      case "Term": {
        const term = { $Kind: node.local };
        addLineNumber(term);
        setAttributes(term, node, [
          "Type",
          "Nullable",
          "DefaultValue",
          "AppliesTo",
          "BaseTerm",
          "MaxLength",
          "Precision",
          "Scale",
          "SRID",
        ]);
        current.schema[node.attributes.Name.value] = term;
        annotatable.target = term;
        break;
      }
      case "Annotations": {
        checkAttribute(node, "Target");
        const target = normalizeTarget(node.attributes.Target?.value);
        if (!current.schema.$Annotations) current.schema.$Annotations = {};
        if (!current.schema.$Annotations[target])
          current.schema.$Annotations[target] = {};
        annotatable.target = current.schema.$Annotations[target];
        current.qualifier = node.attributes.Qualifier?.value;
        break;
      }
      case "Documentation":
        annotatable.target = last.target;
        break;
      case "Summary":
      case "LongDescription":
        current.text = "";
        annotation.$Term =
          "Org.OData.Core.V1." +
          (node.local === "Summary" ? "Description" : node.local);
      // fall through
      case "ValueAnnotation":
      case "Annotation":
        if (node.local === "Annotation") checkAttribute(node, "Term");
        setAttributes(
          annotation,
          node,
          ["Term", "Qualifier"].concat(attributeExpressions)
        );
        current.annotation.unshift(annotation);
        annotatable.target = current.annotatable[0].target;
        annotatable.prefix =
          current.annotatable[0].prefix +
          "@" +
          normalizeTarget(annotation.$Term) +
          (current.qualifier ? "#" + current.qualifier : "") +
          (annotation.$Qualifier ? "#" + annotation.$Qualifier : "");
        break;
      case "Collection":
        setAttributes(null, node, []);
        annotation.value = [];
        current.annotation.unshift(annotation);
        break;
      case "Record":
        setAttributes(null, node, [], ["Type"]);
        annotation.value = record(node);
        addLineNumber(annotation.value);
        current.annotation.unshift(annotation);
        annotatable.target = current.annotation[0].value;
        break;
      case "PropertyValue":
        checkAttribute(node, "Property");
        setAttributes(
          annotation,
          node,
          ["Property"].concat(attributeExpressions)
        );
        current.annotation.unshift(annotation);
        annotatable.target = current.annotatable[0].target;
        annotatable.prefix = annotation.$Property;
        break;
      case "Apply":
        checkAttribute(node, "Function");
        setAttributes(annotation, node, ["Function"]);
        annotation.value = [];
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "Add":
      case "And":
      case "Div":
      case "DivBy":
      case "Eq":
      case "Ge":
      case "Gt":
      case "Has":
      case "If":
      case "In":
      case "Le":
      case "Lt":
      case "Mod":
      case "Mul":
      case "Ne":
      case "Or":
      case "Sub":
        setAttributes(null, node, []);
        annotation.value = [];
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "Cast":
      case "IsOf":
        checkAttribute(node, "Type");
        setAttributes(annotation, node, [
          "Type",
          "MaxLength",
          "Unicode",
          "Precision",
          "Scale",
          "SRID",
        ]);
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "LabeledElement":
        checkAttribute(node, "Name");
        setAttributes(annotation, node, ["Name"].concat(attributeExpressions));
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "Neg":
      case "Not":
      case "Null":
        setAttributes(null, node, []);
        annotation.value = {};
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "UrlRef":
        setAttributes(null, node, []);
        current.annotation.unshift(annotation);
        annotatable.target = annotation;
        break;
      case "String":
        setAttributes(null, node, []);
        current.text = "";
        break;
      // valid V4 elements that may also be used in V2/V3
      case "DataServices":
        setAttributes(null, node, []);
        break;
      case "AnnotationPath":
      case "ModelElementPath":
      case "NavigationPropertyPath":
      case "PropertyPath":
      case "Path":
      case "Binary":
      case "Bool":
      case "Date":
      case "DateTimeOffset":
      case "Decimal":
      case "Duration":
      case "EnumMember":
      case "Float":
      case "Guid":
      case "Int":
      case "TimeOfDay":
      case "LabeledElementReference":
        setAttributes(null, node, []);
        current.text = "";
        break;
      // valid V2/V3 elements
      case "Association":
      case "End":
      case "AssociationSet":
      case "Principal":
      case "Dependent":
        //TODO: throw if V2/V3 elements are used in V4 - also in some of the cases above, e.g. Documentation
        if (result.$Version >= "4.0")
          throw new Error(`Unexpected element: ${node.local}`);
        break;
      // default:
      //   if (result.$Version >= "4.0")
      //     throw new Error(`Unexpected element: ${node.local}`);
    }

    current.annotatable.unshift(annotatable);
    last.target = annotatable.target;
  };

  parser.ontext = function (text) {
    if (current.text === "") current.text = text;
    else if (text.trim() !== "")
      throw new Error(`Element ${elements[0].local}, unexpected text: ${text}`);
  };

  parser.onclosetag = function (tag) {
    const local = tag.includes(":") ? tag.split(":")[1] : tag;

    if (!checkElementComplete(local)) return;

    switch (local) {
      case "Edmx":
        break;
      case "Reference":
        current.reference = null;
        break;
      case "Include":
        current.include = null;
        break;
      case "Schema":
        current.schema = null;
        current.schemaName = null;
        break;
      case "ComplexType":
      case "EntityType":
        current.type = null;
        current.qualifiedTypeName = null;
        break;
      case "NavigationProperty":
      case "Property":
        current.property = null;
        break;
      case "EnumType":
        current.enumMemberValue = null;
        break;
      case "Action":
      case "Function":
        current.overload = null;
        break;
      case "ActionImport":
      case "EntitySet":
      case "FunctionImport":
      case "Singleton":
        current.containerChild = null;
        break;
      case "Annotations":
        current.qualifier = null;
        break;
      case "Summary":
      case "LongDescription":
        if (current.text.length === 0) {
          current.text = null;
          break;
        }
        updateValue(
          current.annotation[0],
          current.text.replace(/\r\n|\r(?!\n)/g, "\n")
        );
        current.text = null;
      // fall through
      case "ValueAnnotation":
      case "Annotation": {
        let annotation = current.annotation.shift();
        if (current.annotatable[1].target != null) {
          const name =
            current.annotatable[1].prefix +
            "@" +
            normalizeTarget(annotation.$Term) +
            (current.qualifier ? "#" + current.qualifier : "") +
            (annotation.$Qualifier ? "#" + annotation.$Qualifier : "");

          const np = nameParts(annotation.$Term);
          if (
            np.name == "MediaType" &&
            namespace[np.qualifier] == "Org.OData.Core.V1" &&
            annotation.value == "application/json"
          ) {
            if (current.annotation.length > 0) {
              current.annotation[0].$isJSON = true;
            }
          }

          if (
            annotation.$isJSON ||
            (np.name == "Schema" &&
              namespace[np.qualifier] == "Org.OData.JSON.V1")
          ) {
            //TODO: check for string value, error handling in case of invalid JSON
            annotation.value = JSON.parse(annotation.value);
          }
          current.annotatable[1].target[name] =
            annotation.value !== undefined ? annotation.value : true;
        }
        break;
      }
      case "Record":
      case "Collection": {
        let annotation = current.annotation.shift();
        updateValue(current.annotation[0], annotation.value);
        break;
      }
      case "PropertyValue": {
        let annotation = current.annotation.shift();
        if (annotation.$isJSON) {
          //TODO: check for string value, error handling in case of invalid JSON
          annotation.value = JSON.parse(annotation.value);
        }
        current.annotation[0].value[annotation.$Property] = annotation.value;
        break;
      }
      case "Add":
      case "Apply":
      case "And":
      case "Div":
      case "DivBy":
      case "Eq":
      case "Ge":
      case "Gt":
      case "Has":
      case "In":
      case "Le":
      case "Lt":
      case "If":
      case "Mod":
      case "Mul":
      case "Ne":
      case "Neg":
      case "Not":
      case "Or":
      case "Sub":
      case "Cast":
      case "IsOf": {
        let annotation = current.annotation.shift();
        annotation["$" + local] = annotation.value;
        delete annotation.value;
        updateValue(current.annotation[0], annotation);
        break;
      }
      case "LabeledElement": {
        let annotation = current.annotation.shift();
        updateValue(current.annotation[0], {
          $LabeledElement: annotation.value,
          $Name: annotation.$Name,
        });
        break;
      }
      case "LabeledElementReference":
      case "Path": {
        let annotation = {};
        annotation["$" + local] = normalizePath(current.text);
        updateValue(current.annotation[0], annotation);
        current.text = null;
        break;
      }
      case "AnnotationPath":
      case "ModelElementPath":
      case "NavigationPropertyPath":
      case "PropertyPath":
        updateValue(current.annotation[0], normalizePath(current.text));
        current.text = null;
        break;
      case "Bool":
        updateValue(current.annotation[0], current.text === "true");
        current.text = null;
        break;
      case "Binary":
      case "Date":
      case "DateTimeOffset":
      case "Duration":
      case "Guid":
      case "TimeOfDay":
        updateValue(current.annotation[0], current.text);
        current.text = null;
        break;
      case "String":
        updateValue(
          current.annotation[0],
          current.text.replace(/\r\n|\r(?!\n)/g, "\n")
        );
        current.text = null;
        break;
      case "Decimal":
      case "Float":
      case "Int":
        updateValue(
          current.annotation[0],
          isNaN(current.text) ? current.text : Number(current.text)
        );
        current.text = null;
        break;
      case "EnumMember":
        if (current.annotation[0].$Term || current.annotation[0].$Property)
          updateValue(current.annotation[0], enumValue(current.text));
        else
          updateValue(current.annotation[0], {
            $Cast: enumValue(current.text),
            $Type: current.text.substring(0, current.text.indexOf("/")),
          });
        current.text = null;
        break;
      case "Null": {
        let annotation = current.annotation.shift();
        if (Object.keys(annotation).length === 1)
          updateValue(current.annotation[0], null);
        else {
          annotation.$Null = null;
          delete annotation.value;
          updateValue(current.annotation[0], annotation);
        }
        break;
      }
      case "UrlRef": {
        let annotation = current.annotation.shift();
        updateValue(current.annotation[0], { $UrlRef: annotation.value });
        break;
      }
      default:
    }

    current.annotatable.shift();
  };

  const BINARY_EXPRESSION = {
    isExpression: true,
    expression: { min: 2, max: 2 },
    Annotation: { min: 0 },
  };
  const UNARY_EXPRESSION = {
    isExpression: true,
    expression: { min: 1, max: 1 },
    Annotation: { min: 0 },
  };
  const SCHEMA = {
    "": { Edmx: { min: 1, max: 1 } },
    Edmx: { x: true, Reference: { min: 0 }, DataServices: { min: 1, max: 1 } },
    Reference: {
      x: true,
      v2x: true,
      Include: { min: 0 },
      Annotation: { min: 0 },
      IncludeAnnotations: { min: 0 },
    },
    Include: { x: true, v2x: true, Annotation: { min: 0 } },
    IncludeAnnotations: { x: true },
    DataServices: { x: true, Schema: { min: 1 } },
    Schema: {
      Annotations: { min: 0 },
      EntityType: { min: 0 },
      ComplexType: { min: 0 },
      EnumType: { min: 0 },
      TypeDefinition: { min: 0 },
      Function: { min: 0 },
      Action: { min: 0 },
      EntityContainer: { min: 0 },
      Term: { min: 0 },
      Annotation: { min: 0 },
      // V2/V3
      Association: { min: 0 },
      //TODO: complete list of V2/V3 child elements
    },
    Annotations: {
      v2x: true,
      Annotation: { min: 0 },
      // V3
      ValueAnnotation: { min: 0 },
      //TODO: complete list of V2/V3 child elements
    },
    EntityType: {
      Key: { min: 0, max: 1 },
      Property: { min: 0 },
      NavigationProperty: { min: 0 },
      Annotation: { min: 0 },
    },
    Key: { PropertyRef: { min: 1 } },
    PropertyRef: {},
    Property: {
      Annotation: { min: 0 },
      // V2/V3
      Documentation: { min: 0 },
    },
    NavigationProperty: {
      OnDelete: { min: 0, max: 1 },
      ReferentialConstraint: { min: 0 },
      Annotation: { min: 0 },
      // V2/V3
      Documentation: { min: 0 },
    },
    OnDelete: { Annotation: { min: 0 } },
    ReferentialConstraint: {
      Annotation: { min: 0 },
      // V2/V3
      Principal: { min: 0 },
      Dependent: { min: 0 },
    },
    ComplexType: {
      Property: { min: 0 },
      NavigationProperty: { min: 0 },
      Annotation: { min: 0 },
    },
    EnumType: { Member: { min: 1 }, Annotation: { min: 0 } },
    Member: { Annotation: { min: 0 } },
    TypeDefinition: { Annotation: { min: 0 } },
    Action: {
      Parameter: { min: 0 },
      ReturnType: { min: 0 },
      Annotation: { min: 0 },
    },
    Function: {
      Parameter: { min: 0 },
      ReturnType: { min: 1 },
      Annotation: { min: 0 },
    },
    Parameter: {
      Annotation: { min: 0 },
      // V2/V3
      Documentation: { min: 0 },
    },
    ReturnType: { Annotation: { min: 0 } },
    EntityContainer: {
      EntitySet: { min: 0 },
      Singleton: { min: 0 },
      ActionImport: { min: 0 },
      FunctionImport: { min: 0 },
      Annotation: { min: 0 },
      // V2/V3
      AssociationSet: { min: 0 },
    },
    EntitySet: {
      NavigationPropertyBinding: { min: 0 },
      Annotation: { min: 0 },
      // V2/V3
      Documentation: { min: 0 },
    },
    Singleton: {
      NavigationPropertyBinding: { min: 0 },
      Annotation: { min: 0 },
    },
    NavigationPropertyBinding: {},
    ActionImport: { Annotation: { min: 0 } },
    FunctionImport: {
      Annotation: { min: 0 },
      // V2/V3
      Documentation: { min: 0 },
      Parameter: { min: 0 },
    },
    Term: { Annotation: { min: 0 } },
    Annotation: {
      v2x: true,
      expression: { min: 0 },
      Annotation: { min: 0 },
    },
    // constant expressions
    Binary: { isExpression: true },
    Bool: { isExpression: true },
    Date: { isExpression: true },
    DateTimeOffset: { isExpression: true },
    Decimal: { isExpression: true },
    Duration: { isExpression: true },
    EnumMember: { isExpression: true },
    Float: { isExpression: true },
    Guid: { isExpression: true },
    Int: { isExpression: true },
    String: { isExpression: true },
    TimeOfDay: { isExpression: true },
    // path expressions
    AnnotationPath: { isExpression: true },
    ModelElementPath: { isExpression: true },
    NavigationPropertyPath: { isExpression: true },
    PropertyPath: { isExpression: true },
    Path: { isExpression: true },
    // comparison and logic expressions
    And: BINARY_EXPRESSION,
    Or: BINARY_EXPRESSION,
    Not: UNARY_EXPRESSION,
    Eq: BINARY_EXPRESSION,
    Ne: BINARY_EXPRESSION,
    Gt: BINARY_EXPRESSION,
    Ge: BINARY_EXPRESSION,
    Lt: BINARY_EXPRESSION,
    Le: BINARY_EXPRESSION,
    Has: BINARY_EXPRESSION,
    In: BINARY_EXPRESSION,
    // arithmetic expressions
    Neg: UNARY_EXPRESSION,
    Add: BINARY_EXPRESSION,
    Sub: BINARY_EXPRESSION,
    Mul: BINARY_EXPRESSION,
    Div: BINARY_EXPRESSION,
    DivBy: BINARY_EXPRESSION,
    Mod: BINARY_EXPRESSION,
    // other dynamic expressions
    Apply: {
      isExpression: true,
      expression: { min: 0 },
      Annotation: { min: 0 },
    },
    Cast: {
      isExpression: true,
      expression: { min: 1, max: 1 },
      Annotation: { min: 0 },
    },
    Collection: {
      isExpression: true,
      expression: { min: 0 },
      Annotation: { min: 0 },
    },
    If: {
      isExpression: true,
      expression: { min: 2, max: 3 },
      Annotation: { min: 0 },
    },
    IsOf: {
      isExpression: true,
      expression: { min: 1, max: 1 },
      Annotation: { min: 0 },
    },
    LabeledElement: {
      isExpression: true,
      expression: { min: 0, max: 1 },
      Annotation: { min: 0 },
    },
    LabeledElementReference: { isExpression: true },
    Null: { isExpression: true, Annotation: { min: 0 } },
    Record: {
      isExpression: true,
      PropertyValue: { min: 0 },
      Annotation: { min: 0 },
    },
    PropertyValue: {
      v2x: true,
      expression: { min: 0 },
      Annotation: { min: 0 },
    },
    UrlRef: {
      isExpression: true,
      expression: { min: 0, max: 1 },
      Annotation: { min: 0 },
    },

    // V2/V3
    //TODO: complete?
    Association: {
      End: { min: 2, max: 2 },
      ReferentialConstraint: { min: 0, max: 1 },
    },
    AssociationSet: {
      End: { min: 2, max: 2 },
    },
    End: { OnDelete: { min: 0, max: 1 } },
    Principal: { PropertyRef: { min: 1 } },
    Dependent: { PropertyRef: { min: 1 } },
    Documentation: {
      Summary: { min: 0, max: 1 },
      LongDescription: { min: 0, max: 1 },
    },
  };

  const EDMX = "http://docs.oasis-open.org/odata/ns/edmx";
  const EDM = "http://docs.oasis-open.org/odata/ns/edm";
  const EDMX_V1 = "http://schemas.microsoft.com/ado/2007/06/edmx";
  const EDM_1 = "http://schemas.microsoft.com/ado/2006/04/edm";
  const EDM_2 = "http://schemas.microsoft.com/ado/2007/05/edm";
  const EDM_3 = "http://schemas.microsoft.com/ado/2008/01/edm";
  const EDM_4 = "http://schemas.microsoft.com/ado/2008/09/edm";
  const EDM_5 = "http://schemas.microsoft.com/ado/2009/11/edm";
  const EDM_OLD = [EDM_1, EDM_2, EDM_3, EDM_4, EDM_5];
  const KNOWN_NAMESPACES = [
    EDMX,
    EDM,
    EDMX_V1,
    EDM_1,
    EDM_2,
    EDM_3,
    EDM_4,
    EDM_5,
  ];

  const elements = [{ local: "" }];

  /**
   * Check if element is allowed at this position
   * @param {Object} node The current XML node
   */
  function checkElementAllowed(node) {
    const parent = elements[0];
    elements.unshift({ local: node.local });

    // skip foreign markup in V2/V3
    if (result.$Version < "4.0" && !KNOWN_NAMESPACES.includes(node.uri)) {
      elements[0].foreignMarkup = true;
      return false;
    }

    const schema = SCHEMA[node.local] || {};
    const childName = schema.isExpression ? "expression" : node.local;
    const parentSchema = SCHEMA[parent.local];
    if (!parentSchema || !parentSchema[childName]) {
      throw new Error(
        `Element ${parent.local}, unexpected child: ${node.local}`
      );
    }

    // increment occurrence in parent, check against max occurrence in parent's schema
    if (!parent[childName]) parent[childName] = { occ: 1 };
    else parent[childName].occ += 1;
    if (parent[childName].occ > parentSchema[childName].max) {
      throw new Error(
        `Element ${childName}: ${parent[childName].occ} occurrences instead of at most ${parentSchema[childName].max}`
      );
    }

    // check node.uri for the correct XML namespace(s)
    if (
      !(
        (schema.x &&
          (((result.$Version >= "4.0" || schema.v2x) && node.uri === EDMX) ||
            (result.$Version < "4.0" && node.uri === EDMX_V1))) ||
        (!schema.x &&
          (((result.$Version >= "4.0" || schema.v2x || schema.isExpression) &&
            node.uri === EDM) ||
            (result.$Version < "4.0" && EDM_OLD.includes(node.uri))))
      )
    )
      throw new Error(
        `Element ${node.local}: invalid or missing XML namespace ${node.uri}`
      );

    return true;
  }

  /**
   * Check if element has the correct minimum number of each child element
   * @param {String} local The local element name
   */
  function checkElementComplete(local) {
    const element = elements.shift();

    if (element.foreignMarkup) return false;

    // check if all child elements have the correct minimum occurrence
    const required = Object.entries(SCHEMA[element.local] || {}).filter(
      ([, v]) => v.min && v.min > 0
    );
    for (const [name, constraints] of required) {
      if (strict && (!element[name] || element[name].occ < constraints.min))
        throw new Error(
          `Element ${local}, child element ${name}: ${
            element[name] ? element[name].occ : 0
          } occurrences instead of at least ${constraints.min}`
        );
    }

    return true;
  }

  /**
   * Add line number to model element, which is either an object or an object member
   * @param {Object} object The model element or wrapper object
   * @param {String} member The value to append or set
   */
  function addLineNumber(object, member) {
    if (lineNumbers) {
      if (member) object[member + "@parser.line"] = parser.line + 1;
      else object["@parser.line"] = parser.line + 1;
    }
  }

  /**
   * Update annotation value: append if array, replace otherwise
   * @param {Object} annotation The annotation to update
   * @param {Object} value The value to append or set
   */
  function updateValue(annotation, value) {
    if (Array.isArray(annotation.value)) annotation.value.push(value);
    else annotation.value = value;
  }

  /**
   * create JSON enum value from XML enum value
   * @param {string} name The path
   * @return {string} The normalized path
   */
  function enumValue(value) {
    return value
      .trim()
      .replace(/\s+/g, " ")
      .split(" ")
      .map((part) => {
        return part.substring(part.indexOf("/") + 1);
      })
      .join(",");
  }

  /**
   * alias-normalize path expression
   * @param {string} name The path
   * @return {string} The normalized path
   */
  function normalizePath(path) {
    return path
      .split("/")
      .map((part) => {
        const at = part.indexOf("@") + 1;
        const prefix = part.substring(0, at);
        const suffix = part.substring(at);
        const dot = suffix.lastIndexOf(".");
        return (
          prefix +
          (dot === -1
            ? suffix
            : (alias[suffix.substring(0, dot)] || suffix.substring(0, dot)) +
              suffix.substring(dot))
        );
      })
      .join("/");
  }

  /**
   * alias-normalize target path
   * @param {string} name The target
   * @return {string} The normalized target
   */
  function normalizeTarget(target) {
    const open = target.indexOf("(");
    const close = target.lastIndexOf(")");
    let path = open === -1 ? target : target.substring(0, open);
    let args = open === -1 ? "" : target.substring(open, close + 1);
    let rest = open === -1 ? "" : target.substring(close + 1);

    path = path
      .split("/")
      .map((part) => {
        const dot = part.lastIndexOf(".");
        return dot === -1
          ? part
          : (alias[part.substring(0, dot)] || part.substring(0, dot)) +
              part.substring(dot);
      })
      .join("/");

    if (args !== "") {
      let params = args.substring(1, args.length - 1);
      args =
        "(" +
        params
          .split(/,\s*/)
          .map((part) => {
            const collection = part.startsWith("Collection(");
            const type = collection
              ? part.substring(11, part.length - 1)
              : part;
            const dot = type.lastIndexOf(".");
            const normalizedType =
              (alias[type.substring(0, dot)] || type.substring(0, dot)) +
              type.substring(dot);
            return collection
              ? `Collection(${normalizedType})`
              : normalizedType;
          })
          .join(",") +
        ")";
    }

    return path + args + rest;
  }

  /**
   * a qualified name consists of a namespace or alias, a dot, and a simple name
   * @param {string} qualifiedName
   * @return {object} with components qualifier and name
   */
  function nameParts(qualifiedName) {
    const pos = qualifiedName.lastIndexOf(".");
    console.assert(pos > 0, "Invalid qualified name " + qualifiedName);
    return {
      qualifier: qualifiedName.substring(0, pos),
      name: qualifiedName.substring(pos + 1),
    };
  }

  /**
   * Get attribute value from an XML node
   * @param {Object} node The XML node
   * @return {string} The record with type
   */
  function record(node) {
    const record = {};
    if (node.attributes.Type) {
      const type = node.attributes.Type.value;
      const namespace = type.substring(0, type.lastIndexOf("."));
      const uri = namespaceUri[namespace];
      record[`@${result.$Version <= "4.0" ? "odata." : ""}type`] =
        (uri || "") + "#" + normalizePath(type);
    }
    return record;
  }

  /**
   * Check if attribute exists on XML node
   * @param {Object} node The XML node
   * @param {string} name The attribute name
   */
  function checkAttribute(node, name) {
    if (!node.attributes[name])
      throw new Error(`Element ${node.local}, missing attribute: ${name}`);
  }

  /**
   * Set attributes from an XML node
   * @param {Object} target The object to fill
   * @param {Object} node The XML node
   * @param {Array} extract An array of attribute names to extract
   * @param {Array} ignore An array of attribute names to ignore
   */
  function setAttributes(target, node, extract, ignore = []) {
    extract.forEach((name) => {
      if (node.attributes[name]) {
        switch (name) {
          case "Nullable":
            if (
              target.$Collection &&
              target.$Type === "Edm.EntityType" &&
              node.local === "ReturnType"
            )
              throw new Error(
                `Element ${node.local}, Type=Collection(Edm.EntityType) with Nullable attribute`
              );
            if (node.attributes[name].value === "true") target.$Nullable = true;
            break;
          case "Abstract":
          case "ContainsTarget":
          case "HasStream":
          case "IsBound":
          case "IsComposable":
          case "IsFlags":
          case "OpenType":
            if (node.attributes[name].value === "true")
              target["$" + name] = true;
            break;
          case "Action":
          case "Function":
            target["$" + name] = normalizePath(node.attributes[name].value);
            break;
          case "Alias":
          case "Name":
          case "Namespace":
          case "Partner":
          case "Property":
          case "Qualifier":
          case "TargetNamespace":
          case "Term":
          case "TermNamespace":
          case "UnderlyingType":
          case "Version":
            target["$" + name] = node.attributes[name].value;
            break;
          case "AppliesTo":
            target.$AppliesTo = node.attributes[name].value
              .trim()
              .replace(/\s+/g, " ")
              .split(" ");
            break;
          case "BaseTerm":
          case "BaseType":
          case "EntitySetPath":
          case "Extends":
            target["$" + name] = normalizeTarget(node.attributes[name].value);
            break;
          case "DefaultValue": {
            const value = node.attributes[name].value;
            const type = node.attributes.Type && node.attributes.Type.value;
            if (value === "null") target.$DefaultValue = null;
            else if (value === "true") target.$DefaultValue = true;
            else if (value === "false") target.$DefaultValue = false;
            else
              target.$DefaultValue =
                value === "" || isNaN(value) || type === "Edm.String"
                  ? value
                  : Number(value);
            break;
          }
          case "EntitySet": {
            let set = normalizeTarget(node.attributes[name].value);
            if (set.startsWith(normalizeTarget(result.$EntityContainer) + "/"))
              set = set.substring(set.indexOf("/") + 1);
            target.$EntitySet = set;
            break;
          }
          case "EntityType":
            target.$Type = normalizeTarget(node.attributes[name].value);
            break;
          case "MaxLength":
            if (node.attributes[name].value !== "max")
              target.$MaxLength = Number(node.attributes[name].value);
            break;
          case "Precision":
          case "SRID":
            target["$" + name] =
              node.attributes[name].value === "" ||
              isNaN(node.attributes[name].value)
                ? node.attributes[name].value
                : Number(node.attributes[name].value);
            break;
          case "Unicode":
            if (node.attributes[name].value === "false")
              target.$Unicode = false;
            break;
          case "IncludeInServiceDocument":
            if (
              node.local === "EntitySet" &&
              node.attributes[name].value === "false"
            )
              target.$IncludeInServiceDocument = false;
            else if (
              node.local === "FunctionImport" &&
              node.attributes[name].value === "true"
            )
              target.$IncludeInServiceDocument = true;
            break;
          case "Scale":
            if (
              node.attributes[name].value !== "variable" ||
              node.local === "Cast" ||
              node.local === "IsOf"
            )
              target.$Scale = isNaN(node.attributes[name].value)
                ? node.attributes[name].value
                : Number(node.attributes[name].value);
            break;
          case "ReturnType":
          case "Type": {
            let type = node.attributes[name].value;
            if (type.substring(0, 11) === "Collection(") {
              target.$Collection = true;
              type = type.substring(11, type.length - 1);
            }
            if (type !== "Edm.String") target.$Type = normalizeTarget(type);
            break;
          }
          case "AnnotationPath":
          case "ModelElementPath":
          case "NavigationPropertyPath":
          case "PropertyPath":
            target.value = normalizePath(node.attributes[name].value);
            break;
          case "Path":
            target.value = {
              $Path: normalizePath(node.attributes[name].value),
            };
            break;
          case "String":
            target.value = node.attributes[name].value.replace(
              /\r\n|\r(?!\n)/g,
              "\n"
            );
            break;
          case "Binary":
          case "Date":
          case "DateTimeOffset":
          case "Duration":
          case "Guid":
          case "TimeOfDay":
            target.value = node.attributes[name].value;
            break;
          case "Bool":
            target.value = node.attributes[name].value === "true";
            break;
          case "Decimal":
          case "Float":
          case "Int":
            target.value = isNaN(node.attributes[name].value)
              ? node.attributes[name].value
              : Number(node.attributes[name].value);
            break;
          case "EnumMember":
            target.value = enumValue(node.attributes[name].value);
            break;
          case "UrlRef":
            target.value = { $UrlRef: node.attributes[name].value };
            break;
        }
      } else {
        // Note: need to pass Type before Nullable, Precision, and Scale
        if (
          name === "Nullable" &&
          target.$Collection &&
          (node.local === "Property" || node.local === "Term") &&
          result.$Version >= "4.01"
        )
          throw new Error(
            `Element ${node.local}, Type=Collection without Nullable attribute`
          );
        else if (
          name === "Nullable" &&
          !target.$Collection &&
          node.local !== "Singleton" &&
          !(result.$Version < "4.0" && node.local === "Parameter")
        )
          target.$Nullable = true;
        else if (
          name === "Scale" &&
          (target.$Type === "Edm.Decimal" ||
            target.$UnderlyingType === "Edm.Decimal") &&
          node.local !== "Cast" &&
          node.local !== "IsOf"
        )
          target.$Scale = 0;
        else if (
          name === "Precision" &&
          ["Edm.DateTimeOffset", "Edm.DateTime"].includes(target.$Type)
        )
          target.$Precision = 0;
      }
    });
    if (
      node.local === "NavigationProperty" &&
      target.$Collection &&
      target.$Nullable
    ) {
      //TODO: console.warn
      delete target.$Nullable;
    }
    // check for unexpected attributes
    Object.values(node.attributes)
      .filter(
        (a) =>
          strict &&
          a.prefix === "" &&
          a.local !== "Name" &&
          !extract.includes(a.local) &&
          !ignore.includes(a.local)
      )
      .forEach((attribute) => {
        throw new Error(
          `Element ${node.local}, unexpected attribute: ${attribute.name}`
        );
      });
    if (annotations) {
      // copy "foreign markup" attributes as annotations
      // create $Reference
      Object.values(node.attributes)
        .filter(
          (a) =>
            a.prefix != "" &&
            a.prefix != "xmlns" &&
            a.uri !=
              "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata"
        )
        .forEach((attribute) => {
          if (!result.$Reference) result.$Reference = {};
          if (!result.$Reference[attribute.uri]) {
            result.$Reference[attribute.uri] = {
              $Include: [
                {
                  $Namespace: namespaceFromUri(attribute.uri),
                  $Alias: attribute.prefix,
                },
              ],
            };
          }
          target[
            `@${result.$Reference[attribute.uri].$Include[0].$Alias}.${
              attribute.local
            }`
          ] =
            attribute.value === "true"
              ? true
              : attribute.value === "false"
              ? false
              : attribute.value;
        });
    }
  }

  const SAP_V2_URI = "http://www.sap.com/Protocols/SAPData";
  const SAP_V2_SCHEMA = {
    "schema-version": {
      vocabulary: "Core",
      term: "SchemaVersion",
    },
  };
  const SAP_V2_ENTITY_SET = {
    creatable: {
      attrValue: "false",
      vocabulary: "Capabilities",
      term: "InsertRestrictions",
      value: { Insertable: false },
    },
    updatable: {
      attrValue: "false",
      vocabulary: "Capabilities",
      term: "UpdateRestrictions",
      value: { Updatable: false },
    },
    deletable: {
      attrValue: "false",
      vocabulary: "Capabilities",
      term: "DeleteRestrictions",
      value: { Deletable: false },
    },
    countable: {
      attrValue: "false",
      vocabulary: "Capabilities",
      term: "CountRestrictions",
      value: { Countable: false },
    },
    "requires-filter": {
      attrValue: "true",
      vocabulary: "Capabilities",
      term: "FilterRestrictions",
      value: { RequiresFilter: true },
    },
    pageable: {
      attrValue: "false",
      vocabulary: "Capabilities",
      becomes: [
        {
          term: "TopSupported",
          value: false,
        },
        {
          term: "SkipSupported",
          value: false,
        },
      ],
    },
  };

  /**
   * Interpret OData V2 annotation attributes
   * @param {Object} target The object to fill
   * @param {Object} node The XML node
   * @param {Array} annotationsMap A map of V2 annotation attributes and resulting V4 annotations
   */
  function v2Annotations(target, node, annotationsMap) {
    if (result.$Version !== "2.0") return;
    const attributes = Object.values(node.attributes).filter(
      (a) => a.uri === SAP_V2_URI
    );
    for (const attribute of attributes) {
      const anno = annotationsMap[attribute.local];
      if (
        !anno ||
        (anno.attrValue !== undefined && attribute.value !== anno.attrValue)
      )
        continue;
      preV4.needs[anno.vocabulary] = true;
      const becomes = anno.becomes ?? [{ term: anno.term, value: anno.value }];
      for (const b of becomes) {
        target[`@${alias[VOCABULARIES[anno.vocabulary].Namespace]}.${b.term}`] =
          b.value ?? attribute.value;
      }
    }
  }

  const FILTER_EXPRESSION_RESTRICTION = {
    "single-value": "SingleValue",
    "multi-value": "MultiValue",
    interval: "SingleRange",
  };

  /**
   * Interpret OData V2 annotation attributes on edm:Property elements
   * @param {Object} target The object to fill
   * @param {Object} node The XML node
   */
  function v2PropertyAnnotations(target, node) {
    if (result.$Version !== "2.0") return;
    let creatable = true;
    let updatable = true;
    for (const attribute of Object.values(node.attributes)) {
      if (attribute.uri !== SAP_V2_URI) continue;
      switch (attribute.local) {
        case "creatable":
          creatable = attribute.value === "true";
          break;
        case "updatable":
          updatable = attribute.value === "true";
          break;
        case "display-format":
          if (attribute.value === "Date" && target.$Type === "Edm.DateTime")
            target.$Type = "Edm.Date";
          break;
        case "validation-regexp":
          target[`@${alias[VOCABULARIES.Validation.Namespace]}.Pattern`] =
            attribute.value;
          preV4.needs.Validation = true;
          break;
        case "variable-scale":
          if (attribute.value === "true" && target.$Type === "Edm.Decimal")
            target.$Scale = "floating";
          break;
        case "filterable":
          if (attribute.value === "false") {
            preV4.needs.Capabilities = true;
            if (!preV4.nonFilterable[current.qualifiedTypeName])
              preV4.nonFilterable[current.qualifiedTypeName] = [];
            preV4.nonFilterable[current.qualifiedTypeName].push(
              node.attributes.Name.value
            );
          }
          break;
        case "filter-restriction":
          {
            const restriction = FILTER_EXPRESSION_RESTRICTION[attribute.value];
            if (restriction) {
              preV4.needs.Capabilities = true;
              if (!preV4.filterRestrictions[current.qualifiedTypeName])
                preV4.filterRestrictions[current.qualifiedTypeName] = {};
              preV4.filterRestrictions[current.qualifiedTypeName][
                node.attributes.Name.value
              ] = restriction;
            }
          }
          break;
        case "required-in-filter":
          if (attribute.value === "true") {
            preV4.needs.Capabilities = true;
            if (!preV4.requiredInFilter[current.qualifiedTypeName])
              preV4.requiredInFilter[current.qualifiedTypeName] = [];
            preV4.requiredInFilter[current.qualifiedTypeName].push(
              node.attributes.Name.value
            );
          }
          break;
        case "sortable":
          if (attribute.value === "false") {
            preV4.needs.Capabilities = true;
            if (!preV4.nonSortable[current.qualifiedTypeName])
              preV4.nonSortable[current.qualifiedTypeName] = [];
            preV4.nonSortable[current.qualifiedTypeName].push(
              node.attributes.Name.value
            );
          }
          break;
      }
    }
    if (!creatable && !updatable)
      target[`@${alias[VOCABULARIES.Core.Namespace]}.Computed`] = true;
    if (creatable && !updatable)
      target[`@${alias[VOCABULARIES.Core.Namespace]}.Immutable`] = true;
  }

  try {
    parser.write(xml).close();
  } catch (e) {
    throw enrichedException(e, xml, parser);
  }

  for (const [alias, vocabulary] of Object.entries(VOCABULARIES)) {
    if (preV4.needs[alias] && namespace[vocabulary.Namespace] === undefined) {
      if (!result.$Reference) result.$Reference = {};
      result.$Reference[vocabulary.Uri] = {
        $Include: [{ $Namespace: vocabulary.Namespace, $Alias: alias }],
      };
    }
  }

  const FILTER_RESTRICTIONS = `@${
    alias[VOCABULARIES.Capabilities.Namespace]
  }.FilterRestrictions`;
  const SORT_RESTRICTIONS = `@${
    alias[VOCABULARIES.Capabilities.Namespace]
  }.SortRestrictions`;
  for (const set of Object.values(current.container).filter(
    (c) => c.$Collection
  )) {
    if (preV4.nonFilterable[set.$Type]) {
      if (!set[FILTER_RESTRICTIONS]) set[FILTER_RESTRICTIONS] = {};
      set[FILTER_RESTRICTIONS].NonFilterableProperties =
        preV4.nonFilterable[set.$Type];
    }
    if (preV4.filterRestrictions[set.$Type]) {
      if (!set[FILTER_RESTRICTIONS]) set[FILTER_RESTRICTIONS] = {};
      set[FILTER_RESTRICTIONS].FilterExpressionRestrictions = Object.entries(
        preV4.filterRestrictions[set.$Type]
      ).map(([k, v]) => ({ Property: k, AllowedExpressions: v }));
    }
    if (preV4.requiredInFilter[set.$Type]) {
      if (!set[FILTER_RESTRICTIONS]) set[FILTER_RESTRICTIONS] = {};
      set[FILTER_RESTRICTIONS].RequiredProperties =
        preV4.requiredInFilter[set.$Type];
    }
    if (preV4.nonSortable[set.$Type]) {
      set[SORT_RESTRICTIONS] = {
        NonSortableProperties: preV4.nonSortable[set.$Type],
      };
    }
  }

  return result;
};

/**
 * Construct OData namespace from XML namespace URI
 * @param {string} uri The XML namespace name
 * @return {string} The constructed OData namespace
 */
function namespaceFromUri(uri) {
  return (
    uri
      // suppress scheme, slash becomes dot, numeric segment is prefixed with "n"
      .replace(/^.+:\/\//, "")
      .replace(/\//g, ".")
      .replace(/\.(?=[0-9])/g, ".n")
  );
}

function enrichedException(e, xml, parser) {
  e.parser = {
    construct: xml
      .toString()
      .substring(parser.startTagPosition - 1, parser.position),
    line: parser.line + 1, // sax parser counts from zero, and people and most editors count from 1
    column: parser.column,
  };
  return e;
}
