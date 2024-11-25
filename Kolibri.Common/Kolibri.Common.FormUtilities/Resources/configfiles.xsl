<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<xsl:template match="/">
		<html>
			<HEAD>
				<STYLE TYPE="text/css">
				.tabell 
				{
				background-position: center;
				border: thin;
				}
				.bakgrunnsfarge 
				{
				background-color: #EEE8AA;
				}
				.config 
				{
				background: #FFFFF0;
				font-family: Arial;
				font-size: smaller;
				font-style: oblique;
				}
				.value
				{
				background: #FFFFF0;
				font-family: Arial;
				font-size: smaller;
				}
				.kommentar{
				background-color: #EEE8AA;				
				color: Silver;					
				}

    </STYLE>
			</HEAD>
			<body class="bakgrunnsfarge">
				<table class="tabell">
					<tbody align="center">
						<xsl:for-each select="//*">
							<tr align="left">
								<td align="left" CLASS="config">
									<xsl:value-of select="local-name()"/>
								</td>
								<td align="left" CLASS="value">
									<xsl:apply-templates select="current()/text()"/>
								</td>
							</tr>
						</xsl:for-each>
					</tbody>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>
