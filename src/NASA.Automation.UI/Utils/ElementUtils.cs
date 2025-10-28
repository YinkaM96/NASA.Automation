using Microsoft.Playwright;

namespace NASA.Automation.UI.Utils;

public static class ElementUtils
{
    /// <summary>
    /// Scrolls to the element and fills it with the given text if present.
    /// </summary>
    public static async Task SafeFillAsync(ILocator locator, string value, string elementName = "")
    {
        if (await locator.CountAsync() == 0)
        {
            Console.WriteLine($"Element '{elementName}' not found, skipping fill.");
            return;
        }

        var target = locator.First;

        await target.ScrollIntoViewIfNeededAsync();
        await target.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Visible,
            Timeout = 10000
        });

        await target.FillAsync(value);
        Console.WriteLine($"Filled '{elementName}' with '{value}'.");
        await Task.Delay(200); // short stabilization delay
    }

    /// <summary>
    /// Scrolls to the element and clicks it safely.
    /// </summary>
    public static async Task SafeClickAsync(ILocator locator, string elementName = "")
    {
        if (await locator.CountAsync() == 0)
        {
            Console.WriteLine($"Element '{elementName}' not found, skipping click.");
            return;
        }

        var target = locator.First;

        await target.ScrollIntoViewIfNeededAsync();
        await target.WaitForAsync(new LocatorWaitForOptions
        {
            State = WaitForSelectorState.Attached,
            Timeout = 10000
        });

        await target.ClickAsync();
        Console.WriteLine($"Clicked '{elementName}'.");
        await Task.Delay(200);
    }
}
