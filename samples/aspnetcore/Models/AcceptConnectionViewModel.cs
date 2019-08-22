using System.ComponentModel.DataAnnotations;

namespace WebAgent.Models
{
    public class AcceptConnectionViewModel
    {
        [Required]
        public string InvitationDetails { get; set; }
    }
}