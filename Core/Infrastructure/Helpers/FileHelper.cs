
namespace Core.Infrastructure.Helpers;

public static class FileHelper
{
    public static string GetDateShortId()
    {
        var ticks = new DateTime(2022, 1, 1).Ticks;
        var ans = DateTime.Now.Ticks - ticks;
        var uniqueId = ans.ToString("x");
        return uniqueId;
    }
}
