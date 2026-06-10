using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using StoreApi;

namespace Cs2Casino;

public class Cs2CasinoPlugin : BasePlugin
{
    public override string ModuleName => "CS2 Casino";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Antigravity";
    public override string ModuleDescription => "Casino games via chat using cs2-store credits.";

    private IStoreApi? _storeApi;
    private readonly Random _random = new Random();

    public override void OnAllPluginsLoaded(bool hotReload)
    {
        _storeApi = IStoreApi.Capability.Get();
        if (_storeApi == null)
        {
            Console.WriteLine("[CS2 Casino] ERROR: cs2-store API not found. Please ensure cs2-store is loaded.");
        }
    }

    [ConsoleCommand("css_casino", "Show casino menu and credits")]
    public void OnCasinoCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || _storeApi == null) return;

        int currentCredits = _storeApi.GetPlayerCredits(player);
        player.PrintToChat($" \x04[Casino]\x01 Welcome to the Casino! You have \x06{currentCredits}\x01 credits.");
        player.PrintToChat($" \x04[Casino]\x01 Games available:");
        player.PrintToChat($" \x04-\x01 \x06!flip\x01 : 50/50 Coin flip. Win 2x your bet.");
        player.PrintToChat($" \x04-\x01 \x06!roll\x01 : Dice game. Bet over/under 7 (2x payout) or exactly 7 (4x payout).");
    }

    [ConsoleCommand("css_flip", "Flip a coin to win credits (50/50)")]
    public void OnFlipCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || _storeApi == null) return;

        if (info.ArgCount < 3)
        {
            player.PrintToChat(" \x04[Casino]\x01 \x02Incorrect Syntax!\x01 Usage: \x06!flip <heads/tails> <wager>\x01");
            return;
        }

        string choice = info.GetArg(1).ToLower();
        if (choice != "heads" && choice != "tails" && choice != "h" && choice != "t")
        {
            player.PrintToChat(" \x04[Casino]\x01 You must pick 'heads' or 'tails'.");
            return;
        }

        if (!int.TryParse(info.GetArg(2), out int wager) || wager <= 0)
        {
            player.PrintToChat(" \x04[Casino]\x01 Invalid wager amount.");
            return;
        }

        int currentCredits = _storeApi.GetPlayerCredits(player);
        if (currentCredits < wager)
        {
            player.PrintToChat(" \x04[Casino]\x01 You don't have enough credits.");
            return;
        }

        bool pickedHeads = choice == "heads" || choice == "h";

        // Deduct wager immediately
        _storeApi.SetPlayerCredits(player, currentCredits - wager);

        int result = _random.Next(0, 2);
        bool resultIsHeads = result == 0;
        string resultStr = resultIsHeads ? "Heads" : "Tails";

        if (pickedHeads == resultIsHeads)
        {
            int winnings = wager * 2;
            _storeApi.GivePlayerCredits(player, winnings);
            player.PrintToChat($" \x04[Casino]\x01 The coin landed on \x06{resultStr}\x01! You won \x06{winnings}\x01 credits!");
        }
        else
        {
            player.PrintToChat($" \x04[Casino]\x01 The coin landed on \x02{resultStr}\x01. You lost \x02{wager}\x01 credits.");
        }
    }

    [ConsoleCommand("css_dice", "Play Over/Under 7 with two dice")]
    [ConsoleCommand("css_roll", "Play Over/Under 7 with two dice")]
    public void OnDiceCommand(CCSPlayerController? player, CommandInfo info)
    {
        if (player == null || _storeApi == null) return;

        if (info.ArgCount < 3)
        {
            player.PrintToChat(" \x04[Casino]\x01 \x02Incorrect Syntax!\x01 Usage: \x06!roll <over/under/7> <wager>\x01");
            return;
        }

        string choice = info.GetArg(1).ToLower();
        if (choice != "over" && choice != "under" && choice != "7")
        {
            player.PrintToChat(" \x04[Casino]\x01 You must pick 'over', 'under', or '7'.");
            return;
        }

        if (!int.TryParse(info.GetArg(2), out int wager) || wager <= 0)
        {
            player.PrintToChat(" \x04[Casino]\x01 Invalid wager amount.");
            return;
        }

        int currentCredits = _storeApi.GetPlayerCredits(player);
        if (currentCredits < wager)
        {
            player.PrintToChat(" \x04[Casino]\x01 You don't have enough credits.");
            return;
        }

        // Deduct wager immediately
        _storeApi.SetPlayerCredits(player, currentCredits - wager);

        // Roll two 6-sided dice
        int die1 = _random.Next(1, 7);
        int die2 = _random.Next(1, 7);
        int total = die1 + die2;

        player.PrintToChat($" \x04[Casino]\x01 You rolled \x06{die1}\x01 and \x06{die2}\x01 (Total: \x0C{total}\x01).");

        bool won = false;
        int multiplier = 0;

        if (choice == "over" && total > 7)
        {
            won = true;
            multiplier = 2;
        }
        else if (choice == "under" && total < 7)
        {
            won = true;
            multiplier = 2;
        }
        else if (choice == "7" && total == 7)
        {
            won = true;
            multiplier = 4; // 4x payout for hitting exactly 7
        }

        if (won)
        {
            int winnings = wager * multiplier;
            _storeApi.GivePlayerCredits(player, winnings);
            player.PrintToChat($" \x04[Casino]\x01 You guessed correctly! You won \x06{winnings}\x01 credits!");
        }
        else
        {
            player.PrintToChat($" \x04[Casino]\x01 You guessed wrong. You lost \x02{wager}\x01 credits.");
        }
    }
}
