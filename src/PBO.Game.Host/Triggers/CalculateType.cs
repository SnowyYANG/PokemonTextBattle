using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game;

namespace PokemonBattleOnline.Game.Host.Triggers
{
  internal static class CalculateType
  {
    public static void Execute(AtkContext atk)
    {
      switch (atk.Move.Id)
      {
        case Ms.STRUGGLE: //165
          atk.Type = BattleType.Invalid;
          break;
        case Ms.HIDDEN_POWER: //237
          HiddenPower(atk);
          break;
        case Ms.NATURAL_GIFT: //363
          NatureGift(atk);
          break;
        case Ms.JUDGMENT: //449
          Judgement(atk);
          break;
        case Ms.TECHNO_BLAST: //546
          {
            var i = atk.Attacker.Pokemon.Item;
            atk.Type = i == Is.DOUSE_DRIVE ? BattleType.Water : i == Is.SHOCK_DRIVE ? BattleType.Electric : i == Is.BURN_DRIVE ? BattleType.Fire : i == Is.CHILL_DRIVE ? BattleType.Ice : BattleType.Normal;
          }
          break;
        default:
          var aer = atk.Attacker;
          if (aer.OnboardPokemon.HasCondition(Cs.Electrify)) atk.Type = BattleType.Electric;
          else if (atk.Move.Move.Type == BattleType.Normal || aer.AbilityE(As.NORMALIZE))
            if (aer.AbilityE(As.AERILATE))
            {
              atk.Type = BattleType.Flying;
              atk.SetCondition(Cs.Sukin);
            }
            else if (aer.AbilityE(As.PIXILATE))
            {
              atk.Type = BattleType.Fairy;
              atk.SetCondition(Cs.Sukin);
            }
            else if (aer.AbilityE(As.REFRIGERATE))
            {
              atk.Type = BattleType.Ice;
              atk.SetCondition(Cs.Sukin);
            }
            else atk.Type = atk.Controller.Board.HasCondition(Cs.IonDeluge) ? BattleType.Electric : BattleType.Normal;
          else atk.Type = atk.Move.Move.Type;
          break;
      }
    }

    private static void HiddenPower(AtkContext atk)
    {
      atk.Type = GameHelper.HiddenPower(atk.Attacker.Pokemon.Iv);
    }

    private static void NatureGift(AtkContext atk)
    {
      switch (atk.Attacker.Pokemon.Item)
      {
        case Is.CHERI_BERRY:
        case Is.BLUK_BERRY:
        case Is.WATMEL_BERRY:
        case Is.OCCA_BERRY:
          atk.Type = BattleType.Fire;
          break;
        case Is.CHESTO_BERRY:
        case Is.NANAB_BERRY:
        case Is.DURIN_BERRY:
        case Is.PASSHO_BERRY:
          atk.Type = BattleType.Water;
          break;
        case Is.PECHA_BERRY:
        case Is.BELUE_BERRY:
        case Is.WEPEAR_BERRY:
        case Is.WACAN_BERRY:
          atk.Type = BattleType.Electric;
          break;
        case Is.RAWST_BERRY:
        case Is.PINAP_BERRY:
        case Is.RINDO_BERRY:
        case Is.LIECHI_BERRY:
          atk.Type = BattleType.Grass;
          break;
        case Is.ASPEAR_BERRY:
        case Is.POMEG_BERRY:
        case Is.YACHE_BERRY:
        case Is.GANLON_BERRY:
          atk.Type = BattleType.Ice;
          break;
        case Is.LEPPA_BERRY:
        case Is.KELPSY_BERRY:
        case Is.CHOPLE_BERRY:
        case Is.SALAC_BERRY:
          atk.Type = BattleType.Fighting;
          break;
        case Is.ORAN_BERRY:
        case Is.QUALOT_BERRY:
        case Is.KEBIA_BERRY:
        case Is.PETAYA_BERRY:
          atk.Type = BattleType.Poison;
          break;
        case Is.PERSIM_BERRY:
        case Is.HONDEW_BERRY:
        case Is.SHUCA_BERRY:
        case Is.APICOT_BERRY:
          atk.Type = BattleType.Ground;
          break;
        case Is.LUM_BERRY:
        case Is.GREPA_BERRY:
        case Is.COBA_BERRY:
        case Is.LANSAT_BERRY:
          atk.Type = BattleType.Flying;
          break;
        case Is.SITRUS_BERRY:
        case Is.TAMATO_BERRY:
        case Is.PAYAPA_BERRY:
        case Is.STARF_BERRY:
          atk.Type = BattleType.Psychic;
          break;
        case Is.FIGY_BERRY:
        case Is.CORNN_BERRY:
        case Is.TANGA_BERRY:
        case Is.ENIGMA_BERRY:
          atk.Type = BattleType.Bug;
          break;
        case Is.WIKI_BERRY:
        case Is.MAGOST_BERRY:
        case Is.CHARTI_BERRY:
        case Is.MICLE_BERRY:
          atk.Type = BattleType.Rock;
          break;
        case Is.MAGO_BERRY:
        case Is.RABUTA_BERRY:
        case Is.KASIB_BERRY:
        case Is.CUSTAP_BERRY:
          atk.Type = BattleType.Ghost;
          break;
        case Is.AGUAV_BERRY:
        case Is.NOMEL_BERRY:
        case Is.HABAN_BERRY:
        case Is.JABOCA_BERRY:
          atk.Type = BattleType.Dragon;
          break;
        case Is.IAPAPA_BERRY:
        case Is.SPELON_BERRY:
        case Is.COLBUR_BERRY:
        case Is.ROWAP_BERRY:
          atk.Type = BattleType.Dark;
          break;
        case Is.RAZZ_BERRY:
        case Is.PAMTRE_BERRY:
        case Is.BABIRI_BERRY:
          atk.Type = BattleType.Steel;
          break;
        case Is.ROSELI_BERRY:
        case Is.KEE_BERRY:
        case Is.MARANGA_BERRY:
          atk.Type = BattleType.Fairy;
          break;
        case Is.CHILAN_BERRY:
          atk.Type = BattleType.Normal;
          break;
      }
    }

    private static void Judgement(AtkContext atk)
    {
      switch (atk.Attacker.Pokemon.Item)
      {
        case Is.FLAME_PLATE:
          atk.Type = BattleType.Fire;
          break;
        case Is.SPLASH_PLATE:
          atk.Type = BattleType.Water;
          break;
        case Is.ZAP_PLATE:
          atk.Type = BattleType.Electric;
          break;
        case Is.MEADOW_PLATE:
          atk.Type = BattleType.Grass;
          break;
        case Is.ICICLE_PLATE:
          atk.Type = BattleType.Ice;
          break;
        case Is.FIST_PLATE:
          atk.Type = BattleType.Fighting;
          break;
        case Is.TOXIC_PLATE:
          atk.Type = BattleType.Poison;
          break;
        case Is.EARTH_PLATE:
          atk.Type = BattleType.Ground;
          break;
        case Is.SKY_PLATE:
          atk.Type = BattleType.Flying;
          break;
        case Is.MIND_PLATE:
          atk.Type = BattleType.Psychic;
          break;
        case Is.INSECT_PLATE:
          atk.Type = BattleType.Bug;
          break;
        case Is.STONE_PLATE:
          atk.Type = BattleType.Rock;
          break;
        case Is.SPOOKY_PLATE:
          atk.Type = BattleType.Ghost;
          break;
        case Is.DRACO_PLATE:
          atk.Type = BattleType.Dragon;
          break;
        case Is.DREAD_PLATE:
          atk.Type = BattleType.Dark;
          break;
        case Is.IRON_PLATE:
          atk.Type = BattleType.Steel;
          break;
        case Is.PIXIE_PLATE:
          atk.Type = BattleType.Fairy;
          break;
        default:
          atk.Type = BattleType.Normal;
          break;
      }
    }
  }
}
