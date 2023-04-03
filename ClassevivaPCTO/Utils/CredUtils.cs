using System;
using System.Collections.Generic;
using Windows.Security.Credentials;

namespace ClassevivaPCTO.Utils
{
    internal class CredUtils
    {

        public PasswordCredential GetCredentialFromLocker()
        {
            PasswordCredential credential = null;

            var vault = new Windows.Security.Credentials.PasswordVault();

            IReadOnlyList<PasswordCredential> credentialList = null;

            try
            {
                credentialList = vault.FindAllByResource("classevivapcto");
            }
            catch (Exception)
            {
                return null;
            }

            if (credentialList.Count > 0)
            {
                if (credentialList.Count == 1)
                {
                    credential = credentialList[0];
                }
                else
                {
                    // When there are multiple usernames,
                    // retrieve the default username. If one doesn't
                    // exist, then display UI to have the user select
                    // a default username.

                }
            }

            return credential;
        }
    }
}
