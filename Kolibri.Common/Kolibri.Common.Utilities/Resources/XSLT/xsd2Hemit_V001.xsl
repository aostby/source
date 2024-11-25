<xsl:transform version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:xs="http://my.ns.uri"
xmlns:b="http://my.ns.uri2">

	<!--<xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />-->

	<xsl:template match="/">
		<html>
			<body>
				<h2>Test av xsd transform</h2>
				<xsl:apply-templates select="*" />
			</body>
		</html>
	</xsl:template>

	<xsl:template match="/xs:schema">
		<h1>Schema noda?</h1><xsl:apply-templates select="*" />
	</xsl:template>

	
	<xsl:template match="xs:element">
		<h1>element noda?</h1><xsl:apply-templates select="*" />
	</xsl:template>
	

	<xsl:template match="*">
		<h1>Stjerne noda?</h1>
		<xsl:value-of select="text()"/> 
		<xsl:value-of select="name(parent::*)" /><xsl:apply-templates select="*" />
	</xsl:template>



	<xsl:template match="xs:annotation">
		<h3>Jarrau</h3>
		<!--<table border="1">
     <tr bgcolor="#9acd32">
       <th>appinfo</th>
       <th>fieldInfo</th>
     </tr>
     <xsl:for-each select="/*">
     <tr>
       <td><xsl:value-of select="xs:appinfo"/></td>
       <td><xsl:value-of select="b:fieldInfo"/></td>
     </tr>
     </xsl:for-each>
   </table> -->
	</xsl:template>
</xsl:transform>
