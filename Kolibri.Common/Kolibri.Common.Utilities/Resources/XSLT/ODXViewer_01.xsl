<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:output method="xml" indent="yes" omit-xml-declaration="yes"/>

  <!-- Match the Base element, with high priority to avoid matching the more
       specific template further down, that matches *[*] -->
  <xsl:template match="Base" priority="10">
    <ul>
      <xsl:apply-templates/>
    </ul>
  </xsl:template>

  <!-- Match the Parent element, with high priority to avoid matching the more
       specific template further down, that matches *[*] -->
  <xsl:template match="Parent" priority="10">
    <li>
      <!-- Output a Parent element and append it with its position amongst
           all Parent elements at this level. -->
      <xsl:text>Parent</xsl:text>
      <xsl:value-of select="count(preceding-sibling::Parent) + 1"/>
      <xsl:apply-templates/>
    </li>
  </xsl:template>

  <!-- Match those elements that have children. -->
  <xsl:template match="*[*]">
    <li>
      <xsl:value-of select="local-name(.)"/>
      <ul>
        <xsl:apply-templates/>
      </ul>
    </li>
  </xsl:template>

  <!-- Match the remaining elements (without children) -->
  <xsl:template match="*">
    <li>
      <xsl:value-of select="local-name(.)"/>
    </li>
  </xsl:template>
  
  <!-- attributes to elements -->
<xsl:template match="@*">
    <xsl:element name="{name()}">
        <xsl:value-of select="."/>
    </xsl:element>
</xsl:template>

<!-- avoid mixed content -->
<xsl:template match="text()[../@*]">
    <value>
        <xsl:value-of select="."/>
    </value>
</xsl:template>
<!-- modified identity transform -->
<xsl:template match="node()">
    <xsl:copy>
        <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
</xsl:template>
  
</xsl:stylesheet>