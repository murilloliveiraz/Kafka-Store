namespace BuildingBlocks.Models
{
    public class Order
    {
        public string Email { get; set; }
        public string OrderId { get; set; }
        public decimal Amount { get; set; }

        public Order(string email, string orderId, decimal amount)
        {
            Email = email;
            OrderId = orderId;
            Amount = amount;
        }

        public Order() { }

        public override string ToString()
        {
            return $"Order {{ Email='{Email}', OrderId='{OrderId}', Amount={Amount} }}";
        }
    }
}
