                
                using System.ComponentModel.DataAnnotations;
                using System.ComponentModel.DataAnnotations.Schema;

namespace Kolibri.Desktop.RAPT.Model
{
    [Table("rapt_profiles")]
    public class RaptProfiles : IRaptInterface
    {

        [Key][Column("id")] public string Id { get; set; }
        [Key][Column("profileId")] public string ProfileId { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("createdOn")] public string CreatedOn { get; set; }
        [Column("endingOn")] public string EndingOn { get; set; }
        [Column("active")] public int? Active { get; set; }
        [Column("url")] public string Url { get; set; }
    }
}
