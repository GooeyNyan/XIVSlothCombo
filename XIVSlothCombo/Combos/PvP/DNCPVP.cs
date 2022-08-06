using Dalamud.Game.ClientState.JobGauge.Types;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.Core;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.Services;

namespace XIVSlothCombo.Combos.PvP
{
    internal static class DNCPvP
    {
        public const byte JobID = 38;
        // public const byte JobID = 18;

        internal const uint
            FountainCombo = 54,
            Cascade = 17756,
            Fountain = 17757,
            ReverseCascade = 17758,
            Fountainfall = 17759,
            SaberDance = 17760,
            FanDance = 17761,
            FanDanceii = 18997,
            FanDanceiii = 17762,
            CuringWaltz = 17763,
            StandardStep = 17766,
            TechnicalStep = 17824,
            Emboite = 17767,
            Entrechat = 17768,
            Jete = 17769,
            Pirouette = 17770,
            Focus = 18955,
            HeadGraze = 17680,
            Recuperate = 18928,
            MedicalKit = 18943;

        internal class Buffs
        {
            internal const ushort
                StandardStep = 2023,
                StandardFinish = 2024,
                TechnicalStep = 2049,
                TechnicalFinish = 2050,
                FanDanceiiiReady = 2021,
                FanDanceiii = 2052,
                SaberDance = 2022,
                Focus = 2186;
        }
        public static class Config
        {
            public const string
                DNCPvP_WaltzThreshold = "DNCWaltzThreshold";
        }

        internal class DNCPvP_BurstMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.DNCPvP_BurstMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Cascade or Fountain or ReverseCascade or Fountainfall)
                {
                    DNCGauge? gauge = GetJobGauge<DNCGauge>();

                    bool standardStepReady = !GetCooldown(StandardStep).IsCooldown;
                    bool technicalStep = !GetCooldown(TechnicalStep).IsCooldown;
                    bool curingWaltzReady = !GetCooldown(CuringWaltz).IsCooldown;
                    bool focusReady= !GetCooldown(Focus).IsCooldown;
                    bool headGrazeReady = !GetCooldown(HeadGraze).IsCooldown;
                    bool recuperateReady = !GetCooldown(Recuperate).IsCooldown;
                    
                    bool saberDance = HasEffect(Buffs.SaberDance);
                    bool fanDanceiii = HasEffect(Buffs.FanDanceiiiReady);
                    bool focus = HasEffect(Buffs.Focus);

                    ushort fanDanceiiiCharges = GetRemainingCharges(FanDanceiii);
                    ushort medicalKitCharges = GetRemainingCharges(MedicalKit);

                    bool canWeave = CanWeave(actionID);
                    var distance = GetTargetDistance();
                    var HPThreshold = PluginConfiguration.GetCustomIntValue(Config.DNCPvP_WaltzThreshold);
                    var HP = PlayerHealthPercentageHp();

                    if (canWeave)
                    {
                        if (fanDanceiiiCharges >= 1 && fanDanceiii)
                            return OriginalHook(FanDanceiii);
                        if (gauge.Feathers > 0)
                            return OriginalHook(FanDance);
                        if (headGrazeReady && HasTarget())
                            return OriginalHook(HeadGraze);
                        if (HP <= HPThreshold)
                        {
                            if (IsEnabled(CustomComboPreset.DNCPvP_BurstMode_CuringWaltz) && curingWaltzReady)
                                return OriginalHook(CuringWaltz);
                            if (medicalKitCharges >= 1)
                                return OriginalHook(MedicalKit);
                            if (recuperateReady)
                                return OriginalHook(Recuperate);
                        }
                    }

                    if (gauge.Esprit >= 50)
                    {
                        if (!saberDance)
                        {
                            return OriginalHook(SaberDance);
                        }
                        if (!focus && (focusReady || GetCooldown(Focus).CooldownRemaining < 0.5))
                        {
                            return OriginalHook(Focus);
                        }
                        return OriginalHook(SaberDance);
                    }
                } else if (actionID is Focus)
                {
                    bool focus = HasEffect(Buffs.Focus);
                    if (focus)
                    {
                        return OriginalHook(SaberDance);
                    }
                }

                return actionID;
            }
        }
    }
}
