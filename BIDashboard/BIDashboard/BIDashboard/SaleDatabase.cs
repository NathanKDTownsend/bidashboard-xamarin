using System.Collections.Generic;
using SQLite;

namespace BIDashboard
{
    public class SaleDatabase
    {
        private SQLiteConnection _db;

        public SaleDatabase(string dbPath)
        {
            _db = new SQLiteConnection(dbPath);
            _db.CreateTable<Sale>();
        }

        public void SaveSales(IEnumerable<Sale> sales)
        {
            _db.DeleteAll<Sale>();
            _db.InsertAll(sales);
        }

        public List<Sale> LoadSales()
        {
            return _db.Table<Sale>().ToList();
        }
    }
}
