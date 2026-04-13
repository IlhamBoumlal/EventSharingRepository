using Microsoft.AspNetCore.Identity;

namespace EventSharing.Models
{
    public class User : IdentityUser
    {
        //on a pas ajouter Id ici parceque sera hérité avec d'autre champs depuis User 
        public string? Name { get; set; }
        public List<Event>? CreatedEvents { get; set; }
        public List<Event>? JoinedEvents { get; set; }

    }
}
