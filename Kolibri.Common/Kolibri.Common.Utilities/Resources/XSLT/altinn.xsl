<?xml version="1.0" encoding="iso-8859-1"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
  <xsl:template match="/">
    <html>
      <HEAD>
        <STYLE TYPE="text/css">
          .tabeller
          {
          background-position: center;
          border: thin;
          }
          .bakgrunnsfarge
          {
          background-color: #B7DFE8;
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
        <script language="javascript" type="text/javascript">
          function toggle(ele)
          {
          var div1 = document.getElementById(ele)

          if (div1.style.display == 'none')
          {
          div1.style.display = 'block'
          }
          else {
          div1.style.display = 'none'
          }
          }

          function collapseAll()
          {
          var divs = document.getElementsByTagName("div"), i=divs.length; while (i--)
          {
          divs[i].style.display = 'none'
          }
          }

          function showAll()
          {
          var divs = document.getElementsByTagName("div"), i=divs.length; while (i--)
          {
          divs[i].style.display = 'block'
          }
          }

        </script>

      </HEAD>
      <body class="bakgrunnsfarge">
        <table>
          <td>
            <a href="#" onclick="collapseAll()">Skjul alle</a>
          </td>
          <td>
            <a href="#" onclick="showAll()">Vis alle</a>
          </td>
        </table>

        <xsl:apply-templates />
      </body>
    </html>
  </xsl:template>

  <xsl:template match="Skjema">
    <xsl:variable name="tabell" select="generate-id(.//*)"/>
    <xsl:variable name="navn" select =".//AnsattNavn-datadef-25426" />
    <table>
     
     
          <td>
          <a href="#" onclick="toggle('{$tabell}')">
            Skjema <xsl:copy-of select="$navn"/>_<xsl:copy-of select="$tabell" />
          </a>
          </td>


        <td>

          <div id="{$tabell}" style="display:'none'">


            <table class="tabeller">
              <tbody align="center">
                <xsl:for-each select=".//*">
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

          </div>
        </td>

    </table>
  </xsl:template>
</xsl:stylesheet>
