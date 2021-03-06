using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
  public static class GameHelper
  {
    public static readonly StatType[] SEVEN_D = { StatType.Atk, StatType.Def, StatType.SpAtk, StatType.SpDef, StatType.Speed, StatType.Accuracy, StatType.Evasion };
    /// <summary>
    /// [atk, def]
    /// </summary>
    private static readonly int[,] REVISE = new int[19, 19] { { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0, 0, -1, 0, 0, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, { 0, 1, 0, -1, -1, 0, 1, -1, 0, 1, 0, 0, 0, 0, -1, 1, 0, 1, -1 }, { 0, 0, 1, 0, 0, 0, -1, 1, 0, -1, 0, 0, 1, -1, 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, -1, -1, -1, 0, -1, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1 }, { 0, 0, 0, 0, 1, 0, 1, -1, 0, 1, 1, 0, -1, 1, 0, 0, 0, 0, 0 }, { 0, 0, -1, 1, 0, -1, 0, 1, 0, -1, 1, 0, 0, 0, 0, 1, 0, 0, 0 }, { 0, 0, -1, -1, -1, 0, 0, 0, -1, -1, -1, 0, 1, 0, 1, 0, 0, 1, -1 }, { 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, 0 }, { 0, 0, 0, 0, 0, 0, 1, 0, 0, -1, -1, -1, 0, -1, 0, 1, 0, 0, 1 }, { 0, 0, 0, 0, 0, 0, -1, 1, 0, 1, -1, -1, 1, 0, 0, 1, -1, 0, 0 }, { 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, -1, -1, 0, 0, 0, -1, 0, 0 }, { 0, 0, 0, -1, -1, 1, 1, -1, 0, -1, -1, 1, -1, 0, 0, 0, -1, 0, 0 }, { 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1, -1, -1, 0, 0, -1, 0, 0 }, { 0, 0, 1, 0, 1, 0, 0, 0, 0, -1, 0, 0, 0, 0, -1, 0, 0, 0, 0 }, { 0, 0, 0, 1, 0, 1, 0, 0, 0, -1, -1, -1, 1, 0, 0, -1, 1, 0, 0 }, { 0, 0, 0, 0, 0, 0, 0, 0, 0, -1, 0, 0, 0, 0, 0, 0, 1, 0, 0 }, { 0, 0, -1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, -1, -1 }, { 0, 0, 1, 0, -1, 0, 0, 0, 0, -1, -1, 0, 0, 0, 0, 0, 1, 1, 0 } };
    /// <summary>
    /// [atk]
    /// </summary>
    private static readonly BattleType[] NO_EFFECT;

    static GameHelper()
    {
      NO_EFFECT = new BattleType[RomData.BATTLETYPES + 1];
      NO_EFFECT[(int)BattleType.Normal] = NO_EFFECT[(int)BattleType.Fighting] = BattleType.Ghost;
      NO_EFFECT[(int)BattleType.Electric] = BattleType.Ground;
      NO_EFFECT[(int)BattleType.Poison] = BattleType.Steel;
      NO_EFFECT[(int)BattleType.Psychic] = BattleType.Dark;
      NO_EFFECT[(int)BattleType.Ghost] = BattleType.Normal;
      NO_EFFECT[(int)BattleType.Ground] = BattleType.Flying;
      NO_EFFECT[(int)BattleType.Dragon] = BattleType.Fairy;
    }
    public static int Get5D(StatType statType, PokemonNature nature, int typeBase, int iv, int ev, int lv)
    {
      return (((typeBase << 1) + iv + (ev >> 2)) * lv / 100 + 5) * nature.StatRevise(statType) / 10;
    }
    public static int GetHp(int typeBase, int iv, int ev, int lv)
    {
      return typeBase == 1 ? 1 : (((typeBase << 1) + iv + (ev >> 2)) * lv / 100 + 10 + lv);
    }

    public static bool NoEffect(this BattleType a, BattleType d)
    {
      return d != BattleType.Invalid && NO_EFFECT[(int)a] == d;
    }
    public static BattleType NoEffect(this BattleType a)
    {
      return NO_EFFECT[(int)a];
    }
    public static int EffectRevise(this BattleType a, BattleType d)
    {
      return REVISE[(int)a, (int)d];
    }
    public static int EffectRevise(this BattleType a, IEnumerable<BattleType> d)
    {
      var r = 0;
      foreach(var t in d) r += EffectRevise(a, t);
      return r;
    }

    public static BattleType HiddenPower(I6D iv)
    {
      int pI = iv.Hp & 1;
      pI |= (iv.Atk & 1) << 1;
      pI |= (iv.Def & 1) << 2;
      pI |= (iv.Speed & 1) << 3;
      pI |= (iv.SpAtk & 1) << 4;
      pI |= (iv.SpDef & 1) << 5;
      return (BattleType)(pI * 15 / 63 + 2);
    }
  }
}
