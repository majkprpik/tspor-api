using WebApi.Entities.Common;

namespace WebApi.Entities.Users;

public class UserDetails : AuditableBaseEntity
{    
    public string Description { get; set; }
    public string ImageUrl { get; set; }
}
