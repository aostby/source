using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kolibri.Common.MovieAPI.Entities
{
  
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class videodb
    {

        private byte versionField;

        private videodbMovie[] movieField;

        private videodbTvshow[] tvshowField;

        private videodbPath[] pathsField;

        /// <remarks/>
        public byte version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("movie")]
        public videodbMovie[] movie
        {
            get
            {
                return this.movieField;
            }
            set
            {
                this.movieField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tvshow")]
        public videodbTvshow[] tvshow
        {
            get
            {
                return this.tvshowField;
            }
            set
            {
                this.tvshowField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("path", IsNullable = false)]
        public videodbPath[] paths
        {
            get
            {
                return this.pathsField;
            }
            set
            {
                this.pathsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovie
    {

        private string titleField;

        private string originaltitleField;

        private videodbMovieRatings ratingsField;

        private byte userratingField;

        private byte top250Field;

        private object outlineField;

        private string plotField;

        private string taglineField;

        private ushort runtimeField;

        private videodbMovieThumb[] thumbField;

        private videodbMovieThumb1[] fanartField;

        private string mpaaField;

        private byte playcountField;

        private string lastplayedField;

        private object fileField;

        private string pathField;

        private string filenameandpathField;

        private string basepathField;

        private uint idField;

        private videodbMovieUniqueid[] uniqueidField;

        private string[] genreField;

        private string[] countryField;

        private videodbMovieSet setField;

        private string[] tagField;

        private string videoassettitleField;

        private ushort videoassetidField;

        private byte videoassettypeField;

        private bool hasvideoversionsField;

        private bool hasvideoextrasField;

        private bool isdefaultvideoversionField;

        private string[] creditsField;

        private string[] directorField;

        private System.DateTime premieredField;

        private bool premieredFieldSpecified;

        private ushort yearField;

        private bool yearFieldSpecified;

        private object statusField;

        private object codeField;

        private object airedField;

        private string[] studioField;

        private string trailerField;

        private videodbMovieFileinfo fileinfoField;

        private videodbMovieActor[] actorField;

        private videodbMovieResume resumeField;

        private string dateaddedField;

        private videodbMovieArt artField;

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string originaltitle
        {
            get
            {
                return this.originaltitleField;
            }
            set
            {
                this.originaltitleField = value;
            }
        }

        /// <remarks/>
        public videodbMovieRatings ratings
        {
            get
            {
                return this.ratingsField;
            }
            set
            {
                this.ratingsField = value;
            }
        }

        /// <remarks/>
        public byte userrating
        {
            get
            {
                return this.userratingField;
            }
            set
            {
                this.userratingField = value;
            }
        }

        /// <remarks/>
        public byte top250
        {
            get
            {
                return this.top250Field;
            }
            set
            {
                this.top250Field = value;
            }
        }

        /// <remarks/>
        public object outline
        {
            get
            {
                return this.outlineField;
            }
            set
            {
                this.outlineField = value;
            }
        }

        /// <remarks/>
        public string plot
        {
            get
            {
                return this.plotField;
            }
            set
            {
                this.plotField = value;
            }
        }

        /// <remarks/>
        public string tagline
        {
            get
            {
                return this.taglineField;
            }
            set
            {
                this.taglineField = value;
            }
        }

        /// <remarks/>
        public ushort runtime
        {
            get
            {
                return this.runtimeField;
            }
            set
            {
                this.runtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("thumb")]
        public videodbMovieThumb[] thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("thumb", IsNullable = false)]
        public videodbMovieThumb1[] fanart
        {
            get
            {
                return this.fanartField;
            }
            set
            {
                this.fanartField = value;
            }
        }

        /// <remarks/>
        public string mpaa
        {
            get
            {
                return this.mpaaField;
            }
            set
            {
                this.mpaaField = value;
            }
        }

        /// <remarks/>
        public byte playcount
        {
            get
            {
                return this.playcountField;
            }
            set
            {
                this.playcountField = value;
            }
        }

        /// <remarks/>
        public string lastplayed
        {
            get
            {
                return this.lastplayedField;
            }
            set
            {
                this.lastplayedField = value;
            }
        }

        /// <remarks/>
        public object file
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        public string filenameandpath
        {
            get
            {
                return this.filenameandpathField;
            }
            set
            {
                this.filenameandpathField = value;
            }
        }

        /// <remarks/>
        public string basepath
        {
            get
            {
                return this.basepathField;
            }
            set
            {
                this.basepathField = value;
            }
        }

        /// <remarks/>
        public uint id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("uniqueid")]
        public videodbMovieUniqueid[] uniqueid
        {
            get
            {
                return this.uniqueidField;
            }
            set
            {
                this.uniqueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("genre")]
        public string[] genre
        {
            get
            {
                return this.genreField;
            }
            set
            {
                this.genreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("country")]
        public string[] country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        public videodbMovieSet set
        {
            get
            {
                return this.setField;
            }
            set
            {
                this.setField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public string[] tag
        {
            get
            {
                return this.tagField;
            }
            set
            {
                this.tagField = value;
            }
        }

        /// <remarks/>
        public string videoassettitle
        {
            get
            {
                return this.videoassettitleField;
            }
            set
            {
                this.videoassettitleField = value;
            }
        }

        /// <remarks/>
        public ushort videoassetid
        {
            get
            {
                return this.videoassetidField;
            }
            set
            {
                this.videoassetidField = value;
            }
        }

        /// <remarks/>
        public byte videoassettype
        {
            get
            {
                return this.videoassettypeField;
            }
            set
            {
                this.videoassettypeField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoversions
        {
            get
            {
                return this.hasvideoversionsField;
            }
            set
            {
                this.hasvideoversionsField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoextras
        {
            get
            {
                return this.hasvideoextrasField;
            }
            set
            {
                this.hasvideoextrasField = value;
            }
        }

        /// <remarks/>
        public bool isdefaultvideoversion
        {
            get
            {
                return this.isdefaultvideoversionField;
            }
            set
            {
                this.isdefaultvideoversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("credits")]
        public string[] credits
        {
            get
            {
                return this.creditsField;
            }
            set
            {
                this.creditsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("director")]
        public string[] director
        {
            get
            {
                return this.directorField;
            }
            set
            {
                this.directorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime premiered
        {
            get
            {
                return this.premieredField;
            }
            set
            {
                this.premieredField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool premieredSpecified
        {
            get
            {
                return this.premieredFieldSpecified;
            }
            set
            {
                this.premieredFieldSpecified = value;
            }
        }

        /// <remarks/>
        public ushort year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool yearSpecified
        {
            get
            {
                return this.yearFieldSpecified;
            }
            set
            {
                this.yearFieldSpecified = value;
            }
        }

        /// <remarks/>
        public object status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public object code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public object aired
        {
            get
            {
                return this.airedField;
            }
            set
            {
                this.airedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("studio")]
        public string[] studio
        {
            get
            {
                return this.studioField;
            }
            set
            {
                this.studioField = value;
            }
        }

        /// <remarks/>
        public string trailer
        {
            get
            {
                return this.trailerField;
            }
            set
            {
                this.trailerField = value;
            }
        }

        /// <remarks/>
        public videodbMovieFileinfo fileinfo
        {
            get
            {
                return this.fileinfoField;
            }
            set
            {
                this.fileinfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("actor")]
        public videodbMovieActor[] actor
        {
            get
            {
                return this.actorField;
            }
            set
            {
                this.actorField = value;
            }
        }

        /// <remarks/>
        public videodbMovieResume resume
        {
            get
            {
                return this.resumeField;
            }
            set
            {
                this.resumeField = value;
            }
        }

        /// <remarks/>
        public string dateadded
        {
            get
            {
                return this.dateaddedField;
            }
            set
            {
                this.dateaddedField = value;
            }
        }

        /// <remarks/>
        public videodbMovieArt art
        {
            get
            {
                return this.artField;
            }
            set
            {
                this.artField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieRatings
    {

        private videodbMovieRatingsRating ratingField;

        /// <remarks/>
        public videodbMovieRatingsRating rating
        {
            get
            {
                return this.ratingField;
            }
            set
            {
                this.ratingField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieRatingsRating
    {

        private decimal valueField;

        private ushort votesField;

        private string nameField;

        private byte maxField;

        private bool defaultField;

        private bool defaultFieldSpecified;

        /// <remarks/>
        public decimal value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        public ushort votes
        {
            get
            {
                return this.votesField;
            }
            set
            {
                this.votesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte max
        {
            get
            {
                return this.maxField;
            }
            set
            {
                this.maxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool defaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieThumb
    {

        private string spoofField;

        private string cacheField;

        private string aspectField;

        private string previewField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string spoof
        {
            get
            {
                return this.spoofField;
            }
            set
            {
                this.spoofField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cache
        {
            get
            {
                return this.cacheField;
            }
            set
            {
                this.cacheField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string aspect
        {
            get
            {
                return this.aspectField;
            }
            set
            {
                this.aspectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string preview
        {
            get
            {
                return this.previewField;
            }
            set
            {
                this.previewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieThumb1
    {

        private string colorsField;

        private string previewField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string colors
        {
            get
            {
                return this.colorsField;
            }
            set
            {
                this.colorsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string preview
        {
            get
            {
                return this.previewField;
            }
            set
            {
                this.previewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieUniqueid
    {

        private string typeField;

        private bool defaultField;

        private bool defaultFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool defaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieSet
    {

        private string nameField;

        private string overviewField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string overview
        {
            get
            {
                return this.overviewField;
            }
            set
            {
                this.overviewField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieFileinfo
    {

        private videodbMovieFileinfoStreamdetails streamdetailsField;

        /// <remarks/>
        public videodbMovieFileinfoStreamdetails streamdetails
        {
            get
            {
                return this.streamdetailsField;
            }
            set
            {
                this.streamdetailsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieFileinfoStreamdetails
    {

        private videodbMovieFileinfoStreamdetailsVideo videoField;

        private videodbMovieFileinfoStreamdetailsAudio[] audioField;

        private videodbMovieFileinfoStreamdetailsSubtitle[] subtitleField;

        /// <remarks/>
        public videodbMovieFileinfoStreamdetailsVideo video
        {
            get
            {
                return this.videoField;
            }
            set
            {
                this.videoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("audio")]
        public videodbMovieFileinfoStreamdetailsAudio[] audio
        {
            get
            {
                return this.audioField;
            }
            set
            {
                this.audioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subtitle")]
        public videodbMovieFileinfoStreamdetailsSubtitle[] subtitle
        {
            get
            {
                return this.subtitleField;
            }
            set
            {
                this.subtitleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieFileinfoStreamdetailsVideo
    {

        private string codecField;

        private decimal aspectField;

        private ushort widthField;

        private ushort heightField;

        private ushort durationinsecondsField;

        private object stereomodeField;

        private string hdrtypeField;

        /// <remarks/>
        public string codec
        {
            get
            {
                return this.codecField;
            }
            set
            {
                this.codecField = value;
            }
        }

        /// <remarks/>
        public decimal aspect
        {
            get
            {
                return this.aspectField;
            }
            set
            {
                this.aspectField = value;
            }
        }

        /// <remarks/>
        public ushort width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        public ushort height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        public ushort durationinseconds
        {
            get
            {
                return this.durationinsecondsField;
            }
            set
            {
                this.durationinsecondsField = value;
            }
        }

        /// <remarks/>
        public object stereomode
        {
            get
            {
                return this.stereomodeField;
            }
            set
            {
                this.stereomodeField = value;
            }
        }

        /// <remarks/>
        public string hdrtype
        {
            get
            {
                return this.hdrtypeField;
            }
            set
            {
                this.hdrtypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieFileinfoStreamdetailsAudio
    {

        private string codecField;

        private string languageField;

        private byte channelsField;

        /// <remarks/>
        public string codec
        {
            get
            {
                return this.codecField;
            }
            set
            {
                this.codecField = value;
            }
        }

        /// <remarks/>
        public string language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        public byte channels
        {
            get
            {
                return this.channelsField;
            }
            set
            {
                this.channelsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieFileinfoStreamdetailsSubtitle
    {

        private string languageField;

        /// <remarks/>
        public string language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieActor
    {

        private string nameField;

        private string roleField;

        private ushort orderField;

        private string thumbField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        public ushort order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieResume
    {

        private decimal positionField;

        private decimal totalField;

        /// <remarks/>
        public decimal position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public decimal total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbMovieArt
    {

        private string bannerField;

        private string clearartField;

        private string clearlogoField;

        private string discartField;

        private string fanartField;

        private string keyartField;

        private string landscapeField;

        private string posterField;

        private string thumbField;

        /// <remarks/>
        public string banner
        {
            get
            {
                return this.bannerField;
            }
            set
            {
                this.bannerField = value;
            }
        }

        /// <remarks/>
        public string clearart
        {
            get
            {
                return this.clearartField;
            }
            set
            {
                this.clearartField = value;
            }
        }

        /// <remarks/>
        public string clearlogo
        {
            get
            {
                return this.clearlogoField;
            }
            set
            {
                this.clearlogoField = value;
            }
        }

        /// <remarks/>
        public string discart
        {
            get
            {
                return this.discartField;
            }
            set
            {
                this.discartField = value;
            }
        }

        /// <remarks/>
        public string fanart
        {
            get
            {
                return this.fanartField;
            }
            set
            {
                this.fanartField = value;
            }
        }

        /// <remarks/>
        public string keyart
        {
            get
            {
                return this.keyartField;
            }
            set
            {
                this.keyartField = value;
            }
        }

        /// <remarks/>
        public string landscape
        {
            get
            {
                return this.landscapeField;
            }
            set
            {
                this.landscapeField = value;
            }
        }

        /// <remarks/>
        public string poster
        {
            get
            {
                return this.posterField;
            }
            set
            {
                this.posterField = value;
            }
        }

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshow
    {

        private string titleField;

        private string originaltitleField;

        private string showtitleField;

        private videodbTvshowRatings ratingsField;

        private byte userratingField;

        private byte top250Field;

        private byte seasonField;

        private byte episodeField;

        private sbyte displayseasonField;

        private sbyte displayepisodeField;

        private object outlineField;

        private string plotField;

        private object taglineField;

        private byte runtimeField;

        private videodbTvshowThumb[] thumbField;

        private videodbTvshowThumb1[] fanartField;

        private string mpaaField;

        private byte playcountField;

        private string lastplayedField;

        private object fileField;

        private string pathField;

        private object filenameandpathField;

        private string basepathField;

        private string episodeguideField;

        private uint idField;

        private videodbTvshowUniqueid[] uniqueidField;

        private string[] genreField;

        private string[] tagField;

        private object videoassettitleField;

        private sbyte videoassetidField;

        private sbyte videoassettypeField;

        private bool hasvideoversionsField;

        private bool hasvideoextrasField;

        private bool isdefaultvideoversionField;

        private System.DateTime premieredField;

        private ushort yearField;

        private string statusField;

        private object codeField;

        private object airedField;

        private string studioField;

        private string trailerField;

        private videodbTvshowActor[] actorField;

        private videodbTvshowNamedseason[] namedseasonField;

        private videodbTvshowResume resumeField;

        private string dateaddedField;

        private videodbTvshowArt artField;

        private videodbTvshowEpisodedetails[] episodedetailsField;

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string originaltitle
        {
            get
            {
                return this.originaltitleField;
            }
            set
            {
                this.originaltitleField = value;
            }
        }

        /// <remarks/>
        public string showtitle
        {
            get
            {
                return this.showtitleField;
            }
            set
            {
                this.showtitleField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowRatings ratings
        {
            get
            {
                return this.ratingsField;
            }
            set
            {
                this.ratingsField = value;
            }
        }

        /// <remarks/>
        public byte userrating
        {
            get
            {
                return this.userratingField;
            }
            set
            {
                this.userratingField = value;
            }
        }

        /// <remarks/>
        public byte top250
        {
            get
            {
                return this.top250Field;
            }
            set
            {
                this.top250Field = value;
            }
        }

        /// <remarks/>
        public byte season
        {
            get
            {
                return this.seasonField;
            }
            set
            {
                this.seasonField = value;
            }
        }

        /// <remarks/>
        public byte episode
        {
            get
            {
                return this.episodeField;
            }
            set
            {
                this.episodeField = value;
            }
        }

        /// <remarks/>
        public sbyte displayseason
        {
            get
            {
                return this.displayseasonField;
            }
            set
            {
                this.displayseasonField = value;
            }
        }

        /// <remarks/>
        public sbyte displayepisode
        {
            get
            {
                return this.displayepisodeField;
            }
            set
            {
                this.displayepisodeField = value;
            }
        }

        /// <remarks/>
        public object outline
        {
            get
            {
                return this.outlineField;
            }
            set
            {
                this.outlineField = value;
            }
        }

        /// <remarks/>
        public string plot
        {
            get
            {
                return this.plotField;
            }
            set
            {
                this.plotField = value;
            }
        }

        /// <remarks/>
        public object tagline
        {
            get
            {
                return this.taglineField;
            }
            set
            {
                this.taglineField = value;
            }
        }

        /// <remarks/>
        public byte runtime
        {
            get
            {
                return this.runtimeField;
            }
            set
            {
                this.runtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("thumb")]
        public videodbTvshowThumb[] thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("thumb", IsNullable = false)]
        public videodbTvshowThumb1[] fanart
        {
            get
            {
                return this.fanartField;
            }
            set
            {
                this.fanartField = value;
            }
        }

        /// <remarks/>
        public string mpaa
        {
            get
            {
                return this.mpaaField;
            }
            set
            {
                this.mpaaField = value;
            }
        }

        /// <remarks/>
        public byte playcount
        {
            get
            {
                return this.playcountField;
            }
            set
            {
                this.playcountField = value;
            }
        }

        /// <remarks/>
        public string lastplayed
        {
            get
            {
                return this.lastplayedField;
            }
            set
            {
                this.lastplayedField = value;
            }
        }

        /// <remarks/>
        public object file
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        public object filenameandpath
        {
            get
            {
                return this.filenameandpathField;
            }
            set
            {
                this.filenameandpathField = value;
            }
        }

        /// <remarks/>
        public string basepath
        {
            get
            {
                return this.basepathField;
            }
            set
            {
                this.basepathField = value;
            }
        }

        /// <remarks/>
        public string episodeguide
        {
            get
            {
                return this.episodeguideField;
            }
            set
            {
                this.episodeguideField = value;
            }
        }

        /// <remarks/>
        public uint id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("uniqueid")]
        public videodbTvshowUniqueid[] uniqueid
        {
            get
            {
                return this.uniqueidField;
            }
            set
            {
                this.uniqueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("genre")]
        public string[] genre
        {
            get
            {
                return this.genreField;
            }
            set
            {
                this.genreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("tag")]
        public string[] tag
        {
            get
            {
                return this.tagField;
            }
            set
            {
                this.tagField = value;
            }
        }

        /// <remarks/>
        public object videoassettitle
        {
            get
            {
                return this.videoassettitleField;
            }
            set
            {
                this.videoassettitleField = value;
            }
        }

        /// <remarks/>
        public sbyte videoassetid
        {
            get
            {
                return this.videoassetidField;
            }
            set
            {
                this.videoassetidField = value;
            }
        }

        /// <remarks/>
        public sbyte videoassettype
        {
            get
            {
                return this.videoassettypeField;
            }
            set
            {
                this.videoassettypeField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoversions
        {
            get
            {
                return this.hasvideoversionsField;
            }
            set
            {
                this.hasvideoversionsField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoextras
        {
            get
            {
                return this.hasvideoextrasField;
            }
            set
            {
                this.hasvideoextrasField = value;
            }
        }

        /// <remarks/>
        public bool isdefaultvideoversion
        {
            get
            {
                return this.isdefaultvideoversionField;
            }
            set
            {
                this.isdefaultvideoversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime premiered
        {
            get
            {
                return this.premieredField;
            }
            set
            {
                this.premieredField = value;
            }
        }

        /// <remarks/>
        public ushort year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        /// <remarks/>
        public string status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public object code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        public object aired
        {
            get
            {
                return this.airedField;
            }
            set
            {
                this.airedField = value;
            }
        }

        /// <remarks/>
        public string studio
        {
            get
            {
                return this.studioField;
            }
            set
            {
                this.studioField = value;
            }
        }

        /// <remarks/>
        public string trailer
        {
            get
            {
                return this.trailerField;
            }
            set
            {
                this.trailerField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("actor")]
        public videodbTvshowActor[] actor
        {
            get
            {
                return this.actorField;
            }
            set
            {
                this.actorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("namedseason")]
        public videodbTvshowNamedseason[] namedseason
        {
            get
            {
                return this.namedseasonField;
            }
            set
            {
                this.namedseasonField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowResume resume
        {
            get
            {
                return this.resumeField;
            }
            set
            {
                this.resumeField = value;
            }
        }

        /// <remarks/>
        public string dateadded
        {
            get
            {
                return this.dateaddedField;
            }
            set
            {
                this.dateaddedField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowArt art
        {
            get
            {
                return this.artField;
            }
            set
            {
                this.artField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("episodedetails")]
        public videodbTvshowEpisodedetails[] episodedetails
        {
            get
            {
                return this.episodedetailsField;
            }
            set
            {
                this.episodedetailsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowRatings
    {

        private videodbTvshowRatingsRating ratingField;

        /// <remarks/>
        public videodbTvshowRatingsRating rating
        {
            get
            {
                return this.ratingField;
            }
            set
            {
                this.ratingField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowRatingsRating
    {

        private decimal valueField;

        private ushort votesField;

        private string nameField;

        private byte maxField;

        private bool defaultField;

        /// <remarks/>
        public decimal value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        public ushort votes
        {
            get
            {
                return this.votesField;
            }
            set
            {
                this.votesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte max
        {
            get
            {
                return this.maxField;
            }
            set
            {
                this.maxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowThumb
    {

        private string spoofField;

        private string cacheField;

        private string aspectField;

        private string previewField;

        private byte seasonField;

        private bool seasonFieldSpecified;

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string spoof
        {
            get
            {
                return this.spoofField;
            }
            set
            {
                this.spoofField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cache
        {
            get
            {
                return this.cacheField;
            }
            set
            {
                this.cacheField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string aspect
        {
            get
            {
                return this.aspectField;
            }
            set
            {
                this.aspectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string preview
        {
            get
            {
                return this.previewField;
            }
            set
            {
                this.previewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte season
        {
            get
            {
                return this.seasonField;
            }
            set
            {
                this.seasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool seasonSpecified
        {
            get
            {
                return this.seasonFieldSpecified;
            }
            set
            {
                this.seasonFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowThumb1
    {

        private string colorsField;

        private string previewField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string colors
        {
            get
            {
                return this.colorsField;
            }
            set
            {
                this.colorsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string preview
        {
            get
            {
                return this.previewField;
            }
            set
            {
                this.previewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowUniqueid
    {

        private string typeField;

        private bool defaultField;

        private bool defaultFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool defaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowActor
    {

        private string nameField;

        private string roleField;

        private byte orderField;

        private string thumbField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        public byte order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowNamedseason
    {

        private byte numberField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte number
        {
            get
            {
                return this.numberField;
            }
            set
            {
                this.numberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowResume
    {

        private decimal positionField;

        private decimal totalField;

        /// <remarks/>
        public decimal position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public decimal total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowArt
    {

        private string bannerField;

        private string characterartField;

        private string clearartField;

        private string clearlogoField;

        private string fanartField;

        private string landscapeField;

        private string posterField;

        private videodbTvshowArtSeason[] seasonField;

        /// <remarks/>
        public string banner
        {
            get
            {
                return this.bannerField;
            }
            set
            {
                this.bannerField = value;
            }
        }

        /// <remarks/>
        public string characterart
        {
            get
            {
                return this.characterartField;
            }
            set
            {
                this.characterartField = value;
            }
        }

        /// <remarks/>
        public string clearart
        {
            get
            {
                return this.clearartField;
            }
            set
            {
                this.clearartField = value;
            }
        }

        /// <remarks/>
        public string clearlogo
        {
            get
            {
                return this.clearlogoField;
            }
            set
            {
                this.clearlogoField = value;
            }
        }

        /// <remarks/>
        public string fanart
        {
            get
            {
                return this.fanartField;
            }
            set
            {
                this.fanartField = value;
            }
        }

        /// <remarks/>
        public string landscape
        {
            get
            {
                return this.landscapeField;
            }
            set
            {
                this.landscapeField = value;
            }
        }

        /// <remarks/>
        public string poster
        {
            get
            {
                return this.posterField;
            }
            set
            {
                this.posterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("season")]
        public videodbTvshowArtSeason[] season
        {
            get
            {
                return this.seasonField;
            }
            set
            {
                this.seasonField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowArtSeason
    {

        private string bannerField;

        private string landscapeField;

        private string posterField;

        private sbyte numField;

        /// <remarks/>
        public string banner
        {
            get
            {
                return this.bannerField;
            }
            set
            {
                this.bannerField = value;
            }
        }

        /// <remarks/>
        public string landscape
        {
            get
            {
                return this.landscapeField;
            }
            set
            {
                this.landscapeField = value;
            }
        }

        /// <remarks/>
        public string poster
        {
            get
            {
                return this.posterField;
            }
            set
            {
                this.posterField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public sbyte num
        {
            get
            {
                return this.numField;
            }
            set
            {
                this.numField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetails
    {

        private string titleField;

        private string showtitleField;

        private videodbTvshowEpisodedetailsRatings ratingsField;

        private byte userratingField;

        private byte top250Field;

        private byte seasonField;

        private byte episodeField;

        private sbyte displayseasonField;

        private sbyte displayepisodeField;

        private object outlineField;

        private string plotField;

        private object taglineField;

        private byte runtimeField;

        private videodbTvshowEpisodedetailsThumb[] thumbField;

        private string mpaaField;

        private byte playcountField;

        private string lastplayedField;

        private object fileField;

        private string pathField;

        private string filenameandpathField;

        private string basepathField;

        private uint idField;

        private videodbTvshowEpisodedetailsUniqueid[] uniqueidField;

        private string[] genreField;

        private object videoassettitleField;

        private sbyte videoassetidField;

        private sbyte videoassettypeField;

        private bool hasvideoversionsField;

        private bool hasvideoextrasField;

        private bool isdefaultvideoversionField;

        private string[] creditsField;

        private string[] directorField;

        private System.DateTime premieredField;

        private ushort yearField;

        private object statusField;

        private object codeField;

        private System.DateTime airedField;

        private string studioField;

        private object trailerField;

        private videodbTvshowEpisodedetailsFileinfo fileinfoField;

        private videodbTvshowEpisodedetailsActor[] actorField;

        private videodbTvshowEpisodedetailsResume resumeField;

        private string dateaddedField;

        private videodbTvshowEpisodedetailsArt artField;

        /// <remarks/>
        public string title
        {
            get
            {
                return this.titleField;
            }
            set
            {
                this.titleField = value;
            }
        }

        /// <remarks/>
        public string showtitle
        {
            get
            {
                return this.showtitleField;
            }
            set
            {
                this.showtitleField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowEpisodedetailsRatings ratings
        {
            get
            {
                return this.ratingsField;
            }
            set
            {
                this.ratingsField = value;
            }
        }

        /// <remarks/>
        public byte userrating
        {
            get
            {
                return this.userratingField;
            }
            set
            {
                this.userratingField = value;
            }
        }

        /// <remarks/>
        public byte top250
        {
            get
            {
                return this.top250Field;
            }
            set
            {
                this.top250Field = value;
            }
        }

        /// <remarks/>
        public byte season
        {
            get
            {
                return this.seasonField;
            }
            set
            {
                this.seasonField = value;
            }
        }

        /// <remarks/>
        public byte episode
        {
            get
            {
                return this.episodeField;
            }
            set
            {
                this.episodeField = value;
            }
        }

        /// <remarks/>
        public sbyte displayseason
        {
            get
            {
                return this.displayseasonField;
            }
            set
            {
                this.displayseasonField = value;
            }
        }

        /// <remarks/>
        public sbyte displayepisode
        {
            get
            {
                return this.displayepisodeField;
            }
            set
            {
                this.displayepisodeField = value;
            }
        }

        /// <remarks/>
        public object outline
        {
            get
            {
                return this.outlineField;
            }
            set
            {
                this.outlineField = value;
            }
        }

        /// <remarks/>
        public string plot
        {
            get
            {
                return this.plotField;
            }
            set
            {
                this.plotField = value;
            }
        }

        /// <remarks/>
        public object tagline
        {
            get
            {
                return this.taglineField;
            }
            set
            {
                this.taglineField = value;
            }
        }

        /// <remarks/>
        public byte runtime
        {
            get
            {
                return this.runtimeField;
            }
            set
            {
                this.runtimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("thumb")]
        public videodbTvshowEpisodedetailsThumb[] thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }

        /// <remarks/>
        public string mpaa
        {
            get
            {
                return this.mpaaField;
            }
            set
            {
                this.mpaaField = value;
            }
        }

        /// <remarks/>
        public byte playcount
        {
            get
            {
                return this.playcountField;
            }
            set
            {
                this.playcountField = value;
            }
        }

        /// <remarks/>
        public string lastplayed
        {
            get
            {
                return this.lastplayedField;
            }
            set
            {
                this.lastplayedField = value;
            }
        }

        /// <remarks/>
        public object file
        {
            get
            {
                return this.fileField;
            }
            set
            {
                this.fileField = value;
            }
        }

        /// <remarks/>
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }

        /// <remarks/>
        public string filenameandpath
        {
            get
            {
                return this.filenameandpathField;
            }
            set
            {
                this.filenameandpathField = value;
            }
        }

        /// <remarks/>
        public string basepath
        {
            get
            {
                return this.basepathField;
            }
            set
            {
                this.basepathField = value;
            }
        }

        /// <remarks/>
        public uint id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("uniqueid")]
        public videodbTvshowEpisodedetailsUniqueid[] uniqueid
        {
            get
            {
                return this.uniqueidField;
            }
            set
            {
                this.uniqueidField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("genre")]
        public string[] genre
        {
            get
            {
                return this.genreField;
            }
            set
            {
                this.genreField = value;
            }
        }

        /// <remarks/>
        public object videoassettitle
        {
            get
            {
                return this.videoassettitleField;
            }
            set
            {
                this.videoassettitleField = value;
            }
        }

        /// <remarks/>
        public sbyte videoassetid
        {
            get
            {
                return this.videoassetidField;
            }
            set
            {
                this.videoassetidField = value;
            }
        }

        /// <remarks/>
        public sbyte videoassettype
        {
            get
            {
                return this.videoassettypeField;
            }
            set
            {
                this.videoassettypeField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoversions
        {
            get
            {
                return this.hasvideoversionsField;
            }
            set
            {
                this.hasvideoversionsField = value;
            }
        }

        /// <remarks/>
        public bool hasvideoextras
        {
            get
            {
                return this.hasvideoextrasField;
            }
            set
            {
                this.hasvideoextrasField = value;
            }
        }

        /// <remarks/>
        public bool isdefaultvideoversion
        {
            get
            {
                return this.isdefaultvideoversionField;
            }
            set
            {
                this.isdefaultvideoversionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("credits")]
        public string[] credits
        {
            get
            {
                return this.creditsField;
            }
            set
            {
                this.creditsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("director")]
        public string[] director
        {
            get
            {
                return this.directorField;
            }
            set
            {
                this.directorField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime premiered
        {
            get
            {
                return this.premieredField;
            }
            set
            {
                this.premieredField = value;
            }
        }

        /// <remarks/>
        public ushort year
        {
            get
            {
                return this.yearField;
            }
            set
            {
                this.yearField = value;
            }
        }

        /// <remarks/>
        public object status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public object code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime aired
        {
            get
            {
                return this.airedField;
            }
            set
            {
                this.airedField = value;
            }
        }

        /// <remarks/>
        public string studio
        {
            get
            {
                return this.studioField;
            }
            set
            {
                this.studioField = value;
            }
        }

        /// <remarks/>
        public object trailer
        {
            get
            {
                return this.trailerField;
            }
            set
            {
                this.trailerField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowEpisodedetailsFileinfo fileinfo
        {
            get
            {
                return this.fileinfoField;
            }
            set
            {
                this.fileinfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("actor")]
        public videodbTvshowEpisodedetailsActor[] actor
        {
            get
            {
                return this.actorField;
            }
            set
            {
                this.actorField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowEpisodedetailsResume resume
        {
            get
            {
                return this.resumeField;
            }
            set
            {
                this.resumeField = value;
            }
        }

        /// <remarks/>
        public string dateadded
        {
            get
            {
                return this.dateaddedField;
            }
            set
            {
                this.dateaddedField = value;
            }
        }

        /// <remarks/>
        public videodbTvshowEpisodedetailsArt art
        {
            get
            {
                return this.artField;
            }
            set
            {
                this.artField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsRatings
    {

        private videodbTvshowEpisodedetailsRatingsRating ratingField;

        /// <remarks/>
        public videodbTvshowEpisodedetailsRatingsRating rating
        {
            get
            {
                return this.ratingField;
            }
            set
            {
                this.ratingField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsRatingsRating
    {

        private decimal valueField;

        private byte votesField;

        private string nameField;

        private byte maxField;

        private bool defaultField;

        /// <remarks/>
        public decimal value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }

        /// <remarks/>
        public byte votes
        {
            get
            {
                return this.votesField;
            }
            set
            {
                this.votesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte max
        {
            get
            {
                return this.maxField;
            }
            set
            {
                this.maxField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsThumb
    {

        private string spoofField;

        private string cacheField;

        private string aspectField;

        private string previewField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string spoof
        {
            get
            {
                return this.spoofField;
            }
            set
            {
                this.spoofField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string cache
        {
            get
            {
                return this.cacheField;
            }
            set
            {
                this.cacheField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string aspect
        {
            get
            {
                return this.aspectField;
            }
            set
            {
                this.aspectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string preview
        {
            get
            {
                return this.previewField;
            }
            set
            {
                this.previewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsUniqueid
    {

        private string typeField;

        private bool defaultField;

        private bool defaultFieldSpecified;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool @default
        {
            get
            {
                return this.defaultField;
            }
            set
            {
                this.defaultField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool defaultSpecified
        {
            get
            {
                return this.defaultFieldSpecified;
            }
            set
            {
                this.defaultFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsFileinfo
    {

        private videodbTvshowEpisodedetailsFileinfoStreamdetails streamdetailsField;

        /// <remarks/>
        public videodbTvshowEpisodedetailsFileinfoStreamdetails streamdetails
        {
            get
            {
                return this.streamdetailsField;
            }
            set
            {
                this.streamdetailsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsFileinfoStreamdetails
    {

        private videodbTvshowEpisodedetailsFileinfoStreamdetailsVideo videoField;

        private videodbTvshowEpisodedetailsFileinfoStreamdetailsAudio[] audioField;

        private videodbTvshowEpisodedetailsFileinfoStreamdetailsSubtitle[] subtitleField;

        /// <remarks/>
        public videodbTvshowEpisodedetailsFileinfoStreamdetailsVideo video
        {
            get
            {
                return this.videoField;
            }
            set
            {
                this.videoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("audio")]
        public videodbTvshowEpisodedetailsFileinfoStreamdetailsAudio[] audio
        {
            get
            {
                return this.audioField;
            }
            set
            {
                this.audioField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("subtitle")]
        public videodbTvshowEpisodedetailsFileinfoStreamdetailsSubtitle[] subtitle
        {
            get
            {
                return this.subtitleField;
            }
            set
            {
                this.subtitleField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsFileinfoStreamdetailsVideo
    {

        private string codecField;

        private decimal aspectField;

        private ushort widthField;

        private ushort heightField;

        private ushort durationinsecondsField;

        private object stereomodeField;

        private string hdrtypeField;

        /// <remarks/>
        public string codec
        {
            get
            {
                return this.codecField;
            }
            set
            {
                this.codecField = value;
            }
        }

        /// <remarks/>
        public decimal aspect
        {
            get
            {
                return this.aspectField;
            }
            set
            {
                this.aspectField = value;
            }
        }

        /// <remarks/>
        public ushort width
        {
            get
            {
                return this.widthField;
            }
            set
            {
                this.widthField = value;
            }
        }

        /// <remarks/>
        public ushort height
        {
            get
            {
                return this.heightField;
            }
            set
            {
                this.heightField = value;
            }
        }

        /// <remarks/>
        public ushort durationinseconds
        {
            get
            {
                return this.durationinsecondsField;
            }
            set
            {
                this.durationinsecondsField = value;
            }
        }

        /// <remarks/>
        public object stereomode
        {
            get
            {
                return this.stereomodeField;
            }
            set
            {
                this.stereomodeField = value;
            }
        }

        /// <remarks/>
        public string hdrtype
        {
            get
            {
                return this.hdrtypeField;
            }
            set
            {
                this.hdrtypeField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsFileinfoStreamdetailsAudio
    {

        private string codecField;

        private string languageField;

        private byte channelsField;

        /// <remarks/>
        public string codec
        {
            get
            {
                return this.codecField;
            }
            set
            {
                this.codecField = value;
            }
        }

        /// <remarks/>
        public string language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        public byte channels
        {
            get
            {
                return this.channelsField;
            }
            set
            {
                this.channelsField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsFileinfoStreamdetailsSubtitle
    {

        private string languageField;

        /// <remarks/>
        public string language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsActor
    {

        private string nameField;

        private string roleField;

        private ushort orderField;

        private string thumbField;

        /// <remarks/>
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        public string role
        {
            get
            {
                return this.roleField;
            }
            set
            {
                this.roleField = value;
            }
        }

        /// <remarks/>
        public ushort order
        {
            get
            {
                return this.orderField;
            }
            set
            {
                this.orderField = value;
            }
        }

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsResume
    {

        private decimal positionField;

        private decimal totalField;

        /// <remarks/>
        public decimal position
        {
            get
            {
                return this.positionField;
            }
            set
            {
                this.positionField = value;
            }
        }

        /// <remarks/>
        public decimal total
        {
            get
            {
                return this.totalField;
            }
            set
            {
                this.totalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbTvshowEpisodedetailsArt
    {

        private string thumbField;

        /// <remarks/>
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class videodbPath
    {

        private string urlField;

        private uint scanrecursiveField;

        private bool usefoldernamesField;

        private string contentField;

        private string scraperpathField;

        /// <remarks/>
        public string url
        {
            get
            {
                return this.urlField;
            }
            set
            {
                this.urlField = value;
            }
        }

        /// <remarks/>
        public uint scanrecursive
        {
            get
            {
                return this.scanrecursiveField;
            }
            set
            {
                this.scanrecursiveField = value;
            }
        }

        /// <remarks/>
        public bool usefoldernames
        {
            get
            {
                return this.usefoldernamesField;
            }
            set
            {
                this.usefoldernamesField = value;
            }
        }

        /// <remarks/>
        public string content
        {
            get
            {
                return this.contentField;
            }
            set
            {
                this.contentField = value;
            }
        }

        /// <remarks/>
        public string scraperpath
        {
            get
            {
                return this.scraperpathField;
            }
            set
            {
                this.scraperpathField = value;
            }
        }
    }



}
