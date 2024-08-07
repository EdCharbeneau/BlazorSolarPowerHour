using Telerik.SvgIcons;

namespace MachineManufacturingTemplate.Models.Dashboard.MachineManufacturing
{
    public class DrawerItemModel
    {
        public string Text { get; set; }
        public ISvgIcon Icon { get; set; }
        public string Url { get; set; }
        public bool Separator { get; set; }
    }
}
