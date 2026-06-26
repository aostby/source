                
                using System.ComponentModel.DataAnnotations;
                using System.ComponentModel.DataAnnotations.Schema;

namespace Kolibri.Desktop.RAPT.Model
{
    [Table("rapt_hydrometer")]
    public class RaptHydrometer: IRaptInterface
    {

        [Key][Column("id")] public string Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("deviceType")] public string DeviceType { get; set; }
        [Column("createdOn")] public string CreatedOn { get; set; }
    }
}
