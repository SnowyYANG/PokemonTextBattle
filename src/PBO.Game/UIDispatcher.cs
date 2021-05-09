using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline
{
  /// <summary>
  /// this should not be used as a dispatcher
  /// </summary>
  public static class UIDispatcher
  {
    public static void Invoke(Action action)
    {
#if DEBUG
      try
      {
#endif
            action?.Invoke();
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
    public static void Invoke(Delegate method, params object[] args)
    {
#if DEBUG
      try
      {
#endif
        method?.DynamicInvoke(args);
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
    public static void BeginInvoke(Delegate method, params object[] args)
    {
#if DEBUG
      try
      {
#endif
        method?.DynamicInvoke(args);
#if DEBUG
      }
      catch
      {
        System.Diagnostics.Debugger.Break();
      }
#endif
    }
  }
}
