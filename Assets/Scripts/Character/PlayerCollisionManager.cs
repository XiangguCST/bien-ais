using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionManager
{
    private PlayerCollisionManager() { }

    static private PlayerCollisionManager _instance = null;

    private HashSet<Character> _ignoreList = new HashSet<Character>();

    public static PlayerCollisionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayerCollisionManager();
            }
            return _instance;
        }
    }

    public void DisableCollision(Character character)
    {
        _ignoreList.Add(character);
        UpdateCollision(character);
    }

    public void EnableCollision(Character character)
    {
        _ignoreList.Remove(character);
        UpdateCollision(character);
    }

    private void UpdateCollision(Character player)
    {
        foreach (Character enemy in player._targetFinder._enemys)
        {
            Collider2D enemyCollider = enemy.GetComponent<Collider2D>();
            Collider2D ownerCollider = player.GetComponent<Collider2D>();
            bool bShouldIgnore = _ignoreList.Contains(enemy) || _ignoreList.Contains(player);
            Physics2D.IgnoreCollision(ownerCollider, enemyCollider, bShouldIgnore);
        }
    }
}
