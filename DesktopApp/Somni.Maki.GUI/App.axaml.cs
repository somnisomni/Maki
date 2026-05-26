using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Somni.Maki.GUI.I18n;
using Somni.Maki.GUI.Windows.Main;
using MainWindow = Somni.Maki.GUI.Windows.Main.MainWindow;

namespace Somni.Maki.GUI {
  public class App : Application {
    public override void Initialize() {
      AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted() {
      Strings.Culture = CultureInfo.CurrentUICulture;

      if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
        desktop.MainWindow = new MainWindow {
          DataContext = new MainWindowViewModel(),
        };
      }

      base.OnFrameworkInitializationCompleted();
    }
  }
}
