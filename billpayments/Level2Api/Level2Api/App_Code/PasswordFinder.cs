using Org.BouncyCastle.OpenSsl;

internal class PasswordFinder : IPasswordFinder
{
    private string password;

    public PasswordFinder(string Password)
    {
        this.password = Password;
    }

    public char[] GetPassword()
    {
        return password.ToCharArray();
    }
}