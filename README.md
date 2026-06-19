# CS2 Casino

A lightweight CounterStrikeSharp plugin that interfaces with `cs2-store` to bring casino games directly to your server's chat!

## Features
- **!casino** - Shows your current credits and the available games.
- **!flip <heads/tails> <wager>** - A 50/50 coin flip. Guess correctly to double your wager!
- **!roll <over/under/7> <wager>** - A classic dice game. Bet Over/Under 7 for a 2x payout, or take a high-risk shot and bet exactly on 7 for a massive 4x payout!
- **!slots <wager>** - Spin the weighted slot machine! Features various multipliers and a 50x Jackpot. (Max wager: 1000 credits).
- **!roulette <red/black/green/number> <wager>** - A classic roulette wheel. Bet on colors or numbers for payouts up to 36x!

## Requirements
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (v1.0.252+ / .NET 10)
- [cs2-store](https://github.com/schwarper/cs2-store)

## Installation
1. Compile the plugin or download the latest `Cs2Casino.dll` file.
2. Place the file into your server's plugin directory:
   `addons/counterstrikesharp/plugins/Cs2Casino/Cs2Casino.dll`
3. Restart the server or run `css_plugins reload Cs2Casino` in the server console.

## Building from Source
Run the following command to compile the plugin and output it to a clean releases folder:
```bash
dotnet publish -c Release -o ./releases/v1.0.0
```
