<xsl:stylesheet version="2.0"
 xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
 xmlns:xs="http://www.w3.org/2001/XMLSchema"
 xmlns:my="my:my"
 exclude-result-prefixes="xml xsl xs my">
  <xsl:output omit-xml-declaration="yes" indent="yes"/>

  <xsl:template match="/">
    <xsl:variable name="vLines" select="tokenize(., '\n')"/>

    <xsl:sequence select="my:parse-lines($vLines)"/>
  </xsl:template>

  <xsl:function name="my:parse-lines" as="element()*">
    <xsl:param name="pLines" as="xs:string*"/>

    <xsl:sequence select=
       "my:parse-line($pLines, 1, count($pLines))"/>
  </xsl:function>

  <xsl:function name="my:parse-line" as="element()*">
    <xsl:param name="pLines" as="xs:string*"/>
    <xsl:param name="pLineNum" as="xs:integer"/>
    <xsl:param name="pTotalLines" as="xs:integer"/>

    <xsl:if test="not($pLineNum gt $pTotalLines)">
      <xsl:variable name="vLine" select="$pLines[$pLineNum]"/>
      <xsl:variable name="vLineLength"
           select="string-length($vLine)"/>
      <xsl:choose>
        <xsl:when test=
        "starts-with($vLine, '#')
        and
         ends-with($vLine, '#')
        ">
          <xsl:variable name="vInnerString"
           select="substring($vLine, 2, $vLineLength -2)"/>
          <h1>
            <xsl:sequence select="my:parse-string($vInnerString)"/>
          </h1>
          <xsl:sequence select=
        "my:parse-line($pLines, $pLineNum+1, $pTotalLines)"/>
        </xsl:when>
        <xsl:when test=
        "starts-with($vLine, '- ')
       and
         not(starts-with($pLines[$pLineNum -1], '- '))
        ">
          <ul>
            <li>
              <xsl:sequence select="my:parse-string(substring($vLine, 2))"/>
            </li>
            <xsl:sequence select=
           "my:parse-line($pLines, $pLineNum+1, $pTotalLines)"/>
          </ul>
        </xsl:when>
        <xsl:when test="starts-with($vLine, '- ')">
          <li>
            <xsl:sequence select="my:parse-string(substring($vLine, 2))"/>
          </li>
          <xsl:sequence select=
           "my:parse-line($pLines, $pLineNum+1, $pTotalLines)"/>
        </xsl:when>
        <xsl:otherwise>
          <p>
            <xsl:sequence select="my:parse-string($vLine)"/>
          </p>
          <xsl:sequence select=
           "my:parse-line($pLines, $pLineNum+1, $pTotalLines)"/>
        </xsl:otherwise>
      </xsl:choose>
    </xsl:if>
  </xsl:function>

  <xsl:function name="my:parse-string" as="node()*">
    <xsl:param name="pS" as="xs:string"/>

    <xsl:analyze-string select="$pS" flags="x" regex=
  '(__(.*?)__)
  |
   (\*(.*?)\*)
  |
   ("(.*?)"\[(.*?)\])

  '>
      <xsl:matching-substring>
        <xsl:choose>
          <xsl:when test="regex-group(1)">
            <strong>
              <xsl:sequence select="my:parse-string(regex-group(2))"/>
            </strong>
          </xsl:when>
          <xsl:when test="regex-group(3)">
            <span>
              <xsl:sequence select="my:parse-string(regex-group(4))"/>
            </span>
          </xsl:when>
          <xsl:when test="regex-group(5)">
            <a href="{regex-group(7)}">
              <xsl:sequence select="regex-group(6)"/>
            </a>
          </xsl:when>
        </xsl:choose>
      </xsl:matching-substring>

      <xsl:non-matching-substring>
        <xsl:value-of select="."/>
      </xsl:non-matching-substring>
    </xsl:analyze-string>
  </xsl:function>
</xsl:stylesheet>