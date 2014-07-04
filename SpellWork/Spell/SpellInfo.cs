﻿using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SpellWork.Database;
using SpellWork.DBC;
using SpellWork.Extensions;

namespace SpellWork.Spell
{
    class SpellInfo
    {
        private readonly RichTextBox _rtb;
        private SpellEntry _spell;

        private const string _line = "=================================================";

        public SpellInfo(RichTextBox rtb, SpellEntry spell)
        {
            _rtb = rtb;
            _spell = spell;

            ProcInfo.SpellProc = spell;

            ViewSpellInfo();
        }

        private void ViewSpellInfo()
        {
            _rtb.Clear();
            _rtb.SetBold();
            _rtb.AppendFormatLine("ID - {0} {1}", _spell.ID, _spell.SpellNameRank);
            _rtb.SetDefaultStyle();

            _rtb.AppendFormatLine(_line);
            _rtb.AppendFormatLineIfNotNull("Description: {0}", _spell.Description);
            _rtb.AppendFormatLineIfNotNull("ToolTip: {0}", _spell.ToolTip);
            _rtb.AppendFormatLineIfNotNull("Modal Next Spell: {0}", _spell.ModalNextSpell);
            if (_spell.Description != string.Empty && _spell.ToolTip != string.Empty && _spell.ModalNextSpell != 0)
                _rtb.AppendFormatLine(_line);

            _rtb.AppendFormatLine("Category = {0}, SpellIconID = {1}, activeIconID = {2}, SpellVisual = ({3},{4})",
                _spell.Category, _spell.SpellIconID, _spell.ActiveIconID, _spell.SpellVisual[0], _spell.SpellVisual[1]);

            _rtb.AppendFormatLine("Family {0}, flag [0] 0x{1:X8} [1] 0x{2:X8}",
                (SpellFamilyNames)_spell.SpellFamilyName, _spell.SpellFamilyFlags[0], _spell.SpellFamilyFlags[1]);

            _rtb.AppendLine();

            _rtb.AppendFormatLine("SpellSchoolMask = {0} ({1})", _spell.SchoolMask, _spell.School);
            _rtb.AppendFormatLine("DamageClass = {0} ({1})", _spell.DmgClass, (SpellDmgClass)_spell.DmgClass);
            _rtb.AppendFormatLine("PreventionType = {0} ({1})", _spell.PreventionType, (SpellPreventionType)_spell.PreventionType);

            if (_spell.Attributes != 0 || _spell.AttributesEx != 0 || _spell.AttributesEx2 != 0 || _spell.AttributesEx3 != 0
                || _spell.AttributesEx4 != 0 || _spell.AttributesEx5 != 0 || _spell.AttributesEx6 != 0)
                _rtb.AppendLine(_line);

            if (_spell.Attributes != 0)
                _rtb.AppendFormatLine("Attributes: 0x{0:X8} ({1})", _spell.Attributes, (SpellAtribute)_spell.Attributes);
            if (_spell.AttributesEx != 0)
                _rtb.AppendFormatLine("AttributesEx1: 0x{0:X8} ({1})", _spell.AttributesEx, (SpellAtributeEx)_spell.AttributesEx);
            if (_spell.AttributesEx2 != 0)
                _rtb.AppendFormatLine("AttributesEx2: 0x{0:X8} ({1})", _spell.AttributesEx2, (SpellAtributeEx2)_spell.AttributesEx2);
            if (_spell.AttributesEx3 != 0)
                _rtb.AppendFormatLine("AttributesEx3: 0x{0:X8} ({1})", _spell.AttributesEx3, (SpellAtributeEx3)_spell.AttributesEx3);
            if (_spell.AttributesEx4 != 0)
                _rtb.AppendFormatLine("AttributesEx4: 0x{0:X8} ({1})", _spell.AttributesEx4, (SpellAtributeEx4)_spell.AttributesEx4);
            if (_spell.AttributesEx5 != 0)
                _rtb.AppendFormatLine("AttributesEx5: 0x{0:X8} ({1})", _spell.AttributesEx5, (SpellAtributeEx5)_spell.AttributesEx5);
            if (_spell.AttributesEx6 != 0)
                _rtb.AppendFormatLine("AttributesEx6: 0x{0:X8} ({1})", _spell.AttributesEx6, (SpellAtributeEx6)_spell.AttributesEx6);

            _rtb.AppendLine(_line);

            if (_spell.Targets != 0)
                _rtb.AppendFormatLine("Targets Mask = 0x{0:X8} ({1})", _spell.Targets, (SpellCastTargetFlags)_spell.Targets);

            if (_spell.TargetCreatureType != 0)
                _rtb.AppendFormatLine("Creature Type Mask = 0x{0:X8} ({1})",
                    _spell.TargetCreatureType, (CreatureTypeMask)_spell.TargetCreatureType);

            if (_spell.Stances != 0)
                _rtb.AppendFormatLine("Stances: {0}", (ShapeshiftFormMask)_spell.Stances);

            if (_spell.StancesNot != 0)
                _rtb.AppendFormatLine("Stances Not: {0}", (ShapeshiftFormMask)_spell.StancesNot);

            AppendSkillLine();

            // reagents
            {
                var printedHeader = false;
                for (var i = 0; i < _spell.Reagent.Length; ++i)
                {
                    if (_spell.Reagent[i] == 0)
                        continue;

                    if (!printedHeader)
                    {
                        _rtb.AppendLine();
                        _rtb.Append("Reagents:");
                        printedHeader = true;
                    }

                    _rtb.AppendFormat("  {0} x{1}", _spell.Reagent[i], _spell.ReagentCount[i]);
                }

                if (printedHeader)
                    _rtb.AppendLine();
            }

            _rtb.AppendFormatLine("Spell Level = {0}, base {1}, max {2}, maxTarget {3}",
                _spell.SpellLevel, _spell.BaseLevel, _spell.MaxLevel, _spell.MaxTargetLevel);

            if (_spell.EquippedItemClass != -1)
            {
                _rtb.AppendFormatLine("EquippedItemClass = {0} ({1})", _spell.EquippedItemClass, (ItemClass)_spell.EquippedItemClass);

                if (_spell.EquippedItemSubClassMask != 0)
                {
                    switch ((ItemClass)_spell.EquippedItemClass)
                    {
                        case ItemClass.WEAPON:
                            _rtb.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassWeaponMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.ARMOR:
                            _rtb.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassArmorMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.MISC:
                            _rtb.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassMiscMask)_spell.EquippedItemSubClassMask);
                            break;
                    }
                }

                if (_spell.EquippedItemInventoryTypeMask != 0)
                    _rtb.AppendFormatLine("    InventoryType mask = 0x{0:X8} ({1})",
                        _spell.EquippedItemInventoryTypeMask, (InventoryTypeMask)_spell.EquippedItemInventoryTypeMask);
            }

            _rtb.AppendLine();
            _rtb.AppendFormatLine("Category = {0}", _spell.Category);
            _rtb.AppendFormatLine("DispelType = {0} ({1})", _spell.Dispel, (DispelType)_spell.Dispel);
            _rtb.AppendFormatLine("Mechanic = {0} ({1})", _spell.Mechanic, (Mechanics)_spell.Mechanic);

            _rtb.AppendLine(_spell.Range);

            _rtb.AppendFormatLineIfNotNull("Speed {0:F}", _spell.Speed);
            _rtb.AppendFormatLineIfNotNull("Stackable up to {0}", _spell.StackAmount);

            _rtb.AppendLine(_spell.CastTime);

            if (_spell.RecoveryTime != 0 || _spell.CategoryRecoveryTime != 0 || _spell.StartRecoveryCategory != 0)
            {
                _rtb.AppendFormatLine("RecoveryTime: {0} ms, CategoryRecoveryTime: {1} ms", _spell.RecoveryTime, _spell.CategoryRecoveryTime);
                _rtb.AppendFormatLine("StartRecoveryCategory = {0}, StartRecoveryTime = {1:F} ms", _spell.StartRecoveryCategory, _spell.StartRecoveryTime);
            }

            _rtb.AppendLine(_spell.Duration);

            if (_spell.ManaCost != 0 || _spell.ManaCostPercentage != 0)
            {
                _rtb.AppendFormat("Power {0}, Cost {1}",
                    (Powers)_spell.PowerType, _spell.ManaCost == 0 ? _spell.ManaCostPercentage + " %" : _spell.ManaCost.ToString());
                _rtb.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaCostPerlevel);
                _rtb.AppendFormatIfNotNull(" + {0} Per Second", _spell.ManaPerSecond);
                _rtb.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaPerSecondPerLevel);
                _rtb.AppendLine();
            }

            _rtb.AppendFormatLine("Interrupt Flags: 0x{0:X8}, AuraIF 0x{1:X8}, ChannelIF 0x{2:X8}",
                _spell.InterruptFlags, _spell.AuraInterruptFlags, _spell.ChannelInterruptFlags);

            if (_spell.CasterAuraState != 0)
                _rtb.AppendFormatLine("CasterAuraState = {0} ({1})", _spell.CasterAuraState, (AuraState)_spell.CasterAuraState);

            if (_spell.TargetAuraState != 0)
                _rtb.AppendFormatLine("TargetAuraState = {0} ({1})", _spell.TargetAuraState, (AuraState)_spell.TargetAuraState);

            if (_spell.CasterAuraStateNot != 0)
                _rtb.AppendFormatLine("CasterAuraStateNot = {0} ({1})", _spell.CasterAuraStateNot, (AuraState)_spell.CasterAuraStateNot);

            if (_spell.TargetAuraStateNot != 0)
                _rtb.AppendFormatLine("TargetAuraStateNot = {0} ({1})", _spell.TargetAuraStateNot, (AuraState)_spell.TargetAuraStateNot);

            if (_spell.MaxAffectedTargets != 0)
                _rtb.AppendFormatLine("MaxAffectedTargets = {0}", _spell.MaxAffectedTargets);

            _rtb.AppendFormatLineIfNotNull("Requires Spell Focus {0}", _spell.RequiresSpellFocus);

            if (_spell.ProcFlags != 0)
            {
                _rtb.SetBold();
                _rtb.AppendFormatLine("Proc flag 0x{0:X8}, chance = {1}, charges - {2}",
                _spell.ProcFlags, _spell.ProcChance, _spell.ProcCharges);
                _rtb.SetDefaultStyle();
                _rtb.AppendFormatLine(_line);
                _rtb.AppendText(_spell.ProcInfo);
            }
            else
            {
                _rtb.AppendFormatLine("Chance = {0}, charges - {1}", _spell.ProcChance, _spell.ProcCharges);
            }

            AppendSpellEffectInfo();
            AppendItemInfo();
        }
        private void AppendSkillLine()
        {
            var query = from skillLineAbility in DBC.DBC.SkillLineAbility
                        join skillLine in DBC.DBC.SkillLine
                        on skillLineAbility.Value.SkillId equals skillLine.Key
                        where skillLineAbility.Value.SpellId == _spell.ID
                        select new
                        {
                            skillLineAbility,
                            skillLine
                        };

            if (query.Count() == 0)
                return;

            var skill = query.First().skillLineAbility.Value;
            var line =  query.First().skillLine.Value;

            _rtb.AppendFormatLine("Skill (Id {0}) \"{1}\"", skill.SkillId, line.Name);
            _rtb.AppendFormat("    ReqSkillValue {0}", skill.ReqSkillValue);

            _rtb.AppendFormat(", Forward Spell = {0}, MinMaxValue ({1}, {2})", skill.ForwardSpellid, skill.MinValue, skill.MaxValue);
            _rtb.AppendFormat(", CharacterPoints ({0}, {1})", skill.CharacterPoints[0], skill.CharacterPoints[1]);
        }

        private void AppendSpellEffectInfo()
        {
            _rtb.AppendLine(_line);

            for (var effectIndex = 0; effectIndex < DBC.DBC.MaxEffectIndex; effectIndex++)
            {
                _rtb.SetBold();
                if ((SpellEffects)_spell.Effect[effectIndex] == SpellEffects.SPELL_EFFECT_NONE)
                {
                    _rtb.AppendFormatLine("Effect {0}:  NO EFFECT", effectIndex);
                    _rtb.AppendLine();
                    continue;
                }

                _rtb.AppendFormatLine("Effect {0}: Id {1} ({2})", effectIndex, _spell.Effect[effectIndex], (SpellEffects)_spell.Effect[effectIndex]);
                _rtb.SetDefaultStyle();
                _rtb.AppendFormat("BasePoints = {0}", _spell.EffectBasePoints[effectIndex] + 1);

                if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                    _rtb.AppendFormat(" + Level * {0:F}", _spell.EffectRealPointsPerLevel[effectIndex]);

                // WTF ? 1 = spell.EffectBaseDice[i]
                if (1 < _spell.EffectDieSides[effectIndex])
                {
                    if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                        _rtb.AppendFormat(" to {0} + lvl * {1:F}",
                            _spell.EffectBasePoints[effectIndex] + 1 + _spell.EffectDieSides[effectIndex], _spell.EffectRealPointsPerLevel[effectIndex]);
                    else
                        _rtb.AppendFormat(" to {0}", _spell.EffectBasePoints[effectIndex] + 1 + _spell.EffectDieSides[effectIndex]);
                }

                _rtb.AppendFormatIfNotNull(" + combo * {0:F}", _spell.EffectPointsPerComboPoint[effectIndex]);

                if (_spell.DmgMultiplier[effectIndex] != 1.0f)
                    _rtb.AppendFormat(" x {0:F}", _spell.DmgMultiplier[effectIndex]);

                _rtb.AppendFormatIfNotNull("  Multiple = {0:F}", _spell.EffectMultipleValue[effectIndex]);
                _rtb.AppendLine();

                _rtb.AppendFormatLine("Targets ({0}, {1}) ({2}, {3})",
                    _spell.EffectImplicitTargetA[effectIndex], _spell.EffectImplicitTargetB[effectIndex],
                    (Targets)_spell.EffectImplicitTargetA[effectIndex], (Targets)_spell.EffectImplicitTargetB[effectIndex]);

                AuraModTypeName(effectIndex);

                _rtb.AppendFormatLineIfNotNull("{0}", _spell.GetRadius(effectIndex));

                // append trigger spell
                var trigger = _spell.EffectTriggerSpell[effectIndex];
                if (trigger != 0)
                {
                    if (DBC.DBC.Spell.ContainsKey(trigger))
                    {
                        var triggerSpell = DBC.DBC.Spell[trigger];
                        _rtb.SetStyle(Color.Blue, FontStyle.Bold);
                        _rtb.AppendFormatLine("   Trigger spell ({0}) {1}. Chance = {2}", trigger, triggerSpell.SpellNameRank, _spell.ProcChance);
                        _rtb.AppendFormatLineIfNotNull("   Description: {0}", triggerSpell.Description);
                        _rtb.AppendFormatLineIfNotNull("   ToolTip: {0}", triggerSpell.ToolTip);
                        _rtb.SetDefaultStyle();
                        if (triggerSpell.ProcFlags != 0)
                        {
                            _rtb.AppendFormatLine("Charges - {0}", triggerSpell.ProcCharges);
                            _rtb.AppendLine(_line);
                            _rtb.AppendLine(triggerSpell.ProcInfo);
                            _rtb.AppendLine(_line);
                        }
                    }
                    else
                    {
                        _rtb.AppendFormatLine("Trigger spell ({0}) Not found, Chance = {1}", trigger, _spell.ProcChance);
                    }
                }

                _rtb.AppendFormatLineIfNotNull("EffectChainTarget = {0}", _spell.EffectChainTarget[effectIndex]);
                _rtb.AppendFormatLineIfNotNull("EffectItemType = {0}", _spell.EffectItemType[effectIndex]);

                if((Mechanics)_spell.EffectMechanic[effectIndex] != Mechanics.MECHANIC_NONE)
                    _rtb.AppendFormatLine("Effect Mechanic = {0} ({1})", _spell.EffectMechanic[effectIndex], (Mechanics)_spell.EffectMechanic[effectIndex]);

                _rtb.AppendLine();
            }
        }

        private void AuraModTypeName(int index)
        {
            var aura    = (AuraType)_spell.EffectApplyAuraName[index];
            var misc          = _spell.EffectMiscValue[index];

            if (_spell.EffectApplyAuraName[index] == 0)
            {
                _rtb.AppendFormatLineIfNotNull("EffectMiscValueA = {0}", _spell.EffectMiscValue[index]);
                _rtb.AppendFormatLineIfNotNull("EffectMiscValueB = {0}", _spell.EffectMiscValueB[index]);
                _rtb.AppendFormatLineIfNotNull("EffectAmplitude = {0}",  _spell.EffectAmplitude[index]);

                return;
            }

            _rtb.AppendFormat("Aura Id {0:D} ({0})", aura);
            _rtb.AppendFormat(", value = {0}", _spell.EffectBasePoints[index] + 1);
            _rtb.AppendFormat(", misc = {0} (", misc);

            switch (aura)
            {
                case AuraType.SPELL_AURA_MOD_STAT:
                    _rtb.Append((UnitMods)misc);
                    break;
                case AuraType.SPELL_AURA_MOD_RATING:
                    _rtb.Append((CombatRating)misc);
                    break;
                case AuraType.SPELL_AURA_ADD_FLAT_MODIFIER:
                case AuraType.SPELL_AURA_ADD_PCT_MODIFIER:
                    _rtb.Append((SpellModOp)misc);
                    break;
                // todo: more case
                default:
                    _rtb.Append(misc);
                    break;
            }

            _rtb.AppendFormat("), miscB = {0}", _spell.EffectMiscValueB[index]);
            _rtb.AppendFormatLine(", periodic = {0}", _spell.EffectAmplitude[index]);
        }

        private void AppendItemInfo()
        {
            if (!MySqlConnection.Connected)
                return;

            var items = from item in DBC.DBC.ItemTemplate
                        where  item.SpellId.ContainsElement((int)_spell.ID)
                        select item;

            if (items.Count() == 0)
                return;

            _rtb.AppendLine(_line);
            _rtb.SetStyle(Color.Blue, FontStyle.Bold);
            _rtb.AppendLine("Items using this spell:");
            _rtb.SetDefaultStyle();

            foreach (var item in items)
            {
                var name = string.IsNullOrEmpty(item.LocalesName) ? item.Name : item.LocalesName;
                var desc = string.IsNullOrEmpty(item.LocalesDescription) ? item.Description : item.LocalesDescription;

                desc = string.IsNullOrEmpty(desc) ? string.Empty : string.Format(" - \"{0}\"", desc);

                _rtb.AppendFormatLine(@"   {0}: {1} {2}", item.Entry, name, desc);
            }
        }
    }
}
