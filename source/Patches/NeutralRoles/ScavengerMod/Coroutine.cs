using Reactor.Utilities;
using System.Collections;
using System.Linq;
using TownOfUs.Patches.NeutralRoles;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ScavengerMod
{
    public class Coroutine
    {
        private static readonly int BodyColor = Shader.PropertyToID("_BodyColor");
        private static readonly int BackColor = Shader.PropertyToID("_BackColor");

        public static IEnumerator DevourCoroutine(DeadBody body, Scavenger role)
        {
            SpriteRenderer renderer = null;
            foreach (var body2 in body.bodyRenderers) renderer = body2;
            var backColor = renderer.material.GetColor(BackColor);
            var bodyColor = renderer.material.GetColor(BodyColor);
            var newColor = new Color(1f, 1f, 1f, 0f);
            for (var i = 0; i < 60; i++)
            {
                if (body == null) yield break;
                renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                yield return null;
            }

            Object.Destroy(body.gameObject);
            role.CorpsesEaten += 1;
        }
    }
}