using CounterStrikeSharp.API.Core;
using StoreApi;

namespace Cs2Casino.Games;

public static class SlotsGame
{
    private static readonly Random _random = new Random();
    private const int MaxWager = 1000;
    
    // Weighted array to make higher tier symbols rarer
    private static readonly string[] Symbols = { 
        "\x02[CHERRY]\x01", "\x02[CHERRY]\x01", "\x02[CHERRY]\x01", "\x02[CHERRY]\x01", "\x02[CHERRY]\x01", 
        "\x09[LEMON]\x01", "\x09[LEMON]\x01", "\x09[LEMON]\x01", "\x09[LEMON]\x01", 
        "\x04[MELON]\x01", "\x04[MELON]\x01", "\x04[MELON]\x01", 
        "\x0A[BELL]\x01", "\x0A[BELL]\x01", 
        "\x0B[DIAMOND]\x01", 
        "\x02[SEVEN]\x01" 
    };

    public static void Play(CCSPlayerController player, IStoreApi storeApi, int wager)
    {
        if (wager > MaxWager)
        {
            player.PrintToChat($" \x04[Casino]\x01 The maximum wager for slots is \x06{MaxWager}\x01 credits.");
            return;
        }

        int currentCredits = storeApi.GetPlayerCredits(player);
        if (currentCredits < wager)
        {
            player.PrintToChat(" \x04[Casino]\x01 You don't have enough credits.");
            return;
        }

        // Deduct wager immediately
        storeApi.SetPlayerCredits(player, currentCredits - wager);

        // Spin reels
        string reel1 = Symbols[_random.Next(Symbols.Length)];
        string reel2 = Symbols[_random.Next(Symbols.Length)];
        string reel3 = Symbols[_random.Next(Symbols.Length)];

        int multiplier = 0;
        string message = "";

        // Determine payout
        if (reel1 == reel2 && reel2 == reel3)
        {
            // 3 of a kind
            multiplier = reel1 switch
            {
                "\x02[SEVEN]\x01" => 50,
                "\x0B[DIAMOND]\x01" => 25,
                "\x0A[BELL]\x01" => 10,
                "\x04[MELON]\x01" => 5,
                "\x09[LEMON]\x01" => 3,
                "\x02[CHERRY]\x01" => 2,
                _ => 0
            };
            message = multiplier == 50 ? "\x0CJACKPOT!\x01" : "\x06Winner!\x01";
        }
        else
        {
            // Check for 2 cherries
            int cherryCount = 0;
            if (reel1 == "\x02[CHERRY]\x01") cherryCount++;
            if (reel2 == "\x02[CHERRY]\x01") cherryCount++;
            if (reel3 == "\x02[CHERRY]\x01") cherryCount++;

            if (cherryCount == 2)
            {
                multiplier = 1; // Break even
                message = "\u0004Consolation Prize!\x01";
            }
            else
            {
                message = "\x02You lost!\x01";
            }
        }

        player.PrintToChat($" \x04[Casino]\x01 [ {reel1} | {reel2} | {reel3} ] - {message}");

        if (multiplier > 0)
        {
            int winnings = wager * multiplier;
            storeApi.GivePlayerCredits(player, winnings);
            if (multiplier > 1)
            {
                player.PrintToChat($" \x04[Casino]\x01 You won \x06{winnings}\x01 credits! ({multiplier}x) You now have \x06{storeApi.GetPlayerCredits(player)}\x01 credits.");
            }
            else
            {
                player.PrintToChat($" \x04[Casino]\x01 You broke even and got your \x06{winnings}\x01 credits back. You now have \x06{storeApi.GetPlayerCredits(player)}\x01 credits.");
            }
        }
        else
        {
            player.PrintToChat($" \x04[Casino]\x01 You lost \x02{wager}\x01 credits. You now have \x06{storeApi.GetPlayerCredits(player)}\x01 credits.");
        }
    }
}
