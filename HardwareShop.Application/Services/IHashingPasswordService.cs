namespace HardwareShop.Application.Services
{
    public interface IHashingPasswordService
    {
        String Hash(String text, Int32 iterations = 1000);
        Boolean IsHashedPasswordSupported(String text);
        Boolean Verify(String password, String hashedPassword);
    }
}
