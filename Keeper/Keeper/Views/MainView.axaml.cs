using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;
using Keeper.ViewModels;
using Avalonia.ReactiveUI;
using System.Reactive;
using System;
using Avalonia.VisualTree;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.LogicalTree;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Keeper.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
    // private async Task ShowFileDialog(InteractionContext<Unit,Unit> interaction)
    // {
    //     ContentControl control = this;
    //     try
    //     {
    //         var result = await ShowOpenFileDialogAsync(control, new FilePickerOpenOptions(){ AllowMultiple = false});
    //         System.Console.WriteLine(result.GetType());
    //     }
    //     catch(Exception ex)
    //     {
    //         System.Console.WriteLine(ex.Message);
    //     }
    // }

    // public async Task<IReadOnlyList<IStorageFile>> ShowOpenFileDialogAsync(ContentControl? owner, FilePickerOpenOptions options)
    // {
    //     if (owner == null) { throw new ArgumentNullException(nameof(owner)); }
    //     var result = await GetStorage(owner).OpenFilePickerAsync(options).ConfigureAwait(true);
    //     return result.ToList();
    // }

    // private static IStorageProvider GetStorage(ContentControl owner) => TopLevel.GetTopLevel(owner)?.StorageProvider ?? 
    //                                                                     throw new ArgumentException("Cannot find StorageProvider for specified dialog owner.", nameof(owner));
}