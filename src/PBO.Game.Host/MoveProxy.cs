using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.GameEvents;

namespace PokemonBattleOnline.Game.Host
{
    internal class MoveProxy
    {
        public readonly Move Move;
        public readonly MoveTypeE MoveE;
        public readonly PokemonProxy Owner;

        internal MoveProxy(Move move, PokemonProxy owner)
        {
            Move = move;
            MoveE = MoveTypeE.Get(move.Type);
            Owner = owner;
        }
        internal MoveProxy(MoveTypeE move, PokemonProxy owner)
        {
            Move = new Move(move.Move, 5);
            MoveE = move;
            Owner = owner;
        }

        public int PP
        {
            get { return Move.PP.Value; }
            set
            {
                var pp = (PairValue)Move.PP;
                if (value < 0) value = 0;
                else if (value > pp.Origin) value = pp.Origin;
                if (pp.Value != value)
                {
                    pp.Value = value;
                    Owner.Controller.ReportBuilder.SetPP(this);
                }
            }
        }
        public bool HasUsed
        { get; internal set; }

        public bool CanBeSelected
        { get { return PP > 0 && IfSelected() == null; } }

        /// <summary>
        /// CanSelect不代表技能一定能用，http://www.smogon.com/dp/articles/move_restrictions#disable
        /// </summary>
        /// <returns>key, null if no problem</returns>
        internal SelectMoveFail IfSelected()
        {
            if (MoveE.Id != Ms.STRUGGLE)
            {
                var op = Owner.OnboardPokemon;
                //专爱
                {
                    var o = op.GetCondition<MoveTypeE>(Cs.ChoiceItem);
                    if (o != null && o != MoveE && ITs.ChoiceItem(Owner.Item)) return new SelectMoveFail("ChoiceItem", o.Id);
                }
                //寻衅
                if (op.HasCondition(Cs.Torment) && Owner.LastMove == MoveE) return new SelectMoveFail("Torment", Owner.AtkContext.MoveProxy.MoveE.Id);
                //鼓掌
                {
                    var o = op.GetCondition(Cs.Encore);
                    if (o != null && o.Move != MoveE) return new SelectMoveFail("Encore", Owner.AtkContext.MoveProxy.MoveE.Id);
                }
                //封印
                foreach (var pm in Owner.Controller.Board[1 - Owner.Pokemon.TeamId].GetAdjacentPokemonsByOppositeX(Owner.OnboardPokemon.X))
                    if (pm.OnboardPokemon.HasCondition(Cs.Imprison))
                        foreach (var m in pm.Moves)
                            if (m.MoveE == MoveE) return new SelectMoveFail("Imprison", MoveE.Id);
                //残废
                {
                    var o = op.GetCondition(Cs.Disable);
                    if (o != null && o.Move == MoveE) return new SelectMoveFail("Disable", MoveE.Id);
                }
                //重力
                if (MoveE.UnavailableWithGravity && Owner.Controller.Board.HasCondition(Cs.Gravity)) return new SelectMoveFail("GravityCantUseMove", MoveE.Id);
                //回复封印
                if (MoveE.Heal && op.HasCondition(Cs.HealBlock)) return new SelectMoveFail("HealBlockCantUseMove", MoveE.Id);
                //挑拨
                if (MoveE.Move.Category == MoveCategory.Status && op.HasCondition(Cs.Taunt)) return new SelectMoveFail("Taunt", MoveE.Id);
                //突击背心
                if (MoveE.Move.Category == MoveCategory.Status && Owner.ItemE(Is.ASSAULT_VEST)) return new SelectMoveFail("AssaultVest", MoveE.Id);
            }
            return null;
        }

        internal void Execute()
        {
            if (ITs.ChoiceItem(Owner.Item)) Owner.OnboardPokemon.AddCondition(Cs.ChoiceItem, MoveE);
            Owner.ShowLogPm("UseMove", MoveE.Id);
            Owner.BuildAtkContext(this);
            Owner.AtkContext.StartExecute(MoveE, Owner.SelectedTarget, null);
            HasUsed = true;
        }

        internal bool CanExecute()
        {
            if (MoveE.Id != Ms.STRUGGLE)
            {
                if (PP == 0)
                {
                    Owner.Controller.ReportBuilder.ShowLog("NoPP");
                    return false;
                }
                PP--;
            }
            return true;
        }
    }
}
