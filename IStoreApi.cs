using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Capabilities;

namespace StoreApi;

public interface IStoreApi
{
    public static PluginCapability<IStoreApi> Capability { get; } = new("store:api");

    int GetPlayerCredits(CCSPlayerController player);
    void SetPlayerCredits(CCSPlayerController player, int credits);
    void GivePlayerCredits(CCSPlayerController player, int credits);
}
