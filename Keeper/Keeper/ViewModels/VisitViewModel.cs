using System;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.ReactiveUI;
using System.Windows.Input;
using System.Reactive;
using Keeper.Models;
using System.Reactive.Linq;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive.Concurrency;
using System.Text.Json;

namespace Keeper.ViewModels;
public class VisitViewModel : ViewModelBase
{
    private string _mode;
    private HttpClient _client;

    private string _purpose = string.Empty;
    private DateTime? _validityAppBegin;
    private DateTime? _validityAppEnd;
    private string? _pasportB64;


    public string Purpose { get => _purpose; set => this.RaiseAndSetIfChanged(ref _purpose, value); }
    public DateTime? ValidityApplicationBegin { get => _validityAppBegin; set => this.RaiseAndSetIfChanged(ref _validityAppBegin, value); }
    public DateTime? ValidityApplicationEnd { get => _validityAppEnd; set => this.RaiseAndSetIfChanged(ref _validityAppEnd, value); }
    public string? PasportB64 { get => _pasportB64; set => this.RaiseAndSetIfChanged(ref _pasportB64, value); }

    public DateTime MinBeginDate => DateTime.Now.AddDays(1);
    public DateTime MaxBeginDate => DateTime.Now.AddDays(15);

    private DateTime? _minEndDate = DateTime.Now.AddDays(1);
    private DateTime? _maxEndDate = DateTime.Now.AddDays(15);
    public DateTime? MinEndDate { get => _minEndDate; set => this.RaiseAndSetIfChanged(ref _minEndDate, value);}
    public DateTime? MaxEndDate { get => _maxEndDate; set => this.RaiseAndSetIfChanged(ref _maxEndDate, value); }

    private SubdivisionModel? _selectedSubdivision;
    public SubdivisionModel? SelectedSubdivision { get => _selectedSubdivision; set => this.RaiseAndSetIfChanged(ref _selectedSubdivision, value); }

    public ObservableCollection<SubdivisionModel> Subdivisions { get; } = new();
    public ObservableCollection<CustomerViewModel> Customers { get; } = new() { new CustomerViewModel() };

    private CustomerViewModel _selectedCustomer;
    public CustomerViewModel SelectedCustomer { get => _selectedCustomer; set => this.RaiseAndSetIfChanged(ref _selectedCustomer, value); }


    public Interaction<Unit, string?> ShowPasportPickDialog { get; }

    public ICommand ResetFormCommand { get; }
    public ReactiveCommand<int?, Unit> SendFormCommand { get; }
    public ICommand PickPasportCommand { get; }

    public ICommand AddCustomerCommand { get; }

    private IObservable<bool> _canSendForm;
    public IObservable<bool> CanSendForm { get => _canSendForm; set => this.RaiseAndSetIfChanged(ref _canSendForm, value); }

    private bool _customersIsValid = false;
    public bool CustomersIsValid { get => _customersIsValid; set => this.RaiseAndSetIfChanged(ref _customersIsValid, value); }

    public VisitViewModel(string mode, HttpClient client)
    {
        _mode = mode;
        _client = client;
        _selectedCustomer = Customers[0];

        RxApp.MainThreadScheduler.Schedule(LoadSubdivisions);

        this.WhenAnyObservable(x => x.SelectedCustomer.IsValidObserv).Subscribe(s => CheckValidCustomers());
        _canSendForm = this.WhenAnyValue(
                        x => x.Purpose, x => x.ValidityApplicationBegin, x => x.ValidityApplicationEnd,
                        x => x.SelectedSubdivision, x => x.CustomersIsValid, x => x.PasportB64,
                        (purpose, beginDate, endDate, subdivision, customersValid, pasport) =>
                        !string.IsNullOrWhiteSpace(purpose) && customersValid && beginDate != null && endDate != null &&
                        subdivision != null && pasport != null
        )
        .DistinctUntilChanged();

        ShowPasportPickDialog = new Interaction<Unit, string?>();

        ResetFormCommand = ReactiveCommand.Create(() =>
        {
            Purpose = string.Empty;
            ValidityApplicationBegin = null;
            ValidityApplicationEnd = null;
            PasportB64 = null;
            SelectedSubdivision = null;
            Customers.Clear();
            Customers.Add(new CustomerViewModel());
            SelectedCustomer = Customers[0];
        });

        SendFormCommand = ReactiveCommand.CreateFromTask(async (int? UserID) =>
        {
            var customers = Customers.Select(s => new ApplicationRequestModel.RequestCustomers()
            {
                Surname = s.Surname,
                Name = s.Name,
                Patronymic = s.Patronymic,
                Phone = s.Phone,
                Email = s.Email,
                Organization = s.Organization,
                Note = s.Note,
                BirthOfDate = new DateOnly(s.BirthOfDate!.Value.Year, s.BirthOfDate!.Value.Month, s.BirthOfDate!.Value.Day),
                NumberPasport = s.NumberPasport,
                SeriaPasport = s.SeriaPasport,
                PhotoB64 = s.PhotoB64
            }).ToList();

            var application = new ApplicationRequestModel()
            {
                PasportB64 = PasportB64!,
                Customers = customers,
                Purpose = Purpose!,
                SubdivisionName = SelectedSubdivision!.subdivisionName,
                UserId = UserID,
                ValidityApplicationBegin = new DateOnly(ValidityApplicationBegin!.Value.Year, ValidityApplicationBegin!.Value.Month, ValidityApplicationBegin!.Value.Day),
                ValidityApplicationEnd = new DateOnly(ValidityApplicationEnd!.Value.Year, ValidityApplicationEnd!.Value.Month, ValidityApplicationEnd!.Value.Day)
            };

            var requestContent = new StringContent(JsonSerializer.Serialize<ApplicationRequestModel>(application));
            requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("https://localhost:7125/api/Application"),
                Method = HttpMethod.Post,
                Content = requestContent
            };

            try
            {
                var result = await _client.SendAsync(request);

                if (!result.IsSuccessStatusCode)
                {
                    System.Console.WriteLine("Не удалось отправить данные на сервер");
                    return;
                }

                ResetFormCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Error: {ex.Message}\n\n{ex.InnerException}");
            }

        }, CanSendForm);

        PickPasportCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ShowPasportPickDialog.Handle(new Unit());

            if (result is null)
                return;

            PasportB64 = result;
        });

        AddCustomerCommand = ReactiveCommand.Create(() =>
        {
            var newCustomer = new CustomerViewModel();
            Customers.Add(newCustomer);
            SelectedCustomer = newCustomer;
        });

        this.WhenAnyValue(x => x.ValidityApplicationBegin).Subscribe(s => { if( s != null ) { MaxEndDate = s.Value.AddDays(15); MinEndDate = s.Value.AddDays(1);  } });
    }

    private void CheckValidCustomers()
    {
        bool result = true;
        if (_mode == "group" && Customers.Count < 5)
        {
            CustomersIsValid = false;
            return;
        }
        foreach (var customer in Customers)
        {
            bool valid = customer.IsValid;
            if (valid != result)
            {
                result = false;
                break;
            }
        }
        CustomersIsValid = result;
    }

    private async void LoadSubdivisions()
    {
        var subdivisions = await SubdivisionModel.GetSubdivisions(_client);

        if (subdivisions is null)
            return;

        foreach (var subdivision in subdivisions)
            Subdivisions.Add(subdivision);

    }
}
