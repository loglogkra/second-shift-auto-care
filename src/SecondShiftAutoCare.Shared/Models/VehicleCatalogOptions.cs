namespace SecondShiftAutoCare.Shared.Models;

public static class VehicleCatalogOptions
{
    public const string OtherModel = "Other / Not Listed";
    public static readonly string[] Makes = ["Acura","Audi","BMW","Buick","Cadillac","Chevrolet","Chrysler","Dodge","Ford","GMC","Honda","Hyundai","Jeep","Kia","Lexus","Lincoln","Mazda","Mercedes-Benz","Nissan","Ram","Subaru","Toyota","Volkswagen","Volvo"];

    public static readonly VehicleCatalogModel[] Models =
    [
        M("Acura","ILX",2013,2022), M("Acura","MDX",2001), M("Acura","RDX",2007), M("Acura","TLX",2015),
        M("Audi","A3",2006), M("Audi","A4",1996), M("Audi","Q5",2009), M("Audi","Q7",2007),
        M("BMW","3 Series",1975), M("BMW","5 Series",1972), M("BMW","X3",2004), M("BMW","X5",2000),
        M("Buick","Enclave",2008), M("Buick","Encore",2013), M("Buick","Envision",2016), M("Buick","LaCrosse",2005,2019),
        M("Cadillac","Escalade",1999), M("Cadillac","CTS",2003,2019), M("Cadillac","XT4",2019), M("Cadillac","XT5",2017),
        M("Chevrolet","Colorado",2004), M("Chevrolet","Equinox",2005), M("Chevrolet","Malibu",1997), M("Chevrolet","Silverado",1999), M("Chevrolet","Suburban",1970), M("Chevrolet","Tahoe",1995), M("Chevrolet","Traverse",2009),
        M("Chrysler","200",2011,2017), M("Chrysler","300",2005,2023), M("Chrysler","Pacifica",2017), M("Chrysler","Town & Country",1990,2016),
        M("Dodge","Challenger",2008,2023), M("Dodge","Charger",2006,2023), M("Dodge","Durango",1998), M("Dodge","Grand Caravan",1987,2020),
        M("Ford","Escape",2001), M("Ford","Explorer",1991), M("Ford","F-150",1975), M("Ford","Focus",2000,2018), M("Ford","Fusion",2006,2020), M("Ford","Mustang",1970),
        M("GMC","Acadia",2007), M("GMC","Canyon",2004), M("GMC","Sierra",1999), M("GMC","Terrain",2010), M("GMC","Yukon",1992),
        M("Honda","Accord",1976), M("Honda","Civic",1973), M("Honda","CR-V",1997), M("Honda","Odyssey",1995), M("Honda","Pilot",2003),
        M("Hyundai","Elantra",1992), M("Hyundai","Santa Fe",2001), M("Hyundai","Sonata",1989), M("Hyundai","Tucson",2005),
        M("Jeep","Cherokee",1974), M("Jeep","Compass",2007), M("Jeep","Grand Cherokee",1993), M("Jeep","Wrangler",1987),
        M("Kia","Forte",2010), M("Kia","Optima",2001,2020), M("Kia","Sorento",2003), M("Kia","Soul",2010), M("Kia","Sportage",1995),
        M("Lexus","ES",1990), M("Lexus","IS",2001), M("Lexus","RX",1999), M("Lexus","NX",2015),
        M("Lincoln","MKZ",2007,2020), M("Lincoln","Navigator",1998), M("Lincoln","Aviator",2003), M("Lincoln","Corsair",2020),
        M("Mazda","CX-5",2013), M("Mazda","CX-9",2007), M("Mazda","Mazda3",2004), M("Mazda","Mazda6",2003,2021),
        M("Mercedes-Benz","C-Class",1994), M("Mercedes-Benz","E-Class",1994), M("Mercedes-Benz","GLC",2016), M("Mercedes-Benz","GLE",2016),
        M("Nissan","Altima",1993), M("Nissan","Frontier",1998), M("Nissan","Rogue",2008), M("Nissan","Sentra",1982), M("Nissan","Titan",2004),
        M("Ram","1500",2011), M("Ram","2500",2011), M("Ram","3500",2011), M("Ram","ProMaster",2014),
        M("Subaru","Forester",1998), M("Subaru","Impreza",1993), M("Subaru","Outback",1995), M("Subaru","Crosstrek",2013),
        M("Toyota","Camry",1983), M("Toyota","Corolla",1970), M("Toyota","Highlander",2001), M("Toyota","RAV4",1996), M("Toyota","Tacoma",1995), M("Toyota","Tundra",2000),
        M("Volkswagen","Atlas",2018), M("Volkswagen","Golf",1975), M("Volkswagen","Jetta",1980), M("Volkswagen","Passat",1990,2022), M("Volkswagen","Tiguan",2009),
        M("Volvo","S60",2001), M("Volvo","S80",1999,2016), M("Volvo","XC60",2010), M("Volvo","XC90",2003)
    ];
    private static VehicleCatalogModel M(string make, string name, int startYear, int? endYear = null) => new(make, name, startYear, endYear);
}

public sealed record VehicleCatalogModel(string Make, string Name, int StartYear, int? EndYear)
{
    public bool IsAvailableForYear(int year) => year >= StartYear && (EndYear is null || year <= EndYear.Value);
}
