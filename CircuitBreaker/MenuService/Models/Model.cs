namespace MenuService.Models
{
    public abstract class Model<TPKey>
    {
        public TPKey Id { get; set; }
    }
}
