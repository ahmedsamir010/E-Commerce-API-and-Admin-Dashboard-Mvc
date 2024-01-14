namespace Store.API.Dtos
{
    public class ProductRatingDto
    {
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string Message { get; set; }
        public string Email { get; set; }
        public DateTime DateTime { get; set; }
    }
}
