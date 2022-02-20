using System.CommandLine;
using System.Net.NetworkInformation;

var checkIntervalOption = new Option<int>(new string[] { "--interval", "-i" }, () => 10, "Interval between checks (in seconds)");
var stopOnSuccessOption = new Option<bool>(new string[] { "--stop", "-s" }, () => false, "Flag if checks stop on first success");
var checkAddressOption = new Option<string>(new string[] { "--address", "-a" }, () => "8.8.8.8", "Address to check against");

var rootCommand = new RootCommand("Checks internet connection with ping command")
{
    checkIntervalOption,
    stopOnSuccessOption,
    checkAddressOption
};

rootCommand.SetHandler(
    async (int checkInterval, bool stopOnSuccess, string checkAddress) =>
    {
        var p = new Ping();
        while (true)
        {
            var response = await p.SendPingAsync(checkAddress);
            var isSuccess = response.Status == IPStatus.Success;

            Console.Write($"[{DateTime.Now:HH:mm:ss}] ");
            Console.ForegroundColor = isSuccess ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(response.Status);
            Console.ResetColor();

            if (isSuccess && stopOnSuccess)
            {
                return;
            }

            await Task.Delay(checkInterval * 1000);
        }
    },
    checkIntervalOption,
    stopOnSuccessOption,
    checkAddressOption);

return await rootCommand.InvokeAsync(args);