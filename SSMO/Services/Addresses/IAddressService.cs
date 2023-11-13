namespace SSMO.Services.Addresses
{
    public interface IAddressService
    {
        public int CreateAddress
            (string street, string city, string country, string bgStreet,string bgCity, string BgCountry,
           string correspondStreet, string correspondCity, string correspondCountry, string correspondBgStreet,
           string correspondBgCity, string correspondBgCountry);

        public void ЕditAddress
            (int id, string street, string city, string country, string bgStreet, string bgCity, string BgCountry,
             string correspondStreet, string correspondCity, string correspondCountry, string correspondBgStreet,
             string correspondBgCity, string correspondBgCountry);
    }
}
