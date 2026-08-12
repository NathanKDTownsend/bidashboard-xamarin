using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Syncfusion.SfChart.XForms;
using Xamarin.Forms;
using Newtonsoft.Json;

namespace BIDashboard
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Sale> SalesCollection { get; set; } = new ObservableCollection<Sale>();

        private string JsonPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sales.json");

        public MainPage()
        {
            InitializeComponent();
            _ = LoadDataAsync();
        }

        private async System.Threading.Tasks.Task LoadDataAsync()
        {
            if (File.Exists(JsonPath))
            {
                var jsonData = File.ReadAllText(JsonPath);
                var list = JsonConvert.DeserializeObject<ObservableCollection<Sale>>(jsonData);

                SalesCollection = list;
            }
            else
            {
                var sales = await FetchSalesAsync();

                foreach (var s in sales)
                    SalesCollection.Add(s);

                // Saveing the JSON for future use
                SaveJsonCache();
            }

            PopulateFilters();
            UpdateUI();
        }

        private async System.Threading.Tasks.Task<ObservableCollection<Sale>> FetchSalesAsync()
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                string resourceName = "BIDashboard.SGetSales-CW2.xlsx";

                using (Stream stream = GetType().Assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new Exception($"Embedded Excel file not found: {resourceName}");

                    var list = ExcelReader.ReadSalesStream(stream);
                    return new ObservableCollection<Sale>(list);
                }
            });
        }

        private void SaveJsonCache()
        {
            string json = JsonConvert.SerializeObject(SalesCollection, Formatting.Indented);
            File.WriteAllText(JsonPath, json);
        }

        private void ReloadJson()
        {
            if (!File.Exists(JsonPath))
                return;

            var json = File.ReadAllText(JsonPath);
            var list = JsonConvert.DeserializeObject<ObservableCollection<Sale>>(json);

            SalesCollection = list;
        }

        private void PopulateFilters()
        {
            YearPicker.ItemsSource = SalesCollection.Select(s => s.Year.ToString()).Distinct().OrderBy(y => y).ToList();
            RegionPicker.ItemsSource = SalesCollection.Select(s => s.Region).Distinct().OrderBy(r => r).ToList();
            NamePicker.ItemsSource = SalesCollection.Select(s => s.Vehicle).Distinct().OrderBy(n => n).ToList();
            QTRPicker.ItemsSource = new string[] { "Q1", "Q2", "Q3", "Q4" };

            var maxQuantity = SalesCollection.Max(s => s.Quantity);
            var ranges = new ObservableCollection<string>();
            for (int i = 0; i <= maxQuantity; i += 20)
            {
                int end = Math.Min(i + 19, maxQuantity);
                ranges.Add($"{i + 1}-{end + 1}");
            }
            QuantityPicker.ItemsSource = ranges;
        }

        private void UpdateUI()
        {
            var filtered = SalesCollection
                .Where(s =>
                    (YearPicker.SelectedItem == null || s.Year.ToString() == YearPicker.SelectedItem.ToString()) &&
                    (RegionPicker.SelectedItem == null || s.Region == RegionPicker.SelectedItem.ToString()) &&
                    (NamePicker.SelectedItem == null || s.Vehicle == NamePicker.SelectedItem.ToString()) &&
                    (QTRPicker.SelectedItem == null || $"Q{s.QTR}" == QTRPicker.SelectedItem.ToString()) &&
                    (QuantityPicker.SelectedItem == null || FilterByQuantity(s.Quantity, QuantityPicker.SelectedItem.ToString()))
                )
                .ToList();

            DataListView.ItemsSource = filtered;

            // ---- BAR CHART ----
            BarChart.Series.Clear();
            var barSeries = new ColumnSeries
            {
                ItemsSource = filtered.Select(s => new { Label = $"Q{s.QTR}-{s.Year}", Quantity = s.Quantity }).ToList(),
                XBindingPath = "Label",
                YBindingPath = "Quantity"
            };
            BarChart.Series.Add(barSeries);

            // ---- PIE CHART ----
            PieChart.Series.Clear();
            var pieSeries = new PieSeries
            {
                ItemsSource = filtered.GroupBy(s => s.Vehicle)
                                      .Select(g => new { Vehicle = g.Key, Quantity = g.Sum(s => s.Quantity) })
                                      .ToList(),
                XBindingPath = "Vehicle",
                YBindingPath = "Quantity"
            };
            PieChart.Series.Add(pieSeries);
        }

        private bool FilterByQuantity(int quantity, string range)
        {
            var parts = range.Split('-');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int min) &&
                int.TryParse(parts[1], out int max))
            {
                return quantity >= min && quantity <= max;
            }
            return true;
        }

        private void OnFilterChanged(object sender, EventArgs e)
        {
            UpdateUI();
        }

        private void OnResetClicked(object sender, EventArgs e)
        {
            YearPicker.SelectedIndex = -1;
            RegionPicker.SelectedIndex = -1;
            NamePicker.SelectedIndex = -1;
            QTRPicker.SelectedIndex = -1;
            QuantityPicker.SelectedIndex = -1;
            UpdateUI();
        }

        private void OnReloadJsonClicked(object sender, EventArgs e)
        {
            ReloadJson();
            PopulateFilters();
            UpdateUI();
        }
    }
}
