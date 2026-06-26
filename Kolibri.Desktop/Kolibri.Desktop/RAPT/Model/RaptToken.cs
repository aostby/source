                
                using System.ComponentModel.DataAnnotations;
                using System.ComponentModel.DataAnnotations.Schema;

namespace Kolibri.Desktop.RAPT.Model
{
    [Table("rapt_token")]
    public class RaptToken
    {

        [Column("access_token")] public string AccessToken { get; set; }
        [Column("expires_in")] public double? ExpiresIn { get; set; }
        [Column("token_type")] public string TokenType { get; set; }
        [Column("scope")] public string Scope { get; set; }
        [Column("iat")] public double Iat { get; set; }
        [Column("createdOn")] public string CreatedOn { get; set; }
    }
}
