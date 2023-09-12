// @ts-nocheck
/**
 * Converts OData CSDL JSON to OpenAPI 3.0.2
 *
 * Latest version: https://github.com/oasis-tcs/odata-openapi/blob/master/lib/csdl2openapi.js
 */

const { EDM, nameParts } = require("./edm");
const { resourceDiagram } = require("./diagram");

//TODO
// - Core.Example for complex types
// - reduce number of loops over schemas
// - inject $$name into each model element to make parameter passing easier?
// - allow passing additional files for referenced documents
// - delta: headers Prefer and Preference-Applied
// - inline definitions for Edm.* to make OpenAPI documents self-contained
// - $Extends for entity container: include /paths from referenced container
// - both "clickable" and freestyle $expand, $select, $orderby - does not work yet, open issue for Swagger UI
// - system query options for actions/functions/imports depending on $Collection
// - 200 response for PATCH
// - ETag for GET / If-Match for PATCH and DELETE depending on @Core.OptimisticConcurrency
// - CountRestrictions for GET collection-valued (containment) navigation - https://issues.oasis-open.org/browse/ODATA-1300
// - InsertRestrictions/NonInsertableProperties
// - InsertRestrictions/NonInsertableNavigationProperties
// - UpdateRestrictions/NonUpdatableProperties
// - UpdateRestrictions/NonUpdatableNavigationProperties
// - see //TODO comments below

const SUFFIX = {
  read: "",
  create: "-create",
  update: "-update",
};

const TITLE_SUFFIX = {
  "": "",
  "-create": " (for create)",
  "-update": " (for update)",
};

/**
 * Construct an OpenAPI description from a CSDL document
 * @param {object} csdl CSDL document
 * @param {object} options Optional parameters
 * @return {object} OpenAPI description
 */
module.exports.csdl2openapi = function (
  csdl,
  {
    scheme = "https",
    host = "localhost",
    basePath = "/service-root",
    diagram = false,
    openapiVersion = "3.0.2",
    maxLevels = 4,
    messages = [],
    skipBatchPath = false,
    defaultTitle = null,
    defaultDescription = null,
  } = {}
) {
  const serviceRoot = scheme + "://" + host + basePath;
  const queryOptionPrefix = csdl.$Version <= "4.0" ? "$" : "";
  const typesToInline = {}; // filled in schema() and used in inlineTypes()
  const oas31 = openapiVersion > "3.0.3";
  const requiredSchemas = { list: [], used: {}, entityReferenceNeeded: false };

  const model = new EDM();
  model.addDocument(csdl, messages);

  const voc = model.voc;
  const entityContainer = model.entityContainer ?? {};
  const keyAsSegment = entityContainer[voc.Capabilities.KeyAsSegmentSupported];
  const applySupported = entityContainer[voc.Aggregation.ApplySupported];
  const deepUpdate =
    entityContainer[voc.Capabilities.DeepUpdateSupport] &&
    entityContainer[voc.Capabilities.DeepUpdateSupport].Supported;

  const openapi = {
    openapi: openapiVersion,
    info: info(csdl, entityContainer),
    servers: servers(serviceRoot),
    tags: tags(),
    paths: paths(model.entityContainer),
    components: components(!!model.entityContainer),
  };
  if (!model.entityContainer) {
    delete openapi.servers;
    delete openapi.tags;
  }

  securitySchemes(openapi.components, entityContainer);
  security(openapi, model.entityContainer);

  return openapi;

  /**
   * Construct the Info Object
   * @param {object} csdl CSDL document
   * @param {object} entityContainer Entity Container object
   * @return {object} Info Object
   */
  function info(csdl, entityContainer) {
    const containerNamespace =
      csdl.$EntityContainer && nameParts(csdl.$EntityContainer).qualifier;
    const containerSchema = csdl[containerNamespace] ?? {};
    const description =
      (entityContainer[voc.Core.LongDescription] ||
        containerSchema[voc.Core.LongDescription] ||
        defaultDescription ||
        "This service is located at [" +
          serviceRoot +
          "/](" +
          serviceRoot.replace(/\(/g, "%28").replace(/\)/g, "%29") +
          "/)") + (diagram ? resourceDiagram(model) : "");
    return {
      title:
        entityContainer[voc.Core.Description] ||
        containerSchema[voc.Core.Description] ||
        defaultTitle ||
        (csdl.$EntityContainer
          ? "Service for namespace " + containerNamespace
          : "OData CSDL document"),
      description: csdl.$EntityContainer ? description : "",
      version: containerSchema[voc.Core.SchemaVersion] ?? "",
    };
  }

  /**
   * Construct an array of Server Objects
   * @param {string} serviceRoot Service root URL
   * @return {Array} The list of servers
   */
  function servers(serviceRoot) {
    return [{ url: serviceRoot }];
  }

  /**
   * Construct an array of Tag Objects from the entity container
   * @param {object} container The entity container
   * @return {Array} The list of tags
   */
  function tags() {
    const tags = [];
    for (const [name, resource] of model.resources) {
      // all entity sets and singletons
      if (!resource.$Type) continue;
      const tag = { name };
      const type = model.element(resource.$Type);
      const description =
        resource[voc.Core.Description] || (type && type[voc.Core.Description]);
      if (description) tag.description = description;
      tags.push(tag);
    }
    return tags;
  }

  /**
   * Construct the Paths Object from the entity container
   * @param {object} container Entity container
   * @return {object} Paths Object
   */
  function paths(container) {
    const paths = {};
    for (const [name, child] of model.resources) {
      if (child.$Type) {
        pathItems(
          paths,
          "/" + name,
          [],
          // entity sets and singletons are almost containment navigation properties
          { ...child, $ContainsTarget: true },
          child,
          name,
          name,
          child,
          0,
          ""
        );
      } else if (child.$Action) {
        pathItemActionImport(paths, name, child);
      } else if (child.$Function) {
        pathItemFunctionImport(paths, name, child);
      } else {
        messages.push("Unrecognized entity container child: " + name);
      }
    }
    if (model.resources.length > 0) pathItemBatch(paths, container);
    return paths;
  }

  /**
   * Add path and Path Item Object for a navigation segment
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} element Model element of navigation segment
   * @param {object} root Root model element
   * @param {string} sourceName Name of path source
   * @param {string} targetName Name of path target
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {string} navigationPath Path for finding navigation restrictions
   */
  function pathItems(
    paths,
    prefix,
    prefixParameters,
    element,
    root,
    sourceName,
    targetName,
    target,
    level,
    navigationPath
  ) {
    const name = prefix.substring(prefix.lastIndexOf("/") + 1);
    const type = model.element(element.$Type);
    const pathItem = {};
    const restrictions = navigationPropertyRestrictions(root, navigationPath);
    const nonExpandable = nonExpandableProperties(root, navigationPath);

    paths[prefix] = pathItem;
    if (prefixParameters.length > 0) pathItem.parameters = prefixParameters;

    operationRead(
      pathItem,
      element,
      name,
      sourceName,
      targetName,
      target,
      level,
      restrictions,
      false,
      nonExpandable
    );
    if (
      element.$Collection &&
      (element.$ContainsTarget || (level < 2 && target))
    ) {
      operationCreate(
        pathItem,
        element,
        name,
        sourceName,
        targetName,
        target,
        level,
        restrictions
      );
    }

    if (element.$ContainsTarget) {
      pathItemsForBoundOperations(
        paths,
        prefix,
        prefixParameters,
        element,
        sourceName
      );

      if (element.$Collection) {
        if (level < maxLevels)
          pathItemsWithKey(
            paths,
            prefix,
            prefixParameters,
            element,
            root,
            sourceName,
            targetName,
            target,
            level,
            navigationPath,
            restrictions,
            nonExpandable
          );
      } else {
        operationUpdate(
          pathItem,
          element,
          name,
          sourceName,
          target,
          level,
          restrictions
        );
        if (element.$Nullable) {
          operationDelete(
            pathItem,
            element,
            name,
            sourceName,
            target,
            level,
            restrictions
          );
        }
        pathItemsForBoundOperations(
          paths,
          prefix,
          prefixParameters,
          element,
          sourceName
        );
        pathItemsWithNavigation(
          paths,
          prefix,
          prefixParameters,
          type,
          root,
          sourceName,
          level,
          navigationPath
        );
      }
    }

    if (Object.keys(pathItem).filter((i) => i !== "parameters").length === 0)
      delete paths[prefix];

    if (element.$Collection) {
      pathItemForFilterSegment(
        paths,
        prefix,
        prefixParameters,
        element,

        sourceName,

        target,
        level,
        restrictions
      );
    }
  }

  /**
   * Find navigation restrictions for a navigation path
   * @param {object} root Root model element
   * @param {string} navigationPath Path for finding navigation restrictions
   * @return Navigation property restrictions of navigation segment
   */
  function navigationPropertyRestrictions(root, navigationPath) {
    const navigationRestrictions =
      root[voc.Capabilities.NavigationRestrictions] || {};
    return (
      (navigationRestrictions.RestrictedProperties || []).find(
        (item) => item.NavigationProperty == navigationPath
      ) || {}
    );
  }

  /**
   * Find non-expandable properties for a navigation path
   * @param {object} root Root model element
   * @param {string} navigationPath Path for finding navigation restrictions
   * @return Navigation property restrictions of navigation segment
   */
  function nonExpandableProperties(root, navigationPath) {
    const expandRestrictions = root[voc.Capabilities.ExpandRestrictions] || {};
    const prefix = navigationPath.length === 0 ? "" : navigationPath + "/";
    const from = prefix.length;
    const nonExpandable = [];
    for (const path of expandRestrictions.NonExpandableProperties || []) {
      if (path.startsWith(prefix)) {
        nonExpandable.push(path.substring(from));
      }
    }
    return nonExpandable;
  }

  /**
   * Add path and Path Item Object for a navigation segment with key
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} element Model element of navigation segment
   * @param {object} root Root model element
   * @param {string} sourceName Name of path source
   * @param {string} targetName Name of path target
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {string} navigationPath Path for finding navigation restrictions
   * @param {object} restrictions Navigation property restrictions of navigation segment
   * @param {array} nonExpandable Non-expandable navigation properties
   */
  function pathItemsWithKey(
    paths,
    prefix,
    prefixParameters,
    element,
    root,
    sourceName,
    targetName,
    target,
    level,
    navigationPath,
    restrictions,
    nonExpandable
  ) {
    const targetIndexable =
      target == null || target[voc.Capabilities.IndexableByKey] != false;
    if (
      restrictions.IndexableByKey == true ||
      (restrictions.IndexableByKey != false && targetIndexable)
    ) {
      const name = prefix.substring(prefix.lastIndexOf("/") + 1);
      const type = model.element(element.$Type);
      const key = entityKey(type, level);
      if (key.parameters.length > 0) {
        const path = prefix + key.segment;
        const parameters = prefixParameters.concat(key.parameters);
        const pathItem = { parameters: parameters };
        paths[path] = pathItem;

        operationRead(
          pathItem,
          element,
          name,
          sourceName,
          targetName,
          target,
          level,
          restrictions,
          true,
          nonExpandable
        );
        operationUpdate(
          pathItem,
          element,
          name,
          sourceName,
          target,
          level,
          restrictions
        );
        operationDelete(
          pathItem,
          element,
          name,
          sourceName,
          target,
          level,
          restrictions
        );
        if (
          Object.keys(pathItem).filter((i) => i !== "parameters").length === 0
        )
          delete paths[path];

        pathItemForMediaResource(
          paths,
          path,
          parameters,
          type,
          name,
          sourceName
        );
        pathItemsForBoundOperations(
          paths,
          path,
          parameters,
          element,
          sourceName,
          true
        );
        pathItemsWithNavigation(
          paths,
          path,
          parameters,
          type,
          root,
          sourceName,
          level,
          navigationPath
        );
      }
    }
  }

  /**
   * Construct Operation Object for create
   * @param {object} pathItem Path Item Object to augment
   * @param {object} element Model element of navigation segment
   * @param {string} name Name of navigation segment
   * @param {string} sourceName Name of path source
   * @param {string} targetName Name of path target
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function operationCreate(
    pathItem,
    element,
    name,
    sourceName,
    targetName,
    target,
    level,
    restrictions
  ) {
    const insertRestrictions =
      restrictions.InsertRestrictions ||
      (target && target[voc.Capabilities.InsertRestrictions]) ||
      {};

    if (insertRestrictions.Insertable !== false) {
      const type = model.element(element.$Type);
      const hasStream = type && type.$HasStream;
      pathItem.post = {
        summary:
          insertRestrictions.Description ||
          "Add new entity to " + (level > 0 ? "related " : "") + name,
        tags: [sourceName],
        requestBody: {
          description:
            (type && type[voc.Core.Description]) ||
            (hasStream ? "New media resource" : "New entity"),
          required: true,
          content: hasStream
            ? { "*/*": { schema: { type: "string", format: "binary" } } }
            : {
                "application/json": {
                  schema: ref(element.$Type, SUFFIX.create),
                },
              },
        },
        responses: response(
          201,
          "Created entity",
          { $Type: element.$Type },
          insertRestrictions.ErrorResponses
        ),
      };
      if (insertRestrictions.LongDescription)
        pathItem.post.description = insertRestrictions.LongDescription;
      if (targetName && sourceName != targetName)
        pathItem.post.tags.push(targetName);

      customParameters(pathItem.post, insertRestrictions);
    }
  }

  /**
   * Construct Operation Object for read
   * @param {object} pathItem Path Item Object to augment
   * @param {object} element Model element of navigation segment
   * @param {string} name Name of navigation segment
   * @param {string} sourceName Name of path source
   * @param {string} targetName Name of path target
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {object} restrictions Navigation property restrictions of navigation segment
   * @param {boolean} byKey Read by key
   * @param {array} nonExpandable Non-expandable navigation properties
   */
  function operationRead(
    pathItem,
    element,
    name,
    sourceName,
    targetName,
    target,
    level,
    restrictions,
    byKey,
    nonExpandable
  ) {
    const targetRestrictions = target?.[voc.Capabilities.ReadRestrictions];
    const readRestrictions =
      restrictions.ReadRestrictions || targetRestrictions || {};
    const readByKeyRestrictions = readRestrictions.ReadByKeyRestrictions;
    let readable = true;
    if (
      byKey &&
      readByKeyRestrictions &&
      readByKeyRestrictions.Readable !== undefined
    )
      readable = readByKeyRestrictions.Readable;
    else if (readRestrictions.Readable !== undefined)
      readable = readRestrictions.Readable;

    if (readable) {
      let descriptions =
        (level == 0 ? targetRestrictions : restrictions.ReadRestrictions) || {};
      if (byKey) descriptions = descriptions.ReadByKeyRestrictions || {};

      const collection = !byKey && element.$Collection;
      const operation = {
        summary:
          descriptions.Description ||
          "Get " +
            (byKey
              ? "entity from "
              : element.$Collection
              ? "entities from "
              : "") +
            (level > 0 ? "related " : "") +
            name +
            (byKey ? " by key" : ""),
        tags: [sourceName],
        parameters: [],
        responses: response(
          200,
          "Retrieved entit" + (collection ? "ies" : "y"),
          { $Type: element.$Type, $Collection: collection },
          byKey
            ? readByKeyRestrictions?.ErrorResponses
            : readRestrictions?.ErrorResponses
        ),
      };
      const deltaSupported =
        element[voc.Capabilities.ChangeTracking] &&
        element[voc.Capabilities.ChangeTracking].Supported;
      if (!byKey && deltaSupported) {
        operation.responses[200].content["application/json"].schema.properties[
          "@odata.deltaLink"
        ] = {
          type: "string",
          example:
            basePath +
            "/" +
            name +
            "?$deltatoken=opaque server-generated token for fetching the delta",
        };
      }
      if (descriptions.LongDescription)
        operation.description = descriptions.LongDescription;
      if (target && sourceName != targetName) operation.tags.push(targetName);

      customParameters(
        operation,
        byKey ? readByKeyRestrictions || readRestrictions : readRestrictions
      );

      if (collection) {
        optionTop(operation.parameters, target, restrictions);
        optionSkip(operation.parameters, target, restrictions);
        if (csdl.$Version >= "4.0")
          optionSearch(operation.parameters, target, restrictions);
        optionFilter(operation.parameters, target, restrictions);
        optionCount(operation.parameters, target);
        optionOrderBy(operation.parameters, element, target, restrictions);
      }

      optionSelect(operation.parameters, element, target, restrictions);
      optionExpand(operation.parameters, element, target, nonExpandable);

      if (collection) {
        optionApply(operation.parameters);
      }
      pathItem.get = operation;
    }
  }

  /**
   * Add custom headers and query options
   * @param {object} operation Operation object to augment
   * @param {object} restrictions Restrictions for operation
   */
  function customParameters(operation, restrictions) {
    if (
      !operation.parameters &&
      (restrictions.CustomHeaders || restrictions.CustomQueryOptions)
    )
      operation.parameters = [];
    for (const custom of restrictions.CustomHeaders || []) {
      operation.parameters.push(customParameter(custom, "header"));
    }

    for (const custom of restrictions.CustomQueryOptions || []) {
      operation.parameters.push(customParameter(custom, "query"));
    }
  }

  /**
   * Construct custom parameter
   * @param {object} custom custom parameter in OData format
   * @param {string} location "header" or "query"
   */
  function customParameter(custom, location) {
    let schema = jsonSchema(custom);
    if (!schema) schema = { type: "string" };
    if (custom.DocumentationURL)
      schema.externalDocs = { url: custom.DocumentationURL };
    //TODO: Examples in schema

    return {
      name: custom.Name,
      in: location,
      required: custom.Required || false,
      ...(custom.Description && { description: custom.Description }),
      schema,
    };
  }

  /**
   * Add parameter for query option $apply
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionApply(parameters) {
    if (applySupported) {
      parameters.push({
        $ref: "#/components/parameters/apply",
      });
    }
  }

  /**
   * Add parameter for query option $count
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   */
  function optionCount(parameters, target) {
    const targetRestrictions =
      target && target[voc.Capabilities.CountRestrictions];
    const targetCountable =
      target == null ||
      targetRestrictions == null ||
      targetRestrictions.Countable !== false;

    if (targetCountable) {
      parameters.push({
        $ref: "#/components/parameters/count",
      });
    }
  }

  /**
   * Add parameter for query option $expand
   * @param {Array} parameters Array of parameters to augment
   * @param {object} element Model element of navigation segment
   * @param {string} target Target container child of path
   * @param {array} nonExpandable Non-expandable navigation properties
   */
  function optionExpand(parameters, element, target, nonExpandable) {
    const targetRestrictions =
      target && target[voc.Capabilities.ExpandRestrictions];
    const supported =
      targetRestrictions == null || targetRestrictions.Expandable != false;
    if (supported) {
      const expandItems = ["*"].concat(
        navigationPaths(element).filter((path) => !nonExpandable.includes(path))
      );
      if (expandItems.length > 1) {
        if (csdl.$Version === "2.0") expandItems.shift(1);
        parameters.push({
          name: queryOptionPrefix + "expand",
          description:
            (targetRestrictions && targetRestrictions[voc.Core.Description]) ||
            `Expand related entities, see ${
              csdl.$Version === "2.0"
                ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
                : "[Expand](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionexpand)"
            }`,
          in: "query",
          explode: false,
          schema: {
            type: "array",
            uniqueItems: true,
            items: {
              type: "string",
              enum: expandItems,
            },
          },
        });
      }
    }
  }

  /**
   * Collect navigation paths of a navigation segment and its potentially structured components
   * @param {object} element Model element of navigation segment
   * @param {string} prefix Navigation prefix
   * @param {integer} level Number of navigation segments so far
   * @return {Array} Array of navigation property paths
   */
  function navigationPaths(element, prefix = "", level = 0) {
    const paths = [];
    const type = model.element(element.$Type);
    for (const [key, property] of model.propertiesOfStructuredType(type)) {
      if (property.$Kind == "NavigationProperty") {
        paths.push(prefix + key);
      } else if (property.$Type && level < maxLevels) {
        paths.push(...navigationPaths(property, prefix + key + "/", level + 1));
      }
    }
    return paths;
  }

  /**
   * Add parameter for query option $filter
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionFilter(parameters, target, restrictions) {
    const filterRestrictions =
      restrictions.FilterRestrictions ||
      (target && target[voc.Capabilities.FilterRestrictions]) ||
      {};

    if (filterRestrictions.Filterable !== false) {
      const filter = {
        name: queryOptionPrefix + "filter",
        description:
          filterRestrictions[voc.Core.Description] ||
          `Filter items by property values, see ${
            csdl.$Version === "2.0"
              ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
              : "[Filtering](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionfilter)"
          }`,
        in: "query",
        schema: {
          type: "string",
        },
      };
      if (filterRestrictions.RequiresFilter) filter.required = true;
      if (filterRestrictions.RequiredProperties) {
        filter.description += "\n\nRequired filter properties:";
        for (const item of filterRestrictions.RequiredProperties) {
          filter.description += "\n- " + item;
        }
      }
      parameters.push(filter);
    }
  }

  /**
   * Add parameter for query option $orderby
   * @param {Array} parameters Array of parameters to augment
   * @param {object} element Model element of navigation segment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionOrderBy(parameters, element, target, restrictions) {
    const sortRestrictions =
      restrictions.SortRestrictions ||
      (target && target[voc.Capabilities.SortRestrictions]) ||
      {};

    if (sortRestrictions.Sortable !== false) {
      const nonSortable = {};
      for (const name of sortRestrictions.NonSortableProperties || []) {
        nonSortable[name] = true;
      }
      const orderbyItems = [];
      for (const property of primitivePaths(element)) {
        if (nonSortable[property]) continue;
        orderbyItems.push(property);
        orderbyItems.push(property + " desc");
      }
      if (orderbyItems.length > 0) {
        parameters.push({
          name: queryOptionPrefix + "orderby",
          description:
            sortRestrictions[voc.Core.Description] ||
            `Order items by property values, see ${
              csdl.$Version === "2.0"
                ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
                : "[Sorting](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionorderby)"
            }`,
          in: "query",
          explode: false,
          schema: {
            type: "array",
            uniqueItems: true,
            items: {
              type: "string",
              enum: orderbyItems,
            },
          },
        });
      }
    }
  }

  /**
   * Collect primitive paths of a navigation segment and its potentially structured components
   * @param {object} element Model element of navigation segment
   * @param {string} prefix Navigation prefix
   * @return {Array} Array of primitive property paths
   */
  function primitivePaths(element, prefix = "") {
    const paths = [];
    const elementType = model.element(element.$Type);

    if (!elementType) {
      messages.push(`Unknown type for element: ${JSON.stringify(element)}`);
      return paths;
    }

    const propsOfType = model.propertiesOfStructuredType(elementType);

    const ignore = propsOfType.filter(
      ([, type]) =>
        type.$Kind !== "NavigationProperty" &&
        type.$Type &&
        !type.$Type.startsWith("Edm.") &&
        !model.element(type.$Type)
    );

    // Keep old logging
    for (const entry of ignore) {
      messages.push(`Unknown type for element: ${JSON.stringify(entry)}`);
    }

    const properties = propsOfType
      .filter((entry) => entry[1].$Kind !== "NavigationProperty")
      .filter((entry) => !ignore.includes(entry))
      .map(entryToProperty({ path: prefix, typeRefChain: [] }));

    for (let i = 0; i < properties.length; i++) {
      const property = properties[i];
      if (!property.isComplex) {
        paths.push(property.path);
        continue;
      }

      const typeRefChainTail =
        property.typeRefChain[property.typeRefChain.length - 1];

      // Allow full cycle to be shown (0) times
      if (
        property.typeRefChain.filter((_type) => _type === typeRefChainTail)
          .length > 1
      ) {
        // messages.push(`Cycle detected ${property.typeRefChain.join("->")}`);
        continue;
      }

      const expanded = Object.entries(property.properties)
        .filter((property) => property[1].$Kind !== "NavigationProperty")
        .map(entryToProperty(property));
      properties.splice(i + 1, 0, ...expanded);
    }

    return paths;
  }

  function entryToProperty(parent) {
    return function (entry) {
      const key = entry[0];
      const property = entry[1];
      const propertyType = property.$Type && model.element(property.$Type);

      if (
        propertyType &&
        propertyType.$Kind &&
        propertyType.$Kind === "ComplexType"
      ) {
        return {
          properties: model.propertiesMapOfStructuredType(propertyType),
          path: `${parent.path}${key}/`,
          typeRefChain: parent.typeRefChain.concat(property.$Type),
          isComplex: true,
        };
      }

      return {
        properties: {},
        path: `${parent.path}${key}`,
        typeRefChain: [],
        isComplex: false,
      };
    };
  }

  /**
   * Add parameter for query option $search
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionSearch(parameters, target, restrictions) {
    const searchRestrictions =
      restrictions.SearchRestrictions ||
      (target && target[voc.Capabilities.SearchRestrictions]) ||
      {};

    if (searchRestrictions.Searchable !== false) {
      if (searchRestrictions[voc.Core.Description]) {
        parameters.push({
          name: queryOptionPrefix + "search",
          description: searchRestrictions[voc.Core.Description],
          in: "query",
          schema: { type: "string" },
        });
      } else {
        parameters.push({ $ref: "#/components/parameters/search" });
      }
    }
  }

  /**
   * Add parameter for query option $select
   * @param {Array} parameters Array of parameters to augment
   * @param {object} element Model element of navigation segment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionSelect(parameters, element, target, restrictions) {
    const selectSupport =
      restrictions.SelectSupport ||
      (target && target[voc.Capabilities.SelectSupport]) ||
      {};

    if (selectSupport.Supported !== false) {
      const type = model.element(element.$Type) || {};
      const selectItems = [];

      for (const [name, property] of model.propertiesOfStructuredType(type)) {
        if (property.$Kind === "NavigationProperty" && csdl.$Version !== "2.0")
          continue;
        selectItems.push(name);
      }

      if (selectItems.length > 0) {
        parameters.push({
          name: queryOptionPrefix + "select",
          description: `Select properties to be returned, see ${
            csdl.$Version === "2.0"
              ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
              : "[Select](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionselect)"
          }`,
          in: "query",
          explode: false,
          schema: {
            type: "array",
            uniqueItems: true,
            items: {
              type: "string",
              enum: selectItems,
            },
          },
        });
      }
    }
  }

  /**
   * Add parameter for query option $skip
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionSkip(parameters, target, restrictions) {
    const supported =
      restrictions.SkipSupported !== undefined
        ? restrictions.SkipSupported
        : target == null || target[voc.Capabilities.SkipSupported] !== false;

    if (supported) {
      parameters.push({
        $ref: "#/components/parameters/skip",
      });
    }
  }

  /**
   * Add parameter for query option $top
   * @param {Array} parameters Array of parameters to augment
   * @param {string} target Target container child of path
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function optionTop(parameters, target, restrictions) {
    const supported =
      restrictions.TopSupported !== undefined
        ? restrictions.TopSupported
        : target == null || target[voc.Capabilities.TopSupported] !== false;

    if (supported) {
      parameters.push({
        $ref: "#/components/parameters/top",
      });
    }
  }

  /**
   * Construct Operation Object for update
   * @param {object} pathItem Path Item Object to augment
   * @param {object} element Model element of navigation segment
   * @param {string} name Name of navigation segment
   * @param {string} sourceName Name of path source
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function operationUpdate(
    pathItem,
    element,
    name,
    sourceName,
    target,
    level,
    restrictions
  ) {
    const updateRestrictions =
      restrictions.UpdateRestrictions ||
      (target && target[voc.Capabilities.UpdateRestrictions]) ||
      {};

    if (updateRestrictions.Updatable !== false) {
      const type = model.element(element.$Type);
      const operation = {
        summary:
          updateRestrictions.Description ||
          "Update " +
            (element.$Collection ? "entity in " : "") +
            (level > 0 ? "related " : "") +
            name,
        tags: [sourceName],
        requestBody: {
          description:
            (type && type[voc.Core.Description]) || "New property values",
          required: true,
          content: {
            "application/json": {
              schema:
                csdl.$Version === "2.0"
                  ? {
                      type: "object",
                      title: "Modified " + nameParts(element.$Type).name,
                      properties: { d: ref(element.$Type, SUFFIX.update) },
                    }
                  : ref(element.$Type, SUFFIX.update),
            },
          },
        },
        responses: response(
          204,
          "Success",
          undefined,
          updateRestrictions.ErrorResponses
        ),
      };
      if (updateRestrictions.LongDescription)
        operation.description = updateRestrictions.LongDescription;

      customParameters(operation, updateRestrictions);
      pathItem[updateRestrictions.UpdateMethod?.toLowerCase() || "patch"] =
        operation;
    }
  }

  /**
   * Construct Operation Object for delete
   * @param {object} pathItem Path Item Object to augment
   * @param {object} element Model element of navigation segment
   * @param {string} name Name of navigation segment
   * @param {string} sourceName Name of path source
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function operationDelete(
    pathItem,
    element,
    name,
    sourceName,
    target,
    level,
    restrictions
  ) {
    const deleteRestrictions =
      restrictions.DeleteRestrictions ||
      (target && target[voc.Capabilities.DeleteRestrictions]) ||
      {};

    if (deleteRestrictions.Deletable !== false) {
      pathItem.delete = {
        summary:
          deleteRestrictions.Description ||
          "Delete " +
            (element.$Collection ? "entity from " : "") +
            (level > 0 ? "related " : "") +
            name,
        tags: [sourceName],
        responses: response(
          204,
          "Success",
          undefined,
          deleteRestrictions.ErrorResponses
        ),
      };
      if (deleteRestrictions.LongDescription)
        pathItem.delete.description = deleteRestrictions.LongDescription;

      customParameters(pathItem.delete, deleteRestrictions);
    }
  }

  /**
   * Add path and Path Item Object for media resource
   * @param {object} paths The Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} type Entity type object of navigation segment
   * @param {string} name Name of navigation segment
   * @param {string} sourceName Name of path source
   */
  function pathItemForMediaResource(
    paths,
    prefix,
    prefixParameters,
    type,
    name,
    sourceName
  ) {
    if (!type.$HasStream) return;

    const pathItem = {
      parameters: prefixParameters,
      get: {
        summary: `Get media resource from ${name} by key`,
        tags: [sourceName],

        responses: {
          200: {
            description: "Retrieved media resource",
            content: {
              "*/*": { schema: { type: "string", format: "binary" } },
            },
          },
          "4XX": {
            $ref: "#/components/responses/error",
          },
        },
      },
    };

    paths[`${prefix}/$value`] = pathItem;
  }

  /**
   * Add paths and Path Item Objects for navigation segments
   * @param {object} paths The Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} type Entity type object of navigation segment
   * @param {string} sourceName Name of path source
   * @param {integer} level Number of navigation segments so far
   * @param {string} navigationPrefix Path for finding navigation restrictions
   */
  function pathItemsWithNavigation(
    paths,
    prefix,
    prefixParameters,
    type,
    root,
    sourceName,
    level,
    navigationPrefix
  ) {
    const navigationRestrictions =
      root[voc.Capabilities.NavigationRestrictions] || {};
    const rootNavigable =
      (level == 0 && navigationRestrictions.Navigability != "None") ||
      (level == 1 && navigationRestrictions.Navigability != "Single") ||
      level > 1;

    if (type && level < maxLevels) {
      const properties = navigationPathMap(type);
      for (const [name, property] of Object.entries(properties)) {
        const parentRestrictions = navigationPropertyRestrictions(
          root,
          navigationPrefix
        );
        if (parentRestrictions.Navigability == "Single") return;

        const navigationPath =
          navigationPrefix + (navigationPrefix.length > 0 ? "/" : "") + name;
        const restrictions = navigationPropertyRestrictions(
          root,
          navigationPath
        );
        if (
          ["Recursive", "Single"].includes(restrictions.Navigability) ||
          (restrictions.Navigability == null && rootNavigable)
        ) {
          const targetSetName =
            root.$NavigationPropertyBinding &&
            root.$NavigationPropertyBinding[navigationPath];
          const target = entityContainer[targetSetName];
          pathItems(
            paths,
            prefix + "/" + name,
            prefixParameters,
            property,
            root,
            sourceName,
            targetSetName,
            target,
            level + 1,
            navigationPath
          );
        }
      }
    }
  }

  /**
   * Collect navigation paths of a navigation segment and its potentially structured components
   * @param {object} type Structured type
   * @param {object} map Map of navigation property paths and their types
   * @param {string} prefix Navigation prefix
   * @param {integer} level Number of navigation segments so far
   * @return {object} Map of navigation property paths and their types
   */
  function navigationPathMap(type, map = {}, prefix = "", level = 0) {
    for (const [name, property] of model.propertiesOfStructuredType(type)) {
      if (property.$Kind == "NavigationProperty") {
        map[prefix + name] = property;
      } else if (property.$Type && !property.$Collection && level < maxLevels) {
        navigationPathMap(
          model.element(property.$Type),
          map,
          prefix + name + "/",
          level + 1
        );
      }
    }
    return map;
  }

  /**
   * Construct map of key names for an entity type
   * @param {object} type Entity type object
   * @return {object} Map of key names
   */
  function keyMap(type) {
    const map = {};
    if (type.$Kind == "EntityType") {
      const keys = model.key(type);
      for (const key of keys) {
        if (typeof key == "string") map[key] = true;
      }
    }
    return map;
  }

  /**
   * Key for path item
   * @param {object} entityType Entity Type object
   * @param {integer} level Number of navigation segments so far
   * @return {object} key: Key segment, parameters: key parameters
   */
  function entityKey(entityType, level) {
    let segment = "";
    const params = [];
    const keys = model.key(entityType);
    const properties = model.propertiesMapOfStructuredType(entityType);

    keys.forEach((key, index) => {
      const suffix = level > 0 ? "_" + level : "";
      if (keyAsSegment) segment += "/";
      else {
        if (index > 0) segment += ",";
        if (keys.length != 1) segment += key + "=";
      }
      let parameter;
      let property = {};
      if (typeof key == "string") {
        parameter = key;
        property = properties[key];
      } else {
        parameter = Object.keys(key)[0];
        const segments = key[parameter].split("/");
        property = properties[segments[0]];
        for (let i = 1; i < segments.length; i++) {
          const complexType = model.element(property.$Type);
          const properties = model.propertiesMapOfStructuredType(complexType);
          property = properties[segments[i]];
        }
      }
      const propertyType = property.$Type;
      segment +=
        pathValuePrefix(propertyType) +
        "{" +
        parameter +
        suffix +
        "}" +
        pathValueSuffix(propertyType);
      const param = {
        description:
          [property[voc.Core.Description], property[voc.Core.LongDescription]]
            .filter((t) => t)
            .join("  \n") || "key: " + parameter,
        in: "path",
        name: parameter + suffix,
        required: true,
        schema: schema(property, "", true),
      };
      params.push(param);
    });
    return {
      segment: (keyAsSegment ? "" : "(") + segment + (keyAsSegment ? "" : ")"),
      parameters: params,
    };
  }

  /**
   * Prefix for key value in key segment
   * @param {typename} Qualified name of key property type
   * @return {string} value prefix
   */
  function pathValuePrefix(typename) {
    //TODO: handle other Edm types, enumeration types, and type definitions
    if (
      [
        "Edm.Int64",
        "Edm.Int32",
        "Edm.Int16",
        "Edm.SByte",
        "Edm.Byte",
        "Edm.Decimal",
        "Edm.Double",
        "Edm.Single",
        "Edm.Boolean",
        "Edm.Date",
        "Edm.DateTimeOffset",
        "Edm.TimeOfDay",
        "Edm.Guid",
      ].includes(typename)
    )
      return "";

    if (keyAsSegment) return "";

    return "'";
  }

  /**
   * Suffix for key value in key segment
   * @param {typename} Qualified name of key property type
   * @return {string} value prefix
   */
  function pathValueSuffix(typename) {
    //TODO: handle other Edm types, enumeration types, and type definitions
    if (
      [
        "Edm.Int64",
        "Edm.Int32",
        "Edm.Int16",
        "Edm.SByte",
        "Edm.Byte",
        "Edm.Decimal",
        "Edm.Double",
        "Edm.Single",
        "Edm.Boolean",
        "Edm.Date",
        "Edm.DateTimeOffset",
        "Edm.TimeOfDay",
        "Edm.Guid",
      ].includes(typename)
    )
      return "";
    if (keyAsSegment) return "";
    return "'";
  }

  /**
   * Add path and Path Item Object for actions and functions bound to the element
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} element Model element the operations are bound to
   * @param {string} sourceName Name of path source
   * @param {boolean} byKey read by key
   */
  function pathItemsForBoundOperations(
    paths,
    prefix,
    prefixParameters,
    element,
    sourceName,
    byKey = false
  ) {
    const overloads =
      //TODO: make method
      model.boundOverloads[
        element.$Type + (!byKey && element.$Collection ? "-c" : "")
      ] || [];
    for (const item of overloads) {
      if (item.overload.$Kind == "Action")
        pathItemAction(
          paths,
          prefix + "/" + item.name,
          prefixParameters,
          item.name,
          item.overload,
          sourceName
        );
      else
        pathItemFunction(
          paths,
          prefix + "/" + item.name,
          prefixParameters,
          item.name,
          item.overload,
          sourceName
        );
    }
  }

  /**
   * Add path and Path Item Object for an action import
   * @param {object} paths Paths Object to augment
   * @param {string} name Name of action import
   * @param {object} child Action import object
   */
  function pathItemActionImport(paths, name, child) {
    const overload = model
      .element(child.$Action)
      .find((overload) => !overload.$IsBound);
    pathItemAction(
      paths,
      "/" + name,
      [],
      child.$Action,
      overload,
      child.$EntitySet,
      child
    );
  }

  /**
   * Add path and Path Item Object for action overload
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {string} actionName Qualified name of function
   * @param {object} overload Function overload
   * @param {string} sourceName Name of path source
   * @param {string} actionImport Action import
   */
  function pathItemAction(
    paths,
    prefix,
    prefixParameters,
    actionName,
    overload,
    sourceName,
    actionImport = {}
  ) {
    const name =
      actionName.indexOf(".") === -1 ? actionName : nameParts(actionName).name;
    const pathItem = {
      post: {
        summary:
          actionImport[voc.Core.Description] ||
          overload[voc.Core.Description] ||
          "Invoke action " + name,
        tags: [sourceName || "Service Operations"],
        responses: overload.$ReturnType
          ? response(
              200,
              "Success",
              overload.$ReturnType,
              overload[voc.Capabilities.OperationRestrictions]?.ErrorResponses
            )
          : response(
              204,
              "Success",
              undefined,
              overload[voc.Capabilities.OperationRestrictions]?.ErrorResponses
            ),
      },
    };
    const description =
      actionImport[voc.Core.LongDescription] ||
      overload[voc.Core.LongDescription];
    if (description) pathItem.post.description = description;
    if (prefixParameters.length > 0)
      pathItem.post.parameters = [...prefixParameters];
    let parameters = overload.$Parameter || [];
    if (overload.$IsBound) parameters = parameters.slice(1);
    if (parameters.length > 0) {
      const requestProperties = {};
      for (const p of parameters) {
        requestProperties[p.$Name] = schema(p);
      }
      pathItem.post.requestBody = {
        description: "Action parameters",
        content: {
          "application/json": {
            schema: {
              type: "object",
              properties: requestProperties,
            },
          },
        },
      };
    }
    customParameters(
      pathItem.post,
      overload[voc.Capabilities.OperationRestrictions] || {}
    );
    paths[prefix] = pathItem;
  }

  /**
   * Add path and Path Item Object for an action import
   * @param {object} paths Paths Object to augment
   * @param {string} name Name of function import
   * @param {object} child Function import object
   */
  function pathItemFunctionImport(paths, name, child) {
    const overloads = model.element(child.$Function);
    for (const overload of overloads) {
      if (overload.$IsBound) continue;
      pathItemFunction(
        paths,
        "/" + name,
        [],
        child.$Function,
        overload,
        child.$EntitySet,
        child
      );
    }
  }

  /**
   * Add path and Path Item Object for function overload
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {string} functionName Qualified name of function
   * @param {object} overload Function overload
   * @param {string} sourceName Name of path source
   * @param {object} functionImport Function Import
   */
  function pathItemFunction(
    paths,
    prefix,
    prefixParameters,
    functionName,
    overload,
    sourceName,
    functionImport = {}
  ) {
    const name =
      functionName.indexOf(".") === -1
        ? functionName
        : nameParts(functionName).name;
    let parameters = overload.$Parameter || [];
    if (overload.$IsBound) parameters = parameters.slice(1);

    const pathSegments = [];
    const params = [];

    const implicitAliases =
      csdl.$Version > "4.0" ||
      csdl.$Version === "2.0" ||
      parameters.some((p) => p[voc.Core.OptionalParameter]);

    for (const p of parameters) {
      const param = {
        required: implicitAliases ? !p[voc.Core.OptionalParameter] : true,
      };
      const description = [p[voc.Core.Description], p[voc.Core.LongDescription]]
        .filter((t) => t)
        .join("  \n");
      if (description) param.description = description;
      const type = model.element(p.$Type || "Edm.String");
      if (
        p.$Collection ||
        p.$Type == "Edm.Stream" ||
        ["ComplexType", "EntityType"].includes(type?.$Kind) ||
        type?.$UnderlyingType == "Edm.Stream"
      ) {
        param.in = "query";
        if (implicitAliases) {
          param.name = p.$Name;
        } else {
          pathSegments.push(p.$Name + "=@" + p.$Name);
          param.name = "@" + p.$Name;
        }
        param.schema = jsonSchema(p) || (type && jsonSchema(type));
        if (!param.schema) param.schema = { type: "string" };
        if (description) param.description += "  \n";
        else param.description = "";
        param.description +=
          "This is " +
          (p.$Collection ? "a " : "") +
          "URL-encoded JSON " +
          (p.$Collection ? "array with items " : "") +
          "of type " +
          model.namespaceQualifiedName(p.$Type || "Edm.String") +
          ", see [Complex and Collection Literals](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#sec_ComplexandCollectionLiterals)";
        param.example = p.$Collection ? "[]" : "{}";
      } else {
        if (implicitAliases) {
          param.in = "query";
        } else {
          pathSegments.push(p.$Name + "={" + p.$Name + "}");
          param.in = "path";
        }
        param.name = p.$Name;
        if (
          !p.$Type ||
          p.$Type === "Edm.String" ||
          type?.$UnderlyingType === "Edm.String"
        ) {
          if (description) param.description += "  \n";
          else param.description = "";
          param.description +=
            "String value needs to be enclosed in single quotes";
        }

        param.schema = schema(p, "", true, true);
      }
      params.push(param);
    }

    const pathParameters = implicitAliases
      ? ""
      : "(" + pathSegments.join(",") + ")";
    const pathItem = {
      get: {
        summary:
          functionImport[voc.Core.Description] ||
          overload[voc.Core.Description] ||
          "Invoke function " + name,
        tags: [sourceName || "Service Operations"],
        parameters: prefixParameters.concat(params),
        responses: response(
          200,
          "Success",
          overload.$ReturnType,
          overload[voc.Capabilities.OperationRestrictions]?.ErrorResponses
        ),
      },
    };
    const description =
      functionImport[voc.Core.LongDescription] ||
      overload[voc.Core.LongDescription];
    if (description) pathItem.get.description = description;

    customParameters(
      pathItem.get,
      overload[voc.Capabilities.OperationRestrictions] || {}
    );

    paths[prefix + pathParameters] = pathItem;
  }

  /**
   * Add path and Path Item Object for a filtered collection
   * @param {object} paths Paths Object to augment
   * @param {string} prefix Prefix for path
   * @param {Array} prefixParameters Parameter Objects for prefix
   * @param {object} element Model element of navigation segment
   * @param {string} sourceName Name of path source
   * @param {string} target Target container child of path
   * @param {integer} level Number of navigation segments so far
   * @param {object} restrictions Navigation property restrictions of navigation segment
   */
  function pathItemForFilterSegment(
    paths,
    prefix,
    prefixParameters,
    element,

    sourceName,

    target,
    level,
    restrictions
  ) {
    const name = prefix.substring(prefix.lastIndexOf("/") + 1);
    const path = prefix + "/$filter({filter_expression})/$each";
    const parameters = [
      ...prefixParameters,
      {
        name: "filter_expression",
        in: "path",
        required: true,
        description:
          "Filter expression using the [Common Expression Syntax](https://docs.oasis-open.org/odata/odata/v4.01/os/part2-url-conventions/odata-v4.01-os-part2-url-conventions.html#sec_CommonExpressionSyntax)",
        schema: { type: "string" },
      },
    ];
    const pathItem = { parameters: parameters };

    const deleteRestrictions =
      restrictions.DeleteRestrictions ||
      (target && target[voc.Capabilities.DeleteRestrictions]) ||
      {};

    const updateRestrictions =
      restrictions.UpdateRestrictions ||
      (target && target[voc.Capabilities.UpdateRestrictions]) ||
      {};

    if (
      deleteRestrictions.FilterSegmentSupported ||
      updateRestrictions.FilterSegmentSupported
    ) {
      paths[path] = pathItem;
    }

    if (updateRestrictions.FilterSegmentSupported) {
      const type = model.element(element.$Type);
      pathItem.patch = {
        summary:
          "Update entities matching the filter expression in " +
          (level > 0 ? "related " : "") +
          name,
        description:
          "See [Update Members of a Collection](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_UpdateMembersofaCollection)",
        tags: [sourceName],
        requestBody: {
          description:
            (type && type[voc.Core.Description]) || "New property values",
          required: true,
          content: {
            "application/json": {
              schema: ref(element.$Type, SUFFIX.update),
            },
          },
        },
        responses: response(
          204,
          "Success",
          undefined,
          updateRestrictions.ErrorResponses
        ),
      };
      customParameters(pathItem.patch, updateRestrictions);
    }

    if (deleteRestrictions.FilterSegmentSupported) {
      pathItem.delete = {
        summary:
          "Delete entities matching the filter expression from " +
          (level > 0 ? "related " : "") +
          name,
        description:
          "See [Delete Members of a Collection](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_DeleteMembersofaCollection)",
        tags: [sourceName],
        responses: response(
          204,
          "Success",
          undefined,
          deleteRestrictions.ErrorResponses
        ),
      };
      customParameters(pathItem.delete, deleteRestrictions);
    }
  }

  /**
   * Add path and Path Item Object for batch requests
   * @param {object} paths Paths Object to augment
   * @param {object} container Entity container
   */
  function pathItemBatch(paths, container) {
    const batchSupport = container[voc.Capabilities.BatchSupport] || {};
    const supported = skipBatchPath
      ? false
      : container[voc.Capabilities.BatchSupported] !== false &&
        batchSupport.Supported !== false;
    if (supported) {
      const firstEntitySet = model.resources.filter(
        ([, resource]) => resource.$Collection
      )[0]?.[0];
      paths["/$batch"] = {
        post: {
          summary:
            batchSupport[voc.Core.Description] || "Send a group of requests",
          description:
            (batchSupport[voc.Core.LongDescription] ||
              "Group multiple requests into a single request payload, see " +
                (csdl.$Version === "2.0"
                  ? "[Batch Processing (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/batch-processing/)."
                  : "[Batch Requests](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_BatchRequests).")) +
            '\n\n*Please note that "Try it out" is not supported for this request.*',
          tags: ["Batch Requests"],
          requestBody: {
            required: true,
            description: "Batch request",
            content: {
              "multipart/mixed;boundary=request-separator": {
                schema: {
                  type: "string",
                },
                example:
                  "--request-separator\n" +
                  "Content-Type: application/http\n" +
                  "Content-Transfer-Encoding: binary\n\n" +
                  "GET " +
                  firstEntitySet +
                  " HTTP/1.1\n" +
                  "Accept: application/json\n\n" +
                  "\n--request-separator--",
              },
            },
          },
          responses: {
            "4XX": {
              $ref: "#/components/responses/error",
            },
          },
        },
      };
      paths["/$batch"].post.responses[csdl.$Version < "4.0" ? 202 : 200] = {
        description: "Batch response",
        content: {
          "multipart/mixed": {
            schema: {
              type: "string",
            },
            example:
              "--response-separator\n" +
              "Content-Type: application/http\n\n" +
              "HTTP/1.1 200 OK\n" +
              "Content-Type: application/json\n\n" +
              "{...}" +
              "\n--response-separator--",
          },
        },
      };
    }
  }

  /**
   * Construct Responses Object
   * @param {string} code HTTP response code
   * @param {string} description Description
   * @param {object} type Response type object
   * @param {array} errors Array of operation-specific status codes with descriptions
   */
  function response(code, description, type, errors) {
    const r = {};
    r[code] = {
      description: description,
    };
    if (code != 204) {
      let s = schema(type);
      if (type.$Collection) {
        s = {
          type: "object",
          title: "Collection of " + nameParts(type.$Type || "Edm.String").name,
          properties:
            csdl.$Version === "2.0"
              ? { __count: ref("count"), results: s }
              : {
                  [csdl.$Version > "4.0" ? "@count" : "@odata.count"]:
                    ref("count"),
                  value: s,
                },
        };
      }
      if (csdl.$Version === "2.0") {
        s = {
          type: "object",
          title: type.$Collection ? "Wrapper" : nameParts(type.$Type).name,
          properties: { d: s },
        };
      }
      r[code].content = {
        "application/json": {
          schema: s,
        },
      };
    }
    if (errors) {
      for (const e of errors) {
        r[e.StatusCode] = {
          description: e.Description,
          content: {
            "application/json": {
              schema: { $ref: "#/components/schemas/error" },
            },
          },
        };
      }
    } else {
      r["4XX"] = {
        $ref: "#/components/responses/error",
      };
    }
    return r;
  }

  /**
   * Construct the Components Object from the types of the CSDL document
   * @param {boolean} hasEntityContainer CSDL document has entity container
   * @return {object} Components Object
   */
  function components(hasEntityContainer) {
    const c = {
      schemas: schemas(hasEntityContainer),
    };

    if (hasEntityContainer) {
      c.parameters = parameters();
      c.responses = {
        error: {
          description: "Error",
          content: {
            "application/json": {
              schema: ref("error"),
            },
          },
        },
      };
    }

    return c;
  }

  /**
   * Construct Schema Objects from the types of the CSDL document
   * @param {boolean} hasEntityContainer CSDL document has entity container
   * @return {object} Map of Schema Objects
   */
  function schemas(hasEntityContainer) {
    const unordered = {};

    for (const r of requiredSchemas.list) {
      const type = model.element(`${r.namespace}.${r.name}`);
      if (!type) continue;
      switch (type.$Kind) {
        case "ComplexType":
        case "EntityType":
          schemasForStructuredType(
            unordered,
            r.namespace,
            r.name,
            type,
            r.suffix
          );
          break;
        case "EnumType":
          schemaForEnumerationType(unordered, r.namespace, r.name, type);
          break;
        case "TypeDefinition":
          schemaForTypeDefinition(unordered, r.namespace, r.name, type);
          break;
      }
    }

    const ordered = {};
    for (const name of Object.keys(unordered).sort()) {
      ordered[name] = unordered[name];
    }

    inlineTypes(ordered);

    if (hasEntityContainer) {
      ordered.count = count();
      if (requiredSchemas.entityReferenceNeeded)
        ordered.entityReference = entityReference();
      ordered.error = error();
    }

    return ordered;
  }

  /**
   * Construct Schema Objects from the types of the CSDL document
   * @param {object} schemas Map of Schema Objects to augment
   */
  function inlineTypes(schemas) {
    if (typesToInline.geoPoint) {
      schemas.geoPoint = {
        type: "object",
        properties: {
          coordinates: ref("geoPosition"),
          type: {
            type: "string",
            enum: ["Point"],
            default: "Point",
          },
        },
        required: ["type", "coordinates"],
      };
      schemas.geoPosition = {
        type: "array",
        minItems: 2,
        items: {
          type: "number",
        },
      };
    }
  }

  /**
   * Construct Schema Objects for an enumeration type
   * @param {object} schemas Map of Schema Objects to augment
   * @param {string} qualifier Qualifier for structured type
   * @param {string} name Simple name of structured type
   * @param {object} type Structured type
   * @return {object} Map of Schemas Objects
   */
  function schemaForEnumerationType(schemas, qualifier, name, type) {
    const members = model.enumTypeMembers(type).map(([name]) => name);

    const s = {
      type: "string",
      title: type[voc.Core.Description] || name,
      enum: members,
    };
    const description = type[voc.Core.LongDescription];
    if (description) s.description = description;
    schemas[qualifier + "." + name] = s;
  }

  /**
   * Construct Schema Objects for a type definition
   * @param {object} schemas Map of Schema Objects to augment
   * @param {string} qualifier Qualifier for structured type
   * @param {string} name Simple name of structured type
   * @param {object} type Structured type
   * @return {object} Map of Schemas Objects
   */
  function schemaForTypeDefinition(schemas, qualifier, name, type) {
    const s = schema(Object.assign({ $Type: type.$UnderlyingType }, type));
    s.title = type[voc.Core.Description] || name;
    const description = type[voc.Core.LongDescription];
    if (description) s.description = description;
    schemas[qualifier + "." + name] = s;
  }

  /**
   * Construct Schema Objects for a structured type
   * @param {object} schemas Map of Schema Objects to augment
   * @param {string} qualifier Qualifier for structured type
   * @param {string} name Simple name of structured type
   * @param {object} type Structured type
   * @param {string} suffix Suffix for read/create/update
   * @return {object} Map of Schemas Objects
   */
  function schemasForStructuredType(schemas, qualifier, name, type, suffix) {
    const schemaName = qualifier + "." + name + suffix;
    const baseName = qualifier + "." + name;
    const isKey = keyMap(type);
    const required = Object.keys(isKey);
    const schemaProperties = {};

    for (const [name, property] of model.propertiesOfStructuredType(type)) {
      if (suffix === SUFFIX.read) schemaProperties[name] = schema(property);
      if (property.$Kind == "NavigationProperty") {
        if (property.$Collection && suffix === SUFFIX.read) {
          if (csdl.$Version === "2.0") {
            schemaProperties[name] = {
              type: "object",
              properties: { results: schemaProperties[name] },
            };
          } else {
            schemaProperties[
              `${name}@${csdl.$Version === "4.0" ? "odata." : ""}count`
            ] = ref("count");
          }
        }
        if (
          property[voc.Core.Permissions] != "Read" &&
          !property[voc.Core.Computed]
        ) {
          if (property.$ContainsTarget) {
            if (suffix === SUFFIX.create)
              schemaProperties[name] = schema(property, SUFFIX.create);
            if (suffix === SUFFIX.update && deepUpdate)
              schemaProperties[name] = schema(property, SUFFIX.create);
          } else if (
            !property.$Collection &&
            !property.$ReferentialConstraint &&
            !property.$Nullable &&
            suffix === SUFFIX.create
          ) {
            schemaProperties[
              `${name}${csdl.$Version === "4.0" ? "@odata.bind" : ""}`
            ] = ref("entityReference");
            requiredSchemas.entityReferenceNeeded = true;
          }
        }
      } else {
        if (
          property[voc.Core.Permissions] == "Read" ||
          property[voc.Core.Computed]
        ) {
          let index = required.indexOf(name);
          if (index != -1) required.splice(index, 1);
        } else {
          if (suffix === SUFFIX.create)
            schemaProperties[name] = schema(property, SUFFIX.create);
          if (
            suffix === SUFFIX.update &&
            !isKey[name] &&
            !property[voc.Core.Immutable]
          )
            schemaProperties[name] = schema(property, SUFFIX.update);
        }
      }
    }

    schemas[schemaName] = {
      title: (type[voc.Core.Description] || name) + TITLE_SUFFIX[suffix],
      type: "object",
    };

    if (Object.keys(schemaProperties).length > 0)
      schemas[schemaName].properties = schemaProperties;

    if (suffix === SUFFIX.create && required.length > 0)
      schemas[schemaName].required = required;

    const description = type[voc.Core.LongDescription];
    if (description) {
      schemas[schemaName].description = description;
    }

    //TODO: make method
    const derivedTypes = model.derivedTypes[baseName];
    if (derivedTypes) {
      schemas[schemaName].anyOf = [];
      for (const derivedType of derivedTypes) {
        schemas[schemaName].anyOf.push(ref(derivedType, suffix));
      }
      if (!type.$Abstract) schemas[schemaName].anyOf.push({});
    }
  }

  /**
   * Construct Parameter Objects for type-independent OData system query options
   * @return {object} Map of Parameter Objects
   */
  function parameters() {
    const param = {
      top: {
        name: queryOptionPrefix + "top",
        in: "query",
        description: `Show only the first n items, see ${
          csdl.$Version === "2.0"
            ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
            : "[Paging - Top](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptiontop)"
        }`,
        schema: {
          type: "integer",
          minimum: 0,
        },
        example: 50,
      },
      skip: {
        name: queryOptionPrefix + "skip",
        in: "query",
        description: `Skip the first n items, see ${
          csdl.$Version === "2.0"
            ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
            : "[Paging - Skip](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionskip)"
        }`,
        schema: {
          type: "integer",
          minimum: 0,
        },
      },
      count: {
        name:
          csdl.$Version === "2.0"
            ? "$inlinecount"
            : queryOptionPrefix + "count",
        in: "query",
        description: `Include count of items, see ${
          csdl.$Version === "2.0"
            ? "[URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/)"
            : "[Count](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptioncount)"
        }`,
        schema:
          csdl.$Version === "2.0"
            ? { type: "string", enum: ["allpages", "none"] }
            : { type: "boolean" },
      },
    };

    if (csdl.$Version >= "4.0")
      param.search = {
        name: queryOptionPrefix + "search",
        in: "query",
        description:
          "Search items by search phrases, see [Searching](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptionsearch)",
        schema: {
          type: "string",
        },
      };

    if (applySupported)
      param.apply = {
        name: queryOptionPrefix + "apply",
        in: "query",
        description:
          "Apply basic grouping and aggregation functionality, see [Aggregation](http://docs.oasis-open.org/odata/odata-data-aggregation-ext/v4.0/odata-data-aggregation-ext-v4.0.html)",
        schema: {
          type: "string",
        },
      };

    return param;
  }

  /**
   * Construct OData error response
   * @return {object} Error response schema
   */
  function error() {
    const err = {
      type: "object",
      required: ["error"],
      properties: {
        error: {
          type: "object",
          required: ["code", "message"],
          properties: {
            code: { type: "string" },
            message: { type: "string" },
            target: { type: "string" },
            details: {
              type: "array",
              items: {
                type: "object",
                required: ["code", "message"],
                properties: {
                  code: { type: "string" },
                  message: { type: "string" },
                  target: { type: "string" },
                },
              },
            },
            innererror: {
              type: "object",
              description: "The structure of this object is service-specific",
            },
          },
        },
      },
    };

    if (csdl.$Version < "4.0") {
      err.properties.error.properties.message = {
        type: "object",
        properties: {
          lang: { type: "string" },
          value: { type: "string" },
        },
        required: ["lang", "value"],
      };
      delete err.properties.error.properties.details;
      delete err.properties.error.properties.target;
    }

    return err;
  }

  /**
   * Construct OData count response
   * @return {object} Count response schema
   */
  function count() {
    return csdl.$Version === "2.0"
      ? {
          type: "string",
          description:
            "The number of entities in the collection. Available when using the $inlinecount query option, see [URI Conventions (OData Version 2.0)](https://www.odata.org/documentation/odata-version-2-0/uri-conventions/).",
        }
      : {
          anyOf: [
            {
              type: "integer",
              minimum: 0,
            },
            { type: "string" },
          ],
          description:
            "The number of entities in the collection. Available when using the [$count](http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_SystemQueryOptioncount) query option.",
        };
  }

  /**
   * Construct OData entity reference schema
   * @return {object} Entity reference schema
   */
  function entityReference() {
    const link = {
      type: "string",
      format: "uri",
      description:
        "[Link to a related entity](https://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part1-protocol.html#sec_LinktoRelatedEntitiesWhenCreatinganE)",
    };
    return csdl.$Version === "4.0"
      ? link
      : {
          type: "object",
          properties: {
            "@id": link,
          },
        };
  }

  /**
   * Construct Schema Object for model object referencing a type
   * @param {object} modelElement referencing a type
   * @return {object} Schema Object
   */
  function schema(
    element,
    suffix = "",
    forParameter = false,
    forFunction = false
  ) {
    let s = {};
    switch (element.$Type) {
      case "Edm.AnnotationPath":
      case "Edm.ModelElementPath":
      case "Edm.NavigationPropertyPath":
      case "Edm.PropertyPath":
        s.type = "string";
        break;
      case "Edm.Binary":
        s = oas31
          ? { type: "string", contentEncoding: "base64url" }
          : { type: "string", format: "base64url" };
        if (element.$MaxLength)
          s.maxLength = Math.ceil((4 * element.$MaxLength) / 3);
        break;
      case "Edm.Boolean":
        s.type = "boolean";
        break;
      case "Edm.Byte":
        s = {
          type: "integer",
          format: "uint8",
        };
        break;
      case "Edm.Date":
        s = {
          type: "string",
          format: "date",
          example: "2017-04-13",
        };
        break;
      case "Edm.DateTime":
      case "Edm.DateTimeOffset":
        s =
          csdl.$Version === "2.0"
            ? { type: "string", example: "/Date(1492098664000)/" }
            : {
                type: "string",
                format: "date-time",
                example:
                  "2017-04-13T15:51:04" +
                  (isNaN(element.$Precision) || element.$Precision === 0
                    ? ""
                    : "." + "0".repeat(element.$Precision)) +
                  "Z",
              };
        break;
      case "Edm.Decimal": {
        s =
          csdl.$Version === "2.0"
            ? { type: "string", format: "decimal", example: "0" }
            : {
                ...(!oas31 && {
                  anyOf: [{ type: "number" }, { type: "string" }],
                }),
                ...(oas31 && { type: ["number", "string"] }),
                format: "decimal",
                example: 0,
              };
        if (element.$Precision === 34 && element.$Scale === "floating") {
          s.format = "decimal128";
          s.example = "9.9e6144";
        } else {
          let scale = !isNaN(element.$Scale) ? element.$Scale : null;
          if (scale !== null) {
            s.multipleOf = 1 / 10 ** scale;
            if (element.$Precision < 16) {
              let limit = 10 ** (element.$Precision - scale);
              let delta = 10 ** -scale;
              s.maximum = limit - delta;
              s.minimum = -s.maximum;
            }
          }
        }
        break;
      }
      case "Edm.Double":
        s = {
          type: "number",
          format: "double",
          example: 3.14,
        };
        break;
      case "Edm.Duration":
        s = {
          type: "string",
          format: "duration",
          example: "P4DT15H51M04S",
        };
        break;
      case "Edm.GeographyPoint":
      case "Edm.GeometryPoint":
        s = ref("geoPoint");
        typesToInline.geoPoint = true;
        break;
      case "Edm.Guid":
        s = {
          type: "string",
          format: "uuid",
          example: "01234567-89ab-cdef-0123-456789abcdef",
        };
        break;
      case "Edm.Int16":
        s = {
          type: "integer",
          format: "int16",
        };
        break;
      case "Edm.Int32":
        s = {
          type: "integer",
          format: "int32",
        };
        break;
      case "Edm.Int64":
        s = {
          ...(!oas31 && { anyOf: [{ type: "integer" }, { type: "string" }] }),
          ...(oas31 && { type: ["integer", "string"] }),
          format: "int64",
          example: "42",
        };
        break;
      case "Edm.PrimitiveType":
        s = {
          ...(!oas31 && {
            anyOf: [
              { type: "boolean" },
              { type: "number" },
              { type: "string" },
            ],
          }),
          ...(oas31 && { type: ["boolean", "number", "string"] }),
        };
        break;
      case "Edm.SByte":
        s = {
          type: "integer",
          format: "int8",
        };
        break;
      case "Edm.Single":
        s = {
          type: "number",
          format: "float",
          example: 3.14,
        };
        break;
      case "Edm.Stream": {
        s = jsonSchema(element);
        if (!s) {
          s = oas31
            ? { type: "string", contentEncoding: "base64url" }
            : { type: "string", format: "base64url" };
        }
        break;
      }
      case "Edm.String":
      case undefined:
        s.type = "string";
        if (element.$MaxLength) s.maxLength = element.$MaxLength;
        allowedValues(s, element);
        pattern(s, element);
        break;
      case "Edm.TimeOfDay":
        s = {
          type: "string",
          format: "time",
          example: "15:51:04",
        };
        break;
      default:
        if (element.$Type.startsWith("Edm.")) {
          messages.push("Unknown type: " + element.$Type);
        } else {
          let type = model.element(element.$Type);
          let isStructured =
            type && ["ComplexType", "EntityType"].includes(type.$Kind);
          s = ref(element.$Type, isStructured ? suffix : "");
          if (element.$MaxLength) {
            s = {
              allOf: [s],
              maxLength: element.$MaxLength,
            };
          }
        }
    }

    if (element.$Nullable) {
      if (oas31) {
        if (s.$ref) s = { anyOf: [s, { type: "null" }] };
        else if (Array.isArray(s.type)) s.type.push("null");
        else s.type = [s.type, "null"];
      } else {
        if (s.$ref) s = { allOf: [s] };
        s.nullable = true;
      }
    }

    if (element.$DefaultValue !== undefined) {
      if (s.$ref) s = { allOf: [s] };
      s.default = element.$DefaultValue;
    }

    if (element[voc.Core.Example]) {
      if (s.$ref) s = { allOf: [s] };
      s.example = element[voc.Core.Example].Value;
    }

    if (forFunction) {
      if (s.example && typeof s.example === "string") {
        s.example = `${pathValuePrefix(element.$Type)}${
          s.example
        }${pathValueSuffix(element.$Type)}`;
      }
      if (s.pattern) {
        const pre = pathValuePrefix(element.$Type);
        const suf = pathValueSuffix(element.$Type);
        s.pattern = s.pattern.replace(/^\^/, `^${pre}(`);
        s.pattern = s.pattern.replace(/\$$/, `)${suf}$`);
      } else if (!element.$Type || element.$Type === "Edm.String") {
        s.pattern = "^'([^']|'')*'$";
      }
      if (element.$Nullable) {
        if (!s.anyOf && !s.allOf) s.type = "string";
        if (s.format) s.format += ",null";
        s.default = "null"; //TODO: @Core.OptionalParameter.DefaultValue
        if (s.pattern) {
          s.pattern = s.pattern.replace(/^\^/, "^(null|");
          s.pattern = s.pattern.replace(/\$$/, ")$");
        }
      }
    }

    if (typeof element[voc.Validation.Maximum] === "number") {
      if (s.$ref) s = { allOf: [s] };
      s.maximum = element[voc.Validation.Maximum];
      if (element[voc.Validation.Maximum + voc.Validation.Exclusive]) {
        s.exclusiveMaximum = oas31 ? s.maximum : true;
        if (oas31) delete s.maximum;
      }
    }

    if (typeof element[voc.Validation.Minimum] === "number") {
      if (s.$ref) s = { allOf: [s] };
      s.minimum = element[voc.Validation.Minimum];
      if (element[voc.Validation.Minimum + voc.Validation.Exclusive]) {
        s.exclusiveMinimum = oas31 ? s.minimum : true;
        if (oas31) delete s.minimum;
      }
    }

    if (element.$Collection) {
      s = {
        type: "array",
        items: s,
      };
    }

    if (!forParameter && element[voc.Core.Description]) {
      if (s.$ref) s = { allOf: [s] };
      s.title = element[voc.Core.Description];
    }

    if (!forParameter && element[voc.Core.LongDescription]) {
      if (s.$ref) s = { allOf: [s] };
      s.description = element[voc.Core.LongDescription];
    }

    return s;
  }

  /**
   * Add allowed values enum to Schema Object for string-like model element
   * @param {object} schema Schema Object to augment
   * @param {object} element Model element
   */
  function allowedValues(schema, element) {
    const values = element[voc.Validation.AllowedValues];
    if (values) schema.enum = values.map((record) => record.Value);
  }

  /**
   * Add pattern to Schema Object for string-like model element
   * @param {object} schema Schema Object to augment
   * @param {object} element Model element
   */
  function pattern(schema, element) {
    const pattern = element[voc.Validation.Pattern];
    if (pattern) schema.pattern = pattern;
  }

  function jsonSchema(element) {
    let schema = element[voc.JSON.Schema];
    if (!schema) return undefined;
    if (typeof schema == "string") return JSON.parse(schema);
    else return schema;
  }

  /**
   * Construct Reference Object for a type
   * @param {string} typename Qualified name of referenced type
   * @param {string} suffix Optional suffix for referenced schema
   * @return {object} Reference Object
   */
  function ref(typename, suffix = "") {
    let name = typename;
    let nsp = "";
    let url = "";

    if (typename.indexOf(".") != -1) {
      let parts = nameParts(typename);
      //TODO: make method
      nsp = model.namespace[parts.qualifier];
      name = nsp + "." + parts.name;
      //TODO: make method
      url = model.namespaceUrl[nsp] || "";
      if (url === "" && !requiredSchemas.used[name + suffix]) {
        requiredSchemas.used[name + suffix] = true;
        requiredSchemas.list.push({ namespace: nsp, name: parts.name, suffix });
      }
      //TODO: introduce better way than guessing
      if (url.endsWith(".xml"))
        url = url.substring(0, url.length - 3) + "openapi3.json";
    }

    return {
      $ref: url + "#/components/schemas/" + name + suffix,
    };
  }

  /**
   * Augment Components Object with map of Security Scheme Objects
   * @param {object} components Components Object to augment
   * @param {object} entityContainer Entity Container object
   */
  function securitySchemes(components, entityContainer) {
    const authorizations =
      entityContainer?.[voc.Authorization.Authorizations] || [];
    const schemes = {};
    const location = {
      Header: "header",
      QueryOption: "query",
      Cookie: "cookie",
    };
    for (const auth of authorizations) {
      const scheme = {};
      const flow = {};
      if (auth.Description) scheme.description = auth.Description;
      const qualifiedType = auth["@type"] || auth["@odata.type"];
      const type = qualifiedType.substring(qualifiedType.lastIndexOf(".") + 1);
      let unknown = false;
      switch (type) {
        case "ApiKey":
          scheme.type = "apiKey";
          scheme.name = auth.KeyName;
          scheme.in = location[auth.Location];
          break;
        case "Http":
          scheme.type = "http";
          scheme.scheme = auth.Scheme;
          scheme.bearerFormat = auth.BearerFormat;
          break;
        case "OAuth2AuthCode":
          scheme.type = "oauth2";
          scheme.flows = { authorizationCode: flow };
          flow.authorizationUrl = auth.AuthorizationUrl;
          flow.tokenUrl = auth.TokenUrl;
          if (auth.RefreshUrl) flow.refreshUrl = auth.RefreshUrl;
          flow.scopes = scopes(auth);
          break;
        case "OAuth2ClientCredentials":
          scheme.type = "oauth2";
          scheme.flows = { clientCredentials: flow };
          flow.tokenUrl = auth.TokenUrl;
          if (auth.RefreshUrl) flow.refreshUrl = auth.RefreshUrl;
          flow.scopes = scopes(auth);
          break;
        case "OAuth2Implicit":
          scheme.type = "oauth2";
          scheme.flows = { implicit: flow };
          flow.authorizationUrl = auth.AuthorizationUrl;
          if (auth.RefreshUrl) flow.refreshUrl = auth.RefreshUrl;
          flow.scopes = scopes(auth);
          break;
        case "OAuth2Password":
          scheme.type = "oauth2";
          scheme.flows = {};
          scheme.flows = { password: flow };
          flow.tokenUrl = auth.TokenUrl;
          if (auth.RefreshUrl) flow.refreshUrl = auth.RefreshUrl;
          flow.scopes = scopes(auth);
          break;
        case "OpenIDConnect":
          scheme.type = "openIdConnect";
          scheme.openIdConnectUrl = auth.IssuerUrl;
          break;
        default:
          unknown = true;
          messages.push("Unknown Authorization type " + qualifiedType);
      }
      if (!unknown) schemes[auth.Name] = scheme;
    }
    if (Object.keys(schemes).length > 0) components.securitySchemes = schemes;
  }

  function scopes(authorization) {
    const scopes = {};
    for (const scope of authorization.Scopes) {
      scopes[scope.Scope] = scope.Description;
    }
    return scopes;
  }

  /**
   * Augment OpenAPI document with Security Requirements Object
   * @param {object} openapi OpenAPI document to augment
   * @param {object} entityContainer Entity Container object
   */
  function security(openapi, entityContainer) {
    const securitySchemes =
      entityContainer?.[voc.Authorization.SecuritySchemes] || [];
    if (securitySchemes.length > 0) openapi.security = [];
    for (const scheme of securitySchemes) {
      const s = {};
      s[scheme.Authorization] = scheme.RequiredScopes || [];
      openapi.security.push(s);
    }
  }
};
