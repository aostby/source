<?xml version="1.0" ?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:template match="@*|node()">
  <xsl:copy>           
    <xsl:apply-templates select="@*|node()"/>
  </xsl:copy>
</xsl:template>
<xsl:template match="/configuration/appSettings">
  <xsl:copy>
    <xsl:apply-templates select="node()|@*"/>
    <xsl:element name="add">
      <xsl:attribute name="key">NewSetting</xsl:attribute>
      <xsl:attribute name="value">New Setting Value</xsl:attribute>
    </xsl:element>
  </xsl:copy>
</xsl:template>
</xsl:stylesheet>