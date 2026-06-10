using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn;
using Jotunn.Configs;
using Jotunn.Managers;
using RunicVoice.Abilities;
using RunicVoice.Config;
using RunicVoice.Utility;

namespace RunicVoice;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(Main.ModGuid)]
[BepInProcess("valheim.exe")]
public sealed class RunicVoicePlugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.gerominoes.runicvoice";
    public const string PluginName = "Runic Voice";
    public const string PluginVersion = "0.1.0";
    public const string ShoutButtonName = "RunicVoiceShout";

    internal static ManualLogSource Log = null!;

    private Harmony? _harmony;
    private IRunicVoiceAbility? _activeAbility;

    private void Awake()
    {
        Log = Logger;

        // Config entries are bound before input and abilities so every subsystem reads current values.
        RunicVoiceConfig.Bind(Config);
        DebugLog("Config loaded.");

        // Jotunn input registration keeps the shout key visible to Valheim's input layer.
        RegisterInput();

        // Version 0.1.0 starts with one focused ability that can be expanded later.
        _activeAbility = new BellowOfTheMountain();
        DebugLog($"Ability registered: {_activeAbility.DisplayName}.");

        // Harmony is initialized for future patches, while this first release uses Update only.
        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll();

        Log.LogInfo($"{PluginName} {PluginVersion} loaded.");
    }

    private void Update()
    {
        // Input polling is intentionally the only per-frame gameplay work.
        if (_activeAbility == null)
            return;

        if (ZInput.instance == null)
            return;

        if (!ZInput.GetButtonDown(ShoutButtonName))
            return;

        TryCastActiveAbility();
    }

    private static void RegisterInput()
    {
        // The config-backed button lets players remap the shout key through the BepInEx config file.
        ButtonConfig shoutButton = new()
        {
            Name = ShoutButtonName,
            Config = RunicVoiceConfig.ShoutKey,
            ActiveInGUI = false,
            ActiveInCustomGUI = false,
            BlockOtherInputs = true,
            Hint = "Runic Voice"
        };

        InputManager.Instance.AddButton(PluginGuid, shoutButton);
    }

    private void TryCastActiveAbility()
    {
        // Local player checks protect menus, loading screens, and pre-spawn timing.
        Player? player = Player.m_localPlayer;
        if (player == null)
            return;

        DebugLog($"Shout attempted: {_activeAbility!.DisplayName}.");

        if (!_activeAbility.CanCast(player, out string reason))
        {
            DebugLog($"Shout blocked with reason: {reason}");
            MessageUtils.ShowCenterMessage(reason);
            return;
        }

        _activeAbility.Cast(player);
    }

    internal static void DebugLog(string message)
    {
        // Debug logging stays silent unless explicitly enabled by config.
        if (RunicVoiceConfig.EnableDebugLogs?.Value == true)
            Log.LogInfo($"[Debug] {message}");
    }

    private void OnDestroy()
    {
        // Plugin cleanup removes Harmony patches if later releases add any.
        _harmony?.UnpatchSelf();
    }
}
