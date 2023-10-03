using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public class EmailValidator : MonoBehaviour
{
    // Found that this regex compiled and worked well for most possible email addresses.
    // NOTE: Microsoft recommends NOT using the regex as the only validation step.
    // NOTE: In another application, we might send a test email to this new 'user.'
    // Pattern Source: https://code-maze.com/csharp-validate-email-address/
    /// <summary>
    /// Regex to validate email address against.
    /// </summary>
    private string MatchEmailPattern = @"^[a-zA-Z0-9.!#$%&'*+,\-.\/=?^_`{|}~]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

    public enum ValidationResponse
    {
        Unknown = -2,
        Timeout = -1,
        Success = 0,
        Empty = 1,
        InvalidFormat = 2,
        TooLong = 3
    }

    /// <summary>
    /// Checks if email is valid against a regex pattern,
    /// with domain normalization and some additional checks for character lengths
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public ValidationResponse Validate(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return ValidationResponse.Empty;

        if (!email.Contains(@"@"))
            return ValidationResponse.InvalidFormat;

        //Normalize Domain
        //https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
        try
        {
            // Normalize the domain
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                  RegexOptions.None, TimeSpan.FromMilliseconds(200));

            // Examines the domain part of the email and normalizes it.
            string DomainMapper(Match match)
            {
                // Use IdnMapping class to convert Unicode domain names.
                var idn = new IdnMapping();

                // Pull out and process domain name (throws ArgumentException on invalid)
                string domainName = idn.GetAscii(match.Groups[2].Value);

                return match.Groups[1].Value + domainName;
            }
        }
        catch (RegexMatchTimeoutException e)
        {
            return ValidationResponse.Timeout;
        }
        catch (ArgumentException e)
        {
            return ValidationResponse.Unknown;
        }

        //Check lengths
        // https://www.mindbaz.com/en/email-deliverability/what-is-the-maximum-size-of-an-mail-address/
        if (email.Length > 64 + 1 + 255)
            return ValidationResponse.TooLong;

        string[] emailSplit = email.Split(@"@");
        if (emailSplit[0].Length > 64 || emailSplit[1].Length > 255)
            return ValidationResponse.TooLong;

        //Finally check against the Regex pattern
        try
        {
            if (Regex.IsMatch(email,
                MatchEmailPattern,
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                return ValidationResponse.Success;
            }
            else
                return ValidationResponse.InvalidFormat;
        }
        catch (RegexMatchTimeoutException)
        {
            return ValidationResponse.Timeout;
        }

        
    }
}
