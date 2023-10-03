using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailValidationTest : MonoBehaviour
{
    [SerializeField]
    private EmailValidator emailValidator;

    private class EmailTest
    {
        public string input;
        public bool isValidNarrow;
        public bool isValidComprehensive;

        public EmailTest (string s, bool v, bool vO)
        {
            input = s;
            isValidNarrow = v;
            isValidComprehensive = vO;
        }
    }

    private List<EmailTest> tests = new List<EmailTest>()
    {
        //True in the narrow sense
        new EmailTest(@"simple@example.com", true, true),
        new EmailTest(@"very.common@example.com", true, true),
        new EmailTest(@"x@example.com", true, true),
        new EmailTest(@"long.email-address-with-hyphens@and.subdomains.example.com", true, true),
        new EmailTest(@"user.name+tag+sorting@example.com", true, true),
        new EmailTest(@"forward/slash@example.com", true, true),
        new EmailTest(@"dash-dash@example.com", true, true),
        new EmailTest(@"name/surname@example.com", true, true),
        new EmailTest(@"admin@example", true, true),
        new EmailTest(@"example@s.example", true, true),
        new EmailTest(@"mailhost!username@example.org", true, true),
        new EmailTest(@"user%example.com@example.org", true, true),

        //Comprehensively true
        new EmailTest("\" \"@example.org", false, true),
        new EmailTest("\"john..doe\"@example.org", false, true),
        new EmailTest("\"very.(),:;<>[]\\\".VERY.\\\"very@\\\\ \\\"very\\\".unusual\"@strange.example.com", false, true),
        new EmailTest(@"postmaster@[123.123.123.123]", false, true),
        new EmailTest(@"postmaster@[IPv6:2001:0db8:85a3:0000:0000:8a2e:0370:7334]", false, true),

        //Unicode - special case
        new EmailTest(@"I❤️CHOCOLATE🍫@example.com", false, true),

        //Never true
        new EmailTest(@"example.com", false, false),
        new EmailTest(@"localpart@", false, false),
        new EmailTest(@"@example.com", false, false),
        new EmailTest(@"back\slash@example.com", false, false),
        new EmailTest(@"abc.example.com", false, false),
        new EmailTest(@"a@b@c@example.com", false, false),
        new EmailTest("a\"b(c)d,e:f; g<h> i[j\\k]l @example.com", false, false),
        new EmailTest("just\"not\"right@example.com",false, false),
        new EmailTest("this is\"not\\allowed@example.com",false, false),
        new EmailTest("this\\ still\\\"not\\\\allowed@example.com", false, false),
        new EmailTest(@"1234567890123456789012345678901234567890123456789012345678901234+x@example.com", false, false),
        new EmailTest(@"i.like.underscores@but_they_are_not_allowed_in_this_part", false, false)
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Run();
    }

    private void Run()
    {
        if (emailValidator == null) { return; }

        bool allPassed = true;

        foreach (EmailTest test in tests)
        {
            if (!(emailValidator.Validate(test.input) == EmailValidator.ValidationResponse.Success))
            {
                allPassed = false;
                Debug.Log("Failed test: " + test.input);
                Debug.Log("Validator Result: " + emailValidator.Validate(test.input));
                Debug.Log("Should be: " + test.isValidNarrow);
            }
        }

        Debug.Log("Results for all tests: " + (allPassed ? "Passed" : "Failed"));
    }


}
