using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ScavengerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Scavenger)) return;
            var role = Role.GetRole<Scavenger>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();
                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            KillButtonTarget.SetTarget(__instance.KillButton, closestBody, role);
            __instance.KillButton.SetCoolDown(role.DevourTimer(), CustomGameOptions.DevourCd);
        }
    }
}