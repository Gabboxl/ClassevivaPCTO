﻿using System;
using Windows.UI.Xaml.Controls;

// Il modello di elemento Pagina vuota è documentato all'indirizzo https://go.microsoft.com/fwlink/?LinkId=234238

namespace ClassevivaPCTO.Views
{
    /// <summary>
    /// Pagina vuota che può essere usata autonomamente oppure per l'esplorazione all'interno di un frame.
    /// </summary>
    public sealed partial class Agenda : Page
    {
        public Agenda()
        {
            this.InitializeComponent();


            //imposto la data di oggi del picker
            CalendarAgenda.Date = DateTime.Now;
        }
    }
}