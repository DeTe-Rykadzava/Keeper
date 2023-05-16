using Avalonia;
using Avalonia.Controls;
using ReactiveUI;
using Keeper.Views;
using System;
using System.Windows.Input;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Data;
using System.Text.Json;


namespace Keeper.ViewModels;

public class MainViewModel : ViewModelBase
{
    private HttpClient client = new HttpClient();
    private int? _userID;
    private UserControl _currentWebPage;
    private UserControl _currentView;
    private bool _isSignIn = true;

    private string _login = string.Empty;
    public string Login
    {
        get { return _login; }
        set { this.RaiseAndSetIfChanged(ref _login , value); }
    }
    private string _password = string.Empty;
    public string Password
    {
        get { return _password; }
        set { this.RaiseAndSetIfChanged( ref _password , value); }
    }
    public int? UserID { get => _userID; set => this.RaiseAndSetIfChanged( ref _userID , value); }

    public bool IsSignIn { get => _isSignIn; set => this.RaiseAndSetIfChanged(ref _isSignIn, value); }

    public string Platform => Environment.OSVersion.Platform.ToString();

    public ICommand ShowSingCommand { get; }
    public ICommand ShowMainCommand { get; }
    public ICommand SignInCommand { get; }
    public ICommand SignUpCommand { get; }
    public ICommand SignOutCommand { get; }
    public ICommand ShowSingleCreateApplicaitonCommand { get; }
    public ICommand ShowGroupCreateApplicaitonCommand { get; }
    public ICommand ClearSingInput { get ;}

    public ReactiveCommand<int, Unit> SwapSingState { get; }

    public UserControl CurrentView { get => _currentView; set => this.RaiseAndSetIfChanged(ref _currentView, value); }

    public UserControl CurrentWebPage { get => _currentWebPage; set => this.RaiseAndSetIfChanged(ref _currentWebPage, value); }

    private VisitViewModel? _currentVisitModel;

    public VisitViewModel? CurrentVisitModel { get => _currentVisitModel; set => this.RaiseAndSetIfChanged(ref _currentVisitModel, value); }

    public MainViewModel()
    {
        _currentView = new AuthView() {DataContext = this};
    }
    public MainViewModel(UserControl mainView)
    {
        _currentView = mainView;
        _currentWebPage = new WebAddApplications();
        
        ShowSingCommand = ReactiveCommand.Create(() => { CurrentWebPage = new WebAuthRegistration(); });

        ShowMainCommand = ReactiveCommand.Create(() => { CurrentWebPage = new WebAddApplications(); });

        SwapSingState = ReactiveCommand.Create((int StateID) => 
        { 
            switch (StateID) 
            {
                case 0:
                    IsSignIn = true;
                    break; 
                case 1:
                    IsSignIn = false;
                    break;
            }
            return new Unit(); 
        });

        ClearSingInput = ReactiveCommand.Create(() => { Login = string.Empty; Password = string.Empty; });

        SignInCommand = ReactiveCommand.CreateFromTask(async () => 
        {

            if(string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
                return;

            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri( $"https://localhost:7125/api/User/{Login}&{Password}"),
                    Method = HttpMethod.Post,
                    Content = null
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                var result = await client.SendAsync(request);

                if(!result.IsSuccessStatusCode)
                    return;

                UserID = int.Parse(await result.Content.ReadAsStringAsync());

                ClearSingInput.Execute(null);

                ShowMainCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message + "\n\n\n\n" + ex.InnerException);
            }
            
        });

        SignUpCommand = ReactiveCommand.CreateFromTask(async ()=> 
        {
             if(string.IsNullOrWhiteSpace(Login) || string.IsNullOrWhiteSpace(Password))
                return;

            try
            {
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri( $"https://localhost:7125/api/User/"),
                    Method = HttpMethod.Post,
                    Content = new StringContent(JsonSerializer.Serialize(new RegistrationModel(Login,Password)))
                };

                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));

                var result = await client.SendAsync(request);

                if(!result.IsSuccessStatusCode)
                    return;

                UserID = int.Parse(await result.Content.ReadAsStringAsync());

                ClearSingInput.Execute(null);

                ShowMainCommand.Execute(null);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message + "\n\n\n\n" + ex.InnerException);
            }

        });

        SignOutCommand = ReactiveCommand.Create(() => { UserID = null; });

        ShowSingleCreateApplicaitonCommand = ReactiveCommand.Create(() => { ShowApplicationFromMode("single"); });

        ShowGroupCreateApplicaitonCommand = ReactiveCommand.Create(() => { ShowApplicationFromMode("group"); });
    }

    private void ShowApplicationFromMode(string mode)
    {
        CurrentVisitModel = new VisitViewModel(mode, client);
        switch (mode)
        {
            case "single":
                CurrentWebPage = new SingleVisitView();
                break;
            case "group":
                CurrentWebPage = new GroupVisitView();
                break;
        }
        
    }

    public record RegistrationModel(string Login, string Password);

}