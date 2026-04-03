namespace SecretAgentGadgetLab.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }

        // Foreign key to Gadget
        public int GadgetId { get; set; }

        public int Quantity { get; set; }

        public double Price { get; set; }

        // Navigation properties
        public Order? Order { get; set; }

        public Gadget? Gadget { get; set; }
    }
}