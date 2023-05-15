using ClassevivaPCTO.Utils;
using ClassevivaPCTO.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace ClassevivaPCTO.Dialogs
{
    public sealed partial class NoteDialogContent : Page
    {
        Note CurrentNote;
        ReadNoteResult CurrentReadResult;

        private readonly IClassevivaAPI apiWrapper;


        public NoteDialogContent(Note note, ReadNoteResult readNoteResult)
        {
            this.InitializeComponent();

            CurrentNote = note;
            CurrentReadResult = readNoteResult;
            
            App app = (App)App.Current;
            var apiClient = app.Container.GetService<IClassevivaAPI>();

            apiWrapper = PoliciesDispatchProxy<IClassevivaAPI>.CreateProxy(apiClient);
        }
        
        
    }
}
