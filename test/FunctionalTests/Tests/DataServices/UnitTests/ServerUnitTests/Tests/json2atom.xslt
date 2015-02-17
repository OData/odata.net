<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet
    version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
    xmlns:d="http://docs.oasis-open.org/odata/ns/data"
    xmlns:m="http://docs.oasis-open.org/odata/ns/metadata"
    xmlns="http://www.w3.org/2005/Atom"
    xmlns:metadatautils="www.metadatautils.com"
>
  <xsl:output method="xml" indent="yes"/>

  <!-- passing the request uri so that we can put the correct value in the atom:feed/atom:id element-->
  <xsl:param name="requesturi" />
  <xsl:param name="usemetadatautil" />
  <xsl:param name="isupload" />

  <xsl:template match="Array">
    <feed>
      <title type="text"></title>
      <id>
        <xsl:value-of select="$requesturi"/>
      </id>
      <updated></updated>
      <xsl:apply-templates/>
      <xsl:if test="../../__next">
        <link rel="next" href="{../../__next}" />
      </xsl:if>
    </feed>
  </xsl:template>

  <xsl:template match="results | Object[results]">
    <xsl:apply-templates select="*"/>
  </xsl:template>

  <xsl:template match="__next"/>

  <xsl:template match="associationuri"/>

  <xsl:template match="Object[not(results) and __metadata/uri]">
    <xsl:call-template name="EntityTemplate"/>
  </xsl:template>

  <xsl:template match="Object[not(results) and not(__metadata/uri)]">
    <xsl:call-template name="WriteProperties"/>
  </xsl:template>

  <xsl:template name="EntityTemplate">
    <entry>
      <xsl:if test="__metadata/etag">
        <xsl:attribute name="m:etag">
          <xsl:value-of select="__metadata/etag"/>
        </xsl:attribute>
      </xsl:if>

      <id>
        <xsl:choose>
          <!--When we have metadatautils available, use the custom method there for extracting the id.-->
          <xsl:when test="$usemetadatautil">
            <xsl:value-of select="metadatautils:GetIdFromEditLink(__metadata/id, __metadata/uri, __metadata/type)"/>  
          </xsl:when>
          <!--Otherwise, use the id element if one exists. If no id is present, fallback to the uri (edit link).-->
          <xsl:otherwise>
            <xsl:choose>
              <xsl:when test="__metadata/id">
                <xsl:value-of select="__metadata/id" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:value-of select="__metadata/uri"/>
              </xsl:otherwise>
            </xsl:choose>
          </xsl:otherwise>
        </xsl:choose>
      </id>
      <xsl:if test="$isupload!='true'">
        <title></title>
        <updated></updated>
        <author>
          <name/>
        </author>
      </xsl:if>
      <category term="{__metadata/type}" scheme="http://docs.oasis-open.org/odata/ns/scheme" />
      <xsl:if test="__metadata/edit_media">
        <link rel="edit-media" href="{__metadata/edit_media}">
          <xsl:attribute name="title">
            <xsl:call-template name="WriteTitle" />
          </xsl:attribute>
          <xsl:if test="__metadata/media_etag">
            <xsl:attribute name="etag" namespace="http://docs.oasis-open.org/odata/ns/metadata">
              <xsl:value-of select="__metadata/media_etag"/>
            </xsl:attribute>
          </xsl:if>
        </link>
      </xsl:if>

      <link rel="edit" href="{__metadata/uri}">
        <xsl:attribute name="title">
          <xsl:call-template name="WriteTitle" />
        </xsl:attribute>
      </link>

      <!-- Handle deferred navigation properties -->
      <xsl:for-each select="*[__deferred]">
        <xsl:if test="$usemetadatautil">
          <link rel="http://docs.oasis-open.org/odata/ns/related/{local-name()}" title="{local-name()}" href="{__deferred/uri}">
            <xsl:attribute name="type">
              <xsl:value-of select="metadatautils:GetLinkTypeAttributeValue(../__metadata/type, local-name())" />
            </xsl:attribute>
          </link>
        </xsl:if>
        <xsl:if test="$usemetadatautil!='true'">
          <link rel="http://docs.oasis-open.org/odata/ns/related/{local-name()}" type="application/atom+xml;type=entry" title="{local-name()}" href="{__deferred/uri}" />
        </xsl:if>
      </xsl:for-each>

      <!-- Handle null reference navigation properties -->
      <xsl:if test="$usemetadatautil">
        <xsl:for-each select="*[@IsNull='true']">
          <xsl:if test="metadatautils:IsNavigationProperty(../__metadata/type, local-name())" >
            <link rel="http://docs.oasis-open.org/odata/ns/related/{local-name()}" title="{local-name()}" href="{../__metadata/uri}/{local-name()}">
              <xsl:attribute name="type">
                <xsl:value-of select="metadatautils:GetLinkTypeAttributeValue(../__metadata/type, local-name())" />
              </xsl:attribute>
              <m:inline />
            </link>
          </xsl:if>
        </xsl:for-each>
      </xsl:if>

      <xsl:for-each select="*[__mediaresource]">
        <xsl:if test="__mediaresource/media_src">
          <link rel="http://docs.oasis-open.org/odata/ns/mediaresource/{local-name()}" title="{local-name()}" href="{__mediaresource/media_src}">
            <xsl:if test="__mediaresource/content_type">
              <xsl:attribute name="type">
                <xsl:value-of select="__mediaresource/content_type"/>
              </xsl:attribute>
            </xsl:if>
          </link>
        </xsl:if>
        <xsl:if test="__mediaresource/edit_media">
          <link rel="http://docs.oasis-open.org/odata/ns/edit-media/{local-name()}" title="{local-name()}" href="{__mediaresource/edit_media}">
            <xsl:if test="__mediaresource/content_type">
              <xsl:attribute name="type">
                <xsl:value-of select="__mediaresource/content_type"/>
              </xsl:attribute>
            </xsl:if>
            <xsl:if test="__mediaresource/media_etag">
              <xsl:attribute name="etag" namespace="http://docs.oasis-open.org/odata/ns/metadata">
                <xsl:value-of select="__mediaresource/media_etag"/>
              </xsl:attribute>
            </xsl:if>
          </link>
        </xsl:if>
      </xsl:for-each>

      <!-- Handle association links -->
      <xsl:for-each select="__metadata/properties/*">
        <link rel="http://docs.oasis-open.org/odata/ns/relatedlinks/{local-name()}" type="application/xml" title="{local-name()}" href="{associationuri}" />
      </xsl:for-each>

      <xsl:for-each select="*[Array/Object/__metadata/uri or (results and not(starts-with(__metadata/type, 'Collection('))) or __metadata/uri]">
        <link rel="http://docs.oasis-open.org/odata/ns/related/{local-name()}" title="{local-name()}" href="{../__metadata/uri}/{local-name()}">
          <xsl:if test="$usemetadatautil">
            <xsl:attribute name="type">
              <xsl:value-of select="metadatautils:GetLinkTypeAttributeValue(../__metadata/type, local-name())" />
            </xsl:attribute>
          </xsl:if>
          <xsl:if test="$usemetadatautil!='true'">
            <xsl:attribute name="type">
              <xsl:value-of select="'application/atom+xml;type=feed'" />
            </xsl:attribute>
          </xsl:if>
          <m:inline>
            <xsl:choose>
              <xsl:when test="__metadata/uri">
                <xsl:call-template name="EntityTemplate" />
              </xsl:when>
              <xsl:otherwise>
                <xsl:apply-templates/>
              </xsl:otherwise>
            </xsl:choose>
          </m:inline>
        </link>
      </xsl:for-each>

      <content>
        <xsl:choose>
          <xsl:when test="__metadata/media_src">
            <xsl:attribute name="src">
              <xsl:value-of select="__metadata/media_src"/>
            </xsl:attribute>
            <xsl:attribute name="type">
              <xsl:value-of select="__metadata/content_type"/>
            </xsl:attribute>
          </xsl:when>
          <xsl:otherwise>
            <xsl:attribute name="type">application/xml</xsl:attribute>
            <xsl:call-template name="CreateProperties" />
          </xsl:otherwise>
        </xsl:choose>
      </content>
      <xsl:if test="__metadata/edit_media">
        <xsl:call-template name="CreateProperties" />
      </xsl:if>
    </entry>
  </xsl:template>

  <xsl:template name="CreateProperties">
    <xsl:if test="*[local-name()!='__metadata' and count(__deferred)=0 and count(__mediaresource)=0 and not(Array/Object/__metadata/uri or (results and not(starts-with(__metadata/type, 'Collection('))) or __metadata/uri)]">
      <m:properties>
        <xsl:call-template name="WriteProperties"/>
      </m:properties>
    </xsl:if>
  </xsl:template>

  <xsl:template name="WriteProperties">
    <xsl:for-each select="*[local-name()!='__metadata' and count(__deferred)=0 and count(__mediaresource)=0 and not(Array/Object/__metadata/uri or (results and not(starts-with(__metadata/type, 'Collection('))) or __metadata/uri)]">
      <xsl:if test="$usemetadatautil and metadatautils:IsNavigationProperty(../__metadata/type, local-name())!='true'" >
        <xsl:call-template name="WriteProperty"/>
      </xsl:if>
      <xsl:if test="$usemetadatautil!='true'">
        <xsl:call-template name="WriteProperty"/>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="WriteProperty">
    <xsl:choose>
      <xsl:when test="namespace-uri(.) = 'http://www.opengis.net/gml'">
        <xsl:copy-of select="." />
      </xsl:when>
      <xsl:otherwise>
        <xsl:element name="d:{local-name()}">
          <xsl:call-template name="WritePropertyValue"/>
        </xsl:element>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="WritePropertyValue">
    <xsl:choose>
      <xsl:when test="starts-with(__metadata/type, 'Collection(')">
        <xsl:attribute name="m:type">
          <xsl:value-of select="__metadata/type"/>
        </xsl:attribute>
        <xsl:for-each select="results/Array/Object">
          <m:element>
            <xsl:choose>
              <xsl:when test="* and namespace-uri(child::*) != 'http://www.opengis.net/gml'">
                <!-- complex type -->
                <xsl:for-each select="*">
                  <xsl:call-template name="WriteProperty"/>
                </xsl:for-each>
              </xsl:when>
              <xsl:otherwise>
                <!-- primitive value -->
                <xsl:call-template name="WritePropertyValue"/>
              </xsl:otherwise>
            </xsl:choose>
          </m:element>
        </xsl:for-each>
      </xsl:when>
      <xsl:when test="__metadata">
        <xsl:attribute name="m:type">
          <xsl:value-of select="__metadata/type"/>
        </xsl:attribute>
        <xsl:for-each select="*[local-name()!='__metadata']">
          <xsl:call-template name="WriteProperty"/>
        </xsl:for-each>
      </xsl:when>
      <xsl:when test="namespace-uri(*) = 'http://www.opengis.net/gml'">
        <xsl:copy-of select="*"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:if test="@IsNull='true'">
          <xsl:attribute name="m:null">true</xsl:attribute>
        </xsl:if>
        <xsl:if test="not(starts-with(string(), substring(normalize-space(), 1, 1))) or not(substring(string(), string-length(), 1) = substring(normalize-space(), string-length(normalize-space()), 1))">
          <xsl:attribute name="xml:space">preserve</xsl:attribute>
        </xsl:if>
        <xsl:value-of select="."/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <xsl:template name="WriteTitle">
    <xsl:variable name="segments">
      <xsl:call-template name="SplitString">
        <xsl:with-param name="string" select="__metadata/uri" />
        <xsl:with-param name="delimiter" select="'/'" />
      </xsl:call-template>
    </xsl:variable>
    <xsl:value-of select="substring-before(msxsl:node-set($segments)/*[last()], '(')"/>
  </xsl:template>

  <xsl:template name="SplitString">
    <xsl:param name="string" />
    <xsl:param name="delimiter" />

    <xsl:choose>
      <xsl:when test="contains($string, $delimiter)">
        <token>
          <xsl:value-of select="substring-before($string, $delimiter)" />
        </token>
        <xsl:call-template name="SplitString">
          <xsl:with-param name="string" select="substring-after($string, $delimiter)" />
          <xsl:with-param name="delimiter" select="$delimiter" />
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <token>
          <xsl:value-of select="$string" />
        </token>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>
