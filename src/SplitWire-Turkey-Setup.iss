; SplitWire-Turkey Setup Script
; Inno Setup 6

#define MyAppName "SplitWire-Turkey"
#define MyAppVersion "2.1.0"
#define MyAppPublisher "SplitWire-Turkey"
#define MyAppURL "https://github.com/ctnkyaumt/SplitWire-Turkey"
#define MyAppExeName "SplitWire-Turkey.exe"

; Çoklu dil desteği
[Languages]
Name: "turkish"; MessagesFile: "compiler:Languages\Turkish.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[CustomMessages]
; Türkçe mesajlar (varsayılan)
turkish.WelcomeLabel2=Bu sihirbaz [name/ver] uygulamasını bilgisayarınıza kuracaktır.%n%n[name], Discord için DPI aşımı ve tünelleme çözümleri sunan bir araçtır.%n%nKuruluma devam etmek için İleri'ye tıklayın.
turkish.ClickInstall=Kuruluma başlamak için Kurulum'a tıklayın.
turkish.ClickNext=Devam etmek için İleri'ye tıklayın.
turkish.AdditionalIcons=Ek simgeler:
turkish.CreateDesktopShortcut=Masaüstünde kısayol oluştur
turkish.CreateQuickLaunchShortcut=Hızlı başlat çubuğunda kısayol oluştur
turkish.LaunchAfterInstall=Kurulum tamamlandıktan sonra SplitWire-Turkey'i çalıştır
turkish.InstallingDotNet=.NET 8.0 Runtime kuruluyor...
turkish.UninstallProgress=Kaldırma işlemi devam ediyor...

; İngilizce mesajlar
english.WelcomeLabel2=This wizard will install [name/ver] on your computer.%n%n[name] is a tool that provides DPI bypass and tunneling solutions for Discord.%n%nClick Next to continue with the installation.
english.ClickInstall=Click Install to begin the installation.
english.ClickNext=Click Next to continue.
english.AdditionalIcons=Additional icons:
english.CreateDesktopShortcut=Create a desktop shortcut
english.CreateQuickLaunchShortcut=Create a quick launch shortcut
english.LaunchAfterInstall=Launch SplitWire-Turkey after installation
english.InstallingDotNet=Installing .NET 8.0 Runtime...
english.UninstallProgress=Uninstall in progress...

; Rusça mesajlar
russian.WelcomeLabel2=Этот мастер установит [name/ver] на ваш компьютер.%n%n[name] - это инструмент, который предоставляет решения для обхода DPI и туннелирования для Discord.%n%nНажмите Далее, чтобы продолжить установку.
russian.ClickInstall=Нажмите Установить, чтобы начать установку.
russian.ClickNext=Нажмите Далее, чтобы продолжить.
russian.AdditionalIcons=Дополнительные значки:
russian.CreateDesktopShortcut=Создать ярлык на рабочем столе
russian.CreateQuickLaunchShortcut=Создать ярлык в панели быстрого запуска
russian.LaunchAfterInstall=Запустить SplitWire-Turkey после установки
russian.InstallingDotNet=Установка .NET 8.0 Runtime...
russian.UninstallProgress=Удаление выполняется...

; İspanyolca mesajlar
spanish.WelcomeLabel2=Este asistente instalará [name/ver] en su computadora.%n%n[name] es una herramienta que proporciona soluciones de bypass DPI y tunelización para Discord.%n%nHaga clic en Siguiente para continuar con la instalación.
spanish.ClickInstall=Haga clic en Instalar para comenzar la instalación.
spanish.ClickNext=Haga clic en Siguiente para continuar.
spanish.AdditionalIcons=Iconos adicionales:
spanish.CreateDesktopShortcut=Crear un acceso directo en el escritorio
spanish.CreateQuickLaunchShortcut=Crear un acceso directo en la barra de inicio rápido
spanish.LaunchAfterInstall=Ejecutar SplitWire-Turkey después de la instalación
spanish.InstallingDotNet=Instalando .NET 8.0 Runtime...
spanish.UninstallProgress=Desinstalación en progreso...


[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
AppId={{06b842bd-739c-4958-841e-b398791dfaf6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
UsePreviousAppDir=yes
DirExistsWarning=no
LicenseFile=
InfoBeforeFile=
InfoAfterFile=
OutputDir=C:\Users\cagri\Desktop
OutputBaseFilename=SplitWire-Turkey-Setup-Windows-{#MyAppVersion}
SetupIconFile=res\splitwire.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible x86
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequiredOverridesAllowed=commandline
; Dil ayarları
LanguageDetectionMethod=locale
ShowLanguageDialog=yes

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopShortcut}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchShortcut}"; GroupDescription: "{cm:AdditionalIcons}"; Check: not IsAdminInstallMode

[Files]
Source: "SplitWire-Turkey.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "SplitWire-Turkey.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "SplitWire-Turkey.runtimeconfig.json"; DestDir: "{app}"; Flags: ignoreversion
Source: "MaterialDesignThemes.Wpf.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "MaterialDesignColors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "Microsoft.Xaml.Behaviors.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "res\*"; DestDir: "{app}\res"; Flags: ignoreversion recursesubdirs
Source: "Prerequisites\VC_redist.x64.exe"; DestDir: "{app}\Prerequisites"; Flags: ignoreversion
Source: "Prerequisites\Windows.Packet.Filter.3.6.1.1.x64.msi"; DestDir: "{app}\Prerequisites"; Flags: ignoreversion
; .NET Framework Runtime Installers
Source: "Prerequisites\.NET 8.0\windowsdesktop-runtime-8.0.22-win-x64.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: Is64BitInstallMode
Source: "Prerequisites\.NET 8.0\windowsdesktop-runtime-8.0.22-win-x86.exe"; DestDir: "{tmp}"; Flags: deleteafterinstall; Check: not Is64BitInstallMode

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\SplitWire-Turkey'i Kaldır"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

[Registry]
Root: HKCU; Subkey: "SOFTWARE\SplitWire-Turkey"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "SOFTWARE\SplitWire-Turkey"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "SOFTWARE\SplitWire-Turkey"; ValueType: string; ValueName: "Language"; ValueData: "{code:GetLanguageCode}"; Flags: uninsdeletekey

[Run]
; Install .NET 8.0 Runtime if not present
Filename: "{tmp}\windowsdesktop-runtime-8.0.22-win-x64.exe"; Parameters: "/install /quiet /norestart"; StatusMsg: "{cm:InstallingDotNet}"; Flags: runhidden; Check: Is64BitInstallMode and not IsDotNetDetected
Filename: "{tmp}\windowsdesktop-runtime-8.0.22-win-x86.exe"; Parameters: "/install /quiet /norestart"; StatusMsg: "{cm:InstallingDotNet}"; Flags: runhidden; Check: not Is64BitInstallMode and not IsDotNetDetected
; Launch application after installation (with admin privileges)
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchAfterInstall}"; Flags: nowait postinstall skipifsilent runascurrentuser

[Code]
// Global değişkenler
var
  UninstallLanguage: String;

// Forward declarations
function IsServiceInstalled(ServiceName: String): Boolean; forward;
function StopAndRemoveService(ServiceName: String): Boolean; forward;
procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep); forward;

function IsDotNetDetected: Boolean;
var
  InstallPath: String;
  Key: String;
  Version: String;
  DisplayName: String;
  SubKeyNames: TArrayOfString;
  I: Integer;
begin
  Result := False;
  
  // Check for .NET 8.0 Desktop Runtime by enumerating uninstall keys
  Key := 'SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall';
  
  // Check in 64-bit registry
  if RegGetSubkeyNames(HKLM, Key, SubKeyNames) then
  begin
    for I := 0 to GetArrayLength(SubKeyNames) - 1 do
    begin
      if RegQueryStringValue(HKLM, Key + '\' + SubKeyNames[I], 'DisplayName', DisplayName) then
      begin
        // Check if it's .NET 8.0 Desktop Runtime
        if (Pos('Microsoft Windows Desktop Runtime - 8.0', DisplayName) > 0) or
           (Pos('Microsoft Windows Desktop Runtime 8.0', DisplayName) > 0) then
        begin
          if RegQueryStringValue(HKLM, Key + '\' + SubKeyNames[I], 'DisplayVersion', Version) then
          begin
            if Pos('8.0', Version) = 1 then
            begin
              Result := True;
              Exit;
            end;
          end;
        end;
      end;
    end;
  end;
  
  // Check in WOW6432Node (32-bit on 64-bit systems)
  if Is64BitInstallMode then
  begin
    Key := 'SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall';
    if RegGetSubkeyNames(HKLM, Key, SubKeyNames) then
    begin
      for I := 0 to GetArrayLength(SubKeyNames) - 1 do
      begin
        if RegQueryStringValue(HKLM, Key + '\' + SubKeyNames[I], 'DisplayName', DisplayName) then
        begin
          if (Pos('Microsoft Windows Desktop Runtime - 8.0', DisplayName) > 0) or
             (Pos('Microsoft Windows Desktop Runtime 8.0', DisplayName) > 0) then
          begin
            if RegQueryStringValue(HKLM, Key + '\' + SubKeyNames[I], 'DisplayVersion', Version) then
            begin
              if Pos('8.0', Version) = 1 then
              begin
                Result := True;
                Exit;
              end;
            end;
          end;
        end;
      end;
    end;
  end;
end;

function InitializeSetup(): Boolean;
begin
  Result := True;
  
  // Check if .NET 8.0 is installed
  if not IsDotNetDetected then
  begin
    // MsgBox('.NET 8.0 Desktop Runtime, SplitWire-Turkey için gereklidir.' + #13#10 + 
          // 'Kurulum programı şimdi .NET 8.0 Desktop Runtime''ı otomatik olarak kuracaktır.', 
          // mbInformation, MB_OK);
  end;
end;

function InitializeUninstall(): Boolean;
var
  LanguageCode: String;
begin
  Result := True;
  
  // Varsayılan dil
  UninstallLanguage := 'TR';
  
  // Registry'den dil kodunu oku
  if RegQueryStringValue(HKCU, 'SOFTWARE\SplitWire-Turkey', 'Language', LanguageCode) then
  begin
    UninstallLanguage := LanguageCode;
  end;
end;

function GetLanguageCode(Param: String): String;
begin
  // Seçilen dile göre dil kodunu döndür
  case ActiveLanguage of
    'turkish': Result := 'TR';
    'english': Result := 'EN';
    'russian': Result := 'RU';
    'spanish': Result := 'ES';
  else
    Result := 'TR'; // Varsayılan olarak Türkçe
  end;
end;

function GetUninstallLanguage(): String;
begin
  // Uninstaller için dil kodunu döndür
  Result := UninstallLanguage;
end;

function IsUpgradeInstall(): Boolean;
var
  ExistingInstallPath: String;
begin
  // A previous install left InstallPath in the registry (removed only on full uninstall,
  // see [Registry] Flags: uninsdeletekey). If present, this run is an upgrade over an
  // existing installation rather than a fresh install.
  Result := RegQueryStringValue(HKCU, 'SOFTWARE\SplitWire-Turkey', 'InstallPath', ExistingInstallPath);
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  LocalAppDataPath: String;
  SplitWireTurkeyPath: String;
begin
  if CurStep = ssInstall then
  begin
    // Only clean up localappdata\SplitWire-Turkey on a fresh install. On an upgrade this
    // folder holds user data (GoodbyeDPI/ByeDPI presets, logs, WebCord) that must survive
    // the update, so it is intentionally left untouched here.
    if IsUpgradeInstall() then
    begin
      Log('=== Mevcut kurulum tespit edildi (güncelleme), localappdata\SplitWire-Turkey korunuyor ===');
    end
    else
    begin
      Log('=== Kurulum öncesi temizlik işlemi başlatılıyor (ilk kurulum) ===');

      LocalAppDataPath := ExpandConstant('{localappdata}');
      SplitWireTurkeyPath := LocalAppDataPath + '\SplitWire-Turkey';

      if DirExists(SplitWireTurkeyPath) then
      begin
        Log('localappdata\SplitWire-Turkey klasörü bulundu, siliniyor...');
        try
          if DelTree(SplitWireTurkeyPath, True, True, True) then
          begin
            Log('localappdata\SplitWire-Turkey klasörü başarıyla silindi');
          end
          else
          begin
            Log('Uyarı: localappdata\SplitWire-Turkey klasörü tamamen silinemedi (bazı dosyalar kullanımda olabilir)');
                          // Continue with installation even if cleanup is incomplete
          end;
        except
          Log('HATA: localappdata\SplitWire-Turkey klasörü silinirken exception oluştu, kurulum devam ediyor');
        end;
      end
      else
      begin
        Log('localappdata\SplitWire-Turkey klasörü bulunamadı, temizlik gerekmiyor');
      end;

      Log('=== Kurulum öncesi temizlik işlemi tamamlandı ===');
    end;
  end;
end;



function IsServiceInstalled(ServiceName: String): Boolean;
var
  ResultCode: Integer;
begin
  Result := False;
  // UI'yi güncelle (işlem öncesi)
  try
    if Assigned(WizardForm) then
      Sleep(100);
  except
  end;
  // 5 saniye timeout ile hizmet sorgula
  if Exec('sc', 'query ' + ServiceName, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
  begin
    Result := (ResultCode = 0);
  end;
  // UI'yi güncelle (işlem sonrası)
  try
    if Assigned(WizardForm) then
    begin
      Sleep(100);
      Sleep(50);
    end;
  except
  end;
end;

function StopAndRemoveService(ServiceName: String): Boolean;
var
  ResultCode: Integer;
begin
  Result := False;
  
  try
    // UI'yi güncelle (işlem öncesi)
    try
      if Assigned(WizardForm) then
        Sleep(100);
    except
    end;
    // Hizmeti durdur (5 saniye timeout)
    if Exec('sc', 'stop ' + ServiceName, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
    begin
      // UI'yi güncelle
      try
        if Assigned(WizardForm) then
        begin
          Sleep(100);
          Sleep(100);
        end;
      except
      end;
      // Hizmeti kaldır (5 saniye timeout)
      if Exec('sc', 'delete ' + ServiceName, '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        Result := True;
      end;
      // UI'yi güncelle
      try
        if Assigned(WizardForm) then
        begin
          Sleep(100);
          Sleep(100);
        end;
      except
      end;
    end;
  except
    // Hata durumunda false döndür
    Result := False;
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  ResultCode: Integer;
  LocalAppDataPath: String;
  SplitWireTurkeyPath: String;
  ResetDNSScriptPath: String;
  WireSockUninstallPath: String;
  WireSockMSIPath: String;
  UninstallLogPath: String;
  AppPath: String;
begin
  if CurUninstallStep = usUninstall then
  begin
    // Kaldırma log dosyası yolu
    UninstallLogPath := ExpandConstant('{app}') + '\unins000.log';
    
    // İlerleme çubuğunu başlat ve status mesajını göster
    try
      if Assigned(WizardForm) then
      begin
        if Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 0;
          WizardForm.ProgressGauge.Style := npbstNormal;
          WizardForm.ProgressGauge.Max := 100;
        end;
        // Status mesajını göster
        if Assigned(WizardForm.StatusLabel) then
        begin
          WizardForm.StatusLabel.Caption := CustomMessage('UninstallProgress');
        end;
        Sleep(100);
        Sleep(50);
      end;
    except
      // Progress bar mevcut değilse devam et
    end;
    
    // Dil bilgisini log'a yaz
    Log('Uninstaller dili: ' + UninstallLanguage);
    
    // 1. Aşama: Hizmetleri kaldır (İlerleme: %20)
    Log('=== 1. AŞAMA: Hizmetler kaldırılıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 5;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // WireSock hizmetini kaldır
    if IsServiceInstalled('wiresock-client-service') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 7;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('wiresock-client-service') then
      begin
        Log('Uyarı: WireSock hizmeti kaldırılamadı');
      end
      else
      begin
        Log('WireSock hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('WireSock hizmeti zaten yüklü değil');
    end;

    // ByeDPI hizmetini kaldır
    if IsServiceInstalled('byedpi') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 9;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('byedpi') then
      begin
        Log('Uyarı: ByeDPI hizmeti kaldırılamadı');
      end
      else
      begin
        Log('ByeDPI hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('ByeDPI hizmeti zaten yüklü değil');
    end;

    // ProxiFyre hizmetini kaldır
    if IsServiceInstalled('ProxiFyreService') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 11;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('ProxiFyreService') then
      begin
        Log('Uyarı: ProxiFyre hizmeti kaldırılamadı');
      end
      else
      begin
        Log('ProxiFyre hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('ProxiFyre hizmeti zaten yüklü değil');
    end;

    // WinWS1 hizmetini kaldır
    if IsServiceInstalled('winws1') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 13;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('winws1') then
      begin
        Log('Uyarı: WinWS1 hizmeti kaldırılamadı');
      end
      else
      begin
        Log('WinWS1 hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('WinWS1 hizmeti zaten yüklü değil');
    end;

    // WinWS2 hizmetini kaldır
    if IsServiceInstalled('winws2') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 15;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('winws2') then
      begin
        Log('Uyarı: WinWS2 hizmeti kaldırılamadı');
      end
      else
      begin
        Log('WinWS2 hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('WinWS2 hizmeti zaten yüklü değil');
    end;

    // Zapret hizmetini kaldır
    if IsServiceInstalled('zapret') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 17;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('zapret') then
      begin
        Log('Uyarı: Zapret hizmeti kaldırılamadı');
      end
      else
      begin
        Log('Zapret hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('Zapret hizmeti zaten yüklü değil');
    end;

    // GoodbyeDPI hizmetini kaldır
    if IsServiceInstalled('GoodbyeDPI') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 19;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('GoodbyeDPI') then
      begin
        Log('Uyarı: GoodbyeDPI hizmeti kaldırılamadı');
      end
      else
      begin
        Log('GoodbyeDPI hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('GoodbyeDPI hizmeti zaten yüklü değil');
    end;

    // WinDivert hizmetini kaldır (en son)
    if IsServiceInstalled('WinDivert') then
    begin
      try
        if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
        begin
          WizardForm.ProgressGauge.Position := 20;
          Sleep(100);
        Sleep(50);
        end;
      except
      end;
      if not StopAndRemoveService('WinDivert') then
      begin
        Log('Uyarı: WinDivert hizmeti kaldırılamadı');
      end
      else
      begin
        Log('WinDivert hizmeti başarıyla kaldırıldı');
      end;
    end
    else
    begin
      Log('WinDivert hizmeti zaten yüklü değil');
    end;

    // Windows Firewall kurallarını kaldır
    Log('Windows Firewall kuralları kaldırılıyor...');
    try
      Exec('netsh', 'advfirewall firewall delete rule name="ProxiFyre Inbound"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Exec('netsh', 'advfirewall firewall delete rule name="ProxiFyre Outbound"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Exec('netsh', 'advfirewall firewall delete rule name="ByeDPI ciadpi Inbound"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Exec('netsh', 'advfirewall firewall delete rule name="ByeDPI ciadpi Outbound"', '', SW_HIDE, ewWaitUntilTerminated, ResultCode);
      Log('Windows Firewall kuralları başarıyla kaldırıldı');
    except
      Log('UYARI: Windows Firewall kuralları kaldırılamadı');
    end;

    Log('1. Aşama tamamlandı - İlerleme: %20');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 20;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 1.5. Aşama: WebCord.exe sürecini sonlandır
    Log('=== 1.5. AŞAMA: WebCord.exe süreçleri sonlandırılıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 25;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    try
      if Exec('taskkill', '/F /IM webcord.exe /T', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
      begin
        if ResultCode = 0 then
        begin
          Log('WebCord.exe süreçleri başarıyla sonlandırıldı');
        end
        else
        begin
          Log('WebCord.exe süreçleri bulunamadı veya zaten sonlandırılmış (kod: ' + IntToStr(ResultCode) + ')');
        end;
      end
      else
      begin
        Log('Uyarı: WebCord.exe süreçleri sonlandırılamadı');
      end;
    except
      Log('HATA: WebCord.exe süreçleri sonlandırma sırasında exception oluştu');
    end;
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    
    // Kısa bir bekleme süresi ekle (süreçlerin tamamen sonlanması için)
    Sleep(1000);
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    
    Log('1.5. Aşama tamamlandı - İlerleme: %30');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 30;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 2. Aşama: %localappdata%/SplitWire-Turkey klasörünü sil (İlerleme: %40)
    Log('=== 2. AŞAMA: SplitWire-Turkey klasörü siliniyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 35;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    LocalAppDataPath := ExpandConstant('{localappdata}');
    SplitWireTurkeyPath := LocalAppDataPath + '\SplitWire-Turkey';
    
    if DirExists(SplitWireTurkeyPath) then
    begin
      if DelTree(SplitWireTurkeyPath, True, True, True) then
      begin
        Log('SplitWire-Turkey klasörü başarıyla silindi');
      end
      else
      begin
        Log('Uyarı: SplitWire-Turkey klasörü silinemedi');
      end;
    end
    else
    begin
      Log('SplitWire-Turkey klasörü bulunamadı');
    end;

    Log('2. Aşama tamamlandı - İlerleme: %40');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 40;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 2.5. Aşama: WebCord masaüstü kısayolunu kaldır
    Log('=== 2.5. AŞAMA: WebCord masaüstü kısayolu kaldırılıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 42;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // Kullanıcı masaüstü klasörü
    if FileExists(ExpandConstant('{userdesktop}\WebCord.lnk')) then
    begin
      if DeleteFile(ExpandConstant('{userdesktop}\WebCord.lnk')) then
      begin
        Log('WebCord.lnk kullanıcı masaüstünden başarıyla kaldırıldı');
      end
      else
      begin
        Log('Uyarı: WebCord.lnk kullanıcı masaüstünden kaldırılamadı');
      end;
    end
    else
    begin
      Log('WebCord.lnk kullanıcı masaüstünde bulunamadı');
    end;
    
    // Ortak masaüstü klasörü
    if FileExists(ExpandConstant('{commondesktop}\WebCord.lnk')) then
    begin
      if DeleteFile(ExpandConstant('{commondesktop}\WebCord.lnk')) then
      begin
        Log('WebCord.lnk ortak masaüstünden başarıyla kaldırıldı');
      end
      else
      begin
        Log('Uyarı: WebCord.lnk ortak masaüstünden kaldırılamadı');
      end;
    end
    else
    begin
      Log('WebCord.lnk ortak masaüstünde bulunamadı');
    end;
    
    Log('2.5. Aşama tamamlandı - İlerleme: %45');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 45;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 3. Aşama: reset_dns_settings.bat çalıştır (İlerleme: %60)
    Log('=== 3. AŞAMA: DNS ayarları sıfırlanıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 50;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    ResetDNSScriptPath := ExpandConstant('{app}') + '\res\reset_dns_settings.bat';
    if FileExists(ResetDNSScriptPath) then
    begin
      try
        if Exec(ResetDNSScriptPath, '', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
        begin
          Log('DNS ayarları başarıyla sıfırlandı');
        end
        else
        begin
          Log('Uyarı: DNS sıfırlama scripti çalıştırılamadı');
        end;
      except
        Log('HATA: DNS sıfırlama scripti çalıştırma sırasında exception oluştu');
      end;
    end
    else
    begin
      Log('Uyarı: DNS sıfırlama scripti bulunamadı');
    end;

    Log('3. Aşama tamamlandı - İlerleme: %60');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 60;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 4. Aşama: WireSock 2.4.23.1'i sessiz kaldır (İlerleme: %80)
    Log('=== 4. AŞAMA: WireSock 2.4.23.1 kaldırılıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 70;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    WireSockUninstallPath := ExpandConstant('{app}') + '\res\wiresock-secure-connect-x64-2.4.23.1.exe';
    
    if FileExists(WireSockUninstallPath) then
    begin
      Log('WireSock 2.4.23.1 kaldırılıyor...');
      try
        if Exec(WireSockUninstallPath, '/uninstall /S', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
        begin
          Log('WireSock 2.4.23.1 başarıyla kaldırıldı');
        end
        else
        begin
          Log('Uyarı: WireSock 2.4.23.1 kaldırılamadı');
        end;
      except
        Log('HATA: WireSock 2.4.23.1 kaldırma işlemi sırasında exception oluştu');
      end;
    end
    else
    begin
      Log('WireSock 2.4.23.1 kaldırma dosyası bulunamadı');
    end;

    Log('4. Aşama tamamlandı - İlerleme: %80');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 80;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 5. Aşama: WireSock 1.4.7.1'i sessiz kaldır (İlerleme: %100)
    Log('=== 5. AŞAMA: WireSock 1.4.7.1 kaldırılıyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 85;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    WireSockMSIPath := ExpandConstant('{app}') + '\res\wiresock-vpn-client-x64-1.4.7.1.msi';
    
    if FileExists(WireSockMSIPath) then
    begin
      Log('WireSock 1.4.7.1 kaldırılıyor...');
      try
        if Exec('msiexec', '/x "' + WireSockMSIPath + '" /quiet /norestart', '', SW_HIDE, ewWaitUntilTerminated, ResultCode) then
        begin
          Log('WireSock 1.4.7.1 başarıyla kaldırıldı');
        end
        else
        begin
          Log('Uyarı: WireSock 1.4.7.1 kaldırılamadı');
        end;
      except
        Log('HATA: WireSock 1.4.7.1 kaldırma işlemi sırasında exception oluştu');
      end;
    end
    else
    begin
      Log('WireSock 1.4.7.1 MSI dosyası bulunamadı');
    end;

    Log('5. Aşama tamamlandı - İlerleme: %100');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 90;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    // 6. Aşama: Program Files klasörünü tamamen sil
    Log('=== 6. AŞAMA: Program Files klasörü siliniyor ===');
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 95;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    
    AppPath := ExpandConstant('{app}');
    
    if DirExists(AppPath) then
    begin
      Log('Program Files\SplitWire-Turkey klasörü siliniyor...');
      try
        if DelTree(AppPath, True, True, True) then
        begin
          Log('Program Files\SplitWire-Turkey klasörü başarıyla silindi');
        end
        else
        begin
          Log('Uyarı: Program Files\SplitWire-Turkey klasörü tamamen silinemedi (bazı dosyalar kullanımda olabilir)');
        end;
      except
        Log('HATA: Program Files\SplitWire-Turkey klasörü silinirken exception oluştu');
      end;
    end
    else
    begin
      Log('Program Files\SplitWire-Turkey klasörü bulunamadı');
    end;
    
    Log('6. Aşama tamamlandı - İlerleme: %100');
    try
      if Assigned(WizardForm) then
        Sleep(100);
        Sleep(50);
    except
    end;
    try
      if Assigned(WizardForm) and Assigned(WizardForm.ProgressGauge) then
      begin
        WizardForm.ProgressGauge.Position := 100;
        Sleep(100);
        Sleep(50);
      end;
    except
    end;
    Log('Tüm kaldırma aşamaları tamamlandı. SplitWire-Turkey kaldırılıyor...');
  end;
end;

 

