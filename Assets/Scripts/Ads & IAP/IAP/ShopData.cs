using System.Collections.Generic;
using Newtonsoft.Json;

public class ShopDataItem
{
    [JsonProperty("Item")]
    public string Item;

    [JsonProperty("Rocket Power Up")]
    public int RocketPowerUp;

    [JsonProperty("Fan Power Up")]
    public int FanPowerUp;

    [JsonProperty("No ads in months")]
    public int NoAdsInMonths;

    [JsonProperty("Dollar Price ")]
    public double DollarPrice;

    [JsonProperty("Gems")]
    public int Gems;

    [JsonProperty("Bundle ID")]
    public string BundleID;

    [JsonProperty("Description ")]
    public string Description;
}

public class ShopData
{
    [JsonProperty("data")]
    public List<ShopDataItem> ShopDataItem;
}