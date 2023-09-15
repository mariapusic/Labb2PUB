namespace Labb2PUB.Server.Data.Models;

public class CountryModel
{
    public string id { get; set; }
    public string CountryName { get; set; }
    public List<CityModel> Cities { get; set; }
}