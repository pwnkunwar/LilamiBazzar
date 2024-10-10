namespace LilamiBazzar.Services.PasswordHashingService
{
    public interface IPasswordHashingService
    {
        public void GeneratePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt);
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
    }
}
