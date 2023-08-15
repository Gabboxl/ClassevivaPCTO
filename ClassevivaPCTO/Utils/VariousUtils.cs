using ClassevivaPCTO.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;

namespace ClassevivaPCTO.Utils
{
    internal class VariousUtils
    {
        public static async void DoLogout()
        {
            var loginCredential = new CredUtils().GetCredentialFromLocker();

            if (loginCredential != null)
            {
                loginCredential
                    .RetrievePassword(); //dobbiamo per forza chiamare questo metodo per fare sì che la proprietà loginCredential.Password non sia vuota

                var vault = new Windows.Security.Credentials.PasswordVault();

                vault.Remove(
                    new Windows.Security.Credentials.PasswordCredential(
                        "classevivapcto",
                        loginCredential.UserName,
                        loginCredential.Password
                    )
                );

                //delete localsettings data in case of multiple account chosen
                if (await ChoiceSaverService.LoadChoiceIdentAsync() != null)
                {
                    ChoiceSaverService.RemoveSavedChoiceIdent();
                }
            }

            Frame rootFrame = (Frame)Window.Current.Content;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack(); //ritorniamo alla pagina di login
            }
        }


        public static float CalcolaMedia(List<Grade> voti)
        {
            float somma = 0;
            float numVoti = 0;

            foreach (Grade voto in voti)
            {
                float? valoreDaSommare = VariousUtils.GradeToFloat(voto);

                if (valoreDaSommare != null)
                {
                    somma += (float)valoreDaSommare;

                    numVoti++;
                }
            }

            return somma / numVoti;
        }

        public static string ToTitleCase(string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            s = s.ToLower();
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }


        public static string ToApiDateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyyMMdd");
        }

        public static float? GradeToFloat(object value)
        {
            float? valoreFinale = null;

            if (value is Grade grade)
            {
                if (grade.decimalValue != null)
                {
                    valoreFinale = grade.decimalValue;
                }
                else if (!grade.displayValue.ToLower().Equals("nv"))
                {
                    switch (grade.displayValue)
                    {
                        case "o":
                            valoreFinale = 10;
                            break;

                        case "ds":
                            valoreFinale = 9;
                            break;

                        case "b":
                            valoreFinale = 8;
                            break;

                        case "dc":
                            valoreFinale = 7;
                            break;

                        case "s":
                            valoreFinale = 6;
                            break;

                        case "i":
                            valoreFinale = 5;
                            break;
                    }
                }
            }
            else
            {
                valoreFinale = (float)value;
            }


            return valoreFinale;
        }
    }
}