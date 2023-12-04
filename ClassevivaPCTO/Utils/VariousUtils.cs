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

            Frame rootFrame = (Frame) Window.Current.Content;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack(); //ritorniamo alla pagina di login
            }
        }


        public static float CalcolaMedia(List<Grade> voti) //media ponderata
        {
            if (voti.Count == 0)
            {
                return float.NaN;
            }

            float somma = 0;
            float sommaPesi = 0;

            foreach (Grade voto in voti)
            {
                float? valoreDaSommare = GradeToFloat(voto);

                if (valoreDaSommare != null && voto.evtCode is GradeEventCode.GRV0 or GradeEventCode.GRV1 or GradeEventCode.GRV2)
                {
                    somma += (float) valoreDaSommare * (float) voto.weightFactor!;

                    sommaPesi += (float) voto.weightFactor;
                }
            }

            return somma / sommaPesi;
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

        public static (DateTime startDate, DateTime endDate) GetAgendaStartEndDates()
        {
            //var StartDate if i am on the first semester, then start date is 1st of september of the current year
            //else start date is 1st of september of the next year

            DateTime startDate = new DateTime(
                DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1,
                9,
                1
            );


            //var EndDate is max +366 days from the start date (this is an api limitation)
            DateTime endDate = startDate.AddDays(366);

            return (startDate, endDate);
        }

        public static (DateTime startDate, DateTime endDate) GetLessonsStartEndDates()
        {
            //var StartDate if i am on the first semester, then start date is 1st of september of the current year
            //else start date is 1st of september of the next year
            DateTime startDate = new DateTime(
                DateTime.Now.Month >= 9 ? DateTime.Now.Year : DateTime.Now.Year - 1,
                9,
                1
            );


            //var EndDate of next year + june 30th
            DateTime endDate = new DateTime(
                DateTime.Now.Month <= 8 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                6, 30);

            return (startDate, endDate);
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
                valoreFinale = (float) value;
            }


            return valoreFinale;
        }
    }
}