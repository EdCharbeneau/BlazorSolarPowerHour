using System.Globalization;
using Telerik.Blazor.Components;
using Telerik.SvgIcons;
using MachineManufacturingTemplate.Models.Dashboard.MachineManufacturing;

namespace MachineManufacturingTemplate.Components.Pages.Dashboard
{
    public partial class MachineManufacturing
    {
        private DrawerItemModel _selectedDrawerItem;

        #region Charts Data

        public static List<object> MachineOneSerriesData { get; set; } = [7890, 6510, 5413, 7910, 2638, 6780, 5190, 67540, 5274, 4200, 3532, 2521];
        public static List<object> MachineTwoSeriesData { get; set; } = [3920, 3810, 5700, 4490, 4980, 5395, 4210, 5700, 5001, 3460, 4100, 2510];
        public static List<object> MachineFourSeriesData { get; set; } = [4600, 5200, 1610, 3110, 2010, 3689, 3244, 4190, 800, 3155, 1900, 3150];
        public static List<object> MachineEigthSeriesData { get; set; } = [1800, 1650, 2550, 1650, 3420, 3000, 3200, 2510, 1810, 2399, 2280, 2560];
        public static string[] ProductionVolumeXAxisItems { get; set; } = DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames.Take(12).ToArray();
        public static List<ChartSeriesModel> QalityIssuesData { get; set; } =
        [
            new() { Name = "Poor Organization", Value = 16.66 },
            new() { Name = "Logistics Challenges", Value = 16.66 },
            new() { Name = "Others", Value = 16.66 },
            new() { Name = "Planned Downtime", Value = 16.66 },
            new() { Name = "Physical Damages", Value = 16.66 },
            new() { Name = "Equipment Failure", Value = 16.66 },
        ];

        public static string[] DefectRateXAxisItems { get; set; } = Enumerable.Range(0, 48).Select(x => $"{(x / 2)}:{(x % 2 == 0 ? "00" : "30")}").ToArray();
        public static List<object> SevereDefectRateData { get; set; } = [ 50, 53, 57, 38, 39, 32, 31, 31, 29, 28, 25, 22, 19, 18, 17, 16, 13, 10, 8, 9, 10, 12,
    14, 15, 17, 13, 10, 8, 6, 5, 5, 6, 8, 13, 14, 17, 30, 31, 33, 38, 42, 48, 50, 51, 52, 53, 48, 47 ];
        public static List<object> MajorDefectRateData { get; set; } = [ 61, 62, 63, 48, 42, 39, 37, 39, 36, 38, 35, 31, 29, 28, 23, 27, 23, 19, 18, 19, 20, 21,
    24, 22, 24, 26, 20, 17, 15, 15, 14, 13, 23, 24, 27, 29, 40, 41, 47, 51, 58, 61, 61, 63, 64, 59, 59, 60];
        public static List<object> AverageDefectRateData { get; set; } = [ 9, 13, 14, 15, 16, 18, 25, 29, 31, 29, 32, 30, 28, 31, 33, 35, 40, 41, 42, 43, 59, 62,
    64, 65, 66, 63, 62, 61, 60, 58, 62, 63, 64, 62, 63, 62, 61, 58, 55, 52, 51, 50, 48, 45, 42, 41, 40, 30];
        public static List<object> MinorDefectRateData { get; set; } = [ 39, 36, 53, 51, 59, 62, 63, 64, 65, 67, 80, 81, 83, 82, 83, 84, 86, 88, 87, 86, 87, 72, 74, 79, 73,
       62, 64, 61, 58, 53, 51, 50, 48, 49, 43, 41, 40, 38, 39, 41, 42, 40, 38, 36, 40, 41, 38, 41];

        public static List<ChartSeriesModel> SatisfactionScoreData { get; set; } =
        [
            new() { Name = "Very dissatisfied", Value = 60 },
            new() { Name = "Dissatisfied", Value = 60},
            new() { Name = "Neutral", Value = 60 },
            new() { Name = "Satisfied", Value = 60 },
            new() { Name = "Very satisfied", Value = 60 },
            new() { Name = "Didn't answer", Value = 60 },
        ];

        #endregion Charts Data

        #region Date Pickers Values

        private DateTime AlertsDatePickerValue { get; set; } = new DateTime(2021, 1, 21);
        private DateTime QualityIssuesDatePickerValue { get; set; } = new DateTime(2023, 6, 14);
        private DateTime DefectRateDatePickerValue { get; set; } = new DateTime(2023, 1, 1);
        private DateTime InventoryDatePickerValue { get; set; } = new DateTime(2023, 6, 14);

        #endregion Date Pickers Values

        #region Static Assets

        private string LogoSrc = "/dashboard/machine-manufacturing/logo.svg";
        private string LogoTextSrc = "/dashboard/machine-manufacturing/logo_text.svg";
        private string AvatarImage1Src = "/dashboard/machine-manufacturing/avatar_1.png";
        private string AvatarImage2Src = "/dashboard/machine-manufacturing/avatar_2.png";
        private string AvatarImage3Src = "/dashboard/machine-manufacturing/avatar_3.png";
        private string AvatarImage4Src = "/dashboard/machine-manufacturing/avatar_4.png";
        private string AvatarImage5Src = "/dashboard/machine-manufacturing/avatar_5.png";
        private string AvatarImage6Src = "/dashboard/machine-manufacturing/avatar_6.png";

        #endregion Static Assets

        #region MutliSelect Data

        private List<string> MultiSelectData { get; set; } = new List<string>()
        {
            "Machine 1", "Machine 2", "Machine 3", "Machine 4", "Machine 5", "Machine 6", "Machine 7", "Machine 8"
        };

        private List<string> MultiSelectSelectedItems { get; set; } = new List<string>()
        {
            "Machine 1", "Machine 2"
        };

        #endregion MutliSelect Data

        #region Grid Data

        public List<GridItemModel> GridData { get; set; } = new List<GridItemModel>()
        {
            new GridItemModel() { ItemNumber = "81154", Availability = "In stock", Available = 49281, Booked = 43966, MinQuantity = 4322, Cost = 479.00M },
            new GridItemModel() { ItemNumber = "17076", Availability = "Low in stock", Available = 52573, Booked = 50226, MinQuantity = 5232, Cost = 672.00M },
            new GridItemModel() { ItemNumber = "43486", Availability = "Out of stock", Available = 13576, Booked = 23611, MinQuantity = 3232, Cost = 312.00M },
            new GridItemModel() { ItemNumber = "39656", Availability = "In stock", Available = 75561, Booked = 56619, MinQuantity = 2324, Cost = 61.00M },
            new GridItemModel() { ItemNumber = "14953", Availability = "Out of stock", Available = 34652, Booked = 36262, MinQuantity = 3232, Cost = 254.00M },
            new GridItemModel() { ItemNumber = "39656", Availability = "In stock", Available = 75561, Booked = 56619, MinQuantity = 2324, Cost = 61.00M },
            new GridItemModel() { ItemNumber = "14953", Availability = "Out of stock", Available = 34652, Booked = 36262, MinQuantity = 3232, Cost = 254.00M },
        };

        #endregion Grid Data

        #region Drawer Data

        private bool DrawerExpanded { get; set; } = true;

        private TelerikDrawer<DrawerItemModel> DrawerRef { get; set; }

        private DrawerItemModel SelectedDrawerItem
        {
            get => _selectedDrawerItem ?? DrawerItems.FirstOrDefault();
            set => _selectedDrawerItem = value;
        }

        private IEnumerable<DrawerItemModel> DrawerItems { get; set; } = new List<DrawerItemModel>()
        {
            new DrawerItemModel() { Text = "Dashboard", Icon = SvgIcon.Inbox },
            new DrawerItemModel() { Text = "Statistics", Icon = SvgIcon.ChartColumnStacked },
            new DrawerItemModel() { Text = "Reports", Icon = SvgIcon.FileTxt },
            new DrawerItemModel() { Separator = true },
            new DrawerItemModel() { Text = "Settings", Icon = SvgIcon.Gear },
            new DrawerItemModel() { Text = "Support", Icon = SvgIcon.QuestionCircle }
        };

        private async Task ToggleDrawer() => await DrawerRef.ToggleAsync();

        #endregion Drawer Data

        #region Lifecycle Methods

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        #endregion Lifecycle Methods
    }
}
