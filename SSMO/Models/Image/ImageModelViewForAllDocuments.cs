namespace SSMO.Models.Image
{
    //model za vizualizaciq na snimka. shte se izpolzva ot vsichki vidove dokumenti
    public class ImageModelViewForAllDocuments
    {
        public int Id { get; set; }
        public string ImageTitle { get; set; }
        public byte[] ImageData { get; set; }
    }
}
