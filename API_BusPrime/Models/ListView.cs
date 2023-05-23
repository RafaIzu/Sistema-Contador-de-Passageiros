namespace BusPrime.Models
{
    public class ListView<T>
    {
        public T list { get; set; }
        public int perPage { get; set; }
        public int currentPage { get; set; }
        public int totalItems { get; set; }
        public int totalPages { get; set; }
        public int take { get; set; }
        public string? searchFilter { get; set; }
        public List<int> listOptions = new List<int>() { 15, 50, 100, 150 };
    }
}
