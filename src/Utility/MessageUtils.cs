using UnityEngine;

namespace RunicVoice.Utility;

internal static class MessageUtils
{
    internal static void ShowCenterMessage(string message)
    {
        // Empty reasons are intentionally silent for states such as missing players during loading.
        if (string.IsNullOrWhiteSpace(message))
            return;

        // MessageHud is preferred for center-screen feedback when the HUD is ready.
        if (MessageHud.instance != null)
        {
            MessageHud.instance.ShowMessage(MessageHud.MessageType.Center, message, 0, null, false);
            return;
        }

        // The local player fallback covers early HUD timing without throwing.
        if (Player.m_localPlayer != null)
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, message, 0, null);
    }
}
