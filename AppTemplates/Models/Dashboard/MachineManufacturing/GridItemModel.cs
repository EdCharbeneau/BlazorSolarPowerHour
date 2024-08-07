namespace MachineManufacturingTemplate.Models.Dashboard.MachineManufacturing
{
    public class GridItemModel
    {
        public string ItemNumber { get; set; }
        public string Availability { get; set; }
        public int Available { get; set; }
        public int Booked { get; set; }
        public int MinQuantity { get; set; }
        public decimal Cost { get; set; }
    }
}
