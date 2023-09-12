// @ts-nocheck
/**
 * Entity Data Model for OData
 *
 * Latest version: https://github.com/oasis-tcs/odata-openapi/blob/master/lib/edm.js
 */

module.exports = { nameParts };

module.exports.EDM = class {
  #elements = {}; // Map of namespace-qualified name to model element
  //TODO: stronger encapsulation: private members with getters?
  //TODO: use namespace-qualified names instead of document-local aliases
  boundOverloads = {}; // Map of action/function names to bound overloads
  derivedTypes = {}; // Map of type names to derived types
  //TODO: maps per document for document-local aliases
  #alias = {}; // Map of namespace or alias to alias
  namespace = { Edm: "Edm" }; // Map of namespace or alias to namespace
  namespaceUrl = {}; // Map of namespace to reference URL
  voc = {}; // Map of vocabulary and terms to qualified name in this CSDL

  /**
   * Add CSDL document to model
   * @param {object} csdl CSDL document
   * @param {Array} messages Warnings
   */
  //TODO: multi-document models
  //TODO: different aliases per document
  addDocument(csdl, messages) {
    this.#processIncludes(csdl);
    //TODO: different term maps per document, at least for Core, JSON, Validation
    vocabularies(this.voc, this.#alias);
    this.#processSchemas(csdl, messages);
    //TODO: only for first? document
    this.entityContainer = this.#elements[csdl.$EntityContainer];
  }

  /**
   * Extract included namespaces and their aliases
   * @param {object} csdl CSDL document
   */
  #processIncludes(csdl) {
    for (const [url, reference] of Object.entries(csdl.$Reference ?? {})) {
      for (const include of reference.$Include ?? []) {
        const qualifier = include.$Alias ?? include.$Namespace;
        this.#alias[include.$Namespace] = qualifier;
        this.namespace[qualifier] = include.$Namespace;
        this.namespace[include.$Namespace] = include.$Namespace;
        this.namespaceUrl[include.$Namespace] = url;
      }
    }
  }

  /**
   * Extract namespaces, aliases, model elements, bound overloads, and derived types
   * @param {object} csdl CSDL document
   * @param {Array} messages Warnings
   */
  #processSchemas(csdl, messages) {
    for (const [namespace, schema] of Object.entries(csdl)) {
      if (!isIdentifier(namespace)) continue;

      const isDefaultNamespace = schema[this.voc.Core.DefaultNamespace];
      const qualifier = schema.$Alias || namespace;
      this.#alias[namespace] = qualifier;
      this.namespace[qualifier] = namespace;
      this.namespace[namespace] = namespace;

      for (const [name, element] of Object.entries(schema)) {
        if (!isIdentifier(name)) continue;

        //TODO: copy element to avoid modifying input CSDL in processAnnotations
        //TODO: namespace-qualify any type references inside copy of element?
        //TODO: - $BaseType, property.$Type, $ReturnType.$Type, $Parameter[].$Type, resource.$Type, resource.$Action, resource.$Function
        //TODO: - embedded annotations
        //TODO: or inject reference to doc-local alias map?
        this.#elements[`${namespace}.${name}`] = element;

        if (Array.isArray(element)) {
          const qualifiedName = qualifier + "." + name;

          for (const overload of element) {
            if (!overload.$IsBound) continue;
            //TODO: this "-c" trick seems a bit hacky
            const type =
              overload.$Parameter[0].$Type +
              (overload.$Parameter[0].$Collection ? "-c" : "");
            if (!this.boundOverloads[type]) this.boundOverloads[type] = [];
            this.boundOverloads[type].push({
              name: isDefaultNamespace ? name : qualifiedName,
              overload: overload,
            });
          }
        }
      }
    }

    for (const [namespace, schema] of Object.entries(csdl)) {
      if (!isIdentifier(namespace)) continue;

      this.#processAnnotations(schema.$Annotations ?? {}, messages);

      for (const [name, element] of Object.entries(schema)) {
        if (!isIdentifier(name)) continue;

        if (element.$BaseType) {
          const base = this.namespaceQualifiedName(element.$BaseType);
          if (!this.derivedTypes[base]) this.derivedTypes[base] = [];
          this.derivedTypes[base].push(namespace + "." + name);
        }
      }
    }
  }

  /**
   * Inject annotations with external targeting into target model elements
   * @param {object} externalAnnotations Annotations with external targeting
   * @param {Array} messages Warnings
   */
  #processAnnotations(externalAnnotations, messages) {
    for (const [target, annotations] of Object.entries(externalAnnotations)) {
      const segments = target.split("/");
      const open = segments[0].indexOf("(");
      let element;
      if (open == -1) {
        element = this.element(segments[0]);
      } else {
        element = this.element(segments[0].substring(0, open));
        let args = segments[0].substring(open + 1, segments[0].length - 1);
        element = element?.find(
          (overload) =>
            (overload.$Kind == "Action" &&
              overload.$IsBound != true &&
              args == "") ||
            (overload.$Kind == "Action" &&
              args ==
                (overload.$Parameter?.[0].$Collection
                  ? `Collection(${overload.$Parameter[0].$Type})`
                  : overload.$Parameter[0].$Type || "")) ||
            args ==
              (overload.$Parameter || [])
                .map((p) => {
                  const type = p.$Type || "Edm.String";
                  return p.$Collection ? `Collection(${type})` : type;
                })
                .join(",")
        );
      }
      if (!element) {
        messages.push(`Invalid annotation target '${target}'`);
      } else if (Array.isArray(element)) {
        messages.push(
          `Ignoring annotations targeting all overloads of '${target}'`
        );
        //TODO: action or function:
        //- loop over all overloads
        //- if there are more segments, a parameter or the return type is targeted
      } else {
        switch (segments.length) {
          case 1:
            Object.assign(element, annotations);
            break;
          case 2:
            if (["Action", "Function"].includes(element.$Kind)) {
              if (segments[1] == "$ReturnType") {
                if (element.$ReturnType)
                  Object.assign(element.$ReturnType, annotations);
              } else {
                const parameter = element.$Parameter.find(
                  (p) => p.$Name == segments[1]
                );
                Object.assign(parameter, annotations);
              }
            } else {
              if (typeof element[segments[1]] === "object") {
                Object.assign(element[segments[1]], annotations);
              } else if (element.$Kind !== "EnumType") {
                messages.push(`Invalid annotation target '${target}'`);
              }
            }
            break;
          default:
            messages.push("More than two annotation target path segments");
        }
      }
    }
  }

  /**
   * Find model element by qualified name
   * @param {string} qualifiedName Qualified name of model element
   * @return {object} Model element
   */
  element(qualifiedName) {
    return this.#elements[this.namespaceQualifiedName(qualifiedName)];
  }

  /**
   * a qualified name consists of a namespace or alias, a dot, and a simple name
   * @param {string} qualifiedName
   * @return {string} namespace-qualified name
   */
  namespaceQualifiedName(qualifiedName) {
    let np = nameParts(qualifiedName);
    return this.namespace[np.qualifier] + "." + np.name;
  }

  /**
   * Key of entity type
   * @param {object} entityType Entity Type object
   * @return {array} Key of entity type or empty array
   */
  key(entityType) {
    let type = entityType;
    let _key = null;
    while (type) {
      _key = type.$Key;
      if (_key || !type.$BaseType) break;
      type = this.element(type.$BaseType);
    }
    return _key ?? [];
  }

  /**
   * All resources of the model's entity container
   * @return {Array} Array of [name, resourceObject] arrays
   */
  get resources() {
    return Object.entries(this.entityContainer ?? {}).filter(([name]) =>
      isIdentifier(name)
    );
  }

  /**
   * All structured types onf the model
   * @return {Array} Array of [name, typeObject] arrays
   */
  get structuredTypes() {
    return Object.entries(this.#elements).filter(([, element]) =>
      ["EntityType", "ComplexType"].includes(element.$Kind)
    );
  }

  /**
   * All properties of a structured type including inherited ones
   * @param {object} type Structured type
   * @return {object} Map of properties
   */
  propertiesMapOfStructuredType(type) {
    const properties =
      type && type.$BaseType
        ? this.propertiesMapOfStructuredType(this.element(type.$BaseType))
        : {};
    if (type) {
      Object.keys(type)
        .filter((name) => isIdentifier(name))
        .forEach((name) => {
          properties[name] = type[name];
        });
    }
    return properties;
  }

  /**
   * All properties of a structured type including inherited ones
   * @param {object} type Structured type
   * @return {Array} Array of [name, propertyObject] arrays
   */
  propertiesOfStructuredType(type) {
    return Object.entries(this.propertiesMapOfStructuredType(type));
  }

  /**
   * Direct properties of a structured type excluding inherited ones
   * @param {object} type Structured type
   * @return {Array} Array of [name, propertyObject] arrays
   */
  directPropertiesOfStructuredType(type) {
    return Object.entries(type).filter(([name]) => isIdentifier(name));
  }

  /**
   * Members of an enumeration type
   * @param {object} type Structured type
   * @return {Array} Array of [name, propertyObject] arrays
   */
  enumTypeMembers(type) {
    return Object.entries(type).filter(([name]) => isIdentifier(name));
  }
};

/**
 * an identifier does not start with $ and does not contain @
 * @param {string} name
 * @return {boolean} name is an identifier
 */
function isIdentifier(name) {
  return !name.startsWith("$") && !name.includes("@");
}

/**
 * a qualified name consists of a namespace or alias, a dot, and a simple name
 * @param {string} qualifiedName
 * @return {object} with components qualifier and name
 */
function nameParts(qualifiedName) {
  const pos = qualifiedName.lastIndexOf(".");
  return {
    qualifier: qualifiedName.substring(0, pos),
    name: qualifiedName.substring(pos + 1),
  };
}

/**
 * Construct map of qualified term names
 * @param {object} voc Map of vocabularies and terms
 * @param {object} alias Map of namespace or alias to alias
 */
function vocabularies(voc, alias) {
  const terms = {
    Aggregation: ["ApplySupported"],
    Authorization: ["Authorizations", "SecuritySchemes"],
    Capabilities: [
      "BatchSupport",
      "BatchSupported",
      "ChangeTracking",
      "CountRestrictions",
      "DeleteRestrictions",
      "DeepUpdateSupport",
      "ExpandRestrictions",
      "FilterRestrictions",
      "IndexableByKey",
      "InsertRestrictions",
      "KeyAsSegmentSupported",
      "NavigationRestrictions",
      "OperationRestrictions",
      "ReadRestrictions",
      "SearchRestrictions",
      "SelectSupport",
      "SkipSupported",
      "SortRestrictions",
      "TopSupported",
      "UpdateRestrictions",
    ],
    Core: [
      "AcceptableMediaTypes",
      "Computed",
      "DefaultNamespace",
      "Description",
      "Example",
      "Immutable",
      "LongDescription",
      "OptionalParameter",
      "Permissions",
      "SchemaVersion",
    ],
    JSON: ["Schema"],
    Validation: ["AllowedValues", "Exclusive", "Maximum", "Minimum", "Pattern"],
  };

  for (const vocab of Object.keys(terms)) {
    voc[vocab] = {};
    const namespace = `Org.OData.${vocab}.V1`;
    for (const term of terms[vocab]) {
      voc[vocab][term] = `@${alias[namespace] || namespace}.${term}`;
    }
  }
}
