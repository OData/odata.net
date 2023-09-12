// @ts-nocheck
/**
 * Resource diagram for an OData model using https://yuml.me
 *
 * Latest version: https://github.com/oasis-tcs/odata-openapi/blob/master/lib/edm.js
 */

import _def from "./edm";
const { isIdentifier, nameParts } = _def;

export default { resourceDiagram };

/**
 * Construct resource diagram
 * @param {object} model EDM for CSDL document
 * @return {string} resource diagram
 */
function resourceDiagram(model) {
  let diagram = "";
  let comma = "";
  //TODO: make colors configurable
  let color = {
    resource: "{bg:lawngreen}",
    entityType: "{bg:lightslategray}",
    complexType: "",
    external: "{bg:whitesmoke}",
  };

  for (const [qualifiedName, type] of model.structuredTypes) {
    const typeName = nameParts(qualifiedName).name;
    diagram +=
      comma +
      (type.$BaseType
        ? "[" +
          nameParts(type.$BaseType).name +
          (type.$Kind == "EntityType" ? color.entityType : color.complexType) +
          "]^"
        : "") +
      "[" +
      typeName +
      (type.$Kind == "EntityType" ? color.entityType : color.complexType) +
      "]";

    for (const [name, property] of model.directPropertiesOfStructuredType(
      type
    )) {
      const targetNP = nameParts(property.$Type ?? "Edm.String");
      if (
        property.$Kind == "NavigationProperty" ||
        targetNP.qualifier != "Edm"
      ) {
        const target = model.element(property.$Type);
        const bidirectional =
          property.$Partner &&
          target &&
          target[property.$Partner] &&
          target[property.$Partner].$Partner == name;
        // Note: if the partner has the same name then it will also be depicted
        if (!bidirectional || name <= property.$Partner) {
          diagram +=
            ",[" +
            typeName +
            "]" +
            (property.$Kind != "NavigationProperty" || property.$ContainsTarget
              ? "++"
              : bidirectional
              ? cardinality(target[property.$Partner])
              : "") +
            "-" +
            cardinality(property) +
            (property.$Kind != "NavigationProperty" || bidirectional
              ? ""
              : ">") +
            "[" +
            (target
              ? targetNP.name +
                (type.$Kind == "EntityType"
                  ? color.entityType
                  : color.complexType)
              : property.$Type + color.external) +
            "]";
        }
      }
    }
    comma = ",";
  }

  for (const [name, resource] of model.resources.reverse()) {
    if (resource.$Type) {
      diagram +=
        comma +
        "[" +
        name +
        "%20" +
        color.resource +
        "]" + // additional space in case entity set and type have same name
        "++-" +
        cardinality(resource) +
        ">[" +
        nameParts(resource.$Type).name +
        "]";
    } else {
      if (resource.$Action) {
        diagram += comma + "[" + name + color.resource + "]";
        const overload = model
          .element(resource.$Action)
          .find((o) => !o.$IsBound);
        diagram += overloadDiagram(name, overload);
      } else if (resource.$Function) {
        diagram += comma + "[" + name + color.resource + "]";
        const overloads = model.element(resource.$Function);
        if (overloads) {
          const unbound = overloads.filter((o) => !o.$IsBound);
          // TODO: loop over all overloads, add new source box after first arrow
          diagram += overloadDiagram(name, unbound[0]);
        }
      }
    }
  }

  if (diagram != "") {
    diagram =
      "\n\n## Entity Data Model\n![ER Diagram](https://yuml.me/diagram/class/" +
      diagram +
      ")\n\n### Legend\n![Legend](https://yuml.me/diagram/plain;dir:TB;scale:60/class/[External.Type" +
      color.external +
      "],[ComplexType" +
      color.complexType +
      "],[EntityType" +
      color.entityType +
      "],[EntitySet/Singleton/Operation" +
      color.resource +
      "])";
  }

  return diagram;

  /**
   * Diagram representation of property cardinality
   * @param {object} typedElement Typed model element, e.g. property
   * @param {boolean} one Explicitly represent to-1
   * @return {string} cardinality
   */
  function cardinality(typedElement) {
    return typedElement.$Collection
      ? "*"
      : typedElement.$Nullable
      ? "0..1"
      : "";
  }

  function overloadDiagram(name, overload) {
    let diag = "";
    if (overload.$ReturnType) {
      const type = model.element(overload.$ReturnType.$Type ?? "Edm.String");
      if (type) {
        diag +=
          "-" +
          cardinality(overload.$ReturnType, true) +
          ">[" +
          nameParts(overload.$ReturnType.$Type).name +
          "]";
      }
    }
    for (const param of overload.$Parameter ?? []) {
      const type = model.element(param.$Type ?? "Edm.String");
      if (type) {
        diag +=
          comma +
          "[" +
          name +
          color.resource +
          "]in-" +
          cardinality(param.$Type) +
          ">[" +
          nameParts(param.$Type).name +
          "]";
      }
    }
    return diag;
  }
}
