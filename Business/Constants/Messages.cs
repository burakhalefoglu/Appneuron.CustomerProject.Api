namespace Business.Constants
{
    /// <summary>
    /// This class was created to get rid of magic strings and write more readable code.
    /// </summary>
    public static class Messages
    {
        internal static string DefaultSuccess => "Success";
        internal static string DefaultError => "Error";

        public static string AppneuronProductNotFound => "Appneuron product not found";
        public static string ClientAlreadyExist => "Client already exist";
        public static string ClientNotFound => "Client not found";
        public static string StringLengthMustBeGreaterThanThree => "String length must be greater than three";
        public static string CouldNotBeVerifyCid => "Could not be verify Cid";
        public static string VerifyCid => "VerifyCid";
        public static string OperationClaimExists => "Operation claim exists";
        public static string AuthorizationsDenied => "Authorizations denied";
        public static string Added => "Added";
        public static string Deleted => "Deleted";
        public static string Updated => "Updated";
        public static string UserNotFound => "User not found";
        public static string PasswordError => "Password error";
        public static string SuccessfulLogin => "Successful login";
        public static string SendMobileCode => "Send mobile code";
        public static string NameAlreadyExist => "Name already exist";
        public static string WrongEmail => "Wrong email";
        public static string CitizenNumber => "CID";
        public static string PasswordEmpty => "Password empty";
        public static string PasswordLength => "Password length";
        public static string PasswordUppercaseLetter => "Password uppercase letter";
        public static string PasswordLowercaseLetter => "Password lowercase letter";
        public static string PasswordDigit => "Password digit";
        public static string PasswordDidNotMatch => "Password didn't match";
        public static string PasswordSpecialCharacter => "Password special character";
        public static string SendPassword => "'Reset password link' sent to email successfully.";
        public static string InvalidCode => "Invalid code";
        public static string SmsServiceNotFound => "Sms service not found";
        public static string TrueButCellPhone => "True but cell phone";
        public static string TokenProviderException => "Token provider exception";
        public static string Unknown => "Unknown";
        public static string NewPassword => "New password";
        public static string ResetPasswordSuccess => "Password changed successfully!";
        public static string ProjectNotFound => "Project not found!";
        public static string CustomerDemographicNotFound => " Customer demographic not found!";
        public static string CustomerDiscountNotFound  => " Customer Discount not found!";
        public static string CustomerNotFound => " Customer not found!";
        public static string CustomerScaleNotFound  => " Customer Scale not found!";
        public static string DiscountNotFound  => " Discount not found!";
        public static string GamePlatformNotFound => " GamePlatform not found!";
    }
}