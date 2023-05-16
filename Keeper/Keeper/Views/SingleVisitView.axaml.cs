using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Reactive;
using Avalonia.ReactiveUI;
using System;
using System.Reactive;
using Keeper.ViewModels;
using ReactiveUI;
using System.Threading.Tasks;
using System.IO;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;
using Keeper.Models;

namespace Keeper.Views;

public partial class SingleVisitView : ReactiveUserControl<MainViewModel>
{
    public SingleVisitView()
    {
        InitializeComponent();
        this.WhenActivated(d => d(ViewModel!.CurrentVisitModel!.SelectedCustomer.ShowPickImageDialog.RegisterHandler(ShowImageDialog)));
        this.WhenActivated(d => d(ViewModel!.CurrentVisitModel!.ShowPasportPickDialog.RegisterHandler(ShowPasportDialog)));
    }

    private async Task ShowPasportDialog(InteractionContext<Unit, string?> context)
    {
        try
        {
            var pickerOpt = new FilePickerOpenOptions();
            pickerOpt.AllowMultiple = false;
            pickerOpt.Title = "SelectPhoto";
            pickerOpt.FileTypeFilter = new List<FilePickerFileType>() { new FilePickerFileType("Image Files (png, jpg)")
            { MimeTypes = new List<string>() { "application/pdf" } } };

            var result = await ShowOpenFileDialog(this, pickerOpt);
            if(result == null) { context.SetOutput(null); return; }

            var stream = await result[0].OpenReadAsync().ConfigureAwait(true);

            var pdfMemoryStream = new MemoryStream();

            await stream.CopyToAsync(pdfMemoryStream);

            var pasportB64 = Convert.ToBase64String(pdfMemoryStream.ToArray());

            context.SetOutput(pasportB64);
        }
        catch (System.Exception ex)
        {
            context.SetOutput(null);
            System.Console.WriteLine($"Error: {ex.Message}\n\n{ex.InnerException}");
            return;
        }
    }

    private async Task ShowImageDialog(InteractionContext<Unit, PhotoResponseModel?> context)
    {
        try
        {
            // настройки окна выбора
            var pickerOpt = new FilePickerOpenOptions();
            pickerOpt.AllowMultiple = false;
            pickerOpt.Title = "SelectPhoto";
            pickerOpt.FileTypeFilter = new List<FilePickerFileType>() { new FilePickerFileType("Image Files (png, jpg)")
            { MimeTypes = new List<string>() { "image/png", "image/jpeg" } } };

            var result = await ShowOpenFileDialog(this, pickerOpt);
            if (result == null)
            { context.SetOutput(null); return; }
            var stream = await result[0].OpenReadAsync().ConfigureAwait(true);

            var imageMemoryStream = new MemoryStream();
            await stream.CopyToAsync(imageMemoryStream);

            imageMemoryStream.Position = 0;

            context.SetOutput(new PhotoResponseModel(imageMemoryStream));
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Error: {ex.Message}\n\n{ex.InnerException}");
            context.SetOutput(null);
        }
    }

    private async Task<IReadOnlyList<IStorageFile>> ShowOpenFileDialog(ContentControl owner, FilePickerOpenOptions options)
    {
        var result = await GetStorage(owner).OpenFilePickerAsync(options).ConfigureAwait(true);
        return result.ToList();
    }

    private IStorageProvider GetStorage(ContentControl owner) => TopLevel.GetTopLevel(owner)?.StorageProvider ??
    throw new ArgumentException("Не возможно получить провайдера файловой системы", nameof(owner));
}