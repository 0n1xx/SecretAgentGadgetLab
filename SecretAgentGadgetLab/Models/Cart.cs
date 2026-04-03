namespace SecretAgentGadgetLab.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public int GadgetId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        public DateTime DateCreated { get; set; }

        public string CustomerId { get; set; }

        public Gadget Gadget { get; set; }
    }
}
