﻿using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Ryan_BugTracker.Models;
using Owin.Security.Providers.LinkedIn;

namespace Ryan_BugTracker
{
    public partial class Startup
    {
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers

            //app.UseLinkedInAuthentication(
            //    ClientId = "77ef28x7j7n8o5",
            //    ClientSecret = "kwCU0YrlywJEYxAa");

            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "0000000044187411",
            //    clientSecret: "PqWgqVocMFEiNuwEJewHp99BZrioXmiP");

            //app.UseTwitterAuthentication(
            //   consumerKey: "kwkqhEllX2O5NobQBF387JQyl",
            //   consumerSecret: "zgC4tt0IfMHU66GBRkfvC1PINyo052LsQxKRgqVZQVNMT4ehJe");

            //app.UseFacebookAuthentication(
            //    appId: "993564077425295",
            //    appSecret: "c9585993104b171f162a47c7208a353b");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "894203972552-s62idubqfse842uo631mceqd725sc63i.apps.googleusercontent.com",
            //    ClientSecret = "vLAnN9GeWrLuJLKF4rTGaihk"
            //});           
        }
    }
}