﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Retrieve a temporary FROB from the Flickr service, to be used in redirecting the
        /// user to the Flickr web site for authentication. Only required for desktop authentication.
        /// </summary>
        /// <remarks>
        /// Pass the FROB to the <see cref="AuthCalcUrl"/> method to calculate the url.
        /// </remarks>
        /// <example>
        /// <code>
        /// string frob = flickr.AuthGetFrob();
        /// string url = flickr.AuthCalcUrl(frob, AuthLevel.Read);
        /// 
        /// // redirect the user to the url above and then wait till they have authenticated and return to the app.
        /// 
        /// Auth auth = flickr.AuthGetToken(frob);
        /// 
        /// // then store the auth.Token for later use.
        /// string token = auth.Token;
        /// </code>
        /// </example>
        /// <returns>The FROB.</returns>
        public string AuthGetFrob()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("method", "flickr.auth.getFrob");

            UnknownResponse response = GetResponseNoCache<UnknownResponse>(parameters);

            return response.GetXPathNavigator().SelectSingleNode("frob").Value;
        }

        /// <summary>
        /// Calculates the URL to redirect the user to Flickr web site for
        /// authentication. Used by desktop application. 
        /// See <see cref="AuthGetFrob"/> for example code.
        /// </summary>
        /// <param name="frob">The FROB to be used for authentication.</param>
        /// <param name="authLevel">The <see cref="AuthLevel"/> stating the maximum authentication level your application requires.</param>
        /// <returns>The url to redirect the user to.</returns>
        public string AuthCalcUrl(string frob, AuthLevel authLevel)
        {
            if (_sharedSecret == null) throw new SignatureRequiredException();

            string hash = _sharedSecret + "api_key" + _apiKey + "frob" + frob + "perms" + authLevel.ToString().ToLower();
            hash = Utils.Md5Hash(hash);
            string url = AuthUrl + "?api_key=" + _apiKey + "&perms=" + authLevel.ToString().ToLower() + "&frob=" + frob;
            url += "&api_sig=" + hash;

            return url;
        }

        /// <summary>
        /// Calculates the URL to redirect the user to Flickr web site for
        /// authentication. Used by Web applications. 
        /// See <see cref="AuthGetFrob"/> for example code.
        /// </summary>
        /// <param name="authLevel">The <see cref="AuthLevel"/> stating the maximum authentication level your application requires.</param>
        /// <returns>The url to redirect the user to.</returns>
        public string AuthCalcWebUrl(AuthLevel authLevel)
        {
            CheckApiKey();

            CheckSigned();

            string hash = _sharedSecret + "api_key" + _apiKey + "perms" + authLevel.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
            hash = Utils.Md5Hash(hash);
            string url = AuthUrl + "?api_key=" + _apiKey + "&perms=" + authLevel.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
            url += "&api_sig=" + hash;

            return url;
        }

        /// <summary>
        /// Calculates the URL to redirect the user to Flickr mobile web site for
        /// authentication. Used by Mobile Web applications. 
        /// See <see cref="AuthGetFrob"/> for example code.
        /// </summary>
        /// <param name="authLevel">The <see cref="AuthLevel"/> stating the maximum authentication level your application requires.</param>
        /// <returns>The url to redirect the user to.</returns>
        public string AuthCalcMobileUrl(AuthLevel authLevel)
        {
            CheckApiKey();

            CheckSigned();

            string hash = _sharedSecret + "api_key" + _apiKey + "perms" + authLevel.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
            hash = Utils.Md5Hash(hash);
            string url = AuthUrl.Replace("www.flickr.com", "m.flickr.com") + "?api_key=" + _apiKey + "&perms=" + authLevel.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
            url += "&api_sig=" + hash;

            return url;
        }

        /// <summary>
        /// After the user has authenticated your application on the flickr web site call this 
        /// method with the FROB (either stored from <see cref="AuthGetFrob"/> or returned in the URL
        /// from the Flickr web site) to get the users token.
        /// </summary>
        /// <param name="frob">The string containing the FROB.</param>
        /// <returns>A <see cref="Auth"/> object containing user and token details.</returns>
        public Auth AuthGetToken(string frob)
        {
            CheckSigned();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("method", "flickr.auth.getToken");
            parameters.Add("frob", frob);

            Auth auth = GetResponseNoCache<Auth>(parameters);
            AuthToken = auth.Token;
            return auth;
        }

        /// <summary>
        /// Gets the full token details for a given mini token, entered by the user following a 
        /// web based authentication.
        /// </summary>
        /// <param name="miniToken">The mini token.</param>
        /// <returns>An instance <see cref="Auth"/> class, detailing the user and their full token.</returns>
        public Auth AuthGetFullToken(string miniToken)
        {
            CheckSigned();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("method", "flickr.auth.getFullToken");
            parameters.Add("mini_token", miniToken.Replace("-", ""));

            Auth auth = GetResponseNoCache<Auth>(parameters);
            AuthToken = auth.Token;
            return auth;
        }

        /// <summary>
        /// Checks a authentication token with the flickr service to make
        /// sure it is still valid.
        /// </summary>
        /// <param name="token">The authentication token to check.</param>
        /// <returns>The <see cref="Auth"/> object detailing the user for the token.</returns>
        public Auth AuthCheckToken(string token)
        {
            CheckSigned();

            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("method", "flickr.auth.checkToken");
            parameters.Add("auth_token", token);

            return GetResponseNoCache<Auth>(parameters);
        }

    }
}