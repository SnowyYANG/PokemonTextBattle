using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline.Game
{
    public class SimOnboardPokemon
    {
        public readonly PokemonOutward Outward;
        public int X;

        internal SimOnboardPokemon(SimPokemon pokemon, PokemonOutward outward)
        {
            _pokemon = pokemon;
            Outward = outward;
            X = outward.Position.X;
            Moves = new SimMove[4];
            for (int i = 0; i < pokemon.Moves.Length; ++i) Moves[i] = new SimMove(pokemon.Moves[i]);
        }

        public int Id
        { get { return Pokemon.Id; } }

        private readonly SimPokemon _pokemon;
        public SimPokemon Pokemon
        { get { return _pokemon; } }

        public SimMove[] Moves
        { get; private set; }

        internal void ChangeMoves(int[] moves)
        {
            int i = -1;
            while (++i < moves.Length) Moves[i] = new SimMove(RomData.GetMove(moves[i]));
            while (i < 4) Moves[i++] = null;
        }

        public void ChangeMove(int from, int to)
        {
            for (int i = 0; i < 4; ++i)
                if (Moves[i] != null && Moves[i].Type.Id == from)
                {
                    Moves[i] = new SimMove(RomData.GetMove(to));
                    break;
                }
        }
    }
}
