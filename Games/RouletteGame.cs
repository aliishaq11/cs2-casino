using CounterStrikeSharp.API.Core;
using StoreApi;

namespace Cs2Casino.Games;

public static class RouletteGame
{
    private static readonly Random _random = new Random();
    
    // 0 is Green, 1-36 are Red/Black. We'll use standard European roulette colors.
    private static readonly int[] RedNumbers = { 1, 3, 5, 7, 9, 12, 14, 16, 18, 19, 21, 23, 25, 27, 30, 32, 34, 36 };

    public static void Play(CCSPlayerController player, IStoreApi storeApi, string betType, int wager)
    {
        int currentCredits = storeApi.GetPlayerCredits(player);
        if (currentCredits < wager)
        {
            player.PrintToChat(" \x04[Casino]\x01 You don't have enough credits.");
            return;
        }

        bool isRedBet = betType == "red" || betType == "r";
        bool isBlackBet = betType == "black" || betType == "b";
        bool isGreenBet = betType == "green" || betType == "g";
        bool isNumberBet = int.TryParse(betType, out int betNumber);

        if (!isRedBet && !isBlackBet && !isGreenBet && !isNumberBet)
        {
            player.PrintToChat(" \x04[Casino]\x01 Invalid bet. Choose \x02red\x01, \x0Cblack\x01, \x04green\x01, or a number (0-36).");
            return;
        }

        if (isNumberBet && (betNumber < 0 || betNumber > 36))
        {
            player.PrintToChat(" \x04[Casino]\x01 Invalid number. Choose a number between 0 and 36.");
            return;
        }

        // Deduct wager
        storeApi.SetPlayerCredits(player, currentCredits - wager);

        // Spin
        int resultNumber = _random.Next(0, 37);
        bool resultIsRed = RedNumbers.Contains(resultNumber);
        bool resultIsGreen = resultNumber == 0;
        bool resultIsBlack = !resultIsRed && !resultIsGreen;

        string resultColorStr = resultIsGreen ? "\x04Green\x01" : resultIsRed ? "\x02Red\x01" : "\x0CBlack\x01";
        
        player.PrintToChat($" \x04[Casino]\x01 🎡 The wheel spins... and lands on {resultColorStr} {resultNumber}!");

        bool won = false;
        int multiplier = 0;

        if (isRedBet && resultIsRed)
        {
            won = true;
            multiplier = 2;
        }
        else if (isBlackBet && resultIsBlack)
        {
            won = true;
            multiplier = 2;
        }
        else if (isGreenBet && resultIsGreen)
        {
            won = true;
            multiplier = 14; // Tuned payout for green
        }
        else if (isNumberBet && betNumber == resultNumber)
        {
            won = true;
            multiplier = 36;
        }

        if (won)
        {
            int winnings = wager * multiplier;
            storeApi.GivePlayerCredits(player, winnings);
            player.PrintToChat($" \x04[Casino]\x01 You won \x06{winnings}\x01 credits! ({multiplier}x)");
        }
        else
        {
            player.PrintToChat($" \x04[Casino]\x01 You lost \x02{wager}\x01 credits.");
        }
    }
}
