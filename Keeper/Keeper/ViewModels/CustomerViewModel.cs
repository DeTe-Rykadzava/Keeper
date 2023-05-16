using System;
using ReactiveUI;
using Avalonia.ReactiveUI;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using Avalonia.Reactive;
using System.Threading;
using System.Reactive;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using Avalonia.Collections;
using System.Reactive.Linq;
using Keeper.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reactive.Concurrency;


namespace Keeper.ViewModels;

public class CustomerViewModel : ViewModelBase
{
    private string _surname = string.Empty;
    private string _name = string.Empty;
    private string _patroinymic = string.Empty;
    private string? _phone = null;
    private string _email = string.Empty;
    private string? _organizaton = null;
    private string _note = string.Empty;
    private DateTime? _birthOFDate = null;
    private string _seriaPasport = string.Empty;
    private string _numberPasport = string.Empty;
    private string? _PhotoB64 = null;
    private Bitmap? _photoBit = null;

    public string Surname { get => _surname; set => this.RaiseAndSetIfChanged(ref _surname, value); }
    public string Name { get => _name; set => this.RaiseAndSetIfChanged(ref _name, value); }
    public string Patronymic { get => _patroinymic; set => this.RaiseAndSetIfChanged(ref _patroinymic, value); }
    public string? Phone { get => _phone; set => this.RaiseAndSetIfChanged(ref _phone, value); }
    public string Email { get => _email; set => this.RaiseAndSetIfChanged(ref _email, value); }
    public string? Organization { get => _organizaton; set => this.RaiseAndSetIfChanged(ref _organizaton, value); }
    public string Note { get => _note; set => this.RaiseAndSetIfChanged(ref _note, value); }
    public DateTime? BirthOfDate { get => _birthOFDate; set => this.RaiseAndSetIfChanged(ref _birthOFDate, value); }
    public string SeriaPasport { get => _seriaPasport; set => this.RaiseAndSetIfChanged(ref _seriaPasport, value); }
    public string NumberPasport { get => _numberPasport; set => this.RaiseAndSetIfChanged(ref _numberPasport, value); }
    public string? PhotoB64 { get => _PhotoB64; set => this.RaiseAndSetIfChanged(ref _PhotoB64, value); }
    public Bitmap? PhotoBit { get => _photoBit; set => this.RaiseAndSetIfChanged(ref _photoBit, value); }

    public DateTime? MinCustomerYear => DateTime.Now.AddYears(-16).AddDays(-1);

    public Interaction<Unit, PhotoResponseModel?> ShowPickImageDialog { get; }

    public ICommand ShowPickImageCommand { get; }

    private IObservable<bool> _isValidObserv;
    public IObservable<bool> IsValidObserv { get => _isValidObserv; set => this.RaiseAndSetIfChanged(ref _isValidObserv, value); }

    public bool IsValid = false;

    public CustomerViewModel()
    {
        ShowPickImageDialog = new Interaction<Unit, PhotoResponseModel?>();
        ShowPickImageCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await ShowPickImageDialog.Handle(new Unit());
            if (result == null)
                return;
            try
            {
                var imageStreamArr = result.PhotoFileStream.ToArray();
                PhotoB64 = Convert.ToBase64String(imageStreamArr);
                PhotoBit = Bitmap.DecodeToWidth(result.PhotoFileStream, 100);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message + "\n\n\n\n" + ex.InnerException);
            }
        });

        _isValidObserv = this.WhenAnyValue(
            x => x.Surname, x => x.Name, x => x.Patronymic,
            x => x.Email, x => x.Note, x => x.BirthOfDate,
            x => x.SeriaPasport, x => x.NumberPasport,
            (surname, name, patronymic, email, note, birthOfDate, seriaPas, numPas) =>
            !string.IsNullOrWhiteSpace(surname) && !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(patronymic) &&
            !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(note) && birthOfDate != null &&
            seriaPas.ToList().TrueForAll(c => char.IsDigit(c)) && numPas.ToList().TrueForAll(c => char.IsDigit(c)))
        .DistinctUntilChanged();

        this.WhenAnyObservable(x => x.IsValidObserv).Subscribe(s => { IsValid = s; });
    }
}
