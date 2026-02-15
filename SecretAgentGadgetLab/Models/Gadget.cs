using System.ComponentModel.DataAnnotations;

namespace SecretAgentGadgetLab.Models
{
    public class Gadget
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string? Photo { get; set; }
        [Display(Name = "Agent")]
        // Foreign key
        public int AgentId { get; set; }

        // Navigation property (1 agent -> many gadgets)
        public Agent? Agent { get; set; }
    }
}
