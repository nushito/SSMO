using DocumentFormat.OpenXml.ExtendedProperties;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using SSMO.Data;
using SSMO.Data.Models;

namespace SSMO.Services.Addresses
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext dbContext;

        public AddressService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public int CreateAddress
            (string street, string city, string country, string bgStreet, string bgCity, string BgCountry,
             string correspondStreet, string correspondCity, string correspondCountry, string correspondBgStreet,
           string correspondBgCity, string correspondBgCountry)
        {
            var address = new Data.Models.Address
            {
                City = city,
                Country = country,
                Street = street,
                BgCity = bgCity,
                Bgcountry = BgCountry,
                BgStreet = bgStreet,
                CorrespondBgCity= correspondBgCity,
                CorrespondBgCountry = correspondBgCountry,
                CorrespondBgStreet= correspondBgStreet,
                CorrespondCity = correspondCity, 
                CorrespondCountry= correspondCountry,
                CorrespondStreet= correspondStreet
            };

            dbContext.Addresses.Add(address);
            dbContext.SaveChanges();             
           
            return address.Id;
        }

        public void ЕditAddress
            (int id, string street, string city, string country, string bgStreet, string bgCity, string bgCountry,
             string correspondStreet, string correspondCity, string correspondCountry, string correspondBgStreet,
            string correspondBgCity, string correspondBgCountry)
        {
            var address = dbContext.Addresses.Find(id);

            if(address != null)
            {
                address.Street = street;
                address.City = city;
                address.Country = country;
                address.BgCity = bgCity;
                address.BgStreet = bgStreet;
                address.Bgcountry = bgCountry;
                address.CorrespondBgCity = correspondBgCity;
                address.CorrespondBgCountry = correspondBgCountry;
                address.CorrespondBgStreet = correspondBgStreet;
                address.CorrespondCity = correspondCity;
                address.CorrespondCountry = correspondCountry;
                address.CorrespondStreet = correspondStreet;
            }
            dbContext.SaveChanges ();
        }
    }
}
