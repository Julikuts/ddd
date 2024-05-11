namespace DeliveryApp.Core.Application.UseCases.Queries.GetActiveOrders
{
    public class Response
    {
        public List<Order> Orders { get; set; } = new List<Order>();

        public Response(List<Order> orders)
        {
            Orders.AddRange(orders);
        }
    }

    public class Order
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Геопозиция (X,Y)
        /// </summary>
        public Location Location { get; set; }
    }

    public class Location
    {
        /// <summary>
        /// Горизонталь
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Вертикаль
        /// </summary>
        public int Y { get; set; }
    }
}