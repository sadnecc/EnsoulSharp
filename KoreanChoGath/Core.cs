﻿namespace KoreanChoGath
{
    using System.Collections.Generic;

    using KoreanCommon;

    using EnsoulSharp;
    using EnsoulSharp.Common;

    internal class Core : CommonCore
    {
        private readonly KoreanPrediction koreanPredictionQ;

        private readonly KoreanPrediction koreanPredictionW;

        public Core(CommonChampion champion)
            : base(champion)
        {
            koreanPredictionQ = new KoreanPrediction(spells.Q);
            koreanPredictionW = new KoreanPrediction(spells.W, KoreanPredictionTypes.Fast);
        }

        private void UseQ(AIHeroClient target)
        {
            if (!spells.Q.IsReadyToCastOn(target))
            {
                return;
            }

            koreanPredictionQ.Cast(target);
        }

        private void UseW(AIHeroClient target)
        {
            if (!spells.W.IsReadyToCastOn(target))
            {
                return;
            }

            koreanPredictionW.Cast(target);
        }

        private bool HaveManaToHaras()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittoharas");
        }

        private bool HaveManaToLaneclear()
        {
            return champion.Player.ManaPercent > KoreanUtils.GetParamSlider(champion.MainMenu, "manalimittolaneclear");
        }

        public override void LastHitMode()
        {
            //
        }

        public override void HarasMode()
        {
            if (!HaveManaToHaras())
            {
                return;
            }

            AIHeroClient target = TargetSelector.GetTarget(
                champion.Spells.MaxRangeHaras,
                TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (Q.UseOnHaras)
            {
                UseQ(target);
            }

            if (!HaveManaToHaras())
            {
                return;
            }

            if (W.UseOnHaras)
            {
                UseW(target);
            }
        }

        public override void LaneClearMode()
        {
            if (!HaveManaToLaneclear())
            {
                return;
            }
            if (spells.Q.UseOnLaneClear && spells.Q.IsReady() && spells.Q.CanCast())
            {
                List<AIBaseClient> minions = MinionManager.GetMinions(champion.Player.Position, spells.Q.Range);
                MinionManager.FarmLocation farmLocation = spells.Q.GetCircularFarmLocation(minions);

                if (farmLocation.MinionsHit >= KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstoq"))
                {
                    spells.Q.Cast(farmLocation.Position);
                }
            }

            if (!HaveManaToLaneclear())
            {
                return;
            }

            if (spells.W.UseOnLaneClear && spells.W.IsReady() && spells.W.CanCast())
            {
                List<AIBaseClient> minions = MinionManager.GetMinions(champion.Player.Position, spells.W.Range);

                MinionManager.FarmLocation wFarmLocation = spells.W.GetCircularFarmLocation(minions);

                if (wFarmLocation.MinionsHit >= KoreanUtils.GetParamSlider(champion.MainMenu, "minminionstow"))
                {
                    spells.W.Cast(wFarmLocation.Position);
                }
            }

            if (KoreanUtils.GetParamBool(champion.MainMenu, "harasonlaneclear"))
            {
                HarasMode();
            }
        }

        public override void ComboMode()
        {
            AIHeroClient target = TargetSelector.GetTarget(
                champion.Spells.MaxRangeHaras,
                TargetSelector.DamageType.Magical);

            if (target == null)
            {
                return;
            }

            if (Q.UseOnCombo)
            {
                UseQ(target);
            }

            if (W.UseOnCombo)
            {
                UseW(target);
            }

            if (R.IsKillable(target))
            {
                Ultimate(target);
            }
        }

        public override void Ultimate(AIHeroClient target)
        {
            if (!R.IsReady())
            {
                return;
            }

            AIHeroClient championTargeted;
            if (target != null)
            {
                championTargeted = target;
            }
            else
            {
                championTargeted = TargetSelector.GetTarget(spells.R.Range, TargetSelector.DamageType.Magical);
            }

            if (championTargeted != null)
            {
                R.Cast(championTargeted);
            }
        }
    }
}