                
                using System.ComponentModel.DataAnnotations;
                using System.ComponentModel.DataAnnotations.Schema;

namespace Kolibri.Desktop.RAPT.Model
{
    [Table("rapt_posts")]
    public class RaptPosts : IRaptInterface
    {

        [Key][Column("id")] public string Id { get; set; }
        [Column("name")] public string Name { get; set; }
        [Column("type")] public string Type { get; set; }
        [Key][Column("createdOn")] public string CreatedOn { get; set; }
        [Column("temperature")] public double? Temperature { get; set; }
        [Column("temperature_unit")] public string TemperatureUnit { get; set; }
        [Column("gravity")] public double? Gravity { get; set; }
        [Column("gravity_unit")] public string GravityUnit { get; set; }
        [Column("battery")] public double? Battery { get; set; }
        [Column("rssi")] public double? Rssi { get; set; }
        [Column("profileId")] public string ProfileId { get; set; }
    }
}
