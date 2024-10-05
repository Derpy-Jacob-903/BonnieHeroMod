using MelonLoader;
using BTD_Mod_Helper;
using BonnieHeroMod;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Models.Towers;
using BTD_Mod_Helper.Extensions;
using HarmonyLib;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity.UI_New.InGame.TowerSelectionMenu;
using Il2CppAssets.Scripts.Models.Profile;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;

[assembly: MelonInfo(typeof(BonnieHeroMod.BonnieHeroMod), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace BonnieHeroMod;

[HarmonyPatch]
public class BonnieHeroMod : BloonsTD6Mod
{
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        base.OnTowerUpgraded(tower, upgradeName, newBaseTowerModel);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            var towerLogic = tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
            switch (tower.towerModel.tier)
            {
                case 2:
                    BonnieLogic.BonnieUI.Init(TowerSelectionMenu.instance);
                    tower.AddMutator(new RangeSupport.MutatorTower(false, "MinecartTier", 0, 0, null));
                    tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().glueLevel = 10;
                    break;
                case 5:
                    towerLogic.glueLevel = 15;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 8:
                    towerLogic.glueLevel = 20;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 11:
                    towerLogic.glueLevel = 25;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 14:
                    towerLogic.glueLevel = 30;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 17:
                    towerLogic.glueLevel = 35;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
                case 20:
                    towerLogic.glueLevel = 40;
                    BonnieLogic.BonnieUI.UpdateUI();
                    break;
            }
        }
    }

    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        base.OnTowerCreated(tower, target, modelToUse);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                BonnieLogic.BonnieUI.Init(TowerSelectionMenu.instance);
                tower.AddMutator(new RangeSupport.MutatorTower(false, "MinecartTier", 0, 0, null));
                tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>().glueLevel = 10;
            }
        }
    }

    public override void OnTowerSold(Tower tower, float amount)
    {
        base.OnTowerSold(tower, amount);

        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.towerModel.tier > 0)
            {
                var towerLogic = tower.GetMutator("MinecartTier").Cast<RangeSupport.MutatorTower>();
                BonnieLogic.CartSellLogic();
                towerLogic.glueLevel = 10;
            }
        }
    }


    public override void OnTowerSelected(Tower tower)
    {
        base.OnTowerSelected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieLogic.BonnieUI.bonniePanel != null)
            {
                BonnieLogic.BonnieUI.BonnieUIToggle(true);
            }
            else
            {
                BonnieLogic.BonnieUI.Init(TowerSelectionMenu.instance);
            }
        }
    }

    public override void OnTowerDeselected(Tower tower)
    {
        base.OnTowerDeselected(tower);
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (BonnieLogic.BonnieUI.bonniePanel != null)
            {
                BonnieLogic.BonnieUI.BonnieUIToggle(false);
            }
        }
    }
    public override void OnTowerSaved(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            var MinecartTier = tower.GetMutator("MinecartTier")?.TryCast<RangeSupport.MutatorTower>();
            if (MinecartTier != null)
            {
                saveData.metaData["MinecartTier"] = MinecartTier.multiplier.ToString();
                saveData.metaData["MinecartTierBank"] = MinecartTier.additive.ToString();
                saveData.metaData["MinecartMaxTier"] = MinecartTier.glueLevel.ToString();
            }
        }
    }

    public override void OnTowerLoaded(Tower tower, TowerSaveDataModel saveData)
    {
        if (tower.towerModel.baseId == ModContent.TowerID<BonnieHero>())
        {
            if (tower.mutators != null)
            {
                tower.RemoveMutatorsById("MinecartTier");

                saveData.metaData.TryGetValue("MinecartTier", out var minecartTier);
                saveData.metaData.TryGetValue("MinecartTierBank", out var minecartTierBank);
                saveData.metaData.TryGetValue("MinecartMaxTier", out var minecartMaxTier);

                var minecartMutator = new RangeSupport.MutatorTower(false, "MinecartTier", float.Parse(minecartTierBank), float.Parse(minecartTier), null);
                minecartMutator.glueLevel = int.Parse(minecartMaxTier);
                tower.AddMutator(minecartMutator);
            }
        }
    }

    public override void OnAbilityCast(Ability ability)
    {
        base.OnAbilityCast(ability);
        if (ability.abilityModel.name == "AbilityModel_MassDetonation")
        {

        }
    }

    [HarmonyPatch(typeof(Ability), nameof(Ability.Activate))]
    public static class BonnieAbility
    {
        [HarmonyPostfix]
        public static void baPostfix(Ability __instance)
        {
            var bonnieHero = InGame.instance.GetTowers().Find(tower => tower.towerModel.baseId == ModContent.TowerID<BonnieHero>());
            if (bonnieHero != null)
            {
                if (__instance.abilityModel.name == "AbilityModel_MassDetonation")
                {
                    var bloons = InGame.instance.GetBloons();
                    if (bloons != null)
                    {
                        for (int i = 0; i < bloons.Count; i++)
                        {
                            if (bloons[i].bloonModel.baseId == ModContent.BloonID<BloonstoneCart>())
                            {
                                var attackModel = bonnieHero.towerModel.GetAttackModel();
                                var dynamite = attackModel.weapons[0].projectile;
                                var explosion = dynamite.GetBehavior<CreateProjectileOnContactModel>().projectile;

                                var cartExplosionProjectile = InGame.instance.GetMainFactory().CreateEntityWithBehavior<Il2CppAssets.Scripts.Simulation.Towers.Projectiles.Projectile, ProjectileModel>(explosion);

                                cartExplosionProjectile.Position.X = bloons[i].Position.X;
                                cartExplosionProjectile.Position.Y = bloons[i].Position.Y;
                                cartExplosionProjectile.Position.Z = bloons[i].Position.Z;

                                cartExplosionProjectile.owner = InGame.instance.GetUnityToSimulation().MyPlayerNumber;

                                bloons[i].Degrade(false, bonnieHero, false);
                            }
                        }
                    }
                }
                if (__instance.abilityModel.name == "AbilityModel_BEAST")
                {
                    InGame.instance.SpawnBloons(ModContent.BloonID<BEAST>(), 1, 0);

                    var beast = InGame.instance.GetBloons().Find(bloon => bloon.bloonModel.baseId == ModContent.BloonID<BEAST>());
                }
            }
        }
    }

    public override void OnRoundStart()
    {
        base.OnRoundStart();
    }
}
