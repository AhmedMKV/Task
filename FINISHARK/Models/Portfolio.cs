using System.ComponentModel.DataAnnotations.Schema;

namespace FINISHARK.Models
{
    [Table("Portfolios")]
    public class Portfolio
    {


        public string AppUserId { get; set; }
        public int StockID { get; set; }
        public AppUser User { get; set; }

        public Stock stock { get; set; }

    }
}
