<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:output method="html" indent="no"/>

  <!--
  <xsl:strip-space elements="*"/>
-->

  <xsl:param name="show_ns"/>
  <xsl:variable name="apos">'</xsl:variable>

  <xsl:template match="/">
    <html>
      <head>
        <STYLE TYPE="text/css">
          body       { font-family: sans-serif; font-size: 80%; background-color: #EAEAD9; color: black }

          .connector { font-family: monospace; }

          .name      { color: navy; background-color: white; text-decoration: underline; font-weight: bold;
          padding-top: 0px; padding-bottom: 1px; padding-left: 3px; padding-right: 3px }
          .altname   { color: navy; text-decoration: underline }
          .uri       { color: #444; font-style: italic }
          .value     { color: #040; background-color: #CCC; font-weight: bold }
          .escape    { color: #620; font-family: monospace }

          .root      { color: yellow; background-color: black }
          .element   { color: yellow; background-color: navy }
          .namespace { color: yellow; background-color: #333 }
          .attribute { color: yellow; background-color: #040 }
          .text      { color: yellow; background-color: #400 }
          .pi        { color: yellow; background-color: #044 }
          .comment   { color: yellow; background-color: #303 }

          .root,.element,.attribute,.namespace,.text,.comment,.pi
          { font-weight: bold;
          padding-top: 0px; padding-bottom: 1px; padding-left: 3px; padding-right: 3px }

        </STYLE>
        <title>tree.xslt output</title>
        
      </head>
      <body>
        <h3>tree-view.xsl output</h3>
        <xsl:apply-templates select="." mode="render"/>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="/" mode="render">
    <span class="root">root</span>
    <br/>
    <xsl:apply-templates mode="render"/>
  </xsl:template>

  <xsl:template match="*" mode="render">
    <xsl:call-template name="ascii-art-hierarchy"/>
    <br/>
    <xsl:call-template name="ascii-art-hierarchy"/>
    <span class='connector'>___</span>
    <span class="element">element</span>
    <xsl:text>&#160;</xsl:text>
    <xsl:if test="namespace-uri()">
      <xsl:text>{</xsl:text>
      <span class="uri">
        <xsl:value-of select="namespace-uri()"/>
      </span>
      <xsl:text>}</xsl:text>
    </xsl:if>
    <span class="name">
      <xsl:value-of select="local-name()"/>
    </span>
    <xsl:if test="local-name() != name()">
      <xsl:text> (QName </xsl:text>
      <span class="altname">
        <xsl:value-of select="name()"/>
      </span>
      <xsl:text>)</xsl:text>
    </xsl:if>
    <br/>
    <xsl:apply-templates select="@*" mode="render"/>
    <xsl:if test="$show_ns">
      <xsl:for-each select="namespace::*">
        <xsl:sort select="name()"/>
        <xsl:call-template name="ascii-art-hierarchy"/>
        <span class='connector'>&#160;&#160;</span>
        <span class='connector'>\___</span>
        <span class="namespace">namespace</span>
        <xsl:text>&#160;</xsl:text>
        <xsl:choose>
          <xsl:when test="name()">
            <span class="name">
              <xsl:value-of select="name()"/>
            </span>
          </xsl:when>
          <xsl:otherwise>#default</xsl:otherwise>
        </xsl:choose>
        <xsl:text> = </xsl:text>
        <span class="uri">
          <xsl:value-of select="."/>
        </span>
        <br/>
      </xsl:for-each>
    </xsl:if>
    <xsl:apply-templates mode="render"/>
  </xsl:template>

  <xsl:template match="@*" mode="render">
    <xsl:call-template name="ascii-art-hierarchy"/>
    <span class='connector'>&#160;&#160;</span>
    <span class='connector'>\___</span>
    <span class="attribute">attribute</span>
    <xsl:text>&#160;</xsl:text>
    <xsl:if test="namespace-uri()">
      <xsl:text>{</xsl:text>
      <span class="uri">
        <xsl:value-of select="namespace-uri()"/>
      </span>
      <xsl:text>}</xsl:text>
    </xsl:if>
    <span class="name">
      <xsl:value-of select="local-name()"/>
    </span>
    <xsl:if test="local-name() != name()">
      <xsl:text> (QName </xsl:text>
      <span class="altname">
        <xsl:value-of select="name()"/>
      </span>
      <xsl:text>)</xsl:text>
    </xsl:if>
    <xsl:text> = </xsl:text>
    <span class="value">
      <!-- make spaces be non-breaking spaces, since this is HTML -->
      <xsl:call-template name="escape-ws">
        <xsl:with-param name="text" select="translate(.,' ','&#160;')"/>
      </xsl:call-template>
    </span>
    <br/>
  </xsl:template>

  <xsl:template match="text()" mode="render">
    <xsl:call-template name="ascii-art-hierarchy"/>
    <br/>
    <xsl:call-template name="ascii-art-hierarchy"/>
    <span class='connector'>___</span>
    <span class="text">text</span>
    <xsl:text>&#160;</xsl:text>
    <span class="value">
      <!-- make spaces be non-breaking spaces, since this is HTML -->
      <xsl:call-template name="escape-ws">
        <xsl:with-param name="text" select="translate(.,' ','&#160;')"/>
      </xsl:call-template>
    </span>
    <br/>
  </xsl:template>

  <xsl:template match="comment()" mode="render">
    <xsl:call-template name="ascii-art-hierarchy"/>
    <br/>
    <xsl:call-template name="ascii-art-hierarchy"/>
    <span class='connector'>___</span>
    <span class="comment">comment</span>
    <xsl:text>&#160;</xsl:text>
    <span class="value">
      <!-- make spaces be non-breaking spaces, since this is HTML -->
      <xsl:call-template name="escape-ws">
        <xsl:with-param name="text" select="translate(.,' ','&#160;')"/>
      </xsl:call-template>
    </span>
    <br/>
  </xsl:template>

  <xsl:template match="processing-instruction()" mode="render">
    <xsl:call-template name="ascii-art-hierarchy"/>
    <br/>
    <xsl:call-template name="ascii-art-hierarchy"/>
    <span class='connector'>___</span>
    <span class="pi">processing instruction</span>
    <xsl:text>&#160;</xsl:text>
    <xsl:text>target=</xsl:text>
    <span class="value">
      <xsl:value-of select="name()"/>
    </span>
    <xsl:text>&#160;instruction=</xsl:text>
    <span class="value">
      <xsl:value-of select="."/>
    </span>
    <br/>
  </xsl:template>

  <xsl:template name="ascii-art-hierarchy">
    <xsl:for-each select="ancestor::*">
      <xsl:choose>
        <xsl:when test="following-sibling::node()">
          <span class='connector'>&#160;&#160;</span>|<span class='connector'>&#160;&#160;</span>
          <xsl:text>&#160;</xsl:text>
        </xsl:when>
        <xsl:otherwise>
          <span class='connector'>&#160;&#160;&#160;&#160;</span>
          <span class='connector'>&#160;&#160;</span>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:for-each>
    <xsl:choose>
      <xsl:when test="parent::node() and ../child::node()">
        <span class='connector'>&#160;&#160;</span>
        <xsl:text>|</xsl:text>
      </xsl:when>
      <xsl:otherwise>
        <span class='connector'>&#160;&#160;&#160;</span>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

  <!-- recursive template to escape linefeeds, tabs -->
  <xsl:template name="escape-ws">
    <xsl:param name="text"/>
    <xsl:choose>
      <xsl:when test="contains($text, '&#xA;')">
        <xsl:call-template name="escape-ws">
          <xsl:with-param name="text" select="substring-before($text, '&#xA;')"/>
        </xsl:call-template>
        <span class="escape">\n</span>
        <xsl:call-template name="escape-ws">
          <xsl:with-param name="text" select="substring-after($text, '&#xA;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:when test="contains($text, '&#x9;')">
        <xsl:value-of select="substring-before($text, '&#x9;')"/>
        <span class="escape">\t</span>
        <xsl:call-template name="escape-ws">
          <xsl:with-param name="text" select="substring-after($text, '&#x9;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$text"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>

</xsl:stylesheet>
