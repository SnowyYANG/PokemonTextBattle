using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace PokemonBattleOnline.PBO.Elements
{
  public static class Labels
  {
    public static readonly DataTemplate Ability;
    public static readonly DataTemplate Item;
    public static readonly DataTemplate PokemonState;
    public static readonly DataTemplate Gender;

    static Labels()
    {
      Ability = GetDataTemplate("Ability");
      Item = GetDataTemplate("Item");
      PokemonState = GetDataTemplate("PokemonState");
      Gender = GetDataTemplate("Gender");
    }

    static ResourceDictionary GetDictionary(string filename)
    {
      return Helper.GetDictionary("Elements/Labels", filename);
    }
    static DataTemplate GetDataTemplate(string filename)
    {
      return GetDictionary(filename)[filename + "Label"] as DataTemplate;
    }
  }
}
