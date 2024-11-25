<xsl:transform version="1.0" 
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:xs="http://my.ns.uri"
xmlns:b="http://my.ns.uri2">

	<!--<xsl:output omit-xml-declaration="yes" method="xml" version="1.0" />-->

	<!--
	
	<?xml version="1.0" encoding="UTF-8"?>
<catalog>
  <cd>
    <title>Empire Burlesque</title>
    <artist>Bob Dylan</artist>
    <country>USA</country>
    <company>Columbia</company>
    <price>10.90</price>
    <year>1985</year>
  </cd>
.
.
</catalog>

-->


	<!--
	
	<?xml version="1.0" encoding="UTF-8"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="/">
  <html>
  <body>
  <h2>My CD Collection</h2>
  <table border="1">
    <tr bgcolor="#9acd32">
      <th>Title</th>
      <th>Artist</th>
    </tr>
    <xsl:for-each select="catalog/cd">
    <tr>
      <td><xsl:value-of select="title"/></td>
      <td><xsl:value-of select="artist"/></td>
    </tr>
    </xsl:for-each>
  </table>
  </body>
  </html>
</xsl:template>

</xsl:stylesheet>
	-->


	<xsl:template match="/">
		<html>
			<body>
				<h2>Ny Test</h2>
				<table border="1">
					<tr bgcolor="#9acd32">
					</tr>
					<xsl:for-each select="/xs:schema/xs:annotation">
						<tr>
							<td>
<!-- 							 	<xsl:value-of select="/xs:schema[@xmlns="http://Hemit.BizTalk.TDOC.Schemas.TDocInvoiceExport"]/xs:annotation"/>   -->
								<xsl:text>Jadda</xsl:text>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template> 
</xsl:transform>

