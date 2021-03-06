using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game.Host.Triggers
{
    internal static class AbilityDetach
    {
        public static void Execute(PokemonProxy pm)
        {
            Execute(pm, pm.Ability);
        }
        public static void Execute(PokemonProxy pm, int ability)
        {
            switch (ability)
            {
                case As.ZEN_MODE:
                    if (pm.CanChangeForm(555, 0)) pm.ChangeForm(0, false, "DeZenMode");
                    break;
                case As.ILLUSION:
                    ATs.DeIllusion(pm);
                    break;
                case As.FLOWER_GIFT:
                    WeatherObserver(pm, 421);
                    break;
                case As.FORECAST:
                    WeatherObserver(pm, 351);
                    break;
                case As.PRIMORDIAL_SEA:
                    ATs.DeSpWeather(pm, ability, Ls.DeHeavyRain);
                    break;
                case As.DESOLATE_LAND:
                    ATs.DeSpWeather(pm, ability, Ls.DeHarshSunlight);
                    break;
                case As.DELTA_STREAM:
                    ATs.DeSpWeather(pm, ability, Ls.DeMysteriousAirCurrent);
                    break;
            }
        }
        private static void WeatherObserver(PokemonProxy pm, int number)
        {
            if (pm.CanChangeForm(number, 0)) pm.ChangeForm(0);
            pm.OnboardPokemon.RemoveCondition(Cs.ObserveWeather);
        }
    }
}
