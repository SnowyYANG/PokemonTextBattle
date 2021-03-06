using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PokemonBattleOnline.Game.Host.Triggers;

namespace PokemonBattleOnline.Game.Host
{
    internal class PokemonProxy
    {
        public readonly Pokemon Pokemon;
        public readonly Controller Controller;

        internal PokemonProxy(Pokemon pokemon)
        {
            Controller = pokemon.Controller;
            Pokemon = pokemon;
            NullOnboardPokemon = new OnboardPokemon(pokemon, -1);
            StruggleMove = new MoveProxy(new Move(RomData.GetMove(Ms.STRUGGLE), 1), this);
            _moves = new List<MoveProxy>(4);
        }

        internal readonly OnboardPokemon NullOnboardPokemon;
        public bool AliveOnboard
        { get { return OnboardPokemon != NullOnboardPokemon && Hp > 0; } }
        public OnboardPokemon OnboardPokemon
        { get; internal set; }

        public PokemonAction Action
        { get; internal set; }
        public Field Field
        { get { return Controller.Board[Pokemon.TeamId]; } }
        public Tile Tile
        { get { return Controller.Board[Pokemon.TeamId][OnboardPokemon.X]; } }
        public int Id
        { get { return Pokemon.Id; } }
        public int Hp
        {
            get { return Pokemon.Hp; }
            set
            {
                if (value < 0) value = 0;
                else if (value > Pokemon.MaxHp) value = Pokemon.MaxHp;
                if (Pokemon._hp != value)
                {
                    Pokemon._hp = value;
                    Controller.ReportBuilder.ShowHp(this);
                }
            }
        }
        public PokemonState State
        { get { return Pokemon.State; } }
        public MoveProxy SelectedMove //先取
        { get; private set; }
        public Tile SelectedTarget
        { get; private set; }
        public bool SelectMega
        { get; private set; }
        private AtkContext _atkContext;
        public AtkContext AtkContext
        { get { return _atkContext; } }
        public MoveTypeE LastMove
        { get { return AtkContext == null ? null : AtkContext.MoveProxy == null ? null : AtkContext.MoveProxy.MoveE; } }
        private bool NoAbilityE
        { get { return !AliveOnboard || OnboardPokemon.HasCondition(Cs.GastroAcid); } }
        public int Ability
        { get { return NoAbilityE ? 0 : OnboardPokemon.Ability; } }
        public bool AbilityE(int ability)
        {
            return OnboardPokemon.Ability == ability && !NoAbilityE;
        }
        private bool NoItemE
        {
            get
            {
                return
                  !AliveOnboard ||
                  !ITs.CanUseItem(this) ||
                  ITs.Berry(Pokemon.Item) && Controller.Board[1 - Pokemon.TeamId].Pokemons.Any(ATs.Unnerve);
            }
        }
        public int Item
        { get { return NoItemE ? 0 : Pokemon.Item; } }
        public bool ItemE(int item)
        {
            return Pokemon.Item == item && !NoItemE;
        }
        private List<MoveProxy> _moves;
        public IEnumerable<MoveProxy> Moves
        { get { return _moves; } }
        public MoveProxy StruggleMove
        { get; private set; }
        public int Speed
        { get { return SpeedRevise.Execute(this, OnboardPokemon.Get5D(OnboardPokemon.FiveD.Speed, OnboardPokemon.Lv5D.Speed)); } }
        public double Weight
        {
            get
            {
                double w = OnboardPokemon.Weight;
                w *= ATs.WeightModifier(this);
                w *= ITs.FloatStone(this);
                return w;
            }
        }
        public CoordY CoordY
        {
            get { return OnboardPokemon.CoordY; }
            set
            {
                if (OnboardPokemon.CoordY != value)
                {
                    OnboardPokemon.CoordY = value;
                    Controller.ReportBuilder.SetY(this);
                }
            }
        }
        public bool CanTransform(PokemonProxy target)
        {
            if (target == null) return false;
            var to = target.OnboardPokemon;
            return !(OnboardPokemon.HasCondition(Cs.Transform) || to.HasCondition(Cs.Illusion) || to.HasCondition(Cs.Transform) || to.HasCondition(Cs.Substitute));
        }
        public void Transform(PokemonProxy target)
        {
            OnboardPokemon.SetCondition(Cs.Transform);
            OnboardPokemon.Transform(target.OnboardPokemon);
            _moves.Clear();
            foreach (var m in target._moves) _moves.Add(new MoveProxy(m.MoveE, this));
            Controller.ReportBuilder.Transform(this);
            Controller.ReportBuilder.ShowLog(Ls.Transform, Id, target.Id);
        }
        public void ChangeMove(MoveTypeE from, MoveTypeE to)
        {
            for (int i = 0; i < _moves.Count; ++i)
                if (_moves[i].MoveE == from)
                {
                    _moves[i] = new MoveProxy(to, this);
                    break;
                }
        }
        public int LastMoveTurn
        { get; private set; }
        public bool CanEffectHurt
        {
            get { return AliveOnboard && !AbilityE(As.MAGIC_GUARD); }
        }
        private bool CanExecute()
        {
            CoordY = CoordY.Plate;
            return Triggers.CanExecute.Execute(this);
        }

        internal void Reset()
        {
            _atkContext = null;
            SelectedMove = null;
            SelectedTarget = null;
            _moves.Clear();
            foreach (var m in Pokemon.Moves) _moves.Add(new MoveProxy(m, this));
            LastMoveTurn = 0;
        }
        internal void BuildAtkContext(MoveProxy move)
        {
            if (move.MoveE.Id == Ms.STRUGGLE) _atkContext = new AtkContext(this);
            else _atkContext = new AtkContext(move);
        }
        #region Input
        internal bool CanSelectWithdraw
        {
            get
            {
                if (OnboardPokemon.HasType(BattleType.Ghost) || ItemE(Is.SHED_SHELL)) return true;
                if (OnboardPokemon.HasCondition(Cs.Trap) || OnboardPokemon.HasCondition(Cs.Ingrain) || OnboardPokemon.HasCondition(Cs.CantSelectWithdraw) || OnboardPokemon.HasCondition(Cs.SkyDrop) || Controller.Board.GetCondition<int>(Cs.FairyLock, -1) == Controller.TurnNumber) return false;
                bool arenaTrap = false, magnetPull = false, shadowTag = false;
                foreach (var pm in Controller.GetOnboardPokemons(1 - Pokemon.TeamId))
                {
                    int ab = pm.Ability;
                    if (ab == As.SHADOW_TAG) shadowTag = true;
                    else if (ab == As.ARENA_TRAP) arenaTrap = true;
                    else if (ab == As.MAGNET_PULL) magnetPull = true;
                }
                return
                  !
                  (
                  magnetPull && OnboardPokemon.HasType(BattleType.Steel) ||
                  shadowTag && !AbilityE(As.SHADOW_TAG) ||
                  arenaTrap && HasEffect.IsGroundAffectable(this, true, false)
                  );
            }
        }
        /// <summary>
        /// 和Struggle一起的
        /// </summary>
        internal bool CanInput
        { get { return Action == PokemonAction.WaitingForInput; } }
        internal bool CheckNeedInput()
        {
            if (Action == PokemonAction.Done || State == PokemonState.SLP && Action == PokemonAction.Moving && AtkContext.Move.SkipSleepMTA)
            {
                {
                    var o = OnboardPokemon.GetCondition(Cs.Encore);
                    if (o != null)
                    {
                        foreach (var m in Moves)
                            if (m.MoveE == o.Move)
                            {
                                if (m.PP == 0) break;
                                else goto DONE1;
                            }
                        OnboardPokemon.RemoveCondition(Cs.Encore);
                    }
                }
                DONE1:
                if (ITs.ChoiceItem(Item))
                {
                    var o = OnboardPokemon.GetCondition<MoveTypeE>(Cs.ChoiceItem);
                    if (o != null)
                    {
                        foreach (var m in Moves)
                            if (m.MoveE == o) goto DONE2;
                        OnboardPokemon.RemoveCondition(Cs.ChoiceItem);
                    }
                }
                DONE2:
                Action = PokemonAction.WaitingForInput;
                return true;
            }
            return false;
        }
        internal bool InputSwitch(int sendoutIndex)
        {
            if (Controller.CanWithdraw(this) && Controller.CanSendOut(Pokemon.Owner.GetPokemon(sendoutIndex)))
            {
                Action = PokemonAction.WillSwitch;
                Tile.WillSendOutPokemonIndex = sendoutIndex;
                return true;
            }
            return false;
        }
        internal bool SelectMove(MoveProxy move, Tile target, bool mega)
        {
            if (Hp > 0 && move.CanBeSelected && (!mega || CanMega))
            {
                Action = PokemonAction.WillMove;
                SelectedMove = move;
                SelectedTarget = target;
                SelectMega = mega;
                return true;
            }
            return false;
        }
        #endregion
        #region Turn
        public int ItemSpeedValue;
        public int Priority;
        internal bool CanMove
        { get { return Hp != 0 && LastMoveTurn != Controller.TurnNumber && (Action == PokemonAction.MoveAttached || Action == PokemonAction.Stiff || Action == PokemonAction.Moving); } }
        public bool CanMega
        {
            get
            {
                foreach (var p in Pokemon.Owner.Pokemons)
                    if (p.SelectMega || p.Pokemon.Mega) return false;
                if (PTs.CanChangeForm(this, Ps.RAYQUAZA, 1))
                {
                    foreach (var m in Moves)
                        if (m.MoveE.Id == Ms.DRAGON_ASCENT) return true;
                    return false;
                }
                return PTs.CanChangeForm(this, ITs.MegaNumber(Pokemon.Item), ITs.MegaForm(Pokemon.Item));
            }
        }

        internal void Debut()
        {
            if (Action == PokemonAction.Debuting)
            {
                Tile.Debut();
                if (!(OnboardPokemon.HasCondition(Cs.Substitute) || AbilityE(As.OVERCOAT))) EHTs.Debut(this);
                if (!PTs.CheckFaint(this))
                {
                    if (ItemE(Is.BLUE_ORB) && PTs.CanChangeForm(this, Ps.KYOGRE, 1) || ItemE(Is.RED_ORB) && PTs.CanChangeForm(this, Ps.GROUNDON, 1)) PTs.ChangeForm(this, 1, true, Ls.Primal);
                    if (OnboardPokemon.Ability != As.FLOWER_GIFT && OnboardPokemon.Ability != As.FORECAST) AbilityAttach.Execute(this);
                    if (!ITs.AirBalloon(this)) ITs.Attach(this);
                }
                Action = PokemonAction.Done;
            }
        }
        internal void AttachBehaviors()
        {
            if (Action == PokemonAction.WillMove)
                Action = PokemonAction.MoveAttached;
        }
        internal void Switch()
        {
            if (Action == PokemonAction.WillSwitch)
            {
                STs.WillAct(this);
                Action = PokemonAction.Switching;
                Tile tile = Tile;
                if (Controller.Withdraw(this, "Withdraw", 0, true)) Controller.SendOut(tile);
                Action = PokemonAction.InBall;
            }
        }
        internal void Move()
        {
            STs.FocusPunch(Controller);
            LastMoveTurn = Controller.TurnNumber;
            STs.WillAct(this);
            switch (Action)
            {
                case PokemonAction.Stiff:
                    PTs.ShowLogPm(this, "Stiff");
                    Action = PokemonAction.Done;
                    break;
                case PokemonAction.Moving:
                    if (AtkContext.Move.Id == Ms.SKY_DROP)
                    {
                        CoordY = CoordY.Plate;
                        if (AtkContext.Target.Defender.AliveOnboard)
                        {
                            AtkContext.Target.Defender.CoordY = CoordY.Plate;
                            AtkContext.Target.Defender.OnboardPokemon.RemoveCondition(Cs.SkyDrop);
                        }
                        else AtkContext.SetTargets(Enumerable.Empty<DefContext>());
                    }
                    if (CanExecute())
                    {
                        if (AtkContext.Move.Id != Ms.BIDE) PTs.ShowLogPm(this, "UseMove", AtkContext.Move.Id);
                        AtkContext.ContinueExecute(SelectedTarget);
                    }
                    else Action = PokemonAction.Done;
                    break;
                case PokemonAction.MoveAttached:
                    {
                        var o = OnboardPokemon.GetCondition(Cs.Encore);
                        if (o != null)
                            foreach (var m in Moves)
                                if (m.MoveE == o.Move) SelectedMove = m;
                    }
                    ATs.StanceChange(this);
                    if (CanExecute() && SelectedMove.CanExecute())
                    {
                        _atkContext = null;
                        SelectedMove.Execute();
                        var o = OnboardPokemon.GetCondition(Cs.LastMove);
                        if (o == null)
                        {
                            o = new Condition();
                            o.Move = AtkContext.Move;
                            OnboardPokemon.SetCondition(Cs.LastMove, o);
                        }
                        else if (o.Move != AtkContext.Move)
                        {
                            o.Move = AtkContext.Move;
                            o.Int = 0;
                        }
                        if (AtkContext.Fail) o.Int = 0;
                        else o.Int++;
                        Controller.Board.SetCondition(Cs.LastMove, o);
                    }
                    else
                    {
                        OnboardPokemon.RemoveCondition(Cs.LastMove);
                        Action = PokemonAction.Done;
                    }
                    break;
            } //switch(Action)
        }

        /// <summary>
        /// 同一精灵的Outward在一段战报中可能出现多次，每次应是不同的Outward
        /// </summary>
        /// <returns></returns>
        internal PokemonOutward GetOutward()
        {
            Pokemon o = OnboardPokemon.GetCondition<Pokemon>(Cs.Illusion);
            var form = o == null ? OnboardPokemon.Form : o.Form;
            if (o == null) o = Pokemon;
            var name = o.Name;
            var gender = o.Gender;//即使对战画面中不显示性别，实际性别也与变身对象一致，可以被着迷。
            var lv = Pokemon.Lv;
            var shiny = o.Shiny;
            var position = new Position(Pokemon.TeamId, OnboardPokemon.X, OnboardPokemon.CoordY);
            var substitute = OnboardPokemon.HasCondition(Cs.Substitute);
            var hp = Pokemon.Hp;
            var state = State;
            var outward = new PokemonOutward(Id, Pokemon.TeamId, Pokemon.MaxHp);
            outward.SetAll(name, form, gender, lv, position, substitute, hp, state, shiny);
            return outward;
        }
        #endregion
    }
}
