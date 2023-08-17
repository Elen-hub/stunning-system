using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : TSingleton<PlayerManager>
{
    Dictionary<int, Player> _playerDictionary = new Dictionary<int, Player>();
    Dictionary<string, Player> _cachedPlayerDictionary = new Dictionary<string, Player>();
    public IEnumerable<KeyValuePair<int, Player>> GetPlayerDictionary => _playerDictionary;
    public bool IsCachedPlayer(string guid) => _cachedPlayerDictionary.ContainsKey(guid);
    public Player Me;
    public Player GetPlayer(int playerID) => _playerDictionary.ContainsKey(playerID) ? _playerDictionary[playerID] : null;
    protected override void OnInitialize()
    {
        
    }
    public void ActivateCachePlayer(int player, string guid)
    {
        _playerDictionary.Remove(player);
    }
    public void JoinPlayer(Player player)
    {
        if (_playerDictionary.ContainsKey(player.PlayerID))
            return;

        _playerDictionary.Add(player.PlayerID, player);
        _cachedPlayerDictionary.Add(player.Guid, player);
    }
    public void LeavePlayer(int playerID)
    {
        if (_playerDictionary.ContainsKey(playerID))
        {
            _playerDictionary[playerID].Destroy();
            _playerDictionary.Remove(playerID);
        }
    }
}
