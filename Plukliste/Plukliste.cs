namespace Plukliste
{
    public class Pluklist
    {
        public string? Name { get; set; }
        public string? Shipment { get; set; }
        public string? Address { get; set; }
        public List<Item> Items { get; private set; } = new List<Item>();

        public void AddItem(Item item)
        {
            Items.Add(item);
        }
    }

    public class Item
    {
        public string ProductID { get; set; }
        public string Title { get; set; }
        public ItemType Type { get; set; }
        public int Amount { get; set; }
    }

    public enum ItemType
    {
        Physical,
        Print
    }
}
