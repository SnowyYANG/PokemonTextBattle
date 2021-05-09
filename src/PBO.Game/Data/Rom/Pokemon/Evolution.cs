using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace PokemonBattleOnline.Game
{
  public class Evolution
  {
    internal Evolution(int from, int to)
    {
      _from = from;
      _to = to;
    }
    
    private readonly int _from;  
    public int From
    { get { return _from; } }
    
    private readonly int _to;  
    public int To
    { get { return _to; } }
    
    public bool NeedLvUp
    { get { return false; } }
  }
}
