namespace MinimalApi.Extensions;

public static class DoubleExtensions
{
    public static string ToBDTString(this double value)
    {
        return string.Format("{0:###,###,###,###.00}", value) + " ৳"; // Bangladeshi Taka format
    }

    public static string ToDKKString(this double value)
    {
        return string.Format("{0:###.###.###,00} Kr", value); // Danish Krone format
    }

}
