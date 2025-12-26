using UnityEngine;

public static class InteractionBlocker
{
    private static bool isBlocked = false;

    public static bool IsBlocked()
    {
        return isBlocked;
    }

    public static void BlockInteractions(bool block)
    {
        isBlocked = block;
        Debug.Log($"Интеракции {(block ? "заблокированы" : "разблокированы")}");
    }
}

