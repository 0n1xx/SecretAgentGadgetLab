using System.ComponentModel.DataAnnotations;

namespace SecretAgentGadgetLab.Models
{
    public class Agent
    {
        // Each agent has a unique identifier, a code name, a country code, and a salary.
        public int Id { get; set; }
        [Required]
        public string CodeName { get; set; }
        [Required]
        public string CountryCode { get; set; }
        [Required]
        public double Salary { get; set; }
    }
}
