﻿<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <!-- Created by Asbjørn Østby, Uranus Garange 2021 - Covid-19 pain in the ass-->
  <xsl:output omit-xml-declaration="yes" indent="yes"/>
  <xsl:template match="/">
    <html>
      <head>
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8"/>
        <title>
          <xsl:value-of select="//NAME"/> | <xsl:value-of select="//BREWER"/>
        </title>
        <style>
          hr.bold {
          border: 5px solid black;
          border-radius: 5px;
          }
          hr.solid {
          border: 1px solid black;
          }

          .box {
          display: flex;
          align-items:center;
          }

        </style>
      </head>
      <style>
        body {font-family: sans-serif;}
        td {padding: 4px;}
      </style>
      <body style="font: 14px MuseoSans500, Helvetica, Arial, sans-serif; font-size: 12px;">
        <h3 style="font-size: 40px; font-weight: normal; line-height: 110%; padding: 5px 0 0 0; margin: 0;">
          <hr class="bold"/>
          <div class="box">
            <img src="https://www.problembar.net/wordpress/wp-content/uploads/2022/03/uranus_garage_logo-edited.jpg" alt="Notes" width="150" height="100"/>
            <span style="">
              <xsl:value-of select="//NAME"/>
            </span>
          </div>
          <hr class="bold"/>
        </h3>
        <div style="font-size: 14px; font-weight: normal; padding: 5px 0 10px 5px;">
          <xsl:value-of select="//BREWER"/>
          <hr class="solid"/>
        </div>
        <table>
          <tr>
            <td>Method:</td>
            <td>
              <b>
                <xsl:value-of select="//TYPE"/>
              </b>
            </td>
            <td>Style:</td>
            <td>
              <b>
                <xsl:value-of select="//STYLE/NAME"/>
              </b>
            </td>
          </tr>
          <tr>
            <td>Boil Time:</td>
            <td>
              <b>
                <xsl:call-template name="round">
                  <xsl:with-param name="value" select="//BOIL_TIME"/>
                </xsl:call-template>
                <xsl:text>&#xa;</xsl:text>
                <xsl:text>min</xsl:text>
              </b>
            </td>
            <td>Batch Size:</td>
            <td>
              <b>
                <xsl:call-template name="gallonToLiters">
                  <xsl:with-param name="value" select="//DISPLAY_BATCH_SIZE"/>
                </xsl:call-template>
                <xsl:text>&#xa;</xsl:text>
              </b>
              <xsl:text>&#xa;</xsl:text>
              <span style='font-size: 0.8em; font-style: italic;'>(ending kettle volume)</span>
            </td>
          </tr>
          <tr>
            <td>Boil Size:</td>
            <td>
              <b>
                <xsl:call-template name="round">
                  <xsl:with-param name="value" select="//BOIL_SIZE"/>
                </xsl:call-template>
                <xsl:text>&#xa;</xsl:text>
                <xsl:text>liters</xsl:text>
              </b>
            </td>
            <td>Efficiency:</td>
            <td>
              <b>
                <xsl:call-template name="round">
                  <xsl:with-param name="value" select="//EFFICIENCY"/>
                </xsl:call-template>

                %
              </b>
              <span style='font-size: 0.8em; font-style: italic;'>(ending kettle)</span>
            </td>
          </tr>
          <tr>
            <td>Boil Gravity:</td>
            <td>
              <b>
                <xsl:value-of select="//EST_OG"/>
              </b>
              <span style='font-size: 0.8em; font-style: italic;'> (recipe based estimate)</span>
            </td>
            <td>
              <xsl:text>&#160;</xsl:text>
            </td>
            <td>
              <xsl:text>&#160;</xsl:text>
            </td>
            <td>
              <xsl:text>&#160;</xsl:text>
            </td>
            <td>
              <xsl:text>&#160;</xsl:text>
            </td>
          </tr>
        </table>
        <hr class="solid"/>
        <div style="display: inline-block; width: 100%; padding: 5px 0px 5px 0px; background: #D6D6D6; border-bottom: #8E8E8E 1px solid; border-top: #C8C8C8 1px solid;">
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">Original Gravity:</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;">
              <xsl:value-of select="//EST_OG"/>
            </span>
          </div>
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">Final Gravity:</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;">
              <xsl:value-of select="//EST_FG"/>
            </span>
          </div>
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">ABV (standard):</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;">
              <xsl:variable name="abv">
                <xsl:value-of select="//EST_ABV"/>
              </xsl:variable>
              <xsl:value-of select="translate($abv, '%', '')"/>%
            </span>
          </div>
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">IBU (tinseth):</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;">
              <xsl:value-of select="//IBU"/>
            </span>
          </div>
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">SRM (morey):</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;">
              <xsl:value-of select="//EST_COLOR"/>
            </span>
          </div>
          <div style="display: block; float: left; width: 180px;">
            <span style="display: block; float: left; margin: 4px 4px 0 8px; width: 90px;">Mash pH:</span>
            <span style="display: block; font-size: 20px; float: left; line-height: 100%; font-weight: bold; width: 65px;"/>
          </div>
          <div style="clear: both;"/>
        </div>
        <div style="font-size: 16px; font-weight: normal; padding: 0 0 0 5px;">
          <br/>
          <xsl:if test="//NOTES">
            <xsl:if test="string-length(//NOTES/text())>=1">
              <div style="font-size: 16px; font-weight: normal; padding: 0 0 0 5px;">
                <xsl:call-template name="NOTES"/>
              </div>
            </xsl:if>
          </xsl:if>
          
          <br/>
          <xsl:call-template name="FERMENTABLES"/>
          <br/>
          <xsl:call-template name="HOPS"/>
          <br/>
          <xsl:if test="//MASH">
            <xsl:call-template name="MASH"/>
            <br/>
          </xsl:if>
          <xsl:if test="//MISCS">
            <xsl:call-template name="MISCS"/>
            <br/>
          </xsl:if>
          <xsl:call-template name="YEASTS"/>
          <br/>
          <xsl:if test="//PRIMING_SUGAR_NAME/text()">
            <xsl:call-template name="PRIMING"/>
            <br/>
          </xsl:if>
          <xsl:if test="//WATERS">
            <xsl:call-template name="WATERS"/>
            <br/>
          </xsl:if>
         
        </div>
      </body>
    </html>
  </xsl:template>
  <xsl:template match="RECIPE"/>
  <xsl:template name="FERMENTABLES">
    <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Fermentables</div>
    <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
      <tr>
        <td width="100">
          <img src="https://brewdogrecipes.com/assets/icons/icon-malt.png" alt="Fermentables"/>
        </td>
        <td>
          <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
            <tr style="font-size: 12px;">
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="10%">
                Amount
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="54%">
                Fermentable
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                PPG
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                <xsl:text>&#176;</xsl:text>
                L
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                Yeld %
              </td>
            </tr>
            <xsl:for-each select="//FERMENTABLE">
              <tr style="font-size: 12px; background: #ffffff;">
                <td>
                  <xsl:call-template name="round">
                    <xsl:with-param name="value" select="./AMOUNT"/>
                  </xsl:call-template> Kg
                </td>
                <td>
                  <xsl:value-of select="./NAME"/>
                </td>
                <td>37</td>
                <!-- Needs quality check-->
                <td>
                  <xsl:call-template name="round">
                    <xsl:with-param name="value" select="./COLOR"/>
                  </xsl:call-template>
                </td>
                <td>
                  <xsl:call-template name="round">
                    <xsl:with-param name="value" select="./YIELD"/>
                  </xsl:call-template> %
                </td>
                <!-- Needs quality check-->
                <!--WTF-->
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="HOPS">
    <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Hops</div>
    <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
      <tr>
        <td width="100">
          <img src="https://brewdogrecipes.com/assets/icons/icon-hops.png" alt="Hops"/>
        </td>
        <td>
          <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
            <tr style="font-size: 12px;">
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="10%">
                Amount
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="34%">
                Variety
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="14%">
                Type
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="6%">
                AA
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                Use
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                Time
              </td>
              <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                IBU
              </td>
            </tr>
            <xsl:for-each select="//HOP">
              <tr style="font-size: 12px; background: #ffffff;">
                <td>
                  <xsl:call-template name="kilosToGrams">
                    <xsl:with-param name="value" select="./AMOUNT"/>
                  </xsl:call-template> g
                </td>
                <td>
                  <xsl:value-of select="./NAME"/>
                </td>
                <td>
                  <xsl:value-of select="./FORM"/>
                </td>
                <td>
                  <xsl:call-template name="round">
                    <xsl:with-param name="value" select="./ALPHA"/>
                  </xsl:call-template>
                </td>
                <td>
                  <span style='white-space: nowrap;'>
                    <xsl:value-of select="./USE"/>
                  </span>
                </td>
                <td>
                  <span style='white-space: nowrap;'>
                    <xsl:call-template name="minutesToHours">
                      <xsl:with-param name="value">
                        <xsl:call-template name="round">
                          <xsl:with-param name="value" select="./TIME"/>
                        </xsl:call-template>
                      </xsl:with-param>
                    </xsl:call-template>
                  </span>
                </td>
                <td>--</td>
                <!--WTF-->
              </tr>
            </xsl:for-each>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="MASH">
    <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Mash Guidelines</div>
    <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
      <tr>
        <td width="100">
          <img src="https://brewdogrecipes.com/assets/icons/icon-mash.png" alt="Mash"/>
        </td>
        <td>
          <xsl:for-each select="//MASH_STEPS">
            <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
              <tr style="font-size: 12px;">
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="10%">
                  Amount
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="34%">
                  Description
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="20%">
                  Type
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                  Temp
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="24%">
                  Time
                </td>
              </tr>
              <xsl:for-each select="MASH_STEP">
                <!--/RECIPES/RECIPE/MASH/MASH_STEPS/MASH_STEP-->
                <tr style="font-size: 12px; background: #ffffff;">
                  <td>--</td>
                  <td/>
                  <td>
                    <xsl:value-of select="./TYPE"/>
                  </td>
                  <td>
                    <xsl:call-template name="round">
                      <xsl:with-param name="value" select="./STEP_TEMP"/>
                    </xsl:call-template>
                    <xsl:text>&#xa;</xsl:text>
                    <xsl:text>C</xsl:text>
                  </td>
                  <td>
                    <xsl:call-template name="round">
                      <xsl:with-param name="value" select="./STEP_TIME"/>
                    </xsl:call-template>
                    <xsl:text>&#xa;</xsl:text>
                    <xsl:text>  min</xsl:text>
                  </td>
                </tr>
              </xsl:for-each>
              <tr>
                <td colspan='5'>Starting Mash Thickness: 1.25 qt/lb</td>
              </tr>
            </table>
          </xsl:for-each>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="MISCS">
    <xsl:if test="string-length(./text())>0">
      <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
        <tr>
          <td width="100">
            <img src="https://icon-library.com/images/ingredients-icon/ingredients-icon-8.jpg" alt="Priming" width="100" height="100"/>
          </td>
          <td>
            <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Other Ingredients</div>
            <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
              <tr style="font-size: 12px;">
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="10%">
                  Amount
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="34%">
                  Name
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="20%">
                  Type
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="12%">
                  Use
                </td>
                <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;" width="24%">
                  Time
                </td>
              </tr>
              <xsl:for-each select="//MISCS/MISC">
                <tr style="font-size: 12px; background: #ffffff;">
                  <td>
                    <xsl:call-template name="round">
                      <xsl:with-param name="value" select="./AMOUNT"/>
                    </xsl:call-template> Kg
                  </td>
                  <td>
                    <xsl:value-of select="./NAME"/>
                  </td>
                  <td>
                    <xsl:value-of select="./TYPE"/>
                  </td>
                  <td>
                    <xsl:value-of select="./USE"/>
                  </td>
                  <td>
                    <xsl:value-of select="./TIME"/>
                  </td>
                </tr>
              </xsl:for-each>
            </table>
          </td>
        </tr>
      </table>
    </xsl:if>
  </xsl:template>
  <xsl:template name="YEASTS">
    <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Yeast</div>
    <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
      <tr>
        <td width="100">
          <img src="https://brewdogrecipes.com/assets/icons/icon-yeast.png" alt="Yeasts"/>
        </td>
        <td>
          <xsl:for-each select="//YEAST">
            <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
              <tr style="font-size: 12px;">
                <td style="font-size: 15px;">
                  <xsl:value-of select="./LABORATORY"/>
                  <xsl:text>&#xa;</xsl:text>
                  <xsl:value-of select="./NAME"/>
                </td>
              </tr>
              <tr>
                <td>
                  <table>
                    <tr style="font-size: 12px;">
                      <td style="font-weight: bold; color: #424242;">Attenuation (custom):</td>
                      <td>
                        <xsl:call-template name="round">
                          <xsl:with-param name="value" select="./ATTENUATION"/>
                        </xsl:call-template>
                        <xsl:text>%</xsl:text>
                      </td>
                      <td style="font-weight: bold; color: #424242;">Flocculation:</td>
                      <td>
                        <xsl:value-of select="./FLOCCULATION"/>
                      </td>
                    </tr>
                    <tr style="font-size: 12px;">
                      <td style="font-weight: bold; color: #424242;">Optimum Temp:</td>
                      <td>
                        <xsl:call-template name="round">
                          <xsl:with-param name="value" select="./MIN_TEMPERATURE"/>
                        </xsl:call-template>
                        -
                        <xsl:call-template name="round">
                          <xsl:with-param name="value" select="./MAX_TEMPERATURE"/>
                        </xsl:call-template> 								  &#176; C
                      </td>
                      <td style="font-weight: bold; color: #424242;">Starter:</td>
                      <td>
                        <xsl:value-of select="//YEAST_STARTER"/>
                      </td>
                    </tr>
                    <tr style="font-size: 12px;">
                      <td style="font-weight: bold; color: #424242;">Fermentation Temp:</td>
                      <td>
                        <xsl:call-template name="round">
                          <xsl:with-param name="value" select="./PRIMARY_TEMP"/>
                        </xsl:call-template>  &#176; C
                      </td>
                      <td style="font-weight: bold; color: #424242;">Pitch Rate:</td>
                      <td>
                        <xsl:value-of select="//PITCH_RATE"/> &#176;
                        <i>
                          <span style='white-space: nowrap;'>(M cells / ml / &#176; P)</span>
                        </i>
                      </td>
                    </tr>
                  </table>
                </td>
              </tr>
            </table>
          </xsl:for-each>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="PRIMING">
    <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Priming</div>
    <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
      <tr>
        <td width="100">
          <img src="https://www.greatwesternmalting.com/wp-content/uploads/2021/02/BeerIcon.png" alt="Priming" width="100" height="100"/>
        </td>
        <td>
          <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
            <tr style="font-size: 12px;">
              <td>
                Method:	<xsl:value-of select="//PRIMING_SUGAR_NAME"/>
                &#176; &#176; &#176; &#176; &#176; Amount:<xsl:value-of select="//BF_PRIMING_AMOUNT"/>
                <!--g sukker/L-->
                &#176; &#176; &#176; &#176; &#176; CO2 Level: Method:	<xsl:value-of select="//BF_CO2_LEVEL"/> Volumes
              </td>
            </tr>
          </table>
        </td>
      </tr>
    </table>
  </xsl:template>
  <xsl:template name="NOTES">
   
      <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">Notes</div>
      <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
        <tr>
          <td width="100">
            <img src="https://static.vecteezy.com/system/resources/previews/000/498/536/non_2x/vector-notes-icon-design.jpg" alt="Notes" width="100" height="100"/>
          </td>
          <td>
            <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
              <tr style="font-size: 12px;">
                <td style="font-size: 15px;">
                  <xsl:call-template name="replace">
                    <xsl:with-param name="string" select="//NOTES"/>
                  </xsl:call-template>
                </td>
              </tr>
            </table>
          </td>
        </tr>
      </table>
    
  </xsl:template>
  <xsl:template name="WATERS">
    <xsl:for-each select="//WATERS">
      <xsl:if test="string-length(.)>0">
        <table name="ingredience" cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
          <tr>
            <td width="100">
              <img src="https://icon-library.com/images/water-icon-png/water-icon-png-28.jpg" alt="Priming" width="100" height="100"/>
            </td>
            <td>
              <xsl:for-each select="WATER">
                <div style="border-bottom: #777 1px solid; font-size: 18px; font-weight: bold; padding: 5px 5px 0 5px; margin: 0 0 5px 0; text-align: left;">
                  Target Water Profile: <xsl:value-of select="./NAME"/>
                </div>
                <table cellspacing="0" cellpadding="4" style="margin-bottom: 10px; background-color: #EEE; border: #D2D2D2 1px solid; width: 100%; border-spacing: 0px 0px; padding: 5px; text-align: left;">
                  <tr style="font-size: 12px;">
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      Ca<sup>+2</sup>
                    </td>
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      Mg<sup>+2</sup>
                    </td>
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      Na<sup>+</sup>
                    </td>
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      Cl<sup>-</sup>
                    </td>
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      SO<sub>4</sub>
                      <sup>-2</sup>
                    </td>
                    <td style="border-bottom: #424242 1px solid; text-align: left; font-weight: bold; color: #424242;">
                      HCO<sub>3</sub>
                      <sup>-</sup>
                    </td>
                  </tr>
                  <tr style="font-size: 12px;">
                    <td>
                      <xsl:value-of select="./CALCIUM"/>
                    </td>
                    <td>
                      <xsl:value-of select="./MAGNESIUM"/>
                    </td>
                    <td>
                      <xsl:value-of select="./SODIUM"/>
                    </td>
                    <td>
                      <xsl:value-of select="./CHLORIDE"/>
                    </td>
                    <td>
                      <xsl:value-of select="./SULFATE"/>
                    </td>
                    <td>
                      <xsl:value-of select="./BICARBONATE"/>
                    </td>
                  </tr>
                  <xsl:if test="string-length(./NOTES)>0">
                    <xsl:value-of select="./NOTES"/>
                  </xsl:if>
                  <!--AMOUNT???-->
                </table>
              </xsl:for-each>
            </td>
          </tr>
        </table>
      </xsl:if>
    </xsl:for-each>
  </xsl:template>

  <xsl:template name="replace">
    <xsl:param name="string"/>
    <xsl:choose>
      <xsl:when test="contains($string,'&#13;')">
        <xsl:value-of select="substring-before($string,'&#13;')"/>
        <br/>
        <xsl:call-template name="replace">
          <xsl:with-param name="string" select="substring-after($string,'&#13;')"/>
        </xsl:call-template>
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$string"/>
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="minutesToHours">
    <xsl:param name="value"/>
    <xsl:choose>
      <xsl:when test="($value div 60 div 24)>1">
        <xsl:value-of select='$value div 60 div 24'/> dager
      </xsl:when>
      <xsl:when test="$value>60">
        <xsl:value-of select='format-number(($value) div 60 ,"##0.00")'/> timer
      </xsl:when>
      <xsl:otherwise>
        <xsl:value-of select="$value"/> minutter
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
  <xsl:template name="kilosToGrams">
    <xsl:param name="value"/>
    <xsl:value-of select='format-number(($value * 1000) ,"##0.00")'/>
  </xsl:template>
  <xsl:template name="round">
    <xsl:param name="value"/>
    <xsl:value-of select='format-number( round(100*$value) div 100 ,"##0.00" )'/>
  </xsl:template>
  <xsl:template name="gallonToLiters">
    <xsl:param name="value"/>
    <xsl:choose>
      <xsl:when test="not(contains($value, 'gal'))">
        <xsl:value-of select="$value"/>
      </xsl:when>
      <xsl:otherwise>
        <xsl:variable name="liters">
          <xsl:call-template name="round">
            <xsl:with-param name="value" select="translate($value, 'gal', '')* 3.785411784"/>
          </xsl:call-template>
        </xsl:variable>
        <xsl:value-of select="$liters"/> Liters
      </xsl:otherwise>
    </xsl:choose>
  </xsl:template>
</xsl:stylesheet>


