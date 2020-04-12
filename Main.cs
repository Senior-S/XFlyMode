using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Commands;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace XPlugins.FlyMode
{
    public class FlyMode : RocketPlugin
    {
        public List<UnturnedPlayer> playersFlying = new List<UnturnedPlayer>();

        protected override void Load()
        {
            Logger.Log("+--------------------------------------------+", ConsoleColor.Cyan);
            Logger.Log("|                XFlyMode                    |", ConsoleColor.Cyan);
            Logger.Log("|            Loaded Correctly                |", ConsoleColor.Cyan);
            Logger.Log("+--------------------------------------------+", ConsoleColor.Cyan);
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnGesture;
        }

        private void OnGesture(UnturnedPlayer pl, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            PComponent pc = pl.GetComponent<PComponent>();
            if (pc.FlyMode == true)
            {
                if (gesture == UnturnedPlayerEvents.PlayerGesture.Salute)
                {
                    pl.Player.movement.sendPluginGravityMultiplier(1);
                    return;
                }
                if (gesture == UnturnedPlayerEvents.PlayerGesture.Point)
                {
                    double cc = -0.5;
                    float gm = Convert.ToSingle(cc);
                    pl.Player.movement.sendPluginGravityMultiplier(gm);
                    return;
                }
                if (gesture == UnturnedPlayerEvents.PlayerGesture.Wave)
                {
                    pl.Player.movement.sendPluginGravityMultiplier(0);
                    return;
                }
                if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStart)
                {
                    pl.Player.movement.sendPluginSpeedMultiplier(pl.Player.movement.totalSpeedMultiplier + 1);
                    ChatManager.say(pl.CSteamID, "Actual Speed: " + pl.Player.movement.totalSpeedMultiplier, Color.blue, false);
                    return;
                }
                if (gesture == UnturnedPlayerEvents.PlayerGesture.Facepalm)
                {
                    if (pl.Player.movement.speed == 1)
                    {
                        ChatManager.say(pl.CSteamID, Translate("SpeedDownLimit"), Color.red, false);
                        return;
                    }
                    pl.Player.movement.sendPluginSpeedMultiplier(pl.Player.movement.totalSpeedMultiplier - 1);
                    ChatManager.say(pl.CSteamID, "Actual Speed: " + pl.Player.movement.totalSpeedMultiplier, Color.blue, false);
                    return;
                }
            }
            else
            {
                return;
            }
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            { "FlyMode-On", "Now you are in flymode, to use this use POINT to up, SALUTE to down, WAVE to stop, SURRENDER to up the speed and FACEPALM to down the speed!" },
            { "FlyMode-Off", "You desactivate the flymode!" },
            { "SpeedDownLimit", "You actual speed is 1, you cant down the speed!" }
        };

        protected override void Unload()
        {
            Logger.Log("+--------------------------------------------+", ConsoleColor.Red);
            Logger.Log("|                XFlyMode                    |", ConsoleColor.Red);
            Logger.Log("|            Unloaded Correctly              |", ConsoleColor.Red);
            Logger.Log("+--------------------------------------------+", ConsoleColor.Red);
        }

        [RocketCommand("FlyMode", "A command to activate the flymode", "", AllowedCaller.Both)]
        [RocketCommandAlias("fly")]
        [RocketCommandPermission("flymode.use")]
        public void Flymode(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer pl = UnturnedPlayer.FromName(caller.DisplayName);
            PComponent pc = pl.GetComponent<PComponent>();
            if (pc.FlyMode == false)
            {
                pc.FlyMode = true;
                ChatManager.say(pl.CSteamID, Translate("FlyMode-On"), Color.green, false);
                pl.GodMode = true;
            }
            else
            {
                pc.FlyMode = false;
                ChatManager.say(pl.CSteamID, Translate("FlyMode-Off"), Color.red, false);
                pl.GodMode = false;
                pl.Player.movement.sendPluginSpeedMultiplier(1);
                pl.Player.movement.sendPluginGravityMultiplier(1);
            }
        }
    }
}