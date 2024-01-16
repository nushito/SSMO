using System.Collections;
using System.Collections.Generic;

namespace SSMO.Data.Models
{
    public class Image
    {       
            public int Id { get; set; }
            public string ImageTitle { get; set; }
            public byte[] ImageData { get; set; }
            public int MyCompanyId { get; set; }
            public MyCompany MyCompanyName { get; set;}
            public ICollection<Document> HeaderDocuments { get; set; } 
            public ICollection<Document> FooterDocuments { get; set; }
    }
}
