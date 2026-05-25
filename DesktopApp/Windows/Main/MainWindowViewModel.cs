using System;
using System.Collections.Generic;
using Somni.Maki.Fundamentals;
using Somni.Maki.I18n;
using Somni.Maki.Pages.AttributeSetupPage;

namespace Somni.Maki.Windows.Main {
  public record MainMenuItem(string Name, string Icon, Func<ViewModelBase?> CreatePage);

  public class MainWindowViewModel : ViewModelBase {
    public MainWindowViewModel() {
      MainMenuItemSelected = MainMenuItems[0];
    }

    public List<MainMenuItem> MainMenuItems { get; } = [
      new(Strings.Main_Menu_Attribute, "IconAttr", () => new AttributeSetupPageViewModel()),
      new(Strings.Main_Menu_Verify, "IconVerify", () => null),
    ];

    public MainMenuItem MainMenuItemSelected { get; internal set; }

    public ViewModelBase? CurrentPage => MainMenuItemSelected.CreatePage.Invoke();
  }
}
