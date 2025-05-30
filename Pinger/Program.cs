using System.CommandLine;
using System.Net.NetworkInformation;

var intervalOption = new Option<int>(new[] { "--interval", "-i" }, () => 10, "Interval between pings (in seconds)");
var addressOption = new Option<string>(new[] { "--address", "-a" }, () => "8.8.8.8", "Address to ping against");
var stopOnSuccessOption = new Option<bool>(new[] { "--stop", "-s" }, () => false, "Stop on success");

var rootCommand = new RootCommand("Checks internet connection with ping command")
{
    intervalOption,
    addressOption,
    stopOnSuccessOption
};

rootCommand.SetHandler(
    async (int interval, string address, bool stopOnSuccess) =>
    {
        Ping p = new();

        while (true)
        {
            var isSuccess = false;
            var message = "";

            try
            {
                var response = await p.SendPingAsync(address);
                isSuccess = response.Status == IPStatus.Success;
                message = response.Status.ToString();
            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message;
            }

            Console.Write($"[{DateTime.Now:T}] ");
            Console.ForegroundColor = isSuccess ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();

            if (isSuccess && stopOnSuccess)
            {
                return;
            }

            await Task.Delay(interval * 1000);
        }
    },
    intervalOption,
    addressOption,
    stopOnSuccessOption);

return await rootCommand.InvokeAsync(args);