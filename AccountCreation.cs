using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchAccountCreator
{
    static class AccountCreation
    {
        public static bool GetSignupPage(ref string SignupPage)
        {
            if (HttpRequests.HttpGetRequest("http://de.twitch.tv/user/signup_popup", null, ref SignupPage))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool GetToken(string SignupPage, ref string authenticity_token)
        {
            int index1 = SignupPage.IndexOf("name=\"authenticity_token");
            if (index1 != -1)
            {
                int index2 = SignupPage.IndexOf("value=\"", index1);
                if (index2 != -1)
                {
                    for (int i = index2 + 8; i < SignupPage.Length; i++)
                    {
                        if (SignupPage[i].Equals('"'))
                        {
                            authenticity_token = SignupPage.Substring(index2 + 8, i - index2 - 8);
                            return true;
                        }
                    }
                }

                return false;
            }
            else
                return false;
        }

        public static bool GetRecaptchaChallenge(ref string challenge)
        {
            string response = "";
            Dictionary<string, string> getParams = new Dictionary<string, string>();
            getParams.Add("k", "6Lc6iN0SAAAAAP1C0zsuljLgS8L-w34jVrjMDKB4");
            getParams.Add("ajax", "1");
            getParams.Add("cachestop", "0.19668036280199885");

            if (HttpRequests.HttpGetRequest("http://www.google.com/recaptcha/api/challenge", getParams, ref response))
            {
                if (ParseRecaptchaChallenge(response, ref challenge))
                    return true;
                else
                    return false;
            }
            else
                return false;
        }

        private static bool ParseRecaptchaChallenge(string JSONresponse, ref string challenge)
        {
            int index1 = JSONresponse.IndexOf("challenge : '");
            if (index1 != -1)
            {
                int index2 = JSONresponse.IndexOf("'", index1 + 13);
                if (index2 != -1)
                {
                    challenge = JSONresponse.Substring(index1 + 13, index2 - index1 - 13);

                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        public static bool GetRecaptchaImage(string challenge, ref System.Drawing.Bitmap picture)
        {
            Dictionary<string, string> getParams = new Dictionary<string, string>();
            getParams.Add("c", challenge);

            if(HttpRequests.HttpGetPictureRequest("http://www.google.com/recaptcha/api/image", getParams, ref picture))
                return true;
            else
                return false;
        }

        public static bool SendFormData(string username, string password, string email, string year, string month, string day,
                            string token, string captcha_challenge, string captcha_answer, ref string response)
        {
            Dictionary<string, string> postParams = new Dictionary<string, string>();
            postParams.Add("utf8", "✓");
            postParams.Add("authenticity_token", token);
            postParams.Add("user_facebook_uid", "");
            postParams.Add("user_facebook_access_token", "");
            postParams.Add("username", "");
            postParams.Add("show_facebook_status", "true");
            postParams.Add("user[login]", username);
            postParams.Add("user[password]", password);
            postParams.Add("date[month]", month);
            postParams.Add("date[day]", day);
            postParams.Add("date[year]", year);
            postParams.Add("user[email]", email);
            postParams.Add("recaptcha_challenge_field", captcha_challenge);
            postParams.Add("recaptcha_response_field", captcha_answer);

            if(HttpRequests.HttpPostRequest("http://www.twitch.tv/signup", postParams, ref response))
                return true;
            else
                return false;
        }
    }
}
