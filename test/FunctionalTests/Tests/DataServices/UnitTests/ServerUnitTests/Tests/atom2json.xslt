<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:ads="http://docs.oasis-open.org/odata/ns/data"
    xmlns:adsm="http://docs.oasis-open.org/odata/ns/metadata"
    xmlns:atom="http://www.w3.org/2005/Atom"
    xmlns:jsoninxml="http://astoriaunittests.tests/jsoninxml"
>
  <xsl:output method="text"/>

  <xsl:template match="/*">
    <xsl:text>{&#xA;</xsl:text>
    <xsl:choose>
      <xsl:when test="namespace-uri()='http://docs.oasis-open.org/odata/ns/data'">
        <xsl:call-template name="WriteProperty"/>
      </xsl:when>
      <xsl:when test="local-name()='entry' and namespace-uri()='http://www.w3.org/2005/Atom'">
        <xsl:call-template name="ProcessEntry"/>
      </xsl:when>
    </xsl:choose>
    <xsl:text>&#xA;}</xsl:text>
  </xsl:template>

  <xsl:template name="ProcessEntry">
    <xsl:call-template name="WriteMetadata">
      <xsl:with-param name="type" select="atom:category/@term"/>
    </xsl:call-template>
    <xsl:for-each select="atom:content/adsm:properties/ads:*">
      <xsl:text>,&#xA;</xsl:text>
      <xsl:call-template name="WriteProperty"/>
    </xsl:for-each>
  </xsl:template>
  
  <xsl:template name="WriteProperty">
    <xsl:call-template name="WritePropertyName">
      <xsl:with-param name="propertyName" select="local-name()"/>
    </xsl:call-template>

    <xsl:call-template name="WritePropertyValue" />
  </xsl:template>

  <xsl:template name="WritePropertyName">
    <xsl:param name="propertyName"/>
    <xsl:call-template name="WriteStringValue">
      <xsl:with-param name="value" select="$propertyName"/>
    </xsl:call-template>
    <xsl:text>: </xsl:text>
  </xsl:template>

  <xsl:template name="WritePropertyValue">
    <xsl:param name="type"/>

    <xsl:variable name="_actualType">
      <xsl:choose>
        <xsl:when test="@adsm:type">
          <xsl:value-of select="@adsm:type"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="$type"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>

    <xsl:variable name="actualType" select="string($_actualType)"/>

    <xsl:choose>
      <xsl:when test="@adsm:null='true'">
        <xsl:text>null</xsl:text>
      </xsl:when>
      <xsl:when test="jsoninxml:JsonRepresentation">
        <xsl:value-of select="jsoninxml:JsonRepresentation"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:choose>
          <xsl:when test="starts-with($actualType, 'Collection(') or @jsoninxml:collectionPropertyType">
            <xsl:call-template name="WriteCollectionPropertyValue"/>
          </xsl:when>
          <xsl:when test="$actualType='Edm.String'">
            <xsl:call-template name="WriteStringValue">
              <xsl:with-param name="value" select="string()"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:when test="$actualType='Edm.Int32'">
            <xsl:call-template name="WriteNumberValue">
              <xsl:with-param name="value" select="string()"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:when test="$actualType='Edm.Double'">
            <xsl:call-template name="WriteNumberValue">
              <xsl:with-param name="value" select="string()"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:when test="$actualType">
            <xsl:call-template name="WriteComplexPropertyValue">
              <xsl:with-param name="type" select="$actualType"/>
            </xsl:call-template>
          </xsl:when>
          <xsl:otherwise>
            <xsl:call-template name="WriteStringValue">
              <xsl:with-param name="value" select="string()"/>
            </xsl:call-template>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="WriteCollectionPropertyValue">
    <xsl:text>{&#xA;</xsl:text>
    <xsl:if test="@adsm:type">
      <xsl:call-template name="WriteMetadata">
        <xsl:with-param name="type" select="@adsm:type"/>
      </xsl:call-template>
      <xsl:text>,&#xA;</xsl:text>
    </xsl:if>

    <xsl:variable name="_itemType">
      <xsl:choose>
        <xsl:when test="@adsm:type">
          <xsl:value-of select="@adsm:type"/>
        </xsl:when>
        <xsl:otherwise>
          <xsl:value-of select="@jsoninxml:collectionPropertyType"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:variable>
      
    <!-- 12 = length of "Collection(" + 1
         This extracts the 'itemtype' from 'Collection(itemtype)'.-->
    <xsl:variable name="itemType" select="substring($_itemType, 12, string-length($_itemType) - 12)"/>

    <xsl:call-template name="WritePropertyName">
      <xsl:with-param name="propertyName" select="'results'"/>
    </xsl:call-template>
    <xsl:text>[</xsl:text>
    <xsl:for-each select="ads:element">
      <xsl:if test="position() > 1">, </xsl:if>
      <xsl:call-template name="WritePropertyValue">
        <xsl:with-param name="type" select="$itemType"/>
      </xsl:call-template>
    </xsl:for-each>
    <xsl:text>]</xsl:text>
    
    <xsl:text>&#xA;}</xsl:text>
  </xsl:template>

  <xsl:template name="WriteComplexPropertyValue">
    <xsl:param name="type"/>

    <xsl:text>{&#xA;</xsl:text>
    <xsl:call-template name="WriteMetadata">
      <xsl:with-param name="type" select="$type"/>
    </xsl:call-template>

    <xsl:for-each select="ads:*">
      <xsl:text>,&#xA;</xsl:text>
      <xsl:call-template name="WriteProperty"/>
    </xsl:for-each>
    <xsl:text>&#xA;}</xsl:text>
  </xsl:template>

  <xsl:template name="WriteMetadata">
    <xsl:param name="type"/>
    <xsl:param name="uri"/>

    <xsl:call-template name="WritePropertyName">
      <xsl:with-param name="propertyName" select="'__metadata'"/>
    </xsl:call-template>
    <xsl:text>{ </xsl:text>
    <xsl:if test="$uri">
      <xsl:text>"uri": </xsl:text>
      <xsl:call-template name="WriteStringValue">
        <xsl:with-param name="value" select="$uri"/>
      </xsl:call-template>
      <xsl:if test="$type">
        <xsl:text>, </xsl:text>
      </xsl:if>
    </xsl:if>
    <xsl:if test="$type">
      <xsl:text>"type": </xsl:text>
      <xsl:call-template name="WriteStringValue">
        <xsl:with-param name="value" select="$type"/>
      </xsl:call-template>
    </xsl:if>
    <xsl:text> }</xsl:text>
  </xsl:template>
  
  <!-- Escaping characters which may violate the string value rules is not supported yet. -->
  <xsl:template name="WriteStringValue">
    <xsl:param name="value" />
    <xsl:text>"</xsl:text>
    <xsl:value-of select="$value"/>
    <xsl:text>"</xsl:text>
  </xsl:template>

  <xsl:template name="WriteNumberValue">
    <xsl:param name="value" />
    <xsl:value-of select="$value"/>
  </xsl:template>
</xsl:stylesheet>
