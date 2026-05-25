using System.Globalization;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Somni.Maki.I18n;
using Somni.Maki.Windows.Main;
using MainWindow = Somni.Maki.Windows.Main.MainWindow;

namespace Somni.Maki {
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
