using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal static partial class PTs
    {
        public static void Faint(this PokemonProxy pm)
        {
            pm.Pokemon.Hp = 0;
            pm.CheckFaint();
        }
        public static int MoveHurt(this PokemonProxy pm, int damage, bool ability)
        {
            if (damage >= pm.Hp)
            {
                damage = pm.Hp;
                if (STs.Remaining1HP(pm, ability))
                {
                    damage--;
                    pm.Pokemon.SetHp(1);
                }
                else pm.Pokemon.SetHp(0);
            }
            else pm.Pokemon.SetHp(pm.Hp - damage);
            if (damage != 0) pm.OnboardPokemon.SetTurnCondition(Cs.Assurance);
            return damage;
        }
        public static void HpRecover(this PokemonProxy pm, int changeHp, bool showFail = false, string log = Ls.HpRecover, int arg1 = 0, bool consumeItem = false)
        {
            if (CanHpRecover(pm, showFail))
            {
                if (consumeItem) pm.ConsumeItem();
                if (changeHp == 0) changeHp = 1;
                ShowLogPm(pm, log, arg1);
                pm.Hp += changeHp;
            }
        }
        public static void HpRecoverByOneNth(this PokemonProxy pm, int n, bool showFail = false, string log = Ls.HpRecover, int arg1 = 0, bool consumeItem = false)
        {
            int hp = pm.Pokemon.MaxHp / n;
            HpRecover(pm, hp, showFail, log, arg1, consumeItem);
        }
        public static bool EffectHurt(this PokemonProxy pm, int hp, string log = Ls.Hurt, int arg1 = 0, int arg2 = 0)
        {
            if (pm.CanEffectHurt)
            {
                EffectHurtImplement(pm, hp, log, arg1, arg2);
                return true;
            }
            return false;
        }
        public static void EffectHurtImplement(this PokemonProxy pm, int hp, string log = Ls.Hurt, int arg1 = 0, int arg2 = 0)
        {
            if (hp == 0) hp = 1;
            ShowLogPm(pm, log, arg1, arg2);
            pm.Hp -= hp;
            HpChanged.Execute(pm);
            pm.OnboardPokemon.SetTurnCondition(Cs.Assurance);
        }
        /// <summary>
        /// 不会立刻倒下
        /// </summary>
        public static bool EffectHurtByOneNth(this PokemonProxy pm, int n, string log = Ls.Hurt, int arg1 = 0, int arg2 = 0)
        {
            return EffectHurt(pm, pm.Pokemon.MaxHp / n, log, arg1, arg2);
        }
        public static void EffectHurtByOneNthImplement(this PokemonProxy pm, int n, string log = Ls.Hurt, int arg1 = 0, int arg2 = 0)
        {
            EffectHurtImplement(pm, pm.Pokemon.MaxHp / n, log, arg1, arg2);
        }
    }
}
